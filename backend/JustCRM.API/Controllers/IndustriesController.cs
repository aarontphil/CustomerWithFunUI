using JustCRM.API.DTOs;
using JustCRM.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace JustCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndustriesController : ControllerBase
    {
        private readonly IIndustryService _industryService;

        public IndustriesController(IIndustryService industryService)
        {
            _industryService = industryService;
        }

        // GET: api/industries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IndustryDto>>> GetIndustries()
        {
            var industries = await _industryService.GetAllIndustriesAsync();
            return Ok(industries);
        }

        // GET: api/industries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IndustryDto>> GetIndustry(long id)
        {
            var industry = await _industryService.GetIndustryByIdAsync(id);
            if (industry == null)
                return NotFound(new { message = $"Industry with Id {id} was not found." });

            return Ok(industry);
        }
    }
}
