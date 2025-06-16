using BLL;
using System;
using System.Windows;
using System.Windows.Controls;

namespace UI.Views
{
    public partial class ResponderTickets : Window
    {
        private int tecnicoLogadoId;
        public AdminTicketsViewModel ViewModel { get; set; }
        private Utilizador _utilizador;
        public EstatisticasTicketsViewModel EstatisticasVM { get; set; }

        public ResponderTickets(Utilizador utilizador)
        {
            InitializeComponent();

            ViewModel = new AdminTicketsViewModel(utilizador);

            // Ligação do action para abrir a janela de responder ticket
            ViewModel.AbrirJanelaResponderTicket = AbrirJanelaResponderTicket;

            EstatisticasVM = new EstatisticasTicketsViewModel(
                utilizador.Id, "Todos", "Todos", "Todos", "Todos");

            DataContext = ViewModel;
        }

        private void AbrirJanelaResponderTicket(Ticket ticket)
        {
            if (ticket == null)
                return;

            var janela = new ResponderTicketDetalhes(ticket, _utilizador ?? ViewModel.TecnicoLogado);
            janela.ShowDialog();
        }

        // Método do botão dentro do DataGrid para responder ticket
        private void BtnResponder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var ticket = button.DataContext as Ticket;
            if (ticket == null) return;

            AbrirJanelaResponderTicket(ticket);
        }

        private void BtnAplicarFiltros_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AplicarFiltros();
        }

        private void BtnAplicarFiltros_Click1(object sender, RoutedEventArgs e)
        {
            EstatisticasVM.AtualizarEstatisticas();
        }

        private void BtnVerMapas_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            MapaSection.Visibility = Visibility.Visible;

            MapaSection.DataContext = EstatisticasVM;
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            ResponderSection.Visibility = Visibility.Collapsed;
            MapaSection.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Visible;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
