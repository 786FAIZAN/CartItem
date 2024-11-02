using CartItem.Models;
using System.Data;
using System.Data.SqlClient;

public class CartItemRepository
{
    private readonly string _connectionString;

    public CartItemRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // Add a new cart item
    public void AddToCart(CartItems cartItem)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            // First, check if the item already exists in the cart for the user
            string checkQuery = "SELECT Quantity FROM CartItems WHERE UserId = @UserId AND ItemId = @ItemId and ItemName = @ItemName";
            SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
            checkCommand.Parameters.AddWithValue("@UserId", cartItem.UserId);
            checkCommand.Parameters.AddWithValue("@ItemId", cartItem.ItemId);
            checkCommand.Parameters.AddWithValue("@ItemName", cartItem.ItemName);

            connection.Open();
            var existingQuantity = checkCommand.ExecuteScalar();

            if (existingQuantity != null)
            {
                // Item exists, update the quantity
                int newQuantity = (int)existingQuantity + cartItem.Quantity; // Add new quantity to existing
                string updateQuery = "UPDATE CartItems SET Quantity = @Quantity WHERE UserId = @UserId AND ItemId = @ItemId";
                SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@Quantity", newQuantity);
                updateCommand.Parameters.AddWithValue("@UserId", cartItem.UserId);
                updateCommand.Parameters.AddWithValue("@ItemId", cartItem.ItemId);
                updateCommand.ExecuteNonQuery();
            }
            else
            {
                // Item does not exist, insert a new record
                string insertQuery = "INSERT INTO CartItems (UserId, ItemId, ItemName, Quantity, Price, CreatedAt) VALUES (@UserId, @ItemId, @ItemName, @Quantity, @Price, @CreatedAt)";
                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@UserId", cartItem.UserId);
                insertCommand.Parameters.AddWithValue("@ItemId", cartItem.ItemId);
                insertCommand.Parameters.AddWithValue("@ItemName", cartItem.ItemName);
                insertCommand.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
                insertCommand.Parameters.AddWithValue("@Price", cartItem.Price);
                insertCommand.Parameters.AddWithValue("@CreatedAt", cartItem.AddedDate);

                insertCommand.ExecuteNonQuery();
            }
        }
    }



    // Retrieve all cart items for a specific user
    public List<CartItems> GetCartItems(int userId)
    {
        List<CartItems> cartItems = new List<CartItems>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM CartItems WHERE UserId = @UserId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                cartItems.Add(new CartItems
                {
                    Id = (int)reader["CartItemId"],
                    UserId = (int)reader["UserId"],
                    ItemName = reader["ItemName"].ToString(),
                    ItemId = (int)reader["ItemId"],
                    Quantity = (int)reader["Quantity"],
                    Price = (decimal)reader["Price"],
                    AddedDate = (DateTime)reader["CreatedAt"]
                });
            }
        }
        return cartItems;
    }

    // Optional: Remove an item from the cart
    public void RemoveFromCart(int cartItemId)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "DELETE FROM CartItems WHERE Id = @Id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", cartItemId);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    // Optional: Update an existing cart item
    public void UpdateCartItem(CartItems cartItem)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "UPDATE CartItems SET Quantity = @Quantity, Price = @Price WHERE Id = @Id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", cartItem.Id);
            command.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
            command.Parameters.AddWithValue("@Price", cartItem.Price);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    // Optional: Clear the cart for a specific user
    public void ClearCart(int userId)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "DELETE FROM CartItems WHERE UserId = @UserId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
   public Coupon GetCoupon(string code)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = "SELECT * FROM Coupons WHERE Name = @Code AND Validity >= @CurrentDate";
        SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Code", code);
        command.Parameters.AddWithValue("@CurrentDate", DateTime.Now);

        connection.Open();
        SqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Coupon
            {
                Id = (int)reader["Id"],
                Code = (string)reader["Name"],
                Validity = (DateTime)reader["Validity"],
                IsUsed = (bool)reader["IsUsed"] // Retrieve IsUsed status
            };
        }

        return null; // Coupon not found or expired
    }
}

    public List<Coupon> GetUserCoupon(int Userid)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            List<Coupon> coupons = new List<Coupon>();
            string query = "SELECT * FROM Coupons WHERE Validity >= @CurrentDate and UserId = @UserId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CurrentDate", DateTime.Now);
            command.Parameters.AddWithValue("@UserId", Userid);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                coupons.Add(new Coupon()
                {
                    Id = (int)reader["Id"],
                    Code = (string)reader["Name"],
                    Validity = (DateTime)reader["Validity"],
                    IsUsed = (bool)reader["IsUsed"] ,
                    UserId = (int)reader["UserId"]
                });
            }

            return coupons; // Coupon not found or expired
        }
    }

    public void UpdateCoupon(int couponId, int userId)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "UPDATE Coupons SET IsUsed = 1 WHERE Id = @CouponId AND UserId = @UserId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CouponId", couponId);
            command.Parameters.AddWithValue("@UserId", userId);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    public void LogCouponAction(CouponActionLog log)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand("LogCouponAction", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CouponId", log.CouponId);
                command.Parameters.AddWithValue("@UserId", log.UserId);
                command.Parameters.AddWithValue("@ActionMessage", log.ActionMessage);
                command.Parameters.AddWithValue("@DiscountAmount", log.DiscountAmount );
                command.Parameters.AddWithValue("@FreeItemCartId", log.FreeItemCartId);
                command.Parameters.AddWithValue("@FreeItemName",log.FreeItemName);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
    public List<CouponActionLog> GetCouponActionLogs(int userId)
    {
        List<CouponActionLog> actionLogs = new List<CouponActionLog>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT [ActionMessage], [Timestamp] FROM [MachineTestCart].[dbo].[CouponActionLog] WHERE UserId = @UserId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                actionLogs.Add(new CouponActionLog()
                {
                    ActionMessage = reader["ActionMessage"].ToString(),
                    Timestamp = (DateTime)reader["Timestamp"]
                });
            }
        }

        return actionLogs; // Return the list of coupon action logs
    }

}
