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
    public class PluginHelper
    {
        public PluginHelper() : base() { }

        // Semaphore để giới hạn số lượng thread kết nối đồng thời
        private static SemaphoreSlim semaphore = new SemaphoreSlim(50); // Ví dụ: tối đa 50 luồng

        // Phương thức lấy plugin từ cơ sở dữ liệu
        public async Task<List<Plugins>> FetchPluginsAsync()
        {
            var plugins = new List<Plugins>();
            try
            {
                var dbHelper = new DBHelper();
                await semaphore.WaitAsync();

                string query = "SELECT * FROM SMN_PLUGINS";

                using (var connection = await dbHelper.OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                {
                    command.CommandTimeout = 30; // Set command timeout to 30 seconds
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            plugins.Add(new Plugins
                            {
                                ID = reader.GetInt64(reader.GetOrdinal("ID")),
                                PLUGIN_NAME = reader.GetString(reader.GetOrdinal("PLUGIN_NAME")),
                                PLUGIN_LINK = reader.GetString(reader.GetOrdinal("PLUGIN_LINK")),
                                PLUGIN_GROUP_ID = reader.IsDBNull(reader.GetOrdinal("PLUGIN_GROUP_ID")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("PLUGIN_GROUP_ID")),
                                PLUGIN_TYPE_ID = reader.IsDBNull(reader.GetOrdinal("PLUGIN_TYPE_ID")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("PLUGIN_TYPE_ID")),
                                ICON = reader.GetString(reader.GetOrdinal("ICON")),
                                CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                                CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                                MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                                MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                                IS_ACTIVE = reader.GetInt16(reader.GetOrdinal("IS_ACTIVE"))
                            });
                        }
                    }
                }
                
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error fetching plugins: " + ex.Message);
            }
            finally
            {
                semaphore.Release();
            }

            return plugins;
        }

        // Thêm Plugin mới
        public async Task<(bool, string)> CreatePluginAsync(Plugins plugin)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand(
                        "INSERT INTO SMN_PLUGINS (PLUGIN_NAME, PLUGIN_LINK, PLUGIN_GROUP_ID, PLUGIN_TYPE_ID, ICON, CREATE_TIME, CREATOR, IS_ACTIVE) " +
                        "VALUES (@PLUGIN_NAME, @PLUGIN_LINK, @PLUGIN_GROUP_ID, @PLUGIN_TYPE_ID, @ICON, @CREATE_TIME, @CREATOR, @IS_ACTIVE)", connection);

                    command.Parameters.AddWithValue("@PLUGIN_NAME", plugin.PLUGIN_NAME);
                    command.Parameters.AddWithValue("@PLUGIN_LINK", plugin.PLUGIN_LINK);
                    command.Parameters.AddWithValue("@PLUGIN_GROUP_ID", plugin.PLUGIN_GROUP_ID);
                    command.Parameters.AddWithValue("@PLUGIN_TYPE_ID", plugin.PLUGIN_TYPE_ID);
                    command.Parameters.AddWithValue("@ICON", plugin.ICON);
                    command.Parameters.AddWithValue("@CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now));
                    command.Parameters.AddWithValue("@CREATOR", plugin.CREATOR);
                    command.Parameters.AddWithValue("@IS_ACTIVE", 1);

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
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
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

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
                    if (!string.IsNullOrEmpty(plugin.ICON))
                    {
                        AddSubQuery(ref query, "ICON", plugin.ICON, ref parameters);
                    }
                    if (plugin.MODIFIER != null)
                    {
                        AddSubQuery(ref query, "MODIFIER", plugin.MODIFIER, ref parameters);
                    }

                    query += "MODIFY_TIME = @MODIFY_TIME, IS_ACTIVE = @IS_ACTIVE WHERE ID = @ID";

                    parameters.Add(new MySqlParameter("@MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    parameters.Add(new MySqlParameter("@IS_ACTIVE", plugin.IS_ACTIVE));
                    parameters.Add(new MySqlParameter("@ID", plugin.ID));

                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error updating plugin: " + ex.Message);
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

        // Xóa Plugin
        public async Task<(bool, string)> DeletePluginAsync(long id)
        {
            string error = string.Empty;
            try
            {
                var dbHelper = new DBHelper();
                using (var connection = await dbHelper.OpenConnectionAsync())
                {
                    var command = new MySqlCommand("DELETE FROM SMN_PLUGINS WHERE ID = @ID", connection);
                    command.Parameters.AddWithValue("@ID", id);

                    int result = command.ExecuteNonQuery();
                    return (result > 0, error);
                }
            }
            catch (MySqlException ex)
            {
                LogSystem.Error("Error deleting plugin: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }
    }
}