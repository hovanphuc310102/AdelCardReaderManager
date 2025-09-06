using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace AdelCardReaderManager
{
    public class DatabaseHelper
    {
        // Connection string to connect to the Adel9200 database using SQL authentication
        // Added TrustServerCertificate=True to handle SSL certificate validation issues
        private static readonly string ConnectionString = "Server=(local);Database=Adel9200;User ID=adel;Password=ADELOK;TrustServerCertificate=True";

        /// <summary>
        /// Gets all records from the JBB table
        /// </summary>
        /// <returns>DataTable containing all JBB records</returns>
        public static DataTable GetAllJBBRecords()
        {   
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    
                    using (SqlCommand command = new SqlCommand("SELECT * FROM JBB", connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving JBB records: {ex.Message}", "Database Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }

        /// <summary>
        /// Gets all records from the XTCS table
        /// </summary>
        /// <returns>DataTable containing all XTCS records</returns>
        public static DataTable GetAllXTCSRecords()
        {   
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    
                    using (SqlCommand command = new SqlCommand("SELECT * FROM XTCS", connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving XTCS records: {ex.Message}", "Database Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }

        /// <summary>
        /// Tests the connection to the database
        /// </summary>
        /// <returns>True if connection is successful, false otherwise</returns>
        public static bool TestDatabaseConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Gets the current connection string
        /// </summary>
        /// <returns>Current connection string being used</returns>
        public static string GetConnectionString()
        {
            return ConnectionString;
        }

        /// <summary>
        /// Executes a query that returns no results (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">SQL query to execute</param>
        /// <returns>Number of rows affected</returns>
        public static int ExecuteNonQuery(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing query: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}