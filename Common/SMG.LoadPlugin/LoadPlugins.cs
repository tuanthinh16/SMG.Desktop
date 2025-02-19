﻿using DevExpress.XtraTab;
using SMG.DBConfig;
using SMG.Logging;
using SMG.Module;
using SMG.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;

namespace SMG.LoadPlugin
{
    public class LoadPlugins
    {
        public LoadPlugins()
        {
            
        }
        public LoadPlugins(XtraTabPage tab, SMG.Models.Plugins plugin, ref string error, List<object> listArgs)
        {
            if (plugin != null)
            {
                if(plugin.PLUGIN_TYPE_ID == PluginType.PLUGIN_TYPE_ID__UC && tab != null)
                {
                    tab = LoadUC(plugin, ref error, listArgs);
                }
                else
                {
                    OpenPlugin(plugin.PLUGIN_LINK, ref error, listArgs);
                }
            }

        }
        
        public static bool OpenPlugin(string pluginLink, ref string error, List<object> listArgs)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
                // Get the directory where the executable is running
                string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                // Construct the full path to the plugin
                string pluginPath = Path.Combine(baseDirectory, "Plugins", pluginLink + ".dll");

                // Check if the file exists
                // Kiểm tra plugin có tồn tại không
                if (File.Exists(pluginPath))
                {
                    // Load plugin assembly (DLL)
                    Assembly pluginAssembly = Assembly.LoadFrom(pluginPath);

                    // Tìm các lớp triển khai IPlugin trong assembly
                    var pluginTypes = pluginAssembly.GetTypes()
                        .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var pluginType in pluginTypes)
                    {
                        // Tạo instance của lớp plugin
                        var pluginInstance = Activator.CreateInstance(pluginType);

                        // Gọi phương thức Execute của plugin
                        if (pluginInstance is IPlugin plugin)
                        {
                            plugin.Load(listArgs); 
                        }
                    }

                    return true;
                }
                else
                {
                    error = $"Plugin không tồn tại tại: {pluginPath}";
                    MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                error = ($"Có lỗi xảy ra khi mở module: {ex.Message}");
                return false;
            }
        }
        public static XtraTabPage LoadUC(SMG.Models.Plugins plugin, ref string error, List<object> listArgs)
        {
            XtraTabPage result = null;
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
                // Get the directory where the executable is running
                string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                // Construct the full path to the plugin
                string pluginPath = Path.Combine(baseDirectory, "Plugins", plugin.PLUGIN_LINK + ".dll");
                // Check if the file exists
                // Kiểm tra plugin có tồn tại không
                if (File.Exists(pluginPath))
                {
                    // Load plugin assembly (DLL)
                    Assembly pluginAssembly = Assembly.LoadFrom(pluginPath);
                    // Tìm các lớp triển khai IPlugin trong assembly
                    // 2. Tìm UserControl
                    Type[] types = pluginAssembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsSubclassOf(typeof(System.Windows.Forms.UserControl)))
                        {
                            // Kiểm tra xem lớp có constructor không tham số
                            ConstructorInfo constructorNoParam = type.GetConstructor(Type.EmptyTypes);

                            // Kiểm tra xem lớp có constructor nhận tham số ModuleData
                            ConstructorInfo constructorWithParam = type.GetConstructor(new Type[] { typeof(ModuleData) });

                            if (constructorWithParam != null)
                            {
                                // Tạo đối tượng từ constructor với tham số ModuleData
                                ModuleData data = new ModuleData();  // Tạo hoặc lấy giá trị thích hợp cho tham số
                                System.Windows.Forms.UserControl pluginControl =
                                    (System.Windows.Forms.UserControl)constructorWithParam.Invoke(new object[] { data });

                                // Tiến hành xử lý như trước
                                XtraTabPage tabPage = new XtraTabPage();
                                tabPage.Text = plugin.PLUGIN_NAME; // Đặt tên tab
                                tabPage.ShowCloseButton = DevExpress.Utils.DefaultBoolean.True;
                                tabPage.Controls.Add(pluginControl);
                                pluginControl.Dock = DockStyle.Fill; // Để UserControl lấp đầy TabPage
                                tabPage.Appearance.Header.ForeColor = System.Drawing.Color.Green;
                                tabPage.Appearance.Header.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Bold);

                                result = tabPage;
                                break;
                            }
                            else if (constructorNoParam != null)
                            {
                                // Nếu chỉ có constructor không tham số
                                System.Windows.Forms.UserControl pluginControl =
                                    (System.Windows.Forms.UserControl)constructorNoParam.Invoke(null);

                                // Tiến hành xử lý như trước
                                XtraTabPage tabPage = new XtraTabPage();
                                tabPage.Text = plugin.PLUGIN_NAME; // Đặt tên tab
                                tabPage.ShowCloseButton = DevExpress.Utils.DefaultBoolean.True;
                                tabPage.Controls.Add(pluginControl);
                                pluginControl.Dock = DockStyle.Fill; // Để UserControl lấp đầy TabPage
                                tabPage.Appearance.Header.ForeColor = System.Drawing.Color.Green;
                                tabPage.Appearance.Header.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Bold);

                                result = tabPage;
                                break;
                            }
                            else
                            {
                                error = "Plugin không có constructor hợp lệ.";
                                return null;
                            }
                        }
                    }

                }
                else
                {
                    error = $"Plugin không tồn tại tại: {pluginPath}";
                    MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                error = ($"Có lỗi xảy ra khi mở module: {ex.Message}");
                return null;
            }
            return result;
        }
        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            // Get the directory where the executable is running
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Specify the Libs directory
            string libsDirectory = Path.Combine(baseDirectory, "Libs");

            // Extract the assembly name
            string assemblyName = new AssemblyName(args.Name).Name;

            // Ensure the file extension is included for loading the assembly
            string assemblyPath = Path.Combine(libsDirectory, assemblyName + ".dll");

            // Check if the DLL exists in the Libs directory and load it if available
            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }

            // Return null if the assembly is not found
            return null;
        }
    }
}
