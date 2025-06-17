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
    /**
     * @class ResponderTicketsViewModel
     * @brief ViewModel responsável por gerenciar a interface de resposta a tickets.
     * 
     * Controla a lista de tickets, os filtros aplicados e a lógica para responder tickets.
     */
    public class ResponderTicketsViewModel : INotifyPropertyChanged
    {
        /** Lista observável de tickets filtrados para exibição */
        public ObservableCollection<Ticket> Tickets { get; set; } = new ObservableCollection<Ticket>();

        /** Lista completa de tickets carregada do banco */
        private List<Ticket> TodosOsTickets = new List<Ticket>();

        /** Coleção de tipos de ticket para filtro */
        public ObservableCollection<string> Tipos { get; set; } = new ObservableCollection<string> { "Todos", "Hardware", "Software" };

        /** Coleção de prioridades para filtro */
        public ObservableCollection<string> Prioridades { get; set; } = new ObservableCollection<string> { "Todos", "Baixa", "Média", "Alta" };

        /** Coleção de estados do ticket para filtro */
        public ObservableCollection<string> EstadosTicket { get; set; } = new ObservableCollection<string> { "Todos", "porAtender", "emAtendimento", "atendido" };

        /** Coleção de estados de atendimento para filtro */
        public ObservableCollection<string> EstadosAtendimento { get; set; } = new ObservableCollection<string> { "Todos", "aberto", "naoResolvido", "resolvido" };

        /** Filtro aplicado ao tipo do ticket */
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

        /** Filtro aplicado à prioridade do ticket */
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

        /** Filtro aplicado ao estado do ticket */
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

        /** Filtro aplicado ao estado de atendimento */
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

        /** Ticket atualmente selecionado na interface */
        private Ticket _ticketSelecionado;
        public Ticket TicketSelecionado
        {
            get => _ticketSelecionado;
            set
            {
                _ticketSelecionado = value;
                OnPropertyChanged(nameof(TicketSelecionado));
                CommandManager.InvalidateRequerySuggested(); ///< Atualiza o estado do comando
            }
        }

        /** Comando para responder um ticket selecionado */
        public ICommand ResponderCommand { get; }

        /** Usuário técnico atualmente logado */
        private Utilizador _tecnicoLogado;

        /**
         * @brief Construtor que inicializa o ViewModel com o técnico logado.
         * 
         * @param tecnico Usuário técnico autenticado.
         */
        public ResponderTicketsViewModel(Utilizador tecnico)
        {
            _tecnicoLogado = tecnico;

            ResponderCommand = new RelayCommand(
     _ => ResponderTicket(),
     _ => TicketSelecionado != null
 );


            CarregarTickets();
        }

        /**
         * @brief Carrega todos os tickets do banco e aplica os filtros atuais.
         */
        public void CarregarTickets()
        {
            // Carrega todos os tickets que estão por atender para o técnico logado
            TodosOsTickets = TicketDAL.ObterTicketsPorAtender(null, null, null, null, _tecnicoLogado.Id).ToList();

            AplicarFiltros();
        }

        /**
         * @brief Aplica os filtros selecionados à lista completa de tickets e atualiza a coleção observável.
         */
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

        /**
         * @brief Abre a janela para responder ao ticket selecionado e atualiza a lista após resposta.
         */
        private void ResponderTicket()
        {
            if (TicketSelecionado == null) return;

            var janela = new ResponderTicketDetalhes(TicketSelecionado, _tecnicoLogado);
            janela.ShowDialog();

            CarregarTickets(); // Atualiza a lista após responder
        }

        /// Evento para notificação de mudanças nas propriedades
        public event PropertyChangedEventHandler PropertyChanged;

        /**
         * @brief Dispara o evento PropertyChanged para atualizar a interface.
         * @param nome Nome da propriedade que mudou.
         */
        protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
    }
}
