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
            // ou cria contextos separados para cada secção
        }

        private void AbrirJanelaResponderTicket(Ticket ticket)
        {
            if (ticket == null)
                return;

            var janela = new ResponderTicketDetalhes(ticket, _utilizador ?? ViewModel.TecnicoLogado);
            janela.ShowDialog();
        }

        private void BtnResponderTickets_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            MapaSection.Visibility = Visibility.Collapsed;
            ResponderSection.Visibility = Visibility.Visible;

            // Atualiza o DataContext para a seção responder
            ResponderSection.DataContext = ViewModel;
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

            // Aqui mudas o DataContext desta secção
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

