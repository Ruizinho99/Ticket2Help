using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Ticket
    {
        public int Id { get; set; }
        public int IdUtilizador { get; set; } 
        public string Tipo { get; set; }
        public string Prioridade { get; set; }
        public string Descricao { get; set; }
        public string EstadoTicket { get; set; }
        public string EstadoAtendimento { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtendimento { get; set; }
        public int UtilizadorId { get; set; }
        public string DetalhesTecnico { get; set; }

        public string NomeFuncionario { get; set; }
    }

}
