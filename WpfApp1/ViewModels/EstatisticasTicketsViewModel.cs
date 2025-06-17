using BLL;
using DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

/// <summary>
/// Representa a média de tempo de atendimento por tipo de ticket.
/// </summary>
public class TempoAtendimentoPorTipo
{
    /// <summary>
    /// Tipo do ticket (ex: Hardware, Software).
    /// </summary>
    public string Tipo { get; set; }

    /// <summary>
    /// Média de tempo de atendimento para o tipo especificado.
    /// </summary>
    public TimeSpan MediaTempo { get; set; }

    /// <summary>
    /// Retorna a média de tempo formatada no formato "hh:mm:ss".
    /// </summary>
    public string MediaTempoFormatada => $"{(int)MediaTempo.TotalHours:D2}h {MediaTempo.Minutes:D2}m {MediaTempo.Seconds:D2}s";
}

/// <summary>
/// ViewModel responsável por calcular e exibir estatísticas de tickets.
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

    /// <summary>Filtro de tipo de ticket.</summary>
    public string TipoFiltro
    {
        get => tipoFiltro;
        set { tipoFiltro = value; OnPropertyChanged(nameof(TipoFiltro)); }
    }

    /// <summary>Filtro de prioridade do ticket.</summary>
    public string PrioridadeFiltro
    {
        get => prioridadeFiltro;
        set { prioridadeFiltro = value; OnPropertyChanged(nameof(PrioridadeFiltro)); }
    }

    /// <summary>Filtro de estado geral do ticket.</summary>
    public string EstadoTicketFiltro
    {
        get => estadoTicketFiltro;
        set { estadoTicketFiltro = value; OnPropertyChanged(nameof(EstadoTicketFiltro)); }
    }

    /// <summary>Filtro de estado de atendimento do ticket.</summary>
    public string EstadoAtendimentoFiltro
    {
        get => estadoAtendimentoFiltro;
        set { estadoAtendimentoFiltro = value; OnPropertyChanged(nameof(EstadoAtendimentoFiltro)); }
    }

    /// <summary>Data de início para o filtro por data.</summary>
    public DateTime? DataInicioFiltro
    {
        get => dataInicioFiltro;
        set { dataInicioFiltro = value; OnPropertyChanged(nameof(DataInicioFiltro)); }
    }

    /// <summary>Data de fim para o filtro por data.</summary>
    public DateTime? DataFimFiltro
    {
        get => dataFimFiltro;
        set { dataFimFiltro = value; OnPropertyChanged(nameof(DataFimFiltro)); }
    }

    /// <summary>Total de tickets carregados.</summary>
    public int TotalTickets { get; set; }

    /// <summary>Total de tickets atendidos.</summary>
    public int TicketsAtendidos { get; set; }

    /// <summary>Total de tickets resolvidos.</summary>
    public int TicketsResolvidos { get; set; }

    /// <summary>Total de tickets não resolvidos.</summary>
    public int TicketsNaoResolvidos { get; set; }

    /// <summary>Percentual de tickets respondidos em relação ao total.</summary>
    public string PercentualRespondidos => TotalTickets == 0 ? "0%" : $"{(TicketsAtendidos * 100.0 / TotalTickets):N2}%";

    /// <summary>Percentual de tickets resolvidos em relação aos atendidos.</summary>
    public string PercentualResolvidosVsAtendidos => TicketsAtendidos == 0 ? "0%" : $"{(TicketsResolvidos * 100.0 / TicketsAtendidos):N2}%";

    /// <summary>Percentual de tickets resolvidos em relação aos resolvidos e não resolvidos.</summary>
    public string PercentualResolvidosVsNaoResolvidos
    {
        get
        {
            int total = TicketsResolvidos + TicketsNaoResolvidos;
            return total == 0 ? "0%" : $"{(TicketsResolvidos * 100.0 / total):N2}%";
        }
    }

    /// <summary>Lista com o tempo médio de atendimento por tipo de ticket.</summary>
    public List<TempoAtendimentoPorTipo> MediaTempoAtendimentoPorTipoLista { get; set; }

    /// <summary>Comando para atualizar as estatísticas.</summary>
    public ICommand AtualizarCommand { get; }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="EstatisticasTicketsViewModel"/>.
    /// </summary>
    /// <param name="tecnicoId">ID do técnico logado.</param>
    /// <param name="tipoFiltro">Filtro de tipo de ticket.</param>
    /// <param name="prioridadeFiltro">Filtro de prioridade.</param>
    /// <param name="estadoTicketFiltro">Filtro de estado do ticket.</param>
    /// <param name="estadoAtendimentoFiltro">Filtro de estado de atendimento.</param>
    /// <param name="dataInicio">Data de início para filtro.</param>
    /// <param name="dataFim">Data de fim para filtro.</param>
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

        AtualizarCommand = new RelayCommand(_ => CarregarEstatisticas());
        CarregarEstatisticas();
    }

    /// <summary>Carrega e calcula as estatísticas dos tickets de acordo com os filtros definidos.</summary>
    public void CarregarEstatisticas()
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

    /// <inheritdoc/>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Notifica que uma propriedade foi alterada.</summary>
    /// <param name="nome">Nome da propriedade alterada.</param>
    protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}

