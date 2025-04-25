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
    public class ImpMestHelper
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(20); // Giới hạn số lượng luồng kết nối đồng thời

        // Fetch all ImpMest records
        public async Task<List<ImpMest>> FetchImpMestsAsync()
        {
            var impMests = new List<ImpMest>();
            try
            {
                var dbHelper = new DBHelper();
                await semaphore.WaitAsync();

                string query = "SELECT * FROM SMN_IMP_MEST";

                using (var connection = await dbHelper.OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        impMests.Add(new ImpMest
                        {
                            ID = reader.GetInt64(reader.GetOrdinal("ID")),
                            IMP_MEST_TYPE_ID = reader.GetString(reader.GetOrdinal("IMP_MEST_TYPE_ID")),
                            IMP_MEST_STT_ID = reader.GetString(reader.GetOrdinal("IMP_MEST_STT_ID")),
                            IMP_MEST_CODE = reader.GetString(reader.GetOrdinal("IMP_MEST_CODE")),
                            IMP_MEST_TIME = reader.GetInt64(reader.GetOrdinal("IMP_MEST_TIME")),
                            IMP_MEST_STATUS = reader.GetString(reader.GetOrdinal("IMP_MEST_STATUS")),
                            IMP_MEST_DESCRIPTION = reader.GetString(reader.GetOrdinal("IMP_MEST_DESCRIPTION")),
                            PRODUCT_TYPE_ID = reader.GetInt64(reader.GetOrdinal("PRODUCT_TYPE_ID")),
                            PRODUCT_ID = reader.GetInt64(reader.GetOrdinal("PRODUCT_ID")),
                            AMOUNT = reader.GetInt64(reader.GetOrdinal("AMOUNT")),
                            VAT = reader.GetDecimal(reader.GetOrdinal("VAT")),
                            LOGINNAME = reader.GetString(reader.GetOrdinal("LOGINNAME")),
                            IMP_TIME = Int64.Parse(reader.GetString(reader.GetOrdinal("IMP_TIME"))),
                            REQUEST_TIME = reader.GetString(reader.GetOrdinal("REQUEST_TIME")),
                            REQUEST_LOGINNAME = reader.GetString(reader.GetOrdinal("REQUEST_LOGINNAME")),
                            CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                            CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                            MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                            MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                            IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE")),
                            SUPPLIER = reader.GetString(reader.GetOrdinal("SUPPLIER")),
                            BILL_NUMBER = reader.GetString(reader.GetOrdinal("BILL_NUMBER"))
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error fetching ImpMests: " + ex.Message);
            }
            finally
            {
                semaphore.Release();
            }

            return impMests;
        }

        // Add a new ImpMest record
        public async Task<(bool, string)> AddImpMestAsync(ImpMest impMest)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand(
                        "INSERT INTO SMN_IMP_MEST (IMP_MEST_TYPE_ID, IMP_MEST_STT_ID, IMP_MEST_CODE, IMP_MEST_TIME, IMP_MEST_STATUS, IMP_MEST_DESCRIPTION, PRODUCT_TYPE_ID, PRODUCT_ID, AMOUNT, VAT, LOGINNAME, IMP_TIME, REQUEST_TIME, REQUEST_LOGINNAME, CREATE_TIME, CREATOR, IS_ACTIVE,SUPPLIER,BILL_NUMBER) " +
                        "VALUES (@IMP_MEST_TYPE_ID, @IMP_MEST_STT_ID, @IMP_MEST_CODE, @IMP_MEST_TIME, @IMP_MEST_STATUS, @IMP_MEST_DESCRIPTION, @PRODUCT_TYPE_ID, @PRODUCT_ID, @AMOUNT, @VAT, @LOGINNAME, @IMP_TIME, @REQUEST_TIME, @REQUEST_LOGINNAME, @CREATE_TIME, @CREATOR, @IS_ACTIVE,@SUPPLIER,@BILL_NUMBER)", connection);

                    command.Parameters.AddWithValue("@IMP_MEST_TYPE_ID", impMest.IMP_MEST_TYPE_ID);
                    command.Parameters.AddWithValue("@IMP_MEST_STT_ID", impMest.IMP_MEST_STT_ID);
                    command.Parameters.AddWithValue("@IMP_MEST_CODE", impMest.IMP_MEST_CODE);
                    command.Parameters.AddWithValue("@IMP_MEST_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now));
                    command.Parameters.AddWithValue("@IMP_MEST_STATUS", impMest.IMP_MEST_STATUS);
                    command.Parameters.AddWithValue("@IMP_MEST_DESCRIPTION", impMest.IMP_MEST_DESCRIPTION);
                    command.Parameters.AddWithValue("@PRODUCT_TYPE_ID", impMest.PRODUCT_TYPE_ID);
                    command.Parameters.AddWithValue("@PRODUCT_ID", impMest.PRODUCT_ID);
                    command.Parameters.AddWithValue("@AMOUNT", impMest.AMOUNT);
                    command.Parameters.AddWithValue("@VAT", impMest.VAT);
                    command.Parameters.AddWithValue("@LOGINNAME", impMest.LOGINNAME);
                    command.Parameters.AddWithValue("@IMP_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now));
                    command.Parameters.AddWithValue("@REQUEST_TIME", impMest.REQUEST_TIME);
                    command.Parameters.AddWithValue("@REQUEST_LOGINNAME", impMest.REQUEST_LOGINNAME);
                    command.Parameters.AddWithValue("@CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now));
                    command.Parameters.AddWithValue("@CREATOR", impMest.CREATOR == null ? "ADMIN" : impMest.CREATOR);
                    command.Parameters.AddWithValue("@IS_ACTIVE", 1);
                    command.Parameters.AddWithValue("@SUPPLIER", impMest.SUPPLIER);
                    command.Parameters.AddWithValue("@BILL_NUMBER", impMest.BILL_NUMBER);

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error adding ImpMest: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

        // Update an existing ImpMest record
        public async Task<(bool, string)> UpdateImpMestAsync(ImpMest impMest)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var query = "UPDATE SMN_IMP_MEST SET ";
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(impMest.IMP_MEST_TYPE_ID))
                    {
                        AddSubQuery(ref query, "IMP_MEST_TYPE_ID", impMest.IMP_MEST_TYPE_ID, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(impMest.IMP_MEST_STT_ID))
                    {
                        AddSubQuery(ref query, "IMP_MEST_STT_ID", impMest.IMP_MEST_STT_ID, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(impMest.IMP_MEST_CODE))
                    {
                        AddSubQuery(ref query, "IMP_MEST_CODE", impMest.IMP_MEST_CODE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(impMest.IMP_MEST_STATUS))
                    {
                        AddSubQuery(ref query, "IMP_MEST_STATUS", impMest.IMP_MEST_STATUS, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(impMest.IMP_MEST_DESCRIPTION))
                    {
                        AddSubQuery(ref query, "IMP_MEST_DESCRIPTION", impMest.IMP_MEST_DESCRIPTION, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(impMest.LOGINNAME))
                    {
                        AddSubQuery(ref query, "LOGINNAME", impMest.LOGINNAME, ref parameters);
                    }
                    if (impMest.IMP_TIME != null && impMest.IMP_TIME > 0)
                    {
                        AddSubQuery(ref query, "IMP_TIME", impMest.IMP_TIME, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(impMest.REQUEST_TIME))
                    {
                        AddSubQuery(ref query, "REQUEST_TIME", impMest.REQUEST_TIME, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(impMest.REQUEST_LOGINNAME))
                    {
                        AddSubQuery(ref query, "REQUEST_LOGINNAME", impMest.REQUEST_LOGINNAME, ref parameters);
                    }
                    if (impMest.MODIFIER != null)
                    {
                        AddSubQuery(ref query, "MODIFIER", impMest.MODIFIER, ref parameters);
                    }

                    query += "MODIFY_TIME = @MODIFY_TIME, IS_ACTIVE = @IS_ACTIVE WHERE ID = @ID";

                    parameters.Add(new MySqlParameter("@MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    parameters.Add(new MySqlParameter("@IS_ACTIVE", impMest.IS_ACTIVE));
                    parameters.Add(new MySqlParameter("@ID", impMest.ID));

                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error updating ImpMest: " + ex.Message);
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

        // Delete an ImpMest record by ID
        public async Task<(bool, string)> DeleteImpMestAsync(long id)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand("DELETE FROM SMN_IMP_MEST WHERE ID = @ID", connection);
                    command.Parameters.AddWithValue("@ID", id);

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error deleting ImpMest: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }
    }
}