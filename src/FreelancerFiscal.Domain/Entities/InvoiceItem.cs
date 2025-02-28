using System;

namespace FreelancerFiscal.Domain.Entities
{
    public class InvoiceItem
    {
        public Guid Id { get; private set; }
        public Guid InvoiceId { get; private set; }
        public string Description { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice { get; private set; }

        // Propriedade de navegação
        public Invoice Invoice { get; private set; }

        // Construtor privado para EF Core
        private InvoiceItem() { }

        // Construtor público
        public InvoiceItem(Guid invoiceId, string description, decimal quantity, decimal unitPrice)
        {
            Id = Guid.NewGuid();
            InvoiceId = invoiceId;
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = Quantity * UnitPrice;
        }

        public void Update(string description, decimal quantity, decimal unitPrice)
        {
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CalculateTotalPrice();
        }
    }
}