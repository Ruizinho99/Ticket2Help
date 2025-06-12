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
                cmd.Parameters.AddWithValue("@EstadoAtendimento", "Aberto");
                cmd.Parameters.AddWithValue("@DataCriacao", DateTime.Now);
                cmd.Parameters.AddWithValue("@IdUtilizador", ticket.IdUtilizador);

                cmd.ExecuteNonQuery();
            }
        }

        public static List<Ticket> ObterTicketsPorAtender(
       string tipoFiltro,
       string prioridadeFiltro,
       string estadoTicketFiltro,
       string estadoAtendimentoFiltro,
       int tecnicoLogadoId,
       DateTime? dataInicio = null,
       DateTime? dataFim = null)
        {
            List<Ticket> tickets = new List<Ticket>();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"
        SELECT T.Id, T.Tipo, T.SubtipoProblema, T.Prioridade, T.Descricao, T.EstadoTicket, 
               T.EstadoAtendimento, T.DataCriacao, T.DataAtendimento, T.DataConclusao,
               T.IdUtilizador, T.DetalhesTecnico, U.Nome, T.RespondidoPor
        FROM Ticket T
        INNER JOIN Utilizador U ON T.IdUtilizador = U.Id
        WHERE
            (@Tipo IS NULL OR T.Tipo = @Tipo)
            AND (@Prioridade IS NULL OR T.Prioridade = @Prioridade)
            AND (@EstadoTicket IS NULL OR T.EstadoTicket = @EstadoTicket)
            AND (@EstadoAtendimento IS NULL OR T.EstadoAtendimento = @EstadoAtendimento)
            AND (T.RespondidoPor IS NULL OR T.RespondidoPor = @TecnicoLogadoId)
            AND (@DataInicio IS NULL OR T.DataCriacao >= @DataInicio)
            AND (@DataFim IS NULL OR T.DataCriacao <= @DataFim)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Tipo", string.IsNullOrEmpty(tipoFiltro) || tipoFiltro == "Todos" ? (object)DBNull.Value : tipoFiltro);
                    cmd.Parameters.AddWithValue("@Prioridade", string.IsNullOrEmpty(prioridadeFiltro) || prioridadeFiltro == "Todos" ? (object)DBNull.Value : prioridadeFiltro);
                    cmd.Parameters.AddWithValue("@EstadoTicket", string.IsNullOrEmpty(estadoTicketFiltro) || estadoTicketFiltro == "Todos" ? (object)DBNull.Value : estadoTicketFiltro);
                    cmd.Parameters.AddWithValue("@EstadoAtendimento", string.IsNullOrEmpty(estadoAtendimentoFiltro) || estadoAtendimentoFiltro == "Todos" ? (object)DBNull.Value : estadoAtendimentoFiltro);
                    cmd.Parameters.AddWithValue("@TecnicoLogadoId", tecnicoLogadoId);

                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio.HasValue ? (object)dataInicio.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@DataFim", dataFim.HasValue ? (object)dataFim.Value : DBNull.Value);

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
                            DataConclusao = reader["DataConclusao"] == DBNull.Value ? null : (DateTime?)reader["DataConclusao"],
                            IdUtilizador = (int)reader["IdUtilizador"],
                            DetalhesTecnico = reader["DetalhesTecnico"].ToString(),
                            NomeFuncionario = reader["Nome"].ToString(),
                            RespondidoPor = reader["RespondidoPor"] == DBNull.Value ? (int?)null : (int?)reader["RespondidoPor"]
                        });
                    }
                }
            }

            return tickets;
        }

        public static List<Ticket> ObterTicketsEstatisticas(
   string tipoFiltro,
   string prioridadeFiltro,
   string estadoTicketFiltro,
   string estadoAtendimentoFiltro,
   int tecnicoLogadoId,
   DateTime? dataInicio = null,
   DateTime? dataFim = null)
        {
            List<Ticket> tickets = new List<Ticket>();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"
            SELECT T.Id, T.Tipo, T.SubtipoProblema, T.Prioridade, T.Descricao, T.EstadoTicket, 
                   T.EstadoAtendimento, T.DataCriacao, T.DataAtendimento, T.DataConclusao,
                   T.IdUtilizador, T.DetalhesTecnico, U.Nome, T.RespondidoPor
            FROM Ticket T
            INNER JOIN Utilizador U ON T.IdUtilizador = U.Id
            WHERE
                (@Tipo IS NULL OR T.Tipo = @Tipo)
                AND (@Prioridade IS NULL OR T.Prioridade = @Prioridade)
                AND (@EstadoTicket IS NULL OR T.EstadoTicket = @EstadoTicket)
                AND (@EstadoAtendimento IS NULL OR T.EstadoAtendimento = @EstadoAtendimento)
                AND (T.RespondidoPor IS NULL OR T.RespondidoPor = @TecnicoLogadoId)
                AND (@DataInicio IS NULL OR T.DataCriacao >= @DataInicio)
                AND (@DataFim IS NULL OR T.DataCriacao <= @DataFim)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Tipo", string.IsNullOrEmpty(tipoFiltro) || tipoFiltro == "Todos" ? (object)DBNull.Value : tipoFiltro);
                    cmd.Parameters.AddWithValue("@Prioridade", string.IsNullOrEmpty(prioridadeFiltro) || prioridadeFiltro == "Todos" ? (object)DBNull.Value : prioridadeFiltro);
                    cmd.Parameters.AddWithValue("@EstadoTicket", string.IsNullOrEmpty(estadoTicketFiltro) || estadoTicketFiltro == "Todos" ? (object)DBNull.Value : estadoTicketFiltro);
                    cmd.Parameters.AddWithValue("@EstadoAtendimento", string.IsNullOrEmpty(estadoAtendimentoFiltro) || estadoAtendimentoFiltro == "Todos" ? (object)DBNull.Value : estadoAtendimentoFiltro);
                    cmd.Parameters.AddWithValue("@TecnicoLogadoId", tecnicoLogadoId);

                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio.HasValue ? (object)dataInicio.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@DataFim", dataFim.HasValue ? (object)dataFim.Value : DBNull.Value);

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
                            DataConclusao = reader["DataConclusao"] == DBNull.Value ? null : (DateTime?)reader["DataConclusao"],
                            IdUtilizador = (int)reader["IdUtilizador"],
                            DetalhesTecnico = reader["DetalhesTecnico"].ToString(),
                            NomeFuncionario = reader["Nome"].ToString(),
                            RespondidoPor = reader["RespondidoPor"] == DBNull.Value ? (int?)null : (int?)reader["RespondidoPor"]
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
    DataConclusao = @DataConclusao,
    DetalhesTecnico = @DetalhesTecnico,
    RespondidoPor = @RespondidoPor
WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EstadoTicket", ticket.EstadoTicket);
                cmd.Parameters.AddWithValue("@EstadoAtendimento", ticket.EstadoAtendimento ?? (object)DBNull.Value);

                if (ticket.DataAtendimento.HasValue)
                    cmd.Parameters.AddWithValue("@DataAtendimento", ticket.DataAtendimento.Value);
                else
                    cmd.Parameters.AddWithValue("@DataAtendimento", DBNull.Value);

                if (ticket.DataConclusao.HasValue)
                    cmd.Parameters.AddWithValue("@DataConclusao", ticket.DataConclusao.Value);
                else
                    cmd.Parameters.AddWithValue("@DataConclusao", DBNull.Value);

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
           SELECT T.Id, T.Tipo, T.SubtipoProblema, T.Prioridade, T.Descricao, 
       T.EstadoTicket, T.EstadoAtendimento, T.DataCriacao, 
       T.DataAtendimento, T.DataConclusao, T.IdUtilizador, 
       T.RespondidoPor, T.DetalhesTecnico, U.Nome
 
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
                            SubtipoProblema = reader["SubtipoProblema"].ToString(),
                            Prioridade = reader["Prioridade"].ToString(),
                            Descricao = reader["Descricao"].ToString(),
                            EstadoTicket = reader["EstadoTicket"].ToString(),
                            EstadoAtendimento = reader["EstadoAtendimento"].ToString(),
                            DataCriacao = (DateTime)reader["DataCriacao"],
                            DataAtendimento = reader["DataAtendimento"] == DBNull.Value ? null : (DateTime?)reader["DataAtendimento"],
                            DataConclusao = reader["DataConclusao"] == DBNull.Value ? null : (DateTime?)reader["DataConclusao"],
                            IdUtilizador = (int)reader["IdUtilizador"],
                            RespondidoPor = reader["RespondidoPor"] == DBNull.Value ? null : (int?)reader["RespondidoPor"],
                            DetalhesTecnico = reader["DetalhesTecnico"].ToString(),
                            NomeFuncionario = reader["Nome"].ToString()
                        });

                    }
                }
            }

            return tickets;
        }


        public static void AtualizarTicket(Ticket ticket)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string query = @"
            UPDATE Ticket
            SET EstadoAtendimento = @EstadoAtendimento
            WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EstadoAtendimento", ticket.EstadoAtendimento ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", ticket.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }



    }





}
