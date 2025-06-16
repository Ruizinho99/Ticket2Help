using BLL;
using DAL;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using UI.Helpers;
using UI.Views;

namespace UI.ViewModels
{
    public class ResponderTicketsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Ticket> Tickets { get; set; } = new ObservableCollection<Ticket>();
        private List<Ticket> TodosOsTickets = new List<Ticket>(); // Lista completa carregada

        public ObservableCollection<string> Tipos { get; set; } = new ObservableCollection<string> { "Todos", "Hardware", "Software" };
        public ObservableCollection<string> Prioridades { get; set; } = new ObservableCollection<string> { "Todos", "Baixa", "Média", "Alta" };
        public ObservableCollection<string> EstadosTicket { get; set; } = new ObservableCollection<string> { "Todos", "porAtender", "emAtendimento", "atendido" };
        public ObservableCollection<string> EstadosAtendimento { get; set; } = new ObservableCollection<string> { "Todos", "aberto", "naoResolvido", "resolvido" };

        private string _filtroTipo = "Todos";
        public string FiltroTipo
        {
            get => _filtroTipo;
            set
            {
                _filtroTipo = value;
                OnPropertyChanged(nameof(FiltroTipo));
                AplicarFiltros();
            }
        }

        private string _filtroPrioridade = "Todos";
        public string FiltroPrioridade
        {
            get => _filtroPrioridade;
            set
            {
                _filtroPrioridade = value;
                OnPropertyChanged(nameof(FiltroPrioridade));
                AplicarFiltros();
            }
        }

        private string _filtroEstadoTicket = "Todos";
        public string FiltroEstadoTicket
        {
            get => _filtroEstadoTicket;
            set
            {
                _filtroEstadoTicket = value;
                OnPropertyChanged(nameof(FiltroEstadoTicket));
                AplicarFiltros();
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
                AplicarFiltros();
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

            ResponderCommand = new RelayCommand(
                executar: _ => ResponderTicket(),
                podeExecutar: _ => TicketSelecionado != null
            );

            CarregarTickets();
        }

        public void CarregarTickets()
        {
            // Carrega todos os tickets uma única vez
            TodosOsTickets = TicketDAL.ObterTicketsPorAtender(null, null, null, null, _tecnicoLogado.Id).ToList();

            AplicarFiltros(); // Aplica os filtros atuais
        }

        public void AplicarFiltros()
        {
            var ticketsFiltrados = TodosOsTickets.AsEnumerable();

            if (FiltroTipo != "Todos")
                ticketsFiltrados = ticketsFiltrados.Where(t => t.Tipo == FiltroTipo);

            if (FiltroPrioridade != "Todos")
                ticketsFiltrados = ticketsFiltrados.Where(t => t.Prioridade == FiltroPrioridade);

            if (FiltroEstadoTicket != "Todos")
                ticketsFiltrados = ticketsFiltrados.Where(t => t.EstadoTicket == FiltroEstadoTicket);

            if (FiltroEstadoAtendimento != "Todos")
                ticketsFiltrados = ticketsFiltrados.Where(t => t.EstadoAtendimento == FiltroEstadoAtendimento);

            Tickets.Clear();
            foreach (var ticket in ticketsFiltrados)
            {
                Tickets.Add(ticket);
            }
        }

        private void ResponderTicket()
        {
            if (TicketSelecionado == null) return;

            var janela = new ResponderTicketDetalhes(TicketSelecionado, _tecnicoLogado);
            janela.ShowDialog();

            CarregarTickets(); // Atualiza a lista após responder
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
    }
}
