using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Domain.Enums
{
    /// <summary>
    /// Representa os tipos de transações financeiras no sistema
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Receita/Entrada de dinheiro
        /// </summary>
        Income = 1,

        /// <summary>
        /// Despesa/Saída de dinheiro
        /// </summary>
        Expense = 2
    }
}