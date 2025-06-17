using System;
using System.Windows;
using UI.ViewModels;

namespace UI
{
    public partial class Mapas : Window
    {
        public EstatisticasTicketsViewModel ViewModel { get; set; }

        public Mapas(int tecnicoId)
        {
            InitializeComponent();
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

        private void BtnAplicarFiltro_Click(object sender, RoutedEventArgs e)
        {
            if (dpDataInicio.SelectedDate.HasValue && dpDataFim.SelectedDate.HasValue)
            {
                if (dpDataInicio.SelectedDate > dpDataFim.SelectedDate)
                {
                    MessageBox.Show("A data de início não pode ser maior que a data de fim.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ViewModel.DataInicioFiltro = dpDataInicio.SelectedDate.Value;
                ViewModel.DataFimFiltro = dpDataFim.SelectedDate.Value;

                ViewModel.CarregarEstatisticas();
            }
            else
            {
                MessageBox.Show("Selecione ambas as datas para aplicar o filtro.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
