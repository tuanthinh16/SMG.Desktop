using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraTab;
using SMG.DB.Helpper;
using SMG.LoadPlugin;
using SMG.Logging;
using SMG.Models;
using SMG.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMG.Desktop
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            this.Text = "SERVICES MANAGEMENT SYSTEM";
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                this.xtraTabControl1.Dock = DockStyle.Fill;
                string icon_path = Path.Combine(Application.StartupPath, "Img", "Icon", "icon.ico");
                this.Icon = new System.Drawing.Icon(icon_path);
                LoadMenu();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadMenu()
        {
            try
            {
                List<SMG.Models.Plugins> plugins = new List<SMG.Models.Plugins>();
                plugins = LoadPluginData().Where(s=>s.IS_ACTIVE == SMG.DBConfig.Common.IS_ACTIVE__TRUE).ToList();
                RibbonPage ribbonPage = this.ribbonControl1.Pages[0]; 
                RibbonPageGroup ribbonPageGroup = new RibbonPageGroup("Plugins"); 

                foreach (SMG.Models.Plugins plugin in plugins)
                {
                    BarButtonItem barButtonItem = new BarButtonItem();
                    barButtonItem.Caption = plugin.PLUGIN_NAME;
                    barButtonItem.Name = "barButtonItem_" + plugin.PLUGIN_NAME.Replace(" ", "_");
                    barButtonItem.Appearance.Font = new Font("Tahoma", 9, FontStyle.Bold);
                    barButtonItem.Appearance.ForeColor = Color.Gray;
                    if (plugin.ICON != null)
                    {
                        string fullPath = Path.Combine(Application.StartupPath,"Img","Icon", "Plugin", plugin.ICON);
                        if (File.Exists(fullPath))
                        {
                            barButtonItem.LargeGlyph = Image.FromFile(fullPath);
                        }
                        else
                        {
                            barButtonItem.LargeGlyph = Image.FromFile(Path.Combine(Application.StartupPath, "Img", "Icon", "Plugin", "Home.png"));
                        }
                    }
                    string error = string.Empty;
                    List<object> listArgs = new List<object>();

                    barButtonItem.ItemClick += (sender, e) =>
                    {
                        if(plugin.PLUGIN_TYPE_ID == 1)
                        {
                            
                            //this.xtraTabControl1.TabPages.Clear();
                            XtraTabPage tab = LoadPlugins.LoadUC(plugin, ref error, listArgs);
                            if(tab != null)
                            {
                                this.xtraTabControl1.TabPages.Add(tab);
                                this.xtraTabControl1.SelectedTabPage = tab;
                                
                            }
                            



                        }
                        else
                            LoadPlugins.OpenPlugin(plugin.PLUGIN_LINK, ref error, listArgs);
                    };
                    BarButtonItemLink itemLink = (BarButtonItemLink)ribbonPageGroup.ItemLinks.Add(barButtonItem);
                    itemLink.BeginGroup = true;
                    

                }

                ribbonPage.Groups.Add(ribbonPageGroup);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private List<Models.Plugins> LoadPluginData()
        {
            List<SMG.Models.Plugins> result = new List<Models.Plugins>();
            try
            {
                List<SMG.Models.Plugins> lstPlugins = new List<SMG.Models.Plugins>();
                PluginHelpper pluginHelpper = new PluginHelpper();
                lstPlugins = pluginHelpper.LoadPluginsFromDatabase(0, 100);
                if (lstPlugins != null && lstPlugins.Count > 0)
                {
                    result.AddRange(lstPlugins);
                }
                result.Add(new Models.Plugins() { ID = 3, PLUGIN_NAME = "Plugins", IS_ACTIVE=1,PLUGIN_LINK = "SMG.Plugins.ListPlugin", PLUGIN_TYPE_ID = 2, ICON = "module.png" });
            }
            catch (Exception ex)
            {
                result.Clear();
                LogSystem.Error(ex);
            }
            return result;
        }

        private void xtraTabControl1_CloseButtonClick(object sender, EventArgs e)
        {
            try
            {
                var closeArgs = e as DevExpress.XtraTab.ViewInfo.ClosePageButtonEventArgs;
                if (closeArgs?.Page is XtraTabPage tabPage)
                {
                    xtraTabControl1.TabPages.Remove(tabPage);
                    tabPage.Dispose(); // Giải phóng tài nguyên
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
    }
}
