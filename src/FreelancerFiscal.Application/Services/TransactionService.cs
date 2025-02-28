using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelancerFiscal.Application.DTOs;
using FreelancerFiscal.Domain.Entities;
using FreelancerFiscal.Domain.Enums;
using FreelancerFiscal.Domain.Interfaces;
using Transaction = FreelancerFiscal.Domain.Entities.Transaction;


namespace FreelancerFiscal.Application.Services
{
    public class TransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IInvoiceRepository invoiceRepository)
        {
            _transactionRepository = transactionRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllByUserIdAsync(Guid userId, DateTime? startDate, DateTime? endDate)
        {
            var transactions = await _transactionRepository.GetAllByUserIdAsync(userId, startDate, endDate);
            return transactions.Select(t => MapToDto(t));
        }

        public async Task<TransactionDto> GetByIdAsync(Guid id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            return MapToDto(transaction);
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createDto)
        {
            Transaction transaction;

            if (createDto.InvoiceId.HasValue)
            {
                var invoice = await _invoiceRepository.GetByIdAsync(createDto.InvoiceId.Value);
                if (invoice == null)
                    throw new ApplicationException("Nota fiscal não encontrada.");

                transaction = new Transaction(
                    createDto.UserId,
                    createDto.InvoiceId.Value,
                    createDto.Type,
                    createDto.Description,
                    createDto.Amount,
                    createDto.Date,
                    createDto.Category
                );
            }
            else
            {
                transaction = new Transaction(
                    createDto.UserId,
                    createDto.Type,
                    createDto.Description,
                    createDto.Amount,
                    createDto.Date,
                    createDto.Category
                );
            }

            if (createDto.IsPaid)
            {
                transaction.MarkAsPaid(createDto.PaymentDate ?? createDto.Date);
            }

            await _transactionRepository.CreateAsync(transaction);

            return MapToDto(transaction);
        }

        public async Task<TransactionDto> UpdateTransactionAsync(Guid id, UpdateTransactionDto updateDto)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);

            if (transaction == null)
                return null;

            // Implementar a lógica de atualização conforme necessário
            // Atualização simplificada para exemplo

            await _transactionRepository.UpdateAsync(transaction);

            return MapToDto(transaction);
        }

        public async Task<TransactionDto> MarkAsPaidAsync(Guid id, DateTime paymentDate)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);

            if (transaction == null)
                return null;

            transaction.MarkAsPaid(paymentDate);
            await _transactionRepository.UpdateAsync(transaction);

            return MapToDto(transaction);
        }

        private TransactionDto MapToDto(Transaction transaction)
        {
            if (transaction == null)
                return null;

            return new TransactionDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                InvoiceId = transaction.InvoiceId,
                Type = transaction.Type.ToString(),
                Description = transaction.Description,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Category = transaction.Category,
                IsPaid = transaction.IsPaid,
                PaymentDate = transaction.PaymentDate,
                InvoiceNumber = transaction.Invoice?.InvoiceNumber
            };
        }
    }

    // Adicionar essas classes DTO se ainda não existirem
    public class CreateTransactionDto
    {
        public Guid UserId { get; set; }
        public Guid? InvoiceId { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    public class UpdateTransactionDto
    {
        public Guid UserId { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? InvoiceId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string InvoiceNumber { get; set; }
    }
}