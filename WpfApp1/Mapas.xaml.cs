using System.Windows;
using BLL; // Ajuste conforme seu projeto
using UI.ViewModels; // Supondo que o ViewModel esteja nesse namespace, ajuste se necessário

namespace UI
{
    /**
     * @class Mapas
     * @brief Janela que exibe mapas ou gráficos relacionados às estatísticas de tickets.
     */
    public partial class Mapas : Window
    {
        /**
         * @brief ViewModel que contém as estatísticas dos tickets.
         */
        public EstatisticasTicketsViewModel ViewModel { get; set; }

        /**
         * @brief Construtor da janela Mapas.
         * Inicializa a interface e configura o DataContext com o ViewModel.
         * 
         * @param tecnicoId Identificador do técnico para carregar as estatísticas filtradas.
         */
        public Mapas(int tecnicoId)
        {
            InitializeComponent();

            ViewModel = new EstatisticasTicketsViewModel(tecnicoId, "Todos", "Todos", "Todos", "Todos");
            this.DataContext = ViewModel;
        }
    }
}
