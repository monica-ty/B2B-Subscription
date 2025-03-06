using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2B_Subscription.Core.Entities;
using B2B_Subscription.Core.DTOs;
using B2B_Subscription.Infrastructure.Data.Repositories.Payment;
using Stripe;
using Stripe.Checkout;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using System.Text;

public class CreatePaymentRequest
{
    [Required]
    public string PlanId { get; set; }
}

namespace B2B_Subscription.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentRepository paymentRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _paymentRepository = paymentRepository;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [HttpPost]
        [Route("checkout-session")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            // get price id from plan id
            var token = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Missing authorization token.");
            }
            
            var planServiceUrl = _configuration.GetValue<string>("BaseUrl") + $"/api/Plan/price/{request.PlanId}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, planServiceUrl);
            requestMessage.Headers.Add("Authorization", token);
            var planResponse = await _httpClient.SendAsync(requestMessage);
            if (!planResponse.IsSuccessStatusCode)
            {
                return BadRequest("Plan not found");
            }
            var plan = await planResponse.Content.ReadFromJsonAsync<Core.Entities.Plan>();
            if (plan == null || plan.StripePriceId == null)
            {
                return BadRequest("Plan not registered in Stripe");
            }
            var userId = User.FindFirst(OpenIddict.Abstractions.OpenIddictConstants.Claims.Subject)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            // get stripe customer id from user db
            var userServiceUrl = _configuration.GetValue<string>("BaseUrl") + $"/api/stripe-customer-id/{userId}";
            var userResponse = await _httpClient.GetAsync(userServiceUrl);
            Console.WriteLine("User response: {0}", userResponse.Content);
            if (!userResponse.IsSuccessStatusCode)
            {
                return BadRequest("User not found");
            }
            string? stripeCustomerId = null;
            if (userResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var applicationUserDto = await userResponse.Content.ReadFromJsonAsync<ApplicationUserDto>();
                stripeCustomerId = applicationUserDto?.StripeCustomerId;
            }
            Console.WriteLine("Stripe customer id: {0}", stripeCustomerId);
            
            var domain = "https://localhost:7043";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = plan.StripePriceId,
                        Quantity = 1,
                    }
                },
                Mode = "subscription",
                SuccessUrl = $"{domain}/success",
                CancelUrl = $"{domain}/cancel",
                ClientReferenceId = userId,
                Customer = stripeCustomerId,
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            // update payment db
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = plan.Price,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                StripeSessionId = session.Id,
            };
            await _paymentRepository.CreatePaymentAsync(payment);

            // Response.Headers.Append("Location", session.Url);
            // return StatusCode(StatusCodes.Status303SeeOther);
            return Ok(session);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentsByUserId()
        {
            var userId = User.FindFirst(OpenIddict.Abstractions.OpenIddictConstants.Claims.Subject)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var payments = await _paymentRepository.GetPaymentsByUserIdAsync(userId);
            return Ok(payments);
        }

        // webhook
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhooks()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var secret = _configuration.GetValue<string>("Stripe:WebhookSecret");
            try
            {
                var signatureHeader = Request.Headers["Stripe-Signature"];

                var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, secret);
                
                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded){
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    Console.WriteLine("A successful payment for {0} was made.", paymentIntent?.Amount);
                } else if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted){
                    var checkoutSession = stripeEvent.Data.Object as Session;
                    // get session from stripe api
                    var sessionService = new SessionService();
                    var session = await sessionService.GetAsync(checkoutSession.Id);

                    // get customer id from session
                    var customerId = session.CustomerId;
                    var paymentIntentId = session.PaymentIntentId;
                    var clientReferenceId = session.ClientReferenceId;
                    Console.WriteLine("A checkout session was completed, PaymentIntentId: {0}, CustomerId: {1}, ClientReferenceId: {2}", paymentIntentId, customerId, clientReferenceId);
                    
                    // update customer id in user db if not exists
                    if (clientReferenceId != null)
                    {
                        var getStripeCustomerIdUrl = _configuration.GetValue<string>("BaseUrl") + $"/api/stripe-customer-id/{clientReferenceId}";
                        var getStripeCustomerIdResponse = await _httpClient.GetAsync(getStripeCustomerIdUrl);
                        if (!getStripeCustomerIdResponse.IsSuccessStatusCode)
                        {
                            return BadRequest("User not found");
                        }
                        // stripe customer id not exists
                        if (getStripeCustomerIdResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            var updateStripeCustomerIdUrl = _configuration.GetValue<string>("BaseUrl") + $"/api/stripe-customer-id/{clientReferenceId}";
                            var applicationUserDto = new ApplicationUserDto { StripeCustomerId = customerId };
                            var jsonPayload = JsonConvert.SerializeObject(applicationUserDto);
                            Console.WriteLine($"Sending payload: {jsonPayload}");
                            var updateContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                            var updateStripeCustomerIdResponse = await _httpClient.PutAsync(updateStripeCustomerIdUrl, updateContent);
                            if (!updateStripeCustomerIdResponse.IsSuccessStatusCode)
                            {
                                return BadRequest("Failed to update stripe customer id");
                            }
                        } else if (getStripeCustomerIdResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("Stripe customer id already exists");
                            var existingStripeCustomerId = await getStripeCustomerIdResponse.Content.ReadFromJsonAsync<string>();
                            if (existingStripeCustomerId != customerId)
                            {
                                Console.WriteLine("Stripe customer id mismatch detected for user {0} \n DB Customer ID: {1} \n Stripe Session Customer ID: {2} \n Action required: Investigate whether a new customer was mistakenly created", clientReferenceId, existingStripeCustomerId, customerId);
                            }
                        }
                    }

                    // update payment db
                    try {
                        var payment = await _paymentRepository.GetPaymentByStripeSessionIdAsync(checkoutSession.Id);
                        if (payment != null)
                        {
                            payment.Status = "succeeded";
                            payment.StripePaymentIntentId = paymentIntentId;
                            await _paymentRepository.UpdatePaymentAsync(payment);
                        }
                    } catch (Exception e) {
                        Console.WriteLine("An error occurred: {0}", e.Message);
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine("An Stripe error occurred: {0}", e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: {0}", e.Message);
                return StatusCode(500);
            }
        }
    }


}