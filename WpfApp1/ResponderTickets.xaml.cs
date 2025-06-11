using BLL;
using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    public partial class ResponderTickets : Window
    {
        private Utilizador _utilizador;

        public ResponderTickets(Utilizador utilizador)
        {
            InitializeComponent();
            _utilizador = utilizador;
            DataContext = new ResponderTicketsViewModel(_utilizador);
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

    }
}
