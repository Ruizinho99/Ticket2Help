using DAL;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UI
{
    /// <summary>
    /// Janela para exibir e criar tickets associados a um utilizador.
    /// </summary>
    public partial class UserTickets : Window
    {
        /// <summary>
        /// Utilizador atualmente autenticado.
        /// </summary>
        private Utilizador utilizador;

        /// <summary>
        /// Construtor da janela UserTickets.
        /// </summary>
        /// <param name="utilizador">Instância do utilizador atual.</param>
        public UserTickets(Utilizador utilizador)
        {
            InitializeComponent();
            this.utilizador = utilizador;

            MessageBox.Show($"ID do utilizador atual: {utilizador?.Id}");

            CarregarTicketsSubmetidos(); ///< Carrega os tickets submetidos pelo utilizador
        }

        /**
         * @brief Carrega os tickets do utilizador com opções de filtro e ordenação.
         * 
         * @param tipoFiltro Filtro pelo tipo do ticket (opcional).
         * @param prioridadeFiltro Filtro pela prioridade do ticket (opcional).
         * @param estadoFiltro Filtro pelo estado do ticket (opcional).
         * @param ordenarPor Critério para ordenar os tickets (opcional).
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

            PreencherTickets(tickets);
        }

        /**
         * @brief Cria visualmente os cards de tickets na interface.
         * 
         * @param tickets Lista de tickets a exibir.
         */
        private void PreencherTickets(List<Ticket> tickets)
        {
            TicketsPanel.Children.Clear();

            foreach (var ticket in tickets)
            {
                var border = new Border
                {
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(10),
                    Margin = new Thickness(10),
                    Padding = new Thickness(10),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDD")),
                    BorderThickness = new Thickness(1),
                    Width = 250,
                    Cursor = Cursors.Hand,
                    Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = Colors.Gray,
                        Direction = 320,
                        ShadowDepth = 3,
                        Opacity = 0.3
                    }
                };

                var stack = new StackPanel();

                stack.Children.Add(new TextBlock { Text = $"ID: {ticket.Id}", FontWeight = FontWeights.Bold, FontSize = 16, Margin = new Thickness(0, 0, 0, 5) });
                stack.Children.Add(new TextBlock { Text = $"Tipo: {ticket.Tipo}", Margin = new Thickness(0, 0, 0, 3) });
                stack.Children.Add(new TextBlock { Text = $"Prioridade: {ticket.Prioridade}", Margin = new Thickness(0, 0, 0, 3) });
                stack.Children.Add(new TextBlock
                {
                    Text = $"Estado: {ticket.EstadoTicket}",
                    Margin = new Thickness(0, 0, 0, 3),
                    Foreground = GetEstadoColor(ticket.EstadoTicket)
                });
                stack.Children.Add(new TextBlock { Text = $"Criado em: {ticket.DataCriacao.ToShortDateString()}", Margin = new Thickness(0, 0, 0, 3) });

                border.Child = stack;

                // Evento para abrir detalhes do ticket ao clicar no card
                border.MouseLeftButtonUp += (s, e) =>
                {
                    DetalhesTicket detalhesWindow = new DetalhesTicket(ticket);
                    detalhesWindow.ShowDialog();
                };

                TicketsPanel.Children.Add(border);
            }
        }

        /**
         * @brief Retorna uma cor correspondente ao estado do ticket.
         * 
         * @param estado Estado atual do ticket.
         * @return Brush Cor associada ao estado.
         */
        private Brush GetEstadoColor(string estado)
        {
            switch (estado)
            {
                case "Por resolver":
                    return Brushes.Red;
                case "Em atendimento":
                    return Brushes.Orange;
                case "Resolvido":
                    return Brushes.Green;
                default:
                    return Brushes.Black;
            }
        }

        /**
         * @brief Evento para aplicar filtros e ordenação ao clicar no botão de aplicar filtros.
         * 
         * @param sender Objeto que disparou o evento.
         * @param e Argumentos do evento.
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
         * @brief Evento para submeter um novo ticket.
         * 
         * @param sender Objeto que disparou o evento.
         * @param e Argumentos do evento.
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
         * @brief Evento para alterar a lista de subtipos conforme o tipo selecionado.
         * 
         * @param sender Objeto que disparou o evento.
         * @param e Argumentos do evento de seleção.
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

        /// <summary>
        /// Exibe o formulário para criar um novo ticket.
        /// </summary>
        private void BtnCriarTicket_Click(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Collapsed;
            CriarTicketGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Exibe a visualização dos tickets do utilizador.
        /// </summary>
        private void BtnVerTickets_Click(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Collapsed;
            VerTicketsGrid.Visibility = Visibility.Visible;
            CarregarTicketsSubmetidos();
        }

        /// <summary>
        /// Volta para o menu principal, ocultando outros grids.
        /// </summary>
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            CriarTicketGrid.Visibility = Visibility.Collapsed;
            VerTicketsGrid.Visibility = Visibility.Collapsed;
            MainMenuGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Realiza logout e exibe a janela de login.
        /// </summary>
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
