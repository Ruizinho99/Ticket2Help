using System.Windows;
using BLL; // Ajuste conforme seu projeto
using UI.ViewModels; // Supondo que o ViewModel esteja nesse namespace, ajuste se necessário

namespace UI
{
    public partial class Mapas : Window
    {
        public EstatisticasTicketsViewModel ViewModel { get; set; }

        public Mapas(int tecnicoId)
        {
            InitializeComponent();

            ViewModel = new EstatisticasTicketsViewModel(tecnicoId, "Todos", "Todos", "Todos", "Todos");
            this.DataContext = ViewModel;
        }
    }
}
