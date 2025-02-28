using System;

namespace FreelancerFiscal.Application.DTOs
{
    public class InvoiceItemDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}