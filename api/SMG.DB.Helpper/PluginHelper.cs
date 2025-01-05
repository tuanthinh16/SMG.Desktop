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
    public class PluginHelper
    {
        public PluginHelper() : base() { }

        // Semaphore để giới hạn số lượng thread kết nối đồng thời
        private static SemaphoreSlim semaphore = new SemaphoreSlim(5); // Ví dụ: tối đa 5 luồng

        // Phương thức lấy plugin từ cơ sở dữ liệu
        public async Task<List<Plugins>> LoadPluginsFromDatabaseAsync(int? start, int? limit, string subQuery)
        {
            var plugins = new List<Plugins>();

            try
            {
                var dbHelper = new DBHelper();
                

                string query = @"SELECT *
                            FROM (
                                SELECT SMN_PLUGINS.*, ROW_NUMBER() OVER (ORDER BY CREATE_TIME) AS ROW_NUM
                                FROM SMN_PLUGINS
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
                            plugins.Add(new Plugins
                            {
                                ID = reader.GetInt64(reader.GetOrdinal("ID")),
                                PLUGIN_NAME = reader.GetString(reader.GetOrdinal("PLUGIN_NAME")),
                                PLUGIN_LINK = reader.GetString(reader.GetOrdinal("PLUGIN_LINK")),
                                IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE")),
                                CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                                CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                                MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                                MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? 0 : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                                PLUGIN_GROUP_ID = reader.IsDBNull(reader.GetOrdinal("PLUGIN_GROUP_ID")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("PLUGIN_GROUP_ID")),
                                PLUGIN_TYPE_ID = reader.IsDBNull(reader.GetOrdinal("PLUGIN_TYPE_ID")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("PLUGIN_TYPE_ID")),
                                ICON = reader.IsDBNull(reader.GetOrdinal("ICON")) ? null : reader.GetString(reader.GetOrdinal("ICON"))
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


        public async Task<List<Plugins>> FreeQueryAsync(string query)
        {
            List<Plugins> result = new List<Plugins>();
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
                            result.Add(new Plugins
                            {
                                ID = reader.GetInt64(reader.GetOrdinal("ID")),
                                PLUGIN_NAME = reader.GetString(reader.GetOrdinal("PLUGIN_NAME")),
                                PLUGIN_LINK = reader.GetString(reader.GetOrdinal("PLUGIN_LINK")),
                                IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE")),
                                CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                                CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                                MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                                MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? 0 : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                                PLUGIN_GROUP_ID = reader.IsDBNull(reader.GetOrdinal("PLUGIN_GROUP_ID")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("PLUGIN_GROUP_ID")),
                                PLUGIN_TYPE_ID = reader.IsDBNull(reader.GetOrdinal("PLUGIN_TYPE_ID")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("PLUGIN_TYPE_ID")),
                                ICON = reader.IsDBNull(reader.GetOrdinal("ICON")) ? null : reader.GetString(reader.GetOrdinal("ICON"))
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

        // Thêm Plugin mới
        public async Task<(bool, string)> AddPluginAsync(Plugins plugin)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand(
                        "INSERT INTO SMN_PLUGINS (PLUGIN_NAME, PLUGIN_LINK, IS_ACTIVE, CREATE_TIME, CREATOR, PLUGIN_GROUP_ID, ICON) " +
                        "VALUES (:PLUGIN_NAME, :PLUGIN_LINK, :IS_ACTIVE, :CREATE_TIME, :CREATOR, :PLUGIN_GROUP_ID, :ICON)", connection);

                    command.Parameters.Add(new OracleParameter(":PLUGIN_NAME", plugin.PLUGIN_NAME));
                    command.Parameters.Add(new OracleParameter(":PLUGIN_LINK", plugin.PLUGIN_LINK));
                    command.Parameters.Add(new OracleParameter(":IS_ACTIVE", plugin.IS_ACTIVE));
                    command.Parameters.Add(new OracleParameter(":CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    command.Parameters.Add(new OracleParameter(":CREATOR", plugin.CREATOR));
                    command.Parameters.Add(new OracleParameter(":PLUGIN_GROUP_ID", plugin.PLUGIN_GROUP_ID ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter(":ICON", plugin.ICON));

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);  // Trả về tuple (thành công, lỗi nếu có)
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error adding plugin: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }


        // Cập nhật Plugin
        public async Task<(bool, string)> UpdatePluginAsync(Plugins plugin)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var query = "UPDATE SMN_PLUGINS SET ";
                    List<OracleParameter> parameters = new List<OracleParameter>();

                    // Thêm các trường cần cập nhật
                    if (!string.IsNullOrEmpty(plugin.PLUGIN_NAME))
                    {
                        AddSubQuery(ref query, "PLUGIN_NAME", plugin.PLUGIN_NAME, ref parameters);
                    }
                    if (!string.IsNullOrEmpty(plugin.PLUGIN_LINK))
                    {
                        AddSubQuery(ref query, "PLUGIN_LINK", plugin.PLUGIN_LINK, ref parameters);
                    }
                    if (plugin.PLUGIN_GROUP_ID.HasValue)
                    {
                        AddSubQuery(ref query, "PLUGIN_GROUP_ID", plugin.PLUGIN_GROUP_ID, ref parameters);
                    }
                    if (plugin.PLUGIN_TYPE_ID.HasValue)
                    {
                        AddSubQuery(ref query, "PLUGIN_TYPE_ID", plugin.PLUGIN_TYPE_ID, ref parameters);
                    }
                    AddSubQuery(ref query, "IS_ACTIVE", plugin.IS_ACTIVE, ref parameters);
                    if (plugin.MODIFIER != null)
                    {
                        AddSubQuery(ref query, "MODIFIER", plugin.MODIFIER, ref parameters);
                    }
                    if (plugin.ICON != null)
                    {
                        AddSubQuery(ref query, "ICON", plugin.ICON, ref parameters);
                    }
                    query += "MODIFY_TIME = :MODIFY_TIME WHERE ID = :ID";

                    parameters.Add(new OracleParameter(":MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    parameters.Add(new OracleParameter(":ID", plugin.ID));

                    var command = new OracleCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);  // Trả về tuple (thành công, lỗi nếu có)
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error updating plugin: " + ex.Message);
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

        // Xóa Plugin
        public async Task<(bool, string)> DeletePluginAsync(long pluginId)
        {
            string error = string.Empty;
            try
            {
                if (pluginId <= 0)
                {
                    error = "Invalid plugin ID";
                    return (false, error);
                }

                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new OracleCommand("DELETE FROM SMN_PLUGINS WHERE ID = :ID", connection);
                    command.Parameters.Add(new OracleParameter(":ID", pluginId));

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);  // Trả về tuple (thành công, lỗi nếu có)
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error deleting plugin: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }

    }
}
