using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Classe de testes para o ViewModel EstatisticasTicketsViewModel.
/// </summary>
public class EstatisticasTicketsViewModelTests
{
    /// <summary>
    /// Testa se o método CarregarEstatisticas calcula corretamente os totais, percentuais e médias.
    /// </summary>
    [Fact]
    public void CarregarEstatisticas_DeveCalcularValoresCorretamente()
    {
        /// <summary>
        /// Lista mock de tickets para simular o cenário de teste.
        /// </summary>
        var mockTickets = new List<Ticket>
        {
            new Ticket
            {
                Tipo = "Hardware",
                EstadoTicket = "atendido",
                EstadoAtendimento = "resolvido",
                DataAtendimento = DateTime.Now.AddHours(-2),
                DataConclusao = DateTime.Now
            },
            new Ticket
            {
                Tipo = "Software",
                EstadoTicket = "porAtender",
                EstadoAtendimento = "aberto",
                DataAtendimento = null,
                DataConclusao = null
            },
            new Ticket
            {
                Tipo = "Hardware",
                EstadoTicket = "emAtendimento",
                EstadoAtendimento = "naoResolvido",
                DataAtendimento = DateTime.Now.AddHours(-1),
                DataConclusao = null
            },
            new Ticket
            {
                Tipo = "Software",
                EstadoTicket = "atendido",
                EstadoAtendimento = "resolvido",
                DataAtendimento = DateTime.Now.AddHours(-3),
                DataConclusao = DateTime.Now.AddHours(-1)
            }
        };

        /// <summary>
        /// Mock da função do DAL para retornar os tickets definidos.
        /// </summary>
        TicketDAL.ObterTicketsEstatisticasFunc = (tipoFiltro, prioridadeFiltro, estadoTicketFiltro, estadoAtendimentoFiltro, tecnicoLogadoId, dataInicio, dataFim) =>
        {
            return mockTickets;
        };

        /// <summary>
        /// Instancia o ViewModel com os filtros padrão (Todos) e ID do técnico 1.
        /// </summary>
        var viewModel = new EstatisticasTicketsViewModel(
            tecnicoId: 1,
            tipoFiltro: "Todos",
            prioridadeFiltro: "Todos",
            estadoTicketFiltro: "Todos",
            estadoAtendimentoFiltro: "Todos"
        );

        // Validação dos totais calculados
        Assert.Equal(4, viewModel.TotalTickets); ///< Deve haver 4 tickets no total
        Assert.Equal(2, viewModel.TicketsAtendidos); ///< Dois tickets com EstadoTicket == "atendido"
        Assert.Equal(2, viewModel.TicketsResolvidos); ///< Dois tickets com EstadoAtendimento == "resolvido"
        Assert.Equal(1, viewModel.TicketsNaoResolvidos); ///< Um ticket com EstadoAtendimento == "naoResolvido"

        // Validação da média de tempo de atendimento por tipo - Hardware
        var hardwareMedia = viewModel.MediaTempoAtendimentoPorTipoLista.FirstOrDefault(t => t.Tipo == "Hardware");
        Assert.NotNull(hardwareMedia); ///< Deve existir um cálculo para Hardware
        Assert.True(hardwareMedia.MediaTempo.TotalHours > 0); ///< Média de tempo para Hardware deve ser maior que zero

        // Validação da média de tempo de atendimento por tipo - Software
        var softwareMedia = viewModel.MediaTempoAtendimentoPorTipoLista.FirstOrDefault(t => t.Tipo == "Software");
        Assert.NotNull(softwareMedia); ///< Deve existir um cálculo para Software
        Assert.True(softwareMedia.MediaTempo.TotalHours > 0); ///< Média de tempo para Software deve ser maior que zero

        // Validação dos percentuais calculados
        Assert.Equal("50.00%", viewModel.PercentualRespondidos); ///< 2 atendidos de 4 tickets (50%)
        Assert.Equal("100.00%", viewModel.PercentualResolvidosVsAtendidos); ///< 2 resolvidos de 2 atendidos (100%)
        Assert.Equal("66.67%", viewModel.PercentualResolvidosVsNaoResolvidos); ///< 2 resolvidos de 3 (66.67%)
    }
}
