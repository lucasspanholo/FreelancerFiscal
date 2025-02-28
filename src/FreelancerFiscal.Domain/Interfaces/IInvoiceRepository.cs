using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelancerFiscal.Domain.Entities;

namespace FreelancerFiscal.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetAllByUserIdAsync(Guid userId, DateTime? startDate, DateTime? endDate);
        Task<Invoice> GetByIdAsync(Guid id);
        Task<Invoice> CreateAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task<IEnumerable<Invoice>> GetPendingPaymentAsync(Guid userId);

        // Método faltante
        Task<string> GetLastInvoiceNumberAsync(Guid userId);
    }
}