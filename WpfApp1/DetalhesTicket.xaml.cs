using BLL;
using System.Windows;
using UI.Views;

namespace UI
{
    public partial class DetalhesTicket : Window
    {
        public DetalhesTicket(Ticket ticket)
        {
            InitializeComponent();
            DataContext = ticket;
        }

      
    }
}

