using BLL;
using DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

// Classe para armazenar média de tempo por tipo de ticket
public class TempoAtendimentoPorTipo
{
    public string Tipo { get; set; }
    public TimeSpan MediaTempo { get; set; } // Tempo médio

    // Propriedade formatada para exibir tempo (hh:mm:ss)
    public string MediaTempoFormatada => $"{(int)MediaTempo.TotalHours:D2}h {MediaTempo.Minutes:D2}m {MediaTempo.Seconds:D2}s";
}

// ViewModel para estatísticas dos tickets
public class EstatisticasTicketsViewModel : INotifyPropertyChanged
{
    private int tecnicoLogadoId;

    // Filtros
    private string tipoFiltro = "Todos";
    private string prioridadeFiltro = "Todos";
    private string estadoTicketFiltro = "Todos";
    private string estadoAtendimentoFiltro = "Todos";
    private DateTime? dataInicioFiltro;
    private DateTime? dataFimFiltro;

    // Propriedades públicas com notificação
    public string TipoFiltro
    {
        get => tipoFiltro;
        set { tipoFiltro = value; OnPropertyChanged(nameof(TipoFiltro)); }
    }
    public string PrioridadeFiltro
    {
        get => prioridadeFiltro;
        set { prioridadeFiltro = value; OnPropertyChanged(nameof(PrioridadeFiltro)); }
    }
    public string EstadoTicketFiltro
    {
        get => estadoTicketFiltro;
        set { estadoTicketFiltro = value; OnPropertyChanged(nameof(EstadoTicketFiltro)); }
    }
    public string EstadoAtendimentoFiltro
    {
        get => estadoAtendimentoFiltro;
        set { estadoAtendimentoFiltro = value; OnPropertyChanged(nameof(EstadoAtendimentoFiltro)); }
    }
    public DateTime? DataInicioFiltro
    {
        get => dataInicioFiltro;
        set { dataInicioFiltro = value; OnPropertyChanged(nameof(DataInicioFiltro)); }
    }
    public DateTime? DataFimFiltro
    {
        get => dataFimFiltro;
        set { dataFimFiltro = value; OnPropertyChanged(nameof(DataFimFiltro)); }
    }

    // Estatísticas
    private int totalTickets;
    public int TotalTickets
    {
        get => totalTickets;
        set { totalTickets = value; OnPropertyChanged(nameof(TotalTickets)); }
    }

    private int ticketsAtendidos;
    public int TicketsAtendidos
    {
        get => ticketsAtendidos;
        set { ticketsAtendidos = value; OnPropertyChanged(nameof(TicketsAtendidos)); }
    }

    private int ticketsResolvidos;
    public int TicketsResolvidos
    {
        get => ticketsResolvidos;
        set { ticketsResolvidos = value; OnPropertyChanged(nameof(TicketsResolvidos)); }
    }

    private int ticketsNaoResolvidos;
    public int TicketsNaoResolvidos
    {
        get => ticketsNaoResolvidos;
        set { ticketsNaoResolvidos = value; OnPropertyChanged(nameof(TicketsNaoResolvidos)); }
    }

    // Percentuais formatados
    public string PercentualRespondidos => TotalTickets == 0 ? "0%" : $"{(TicketsAtendidos * 100.0 / TotalTickets):N2}%";
    public string PercentualResolvidosVsAtendidos => TicketsAtendidos == 0 ? "0%" : $"{(TicketsResolvidos * 100.0 / TicketsAtendidos):N2}%";
    public string PercentualResolvidosVsNaoResolvidos
    {
        get
        {
            int total = TicketsResolvidos + TicketsNaoResolvidos;
            return total == 0 ? "0%" : $"{(TicketsResolvidos * 100.0 / total):N2}%";
        }
    }

    public List<TempoAtendimentoPorTipo> MediaTempoAtendimentoPorTipoLista { get; set; }

    public EstatisticasTicketsViewModel(
        int tecnicoId,
        string tipoFiltro,
        string prioridadeFiltro,
        string estadoTicketFiltro,
        string estadoAtendimentoFiltro,
        DateTime? dataInicio = null,
        DateTime? dataFim = null)
    {
        tecnicoLogadoId = tecnicoId;

        TipoFiltro = tipoFiltro;
        PrioridadeFiltro = prioridadeFiltro;
        EstadoTicketFiltro = estadoTicketFiltro;
        EstadoAtendimentoFiltro = estadoAtendimentoFiltro;
        DataInicioFiltro = dataInicio;
        DataFimFiltro = dataFim;

        CarregarEstatisticas();
    }

