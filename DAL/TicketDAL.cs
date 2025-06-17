using BLL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace DAL
{
    public class TicketDAL
    {
        /// <summary>
        /// Cria um novo ticket no banco de dados.
        /// </summary>
        /// <param name="ticket">Objeto Ticket contendo as informações a serem inseridas.</param>
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

        /// <summary>
        /// Obtém uma lista de tickets filtrados que ainda precisam de atendimento.
        /// </summary>
        /// <param name="tipoFiltro">Filtro pelo tipo de ticket.</param>
        /// <param name="prioridadeFiltro">Filtro pela prioridade do ticket.</param>
        /// <param name="estadoTicketFiltro">Filtro pelo estado do ticket.</param>
        /// <param name="estadoAtendimentoFiltro">Filtro pelo estado de atendimento.</param>
        /// <param name="tecnicoLogadoId">ID do técnico logado.</param>
        /// <param name="dataInicio">Data inicial para filtro (opcional).</param>
        /// <param name="dataFim">Data final para filtro (opcional).</param>
        /// <returns>Lista de tickets que correspondem aos filtros.</returns>
        /// 
        // Delegate público, que pode ser substituído para mocks nos testes
        public static Func<string, string, string, string, int, DateTime?, DateTime?, List<Ticket>> ObterTicketsPorAtenderFunc = ObterTicketsPorAtenderImpl;

        // Método público que o resto do sistema chama, e que repassa para o delegate
        public static List<Ticket> ObterTicketsPorAtender(
            string tipoFiltro,
            string prioridadeFiltro,
            string estadoTicketFiltro,
            string estadoAtendimentoFiltro,
            int tecnicoLogadoId,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            return ObterTicketsPorAtenderFunc(tipoFiltro, prioridadeFiltro, estadoTicketFiltro, estadoAtendimentoFiltro, tecnicoLogadoId, dataInicio, dataFim);
        }

        // Método privado com a implementação original da query
        private static List<Ticket> ObterTicketsPorAtenderImpl(
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



        /// <summary>
        /// Obtém uma lista de tickets para fins estatísticos com base nos filtros especificados.
        /// </summary>
        /// <param name="tipoFiltro">Filtro pelo tipo de ticket.</param>
        /// <param name="prioridadeFiltro">Filtro pela prioridade do ticket.</param>
        /// <param name="estadoTicketFiltro">Filtro pelo estado do ticket.</param>
        /// <param name="estadoAtendimentoFiltro">Filtro pelo estado de atendimento.</param>
        /// <param name="tecnicoLogadoId">ID do técnico logado.</param>
        /// <param name="dataInicio">Data inicial para filtro (opcional).</param>
        /// <param name="dataFim">Data final para filtro (opcional).</param>
        /// <returns>Lista de tickets que correspondem aos filtros para estatísticas.</returns>
        public static Func<string, string, string, string, int, DateTime?, DateTime?, List<Ticket>> ObterTicketsEstatisticasFunc
        = ObterTicketsPorAtender;

        public static List<Ticket> ObterTicketsEstatisticas(
            string tipoFiltro,
            string prioridadeFiltro,
            string estadoTicketFiltro,
            string estadoAtendimentoFiltro,
            int tecnicoLogadoId,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            // Chama o delegate, que por padrão aponta para ObterTicketsPorAtender
            return ObterTicketsEstatisticasFunc(tipoFiltro, prioridadeFiltro, estadoTicketFiltro, estadoAtendimentoFiltro, tecnicoLogadoId, dataInicio, dataFim);
        }

        /// <summary>
        /// Obtém todos os tickets de um utilizador que ainda precisam de resposta.
        /// </summary>
        /// <param name="idUtilizador">ID do utilizador.</param>
        /// <returns>Lista de tickets que precisam de resposta.</returns>
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
                  AND (T.EstadoAtendimento = 'aberto' OR T.EstadoAtendimento = 'atendimento')";

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

        /// <summary>
        /// Atualiza as informações de um ticket com a resposta do técnico.
        /// </summary>
        /// <param name="ticket">Objeto Ticket contendo as informações atualizadas.</param>
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
                cmd.Parameters.AddWithValue("@DataAtendimento", ticket.DataAtendimento.HasValue ? (object)ticket.DataAtendimento.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@DataConclusao", ticket.DataConclusao.HasValue ? (object)ticket.DataConclusao.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@DetalhesTecnico", ticket.DetalhesTecnico ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RespondidoPor", ticket.RespondidoPor.HasValue ? (object)ticket.RespondidoPor.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", ticket.Id);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtém todos os tickets de um utilizador.
        /// </summary>
        /// <param name="idUtilizador">ID do utilizador.</param>
        /// <returns>Lista de tickets do utilizador.</returns>
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

        /// <summary>
        /// Atualiza apenas o estado de atendimento de um ticket.
        /// </summary>
        /// <param name="ticket">Objeto Ticket com o estado atualizado.</param>
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
