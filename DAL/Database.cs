using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

namespace DAL
{
    public static class Database
    {
        private static string connectionString = @"Server=localhost\SQLEXPRESS;Database=pa_project;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False";
    


        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
