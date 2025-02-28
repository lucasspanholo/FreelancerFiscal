using System;

namespace FreelancerFiscal.Application.DTOs
{
    public class MonthlyTax
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string TaxType { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}