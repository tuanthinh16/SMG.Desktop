using MySql.Data.MySqlClient;
using SMG.DB.Helper;
using SMG.Logging;
using SMG.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SMG.DB.Helpper
{
    public class ProductHelper
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(20); // Giới hạn số lượng luồng kết nối đồng thời

        // Fetch all products
        public async Task<List<Product>> FetchProductsAsync()
        {
            var products = new List<Product>();
            try
            {
                var dbHelper = new DBHelper();
                await semaphore.WaitAsync();

                string query = "SELECT * FROM SMN_PRODUCT";

                using (var connection = await dbHelper.OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ID = reader.GetInt64(reader.GetOrdinal("ID")),
                            PRODUCT_NAME = reader.GetString(reader.GetOrdinal("PRODUCT_NAME")),
                            PRODUCT_CODE = reader.GetString(reader.GetOrdinal("PRODUCT_CODE")),
                            PRODUCT_GROUP_ID = reader.IsDBNull(reader.GetOrdinal("PRODUCT_GROUP_ID"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PRODUCT_GROUP_ID")),
                            PRODUCT_TYPE_ID = Int64.Parse(reader.GetString(reader.GetOrdinal("PRODUCT_TYPE_ID"))),
                            PRODUCT_UNIT_ID = reader.GetString(reader.GetOrdinal("PRODUCT_UNIT_ID")),
                            PRODUCT_PRICE = reader.GetString(reader.GetOrdinal("PRODUCT_PRICE")),
                            AMOUNT = reader.GetDecimal(reader.GetOrdinal("AMOUNT")),
                            PRODUCT_DESCRIPTION = reader.GetString(reader.GetOrdinal("PRODUCT_DESCRIPTION")),
                            PRODUCT_IMAGE = reader.GetString(reader.GetOrdinal("PRODUCT_IMAGE")),
                            CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                            CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                            MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("MODIFIER")),
                            MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME"))
                            ? (long?)null
                            : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                            IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE"))
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error fetching products: " + ex.Message);
            }
            finally
            {
                semaphore.Release();
            }

            return products;
        }

        // Add a new product
        public async Task<(bool, string)> AddProductAsync(Product product)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand(
                        "INSERT INTO SMN_PRODUCT (PRODUCT_NAME, PRODUCT_CODE, PRODUCT_GROUP_ID, PRODUCT_TYPE_ID, PRODUCT_UNIT_ID, PRODUCT_PRICE, AMOUNT, PRODUCT_DESCRIPTION, PRODUCT_IMAGE, CREATE_TIME, CREATOR, IS_ACTIVE) " +
                        "VALUES (@PRODUCT_NAME, @PRODUCT_CODE, @PRODUCT_GROUP_ID, @PRODUCT_TYPE_ID, @PRODUCT_UNIT_ID, @PRODUCT_PRICE, @AMOUNT, @PRODUCT_DESCRIPTION, @PRODUCT_IMAGE, @CREATE_TIME, @CREATOR, @IS_ACTIVE)", connection);

                    command.Parameters.AddWithValue("@PRODUCT_NAME", product.PRODUCT_NAME);
                    command.Parameters.AddWithValue("@PRODUCT_CODE", product.PRODUCT_CODE);
                    command.Parameters.AddWithValue("@PRODUCT_GROUP_ID", product.PRODUCT_GROUP_ID);
                    command.Parameters.AddWithValue("@PRODUCT_TYPE_ID", product.PRODUCT_TYPE_ID);
                    command.Parameters.AddWithValue("@PRODUCT_UNIT_ID", product.PRODUCT_UNIT_ID);
                    command.Parameters.AddWithValue("@PRODUCT_PRICE", product.PRODUCT_PRICE);
                    command.Parameters.AddWithValue("@AMOUNT", product.AMOUNT);
                    command.Parameters.AddWithValue("@PRODUCT_DESCRIPTION", product.PRODUCT_DESCRIPTION);
                    command.Parameters.AddWithValue("@PRODUCT_IMAGE", product.PRODUCT_IMAGE);
                    command.Parameters.AddWithValue("@CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now));
                    command.Parameters.AddWithValue("@CREATOR", product.CREATOR);
                    command.Parameters.AddWithValue("@IS_ACTIVE", product.IS_ACTIVE);

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error adding product: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

        // Update an existing product
        public async Task<(bool, string)> UpdateProductAsync(Product product)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var query = "UPDATE SMN_PRODUCT SET ";
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(product.PRODUCT_NAME))
                    {
                        AddSubQuery(ref query, "PRODUCT_NAME", product.PRODUCT_NAME, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(product.PRODUCT_CODE))
                    {
                        AddSubQuery(ref query, "PRODUCT_CODE", product.PRODUCT_CODE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(product.PRODUCT_GROUP_ID))
                    {
                        AddSubQuery(ref query, "PRODUCT_GROUP_ID", product.PRODUCT_GROUP_ID, ref parameters);
                    }
                    if (product.PRODUCT_TYPE_ID > 0)
                    {
                        AddSubQuery(ref query, "PRODUCT_TYPE_ID", product.PRODUCT_TYPE_ID, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(product.PRODUCT_UNIT_ID))
                    {
                        AddSubQuery(ref query, "PRODUCT_UNIT_ID", product.PRODUCT_UNIT_ID, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(product.PRODUCT_PRICE))
                    {
                        AddSubQuery(ref query, "PRODUCT_PRICE", product.PRODUCT_PRICE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(product.PRODUCT_DESCRIPTION))
                    {
                        AddSubQuery(ref query, "PRODUCT_DESCRIPTION", product.PRODUCT_DESCRIPTION, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(product.PRODUCT_IMAGE))
                    {
                        AddSubQuery(ref query, "PRODUCT_IMAGE", product.PRODUCT_IMAGE, ref parameters);
                    }
                    if (product.MODIFIER != null)
                    {
                        AddSubQuery(ref query, "MODIFIER", product.MODIFIER, ref parameters);
                    }

                    query += "MODIFY_TIME = @MODIFY_TIME, IS_ACTIVE = @IS_ACTIVE WHERE ID = @ID";

                    parameters.Add(new MySqlParameter("@MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    parameters.Add(new MySqlParameter("@IS_ACTIVE", product.IS_ACTIVE));
                    parameters.Add(new MySqlParameter("@ID", product.ID));

                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error updating product: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

        private void AddSubQuery(ref string query, string fieldName, object fieldValue, ref List<MySqlParameter> parameters)
        {
            if (fieldValue != null)
            {
                query += $"{fieldName} = @{fieldName}, ";
                parameters.Add(new MySqlParameter($"@{fieldName}", fieldValue));
            }
        }

        // Delete a product by ID
        public async Task<(bool, string)> DeleteProductAsync(long id)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand("DELETE FROM SMN_PRODUCT WHERE ID = @ID", connection);
                    command.Parameters.AddWithValue("@ID", id);

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error deleting product: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }
    }
}