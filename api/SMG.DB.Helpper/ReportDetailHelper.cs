using Oracle.ManagedDataAccess.Client;
using SMG.Models;
using SMG.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using SMG.DB.Helper;

namespace SMG.DB.Helpper
{
    public class ReportDetailHelper
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(5); // Giới hạn số lượng luồng kết nối đồng thời

        // Thêm ReportDetail vào cơ sở dữ liệu
        public async Task<(bool, string)> AddReportDetailAsync(ReportDetail reportDetail)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand(
                        "INSERT INTO SMN_REPORT_DETAIL (REPORT_TYPE_CODE, REPORT_CODE, REPORT_JSON_FILTER, REPORT_NAME, REPORT_DETAIL_CODE, CREATE_TIME, CREATOR, IS_ACTIVE, OUTPUT_FILE_NAME) " +
                        "VALUES (:REPORT_TYPE_CODE, :REPORT_CODE, :REPORT_JSON_FILTER, :REPORT_NAME, :REPORT_DETAIL_CODE, :CREATE_TIME, :CREATOR, :IS_ACTIVE, :OUTPUT_FILE_NAME)", connection);

                    command.Parameters.Add(new OracleParameter(":REPORT_TYPE_CODE", reportDetail.REPORT_TYPE_CODE ?? string.Empty));
                    command.Parameters.Add(new OracleParameter(":REPORT_CODE", reportDetail.REPORT_CODE ?? string.Empty));
                    command.Parameters.Add(new OracleParameter(":REPORT_JSON_FILTER", reportDetail.REPORT_JSON_FILTER ?? string.Empty));
                    command.Parameters.Add(new OracleParameter(":REPORT_NAME", reportDetail.REPORT_NAME ?? string.Empty));
                    command.Parameters.Add(new OracleParameter(":REPORT_DETAIL_CODE", reportDetail.REPORT_DETAIL_CODE ?? string.Empty));
                    command.Parameters.Add(new OracleParameter(":CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    command.Parameters.Add(new OracleParameter(":CREATOR", !string.IsNullOrEmpty(reportDetail.CREATOR) ? reportDetail.CREATOR : "ADMIN"));
                    command.Parameters.Add(new OracleParameter(":IS_ACTIVE", reportDetail.IS_ACTIVE));
                    command.Parameters.Add(new OracleParameter(":OUTPUT_FILE_NAME", reportDetail.OUTPUT_FILE_NAME ?? string.Empty));

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error adding report detail: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }


        // Cập nhật ReportDetail
        public async Task<(bool, string)> UpdateReportDetailAsync(ReportDetail reportDetail)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var query = "UPDATE SMN_REPORT_DETAIL SET ";
                    List<OracleParameter> parameters = new List<OracleParameter>();

                    if (!string.IsNullOrEmpty(reportDetail.REPORT_TYPE_CODE))
                    {
                        AddSubQuery(ref query, "REPORT_TYPE_CODE", reportDetail.REPORT_TYPE_CODE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(reportDetail.REPORT_CODE))
                    {
                        AddSubQuery(ref query, "REPORT_CODE", reportDetail.REPORT_CODE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(reportDetail.REPORT_JSON_FILTER))
                    {
                        AddSubQuery(ref query, "REPORT_JSON_FILTER", reportDetail.REPORT_JSON_FILTER, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(reportDetail.REPORT_NAME))
                    {
                        AddSubQuery(ref query, "REPORT_NAME", reportDetail.REPORT_NAME, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(reportDetail.REPORT_DETAIL_CODE))
                    {
                        AddSubQuery(ref query, "REPORT_DETAIL_CODE", reportDetail.REPORT_DETAIL_CODE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(reportDetail.OUTPUT_FILE_NAME))
                    {
                        AddSubQuery(ref query, "OUTPUT_FILE_NAME", reportDetail.OUTPUT_FILE_NAME, ref parameters);
                    }

                    query += "MODIFY_TIME = :MODIFY_TIME WHERE ID = :ID";

                    parameters.Add(new OracleParameter(":MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    parameters.Add(new OracleParameter(":ID", reportDetail.ID));

                    var command = new OracleCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error updating report detail: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

        // Xóa ReportDetail
        public async Task<(bool, string)> DeleteReportDetailAsync(long reportDetailId)
        {
            string error = string.Empty;
            try
            {
                if (reportDetailId <= 0)
                {
                    error = "Invalid ReportDetail ID";
                    return (false, error);
                }

                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand("DELETE FROM SMN_REPORT_DETAIL WHERE ID = :ID", connection);
                    command.Parameters.Add(new OracleParameter(":ID", reportDetailId));

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error deleting report detail: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

        // Lấy danh sách ReportDetails từ cơ sở dữ liệu
        public async Task<List<ReportDetail>> LoadReportDetailsAsync(int? start, int? limit, string subQuery)
        {
            var reportDetails = new List<ReportDetail>();

            try
            {
                var dbHelper = new DBHelper();
                await semaphore.WaitAsync();

                string query = @"SELECT *
                                    FROM (
                                        SELECT SMN_REPORT_DETAIL.*, ROW_NUMBER() OVER (ORDER BY CREATE_TIME) AS ROW_NUM
                                        FROM SMN_REPORT_DETAIL
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

                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand(query, connection);

                    using (var reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            reportDetails.Add(new ReportDetail
                            {
                                ID = reader.GetInt64(reader.GetOrdinal("ID")),
                                REPORT_TYPE_CODE = reader.GetString(reader.GetOrdinal("REPORT_TYPE_CODE")),
                                REPORT_CODE = reader.GetString(reader.GetOrdinal("REPORT_CODE")),
                                REPORT_JSON_FILTER = reader.IsDBNull(reader.GetOrdinal("REPORT_JSON_FILTER")) ? null : reader.GetString(reader.GetOrdinal("REPORT_JSON_FILTER")),
                                REPORT_NAME = reader.GetString(reader.GetOrdinal("REPORT_NAME")),
                                REPORT_DETAIL_CODE = reader.GetString(reader.GetOrdinal("REPORT_DETAIL_CODE")),
                                OUTPUT_FILE_NAME = reader.GetString(reader.GetOrdinal("OUTPUT_FILE_NAME")),
                                IS_ACTIVE = reader.GetBoolean(reader.GetOrdinal("IS_ACTIVE")),
                                CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                                CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                                MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                                MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? 0: reader.GetInt64(reader.GetOrdinal("MODIFY_TIME"))
                            });
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error loading report details: " + ex.Message);
            }
            finally
            {
                semaphore.Release();
            }

            return reportDetails;
        }

        // Thêm câu lệnh con vào query
        private void AddSubQuery(ref string query, string fieldName, object fieldValue, ref List<OracleParameter> parameters)
        {
            if (fieldValue != null)
            {
                query += $"{fieldName} = :{fieldName}, ";
                parameters.Add(new OracleParameter($":{fieldName}", fieldValue));
            }
        }
    }
}
