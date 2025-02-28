using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FreelancerFiscal.Domain.Entities;
using FreelancerFiscal.Domain.Interfaces;
using FreelancerFiscal.Infrastructure.Data;

namespace FreelancerFiscal.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetAllByUserIdAsync(Guid userId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Items)
                .Include(i => i.Taxes)
                .Where(i => i.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(i => i.IssueDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(i => i.IssueDate <= endDate.Value);

            return await query.OrderByDescending(i => i.IssueDate).ToListAsync();
        }

        public async Task<Invoice> GetByIdAsync(Guid id)
        {
            return await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Items)
                .Include(i => i.Taxes)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Entry(invoice).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Invoice>> GetPendingPaymentAsync(Guid userId)
        {
            return await _context.Invoices
                .Include(i => i.Client)
                .Where(i => i.UserId == userId &&
                           (i.Status == Domain.Enums.InvoiceStatus.Issued ||
                            i.Status == Domain.Enums.InvoiceStatus.Sent))
                .OrderBy(i => i.DueDate)
                .ToListAsync();
        }

        public async Task<string> GetLastInvoiceNumberAsync(Guid userId)
        {
            var lastInvoice = await _context.Invoices
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            return lastInvoice?.InvoiceNumber;
        }
    }
}