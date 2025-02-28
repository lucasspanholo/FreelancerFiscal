using System;
using System.Collections.Generic;

namespace FreelancerFiscal.Application.DTOs
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal TaxAmount { get; set; }
        public List<InvoiceItemDto> Items { get; set; }
        public List<InvoiceTaxDto> Taxes { get; set; }
    }
}