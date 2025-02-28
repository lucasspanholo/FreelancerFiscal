using System;
using System.Threading.Tasks;
using FreelancerFiscal.Application.DTOs;
using FreelancerFiscal.Domain.Entities;
using FreelancerFiscal.Domain.Interfaces;

namespace FreelancerFiscal.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return MapToDto(user);
        }

        // Método faltante que mapeia a entidade User para UserDto
        private UserDto MapToDto(User user)
        {
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Document = user.Document,
                Phone = user.Phone,
                TaxRegime = user.TaxRegime.ToString()
            };
        }

        // Outros métodos e mapeamentos
        public async Task<UserDto> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            // Validate if user already exists
            if (await _userRepository.GetByEmailAsync(registerUserDto.Email) != null)
            {
                throw new Exception("User with this email already exists");
            }

            // Hash the password
            var hashedPassword = _passwordHasher.HashPassword(registerUserDto.Password);

            // Create user entity
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = registerUserDto.Email,
                Name = registerUserDto.Name,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            // Save user to database
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Map to DTO and return
            return _mapper.Map<UserDto>(user);
        }
    }
}