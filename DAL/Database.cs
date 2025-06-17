using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

namespace DAL
{
    /// <summary>
    /// Classe estática responsável por fornecer conexões com a base de dados SQL Server.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// String de conexão para acesso à base de dados.
        /// </summary>
        private static string connectionString = @"Server=localhost\SQLEXPRESS;Database=pa_project;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False";

        /// <summary>
        /// Obtém uma nova instância de <see cref="SqlConnection"/> com a string de conexão configurada.
        /// </summary>
        /// <returns>Uma nova conexão aberta com a base de dados.</returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
