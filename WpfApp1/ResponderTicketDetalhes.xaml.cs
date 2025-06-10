using BLL;
using DAL;
using System;
using System.Windows;

namespace UI.Views
{
    public partial class ResponderTicketDetalhes : Window
    {
        private Ticket _ticket;

        public ResponderTicketDetalhes(Ticket ticket)
        {
            InitializeComponent();
            _ticket = ticket;
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            string resposta = txtResposta.Text.Trim();

            if (string.IsNullOrWhiteSpace(resposta))
            {
                MessageBox.Show("Por favor, insira uma resposta antes de guardar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Atualiza o ticket no banco de dados
            _ticket.DetalhesTecnico = resposta;
            _ticket.EstadoTicket = "respondido";
            _ticket.EstadoAtendimento = "concluido";
            _ticket.DataAtendimento = DateTime.Now;

            TicketDAL.ResponderTicket(_ticket);

            MessageBox.Show("Resposta enviada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
