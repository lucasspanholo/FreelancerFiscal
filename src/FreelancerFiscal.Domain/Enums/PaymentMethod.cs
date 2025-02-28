using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Enums
{
    /// <summary>
    /// Representa os métodos de pagamento disponíveis
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// Transferência bancária tradicional
        /// </summary>
        BankTransfer = 1,

        /// <summary>
        /// Pagamento em dinheiro físico
        /// </summary>
        Cash = 2,

        /// <summary>
        /// Pagamento via PIX
        /// </summary>
        Pix = 3,

        /// <summary>
        /// Pagamento com cartão de crédito
        /// </summary>
        CreditCard = 4,

        /// <summary>
        /// Pagamento com cartão de débito
        /// </summary>
        DebitCard = 5,

        /// <summary>
        /// Pagamento com boleto bancário
        /// </summary>
        BankSlip = 6,

        /// <summary>
        /// Pagamento via PayPal ou similar
        /// </summary>
        DigitalWallet = 7,

        /// <summary>
        /// Outro método de pagamento
        /// </summary>
        Other = 8
    }
}
