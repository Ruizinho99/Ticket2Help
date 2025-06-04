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




        private void BtnResponder_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para enviar resposta ao IT-Desk
        }
    }
}
