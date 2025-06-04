using BLL;
using System.Windows;

namespace UI
{
    public partial class UserTickets : Window
    {
        private Utilizador utilizador;

        public UserTickets(Utilizador utilizador)
        {
            InitializeComponent();
            this.utilizador = utilizador;
            // Podes usar utilizador.Nome, etc., se quiseres mostrar dados
        }
    }
}
