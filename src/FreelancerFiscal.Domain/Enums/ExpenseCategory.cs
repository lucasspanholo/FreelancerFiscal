using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Enums
{
    /// <summary>
    /// Representa as categorias de despesas para classificação
    /// </summary>
    public enum ExpenseCategory
    {
        /// <summary>
        /// Aluguel de imóvel comercial ou coworking
        /// </summary>
        Rent = 1,

        /// <summary>
        /// Serviços públicos como água, luz, internet, telefone
        /// </summary>
        Utilities = 2,

        /// <summary>
        /// Software, hardware e material de escritório
        /// </summary>
        Office = 3,

        /// <summary>
        /// Serviços de marketing, publicidade e divulgação
        /// </summary>
        Marketing = 4,

        /// <summary>
        /// Assinaturas de software e serviços recorrentes
        /// </summary>
        Subscriptions = 5,

        /// <summary>
        /// Compra de equipamentos
        /// </summary>
        Equipment = 6,

        /// <summary>
        /// Transporte, passagens e combustível
        /// </summary>
        Transportation = 7,

        /// <summary>
        /// Refeições e alimentação relacionadas ao trabalho
        /// </summary>
        Meals = 8,

        /// <summary>
        /// Serviços profissionais como contabilidade e advocacia
        /// </summary>
        ProfessionalServices = 9,

        /// <summary>
        /// Impostos e taxas
        /// </summary>
        Taxes = 10,

        /// <summary>
        /// Pagamentos a fornecedores e terceirizados
        /// </summary>
        Suppliers = 11,

        /// <summary>
        /// Treinamentos, cursos e educação
        /// </summary>
        Training = 12,

        /// <summary>
        /// Seguros diversos
        /// </summary>
        Insurance = 13,

        /// <summary>
        /// Outras despesas não classificadas
        /// </summary>
        Other = 14
    }
}