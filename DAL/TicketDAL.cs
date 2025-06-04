using BLL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

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
                cmd.Parameters.AddWithValue("@EstadoTicket", "porAtender");
                cmd.Parameters.AddWithValue("@EstadoAtendimento", "aberto");
                cmd.Parameters.AddWithValue("@DataCriacao", DateTime.Now);
                cmd.Parameters.AddWithValue("@IdUtilizador", ticket.IdUtilizador);

                cmd.ExecuteNonQuery();
            }
        }

        // 👇 Nova função para responder ao pedido da UI
        public static List<Ticket> ObterTicketsPorAtender(string tipoFiltro, string prioridadeFiltro)
        {
            List<Ticket> tickets = new List<Ticket>();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"
                    SELECT T.Id, T.Tipo, T.Prioridade, T.Descricao, T.EstadoTicket, 
                           T.EstadoAtendimento, T.DataCriacao, T.DataAtendimento, 
                           T.IdUtilizador, T.DetalhesTecnico, U.Nome 
                    FROM Ticket T
                    INNER JOIN Utilizador U ON T.IdUtilizador = U.Id
                    WHERE T.EstadoTicket = 'porAtender'
                      AND (@Tipo IS NULL OR T.Tipo = @Tipo)
                      AND (@Prioridade IS NULL OR T.Prioridade = @Prioridade)
                ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Tipo", string.IsNullOrEmpty(tipoFiltro) ? (object)DBNull.Value : tipoFiltro);
                    cmd.Parameters.AddWithValue("@Prioridade", string.IsNullOrEmpty(prioridadeFiltro) ? (object)DBNull.Value : prioridadeFiltro);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tickets.Add(new Ticket
                        {
                            Id = (int)reader["Id"],
                            Tipo = reader["Tipo"].ToString(),
                            Prioridade = reader["Prioridade"].ToString(),
                            Descricao = reader["Descricao"].ToString(),
                            EstadoTicket = reader["EstadoTicket"].ToString(),
                            EstadoAtendimento = reader["EstadoAtendimento"].ToString(),
                            DataCriacao = (DateTime)reader["DataCriacao"],
                            DataAtendimento = reader["DataAtendimento"] == DBNull.Value ? null : (DateTime?)reader["DataAtendimento"],
                            IdUtilizador = (int)reader["IdUtilizador"],
                            DetalhesTecnico = reader["DetalhesTecnico"].ToString(),
                            NomeFuncionario = reader["Nome"].ToString()
                        });
                    }
                }
            }

            return tickets;
        }
    }
}
