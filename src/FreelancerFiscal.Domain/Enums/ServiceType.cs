using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Enums
{
    /// <summary>
    /// Representa os tipos de serviço para categorização e impostos
    /// </summary>
    public enum ServiceType
    {
        /// <summary>
        /// Desenvolvimento de software e aplicativos
        /// </summary>
        SoftwareDevelopment = 1,

        /// <summary>
        /// Design gráfico, web design, UI/UX
        /// </summary>
        Design = 2,

        /// <summary>
        /// Marketing digital, SEO, redes sociais
        /// </summary>
        DigitalMarketing = 3,

        /// <summary>
        /// Consultoria em TI, negócios, etc.
        /// </summary>
        Consulting = 4,

        /// <summary>
        /// Criação de conteúdo, redação, tradução
        /// </summary>
        ContentCreation = 5,

        /// <summary>
        /// Serviços de contabilidade e financeiros
        /// </summary>
        Accounting = 6,

        /// <summary>
        /// Serviços jurídicos
        /// </summary>
        Legal = 7,

        /// <summary>
        /// Serviços educacionais, treinamentos
        /// </summary>
        Education = 8,

        /// <summary>
        /// Serviços de fotografia e vídeo
        /// </summary>
        Photography = 9,

        /// <summary>
        /// Serviços de saúde
        /// </summary>
        Health = 10,

        /// <summary>
        /// Outros serviços não classificados
        /// </summary>
        Other = 11
    }
}
