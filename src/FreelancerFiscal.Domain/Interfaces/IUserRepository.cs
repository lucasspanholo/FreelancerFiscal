using FreelancerFiscal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        //Task<User> GetByEmailAsync(string email);
        //Task<User> CreateAsync(User user);
        //Task UpdateAsync(User user);
        // Outros métodos
    }
}
