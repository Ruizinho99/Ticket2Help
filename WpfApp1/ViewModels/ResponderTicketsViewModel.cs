using BLL;
using DAL;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UI.ViewModels
{
    public class ResponderTicketsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Ticket> Tickets { get; set; } = new ObservableCollection<Ticket>();

        private string _filtroTipo;
        public string FiltroTipo
        {
            get => _filtroTipo;
            set
            {
                _filtroTipo = value;
                OnPropertyChanged(nameof(FiltroTipo));
                CarregarTickets();
            }
        }

        private string _filtroPrioridade;
        public string FiltroPrioridade
        {
            get => _filtroPrioridade;
            set
            {
                _filtroPrioridade = value;
                OnPropertyChanged(nameof(FiltroPrioridade));
                CarregarTickets();
            }
        }

        public void CarregarTickets()
        {
            Tickets.Clear();

            string tipo = FiltroTipo == "Todos" ? null : FiltroTipo;
            string prioridade = FiltroPrioridade == "Todos" ? null : FiltroPrioridade;

            var lista = TicketDAL.ObterTicketsPorAtender(tipo, prioridade);
            foreach (var ticket in lista)
            {
                Tickets.Add(ticket);
            }
        }


        public ResponderTicketsViewModel()
            
        {   FiltroTipo = "Todos";
            FiltroPrioridade = "Todos";
            CarregarTickets();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
    }
}
