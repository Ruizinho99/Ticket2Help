using DAL;
using BLL;
using System;
using System.Windows;
using System.Windows.Controls;


namespace UI
{
    public partial class UserTickets : Window
    {
        private Utilizador utilizador;

        public UserTickets(Utilizador utilizador)
        {
            InitializeComponent();
            this.utilizador = utilizador;

            MessageBox.Show($"ID do utilizador atual: {utilizador?.Id}");

            CarregarTicketsParaResponder();      // Tickets pendentes para resposta
            CarregarTicketsSubmetidos();        // Tickets submetidos pelo utilizador
        }


        private void CarregarTicketsSubmetidos()
        {
            var tickets = TicketDAL.ObterTicketsDoUtilizador(utilizador.Id);
            dgTickets.ItemsSource = tickets;
        }

        private void BtnSubmeter_Click(object sender, RoutedEventArgs e)
        {
            if (cbTipo.SelectedItem == null ||
                cbPrioridade.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("Todos os campos são obrigatórios.");
                return;
            }

            string tipo = (cbTipo.SelectedItem as ComboBoxItem)?.Content.ToString();
            string prioridade = (cbPrioridade.SelectedItem as ComboBoxItem)?.Content.ToString();

            Ticket novo = new Ticket
            {
                Tipo = tipo,
                Prioridade = prioridade,
                Descricao = txtDescricao.Text,
                EstadoTicket = "Por resolver",
                EstadoAtendimento = "Pendente",
                DataCriacao = DateTime.Now,
                IdUtilizador = utilizador.Id
            };

            try
            {
                TicketDAL.CriarTicket(novo);
                MessageBox.Show("Ticket criado com sucesso!");
                cbTipo.SelectedIndex = -1;
                cbPrioridade.SelectedIndex = -1;
                txtDescricao.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao criar ticket: " + ex.Message);
                MessageBox.Show($"ID do Utilizador no ticket: {novo.IdUtilizador}");
            }
        }

        private void CarregarTicketsParaResponder()
        {
            // Obtem a lista de tickets para este utilizador
            var tickets = TicketDAL.ObterTicketsParaResponder(utilizador.Id);

            if (tickets.Count == 0)
            {
                cbTicketsParaResponder.ItemsSource = null;
                cbTicketsParaResponder.Visibility = Visibility.Collapsed;
                txtSemTickets.Visibility = Visibility.Visible;
            }
            else
            {
                cbTicketsParaResponder.ItemsSource = tickets;
                cbTicketsParaResponder.Visibility = Visibility.Visible;
                txtSemTickets.Visibility = Visibility.Collapsed;
            }
        }
        private void dgTickets_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgTickets.SelectedItem is Ticket ticketSelecionado)
            {
                DetalhesTicket detalhesWindow = new DetalhesTicket(ticketSelecionado);
                detalhesWindow.ShowDialog();
            }
        }


        private void CbTicketsParaResponder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTicketsParaResponder.SelectedItem is Ticket ticketSelecionado)
            {
                // Preenche o TextBox com o texto atual da resposta técnica
                txtResposta.Text = ticketSelecionado.DetalhesTecnico;
            }
            else
            {
                txtResposta.Clear();
            }
        }

        private void BtnResponder_Click(object sender, RoutedEventArgs e)
        {
            if (cbTicketsParaResponder.SelectedItem is Ticket ticketSelecionado)
            {
                ticketSelecionado.DetalhesTecnico = txtResposta.Text;
                // Se quiser, pode atualizar o estado para “respondido” ou algo assim
                // ticketSelecionado.EstadoTicket = "respondido"; // Exemplo

                try
                {
                    TicketDAL.ResponderTicket(ticketSelecionado);
                    MessageBox.Show("Resposta atualizada com sucesso!");
                    CarregarTicketsParaResponder(); // Atualiza a lista para refletir alterações
                    txtResposta.Clear();
                    cbTicketsParaResponder.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao atualizar resposta: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Selecione um ticket para responder.");
            }
        }
    }
}
