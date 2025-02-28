using System;
using System.Collections.Generic;

namespace FreelancerFiscal.Application.DTOs
{
    public class UpdateInvoiceDto
    {
        public Guid UserId { get; set; }
        public Guid ClientId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        public List<UpdateInvoiceItemDto> Items { get; set; }
    }

    public class UpdateInvoiceItemDto
    {
        public Guid? Id { get; set; } // Null para novos itens
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}