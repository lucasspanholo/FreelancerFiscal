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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllByUserIdAsync(Guid userId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Transactions
                .Include(t => t.Invoice)
                .Where(t => t.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            return await query.OrderByDescending(t => t.Date).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByPeriodAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<Transaction> GetByIdAsync(Guid id)
        {
            return await _context.Transactions
                .Include(t => t.Invoice)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}