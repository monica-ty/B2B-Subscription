using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2B_Subscription.Core.Entities;
using B2B_Subscription.Core.DTOs;
using B2B_Subscription.Infrastructure.Data.Repositories.Subscription;


namespace B2B_Subscription.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController : ControllerBase
    {
        private readonly IPlanRepository _planRepository;

        public PlanController(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlans()
        {
            var plans = await _planRepository.GetAllPlansAsync();
            var response = plans.Select(p => new
            {
                p.Id,
                p.Name,
                p.BillingCycle,
                p.Price,
                p.MaxLicenses
            });
            return Ok(response);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(Guid id)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id);
            if (plan == null)
                return NotFound();

            var response = new
            {
                plan.Id,
                plan.Name,
                plan.BillingCycle,
                plan.Price,
                plan.MaxLicenses
            };
            return Ok(response);
        }

        [HttpGet("price/{id}")]
        public async Task<IActionResult> GetPlanPrice(Guid id)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id);
            var response = new
            {
                plan.StripePriceId
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlan(CreatePlanDto createDto)
        {
            var plan = new Plan
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                StripePriceId = createDto.StripePriceId,
                BillingCycle = createDto.BillingCycle,
                Price = createDto.Price,
                MaxLicenses = createDto.MaxLicenses
            };
            var createdPlan = await _planRepository.CreatePlanAsync(plan);
            return CreatedAtAction(nameof(GetPlanById), new { id = createdPlan.Id }, createdPlan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlan(Guid id, UpdatePlanDto updateDto)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id);
            if (plan == null)
                return NotFound();
            plan.Name = updateDto.Name ?? plan.Name;
            plan.StripePriceId = updateDto.StripePriceId ?? plan.StripePriceId;
            plan.BillingCycle = updateDto.BillingCycle ?? plan.BillingCycle;
            plan.Price = updateDto.Price ?? plan.Price;
            plan.MaxLicenses = updateDto.MaxLicenses ?? plan.MaxLicenses;
            var updatedPlan = await _planRepository.UpdatePlanAsync(plan);
            return Ok(updatedPlan);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(Guid id)
        {
            var result = await _planRepository.DeletePlanAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}