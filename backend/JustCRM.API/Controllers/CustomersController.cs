using JustCRM.API.DTOs;
using JustCRM.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JustCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customers?search=&industryId=&sortBy=&isDescending=&pageNumber=&pageSize=
        [HttpGet]
        public async Task<ActionResult<PagedResult<CustomerListDto>>> GetCustomers(
            [FromQuery] string? search,
            [FromQuery] long? industryId,
            [FromQuery] string? sortBy,
            [FromQuery] bool isDescending = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var result = await _customerService.GetPagedCustomersAsync(
                search, industryId, sortBy, isDescending, pageNumber, pageSize);

            return Ok(result);
        }

        // GET: api/customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(long id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = $"Customer with Id {id} was not found." });

            return Ok(customer);
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var customer = await _customerService.CreateCustomerAsync(dto);
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/customers/5
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer(long id, [FromBody] CustomerCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var customer = await _customerService.UpdateCustomerAsync(id, dto);
                if (customer == null)
                    return NotFound(new { message = $"Customer with Id {id} was not found." });

                return Ok(customer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/customers/5 (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteCustomer(long id)
        {
            var result = await _customerService.SoftDeleteCustomerAsync(id);
            if (!result)
                return NotFound(new { message = $"Customer with Id {id} was not found." });

            return NoContent();
        }



        // POST: api/customers/5/documents
        [HttpPost("{id}/documents")]
        public async Task<ActionResult<CustomerDocumentDto>> UploadDocument(long id, [FromForm] string documentType, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file was uploaded." });

            if (string.IsNullOrWhiteSpace(documentType))
                return BadRequest(new { message = "Document type is required." });

            try
            {
                var doc = await _customerService.UploadCustomerDocumentAsync(id, documentType, file);
                if (doc == null)
                    return NotFound(new { message = $"Customer with Id {id} was not found." });

                return Ok(doc);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/customers/5/documents/10
        [HttpDelete("{id}/documents/{documentId}")]
        public async Task<IActionResult> SoftDeleteDocument(long id, long documentId)
        {
            var result = await _customerService.SoftDeleteCustomerDocumentAsync(id, documentId);
            if (!result)
                return NotFound(new { message = $"Document with Id {documentId} for Customer with Id {id} was not found or is already inactive." });

            return NoContent();
        }
    }
}
