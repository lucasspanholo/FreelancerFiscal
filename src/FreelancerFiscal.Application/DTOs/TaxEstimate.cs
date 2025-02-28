namespace FreelancerFiscal.Application.DTOs
{
    public class TaxEstimate
    {
        public string TaxType { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}