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
            DataContext = new ResponderTicketsViewModel();
        }
    }
}
