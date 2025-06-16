using BLL;
using DAL;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using UI.Views;
using UI.Helpers;
using System.Linq;
using System;

public class AdminTicketsViewModel : INotifyPropertyChanged
{
    private string _filtroTipo = "Todos";
    private string _filtroPrioridade = "Todos";
    private string _filtroEstadoTicket = "Todos";
    private string _filtroEstadoAtendimento = "Todos";

    public Action<Ticket> AbrirJanelaResponderTicket { get; set; }

    public string FiltroTipo
    {
        get => _filtroTipo;
        set
        {
            if (_filtroTipo != value)
            {
                _filtroTipo = value;
                OnPropertyChanged(nameof(FiltroTipo));
                AplicarFiltros();
            }
        }
    }

    public string FiltroPrioridade
    {
        get => _filtroPrioridade;
        set
        {
            if (_filtroPrioridade != value)
            {
                _filtroPrioridade = value;
                OnPropertyChanged(nameof(FiltroPrioridade));
                AplicarFiltros();
            }
        }
    }

    public string FiltroEstadoTicket
    {
        get => _filtroEstadoTicket;
        set
        {
            if (_filtroEstadoTicket != value)
            {
                _filtroEstadoTicket = value;
                OnPropertyChanged(nameof(FiltroEstadoTicket));
                AplicarFiltros();
            }
        }
    }

    public string FiltroEstadoAtendimento
    {
        get => _filtroEstadoAtendimento;
        set
        {
            if (_filtroEstadoAtendimento != value)
            {
                _filtroEstadoAtendimento = value;
                OnPropertyChanged(nameof(FiltroEstadoAtendimento));
                AplicarFiltros();
            }
        }
    }

    public ObservableCollection<Ticket> Tickets { get; set; } = new ObservableCollection<Ticket>();

    private Ticket _ticketSelecionado;
    public Ticket TicketSelecionado
    {
        get => _ticketSelecionado;
        set
        {
            if (_ticketSelecionado != value)
            {
                _ticketSelecionado = value;
                OnPropertyChanged(nameof(TicketSelecionado));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public ICommand ResponderCommand { get; }
    public ICommand AplicarFiltrosCommand { get; }

    private readonly Utilizador _tecnicoLogado;
    public Utilizador TecnicoLogado => _tecnicoLogado; // Expõe o técnico para usar na janela

    public AdminTicketsViewModel(Utilizador tecnicoLogado)
    {
        _tecnicoLogado = tecnicoLogado;

        ResponderCommand = new RelayCommand(
            executar: _ => AbrirJanelaResponderTicket?.Invoke(TicketSelecionado),
            podeExecutar: _ => TicketSelecionado != null
        );

        AplicarFiltrosCommand = new RelayCommand(
            executar: _ => AplicarFiltros(),
            podeExecutar: _ => true
        );

        AplicarFiltros();
    }

    private void AplicarFiltros()
    {
        // Converte "Todos" para null para passar ao DAL
        string tipo = FiltroTipo == "Todos" ? null : FiltroTipo;
        string prioridade = FiltroPrioridade == "Todos" ? null : FiltroPrioridade;
        string estadoTicket = FiltroEstadoTicket == "Todos" ? null : FiltroEstadoTicket;
        string estadoAtendimento = FiltroEstadoAtendimento == "Todos" ? null : FiltroEstadoAtendimento;

        var ticketsFiltrados = TicketDAL.ObterTicketsPorAtender(tipo, prioridade, estadoTicket, estadoAtendimento, _tecnicoLogado.Id);

        Tickets.Clear();
        foreach (var t in ticketsFiltrados)
            Tickets.Add(t);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
