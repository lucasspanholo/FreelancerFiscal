using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Enums
{
    /// <summary>
    /// Representa os papéis/funções dos usuários no sistema
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Usuário comum com acesso padrão
        /// </summary>
        User = 1,

        /// <summary>
        /// Administrador com acesso a configurações avançadas
        /// </summary>
        Administrator = 2,

        /// <summary>
        /// Contador com permissões especiais para análise fiscal
        /// </summary>
        Accountant = 3
    }
}