/// <summary>
/// ViewModel responsável por carregar e gerenciar tickets para atendimento.
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

    /// <summary>Filtro de tipo de ticket.</summary>
    public string TipoFiltro
    {
        get => tipoFiltro;
        set { tipoFiltro = value; OnPropertyChanged(nameof(TipoFiltro)); }
    }

    /// <summary>Filtro de prioridade do ticket.</summary>
    public string PrioridadeFiltro
    {
        get => prioridadeFiltro;
        set { prioridadeFiltro = value; OnPropertyChanged(nameof(PrioridadeFiltro)); }
    }

    /// <summary>Filtro de estado geral do ticket.</summary>
    public string EstadoTicketFiltro
    {
        get => estadoTicketFiltro;
        set { estadoTicketFiltro = value; OnPropertyChanged(nameof(EstadoTicketFiltro)); }
    }

    /// <summary>Filtro de estado de atendimento do ticket.</summary>
    public string EstadoAtendimentoFiltro
    {
        get => estadoAtendimentoFiltro;
        set { estadoAtendimentoFiltro = value; OnPropertyChanged(nameof(EstadoAtendimentoFiltro)); }
    }

    /// <summary>Data de início para o filtro por data.</summary>
    public DateTime? DataInicioFiltro
    {
        get => dataInicioFiltro;
        set { dataInicioFiltro = value; OnPropertyChanged(nameof(DataInicioFiltro)); }
    }

    /// <summary>Data de fim para o filtro por data.</summary>
    public DateTime? DataFimFiltro
    {
        get => dataFimFiltro;
        set { dataFimFiltro = value; OnPropertyChanged(nameof(DataFimFiltro)); }
    }

    /// <summary>Lista de tickets filtrados para atendimento.</summary>
    public ObservableCollection<Ticket> Tickets { get; set; } = new ObservableCollection<Ticket>();

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ResponderTicketsViewModel"/>.
    /// </summary>
    /// <param name="utilizador">Utilizador técnico logado.</param>
    public ResponderTicketsViewModel(Utilizador utilizador)
    {
        tecnicoId = utilizador.Id;
        CarregarTickets();
    }

    /// <summary>Carrega os tickets de acordo com os filtros selecionados.</summary>
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

    /// <inheritdoc/>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Notifica que uma propriedade foi alterada.</summary>
    /// <param name="nome">Nome da propriedade alterada.</param>
    protected void OnPropertyChanged(string nome) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}

/// <summary>
/// Implementação básica de ICommand para manipular eventos de interface.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object> execute;
    private readonly Predicate<object> canExecute;

    /// <summary>
    /// Cria uma nova instância de <see cref="RelayCommand"/>.
    /// </summary>
    /// <param name="execute">Ação a ser executada.</param>
    /// <param name="canExecute">Função opcional que define se o comando pode ser executado.</param>
    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    /// <summary>Determina se o comando pode ser executado.</summary>
    /// <param name="parameter">Parâmetro do comando.</param>
    /// <returns>True se puder executar, caso contrário, false.</returns>
    public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

    /// <summary>Executa o comando.</summary>
    /// <param name="parameter">Parâmetro do comando.</param>
    public void Execute(object parameter) => execute(parameter);

    /// <inheritdoc/>
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
