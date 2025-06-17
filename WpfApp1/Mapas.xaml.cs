using System;
using System.Windows;
using UI.ViewModels;

namespace UI
{
    /// <summary>
    /// @class Mapas
    /// @brief Janela responsável por exibir as estatísticas de tickets.
    /// 
    /// Permite ao usuário visualizar e filtrar estatísticas relacionadas aos tickets de atendimento.
    /// </summary>
    public partial class Mapas : Window
    {
        /// <summary>
        /// ViewModel responsável por carregar e gerenciar os dados das estatísticas de tickets.
        /// </summary>
        public EstatisticasTicketsViewModel ViewModel { get; set; }

        /// <summary>
        /// Construtor da janela Mapas.
        /// </summary>
        /// <param name="tecnicoId">Identificador do técnico que será usado para carregar os dados.</param>
        public Mapas(int tecnicoId)
        {
            InitializeComponent();

            /// <summary>
            /// Inicializa o ViewModel com filtros padrão e define o intervalo de datas inicial (últimos 30 dias).
            /// </summary>
            ViewModel = new EstatisticasTicketsViewModel(
                tecnicoId,
                tipoFiltro: "Todos",
                prioridadeFiltro: "Todos",
                estadoTicketFiltro: "Todos",
                estadoAtendimentoFiltro: "Todos",
                dataInicio: DateTime.Today.AddMonths(-1),
                dataFim: DateTime.Today);

            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Evento chamado ao clicar no botão "Aplicar Filtro".
        /// Valida as datas selecionadas e atualiza os filtros no ViewModel.
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento.</param>
        /// <param name="e">Dados do evento.</param>
        private void BtnAplicarFiltro_Click(object sender, RoutedEventArgs e)
        {
            // Verifica se as datas foram selecionadas
            if (dpDataInicio.SelectedDate.HasValue && dpDataFim.SelectedDate.HasValue)
            {
                // Verifica se a data de início é maior que a data de fim
                if (dpDataInicio.SelectedDate > dpDataFim.SelectedDate)
                {
                    MessageBox.Show("A data de início não pode ser maior que a data de fim.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Atualiza os filtros no ViewModel
                ViewModel.DataInicioFiltro = dpDataInicio.SelectedDate.Value;
                ViewModel.DataFimFiltro = dpDataFim.SelectedDate.Value;

                // Carrega as estatísticas atualizadas
                ViewModel.CarregarEstatisticas();
            }
            else
            {
                // Exibe aviso caso as datas não tenham sido selecionadas
                MessageBox.Show("Selecione ambas as datas para aplicar o filtro.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
