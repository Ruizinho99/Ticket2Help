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
                string query = "SELECT TOP 1 * FROM Utilizador WHERE username = @username";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", "ISLA-5228");

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
