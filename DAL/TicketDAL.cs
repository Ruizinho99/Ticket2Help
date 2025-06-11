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
            INSERT INTO Ticket 
                (Tipo, SubtipoProblema, Prioridade, Descricao, EstadoTicket, EstadoAtendimento, DataCriacao, IdUtilizador)
            VALUES 
                (@Tipo, @SubtipoProblema, @Prioridade, @Descricao, @EstadoTicket, @EstadoAtendimento, @DataCriacao, @IdUtilizador)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Tipo", ticket.Tipo);
                cmd.Parameters.AddWithValue("@SubtipoProblema", ticket.SubtipoProblema);
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
        public static List<Ticket> ObterTicketsPorAtender(
      string tipoFiltro,
      string prioridadeFiltro,
      string estadoTicketFiltro,
      string estadoAtendimentoFiltro)
        {
            List<Ticket> tickets = new List<Ticket>();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"
SELECT T.Id, T.Tipo, T.SubtipoProblema, T.Prioridade, T.Descricao, T.EstadoTicket, 
       T.EstadoAtendimento, T.DataCriacao, T.DataAtendimento, 
       T.IdUtilizador, T.DetalhesTecnico, U.Nome 
FROM Ticket T
INNER JOIN Utilizador U ON T.IdUtilizador = U.Id
WHERE
    (@Tipo IS NULL OR T.Tipo = @Tipo)
    AND (@Prioridade IS NULL OR T.Prioridade = @Prioridade)
    AND (@EstadoTicket IS NULL OR T.EstadoTicket = @EstadoTicket)
    AND (@EstadoAtendimento IS NULL OR T.EstadoAtendimento = @EstadoAtendimento)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Tipo", string.IsNullOrEmpty(tipoFiltro) ? (object)DBNull.Value : tipoFiltro);
                    cmd.Parameters.AddWithValue("@Prioridade", string.IsNullOrEmpty(prioridadeFiltro) ? (object)DBNull.Value : prioridadeFiltro);
                    cmd.Parameters.AddWithValue("@EstadoTicket", string.IsNullOrEmpty(estadoTicketFiltro) ? (object)DBNull.Value : estadoTicketFiltro);
                    cmd.Parameters.AddWithValue("@EstadoAtendimento", string.IsNullOrEmpty(estadoAtendimentoFiltro) ? (object)DBNull.Value : estadoAtendimentoFiltro);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tickets.Add(new Ticket
                        {
                            Id = (int)reader["Id"],
                            Tipo = reader["Tipo"].ToString(),
                            SubtipoProblema = reader["SubtipoProblema"].ToString(),
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


        /*
        // Versão anterior — apenas tickets por atender
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
        WHERE 
            (T.EstadoAtendimento = 'aberto' OR T.EstadoAtendimento = 'naoResolvido')
            AND T.EstadoAtendimento <> 'resolvido'
            AND (T.EstadoTicket = 'porAtender' OR T.EstadoTicket = 'emAtendimento')
            AND T.EstadoTicket <> 'atendido'
            AND (@Tipo IS NULL OR T.Tipo = @Tipo)
            AND (@Prioridade IS NULL OR T.Prioridade = @Prioridade)";

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
        */


        public static List<Ticket> ObterTicketsParaResponder(int idUtilizador)
        {
            List<Ticket> tickets = new List<Ticket>();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"
            SELECT DISTINCT T.Id, T.Tipo, T.Prioridade, T.Descricao, T.EstadoTicket, 
                   T.EstadoAtendimento, T.DataCriacao, T.DataAtendimento, 
                   T.IdUtilizador, T.DetalhesTecnico, U.Nome 
            FROM Ticket T
            INNER JOIN Utilizador U ON T.IdUtilizador = U.Id
            WHERE T.IdUtilizador = @IdUtilizador
              AND (T.EstadoAtendimento = 'aberto' OR T.EstadoAtendimento = 'atendimento')
        ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdUtilizador", idUtilizador);

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


        public static void ResponderTicket(Ticket ticket)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"
UPDATE Ticket
SET EstadoTicket = @EstadoTicket,
    EstadoAtendimento = @EstadoAtendimento,
    DataAtendimento = @DataAtendimento,
    DetalhesTecnico = @DetalhesTecnico,
    RespondidoPor = @RespondidoPor
WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EstadoTicket", ticket.EstadoTicket);
                cmd.Parameters.AddWithValue("@EstadoAtendimento", ticket.EstadoAtendimento ?? (object)DBNull.Value);

                // Aqui verifica se DataAtendimento tem valor, senão passa DBNull.Value
                if (ticket.DataAtendimento.HasValue)
                    cmd.Parameters.AddWithValue("@DataAtendimento", ticket.DataAtendimento.Value);
                else
                    cmd.Parameters.AddWithValue("@DataAtendimento", DBNull.Value);

                cmd.Parameters.AddWithValue("@DetalhesTecnico", ticket.DetalhesTecnico ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RespondidoPor", ticket.RespondidoPor.HasValue ? (object)ticket.RespondidoPor.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", ticket.Id);

                cmd.ExecuteNonQuery();
            }
        }



        public static List<Ticket> ObterTicketsDoUtilizador(int idUtilizador)
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
            WHERE T.IdUtilizador = @IdUtilizador
            ORDER BY T.DataCriacao DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdUtilizador", idUtilizador);

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
