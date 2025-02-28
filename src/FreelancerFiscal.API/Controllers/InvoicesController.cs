using FreelancerFiscal.Application.Services;
using Microsoft.AspNetCore.Mvc;
using FreelancerFiscal.Application.DTOs; // Adicionar esta diretiva

namespace FreelancerFiscal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoicesController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceDto>> Create(CreateInvoiceDto createInvoiceDto)
        {
            try
            {
                var invoice = await _invoiceService.CreateInvoiceAsync(createInvoiceDto);
                return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetById(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        // Outros endpoints
    }
}
