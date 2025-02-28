using FreelancerFiscal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllByUserIdAsync(Guid userId);
        Task<Client> GetByIdAsync(Guid id);
        Task<Client> CreateAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(Guid id);
        // Outros métodos
    }
}
