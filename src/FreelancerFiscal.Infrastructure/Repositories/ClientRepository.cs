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
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllByUserIdAsync(Guid userId)
        {
            return await _context.Clients
                .Where(c => c.UserId == userId && c.IsActive)
                .ToListAsync();
        }

        public async Task<Client> GetByIdAsync(Guid id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task<Client> CreateAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task UpdateAsync(Client client)
        {
            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var client = await GetByIdAsync(id);
            if (client != null)
            {
                // Soft delete
                client.Deactivate();
                await _context.SaveChangesAsync();
            }
        }
    }
}