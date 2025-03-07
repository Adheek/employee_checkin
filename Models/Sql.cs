using System;
using Microsoft.Data.SqlClient;  // ✅ Use this instead of System.Data.SqlClient
using Microsoft.Extensions.Configuration;

namespace AEET.Models
{
    public class Sql
    {
        private readonly string? _connectionString;

        public Sql(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    Console.WriteLine("✅ SQL Connection Successful!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SQL Connection Failed: {ex.Message}");
            }
        }
    }
}
