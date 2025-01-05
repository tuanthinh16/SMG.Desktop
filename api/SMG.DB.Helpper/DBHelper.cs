using Oracle.ManagedDataAccess.Client;
using SMG.Logging;
using SMG.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SMG.DB.Helper
{
    public class DBHelper
    {
        private readonly string _connectionString;
        private OracleConnection _connection;

        // Semaphore để giới hạn số lượng thread kết nối đồng thời
        private static SemaphoreSlim semaphore = new SemaphoreSlim(20); // Ví dụ: tối đa 5 luồng

        // Constructor để khởi tạo chuỗi kết nối
        public DBHelper()
        {
            string connectionString = "User Id=C##SMN_RS;Password=SMN_RS;Data Source=localhost:1522/orcl;persist security info=false;";

            _connectionString = connectionString;
        }

        // Phương thức mở kết nối bất đồng bộ, nếu kết nối chưa mở thì tạo kết nối mới
        public async Task<OracleConnection> OpenConnectionAsync()
        {
            await semaphore.WaitAsync();  
            try
            {
                if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
                {
                    _connection = new OracleConnection(_connectionString);
                    _connection.Open();  
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                semaphore.Release(); 
                throw;
            }
            return _connection;
        }


        // Phương thức đóng kết nối
        public void CloseConnection()
        {
            try
            {
                if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                semaphore.Release();  // Giải phóng semaphore sau khi kết thúc kết nối
            }
        }

        // Phương thức thực hiện truy vấn bất đồng bộ
        public async Task<List<T>> ExecuteQueryAsync<T>(string query, Func<OracleDataReader, T> mapFunction)
        {
            var result = new List<T>();

            try
            {
                using (var connection = await OpenConnectionAsync())
                {
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(mapFunction(reader));
                            }
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

    }
}
