using BLL;
using System;
using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    public partial class ResponderTickets : Window
    {
        private Utilizador _utilizador;
        public EstatisticasTicketsViewModel EstatisticasVM { get; set; }
        public ResponderTickets(Utilizador utilizador)
        {
            InitializeComponent();
            _utilizador = utilizador;
            DataContext = new ResponderTicketsViewModel(_utilizador);

            // Passa o id do técnico logado para carregar os tickets dele
            EstatisticasVM = new EstatisticasTicketsViewModel(
    _utilizador.Id,
    tipoFiltro: "Todos",
    prioridadeFiltro: "Todos",
    estadoTicketFiltro: "Todos",
    estadoAtendimentoFiltro: "Todos"
);

            MapaSection.DataContext = EstatisticasVM;
        }

        private void BtnResponderTickets_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            ResponderSection.Visibility = Visibility.Visible;
        }

        private void BtnVerMapas_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            MapaSection.Visibility = Visibility.Visible;
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
        private void BtnAplicarFiltros_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ResponderTicketsViewModel vm)
            {
                vm.CarregarTickets();
            }
        }
        private void BtnAplicarFiltros_Click1(object sender, RoutedEventArgs e)
        {
            if (DataContext is ResponderTicketsViewModel vm)
            {
                EstatisticasVM = new EstatisticasTicketsViewModel(
                    _utilizador.Id,
                    vm.TipoFiltro,
                    vm.PrioridadeFiltro,
                    vm.EstadoTicketFiltro,
                    vm.EstadoAtendimentoFiltro,
                    vm.DataInicioFiltro,
                    vm.DataFimFiltro);

                MapaSection.DataContext = EstatisticasVM;
            }
        }


    }
}
