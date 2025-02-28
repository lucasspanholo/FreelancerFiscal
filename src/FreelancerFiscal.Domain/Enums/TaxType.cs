using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Enums
{
    /// <summary>
    /// Representa os diferentes tipos de impostos do sistema tributário brasileiro
    /// </summary>
    public enum TaxType
    {
        /// <summary>
        /// Imposto Sobre Serviços - municipal
        /// </summary>
        ISS = 1,

        /// <summary>
        /// Imposto de Renda Pessoa Jurídica
        /// </summary>
        IRPJ = 2,

        /// <summary>
        /// Contribuição Social sobre o Lucro Líquido
        /// </summary>
        CSLL = 3,

        /// <summary>
        /// Programa de Integração Social
        /// </summary>
        PIS = 4,

        /// <summary>
        /// Contribuição para o Financiamento da Seguridade Social
        /// </summary>
        COFINS = 5,

        /// <summary>
        /// Imposto de Renda Pessoa Física (para autônomos)
        /// </summary>
        IRPF = 6,

        /// <summary>
        /// Contribuição Previdenciária (INSS)
        /// </summary>
        INSS = 7,

        /// <summary>
        /// Documento de Arrecadação do Simples Nacional
        /// </summary>
        DAS = 8,

        /// <summary>
        /// Documento de Arrecadação do Simples Nacional para MEI
        /// </summary>
        DASMEI = 9,

        /// <summary>
        /// Imposto sobre Circulação de Mercadorias e Serviços (para alguns serviços)
        /// </summary>
        ICMS = 10,

        /// <summary>
        /// Outros impostos ou contribuições
        /// </summary>
        Other = 11
    }
}