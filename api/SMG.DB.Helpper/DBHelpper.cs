using Oracle.ManagedDataAccess.Client;
using SMG.Logging;
using SMG.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SMG.DB.Helpper
{
    public class DBHelpper
    {
        private readonly string _connectionString;
        private OracleConnection _connection;

        // Semaphore để giới hạn số lượng thread kết nối đồng thời
        private static SemaphoreSlim semaphore = new SemaphoreSlim(5); // Ví dụ: tối đa 5 luồng

        // Constructor để khởi tạo chuỗi kết nối
        public DBHelpper()
        {
            string connectionString = "User Id=C##SMN_RS;Password=SMN_RS;Data Source=localhost:1522/orcl;persist security info=false;";
            _connectionString = connectionString;
        }

        // Phương thức mở kết nối, nếu kết nối chưa mở thì tạo kết nối mới
        public OracleConnection OpenConnection()
        {
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
            }
            return _connection;
        }

        
    }
}
