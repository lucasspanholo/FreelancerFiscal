using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelancerFiscal.Domain.Entities;
using FreelancerFiscal.Domain.Enums;
using FreelancerFiscal.Application.DTOs;

namespace FreelancerFiscal.Application.Interfaces
{
    public interface ITaxCalculator
    {
        /// <summary>
        /// Calcula os impostos aplicáveis para uma nota fiscal
        /// </summary>
        /// <param name="invoice">A nota fiscal para calcular os impostos</param>
        /// <param name="client">O cliente destinatário da nota fiscal</param>
        /// <returns>Uma coleção de objetos InvoiceTax representando os impostos calculados</returns>
        Task<IEnumerable<InvoiceTax>> CalculateTaxesAsync(Invoice invoice, Client client);

        /// <summary>
        /// Calcula os impostos mensais a serem pagos pelo usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="year">Ano fiscal</param>
        /// <param name="month">Mês fiscal</param>
        /// <returns>Uma coleção de impostos mensais a serem pagos</returns>
        Task<IEnumerable<MonthlyTax>> CalculateMonthlyTaxesAsync(Guid userId, int year, int month);

        /// <summary>
        /// Calcula uma previsão de impostos com base nos valores fornecidos
        /// </summary>
        /// <param name="amount">Valor base para cálculo</param>
        /// <param name="taxRegime">Regime tributário do usuário</param>
        /// <returns>Uma coleção de impostos estimados</returns>
        Task<IEnumerable<TaxEstimate>> EstimateTaxesAsync(decimal amount, TaxRegime taxRegime);
    }
}