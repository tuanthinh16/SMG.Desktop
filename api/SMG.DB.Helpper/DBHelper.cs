using MySql.Data.MySqlClient;
using SMG.Logging;
using SMG.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace SMG.DB.Helper
{
    public class DBHelper
    {
        private readonly string _connectionString;
        private static SemaphoreSlim semaphore = new SemaphoreSlim(20); // Giới hạn tối đa 20 kết nối đồng thời

        // Constructor khởi tạo chuỗi kết nối
        public DBHelper()
        {
            _connectionString = "Server=localhost;Port=3306;Database=SMN_RS;User ID=root;Password=;SslMode=None;";

        }

        // Phương thức mở kết nối MySQL bất đồng bộ
        public async Task<MySqlConnection> OpenConnectionAsync()
        {
            await semaphore.WaitAsync();
            var connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                connection.Dispose();
                semaphore.Release();
                throw;
            }
        }

        // Phương thức thực hiện truy vấn SELECT trả về danh sách
        public async Task<List<T>> ExecuteQueryAsync<T>(string query, Func<MySqlDataReader, T> mapFunction)
        {
            var result = new List<T>();

            try
            {
                using (var connection = await OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(mapFunction(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                semaphore.Release();
            }

            return result;
        }

        // Phương thức thực hiện truy vấn INSERT, UPDATE, DELETE
        public async Task<int> ExecuteNonQueryAsync(string query)
        {
            int affectedRows = 0;
            try
            {
                using (var connection = await OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                {
                    affectedRows = await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                semaphore.Release();
            }
            return affectedRows;
        }

        // Phương thức thực hiện truy vấn trả về giá trị đơn lẻ (ví dụ: COUNT, SUM)
        public async Task<object> ExecuteScalarAsync(string query)
        {
            object result = null;
            try
            {
                using (var connection = await OpenConnectionAsync())
                using (var command = new MySqlCommand(query, connection))
                {
                    result = await command.ExecuteScalarAsync();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                semaphore.Release();
            }
            return result;
        }
    }
}
