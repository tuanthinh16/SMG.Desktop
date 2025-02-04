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
    public class UserHelper
    {
        public UserHelper() :base() { }
        private static SemaphoreSlim semaphore = new SemaphoreSlim(5);
        public async Task<List<User>> FreeQueryAsync(string query)
        {
            List<User> result = new List<User>();
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
                            result.Add(new User
                            {
                                ID = reader.GetInt64(reader.GetOrdinal("ID")),
                                USERNAME = reader.GetString(reader.GetOrdinal("USERNAME")),
                                LOGINNAME = reader.GetString(reader.GetOrdinal("LOGINNAME")),
                                FULLNAME = reader.IsDBNull(reader.GetOrdinal("FULLNAME")) ? null : reader.GetString(reader.GetOrdinal("FULLNAME")),
                                ADDRESS = reader.IsDBNull(reader.GetOrdinal("ADDRESS")) ? null : reader.GetString(reader.GetOrdinal("ADDRESS")),
                                PHONE = reader.IsDBNull(reader.GetOrdinal("PHONE")) ? null : reader.GetString(reader.GetOrdinal("PHONE")),
                                EMAIL = reader.IsDBNull(reader.GetOrdinal("EMAIL")) ? null : reader.GetString(reader.GetOrdinal("EMAIL")),
                                ROLE = reader.IsDBNull(reader.GetOrdinal("ROLE")) ? null : reader.GetString(reader.GetOrdinal("ROLE")),
                                CREATE_TIME = reader.GetInt64(reader.GetOrdinal("CREATE_TIME")),
                                CREATOR = reader.GetString(reader.GetOrdinal("CREATOR")),
                                MODIFY_TIME = reader.IsDBNull(reader.GetOrdinal("MODIFY_TIME")) ? 0 : reader.GetInt64(reader.GetOrdinal("MODIFY_TIME")),
                                MODIFIER = reader.IsDBNull(reader.GetOrdinal("MODIFIER")) ? null : reader.GetString(reader.GetOrdinal("MODIFIER")),
                                IS_ACTIVE = reader.GetBoolean(reader.GetOrdinal("IS_ACTIVE"))
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
    }
}
