using Oracle.ManagedDataAccess.Client;
using SMG.Logging;
using SMG.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SMG.DB.Helpper
{
    public class PluginHelpper : DBHelpper
    {
        public PluginHelpper() : base() { }

        // Semaphore để giới hạn số lượng thread kết nối đồng thời
        private static SemaphoreSlim semaphore = new SemaphoreSlim(5); // Ví dụ: tối đa 5 luồng

        // Phương thức lấy plugin từ cơ sở dữ liệu
        public List<Plugins> LoadPluginsFromDatabase(int start, int limit)
        {
            var plugins = new List<Plugins>();

            try
            {
                // Thử khóa semaphore để hạn chế số lượng luồng đồng thời
                semaphore.Wait();

                using (var connection = OpenConnection())  // Dùng phương thức mở kết nối từ DBHelpper
                {
                    LogSystem.Debug("Connect successfully");

                    // Sử dụng tham số start và limit trong truy vấn để lấy dữ liệu theo phân trang
                    string query = string.Format(@"
                                SELECT ID, PLUGIN_NAME, PLUGIN_LINK, IS_ACTIVE, CREATE_TIME, CREATOR, MODIFIER, MODIFY_TIME, PLUGIN_GROUP_ID,PLUGIN_TYPE_ID,ICON
                                FROM (
                                    SELECT ID, PLUGIN_NAME, PLUGIN_LINK, IS_ACTIVE, CREATE_TIME, CREATOR, MODIFIER, MODIFY_TIME, PLUGIN_GROUP_ID,PLUGIN_TYPE_ID,ICON,
                                           ROW_NUMBER() OVER (ORDER BY CREATE_TIME) AS ROW_NUM
                                    FROM SMN_PLUGINS
                                    
                                )
                                WHERE ROW_NUM BETWEEN {0} AND {1}", start,limit);

                    // Cập nhật tham số start và end cho việc phân trang
                    var command = new OracleCommand(query, connection);

                    LogSystem.Debug(command.CommandText);

                    using (var reader = command.ExecuteReader())
                    {
                        // Đọc dữ liệu và ánh xạ vào đối tượng Plugin
                        while (reader.Read())
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
                // Log lỗi kết nối Oracle tại đây (nếu cần thiết)
                LogSystem.Error("Error: " + ex.Message);
            }
            finally
            {
                // Đảm bảo rằng semaphore được giải phóng sau khi hoàn thành
                semaphore.Release();
            }

            return plugins;
        }


        // Thêm Plugin mới
        public bool AddPlugin(Plugins plugin, ref string error)
        {
            try
            {
                using (var connection = OpenConnection())
                {
                    var command = new OracleCommand(
                        "INSERT INTO SMN_PLUGINS (PLUGIN_NAME, PLUGIN_LINK, IS_ACTIVE, CREATE_TIME, CREATOR, PLUGIN_GROUP_ID,ICON) " +
                        "VALUES (:PLUGIN_NAME, :PLUGIN_LINK, :IS_ACTIVE, :CREATE_TIME, :CREATOR, :PLUGIN_GROUP_ID,:ICON)", connection);

                    command.Parameters.Add(new OracleParameter(":PLUGIN_NAME", plugin.PLUGIN_NAME));
                    command.Parameters.Add(new OracleParameter(":PLUGIN_LINK", plugin.PLUGIN_LINK));
                    command.Parameters.Add(new OracleParameter(":IS_ACTIVE", plugin.IS_ACTIVE));
                    command.Parameters.Add(new OracleParameter(":CREATE_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    command.Parameters.Add(new OracleParameter(":CREATOR", plugin.CREATOR));
                    command.Parameters.Add(new OracleParameter(":ICON", plugin.ICON));
                    command.Parameters.Add(new OracleParameter(":PLUGIN_GROUP_ID", plugin.PLUGIN_GROUP_ID ?? (object)DBNull.Value));

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error adding plugin: " + ex.Message);
                error = ex.Message;
                return false;
            }
        }


        // Cập nhật Plugin
        public bool UpdatePlugin(Plugins plugin, ref string error)
        {
            try
            {
                using (var connection = OpenConnection())
                {
                    // Xây dựng câu lệnh SQL với các trường cần cập nhật
                    var query = "UPDATE SMN_PLUGINS SET ";

                    // Kiểm tra từng trường có dữ liệu để cập nhật
                    List<OracleParameter> parameters = new List<OracleParameter>();
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

                    // Thêm tham số cho MODIFY_TIME và ID
                    parameters.Add(new OracleParameter(":MODIFY_TIME", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now)));
                    parameters.Add(new OracleParameter(":ID", plugin.ID));

                    // Tạo câu lệnh SQL cuối cùng
                    var command = new OracleCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error updating plugin: " + ex.Message);
                error = ex.Message;
                return false;
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
        public bool DeletePlugin(long pluginId, ref string error)
        {
            try
            {
                if(pluginId <= 0)
                {
                    error = "Invalid plugin ID";
                    return false;
                }
                using (var connection = OpenConnection())
                {
                    var command = new OracleCommand("DELETE FROM SMN_PLUGINS WHERE ID = :ID", connection);
                    command.Parameters.Add(new OracleParameter(":ID", pluginId));

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error deleting plugin: " + ex.Message);
                error = ex.Message;
                return false;
            }
        }
    }
}
