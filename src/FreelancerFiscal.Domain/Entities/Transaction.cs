using System;
using FreelancerFiscal.Domain.Enums;

namespace FreelancerFiscal.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid? InvoiceId { get; private set; }
        public TransactionType Type { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public string Category { get; private set; }
        public bool IsPaid { get; private set; }
        public DateTime? PaymentDate { get; private set; }

        // Propriedades de navegação
        public User User { get; private set; }
        public Invoice Invoice { get; private set; }

        // Construtor privado para EF Core
        private Transaction() { }

        public Transaction(Guid userId, TransactionType type, string description,
                           decimal amount, DateTime date, string category)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Type = type;
            Description = description;
            Amount = amount;
            Date = date;
            Category = category;
            IsPaid = false;
        }

        public Transaction(Guid userId, Guid invoiceId, TransactionType type,
                          string description, decimal amount, DateTime date, string category)
            : this(userId, type, description, amount, date, category)
        {
            InvoiceId = invoiceId;
        }

        public void MarkAsPaid(DateTime paymentDate)
        {
            IsPaid = true;
            PaymentDate = paymentDate;
        }


    }
}