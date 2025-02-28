using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Enums
{
    /// <summary>
    /// Representa os diferentes regimes tributários disponíveis no Brasil
    /// </summary>
    public enum TaxRegime
    {
        /// <summary>
        /// Microempreendedor Individual - faturamento anual até R$ 81.000,00
        /// </summary>
        MEI = 1,

        /// <summary>
        /// Simples Nacional - faturamento anual até R$ 4.800.000,00
        /// </summary>
        SimplesNacional = 2,

        /// <summary>
        /// Lucro Presumido - para empresas com faturamento anual até R$ 78.000.000,00
        /// </summary>
        LucroPresumido = 3,

        /// <summary>
        /// Lucro Real - obrigatório para empresas com faturamento acima de R$ 78.000.000,00
        /// </summary>
        LucroReal = 4,

        /// <summary>
        /// Profissional Autônomo - pessoa física sem CNPJ
        /// </summary>
        Autonomo = 5
    }
}