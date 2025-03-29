using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Facturacion
{
    public static class DatabaseHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable result = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(result);
                }
                catch (Exception ex)
                {
                    LogError("Error executing query", ex);
                    throw;
                }
            }

            return result;
        }

        public static int ExecuteNonQuery(string commandText, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(commandText, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                try
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    LogError("Error executing non-query", ex);
                    throw;
                }
            }
        }

        public static object ExecuteScalar(string commandText, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(commandText, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                try
                {
                    connection.Open();
                    return command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    LogError("Error executing scalar", ex);
                    throw;
                }
            }
        }

        public static SqlDataReader ExecuteReader(string commandText, SqlParameter[] parameters = null)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(commandText, connection);

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            try
            {
                connection.Open();
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                connection.Dispose();
                LogError("Error executing reader", ex);
                throw;
            }
        }

        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void LogError(string message, Exception ex)
        {
            // TODO: Implement proper error logging
            string errorMessage = $"{message}: {ex.Message}\nStack Trace: {ex.StackTrace}";
            System.Diagnostics.Debug.WriteLine(errorMessage);
        }

        public static string GetNextInvoiceNumber()
        {
            string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(Numero, 6, 4) AS INT)), 0) + 1 FROM Facturas " +
                          "WHERE SUBSTRING(Numero, 1, 5) = 'FACT-' AND " +
                          "SUBSTRING(Numero, 6, 4) = CAST(YEAR(GETDATE()) AS VARCHAR(4))";

            int nextNumber = Convert.ToInt32(ExecuteScalar(query));
            return $"FACT-{DateTime.Today:yyyy}{nextNumber:D4}";
        }
    }
}