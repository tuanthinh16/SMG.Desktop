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
    public class ReportHelper
    {
        public ReportHelper() : base() { }

        private static SemaphoreSlim semaphore = new SemaphoreSlim(5); // Giới hạn số lượng luồng kết nối đồng thời

        public async Task<List<Report>> LoadReportsFromDatabaseAsync(int? start, int? limit, string subQuery)
        {
            var reports = new List<Report>();

            try
            {
                var dbHelper = new DBHelper();
                await semaphore.WaitAsync();

                string query = @"SELECT *
                                    FROM (
                                        SELECT SMN_REPORT.*, ROW_NUMBER() OVER (ORDER BY CREATE_TIME) AS ROW_NUM
                                        FROM SMN_REPORT
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
                            reports.Add(new Report
                            {
                                ID = reader.GetInt64(reader.GetOrdinal("ID")),
                                REPORT_CODE = reader.GetString(reader.GetOrdinal("REPORT_CODE")),
                                REPORT_NAME = reader.GetString(reader.GetOrdinal("REPORT_NAME")),
                                REPORT_TYPE_CODE = reader.GetString(reader.GetOrdinal("REPORT_TYPE_CODE")),
                                IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE")),
                                CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                                CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                                MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                                MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? 0 : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                                REPORT_GROUP_ID = reader.IsDBNull(reader.GetOrdinal("REPORT_GROUP_ID")) ? null : reader.GetString(reader.GetOrdinal("REPORT_GROUP_ID")),
                                REPORT_FILE_NAME = reader.IsDBNull(reader.GetOrdinal("REPORT_FILE_NAME")) ? null : reader.GetString(reader.GetOrdinal("REPORT_FILE_NAME"))
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
                semaphore.Release();
            }

            return reports;
        }

        public async Task<List<Report>> FreeQueryAsync(string query)
        {
            List<Report> result = new List<Report>();
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
                            result.Add(new Report
                            {
                                ID = reader.GetInt64(reader.GetOrdinal("ID")),
                                REPORT_CODE = reader.GetString(reader.GetOrdinal("REPORT_CODE")),
                                REPORT_NAME = reader.GetString(reader.GetOrdinal("REPORT_NAME")),
                                REPORT_TYPE_CODE = reader.GetString(reader.GetOrdinal("REPORT_TYPE_CODE")),
                                IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE")),
                                CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                                CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                                MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                                MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? 0 : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                                REPORT_GROUP_ID = reader.IsDBNull(reader.GetOrdinal("REPORT_GROUP_ID")) ? null : reader.GetString(reader.GetOrdinal("REPORT_GROUP_ID")),
                                REPORT_FILE_NAME = reader.IsDBNull(reader.GetOrdinal("REPORT_FILE_NAME")) ? null : reader.GetString(reader.GetOrdinal("REPORT_FILE_NAME"))
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

        public async Task<(bool, string)> AddReportAsync(Report report)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand(
                        "INSERT INTO SMN_REPORT (REPORT_CODE, REPORT_NAME, IS_ACTIVE, CREATE_TIME, CREATOR,REPORT_TYPE_CODE,REPORT_GROUP_ID,REPORT_FILE_NAME) " +
                        "VALUES (:REPORT_CODE, :REPORT_NAME, :IS_ACTIVE, :CREATE_TIME, :CREATOR,:REPORT_TYPE_CODE,:REPORT_GROUP_ID,:REPORT_FILE_NAME)", connection);

                    command.Parameters.Add(new OracleParameter(":REPORT_CODE", report.REPORT_CODE));
                    command.Parameters.Add(new OracleParameter(":REPORT_NAME", report.REPORT_NAME));
                    command.Parameters.Add(new OracleParameter(":IS_ACTIVE", report.IS_ACTIVE));
                    command.Parameters.Add(new OracleParameter(":CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    command.Parameters.Add(new OracleParameter(":CREATOR", report.CREATOR));
                    command.Parameters.Add(new OracleParameter(":REPORT_TYPE_CODE", report.REPORT_TYPE_CODE));
                    command.Parameters.Add(new OracleParameter(":REPORT_GROUP_ID", report.REPORT_GROUP_ID));
                    command.Parameters.Add(new OracleParameter(":REPORT_FILE_NAME", report.REPORT_FILE_NAME));

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error adding report: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

        public async Task<(bool, string)> UpdateReportAsync(Report report)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var query = "UPDATE SMN_REPORT SET ";
                    List<OracleParameter> parameters = new List<OracleParameter>();

                    if (!string.IsNullOrEmpty(report.REPORT_CODE))
                    {
                        AddSubQuery(ref query, "REPORT_CODE", report.REPORT_CODE, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(report.REPORT_NAME))
                    {
                        AddSubQuery(ref query, "REPORT_NAME", report.REPORT_NAME, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(report.REPORT_FILE_NAME))
                    {
                        AddSubQuery(ref query, "REPORT_FILE_NAME", report.REPORT_FILE_NAME, ref parameters);
                    }
                    AddSubQuery(ref query, "IS_ACTIVE", report.IS_ACTIVE, ref parameters);
                    if (report.MODIFIER != null)
                    {
                        AddSubQuery(ref query, "MODIFIER", report.MODIFIER, ref parameters);
                    }

                    query += "MODIFY_TIME = :MODIFY_TIME WHERE ID = :ID";

                    parameters.Add(new OracleParameter(":MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    parameters.Add(new OracleParameter(":ID", report.ID));

                    var command = new OracleCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error updating report: " + ex.Message);
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

        public async Task<(bool, string)> DeleteReportAsync(long reportId)
        {
            string error = string.Empty;
            try
            {
                if (reportId <= 0)
                {
                    error = "Invalid Report ID";
                    return (false, error);
                }

                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand("DELETE FROM SMN_REPORT WHERE ID = :ID", connection);
                    command.Parameters.Add(new OracleParameter(":ID", reportId));

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error deleting report: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }
    }
}
