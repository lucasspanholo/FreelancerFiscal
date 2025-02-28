using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction = FreelancerFiscal.Domain.Entities.Transaction;

namespace FreelancerFiscal.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllByUserIdAsync(Guid userId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<Transaction>> GetByPeriodAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<Transaction> GetByIdAsync(Guid id);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        // Outros métodos
    }
}
