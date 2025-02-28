using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelancerFiscal.Application.DTOs;
using FreelancerFiscal.Application.Interfaces;
using FreelancerFiscal.Domain.Entities;
using FreelancerFiscal.Domain.Enums;
using FreelancerFiscal.Domain.Interfaces;

namespace FreelancerFiscal.Infrastructure.External
{
    public class BrazilianTaxCalculator : ITaxCalculator
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUserRepository _userRepository;

        public BrazilianTaxCalculator(
            IInvoiceRepository invoiceRepository,
            IUserRepository userRepository)
        {
            _invoiceRepository = invoiceRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<InvoiceTax>> CalculateTaxesAsync(Invoice invoice, Client client)
        {
            // Buscar o usuário para obter seu regime tributário
            var user = await _userRepository.GetByIdAsync(invoice.UserId);
            if (user == null)
            {
                throw new ApplicationException("Usuário não encontrado.");
            }

            var taxes = new List<InvoiceTax>();

            // Calcular com base no regime tributário
            switch (user.TaxRegime)
            {
                case TaxRegime.MEI:
                    // MEI tem isenção de impostos federais até o limite
                    taxes.AddRange(CalculateMEITaxes(invoice));
                    break;

                case TaxRegime.SimplesNacional:
                    // Simples Nacional tem alíquota única que varia por faixa de faturamento
                    taxes.AddRange(await CalculateSimplesTaxesAsync(invoice, user.Id));
                    break;

                case TaxRegime.LucroPresumido:
                    // Lucro Presumido tem PIS, COFINS, IRPJ e CSLL
                    taxes.AddRange(CalculateLucroPresumidoTaxes(invoice));
                    break;

                case TaxRegime.LucroReal:
                    // Lucro Real tem PIS, COFINS, IRPJ e CSLL com alíquotas específicas
                    taxes.AddRange(CalculateLucroRealTaxes(invoice));
                    break;

                case TaxRegime.Autonomo:
                    // Profissional Autônomo tem IRPF e ISS
                    taxes.AddRange(CalculateAutonomoTaxes(invoice));
                    break;

                default:
                    throw new ApplicationException("Regime tributário não suportado.");
            }

            // Calcular ISS
            // O ISS é municipal e varia conforme a cidade
            var issTax = CalculateISS(invoice, client.City);
            if (issTax != null)
            {
                taxes.Add(issTax);
            }

            return taxes;
        }

        public async Task<IEnumerable<MonthlyTax>> CalculateMonthlyTaxesAsync(Guid userId, int year, int month)
        {
            // Buscar o usuário para obter seu regime tributário
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException("Usuário não encontrado.");
            }

            // Buscar todas as notas fiscais do mês/ano
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var invoices = await _invoiceRepository.GetAllByUserIdAsync(userId, startDate, endDate);
            var paidInvoices = invoices.Where(i => i.Status == InvoiceStatus.Paid || i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.Sent);

            // Calcular o total faturado no mês
            decimal monthlyRevenue = paidInvoices.Sum(i => i.TotalAmount);

            var monthlyTaxes = new List<MonthlyTax>();

            // Calcular os impostos mensais com base no regime tributário
            switch (user.TaxRegime)
            {
                case TaxRegime.MEI:
                    monthlyTaxes.Add(new MonthlyTax
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Year = year,
                        Month = month,
                        TaxType = "DAS-MEI",
                        Amount = 65.00m, // Valor fixo para 2023 (exemplo)
                        DueDate = new DateTime(year, month, 20).AddMonths(1),
                        Description = "Documento de Arrecadação do Simples Nacional para MEI"
                    });
                    break;

                case TaxRegime.SimplesNacional:
                    // Alíquota do Simples varia conforme a faixa de faturamento
                    decimal simplesRate = await GetSimplesRateAsync(userId, monthlyRevenue);
                    decimal simplesAmount = monthlyRevenue * simplesRate;

                    monthlyTaxes.Add(new MonthlyTax
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Year = year,
                        Month = month,
                        TaxType = "DAS",
                        Amount = simplesAmount,
                        DueDate = new DateTime(year, month, 20).AddMonths(1),
                        Description = "Documento de Arrecadação do Simples Nacional"
                    });
                    break;

                case TaxRegime.LucroPresumido:
                    // No Lucro Presumido, os impostos são calculados trimestralmente (IRPJ e CSLL)
                    // e mensalmente (PIS e COFINS)
                    decimal pisRate = 0.0065m; // 0,65%
                    decimal cofinsRate = 0.03m; // 3%

