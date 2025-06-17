using BLL;
using DAL;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace UI.Views
{
    public partial class ResponderTickets : Window
    {
        private Utilizador _utilizador;

        public ResponderTickets(Utilizador utilizador)
        {
            InitializeComponent();
            _utilizador = utilizador;

            // Carrega os tickets inicialmente sem filtros (todos)
            CarregarTicketsParaResponder();
        }

        /// <summary>
        /// Carrega os tickets para responder, aplicando filtros e ordenação, e popula o WrapPanel com cartões.
        /// </summary>
        private void CarregarTicketsParaResponder(
            string tipoFiltro = "Todos",
            string prioridadeFiltro = "Todos",
            string estadoTicketFiltro = "Todos",
            string estadoAtendimentoFiltro = "Todos",
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string ordenarPor = null)
        {
            // Obter os tickets do banco aplicando filtros
            var tickets = TicketDAL.ObterTicketsPorAtender(
                tipoFiltro,
                prioridadeFiltro,
                estadoTicketFiltro,
                estadoAtendimentoFiltro,
                _utilizador.Id,
                dataInicio,
                dataFim
            );

            // Ordena conforme critério selecionado
            switch (ordenarPor)
            {
                case "ID Ascendente":
                    tickets = tickets.OrderBy(t => t.Id).ToList();
                    break;
                case "ID Descendente":
                    tickets = tickets.OrderByDescending(t => t.Id).ToList();
                    break;
                case "Data Ascendente":
                    tickets = tickets.OrderBy(t => t.DataCriacao).ToList();
                    break;
                case "Data Descendente":
                    tickets = tickets.OrderByDescending(t => t.DataCriacao).ToList();
                    break;
                default:
                    tickets = tickets.OrderByDescending(t => t.DataCriacao).ToList();
                    break;
            }

            // Limpa o painel antes de adicionar os cartões
            TicketsPanel.Children.Clear();

            // Cria e adiciona um cartão para cada ticket
            foreach (var ticket in tickets)
            {
                var card = CriarCartaoTicket(ticket);
                TicketsPanel.Children.Add(card);
            }
        }

        /// <summary>
        /// Cria um cartão visual para representar um ticket.
        /// </summary>
        private UIElement CriarCartaoTicket(Ticket ticket)
        {
            // Cria uma borda para o cartão com sombra e arredondamento
            var border = new Border
            {
                Width = 240,
                Margin = new Thickness(10),
                Padding = new Thickness(10),
                Background = Brushes.White,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Gray,
                    ShadowDepth = 2,
                    Opacity = 0.3
                },
                Cursor = Cursors.Hand
            };

            // Pilha vertical para organizar as informações
            var stack = new StackPanel();

            // Texto do ID do ticket em negrito
            var txtId = new TextBlock
            {
                Text = $"Ticket #{ticket.Id}",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            };

            // Tipo do ticket
            var txtTipo = new TextBlock
            {
                Text = $"Tipo: {ticket.Tipo}",
                Margin = new Thickness(0, 0, 0, 3)
            };

            // Prioridade
            var txtPrioridade = new TextBlock
            {
                Text = $"Prioridade: {ticket.Prioridade}",
                Margin = new Thickness(0, 0, 0, 3)
            };

            // Descrição resumida (até 80 caracteres)
            var descricao = ticket.Descricao.Length > 80 ? ticket.Descricao.Substring(0, 80) + "..." : ticket.Descricao;
            var txtDescricao = new TextBlock
            {
                Text = descricao,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };

            // Data de criação formatada
            var txtData = new TextBlock
            {
                Text = $"Criado em: {ticket.DataCriacao:dd/MM/yyyy}",
                FontStyle = FontStyles.Italic,
                FontSize = 11
            };

            // Adiciona todos os elementos ao StackPanel
            stack.Children.Add(txtId);
            stack.Children.Add(txtTipo);
            stack.Children.Add(txtPrioridade);
            stack.Children.Add(txtDescricao);
            stack.Children.Add(txtData);

            // Coloca o StackPanel dentro da borda
            border.Child = stack;

            // Evento para abrir janela de resposta ao clicar no cartão
            border.MouseLeftButtonUp += (s, e) =>
            {
                var janelaResposta = new ResponderTicketDetalhes(ticket, _utilizador);
                janelaResposta.Owner = this;
                janelaResposta.ShowDialog();

                // Atualiza a lista de tickets após fechar a janela de resposta
                BtnAplicarFiltros_Click(null, null);
            };

            return border;
        }

        /// <summary>
        /// Evento do botão para aplicar os filtros e atualizar a lista de tickets.
        /// </summary>
        private void BtnAplicarFiltros_Click(object sender, RoutedEventArgs e)
        {
            string tipoFiltro = (cbFiltroTipo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";
            string prioridadeFiltro = (cbFiltroPrioridade.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";
            string estadoTicketFiltro = (cbFiltroEstadoTicket.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";
            string estadoAtendimentoFiltro = (cbFiltroEstadoAtendimento.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";
            string ordenarPor = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString();

            DateTime? dataInicio = dpDataInicio.SelectedDate;
            DateTime? dataFim = dpDataFim.SelectedDate;

            CarregarTicketsParaResponder(
                tipoFiltro,
                prioridadeFiltro,
                estadoTicketFiltro,
                estadoAtendimentoFiltro,
                dataInicio,
                dataFim,
                ordenarPor);
        }

        /// <summary>
        /// Mostra a seção de responder tickets, ocultando o menu inicial.
        /// </summary>
        private void BtnResponderTickets_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            ResponderSection.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Abre a janela de mapas.
        /// </summary>
        private void BtnVerMapas_Click(object sender, RoutedEventArgs e)
        {
            var janelaMapas = new Mapas(_utilizador.Id);
            janelaMapas.Owner = this;
            janelaMapas.Show();
        }

        /// <summary>
        /// Volta para o menu principal, ocultando a seção de resposta.
        /// </summary>
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            ResponderSection.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Faz logout, fecha esta janela e abre a janela de login.
        /// </summary>
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}