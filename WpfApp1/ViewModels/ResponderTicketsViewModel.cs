using BLL;
using DAL;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using UI.Views;
using UI.Helpers; // importa o RelayCommand


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

        private Ticket _ticketSelecionado;
        public Ticket TicketSelecionado
        {
            get => _ticketSelecionado;
            set
            {
                _ticketSelecionado = value;
                OnPropertyChanged(nameof(TicketSelecionado));
                CommandManager.InvalidateRequerySuggested(); // força reavaliação do CanExecute
            }
        }


        public ICommand ResponderCommand { get; }

        public ResponderTicketsViewModel()
        {
            FiltroTipo = "Todos";
            FiltroPrioridade = "Todos";
            CarregarTickets();

            ResponderCommand = new RelayCommand(
             executar: _ => ResponderTicket(),
             podeExecutar: _ => TicketSelecionado != null
         );

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

        private void ResponderTicket()
        {
            if (TicketSelecionado == null) return;

            var janela = new ResponderTicketDetalhes(TicketSelecionado);
            janela.ShowDialog();

            CarregarTickets(); // Atualiza a lista após resposta
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
    }
}
