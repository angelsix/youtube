using System;
using System.Data.SqlClient;

namespace SqlConnector
{
    class Program
    {
        static void Main(string[] args)
        {
            // Annouce search
            Console.WriteLine("Searching for all users in SQL database...");

            // Open connection to SQL Server
            using (var sqlConnection = new SqlConnection("Server=.;Database=test;User Id=testuser; Password = testuser;"))
            {
                // Log it
                Console.WriteLine("Opening connection...");

                // Attemp to open connection
                sqlConnection.Open();

                // Inject a user into the database
                // NOTE: Command this out once it has run once if you don't want 
                //       to re-adding users  every time you run
                using (var command = new SqlCommand($"INSERT INTO dbo.Users (Id, Username, FirstName, LastName, IsEnabled, CreatedDateUtc) VALUES ('{Guid.NewGuid().ToString("N")}', 'Username1', 'My first name', 'My last name', 1, '10/12/2025 12:32:10 +01:00')", sqlConnection))
                {
                    // Log it
                    Console.WriteLine("Adding new user...");

                    // Execute INSERT command
                    var result = command.ExecuteNonQuery();

                    // Log what should be "1 user added"
                    Console.WriteLine($"{result} user added");
                }

                // Select all users from database
                using (var command = new SqlCommand("SELECT * FROM dbo.Users", sqlConnection))
                {
                    // Log it
                    Console.WriteLine("Selecting all users...");

                    // Execute selection as a reader to read each row...
                    using (var reader = command.ExecuteReader())
                    {
                        // While we have a result
                        while (reader.Read())
                        {
                            Console.WriteLine($"Username: {reader["Username"]}, First Name: {reader["FirstName"]}, LastName: {reader["LastName"]}, IsEnabled: {reader["IsEnabled"]}");
                        }
                    }
                }
            }

            // Log it
            Console.WriteLine("Done. Press any key to exit...");

            // Keep window open
            Console.Read();
        }
    }
}
