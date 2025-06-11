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
                string.IsNullOrWhiteSpace(txtDescricao.Text) ||
                cbSubtipoProblema.SelectedItem == null)
            {
                MessageBox.Show("Todos os campos são obrigatórios.");
                return;
            }

            string subtipo = cbSubtipoProblema.SelectedItem.ToString();
            string tipo = (cbTipo.SelectedItem as ComboBoxItem)?.Content.ToString();
            string prioridade = (cbPrioridade.SelectedItem as ComboBoxItem)?.Content.ToString();

            Ticket novo = new Ticket
            {
                Tipo = tipo,
                SubtipoProblema = subtipo,
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
                cbSubtipoProblema.SelectedIndex = -1;
                cbPrioridade.SelectedIndex = -1;
                txtDescricao.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao criar ticket: " + ex.Message);
                MessageBox.Show($"ID do Utilizador no ticket: {novo.IdUtilizador}");
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

        private void cbTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbSubtipoProblema.Items.Clear();

            var tipoSelecionado = (cbTipo.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (tipoSelecionado == "Hardware")
            {
                cbSubtipoProblema.Items.Add("Disco rígido");
                cbSubtipoProblema.Items.Add("Placa mãe");
                cbSubtipoProblema.Items.Add("RAM");
                cbSubtipoProblema.Items.Add("Monitor");
            }
            else if (tipoSelecionado == "Software")
            {
                cbSubtipoProblema.Items.Add("Sistema operativo");
                cbSubtipoProblema.Items.Add("Aplicação interna");
                cbSubtipoProblema.Items.Add("Licença");
                cbSubtipoProblema.Items.Add("Erro de atualização");
            }
        }




    }
}
