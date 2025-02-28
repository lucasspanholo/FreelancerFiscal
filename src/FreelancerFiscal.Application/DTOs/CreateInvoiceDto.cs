using System;
using System.Collections.Generic;

namespace FreelancerFiscal.Application.DTOs
{
    public class CreateInvoiceDto
    {
        public Guid UserId { get; set; }
        public Guid ClientId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        public List<CreateInvoiceItemDto> Items { get; set; }
    }

    public class CreateInvoiceItemDto
    {
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}