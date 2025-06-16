using BLL;
using DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

/// <summary>
/// Classe que representa o tempo médio de atendimento por tipo de ticket.
/// </summary>
public class TempoAtendimentoPorTipo
{
    /// <summary>
    /// Tipo do ticket.
    /// </summary>
    public string Tipo { get; set; }

    /// <summary>
    /// Tempo médio de atendimento.
    /// </summary>
    public TimeSpan MediaTempo { get; set; }

    /// <summary>
    /// Tempo médio de atendimento formatado no padrão hh:mm:ss.
    /// </summary>
    public string MediaTempoFormatada => $"{(int)MediaTempo.TotalHours:D2}h {MediaTempo.Minutes:D2}m {MediaTempo.Seconds:D2}s";
}

/// <summary>
/// ViewModel responsável por calcular e armazenar as estatísticas dos tickets.
/// </summary>
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

    /// <summary>
    /// Filtro pelo tipo de ticket.
    /// </summary>
    public string TipoFiltro
    {
        get => tipoFiltro;
        set { tipoFiltro = value; OnPropertyChanged(nameof(TipoFiltro)); }
    }

    /// <summary>
    /// Filtro pela prioridade do ticket.
    /// </summary>
    public string PrioridadeFiltro
    {
        get => prioridadeFiltro;
        set { prioridadeFiltro = value; OnPropertyChanged(nameof(PrioridadeFiltro)); }
    }

    /// <summary>
    /// Filtro pelo estado do ticket.
    /// </summary>
    public string EstadoTicketFiltro
    {
        get => estadoTicketFiltro;
        set { estadoTicketFiltro = value; OnPropertyChanged(nameof(EstadoTicketFiltro)); }
    }

    /// <summary>
    /// Filtro pelo estado de atendimento.
    /// </summary>
    public string EstadoAtendimentoFiltro
    {
        get => estadoAtendimentoFiltro;
        set { estadoAtendimentoFiltro = value; OnPropertyChanged(nameof(EstadoAtendimentoFiltro)); }
    }

    /// <summary>
    /// Filtro pela data de início.
    /// </summary>
    public DateTime? DataInicioFiltro
    {
        get => dataInicioFiltro;
        set { dataInicioFiltro = value; OnPropertyChanged(nameof(DataInicioFiltro)); }
    }

    /// <summary>
    /// Filtro pela data de fim.
    /// </summary>
    public DateTime? DataFimFiltro
    {
        get => dataFimFiltro;
        set { dataFimFiltro = value; OnPropertyChanged(nameof(DataFimFiltro)); }
    }

    // Estatísticas gerais
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

    /// <summary>
    /// Percentual de tickets respondidos.
    /// </summary>
    public string PercentualRespondidos => TotalTickets == 0 ? "0%" : $"{(TicketsAtendidos * 100.0 / TotalTickets):N2}%";

    /// <summary>
    /// Percentual de tickets resolvidos em relação aos atendidos.
    /// </summary>
    public string PercentualResolvidosVsAtendidos => TicketsAtendidos == 0 ? "0%" : $"{(TicketsResolvidos * 100.0 / TicketsAtendidos):N2}%";

    /// <summary>
    /// Percentual de tickets resolvidos em relação aos não resolvidos.
    /// </summary>
    public string PercentualResolvidosVsNaoResolvidos
    {
        get
        {
            int total = TicketsResolvidos + TicketsNaoResolvidos;
            return total == 0 ? "0%" : $"{(TicketsResolvidos * 100.0 / total):N2}%";
        }
    }

    /// <summary>
    /// Lista de tempos médios de atendimento por tipo de ticket.
    /// </summary>
    public List<TempoAtendimentoPorTipo> MediaTempoAtendimentoPorTipoLista { get; set; }

    /// <summary>
    /// Construtor da ViewModel das estatísticas de tickets.
    /// </summary>
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

    /// <summary>
    /// Carrega e calcula todas as estatísticas com base nos filtros aplicados.
    /// </summary>
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

        // Atualiza as propriedades
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

    /// <summary>
    /// Dispara o evento de mudança de propriedade.
    /// </summary>
    /// <param name="nome">Nome da propriedade alterada.</param>
    protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}

/// <summary>
/// ViewModel responsável por carregar os tickets que o técnico pode responder.
/// </summary>
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

    /// <summary>
    /// Filtro pelo tipo de ticket.
    /// </summary>
    public string TipoFiltro
    {
        get => tipoFiltro;
        set { tipoFiltro = value; OnPropertyChanged(nameof(TipoFiltro)); }
    }

    /// <summary>
    /// Filtro pela prioridade do ticket.
    /// </summary>
    public string PrioridadeFiltro
    {
        get => prioridadeFiltro;
        set { prioridadeFiltro = value; OnPropertyChanged(nameof(PrioridadeFiltro)); }
    }

    /// <summary>
    /// Filtro pelo estado do ticket.
    /// </summary>
    public string EstadoTicketFiltro
    {
        get => estadoTicketFiltro;
        set { estadoTicketFiltro = value; OnPropertyChanged(nameof(EstadoTicketFiltro)); }
    }

    /// <summary>
    /// Filtro pelo estado de atendimento.
    /// </summary>
    public string EstadoAtendimentoFiltro
    {
        get => estadoAtendimentoFiltro;
        set { estadoAtendimentoFiltro = value; OnPropertyChanged(nameof(EstadoAtendimentoFiltro)); }
    }

    /// <summary>
    /// Filtro pela data de início.
    /// </summary>
    public DateTime? DataInicioFiltro
    {
        get => dataInicioFiltro;
        set { dataInicioFiltro = value; OnPropertyChanged(nameof(DataInicioFiltro)); }
    }

    /// <summary>
    /// Filtro pela data de fim.
    /// </summary>
    public DateTime? DataFimFiltro
    {
        get => dataFimFiltro;
        set { dataFimFiltro = value; OnPropertyChanged(nameof(DataFimFiltro)); }
    }

    /// <summary>
    /// Lista de tickets carregados.
    /// </summary>
    public ObservableCollection<Ticket> Tickets { get; set; } = new ObservableCollection<Ticket>();

    /// <summary>
    /// Construtor da ViewModel de resposta de tickets.
    /// </summary>
    /// <param name="utilizador">Utilizador técnico logado.</param>
    public ResponderTicketsViewModel(Utilizador utilizador)
    {
        tecnicoId = utilizador.Id;
        CarregarTickets();
    }

    /// <summary>
    /// Carrega os tickets com base nos filtros aplicados.
    /// </summary>
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

    /// <summary>
    /// Dispara o evento de mudança de propriedade.
    /// </summary>
    /// <param name="nome">Nome da propriedade alterada.</param>
    protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}
