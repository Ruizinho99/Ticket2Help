using BLL;
using DAL;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        private void CarregarTicketsParaResponder(
            string tipoFiltro = "Todos",
            string prioridadeFiltro = "Todos",
            string estadoTicketFiltro = "Todos",
            string estadoAtendimentoFiltro = "Todos",
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string ordenarPor = null)
        {
            var tickets = TicketDAL.ObterTicketsPorAtender(
                tipoFiltro,
                prioridadeFiltro,
                estadoTicketFiltro,
                estadoAtendimentoFiltro,
                _utilizador.Id,
                dataInicio,
                dataFim
            );

            // Ordenar conforme seleção
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

            dgTicketsResponder.ItemsSource = tickets; // Seu DataGrid de tickets para responder
        }

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

        private void BtnResponderTickets_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            ResponderSection.Visibility = Visibility.Visible;
        }

        private void BtnVerMapas_Click(object sender, RoutedEventArgs e)
        {
            var janelaMapas = new Mapas(_utilizador.Id);
            janelaMapas.Owner = this;
            janelaMapas.Show();
        }



        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            ResponderSection.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Visible;
        }
        private void dgTicketsResponder_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var ticketSelecionado = dgTicketsResponder.SelectedItem as Ticket;
            if (ticketSelecionado == null)
            {
                MessageBox.Show("Por favor, selecione um ticket para responder.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Abre a janela de detalhes para responder o ticket
            var janelaResposta = new ResponderTicketDetalhes(ticketSelecionado, _utilizador);
            janelaResposta.Owner = this;  // opcional para manter janela pai
            janelaResposta.ShowDialog();

            // Após fechar a janela de resposta, atualiza a lista de tickets
            BtnAplicarFiltros_Click(null, null);
        }


        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
