using System;

namespace BLL
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string SubtipoProblema { get; set; }
        public string Prioridade { get; set; }
        public string Descricao { get; set; }
        public string EstadoTicket { get; set; }
        public string EstadoAtendimento { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtendimento { get; set; }
        public int IdUtilizador { get; set; }             // quem criou
        public int? RespondidoPor { get; set; }           // quem respondeu (técnico/admin)
        public string DetalhesTecnico { get; set; }
        public string NomeFuncionario { get; set; }       // nome do criador, opcional para mostrar na UI
    }
}

