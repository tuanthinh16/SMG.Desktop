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
    public class ReportTypeHelper
    {
        public ReportTypeHelper() : base() { }

        // Semaphore để giới hạn số lượng thread kết nối đồng thời
        private static SemaphoreSlim semaphore = new SemaphoreSlim(5); // Ví dụ: tối đa 5 luồng

        // Thêm Reportype mới
        public async Task<(bool, string)> AddReportTypeAsync(ReportType reportType)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand(
                        "INSERT INTO SMN_REPORT_TYPE (REPORT_TYPE_CODE, REPORT_TYPE_NAME, REPORT_TYPE_GROUP_ID, CREATE_TIME, CREATOR, IS_ACTIVE) " +
                        "VALUES (@REPORT_TYPE_CODE, @REPORT_TYPE_NAME, @REPORT_TYPE_GROUP_ID, @CREATE_TIME, @CREATOR, @IS_ACTIVE)", connection);

                    command.Parameters.AddWithValue("@REPORT_TYPE_CODE", reportType.REPORT_TYPE_CODE);
                    command.Parameters.AddWithValue("@REPORT_TYPE_NAME", reportType.REPORT_TYPE_NAME);
                    command.Parameters.AddWithValue("@REPORT_TYPE_GROUP_ID", reportType.REPORT_TYPE_GROUP_ID);
                    command.Parameters.AddWithValue("@CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now));
                    command.Parameters.AddWithValue("@CREATOR", reportType.CREATOR);
                    command.Parameters.AddWithValue("@IS_ACTIVE", reportType.IS_ACTIVE);

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error adding report type: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

        // Cập nhật Reportype
        public async Task<(bool, string)> UpdateReportTypeAsync(ReportType reportType)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var query = "UPDATE SMN_REPORT_TYPE SET ";
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(reportType.REPORT_TYPE_CODE))
                    {
                        AddSubQuery(ref query, "REPORT_TYPE_CODE", reportType.REPORT_TYPE_CODE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(reportType.REPORT_TYPE_NAME))
                    {
                        AddSubQuery(ref query, "REPORT_TYPE_NAME", reportType.REPORT_TYPE_NAME, ref parameters);
                    }
                    if (reportType.REPORT_TYPE_GROUP_ID.HasValue)
                    {
                        AddSubQuery(ref query, "REPORT_TYPE_GROUP_ID", reportType.REPORT_TYPE_GROUP_ID, ref parameters);
                    }
                    if (reportType.MODIFIER != null)
                    {
                        AddSubQuery(ref query, "MODIFIER", reportType.MODIFIER, ref parameters);
                    }

                    query += "MODIFY_TIME = @MODIFY_TIME, IS_ACTIVE = @IS_ACTIVE WHERE ID = @ID";

                    parameters.Add(new MySqlParameter("@MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    parameters.Add(new MySqlParameter("@IS_ACTIVE", reportType.IS_ACTIVE));
                    parameters.Add(new MySqlParameter("@ID", reportType.ID));

                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error updating report type: " + ex.Message);
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

        // Xóa Reportype
        public async Task<(bool, string)> DeleteReportTypeAsync(long id)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand("DELETE FROM SMN_REPORT_TYPE WHERE ID = @ID", connection);
                    command.Parameters.AddWithValue("@ID", id);

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error deleting report type: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

        // Lấy danh sách ReportType
        public async Task<List<ReportType>> GetReportTypesAsync()
        {
            var reportTypes = new List<ReportType>();
            try
            {
                var dbHelper = new DBHelper();
                await semaphore.WaitAsync();

                string query = "SELECT * FROM SMN_REPORT_TYPE";

                using (var connection = await dbHelper.OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        reportTypes.Add(new ReportType
                        {
                            ID = reader.GetInt64(reader.GetOrdinal("ID")),
                            REPORT_TYPE_CODE = reader.GetString(reader.GetOrdinal("REPORT_TYPE_CODE")),
                            REPORT_TYPE_NAME = reader.GetString(reader.GetOrdinal("REPORT_TYPE_NAME")),
                            REPORT_TYPE_GROUP_ID = reader.IsDBNull(reader.GetOrdinal("REPORT_TYPE_GROUP_ID")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("REPORT_TYPE_GROUP_ID")),
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
                LogSystem.Error("Error fetching report types: " + ex.Message);
            }
            finally
            {
                semaphore.Release();
            }

            return reportTypes;
        }
    }
}