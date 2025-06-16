    using DAL;
    using BLL;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Linq;


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


            private void CarregarTicketsSubmetidos(string tipoFiltro = null, string prioridadeFiltro = null, string estadoFiltro = null, string ordenarPor = null)
            {
                var tickets = TicketDAL.ObterTicketsDoUtilizador(utilizador.Id);

                // Filtrar tipo
                if (!string.IsNullOrEmpty(tipoFiltro) && tipoFiltro != "Todos")
                    tickets = tickets.Where(t => t.Tipo == tipoFiltro).ToList();

                // Filtrar prioridade
                if (!string.IsNullOrEmpty(prioridadeFiltro) && prioridadeFiltro != "Todas")
                    tickets = tickets.Where(t => t.Prioridade == prioridadeFiltro).ToList();

                // Filtrar estado
                if (!string.IsNullOrEmpty(estadoFiltro) && estadoFiltro != "Todos")
                    tickets = tickets.Where(t => t.EstadoTicket.Equals(estadoFiltro, StringComparison.OrdinalIgnoreCase)).ToList();

                // Ordenar
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

                dgTickets.ItemsSource = tickets;
            }

            private void BtnAplicarFiltros_Click(object sender, RoutedEventArgs e)
            {
                string tipoFiltro = (cbFiltroTipo.SelectedItem as ComboBoxItem)?.Content.ToString();
                string prioridadeFiltro = (cbFiltroPrioridade.SelectedItem as ComboBoxItem)?.Content.ToString();
                string estadoFiltro = (cbFiltroEstado.SelectedItem as ComboBoxItem)?.Content.ToString();
                string ordenarPor = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString();

                CarregarTicketsSubmetidos(tipoFiltro, prioridadeFiltro, estadoFiltro, ordenarPor);
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

            private void BtnCriarTicket_Click(object sender, RoutedEventArgs e)
            {
                MainMenuGrid.Visibility = Visibility.Collapsed;
                CriarTicketGrid.Visibility = Visibility.Visible;
            }

            private void BtnVerTickets_Click(object sender, RoutedEventArgs e)
            {
                MainMenuGrid.Visibility = Visibility.Collapsed;
                VerTicketsGrid.Visibility = Visibility.Visible;
                CarregarTicketsSubmetidos(); // para garantir que atualiza sempre
            }

            private void BtnVoltar_Click(object sender, RoutedEventArgs e)
            {
                CriarTicketGrid.Visibility = Visibility.Collapsed;
                VerTicketsGrid.Visibility = Visibility.Collapsed;
                MainMenuGrid.Visibility = Visibility.Visible;
            }

            private void BtnLogout_Click(object sender, RoutedEventArgs e)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close(); // Fecha a janela atual (UserTickets)
            }




        }
    }
