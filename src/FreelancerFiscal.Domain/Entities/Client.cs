using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Document { get; private set; } // CPF ou CNPJ
        public string Phone { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsActive { get; private set; }
        public User User { get; private set; }
        public ICollection<Invoice> Invoices { get; private set; }

        // Construtor privado para o EF Core
        private Client()
        {
            Invoices = new List<Invoice>();
        }

        // Construtor público para criar um novo cliente
        public Client(Guid userId, string name, string email, string document, string phone,
                      string address, string city, string state, string zipCode)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Email = email;
            Document = document;
            Phone = phone;
            Address = address;
            City = city;
            State = state;
            ZipCode = zipCode;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Invoices = new List<Invoice>();
        }

        // Método para atualizar o cliente
        public void Update(string name, string email, string phone, string address,
                          string city, string state, string zipCode)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
            City = city;
            State = state;
            ZipCode = zipCode;
            UpdatedAt = DateTime.UtcNow;
        }

        // Método para desativar o cliente
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // Método para ativar o cliente
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
