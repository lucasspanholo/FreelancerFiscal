using System;

namespace FreelancerFiscal.Domain.Entities
{
    public class InvoiceTax
    {
        public Guid Id { get; private set; }
        public Guid InvoiceId { get; private set; }
        public string TaxType { get; private set; }
        public decimal Rate { get; private set; }
        public decimal Amount { get; private set; }

        // Propriedade de navegação
        public Invoice Invoice { get; private set; }

        // Construtor privado para EF Core
        private InvoiceTax() { }

        // Construtor público
        public InvoiceTax(Guid invoiceId, string taxType, decimal rate, decimal amount)
        {
            Id = Guid.NewGuid();
            InvoiceId = invoiceId;
            TaxType = taxType;
            Rate = rate;
            Amount = amount;
        }
    }
}