using CartItem.Models;
using System.Data.SqlClient;

namespace CartItem.Services
{
    public class ItemRepository
    {
        private readonly string connectionString;

        public ItemRepository(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Item> GetAllItems()
        {
            var items = new List<Item>();
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Items", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Item
                        {
                            ItemId = (int)reader["ItemId"],
                            Name = (string)reader["Name"],
                            Price = (decimal)reader["Price"],
                            AvlQuantity = (int)reader["AvlQuantity"],
                            UpdatedDate = (DateTime)reader["UpdatedDate"]
                        });
                    }
                }
            }
            return items;
        }

        public Item GetItemById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Items WHERE ItemId = @ItemId", connection);
                command.Parameters.AddWithValue("@ItemId", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Item
                        {
                            ItemId = (int)reader["ItemId"],
                            Name = (string)reader["Name"],
                            Price = (decimal)reader["Price"],
                            AvlQuantity = (int)reader["AvlQuantity"],
                            UpdatedDate = (DateTime)reader["UpdatedDate"]
                        };
                    }
                }
            }
            return null;
        }

        public void AddItem(Item item)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("INSERT INTO Items (Name, Price, AvlQuantity, UpdatedDate) VALUES (@Name, @Price, @AvlQuantity, @UpdatedDate)", connection);
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@Price", item.Price);
                command.Parameters.AddWithValue("@AvlQuantity", item.AvlQuantity);
                command.Parameters.AddWithValue("@UpdatedDate", item.UpdatedDate);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateItem(Item item)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("UPDATE Items SET Name = @Name, Price = @Price, AvlQuantity = @AvlQuantity, UpdatedDate = @UpdatedDate WHERE ItemId = @ItemId", connection);
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@Price", item.Price);
                command.Parameters.AddWithValue("@AvlQuantity", item.AvlQuantity);
                command.Parameters.AddWithValue("@UpdatedDate", item.UpdatedDate);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteItem(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("DELETE FROM Items WHERE ItemId = @ItemId", connection);
                command.Parameters.AddWithValue("@ItemId", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
