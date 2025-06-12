using Microsoft.Data.SqlClient;
using System;
using BLL;

namespace DAL
{
    public class UtilizadorDAL
    {
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
                        break; // Já temos os dois exemplos
                }
            }

            return (itDesk, funcionario);
        }

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
