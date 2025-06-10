using BLL;
using System.Windows;

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

