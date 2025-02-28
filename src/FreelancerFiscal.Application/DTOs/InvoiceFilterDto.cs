using System;

namespace FreelancerFiscal.Application.DTOs
{
    public class InvoiceFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public Guid? ClientId { get; set; }
    }
}