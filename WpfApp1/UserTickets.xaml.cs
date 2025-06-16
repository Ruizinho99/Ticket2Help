using DAL;
using BLL;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace UI
{
    /**
     * @class UserTickets
     * @brief Janela para que um utilizador visualize seus tickets submetidos e possa criar novos tickets.
     */
    public partial class UserTickets : Window
    {
        private Utilizador utilizador; /**< Utilizador logado */

        /**
         * @brief Construtor da janela UserTickets.
         * Inicializa a interface e carrega os tickets submetidos pelo utilizador.
         * 
         * @param utilizador Utilizador atualmente logado.
         */
        public UserTickets(Utilizador utilizador)
        {
            InitializeComponent();
            this.utilizador = utilizador;

            MessageBox.Show($"ID do utilizador atual: {utilizador?.Id}");

            CarregarTicketsSubmetidos(); // Carrega tickets do utilizador
        }

        /**
         * @brief Carrega e filtra os tickets submetidos pelo utilizador, e aplica ordenação.
         * 
         * @param tipoFiltro Filtro para o tipo do ticket ("Hardware", "Software", "Todos", etc).
         * @param prioridadeFiltro Filtro para a prioridade do ticket ("Alta", "Média", "Baixa", "Todas", etc).
         * @param estadoFiltro Filtro para o estado do ticket ("Por resolver", "Resolvido", "Todos", etc).
         * @param ordenarPor Critério para ordenação ("ID Ascendente", "Data Descendente", etc).
         */
        private void CarregarTicketsSubmetidos(string tipoFiltro = null, string prioridadeFiltro = null, string estadoFiltro = null, string ordenarPor = null)
        {
            var tickets = TicketDAL.ObterTicketsDoUtilizador(utilizador.Id);

            if (!string.IsNullOrEmpty(tipoFiltro) && tipoFiltro != "Todos")
                tickets = tickets.Where(t => t.Tipo == tipoFiltro).ToList();

            if (!string.IsNullOrEmpty(prioridadeFiltro) && prioridadeFiltro != "Todas")
                tickets = tickets.Where(t => t.Prioridade == prioridadeFiltro).ToList();

            if (!string.IsNullOrEmpty(estadoFiltro) && estadoFiltro != "Todos")
                tickets = tickets.Where(t => t.EstadoTicket.Equals(estadoFiltro, StringComparison.OrdinalIgnoreCase)).ToList();

            switch (ordenarPor)
            {
                case "ID Ascendente":
                    tickets = tickets.OrderBy(t => t.Id).ToList();
                    break;
                case "ID Descendente":
                    tickets = tickets.OrderByDescending(t => t.Id).ToList();
                    break;
                case "Data Ascendente":
                    tickets = tickets.OrderBy(t => t.DataCriacao).ToList();
                    break;
                case "Data Descendente":
                    tickets = tickets.OrderByDescending(t => t.DataCriacao).ToList();
                    break;
                default:
                    tickets = tickets.OrderByDescending(t => t.DataCriacao).ToList();
                    break;
            }

            dgTickets.ItemsSource = tickets; /**< Atualiza o DataGrid com os tickets filtrados e ordenados */
        }

        /**
         * @brief Evento do botão para aplicar os filtros selecionados na interface.
         */
        private void BtnAplicarFiltros_Click(object sender, RoutedEventArgs e)
        {
            string tipoFiltro = (cbFiltroTipo.SelectedItem as ComboBoxItem)?.Content.ToString();
            string prioridadeFiltro = (cbFiltroPrioridade.SelectedItem as ComboBoxItem)?.Content.ToString();
            string estadoFiltro = (cbFiltroEstado.SelectedItem as ComboBoxItem)?.Content.ToString();
            string ordenarPor = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString();

            CarregarTicketsSubmetidos(tipoFiltro, prioridadeFiltro, estadoFiltro, ordenarPor);
        }

        /**
         * @brief Evento do botão para submeter um novo ticket.
         * Valida os campos obrigatórios, cria o ticket e limpa o formulário.
         */
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

        /**
         * @brief Evento de duplo clique no DataGrid de tickets.
         * Abre uma janela com detalhes do ticket selecionado.
         */
        private void dgTickets_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgTickets.SelectedItem is Ticket ticketSelecionado)
            {
                DetalhesTicket detalhesWindow = new DetalhesTicket(ticketSelecionado);
                detalhesWindow.ShowDialog();
            }
        }

        /**
         * @brief Evento para atualizar os subtipo de problema conforme o tipo selecionado.
         * Exemplo: se selecionar "Hardware", carrega opções relacionadas a hardware.
         */
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

        /**
         * @brief Exibe a tela de criação de novo ticket e oculta o menu principal.
         */
        private void BtnCriarTicket_Click(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Collapsed;
            CriarTicketGrid.Visibility = Visibility.Visible;
        }

        /**
         * @brief Exibe a tela de visualização dos tickets submetidos e oculta o menu principal.
         */
        private void BtnVerTickets_Click(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Collapsed;
            VerTicketsGrid.Visibility = Visibility.Visible;
            CarregarTicketsSubmetidos(); // Atualiza lista ao mostrar
        }

        /**
         * @brief Volta ao menu principal ocultando as outras telas.
         */
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            CriarTicketGrid.Visibility = Visibility.Collapsed;
            VerTicketsGrid.Visibility = Visibility.Collapsed;
            MainMenuGrid.Visibility = Visibility.Visible;
        }

        /**
         * @brief Evento para logout: fecha esta janela e abre a janela de login.
         */
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}

