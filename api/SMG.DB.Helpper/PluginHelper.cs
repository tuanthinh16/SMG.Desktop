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
        graphqlHelper client = new graphqlHelper();
        public PluginHelper() : base() {
            
        }

        // Semaphore để giới hạn số lượng thread kết nối đồng thời
        private static SemaphoreSlim semaphore = new SemaphoreSlim(50); // Ví dụ: tối đa 5 luồng

        // Phương thức lấy plugin từ cơ sở dữ liệu
        
        public List<Plugins> FetchPluginsAsync()
        {
            List<Plugins> lstplugins = new List<Plugins>();
            try
            {
                string query = @"
                  query Plugins { 
                      plugins { 
                          ID 
                          CREATE_TIME 
                          CREATOR 
                          MODIFIER 
                          MODIFY_TIME 
                          PLUGIN_NAME 
                          PLUGIN_LINK 
                          IS_ACTIVE 
                          PLUGIN_GROUP_ID 
                          PLUGIN_TYPE_ID 
                          ICON 
                      } 
                  }";
                var result = client.ExecuteQuery<PluginResponse>(query);

                // Nếu kết quả không null, thêm vào danh sách
                if (result != null && result.Plugins != null)
                {
                    lstplugins.AddRange(result.Plugins);
                }
                return lstplugins;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }
        public bool CreatePlugin(Plugins plugin)
        {
            try
            {

                // Câu lệnh mutation để tạo plugin mới
                string mutation = @"
                        mutation CreatePlugin($pluginName: String!, $pluginLink: String!, $isActive: Boolean!, $icon: String) {
                            createPlugin(pluginName: $pluginName, pluginLink: $pluginLink, isActive: $isActive, icon: $icon) {
                                success
                                plugin {
                                    ID
                                    PLUGIN_NAME
                                    CREATE_TIME
                                    MODIFIER
                                    MODIFY_TIME
                                    IS_ACTIVE
                                    PLUGIN_LINK
                                    ICON
                                }
                            }
                        }
                        ";

                // Định nghĩa các tham số để truyền vào mutation
                var variables = new
                {
                    pluginName = plugin.PLUGIN_NAME,
                    pluginLink = plugin.PLUGIN_LINK,
                    isActive = plugin.IS_ACTIVE,
                    icon = plugin.ICON
                };

                var result = client.ExecuteQuery<CreatePluginResponse>(mutation, variables);

                // Kiểm tra kết quả và trả về
                return result != null && result.CreatePlugin != null && result.CreatePlugin.Success;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
        }

        // Phản hồi từ GraphQL khi tạo plugin
        public class CreatePluginResponse
        {
            public CreatePluginData CreatePlugin { get; set; }
        }

        public class CreatePluginData
        {
            public bool Success { get; set; }
            public Plugins Plugin { get; set; }
        }


        public bool UpdatePlugin(Plugins plugin)
        {
            try
            {

                // Câu lệnh mutation để cập nhật plugin
                string mutation = @"
            mutation UpdatePlugin($id: Int!, $pluginName: String!, $pluginLink: String!, $isActive: Boolean!, $icon: String!) {
                updatePlugin(id: $id, pluginName: $pluginName, pluginLink: $pluginLink, isActive: $isActive, icon: $icon) {
                    success
                    plugin {
                        ID
                        PLUGIN_NAME
                        CREATE_TIME
                        MODIFIER
                        MODIFY_TIME
                        IS_ACTIVE
                        PLUGIN_LINK
                        ICON
                    }
                }
            }";

                // Định nghĩa các tham số để truyền vào mutation
                var variables = new
                {
                    id = plugin.ID,
                    pluginName = plugin.PLUGIN_NAME,
                    pluginLink = plugin.PLUGIN_LINK,
                    isActive = plugin.IS_ACTIVE,
                    icon = plugin.ICON
                };

                var result = client.ExecuteQuery<UpdatePluginResponse>(mutation, variables);

                // Kiểm tra kết quả và trả về
                return result != null && result.UpdatePlugin != null && result.UpdatePlugin.Success;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
        }

        public bool DeletePluginAsync(long iD)
        {
            throw new NotImplementedException();
        }

        // Phản hồi từ GraphQL khi cập nhật plugin
        public class UpdatePluginResponse
        {
            public UpdatePluginData UpdatePlugin { get; set; }
        }

        public class UpdatePluginData
        {
            public bool Success { get; set; }
            public Plugins Plugin { get; set; }
        }

        public class PluginResponse
        {
            public List<Plugins> Plugins { get; set; }
        }


    }
}
