using CartItem.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

public class UserRepository
{
    private readonly string connectionString;

    public UserRepository(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public void RegisterUser(User user)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            // Start the connection and transaction
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Insert the user into the Users table
                    string userQuery = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password); SELECT SCOPE_IDENTITY();";
                    SqlCommand userCommand = new SqlCommand(userQuery, connection, transaction);
                    userCommand.Parameters.AddWithValue("@Username", user.Username);
                    userCommand.Parameters.AddWithValue("@Password", user.Password); // Hash passwords in production

                    // Get the newly created user's ID
                    int newUserId = Convert.ToInt32(userCommand.ExecuteScalar());

                    // Insert default coupons for the new user
                    string couponQuery = "INSERT INTO Coupons (Name, Validity, IsUsed, UserId) VALUES (@Name, @Validity, @IsUsed, @UserId)";
                    using (SqlCommand couponCommand = new SqlCommand(couponQuery, connection, transaction))
                    {
                        // First coupon
                        couponCommand.Parameters.AddWithValue("@Name", "CDP10");
                        couponCommand.Parameters.AddWithValue("@Validity", DateTime.Now.AddDays(2)); // Default validity
                        couponCommand.Parameters.AddWithValue("@IsUsed", false);
                        couponCommand.Parameters.AddWithValue("@UserId", newUserId);
                        couponCommand.ExecuteNonQuery();

                        // Second coupon
                        couponCommand.Parameters["@Name"].Value = "CDPCAP";
                        couponCommand.ExecuteNonQuery();
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // Rollback the transaction if something went wrong
                    transaction.Rollback();
                    throw; // Rethrow the exception for further handling
                }
            }
        }
    }


    public User Login(string username, string password)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password"; // Hash comparison in production
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = (int)reader["Id"],
                    Username = (string)reader["Username"],
                    Password = (string)reader["Password"]
                };
            }
            return null;
        }
    }
}
