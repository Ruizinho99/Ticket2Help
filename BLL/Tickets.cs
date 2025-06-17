using System;

namespace BLL
{
    /// <summary>
    /// Representa um ticket de suporte no sistema.
    /// Contém informações sobre o tipo, prioridade, estado e datas de atendimento.
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Identificador único do ticket.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tipo geral do ticket (ex: Hardware, Software, etc.).
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Subtipo específico do problema relatado.
        /// </summary>
        public string SubtipoProblema { get; set; }

        /// <summary>
        /// Prioridade do ticket (ex: Alta, Média, Baixa).
        /// </summary>
        public string Prioridade { get; set; }

        /// <summary>
        /// Descrição detalhada do problema relatado pelo utilizador.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Estado geral do ticket (ex: porAtender, emAtendimento, atendido).
        /// </summary>
        public string EstadoTicket { get; set; }

        /// <summary>
        /// Estado de atendimento do ticket (ex: aberto, resolvido, naoResolvido).
        /// </summary>
        public string EstadoAtendimento { get; set; }

        /// <summary>
        /// Data e hora de criação do ticket.
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Data e hora de início do atendimento ao ticket.
        /// Pode ser nulo caso o atendimento ainda não tenha começado.
        /// </summary>
        public DateTime? DataAtendimento { get; set; }

        /// <summary>
        /// Data e hora de conclusão do atendimento ao ticket.
        /// Pode ser nulo caso o ticket ainda não tenha sido concluído.
        /// </summary>
        public DateTime? DataConclusao { get; set; }

        /// <summary>
        /// Identificador do utilizador que criou o ticket.
        /// </summary>
        public int IdUtilizador { get; set; }

        /// <summary>
        /// Identificador do técnico ou administrador que respondeu ao ticket.
        /// Pode ser nulo caso ainda não tenha sido respondido.
        /// </summary>
        public int? RespondidoPor { get; set; }

        /// <summary>
        /// Detalhes técnicos adicionados pelo técnico responsável durante o atendimento.
        /// </summary>
        public string DetalhesTecnico { get; set; }

        /// <summary>
        /// Nome do funcionário que criou o ticket.
        /// Campo opcional, útil para exibição na interface do utilizador.
        /// </summary>
        public string NomeFuncionario { get; set; }
    }
}
