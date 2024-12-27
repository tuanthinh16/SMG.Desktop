using DevExpress.Utils.Svg;
using SMG.DB.Helpper;
using SMG.Logging;
using SMG.Models;
using SMG.TokenManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMG.Plugins.ListPlugin
{
    public partial class frmListPlugin : Form
    {
        private int ActionType = -1;
        SMG.Models.Plugins currentPlugin = null;
        PluginHelpper pluginHelpper = null;
        public frmListPlugin()
        {
            InitializeComponent();
        }
        private void frmListPlugin_Load(object sender, EventArgs e)
        {
            try
            {
                pluginHelpper = new PluginHelpper();
                ActionType = SMG.GlobalVariables.ActionType.ACCTION__ADD;
                InitDataToGrid();
                InitDataCboType();
                SetDefaultValue();
                SetDefaultControlState();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDefaultControlState()
        {
            try
            {
                
                btnAdd.Enabled = ActionType == SMG.GlobalVariables.ActionType.ACCTION__ADD;
                btnEdit.Enabled = ActionType == SMG.GlobalVariables.ActionType.ACCTION__EDIT;
                this.layoutControlItemToolTip.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            txtGroup.Text = "";
            txtModuleName.Text = "";
            txtModuleLink.Text = "";
            cboType.EditValue = null;
        }

        class PluginType
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
        private void InitDataCboType()
        {
            try
            {
                List<PluginType> pluginTypes = new List<PluginType>();
                pluginTypes.Add(new PluginType { ID = 1, Name = "UC" });
                pluginTypes.Add(new PluginType { ID = 2, Name = "Form" });

                // Đặt DataSource cho GridLookUpEdit
                cboType.Properties.DataSource = pluginTypes;
                cboType.Properties.DisplayMember = "Name";
                cboType.Properties.ValueMember = "ID";

                // Định cấu hình các cột trong GridLookUpEdit
                cboType.Properties.View.Columns.Clear();
                cboType.Properties.View.Columns.AddField("ID").Visible = false; // Ẩn cột ID nếu không cần hiển thị
                cboType.Properties.View.Columns.AddField("Name").Visible = true; // Hiển thị cột Name

                // Thiết lập các thuộc tính của view nếu cần
                cboType.Properties.View.OptionsView.ShowColumnHeaders = false; // Ẩn tiêu đề cột nếu không cần
                cboType.Properties.View.OptionsView.ShowIndicator = false; // Ẩn chỉ báo dòng
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitDataToGrid()
        {
            try
            {
                List<SMG.Models.Plugins> lstPlugins = new List<SMG.Models.Plugins>();

                PluginHelpper pluginHelpper = new PluginHelpper();
                lstPlugins = pluginHelpper.LoadPluginsFromDatabase(0, 100);
                if(lstPlugins != null && lstPlugins.Count > 0)
                {
                    gridControlPlugin.DataSource = lstPlugins;
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewPlugin_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var data = (SMG.Models.Plugins)gridViewPlugin.GetRow(e.ListSourceRowIndex);
                if (data != null)
                {
                    
                    if (e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            DateTime create = SMG.DateTimeHelpper.Convert.TimeNumberToDateTime(data.MODIFY_TIME);
                            e.Value = create != DateTime.MinValue ? create.ToString("dd/MM/yyyy hh:MM:ss") : null;
                        }
                        if(e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            DateTime create = SMG.DateTimeHelpper.Convert.TimeNumberToDateTime(data.CREATE_TIME);
                            e.Value = create != DateTime.MinValue ? create.ToString("dd/MM/yyyy hh:MM:ss") : null ;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }


        private void gridViewPlugin_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var data = (SMG.Models.Plugins)gridViewPlugin.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "LOCK")
                    {
                        if (data.IS_ACTIVE == 0)
                        {
                            e.RepositoryItem = repoLock;
                        }
                        else
                        {
                            e.RepositoryItem = repoUnlock;
                        }
                    }
                    if(e.Column.FieldName == "DELETE")
                    {
                        if (data.IS_ACTIVE == 1)
                        {
                            e.RepositoryItem = repoDeleteE;
                        }
                        else
                        {
                            e.RepositoryItem = repoDeleteD;
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewPlugin_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = (SMG.Models.Plugins)gridViewPlugin.GetFocusedRow();
                if(data != null)
                {

                    FillDataToEditControl(data);
                    ActionType = SMG.GlobalVariables.ActionType.ACCTION__EDIT;
                    SetDefaultControlState();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataToEditControl(Models.Plugins data)
        {
            try
            {
                txtModuleName.Text = data.PLUGIN_NAME;
                txtModuleLink.Text = data.PLUGIN_LINK;
                cboType.EditValue = data.PLUGIN_TYPE_ID;
                layoutControlItemToolTip.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                labelControlTooltip.Text = "Bạn cần tải lên lại ảnh trước khi lưu";
                labelControlTooltip.Appearance.ForeColor = Color.Red;
                this.currentPlugin = data;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewPlugin_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                var data = (SMG.Models.Plugins)gridViewPlugin.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "STATUS")
                    {
                        if (data.IS_ACTIVE == 1)
                        {
                            e.DisplayText = "Active";
                            e.Appearance.ForeColor = Color.Green;
                        }
                        else
                        {
                            e.DisplayText = "Inactive";
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validation.ValidationRequiredControl(txtModuleName, dxErrorProvider1) || !Validation.ValidationRequiredControl(txtModuleLink, dxErrorProvider1)) return;
                

                ProcessSave();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private string FILE_NAME { get; set; }
        private string FILE_PATH { get; set; }
        private void SaveImage()
        {
            try
            {
                string fullPath = Path.Combine(Application.StartupPath, "Img", "Icon", "Plugin", this.FILE_NAME);
                File.Copy(this.FILE_PATH, fullPath);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void ProcessSave()
        {
            try
            {
                SMG.Models.Plugins plugins = new SMG.Models.Plugins();
                if(this.currentPlugin != null)
                {
                     plugins = this.currentPlugin;
                }
                plugins.PLUGIN_NAME = txtModuleName.Text;
                plugins.PLUGIN_LINK = txtModuleLink.Text;
                if (cboType.EditValue != null &&  Int64.TryParse(cboType.EditValue.ToString(), out long type))
                {
                    plugins.PLUGIN_TYPE_ID = type;
                }

                plugins.IS_ACTIVE = 1;
                if (TokenManager.TokenManager.IsLoggedIn())
                {
                    
                    if (ActionType == GlobalVariables.ActionType.ACCTION__EDIT)
                        plugins.MODIFIER = TokenManager.TokenManager.GetUsername();
                    else
                        plugins.CREATOR = TokenManager.TokenManager.GetUsername();
                }
                else
                {

                    if (ActionType == GlobalVariables.ActionType.ACCTION__EDIT)
                        plugins.MODIFIER = "ADMIN";
                    else
                        plugins.CREATOR = "ADMIN";
                }
                plugins.ICON = this.FILE_NAME;
                if (Int64.TryParse(txtGroup.Text.ToString(), out long gr))
                {
                    plugins.PLUGIN_GROUP_ID = gr;
                }
                
                string error = string.Empty;
                bool action_result = false;

                action_result = ActionType == GlobalVariables.ActionType.ACCTION__ADD?  pluginHelpper.AddPlugin(plugins, ref error): pluginHelpper.UpdatePlugin(plugins, ref error);
                if (action_result)
                {
                    MessageBox.Show("Xử lý thành công!");
                    SaveImage();
                    InitDataToGrid();

                }
                else
                {
                    MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void checkButton1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                
                openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"; 
                openFileDialog.Title = "Select a file";

                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    
                    string fullFilePath = openFileDialog.FileName;  
                    string fileName = Path.GetFileName(fullFilePath); 

                    this.FILE_NAME = fileName;
                    this.FILE_PATH = fullFilePath;
                    
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validation.ValidationRequiredControl(txtModuleName, dxErrorProvider1) || !Validation.ValidationRequiredControl(txtModuleLink, dxErrorProvider1)) return;


                ProcessSave();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            SetDefaultControlState();
            SetDefaultValue();
        }

        private void repoLock_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (SMG.Models.Plugins)gridViewPlugin.GetFocusedRow();
                if(data != null)
                {
                    data.IS_ACTIVE = 1;
                    string error = string.Empty;
                    if (pluginHelpper.UpdatePlugin(data, ref error))
                    {
                        MessageBox.Show("Xử lý thành công!");
                        SaveImage();
                        InitDataToGrid();

                    }
                    else
                    {
                        MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repoUnlock_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (SMG.Models.Plugins)gridViewPlugin.GetFocusedRow();
                if (data != null)
                {
                    data.IS_ACTIVE = 0;
                    string error = string.Empty;
                    if (pluginHelpper.UpdatePlugin(data, ref error))
                    {
                        MessageBox.Show("Xử lý thành công!");
                        SaveImage();
                        InitDataToGrid();

                    }
                    else
                    {
                        MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repoDeleteE_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (SMG.Models.Plugins)gridViewPlugin.GetFocusedRow();
                if (data != null)
                {
                    
                    string error = string.Empty;
                    if (MessageBox.Show(this,"Bạn có chắc muốn xóa bỏ dữ liệu ?","Thông báo",MessageBoxButtons.YesNo)== DialogResult.Yes &&  pluginHelpper.DeletePlugin(data.ID, ref error))
                    {
                        MessageBox.Show("Xử lý thành công!");
                        SaveImage();
                        InitDataToGrid();

                    }
                    else
                    {
                        MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
    }
}
