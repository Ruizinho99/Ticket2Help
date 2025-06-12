using BLL;
using DAL;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using UI.Views;
using UI.Helpers;


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
                CommandManager.InvalidateRequerySuggested(); 
            }
        }


        public ICommand ResponderCommand { get; }
        private Utilizador _tecnicoLogado;

        public ResponderTicketsViewModel(Utilizador tecnico)
        {
            _tecnicoLogado = tecnico;
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
            string estadoTicket = FiltroEstadoTicket == "Todos" ? null : FiltroEstadoTicket;
            string estadoAtendimento = FiltroEstadoAtendimento == "Todos" ? null : FiltroEstadoAtendimento;

            var lista = TicketDAL.ObterTicketsPorAtender(tipo, prioridade, estadoTicket, estadoAtendimento, _tecnicoLogado.Id);



            foreach (var ticket in lista)
            {
                Tickets.Add(ticket);
            }
        }


        private void ResponderTicket()
        {
            if (TicketSelecionado == null) return;

            
            var janela = new ResponderTicketDetalhes(TicketSelecionado, _tecnicoLogado);
            // passa o técnico como segundo parâmetro
            janela.ShowDialog();

            CarregarTickets();
        }

        private string _filtroEstadoTicket = "Todos";
        public string FiltroEstadoTicket
        {
            get => _filtroEstadoTicket;
            set
            {
                _filtroEstadoTicket = value;
                OnPropertyChanged(nameof(FiltroEstadoTicket));
                CarregarTickets();
            }
        }

        private string _filtroEstadoAtendimento = "Todos";
        public string FiltroEstadoAtendimento
        {
            get => _filtroEstadoAtendimento;
            set
            {
                _filtroEstadoAtendimento = value;
                OnPropertyChanged(nameof(FiltroEstadoAtendimento));
                CarregarTickets();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
    }
}
