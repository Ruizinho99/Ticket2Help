using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
// Apenas o namespace correto da classe Utilizador
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
                        Tipo = reader["Tipo"].ToString()
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
                string query = "SELECT TOP 2 * FROM Utilizador";
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


    }
}