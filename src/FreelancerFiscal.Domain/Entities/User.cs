using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using FreelancerFiscal.Domain.Enums;

namespace FreelancerFiscal.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Document { get; private set; } // CPF ou CNPJ
        public string Phone { get; private set; }
        public string PasswordHash { get; private set; }
        public TaxRegime TaxRegime { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsActive { get; private set; }
        public ICollection<Client> Clients { get; private set; }
        public ICollection<Invoice> Invoices { get; private set; }
        public ICollection<Transaction> Transactions { get; private set; }

        // Métodos construtores e de negócio
    }
}