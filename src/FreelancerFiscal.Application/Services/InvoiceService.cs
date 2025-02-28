
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelancerFiscal.Application.DTOs;
using FreelancerFiscal.Application.Interfaces;
using FreelancerFiscal.Domain.Entities;
using FreelancerFiscal.Domain.Interfaces;
using FreelancerFiscal.Domain.Enums;

namespace FreelancerFiscal.Application.Services
{
    public class InvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ITaxCalculator _taxCalculator;
        private readonly INotificationService _notificationService;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IClientRepository clientRepository,
            ITaxCalculator taxCalculator,
            INotificationService notificationService)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _taxCalculator = taxCalculator;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllByUserIdAsync(Guid userId, InvoiceFilterDto filter)
        {
            var invoices = await _invoiceRepository.GetAllByUserIdAsync(
                userId,
                filter.StartDate,
                filter.EndDate);

            if (!string.IsNullOrEmpty(filter.Status))
            {
                if (Enum.TryParse<InvoiceStatus>(filter.Status, out var status))
                {
                    invoices = invoices.Where(i => i.Status == status);
                }
            }

            return invoices.Select(i => MapToDto(i));
        }

        public async Task<InvoiceDto> GetByIdAsync(Guid id, Guid userId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            if (invoice == null || invoice.UserId != userId)
            {
                return null;
            }

            return MapToDto(invoice);
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto)
        {
            // Validar cliente
            var client = await _clientRepository.GetByIdAsync(createInvoiceDto.ClientId);
            if (client == null)
            {
                throw new ApplicationException("Cliente não encontrado.");
            }

            if (client.UserId != createInvoiceDto.UserId)
            {
                throw new ApplicationException("Cliente não pertence a este usuário.");
            }

            // Gerar número da nota
            var invoiceNumber = await GenerateInvoiceNumberAsync(createInvoiceDto.UserId);

            // Criar entidade de nota fiscal
            var invoice = new Invoice(
                createInvoiceDto.UserId,
                createInvoiceDto.ClientId,
                invoiceNumber,
                createInvoiceDto.IssueDate,
                createInvoiceDto.DueDate,
                createInvoiceDto.Description);

            // Adicionar itens
            foreach (var item in createInvoiceDto.Items)
            {
                invoice.AddItem(item.Description, item.Quantity, item.UnitPrice);
            }

            // Calcular impostos baseado no regime tributário do usuário
            var taxResult = await _taxCalculator.CalculateTaxesAsync(invoice, client);
            invoice.SetTaxes(taxResult);

            // Persistir a nota
            await _invoiceRepository.CreateAsync(invoice);

            return MapToDto(invoice);
        }

        public async Task<InvoiceDto> UpdateInvoiceAsync(Guid id, UpdateInvoiceDto updateInvoiceDto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            if (invoice == null)
            {
                throw new ApplicationException("Nota fiscal não encontrada.");
            }

            if (invoice.UserId != updateInvoiceDto.UserId)
            {
                throw new ApplicationException("Nota fiscal não pertence a este usuário.");
            }

            if (invoice.Status != InvoiceStatus.Draft)
            {
                throw new ApplicationException("Apenas notas em rascunho podem ser editadas.");
            }

            var client = await _clientRepository.GetByIdAsync(updateInvoiceDto.ClientId);
            if (client == null)
            {
                throw new ApplicationException("Cliente não encontrado.");
            }

            if (client.UserId != updateInvoiceDto.UserId)
            {
                throw new ApplicationException("Cliente não pertence a este usuário.");
            }

            // Atualizar a nota (implementação fictícia, seria necessário adicionar métodos na entidade)
            // Na prática, seria melhor deletar itens existentes e criar novos

            // Recalcular impostos
            var taxResult = await _taxCalculator.CalculateTaxesAsync(invoice, client);
            invoice.SetTaxes(taxResult);

            // Persistir alterações
            await _invoiceRepository.UpdateAsync(invoice);

            return MapToDto(invoice);
        }

        public async Task<InvoiceDto> IssueInvoiceAsync(Guid id, Guid userId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            if (invoice == null)
            {
                throw new ApplicationException("Nota fiscal não encontrada.");
            }

            if (invoice.UserId != userId)
            {
                throw new ApplicationException("Nota fiscal não pertence a este usuário.");
            }

            invoice.Issue();
            await _invoiceRepository.UpdateAsync(invoice);

            return MapToDto(invoice);
        }

        public async Task<InvoiceDto> MarkAsPaidAsync(Guid id, Guid userId, DateTime paymentDate)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            if (invoice == null)
            {
                throw new ApplicationException("Nota fiscal não encontrada.");
            }

            if (invoice.UserId != userId)
            {
                throw new ApplicationException("Nota fiscal não pertence a este usuário.");
            }

            invoice.MarkAsPaid(paymentDate);
            await _invoiceRepository.UpdateAsync(invoice);

            // Notificar usuário
            await _notificationService.SendPaymentConfirmationAsync(invoice);

            return MapToDto(invoice);
        }

        public async Task<InvoiceDto> CancelInvoiceAsync(Guid id, Guid userId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            if (invoice == null)
            {
                throw new ApplicationException("Nota fiscal não encontrada.");
            }

            if (invoice.UserId != userId)
            {
                throw new ApplicationException("Nota fiscal não pertence a este usuário.");
            }

            invoice.Cancel();
            await _invoiceRepository.UpdateAsync(invoice);

            return MapToDto(invoice);
        }

        public async Task SendInvoiceByEmailAsync(Guid id, Guid userId, string email, string message)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            if (invoice == null)
            {
                throw new ApplicationException("Nota fiscal não encontrada.");
            }

            if (invoice.UserId != userId)
            {
                throw new ApplicationException("Nota fiscal não pertence a este usuário.");
            }

            if (invoice.Status == InvoiceStatus.Draft)
            {
                throw new ApplicationException("Não é possível enviar uma nota fiscal em rascunho.");
            }

            // Enviar email com a nota fiscal
            await _notificationService.SendInvoiceEmailAsync(invoice, email, message);

            // Marcar como enviada se ainda não estiver
            if (invoice.Status == InvoiceStatus.Issued)
            {
                invoice.MarkAsSent();
                await _invoiceRepository.UpdateAsync(invoice);
            }
        }

        private async Task<string> GenerateInvoiceNumberAsync(Guid userId)
        {
            // Implementação simplificada
            // Na prática, isso dependeria do regime tributário e da prefeitura
            var lastInvoice = await _invoiceRepository.GetLastInvoiceNumberAsync(userId);

            if (string.IsNullOrEmpty(lastInvoice))
            {
                return "00001";
            }

            if (int.TryParse(lastInvoice, out int lastNumber))
            {
                return (lastNumber + 1).ToString("D5");
            }

            return "00001";
        }

        private InvoiceDto MapToDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                Id = invoice.Id,
                ClientId = invoice.ClientId,
                ClientName = invoice.Client?.Name,
                InvoiceNumber = invoice.InvoiceNumber,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                TotalAmount = invoice.TotalAmount,
                Description = invoice.Description,
                Status = invoice.Status.ToString(),
                TaxAmount = invoice.TaxAmount,
                Items = invoice.Items.Select(i => new InvoiceItemDto
                {
                    Id = i.Id,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList(),
                Taxes = invoice.Taxes.Select(t => new InvoiceTaxDto
                {
                    Id = t.Id,
                    TaxType = t.TaxType,
                    Rate = t.Rate,
                    Amount = t.Amount
                }).ToList()
            };
        }
        public async Task<InvoiceDto> GetInvoiceByIdAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return null;

            return MapToDto(invoice);
        }
    }
}