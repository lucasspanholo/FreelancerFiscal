using System;
using System.Threading.Tasks;
using FreelancerFiscal.Domain.Entities;

namespace FreelancerFiscal.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendPaymentConfirmationAsync(Invoice invoice);
        Task SendInvoiceEmailAsync(Invoice invoice, string email, string message);
        Task SendTaxDueDateNotificationAsync(Guid userId, string taxType, DateTime dueDate, decimal amount);
        Task SendInvoiceOverdueNotificationAsync(Invoice invoice);
    }
}