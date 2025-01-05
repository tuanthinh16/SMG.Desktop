using Oracle.ManagedDataAccess.Client;
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

        // Phương thức lấy plugin từ cơ sở dữ liệu
        public async Task<List<ReportType>> LoadReportTypeFromDatabaseAsync(int? start, int? limit, string subQuery)
        {
            var plugins = new List<ReportType>();

            try
            {
                var dbHelper = new DBHelper();
                await semaphore.WaitAsync();  // Giới hạn số lượng luồng kết nối đồng thời

                string query = @"SELECT *
                    FROM (
                        SELECT SMN_REPORT_TYPE.*, ROW_NUMBER() OVER (ORDER BY CREATE_TIME) AS ROW_NUM
                        FROM SMN_REPORT_TYPE
                    )
                    WHERE 1=1 ";

                if (start.HasValue && limit.HasValue)
                {
                    query += " AND ROW_NUM BETWEEN :start AND :limit ";
                    query = query.Replace(":start", start.ToString());
                    query = query.Replace(":limit", limit.ToString());
                }
                if (!string.IsNullOrEmpty(subQuery))
                {
                    query += subQuery;
                }

                // Mở kết nối bất đồng bộ
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand(query, connection);

                    using (var reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            plugins.Add(new ReportType
                            {
                                ID = reader.GetInt64(reader.GetOrdinal("ID")),
                                REPORT_TYPE_CODE = reader.GetString(reader.GetOrdinal("REPORT_TYPE_CODE")),
                                REPORT_TYPE_NAME = reader.GetString(reader.GetOrdinal("REPORT_TYPE_NAME")),
                                IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE")),
                                CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                                CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                                MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                                MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? 0 : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                                REPORT_TYPE_GROUP_ID = reader.IsDBNull(reader.GetOrdinal("REPORT_TYPE_GROUP_ID")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("REPORT_TYPE_GROUP_ID"))
                                
                            });
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error: " + ex.Message);
            }
            finally
            {
                semaphore.Release();  // Giải phóng semaphore sau khi hoàn thành
            }

            return plugins;
        }

        public async Task<List<ReportType>> FreeQueryAsync(string query)
        {
            List<ReportType> result = new List<ReportType>();
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand(query, connection);
                    using (var reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new ReportType
                            {
                                ID = reader.GetInt64(reader.GetOrdinal("ID")),
                                REPORT_TYPE_CODE = reader.GetString(reader.GetOrdinal("REPORT_TYPE_CODE")),
                                REPORT_TYPE_NAME = reader.GetString(reader.GetOrdinal("REPORT_TYPE_NAME")),
                                IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE")),
                                CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                                CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                                MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                                MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? 0 : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                                REPORT_TYPE_GROUP_ID = reader.IsDBNull(reader.GetOrdinal("REPORT_TYPE_GROUP_ID")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("REPORT_TYPE_GROUP_ID"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return result;
        }

        // Thêm Reportype mới
        public async Task<(bool, string)> AddReportTypeAsync(ReportType reportType)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand(
                        "INSERT INTO SMN_REPORT_TYPE (REPORT_TYPE_CODE, REPORT_TYPE_NAME, IS_ACTIVE, CREATE_TIME, CREATOR, REPORT_TYPE_GROUP_ID) " +
                        "VALUES (:REPORT_TYPE_CODE, :REPORT_TYPE_NAME, :IS_ACTIVE, :CREATE_TIME, :CREATOR, :REPORT_TYPE_GROUP_ID)", connection);

                    command.Parameters.Add(new OracleParameter(":REPORT_TYPE_CODE", reportType.REPORT_TYPE_CODE));
                    command.Parameters.Add(new OracleParameter(":REPORT_TYPE_NAME", reportType.REPORT_TYPE_NAME));
                    command.Parameters.Add(new OracleParameter(":IS_ACTIVE", reportType.IS_ACTIVE));
                    command.Parameters.Add(new OracleParameter(":CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    command.Parameters.Add(new OracleParameter(":CREATOR", reportType.CREATOR));
                    command.Parameters.Add(new OracleParameter(":REPORT_TYPE_GROUP_ID", reportType.REPORT_TYPE_GROUP_ID ?? (object)DBNull.Value));

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);  // Trả về tuple (thành công, lỗi nếu có)
                }
            }
            catch (OracleException ex)
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
                    List<OracleParameter> parameters = new List<OracleParameter>();

                    // Thêm các trường cần cập nhật
                    if (!string.IsNullOrEmpty(reportType.REPORT_TYPE_CODE))
                    {
                        AddSubQuery(ref query, "REPORT_TYPE_CODE", reportType.REPORT_TYPE_CODE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(reportType.REPORT_TYPE_NAME))
                    {
                        AddSubQuery(ref query, "REPORT_TYPE_NAME", reportType.REPORT_TYPE_NAME, ref parameters);
                    }
                    AddSubQuery(ref query, "IS_ACTIVE", reportType.IS_ACTIVE, ref parameters);
                    if (reportType.REPORT_TYPE_GROUP_ID.HasValue)
                    {
                        AddSubQuery(ref query, "REPORT_TYPE_GROUP_ID", reportType.REPORT_TYPE_GROUP_ID, ref parameters);
                    }
                    if (reportType.MODIFIER != null)
                    {
                        AddSubQuery(ref query, "MODIFIER", reportType.MODIFIER, ref parameters);
                    }

                    query += "MODIFY_TIME = :MODIFY_TIME WHERE ID = :ID";

                    parameters.Add(new OracleParameter(":MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    parameters.Add(new OracleParameter(":ID", reportType.ID));

                    var command = new OracleCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);  // Trả về tuple (thành công, lỗi nếu có)
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error updating report type: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }



        private void AddSubQuery(ref string query, string fieldName, object fieldValue, ref List<OracleParameter> parameters)
        {
            if (fieldValue != null)
            {
                query += $"{fieldName} = :{fieldName}, ";
                parameters.Add(new OracleParameter($":{fieldName}", fieldValue));
            }
        }

        // Xóa Reportype
        public async Task<(bool, string)> DeleteReportTypeAsync(long reportTypeId)
        {
            string error = string.Empty;
            try
            {
                if (reportTypeId <= 0)
                {
                    error = "Invalid Report Type ID";
                    return (false, error);
                }

                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand("DELETE FROM SMN_REPORT_TYPE WHERE ID = :ID", connection);
                    command.Parameters.Add(new OracleParameter(":ID", reportTypeId));

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);  // Trả về tuple (thành công, lỗi nếu có)
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error deleting report type: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

    }
}