                    decimal pisAmount = monthlyRevenue * pisRate;
                    decimal cofinsAmount = monthlyRevenue * cofinsRate;

                    monthlyTaxes.Add(new MonthlyTax
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Year = year,
                        Month = month,
                        TaxType = "PIS",
                        Amount = pisAmount,
                        DueDate = new DateTime(year, month, 25).AddMonths(1),
                        Description = "Programa de Integração Social"
                    });

                    monthlyTaxes.Add(new MonthlyTax
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Year = year,
                        Month = month,
                        TaxType = "COFINS",
                        Amount = cofinsAmount,
                        DueDate = new DateTime(year, month, 25).AddMonths(1),
                        Description = "Contribuição para o Financiamento da Seguridade Social"
                    });

                    // Se for o último mês do trimestre, calcular IRPJ e CSLL
                    if (month == 3 || month == 6 || month == 9 || month == 12)
                    {
                        // Buscar o faturamento do trimestre
                        int startMonth = month - 2;
                        DateTime quarterStartDate = new DateTime(year, startMonth, 1);

                        var quarterInvoices = await _invoiceRepository.GetAllByUserIdAsync(
                            userId, quarterStartDate, endDate);

                        var paidQuarterInvoices = quarterInvoices.Where(
                            i => i.Status == InvoiceStatus.Paid || i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.Sent);

                        decimal quarterRevenue = paidQuarterInvoices.Sum(i => i.TotalAmount);

                        // Presumido para serviços: 32% da receita bruta
                        decimal presumedProfit = quarterRevenue * 0.32m;

                        // IRPJ: 15% sobre o lucro presumido
                        decimal irpjAmount = presumedProfit * 0.15m;

                        // Adicional de IRPJ: 10% sobre o valor do lucro presumido que exceder R$ 60.000,00 no trimestre
                        if (presumedProfit > 60000)
                        {
                            irpjAmount += (presumedProfit - 60000) * 0.10m;
                        }

                        // CSLL: 9% sobre o lucro presumido
                        decimal csllAmount = presumedProfit * 0.09m;

                        monthlyTaxes.Add(new MonthlyTax
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            Year = year,
                            Month = month,
                            TaxType = "IRPJ",
                            Amount = irpjAmount,
                            DueDate = new DateTime(year, month, 30).AddMonths(1),
                            Description = "Imposto de Renda Pessoa Jurídica - Trimestral"
                        });

                        monthlyTaxes.Add(new MonthlyTax
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            Year = year,
                            Month = month,
                            TaxType = "CSLL",
                            Amount = csllAmount,
                            DueDate = new DateTime(year, month, 30).AddMonths(1),
                            Description = "Contribuição Social sobre o Lucro Líquido - Trimestral"
                        });
                    }
                    break;

                case TaxRegime.LucroReal:
                    // Implementação simplificada para Lucro Real
                    // Na prática, o cálculo é muito mais complexo e depende da contabilidade completa
                    break;

                case TaxRegime.Autonomo:
                    // Para autônomos, calcular o IRPF mensal (carnê-leão) e o INSS
                    decimal baseCalculoIR = monthlyRevenue * 0.90m; // Desconto simplificado de 10%

                    // Tabela progressiva do IR (valores de 2023)
                    decimal irpfAmount = CalculateIRPF(baseCalculoIR);

                    // INSS - tabela progressiva
                    decimal inssAmount = CalculateINSS(monthlyRevenue);

                    monthlyTaxes.Add(new MonthlyTax
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Year = year,
                        Month = month,
                        TaxType = "IRPF",
                        Amount = irpfAmount,
                        DueDate = new DateTime(year, month, 20).AddMonths(1),
                        Description = "Imposto de Renda Pessoa Física (Carnê-Leão)"
                    });

                    monthlyTaxes.Add(new MonthlyTax
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Year = year,
                        Month = month,
                        TaxType = "INSS",
                        Amount = inssAmount,
                        DueDate = new DateTime(year, month, 15).AddMonths(1),
                        Description = "Contribuição Previdenciária"
                    });
                    break;
            }

            return monthlyTaxes;
        }

        public async Task<IEnumerable<TaxEstimate>> EstimateTaxesAsync(decimal amount, TaxRegime taxRegime)
        {
            var estimates = new List<TaxEstimate>();

            // Declarar variáveis comuns fora do switch
            decimal issRate = 0.05m; // Taxa média municipal para ISS

            switch (taxRegime)
            {
                case TaxRegime.MEI:
                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "DAS-MEI",
                        Rate = 0,
                        Amount = 65.00m,
                        Description = "Valor fixo mensal independente do faturamento"
                    });

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "ISS",
                        Rate = 0,
                        Amount = 0,
                        Description = "Incluído no DAS-MEI"
                    });
                    break;

                case TaxRegime.SimplesNacional:
                    // Simplificação - alíquota média do Anexo III (serviços)
                    decimal simplesRate = 0.06m; // 6%

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "Simples Nacional",
                        Rate = simplesRate,
                        Amount = amount * simplesRate,
                        Description = "Alíquota única que inclui diversos impostos"
                    });
                    break;

                case TaxRegime.LucroPresumido:
                    decimal pisRate = 0.0065m;
                    decimal cofinsRate = 0.03m;
                    decimal irpjBaseRate = 0.32m; // Base de cálculo para serviços
                    decimal irpjRate = 0.15m;
                    decimal csllBaseRate = 0.32m;
                    decimal csllRate = 0.09m;

                    decimal presumedProfit = amount * irpjBaseRate;

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "PIS",
                        Rate = pisRate,
                        Amount = amount * pisRate,
                        Description = "Programa de Integração Social"
                    });

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "COFINS",
                        Rate = cofinsRate,
                        Amount = amount * cofinsRate,
                        Description = "Contribuição para o Financiamento da Seguridade Social"
                    });

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "IRPJ",
                        Rate = irpjRate * irpjBaseRate,
                        Amount = presumedProfit * irpjRate,
                        Description = "Imposto de Renda Pessoa Jurídica"
                    });

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "CSLL",
                        Rate = csllRate * csllBaseRate,
                        Amount = presumedProfit * csllRate,
                        Description = "Contribuição Social sobre o Lucro Líquido"
                    });

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "ISS",
                        Rate = issRate,
                        Amount = amount * issRate,
                        Description = "Imposto Sobre Serviços"
                    });
                    break;

                case TaxRegime.Autonomo:
                    decimal inssRate = 0.11m; // Simplificação
                    decimal irpfRate = 0.275m; // Alíquota média simplificada

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "INSS",
                        Rate = inssRate,
                        Amount = amount * inssRate,
                        Description = "Contribuição Previdenciária"
                    });

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "IRPF",
                        Rate = irpfRate,
                        Amount = amount * irpfRate,
                        Description = "Imposto de Renda Pessoa Física"
                    });

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "ISS",
                        Rate = issRate,
                        Amount = amount * issRate,
                        Description = "Imposto Sobre Serviços"
                    });
                    break;

                case TaxRegime.LucroReal:
                    // Para o Lucro Real, adicionamos um case explícito
                    // (que estava faltando anteriormente)
                    decimal pisRealRate = 0.0165m;
                    decimal cofinsRealRate = 0.076m;

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "PIS",
                        Rate = pisRealRate,
                        Amount = amount * pisRealRate,
                        Description = "Programa de Integração Social (Lucro Real)"
                    });

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "COFINS",
                        Rate = cofinsRealRate,
                        Amount = amount * cofinsRealRate,
                        Description = "Contribuição para o Financiamento da Seguridade Social (Lucro Real)"
                    });

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "IRPJ/CSLL",
                        Rate = 0.15m,
                        Amount = amount * 0.15m,
                        Description = "Estimativa simplificada para IRPJ/CSLL no Lucro Real"
                    });

                    estimates.Add(new TaxEstimate
                    {
                        TaxType = "ISS",
                        Rate = issRate,
                        Amount = amount * issRate,
                        Description = "Imposto Sobre Serviços"
                    });
                    break;

                default:
                    throw new ApplicationException("Regime tributário não suportado.");
            }

            return estimates;
        }

        #region Métodos Auxiliares

        private IEnumerable<InvoiceTax> CalculateMEITaxes(Invoice invoice)
        {
            // MEI não tem impostos diretamente na nota, apenas o ISS (que já está incluído no DAS)
            return new List<InvoiceTax>();
        }

        private async Task<IEnumerable<InvoiceTax>> CalculateSimplesTaxesAsync(Invoice invoice, Guid userId)
        {
            var taxes = new List<InvoiceTax>();

            // Obter a alíquota do Simples Nacional
            decimal simplesRate = await GetSimplesRateAsync(userId, invoice.TotalAmount);

            // Criar o imposto do Simples Nacional
            taxes.Add(new InvoiceTax(
                invoice.Id,
                "Simples Nacional",
                simplesRate,
                invoice.TotalAmount * simplesRate
            ));

            return taxes;
        }

        private IEnumerable<InvoiceTax> CalculateLucroPresumidoTaxes(Invoice invoice)
        {
            var taxes = new List<InvoiceTax>();

            // PIS (0,65%)
            taxes.Add(new InvoiceTax(
                invoice.Id,
                "PIS",
                0.0065m,
                invoice.TotalAmount * 0.0065m
            ));

            // COFINS (3%)
            taxes.Add(new InvoiceTax(
                invoice.Id,
                "COFINS",
                0.03m,
                invoice.TotalAmount * 0.03m
            ));

            // IRPJ (calculado trimestralmente, mas retido na fonte em alguns casos)
            // Presunção de lucro para serviços: 32%
            decimal presumedProfit = invoice.TotalAmount * 0.32m;

            // IRPJ: 15% sobre o lucro presumido
            taxes.Add(new InvoiceTax(
                invoice.Id,
                "IRPJ",
                0.15m * 0.32m, // Taxa efetiva: 4,8%
                presumedProfit * 0.15m
            ));

            // CSLL: 9% sobre o lucro presumido
            taxes.Add(new InvoiceTax(
                invoice.Id,
                "CSLL",
                0.09m * 0.32m, // Taxa efetiva: 2,88%
                presumedProfit * 0.09m
            ));

            return taxes;
        }

        private IEnumerable<InvoiceTax> CalculateLucroRealTaxes(Invoice invoice)
        {
            // Simplificação - na prática, o Lucro Real requer contabilidade completa
            var taxes = new List<InvoiceTax>();

            // PIS (1,65%)
            taxes.Add(new InvoiceTax(
                invoice.Id,
                "PIS",
                0.0165m,
                invoice.TotalAmount * 0.0165m
            ));

            // COFINS (7,6%)
            taxes.Add(new InvoiceTax(
                invoice.Id,
                "COFINS",
                0.076m,
                invoice.TotalAmount * 0.076m
            ));

            return taxes;
        }

        private IEnumerable<InvoiceTax> CalculateAutonomoTaxes(Invoice invoice)
        {
            var taxes = new List<InvoiceTax>();

            // IRPF (vai depender da faixa de renda)
            // Simplificação: alíquota média de 15%
            taxes.Add(new InvoiceTax(
                invoice.Id,
                "IRPF",
                0.15m,
                invoice.TotalAmount * 0.15m
            ));

            // INSS (alíquota varia conforme a faixa)
            // Simplificação: 11%
            taxes.Add(new InvoiceTax(
                invoice.Id,
                "INSS",
                0.11m,
                invoice.TotalAmount * 0.11m
            ));

            return taxes;
        }

        private InvoiceTax CalculateISS(Invoice invoice, string city)
        {
            // O ISS varia de acordo com o município, normalmente entre 2% e 5%
            // Simplificação: 5%
            decimal issRate = GetISSRateForCity(city);

            return new InvoiceTax(
                invoice.Id,
                "ISS",
                issRate,
                invoice.TotalAmount * issRate
            );
        }

        private decimal GetISSRateForCity(string city)
        {
            // Na prática, isso seria uma tabela ou API com as alíquotas por cidade
            // Simplificação: retorna 5%
            return 0.05m;
        }

        private async Task<decimal> GetSimplesRateAsync(Guid userId, decimal amount)
        {
            // Na prática, isso envolveria calcular o faturamento acumulado nos últimos 12 meses
            // e consultar a tabela do Simples Nacional (Anexo III para a maioria dos serviços)
            // Simplificação: retorna 6%
            return 0.06m;
        }

        private decimal CalculateIRPF(decimal baseCalculoIR)
        {
            // Tabela progressiva do IRPF (valores simplificados)
            if (baseCalculoIR <= 1903.98m)
                return 0;

            if (baseCalculoIR <= 2826.65m)
                return (baseCalculoIR * 0.075m) - 142.80m;

            if (baseCalculoIR <= 3751.05m)
                return (baseCalculoIR * 0.15m) - 354.80m;

            if (baseCalculoIR <= 4664.68m)
                return (baseCalculoIR * 0.225m) - 636.13m;

            return (baseCalculoIR * 0.275m) - 869.36m;
        }

        private decimal CalculateINSS(decimal baseCalculo)
        {
            // Tabela progressiva do INSS (valores simplificados)
            // Na prática, são aplicadas alíquotas por faixa
            // Simplificação:
            if (baseCalculo <= 1100)
                return baseCalculo * 0.075m;

            if (baseCalculo <= 2203.48m)
                return baseCalculo * 0.09m;

            if (baseCalculo <= 3305.22m)
                return baseCalculo * 0.12m;

            if (baseCalculo <= 6433.57m)
                return baseCalculo * 0.14m;

            return 900.70m; // Teto do INSS (2023)
        }

        #endregion
    }
}