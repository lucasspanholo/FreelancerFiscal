using FreelancerFiscal.Domain.Entities;
using FreelancerFiscal.Domain.Enums;

namespace FreelancerFiscal.Domain.Entities
{

    public class Invoice
    {
        private readonly List<InvoiceItem> _items = new();
        private readonly List<InvoiceTax> _taxes = new();

        public Invoice(Guid userId, Guid clientId, string invoiceNumber, DateTime issueDate, DateTime dueDate, string description)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ClientId = clientId;
            InvoiceNumber = invoiceNumber;
            IssueDate = issueDate;
            DueDate = dueDate;
            Description = description;
            Status = InvoiceStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        // Propriedades
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid ClientId { get; private set; }
        public string InvoiceNumber { get; private set; }
        public DateTime IssueDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string Description { get; private set; }
        public InvoiceStatus Status { get; private set; }
        public decimal TaxAmount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? SentAt { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }

        // Propriedades de navegação
        public User User { get; private set; }
        public Client Client { get; private set; }
        public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();
        public IReadOnlyCollection<InvoiceTax> Taxes => _taxes.AsReadOnly();

        // Métodos de negócio
        public InvoiceItem AddItem(string description, decimal quantity, decimal unitPrice)
        {
            var item = new InvoiceItem(Id, description, quantity, unitPrice);
            _items.Add(item);
            CalculateTotalAmount();
            return item;
        }

        public void RemoveItem(Guid itemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _items.Remove(item);
                CalculateTotalAmount();
            }
        }

        public void SetTaxes(IEnumerable<InvoiceTax> taxes)
        {
            _taxes.Clear();
            _taxes.AddRange(taxes);
            CalculateTaxAmount();
        }

        public void Issue()
        {
            if (Status == InvoiceStatus.Draft)
            {
                Status = InvoiceStatus.Issued;
                UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException("Apenas notas em rascunho podem ser emitidas.");
            }
        }

        public void MarkAsSent()
        {
            if (Status == InvoiceStatus.Issued)
            {
                Status = InvoiceStatus.Sent;
                SentAt = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException("Apenas notas emitidas podem ser marcadas como enviadas.");
            }
        }

        public void MarkAsPaid(DateTime paymentDate)
        {
            if (Status == InvoiceStatus.Issued || Status == InvoiceStatus.Sent)
            {
                Status = InvoiceStatus.Paid;
                PaidAt = paymentDate;
                UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException("Esta nota não pode ser marcada como paga no status atual.");
            }
        }

        public void Cancel()
        {
            if (Status != InvoiceStatus.Paid)
            {
                Status = InvoiceStatus.Cancelled;
                CancelledAt = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException("Notas pagas não podem ser canceladas.");
            }
        }

        private void CalculateTotalAmount()
        {
            TotalAmount = _items.Sum(i => i.TotalPrice);
            UpdatedAt = DateTime.UtcNow;
        }

        private void CalculateTaxAmount()
        {
            TaxAmount = _taxes.Sum(t => t.Amount);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}