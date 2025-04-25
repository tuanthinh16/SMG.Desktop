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
    public class ProductTypeHelper
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(20); // Giới hạn số lượng luồng kết nối đồng thời

        // Fetch all product types
        public async Task<List<ProductType>> FetchProductTypesAsync()
        {
            var productTypes = new List<ProductType>();
            try
            {
                var dbHelper = new DBHelper();
                await semaphore.WaitAsync();

                string query = "SELECT * FROM SMN_PRODUCT_TYPE";

                using (var connection = await dbHelper.OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productTypes.Add(new ProductType
                        {
                            ID = reader.GetInt64(reader.GetOrdinal("ID")),
                            PRODUCT_TYPE_NAME = reader.GetString(reader.GetOrdinal("PRODUCT_TYPE_NAME")),
                            PRODUCT_TYPE_CODE = reader.GetString(reader.GetOrdinal("PRODUCT_TYPE_CODE")),
                            PRODUCT_TYPE_GROUP_ID = reader.IsDBNull(reader.GetOrdinal("PRODUCT_TYPE_GROUP_ID"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PRODUCT_TYPE_GROUP_ID")),

                            PRODUCT_TYPE_DESCRIPTION = reader.IsDBNull(reader.GetOrdinal("PRODUCT_TYPE_DESCRIPTION"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PRODUCT_TYPE_DESCRIPTION")),
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
                LogSystem.Error("Error fetching product types: " + ex.Message);
            }
            finally
            {
                semaphore.Release();
            }

            return productTypes;
        }

        // Add a new product type
        public async Task<(bool, string)> AddProductTypeAsync(ProductType productType)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand(
                        "INSERT INTO SMN_PRODUCT_TYPE (PRODUCT_TYPE_NAME, PRODUCT_TYPE_CODE, PRODUCT_TYPE_GROUP_ID, PRODUCT_TYPE_DESCRIPTION, CREATE_TIME, CREATOR, IS_ACTIVE) " +
                        "VALUES (@PRODUCT_TYPE_NAME, @PRODUCT_TYPE_CODE, @PRODUCT_TYPE_GROUP_ID, @PRODUCT_TYPE_DESCRIPTION, @CREATE_TIME, @CREATOR, @IS_ACTIVE)", connection);

                    command.Parameters.AddWithValue("@PRODUCT_TYPE_NAME", productType.PRODUCT_TYPE_NAME);
                    command.Parameters.AddWithValue("@PRODUCT_TYPE_CODE", productType.PRODUCT_TYPE_CODE);
                    command.Parameters.AddWithValue("@PRODUCT_TYPE_GROUP_ID", productType.PRODUCT_TYPE_GROUP_ID);
                    command.Parameters.AddWithValue("@PRODUCT_TYPE_DESCRIPTION", productType.PRODUCT_TYPE_DESCRIPTION);
                    command.Parameters.AddWithValue("@CREATE_TIME", productType.CREATE_TIME);
                    command.Parameters.AddWithValue("@CREATOR", productType.CREATOR);
                    command.Parameters.AddWithValue("@IS_ACTIVE", productType.IS_ACTIVE);

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error adding product type: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

        // Update an existing product type
        public async Task<(bool, string)> UpdateProductTypeAsync(ProductType productType)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var query = "UPDATE SMN_PRODUCT_TYPE SET ";
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(productType.PRODUCT_TYPE_NAME))
                    {
                        AddSubQuery(ref query, "PRODUCT_TYPE_NAME", productType.PRODUCT_TYPE_NAME, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(productType.PRODUCT_TYPE_CODE))
                    {
                        AddSubQuery(ref query, "PRODUCT_TYPE_CODE", productType.PRODUCT_TYPE_CODE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(productType.PRODUCT_TYPE_GROUP_ID))
                    {
                        AddSubQuery(ref query, "PRODUCT_TYPE_GROUP_ID", productType.PRODUCT_TYPE_GROUP_ID, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(productType.PRODUCT_TYPE_DESCRIPTION))
                    {
                        AddSubQuery(ref query, "PRODUCT_TYPE_DESCRIPTION", productType.PRODUCT_TYPE_DESCRIPTION, ref parameters);
                    }
                    if (productType.MODIFIER != null)
                    {
                        AddSubQuery(ref query, "MODIFIER", productType.MODIFIER, ref parameters);
                    }

                    query += "MODIFY_TIME = @MODIFY_TIME, IS_ACTIVE = @IS_ACTIVE WHERE ID = @ID";

                    parameters.Add(new MySqlParameter("@MODIFY_TIME", productType.MODIFY_TIME));
                    parameters.Add(new MySqlParameter("@IS_ACTIVE", productType.IS_ACTIVE));
                    parameters.Add(new MySqlParameter("@ID", productType.ID));

                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error updating product type: " + ex.Message);
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

        // Delete a product type by ID
        public async Task<(bool, string)> DeleteProductTypeAsync(long id)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand("DELETE FROM SMN_PRODUCT_TYPE WHERE ID = @ID", connection);
                    command.Parameters.AddWithValue("@ID", id);

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error deleting product type: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }
    }
}