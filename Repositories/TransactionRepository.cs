using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient; // Required for SqlConnection
using MCT.Functions.Models;       // Required for CoffeeTransaction

namespace MCT.Functions.Repositories
{
    public class TransactionRepository
    {
        private readonly string? _connectionString;

        public TransactionRepository()
        {
            // Reads the connection string you set in local.settings.json
            _connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
        }

        public async Task<List<CoffeeTransaction>> GetAllTransactionsAsync()
        {
            var transactions = new List<CoffeeTransaction>();

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("SqlConnectionString is missing in settings.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // The query from your screenshot
                string query = "SELECT ID, DateTime, CashType, Card, Money, CoffeeName FROM Transactions";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Create a new object for each row
                                var transaction = new CoffeeTransaction
                                {
                                    // Careful: GetOrdinal matches the column name to its index
                                    ID = reader.GetGuid(reader.GetOrdinal("ID")),
                                    DateTime = reader.GetDateTime(reader.GetOrdinal("DateTime")),
                                    
                                    // The screenshot uses this specific way to cast strings safely
                                    CashType = reader["CashType"] as string,
                                    Card = reader["Card"] as string,
                                    
                                    Money = reader.GetDecimal(reader.GetOrdinal("Money")),
                                    CoffeeName = reader["CoffeeName"] as string
                                };

                                transactions.Add(transaction);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // The screenshot shows re-throwing the exception to be handled by the trigger
                        throw; 
                    }
                }
            }

            return transactions;
        }
        public async Task<List<string>> GetCoffeeNamesAsync()
        {
            var names = new List<string>();

            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("SqlConnectionString is missing.");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // DISTINCT ensures we only get each name once
                string query = "SELECT DISTINCT CoffeeName FROM Transactions";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Handle potential nulls safely
                            if (!reader.IsDBNull(0))
                            {
                                names.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            return names;
        }
        public async Task CreateTransactionAsync(CoffeeTransaction transaction)
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("SqlConnectionString is missing.");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // We don't insert ID because the DB generates it (DEFAULT NEWID())
                string query = @"INSERT INTO Transactions (DateTime, CashType, Card, Money, CoffeeName) 
                                VALUES (@DateTime, @CashType, @Card, @Money, @CoffeeName)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DateTime", transaction.DateTime);
                    command.Parameters.AddWithValue("@CashType", transaction.CashType ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Card", transaction.Card ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Money", transaction.Money);
                    command.Parameters.AddWithValue("@CoffeeName", transaction.CoffeeName ?? (object)DBNull.Value);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<int> GetTotalTransactionsAsync()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("SqlConnectionString is missing.");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Transactions";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    // ExecuteScalar is efficient for getting a single value
                    var result = await command.ExecuteScalarAsync();
                    return result != null ? (int)result : 0;
                }
            }
        }
    }
}
    
