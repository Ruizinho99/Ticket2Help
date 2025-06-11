using BLL;
using DAL;
using System;
using System.Windows;
using System.Windows.Controls;

namespace UI.Views
{
    public partial class ResponderTicketDetalhes : Window
    {
        private Ticket _ticket;
        private Utilizador _tecnicoLogado;

        public ResponderTicketDetalhes(Ticket ticket, Utilizador tecnicoLogado)
        {
            InitializeComponent();
            _ticket = ticket;
            _tecnicoLogado = tecnicoLogado;

            // Seleciona o estado atual no ComboBox
            foreach (ComboBoxItem item in cmbEstadoTicket.Items)
            {
                if ((string)item.Content == _ticket.EstadoTicket)
                {
                    cmbEstadoTicket.SelectedItem = item;
                    break;
                }
            }
        }



        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            string estadoSelecionado = (cmbEstadoTicket.SelectedItem as ComboBoxItem)?.Content.ToString();
            string resposta = txtResposta.Text.Trim();

            
            // Se o estado for "atendido", a resposta é obrigatória
            if (estadoSelecionado == "atendido" && string.IsNullOrWhiteSpace(resposta))
            {
                MessageBox.Show("Por favor, insira uma resposta antes de guardar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            // Atualiza o estado do ticket conforme selecionado
            _ticket.EstadoTicket = estadoSelecionado;

            // Atualiza DetalhesTecnico com a resposta do técnico (pode ser vazia se for porAtender)
            _ticket.DetalhesTecnico = resposta;

            if (estadoSelecionado == "porAtender")
            {
                // Não altera EstadoAtendimento e DataAtendimento
                // Apenas atualiza o que for necessário, sem obrigatoriedade de resposta
            }
            else
            {
                _ticket.DataAtendimento = DateTime.Now;
                _ticket.RespondidoPor = _tecnicoLogado.Id;
            }

            // Atualiza o Id do técnico que respondeu (supondo que já tens o tecnicoLogado)
            

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