    private void CarregarEstatisticas()
    {
        var tickets = TicketDAL.ObterTicketsEstatisticas(
            TipoFiltro, PrioridadeFiltro, EstadoTicketFiltro, EstadoAtendimentoFiltro,
            tecnicoLogadoId, DataInicioFiltro, DataFimFiltro);

        TotalTickets = tickets.Count;
        TicketsAtendidos = tickets.Count(t => t.EstadoTicket == "atendido");
        TicketsResolvidos = tickets.Count(t => t.EstadoAtendimento == "resolvido");
        TicketsNaoResolvidos = tickets.Count(t => t.EstadoAtendimento == "naoResolvido");

        MediaTempoAtendimentoPorTipoLista = tickets
            .Where(t => t.DataAtendimento.HasValue && t.DataConclusao.HasValue)
            .GroupBy(t => t.Tipo)
            .Select(g =>
            {
                var mediaSegundos = g.Average(t => (t.DataConclusao.Value - t.DataAtendimento.Value).TotalSeconds);
                return new TempoAtendimentoPorTipo
                {
                    Tipo = g.Key,
                    MediaTempo = TimeSpan.FromSeconds(mediaSegundos)
                };
            })
            .ToList();

        OnPropertyChanged(nameof(MediaTempoAtendimentoPorTipoLista));
        OnPropertyChanged(nameof(TotalTickets));
        OnPropertyChanged(nameof(TicketsAtendidos));
        OnPropertyChanged(nameof(TicketsResolvidos));
        OnPropertyChanged(nameof(TicketsNaoResolvidos));
        OnPropertyChanged(nameof(PercentualRespondidos));
        OnPropertyChanged(nameof(PercentualResolvidosVsAtendidos));
        OnPropertyChanged(nameof(PercentualResolvidosVsNaoResolvidos));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}


// ViewModel para responder tickets
public class ResponderTicketsViewModel : INotifyPropertyChanged
{
    private int tecnicoId;

    // Filtros
    private string tipoFiltro = "Todos";
    private string prioridadeFiltro = "Todos";
    private string estadoTicketFiltro = "Todos";
    private string estadoAtendimentoFiltro = "Todos";
    private DateTime? dataInicioFiltro;
    private DateTime? dataFimFiltro;

    public string TipoFiltro
    {
        get => tipoFiltro;
        set { tipoFiltro = value; OnPropertyChanged(nameof(TipoFiltro)); }
    }
    public string PrioridadeFiltro
    {
        get => prioridadeFiltro;
        set { prioridadeFiltro = value; OnPropertyChanged(nameof(PrioridadeFiltro)); }
    }
    public string EstadoTicketFiltro
    {
        get => estadoTicketFiltro;
        set { estadoTicketFiltro = value; OnPropertyChanged(nameof(EstadoTicketFiltro)); }
    }
    public string EstadoAtendimentoFiltro
    {
        get => estadoAtendimentoFiltro;
        set { estadoAtendimentoFiltro = value; OnPropertyChanged(nameof(EstadoAtendimentoFiltro)); }
    }
    public DateTime? DataInicioFiltro
    {
        get => dataInicioFiltro;
        set { dataInicioFiltro = value; OnPropertyChanged(nameof(DataInicioFiltro)); }
    }
    public DateTime? DataFimFiltro
    {
        get => dataFimFiltro;
        set { dataFimFiltro = value; OnPropertyChanged(nameof(DataFimFiltro)); }
    }

    public ObservableCollection<Ticket> Tickets { get; set; } = new ObservableCollection<Ticket>();

    public ResponderTicketsViewModel(Utilizador utilizador)
    {
        tecnicoId = utilizador.Id;
        CarregarTickets();
    }

    public void CarregarTickets()
    {
        var tickets = TicketDAL.ObterTicketsEstatisticas(
            TipoFiltro,
            PrioridadeFiltro,
            EstadoTicketFiltro,
            EstadoAtendimentoFiltro,
            tecnicoId,
            DataInicioFiltro,
            DataFimFiltro);

        Tickets.Clear();
        foreach (var t in tickets)
            Tickets.Add(t);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}
