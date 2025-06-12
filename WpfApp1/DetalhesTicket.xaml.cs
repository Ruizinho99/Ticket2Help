using BLL;
using DAL;
using System.Windows;
using System.Windows.Controls;
using UI.Views;

namespace UI
{
    public partial class DetalhesTicket : Window
    {
        private Ticket _ticket;

        public DetalhesTicket(Ticket ticket)
        {
            InitializeComponent();

            _ticket = ticket;
            DataContext = _ticket;

            // Habilita o ComboBox só se o ticket estiver "atendido"
            cmbEstadoAtendimento.IsEnabled = _ticket.EstadoTicket == "atendido";

            // Inicializa o ComboBox com o valor atual do EstadoAtendimento
            // Exemplo: "resolvido" ou "naoResolvido"
            if (!string.IsNullOrEmpty(_ticket.EstadoAtendimento))
            {
                if (_ticket.EstadoAtendimento == "resolvido")
                    cmbEstadoAtendimento.SelectedIndex = 0; // "Sim"
                else if (_ticket.EstadoAtendimento == "naoResolvido")
                    cmbEstadoAtendimento.SelectedIndex = 1; // "Não"
                else
                    cmbEstadoAtendimento.SelectedIndex = -1;
            }
            else
            {
                cmbEstadoAtendimento.SelectedIndex = -1;
            }
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEstadoAtendimento.SelectedItem is ComboBoxItem item)
            {
                string escolha = item.Content.ToString();

                if (escolha == "Sim")
                    _ticket.EstadoAtendimento = "resolvido";
                else if (escolha == "Não")
                    _ticket.EstadoAtendimento = "naoResolvido";
            }

            // Atualize o ticket no banco de dados
            TicketDAL.AtualizarTicket(_ticket);

            MessageBox.Show("Ticket atualizado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
