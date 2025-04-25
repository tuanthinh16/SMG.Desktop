using MySql.Data.MySqlClient;
using SMG.Logging;
using SMG.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SMG.DB.Helper
{
    public class ReportDetailHelper
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(5);
        private readonly DBHelper _dbHelper;

        public ReportDetailHelper(DBHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<(bool, string)> AddReportDetailAsync(ReportDetail reportDetail)
        {
            string error = string.Empty;
            string query = "INSERT INTO SMN_REPORT_DETAIL (REPORT_TYPE_CODE, REPORT_CODE, REPORT_NAME, REPORT_JSON_FILTER, REPORT_DETAIL_CODE, OUTPUT_FILE_NAME, CREATE_TIME, CREATOR, IS_ACTIVE) VALUES (@REPORT_TYPE_CODE, @REPORT_CODE, @REPORT_NAME, @REPORT_JSON_FILTER, @REPORT_DETAIL_CODE, @OUTPUT_FILE_NAME, @CREATE_TIME, @CREATOR, @IS_ACTIVE)";

            try
            {
                await semaphore.WaitAsync();
                using (var connection = await _dbHelper.OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@REPORT_TYPE_CODE", reportDetail.REPORT_TYPE_CODE);
                    command.Parameters.AddWithValue("@REPORT_CODE", reportDetail.REPORT_CODE);
                    command.Parameters.AddWithValue("@REPORT_NAME", reportDetail.REPORT_NAME);
                    command.Parameters.AddWithValue("@REPORT_JSON_FILTER", reportDetail.REPORT_JSON_FILTER);
                    command.Parameters.AddWithValue("@REPORT_DETAIL_CODE", reportDetail.REPORT_DETAIL_CODE);
                    command.Parameters.AddWithValue("@OUTPUT_FILE_NAME", reportDetail.OUTPUT_FILE_NAME);
                    command.Parameters.AddWithValue("@CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now));
                    command.Parameters.AddWithValue("@CREATOR", reportDetail.CREATOR);
                    command.Parameters.AddWithValue("@IS_ACTIVE", 1);

                    command.ExecuteNonQuery();
                }
                return (true, error);
            }
            catch (Exception ex)
            {
                LogSystem.Error("Error adding report detail: " + ex.Message);
                return (false, ex.Message);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<(bool, string)> UpdateReportDetailAsync(ReportDetail reportDetail)
        {
            string error = string.Empty;
            string query = "UPDATE SMN_REPORT_DETAIL SET REPORT_NAME = @REPORT_NAME, REPORT_JSON_FILTER = @REPORT_JSON_FILTER, OUTPUT_FILE_NAME = @OUTPUT_FILE_NAME, MODIFIER = @MODIFIER, MODIFY_TIME = @MODIFY_TIME WHERE REPORT_DETAIL_CODE = @REPORT_DETAIL_CODE";

            try
            {
                await semaphore.WaitAsync();
                using (var connection = await _dbHelper.OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@REPORT_NAME", reportDetail.REPORT_NAME);
                    command.Parameters.AddWithValue("@REPORT_JSON_FILTER", reportDetail.REPORT_JSON_FILTER);
                    command.Parameters.AddWithValue("@OUTPUT_FILE_NAME", reportDetail.OUTPUT_FILE_NAME);
                    command.Parameters.AddWithValue("@MODIFIER", reportDetail.MODIFIER);
                    command.Parameters.AddWithValue("@MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now));
                    command.Parameters.AddWithValue("@REPORT_DETAIL_CODE", reportDetail.REPORT_DETAIL_CODE);

                    command.ExecuteNonQuery();
                }
                return (true, error);
            }
            catch (Exception ex)
            {
                LogSystem.Error("Error updating report detail: " + ex.Message);
                return (false, ex.Message);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<(bool, string)> DeleteReportDetailAsync(string reportDetailCode)
        {
            string error = string.Empty;
            string query = "DELETE FROM SMN_REPORT_DETAIL WHERE REPORT_DETAIL_CODE = @REPORT_DETAIL_CODE";

            try
            {
                await semaphore.WaitAsync();
                using (var connection = await _dbHelper.OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@REPORT_DETAIL_CODE", reportDetailCode);

                    command.ExecuteNonQuery();
                }
                return (true, error);
            }
            catch (Exception ex)
            {
                LogSystem.Error("Error deleting report detail: " + ex.Message);
                return (false, ex.Message);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<List<ReportDetail>> LoadReportDetailsAsync()
        {
            var reportDetails = new List<ReportDetail>();
            string query = "SELECT * FROM SMN_REPORT_DETAIL WHERE IS_ACTIVE = 1";

            try
            {
                await semaphore.WaitAsync();
                using (var connection = await _dbHelper.OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        reportDetails.Add(new ReportDetail
                        {
                            REPORT_TYPE_CODE = reader.GetString(reader.GetOrdinal("REPORT_TYPE_CODE")),
                            REPORT_CODE = reader.GetString(reader.GetOrdinal("REPORT_CODE")),
                            REPORT_NAME = reader.GetString(reader.GetOrdinal("REPORT_NAME")),
                            REPORT_JSON_FILTER = reader.GetString(reader.GetOrdinal("REPORT_JSON_FILTER")),
                            REPORT_DETAIL_CODE = reader.GetString(reader.GetOrdinal("REPORT_DETAIL_CODE")),
                            OUTPUT_FILE_NAME = reader.GetString(reader.GetOrdinal("OUTPUT_FILE_NAME")),
                            CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                            CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                            MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                            MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                            IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Error loading report details: " + ex.Message);
            }
            finally
            {
                semaphore.Release();
            }
            return reportDetails;
        }
    }
}