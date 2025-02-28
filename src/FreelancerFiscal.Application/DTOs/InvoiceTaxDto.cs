using System;

namespace FreelancerFiscal.Application.DTOs
{
    public class InvoiceTaxDto
    {
        public Guid Id { get; set; }
        public string TaxType { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}