using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace FreelancerFiscal.Domain.Enums
    {
        /// <summary>
        /// Representa os possíveis estados de uma nota fiscal no sistema
        /// </summary>
        public enum InvoiceStatus
        {
            /// <summary>
            /// Nota fiscal em rascunho, ainda não emitida oficialmente
            /// </summary>
            Draft = 1,

            /// <summary>
            /// Nota fiscal emitida, mas ainda não enviada ao cliente
            /// </summary>
            Issued = 2,

            /// <summary>
            /// Nota fiscal enviada ao cliente, aguardando pagamento
            /// </summary>
            Sent = 3,

            /// <summary>
            /// Nota fiscal paga pelo cliente
            /// </summary>
            Paid = 4,

            /// <summary>
            /// Nota fiscal com pagamento em atraso
            /// </summary>
            Overdue = 5,

            /// <summary>
            /// Nota fiscal cancelada
            /// </summary>
            Cancelled = 6
        }
    }
