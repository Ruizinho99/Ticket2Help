using Microsoft.Data.SqlClient;
using System;
using BLL;

namespace DAL
{
    /// <summary>
    /// Classe responsável pelas operações de acesso a dados relacionadas aos utilizadores.
    /// </summary>
    public class UtilizadorDAL
    {
        /// <summary>
        /// Autentica um utilizador com base no username e password.
        /// </summary>
        /// <param name="username">Nome de utilizador fornecido.</param>
        /// <param name="password">Password fornecida.</param>
        /// <returns>Objeto Utilizador autenticado ou null se não for encontrado.</returns>
        public static Utilizador Login(string username, string password)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Utilizador WHERE Username = @u AND Password = @p";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Utilizador
                    {
                        Id = (int)reader["Id"],
                        Nome = reader["Nome"].ToString(),
                        Username = reader["Username"].ToString(),
                        Tipo = reader["Tipo"].ToString(),
                        Password = reader["Password"].ToString()
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Obtém o primeiro utilizador da tabela Utilizador.
        /// </summary>
        /// <returns>Objeto Utilizador encontrado ou null se não existir nenhum.</returns>
        public static Utilizador GetPrimeiroUtilizador()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = "SELECT TOP 1 * FROM Utilizador";

                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Utilizador
                    {
                        Id = (int)reader["Id"],
                        Nome = reader["Nome"].ToString(),
                        Username = reader["Username"].ToString(),
                        Tipo = reader["Tipo"].ToString(),
                        Password = reader["Password"].ToString()
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Obtém dois utilizadores de exemplo: um com perfil IT_DESK e outro com perfil Funcionario.
        /// </summary>
        /// <returns>Uma tupla contendo os utilizadores IT_DESK e Funcionario encontrados, ou null se não existirem.</returns>
        public static (Utilizador itDesk, Utilizador funcionario) GetUtilizadoresExemplo()
        {
            Utilizador itDesk = null;
            Utilizador funcionario = null;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Utilizador WHERE Tipo IN ('IT_DESK', 'Funcionario')";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string tipo = reader["Tipo"].ToString();
                    if (tipo == "IT_DESK" && itDesk == null)
                    {
                        itDesk = new Utilizador
                        {
                            Id = (int)reader["Id"],
                            Nome = reader["Nome"].ToString(),
                            Username = reader["Username"].ToString(),
                            Tipo = tipo,
                            Password = reader["Password"].ToString()
                        };
                    }
                    else if (tipo == "Funcionario" && funcionario == null)
                    {
                        funcionario = new Utilizador
                        {
                            Id = (int)reader["Id"],
                            Nome = reader["Nome"].ToString(),
                            Username = reader["Username"].ToString(),
                            Tipo = tipo,
                            Password = reader["Password"].ToString()
                        };
                    }

                    if (itDesk != null && funcionario != null)
                        break; // Já encontrou os dois utilizadores necessários
                }
            }

            return (itDesk, funcionario);
        }

        /// <summary>
        /// Obtém um utilizador com base no seu ID.
        /// </summary>
        /// <param name="id">ID do utilizador.</param>
        /// <returns>Objeto Utilizador correspondente ao ID ou null se não existir.</returns>
        public static Utilizador ObterUtilizadorPorId(int id)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Utilizador WHERE Id = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Utilizador
                    {
                        Id = (int)reader["Id"],
                        Nome = reader["Nome"].ToString(),
                        Username = reader["Username"].ToString(),
                        Tipo = reader["Tipo"].ToString(),
                        Password = reader["Password"].ToString()
                    };
                }
            }

            return null;
        }
    }
}
