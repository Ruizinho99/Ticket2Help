using System;
using System.Collections.Generic;
using System.Linq;
using UI.ViewModels;
using Xunit;

namespace Ticket2Help.Tests.Tests
{
    /// <summary>
    /// Classe de testes responsável por validar o correto funcionamento da filtragem de tickets no ViewModel ResponderTicketsViewModel.
    /// </summary>
    public class ResponderTicketsViewModelTests
    {
        /// <summary>
        /// Testa se a aplicação dos filtros no ViewModel retorna os tickets corretamente filtrados.
        /// </summary>
        [Fact]
        public void AplicarFiltros_DeveFiltrarTicketsCorretamente()
        {
            // Arrange

            /// <summary>
            /// Cria um mock de um técnico logado.
            /// </summary>
            var tecnicoMock = new Utilizador { Id = 1 };

            /// <summary>
            /// Lista mock de tickets para simular o repositório de dados.
            /// </summary>
            var ticketsMock = new List<Ticket>
            {
                new Ticket { Tipo = "Hardware", Prioridade = "Alta", EstadoTicket = "porAtender", EstadoAtendimento = "aberto" },
                new Ticket { Tipo = "Software", Prioridade = "Baixa", EstadoTicket = "emAtendimento", EstadoAtendimento = "naoResolvido" },
                new Ticket { Tipo = "Hardware", Prioridade = "Média", EstadoTicket = "atendido", EstadoAtendimento = "resolvido" },
                new Ticket { Tipo = "Software", Prioridade = "Alta", EstadoTicket = "porAtender", EstadoAtendimento = "aberto" }
            };

            /// <summary>
            /// Substitui a função do DAL para retornar a lista mock com possibilidade de filtragem.
            /// </summary>
            TicketDAL.ObterTicketsPorAtenderFunc = (
                string tipoFiltro,
                string prioridadeFiltro,
                string estadoTicketFiltro,
                string estadoAtendimentoFiltro,
                int tecnicoId,
                DateTime? dataInicio,
                DateTime? dataFim) =>
            {
                IEnumerable<Ticket> resultado = ticketsMock;

                // Aplica os filtros conforme os parâmetros recebidos
                if (!string.IsNullOrEmpty(tipoFiltro) && tipoFiltro != "Todos")
                    resultado = resultado.Where(t => t.Tipo == tipoFiltro);

                if (!string.IsNullOrEmpty(prioridadeFiltro) && prioridadeFiltro != "Todos")
                    resultado = resultado.Where(t => t.Prioridade == prioridadeFiltro);

                if (!string.IsNullOrEmpty(estadoTicketFiltro) && estadoTicketFiltro != "Todos")
                    resultado = resultado.Where(t => t.EstadoTicket == estadoTicketFiltro);

                if (!string.IsNullOrEmpty(estadoAtendimentoFiltro) && estadoAtendimentoFiltro != "Todos")
                    resultado = resultado.Where(t => t.EstadoAtendimento == estadoAtendimentoFiltro);

                return resultado.ToList();
            };

            /// <summary>
            /// Cria o ViewModel que será testado.
            /// </summary>
            var vm = new ResponderTicketsViewModel(tecnicoMock);

            // Act & Assert

            /// <summary>
            /// 1) Valida que sem filtros todos os tickets são carregados.
            /// </summary>
            Assert.Equal(4, vm.Tickets.Count);

            /// <summary>
            /// 2) Filtra por Tipo "Hardware" e valida que todos os tickets retornados são do tipo especificado.
            /// </summary>
            vm.FiltroTipo = "Hardware";
            Assert.All(vm.Tickets, t => Assert.Equal("Hardware", t.Tipo));
            Assert.Equal(2, vm.Tickets.Count);

            /// <summary>
            /// 3) Filtra por Prioridade "Alta" e valida que todos os tickets retornados têm a prioridade especificada.
            /// </summary>
            vm.FiltroTipo = "Todos";
            vm.FiltroPrioridade = "Alta";
            Assert.All(vm.Tickets, t => Assert.Equal("Alta", t.Prioridade));
            Assert.Equal(2, vm.Tickets.Count);

            /// <summary>
            /// 4) Filtra por EstadoTicket "porAtender" e valida que todos os tickets retornados têm o estado especificado.
            /// </summary>
            vm.FiltroPrioridade = "Todos";
            vm.FiltroEstadoTicket = "porAtender";
            Assert.All(vm.Tickets, t => Assert.Equal("porAtender", t.EstadoTicket));
            Assert.Equal(2, vm.Tickets.Count);

            /// <summary>
            /// 5) Filtra por EstadoAtendimento "aberto" e valida que todos os tickets retornados têm o estado especificado.
            /// </summary>
            vm.FiltroEstadoTicket = "Todos";
            vm.FiltroEstadoAtendimento = "aberto";
            Assert.All(vm.Tickets, t => Assert.Equal("aberto", t.EstadoAtendimento));
            Assert.Equal(2, vm.Tickets.Count);

            /// <summary>
            /// 6) Combina filtros: Tipo "Software" e EstadoAtendimento "aberto" e valida que os tickets atendem a ambos os filtros.
            /// </summary>
            vm.FiltroTipo = "Software";
            Assert.All(vm.Tickets, t =>
            {
                Assert.Equal("Software", t.Tipo);
                Assert.Equal("aberto", t.EstadoAtendimento);
            });
            Assert.Single(vm.Tickets);
        }
    }
}

