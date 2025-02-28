using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Enums
{
    /// <summary>
    /// Representa os tipos de notificações do sistema
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Notificação informativa geral
        /// </summary>
        Info = 1,

        /// <summary>
        /// Alerta sobre vencimento de prazo fiscal
        /// </summary>
        TaxDueDate = 2,

        /// <summary>
        /// Alerta sobre fatura vencida
        /// </summary>
        InvoiceOverdue = 3,

        /// <summary>
        /// Notificação de pagamento recebido
        /// </summary>
        PaymentReceived = 4,

        /// <summary>
        /// Alerta sobre mudança na legislação fiscal
        /// </summary>
        LegislationChange = 5,

        /// <summary>
        /// Lembrete de ação necessária pelo usuário
        /// </summary>
        ActionRequired = 6,

        /// <summary>
        /// Alerta sobre limite de faturamento próximo (MEI, Simples)
        /// </summary>
        RevenueLimitWarning = 7
    }
}
