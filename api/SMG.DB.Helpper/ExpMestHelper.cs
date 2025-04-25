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
    public class ExpMestHelper
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(20); // Giới hạn số lượng luồng kết nối đồng thời

        // Fetch all ExpMest records
        public async Task<List<ExpMest>> FetchExpMestsAsync()
        {
            var expMests = new List<ExpMest>();
            try
            {
                var dbHelper = new DBHelper();
                await semaphore.WaitAsync();

                string query = "SELECT * FROM SMN_EXP_MEST";

                using (var connection = await dbHelper.OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        expMests.Add(new ExpMest
                        {
                            ID = reader.GetInt64(reader.GetOrdinal("ID")),
                            EXP_MEST_TYPE_ID = reader.GetString(reader.GetOrdinal("EXP_MEST_TYPE_ID")),
                            EXP_MEST_STT_ID = reader.GetString(reader.GetOrdinal("EXP_MEST_STT_ID")),
                            EXP_MEST_CODE = reader.GetString(reader.GetOrdinal("EXP_MEST_CODE")),
                            EXP_MEST_TIME = reader.GetInt64(reader.GetOrdinal("EXP_MEST_TIME")),
                            EXP_MEST_STATUS = reader.GetString(reader.GetOrdinal("EXP_MEST_STATUS")),
                            EXP_MEST_DESCRIPTION = reader.GetString(reader.GetOrdinal("EXP_MEST_DESCRIPTION")),
                            PRODUCT_TYPE_ID = reader.GetInt64(reader.GetOrdinal("PRODUCT_TYPE_ID")),
                            PRODUCT_ID = reader.GetInt64(reader.GetOrdinal("PRODUCT_ID")),
                            AMOUNT = reader.GetInt64(reader.GetOrdinal("AMOUNT")),
                            VAT = reader.GetDecimal(reader.GetOrdinal("VAT")),
                            LOGINNAME = reader.GetString(reader.GetOrdinal("LOGINNAME")),
                            EXP_TIME = reader.GetString(reader.GetOrdinal("EXP_TIME")),
                            REQUEST_TIME = reader.GetString(reader.GetOrdinal("REQUEST_TIME")),
                            REQUEST_LOGINNAME = reader.GetString(reader.GetOrdinal("REQUEST_LOGINNAME")),
                            CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                            CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                            MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                            MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                            IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE"))
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error fetching ExpMests: " + ex.Message);
            }
            finally
            {
                semaphore.Release();
            }

            return expMests;
        }

        // Add a new ExpMest record
        public async Task<(bool, string)> AddExpMestAsync(ExpMest expMest)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand(
                        "INSERT INTO SMN_EXP_MEST (EXP_MEST_TYPE_ID, EXP_MEST_STT_ID, EXP_MEST_CODE, EXP_MEST_TIME, EXP_MEST_STATUS, EXP_MEST_DESCRIPTION, PRODUCT_TYPE_ID, PRODUCT_ID, AMOUNT, VAT, LOGINNAME, EXP_TIME, REQUEST_TIME, REQUEST_LOGINNAME, CREATE_TIME, CREATOR, IS_ACTIVE) " +
                        "VALUES (@EXP_MEST_TYPE_ID, @EXP_MEST_STT_ID, @EXP_MEST_CODE, @EXP_MEST_TIME, @EXP_MEST_STATUS, @EXP_MEST_DESCRIPTION, @PRODUCT_TYPE_ID, @PRODUCT_ID, @AMOUNT, @VAT, @LOGINNAME, @EXP_TIME, @REQUEST_TIME, @REQUEST_LOGINNAME, @CREATE_TIME, @CREATOR, @IS_ACTIVE)", connection);

                    command.Parameters.AddWithValue("@EXP_MEST_TYPE_ID", expMest.EXP_MEST_TYPE_ID);
                    command.Parameters.AddWithValue("@EXP_MEST_STT_ID", expMest.EXP_MEST_STT_ID);
                    command.Parameters.AddWithValue("@EXP_MEST_CODE", expMest.EXP_MEST_CODE);
                    command.Parameters.AddWithValue("@EXP_MEST_TIME", expMest.EXP_MEST_TIME);
                    command.Parameters.AddWithValue("@EXP_MEST_STATUS", expMest.EXP_MEST_STATUS);
                    command.Parameters.AddWithValue("@EXP_MEST_DESCRIPTION", expMest.EXP_MEST_DESCRIPTION);
                    command.Parameters.AddWithValue("@PRODUCT_TYPE_ID", expMest.PRODUCT_TYPE_ID);
                    command.Parameters.AddWithValue("@PRODUCT_ID", expMest.PRODUCT_ID);
                    command.Parameters.AddWithValue("@AMOUNT", expMest.AMOUNT);
                    command.Parameters.AddWithValue("@VAT", expMest.VAT);
                    command.Parameters.AddWithValue("@LOGINNAME", expMest.LOGINNAME);
                    command.Parameters.AddWithValue("@EXP_TIME", expMest.EXP_TIME);
                    command.Parameters.AddWithValue("@REQUEST_TIME", expMest.REQUEST_TIME);
                    command.Parameters.AddWithValue("@REQUEST_LOGINNAME", expMest.REQUEST_LOGINNAME);
                    command.Parameters.AddWithValue("@CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now));
                    command.Parameters.AddWithValue("@CREATOR", expMest.CREATOR);
                    command.Parameters.AddWithValue("@IS_ACTIVE", 1);

                    int result = await command.ExecuteNonQueryAsync();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error adding ExpMest: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

        // Update an existing ExpMest record
        public async Task<(bool, string)> UpdateExpMestAsync(ExpMest expMest)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var query = "UPDATE SMN_EXP_MEST SET ";
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(expMest.EXP_MEST_TYPE_ID))
                    {
                        AddSubQuery(ref query, "EXP_MEST_TYPE_ID", expMest.EXP_MEST_TYPE_ID, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(expMest.EXP_MEST_STT_ID))
                    {
                        AddSubQuery(ref query, "EXP_MEST_STT_ID", expMest.EXP_MEST_STT_ID, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(expMest.EXP_MEST_CODE))
                    {
                        AddSubQuery(ref query, "EXP_MEST_CODE", expMest.EXP_MEST_CODE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(expMest.EXP_MEST_STATUS))
                    {
                        AddSubQuery(ref query, "EXP_MEST_STATUS", expMest.EXP_MEST_STATUS, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(expMest.EXP_MEST_DESCRIPTION))
                    {
                        AddSubQuery(ref query, "EXP_MEST_DESCRIPTION", expMest.EXP_MEST_DESCRIPTION, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(expMest.LOGINNAME))
                    {
                        AddSubQuery(ref query, "LOGINNAME", expMest.LOGINNAME, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(expMest.EXP_TIME))
                    {
                        AddSubQuery(ref query, "EXP_TIME", expMest.EXP_TIME, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(expMest.REQUEST_TIME))
                    {
                        AddSubQuery(ref query, "REQUEST_TIME", expMest.REQUEST_TIME, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(expMest.REQUEST_LOGINNAME))
                    {
                        AddSubQuery(ref query, "REQUEST_LOGINNAME", expMest.REQUEST_LOGINNAME, ref parameters);
                    }
                    if (expMest.MODIFIER != null)
                    {
                        AddSubQuery(ref query, "MODIFIER", expMest.MODIFIER, ref parameters);
                    }

                    query += "MODIFY_TIME = @MODIFY_TIME, IS_ACTIVE = @IS_ACTIVE WHERE ID = @ID";

                    parameters.Add(new MySqlParameter("@MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    parameters.Add(new MySqlParameter("@IS_ACTIVE", expMest.IS_ACTIVE));
                    parameters.Add(new MySqlParameter("@ID", expMest.ID));

                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = await command.ExecuteNonQueryAsync();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error updating ExpMest: " + ex.Message);
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

        // Delete an ExpMest record by ID
        public async Task<(bool, string)> DeleteExpMestAsync(long id)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand("DELETE FROM SMN_EXP_MEST WHERE ID = @ID", connection);
                    command.Parameters.AddWithValue("@ID", id);

                    int result = await command.ExecuteNonQueryAsync();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error deleting ExpMest: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }
    }
}