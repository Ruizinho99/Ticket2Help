using BLL;
using DAL;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UI.Views
{
    /**
     * @class ResponderTickets
     * @brief Janela para listar e gerenciar tickets que o utilizador pode responder.
     * Permite aplicar filtros, ordenar, responder tickets, visualizar mapas e efetuar logout.
     */
    public partial class ResponderTickets : Window
    {
        private Utilizador _utilizador; /**< Utilizador logado que está respondendo aos tickets */

        /**
         * @brief Construtor da janela ResponderTickets.
         * Inicializa a interface e carrega os tickets do utilizador sem filtros.
         * 
         * @param utilizador Utilizador logado.
         */
        public ResponderTickets(Utilizador utilizador)
        {
            InitializeComponent();
            _utilizador = utilizador;

            // Carrega os tickets inicialmente sem filtros (todos)
            CarregarTicketsParaResponder();
        }

        /**
         * @brief Carrega os tickets para responder aplicando filtros e ordenação.
         * 
         * @param tipoFiltro Tipo do ticket (ex: "Hardware", "Software" ou "Todos").
         * @param prioridadeFiltro Prioridade do ticket (ex: "Baixa", "Média", "Alta" ou "Todos").
         * @param estadoTicketFiltro Estado do ticket (ex: "porAtender", "emAtendimento", "atendido" ou "Todos").
         * @param estadoAtendimentoFiltro Estado do atendimento (ex: "aberto", "resolvido", "naoResolvido" ou "Todos").
         * @param dataInicio Data inicial para filtro por data de criação (opcional).
         * @param dataFim Data final para filtro por data de criação (opcional).
         * @param ordenarPor Critério para ordenação dos tickets (ex: "ID Ascendente", "Data Descendente").
         */
        private void CarregarTicketsParaResponder(
            string tipoFiltro = "Todos",
            string prioridadeFiltro = "Todos",
            string estadoTicketFiltro = "Todos",
            string estadoAtendimentoFiltro = "Todos",
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string ordenarPor = null)
        {
            var tickets = TicketDAL.ObterTicketsPorAtender(
                tipoFiltro,
                prioridadeFiltro,
                estadoTicketFiltro,
                estadoAtendimentoFiltro,
                _utilizador.Id,
                dataInicio,
                dataFim
            );

            // Ordenar conforme seleção
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

            dgTicketsResponder.ItemsSource = tickets; ///< Atualiza o DataGrid com a lista de tickets
        }

        /**
         * @brief Evento do botão para aplicar filtros e atualizar a lista de tickets.
         * Obtém os filtros da interface e chama CarregarTicketsParaResponder.
         */
        private void BtnAplicarFiltros_Click(object sender, RoutedEventArgs e)
        {
            string tipoFiltro = (cbFiltroTipo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";
            string prioridadeFiltro = (cbFiltroPrioridade.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";
            string estadoTicketFiltro = (cbFiltroEstadoTicket.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";
            string estadoAtendimentoFiltro = (cbFiltroEstadoAtendimento.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";
            string ordenarPor = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString();

            DateTime? dataInicio = dpDataInicio.SelectedDate;
            DateTime? dataFim = dpDataFim.SelectedDate;

            CarregarTicketsParaResponder(
                tipoFiltro,
                prioridadeFiltro,
                estadoTicketFiltro,
                estadoAtendimentoFiltro,
                dataInicio,
                dataFim,
                ordenarPor);
        }

        /**
         * @brief Evento do botão para mostrar a seção de responder tickets.
         * Oculta o menu principal e exibe a seção de resposta.
         */
        private void BtnResponderTickets_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            ResponderSection.Visibility = Visibility.Visible;
        }

        /**
         * @brief Evento do botão para abrir a janela de mapas.
         */
        private void BtnVerMapas_Click(object sender, RoutedEventArgs e)
        {
            var janelaMapas = new Mapas(_utilizador.Id);
            janelaMapas.Owner = this;
            janelaMapas.Show();
        }

        /**
         * @brief Evento do botão para voltar ao menu principal.
         * Oculta a seção de resposta e exibe o menu principal.
         */
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            ResponderSection.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Visible;
        }

        /**
         * @brief Evento de duplo clique no DataGrid de tickets.
         * Abre a janela para responder ao ticket selecionado.
         * Atualiza a lista após fechamento da janela.
         */
        private void dgTicketsResponder_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var ticketSelecionado = dgTicketsResponder.SelectedItem as Ticket;
            if (ticketSelecionado == null)
            {
                MessageBox.Show("Por favor, selecione um ticket para responder.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Abre a janela de detalhes para responder o ticket
            var janelaResposta = new ResponderTicketDetalhes(ticketSelecionado, _utilizador);
            janelaResposta.Owner = this;  // opcional para manter janela pai
            janelaResposta.ShowDialog();

            // Após fechar a janela de resposta, atualiza a lista de tickets
            BtnAplicarFiltros_Click(null, null);
        }

        /**
         * @brief Evento do botão para fazer logout.
         * Fecha esta janela e abre a janela de login.
         */
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
