using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml.Linq;
using FreelancerFiscal.Application.DTOs;
using FreelancerFiscal.Domain.Entities;
using FreelancerFiscal.Domain.Interfaces;

namespace FreelancerFiscal.Application.Services
{
    public class ClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<ClientDto>> GetAllByUserIdAsync(Guid userId)
        {
            var clients = await _clientRepository.GetAllByUserIdAsync(userId);
            return clients.Select(c => MapToDto(c));
        }

        public async Task<ClientDto> GetByIdAsync(Guid id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            return MapToDto(client);
        }

        // Em ClientService.cs

        public async Task<ClientDto> CreateClientAsync(Guid userId, ClientDto clientDto)
        {
            var client = new Client(
                userId,
                clientDto.Name,
                clientDto.Email,
                clientDto.Document,
                clientDto.Phone,
                clientDto.Address,
                clientDto.City,
                clientDto.State,
                clientDto.ZipCode
            );

            await _clientRepository.CreateAsync(client);

            return MapToDto(client);
        }

        public async Task<ClientDto> UpdateClientAsync(Guid id, ClientDto clientDto)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
                return null;

            client.Update(
                clientDto.Name,
                clientDto.Email,
                clientDto.Phone,
                clientDto.Address,
                clientDto.City,
                clientDto.State,
                clientDto.ZipCode
            );

            await _clientRepository.UpdateAsync(client);

            return MapToDto(client);
        }

        public async Task DeleteClientAsync(Guid id)
        {
            await _clientRepository.DeleteAsync(id);
        }

        private ClientDto MapToDto(Client client)
        {
            if (client == null)
                return null;

            return new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Document = client.Document,
                Phone = client.Phone,
                Address = client.Address,
                City = client.City,
                State = client.State,
                ZipCode = client.ZipCode
            };
        }
    }
}