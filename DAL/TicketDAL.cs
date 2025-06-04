using BLL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class TicketDAL
    {
        public static void CriarTicket(Ticket ticket)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = @"
            INSERT INTO Ticket (Tipo, Prioridade, Descricao, EstadoTicket, EstadoAtendimento, DataCriacao, IdUtilizador)
            VALUES (@Tipo, @Prioridade, @Descricao, @EstadoTicket, @EstadoAtendimento, @DataCriacao, @IdUtilizador)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Tipo", ticket.Tipo);
                cmd.Parameters.AddWithValue("@Prioridade", ticket.Prioridade);
                cmd.Parameters.AddWithValue("@Descricao", ticket.Descricao);
                cmd.Parameters.AddWithValue("@EstadoTicket", "Aberto");
                cmd.Parameters.AddWithValue("@EstadoAtendimento", "Pendente");
                cmd.Parameters.AddWithValue("@DataCriacao", DateTime.Now);
                cmd.Parameters.AddWithValue("@IdUtilizador", ticket.IdUtilizador);


                cmd.ExecuteNonQuery();
            }
        }
    }
}
