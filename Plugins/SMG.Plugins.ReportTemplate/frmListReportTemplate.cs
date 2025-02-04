using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using SMG.Logging;
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

namespace SMG.Plugins.ReportTemplate
{
    public partial class frmListReportTemplate : Form
    {
        List<SMG.Models.Report> listReport = null;
        List<SMG.Models.ReportType> listReportType = null;
        int actionType = -1;
        public frmListReportTemplate()
        {
            InitializeComponent();
        }

        private void frmListReportTemplate_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                InitDataToGridControl();
                InitDatatoCboType();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void InitDatatoCboType()
        {
            try
            {
                SMG.DB.Helpper.ReportTypeHelper reportTypeHelper = new DB.Helpper.ReportTypeHelper();
                string query = string.Empty;
                query += " AND IS_ACTIVE = 1";
                var rs = reportTypeHelper.GetReportTypesAsync();
                if (rs != null)
                {
                    this.listReportType = rs.Result;
                    if (this.listReportType != null)
                    {
                        this.cboType.Properties.DataSource = this.listReportType;
                        this.cboType.Properties.DisplayMember = "REPORT_TYPE_NAME";
                        this.cboType.Properties.ValueMember = "REPORT_TYPE_CODE";
                        cboType.Properties.View.Columns.Clear();
                        cboType.Properties.View.Columns.AddField("REPORT_TYPE_CODE").Visible = true;
                        cboType.Properties.View.Columns.AddField("REPORT_TYPE_NAME").Visible = true; 
                        // Thiết lập các thuộc tính của view nếu cần
                        cboType.Properties.View.OptionsView.ShowColumnHeaders = false; // Ẩn tiêu đề cột nếu không cần
                        cboType.Properties.View.OptionsView.ShowIndicator = false; // Ẩn chỉ báo dòng
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitDataToGridControl()
        {
            try
            {
                LoadDataReportType();

                this.gridControlReportTemplate.DataSource = this.listReport;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDataReportType()
        {
            try
            {
                SMG.DB.Helpper.ReportHelper reportTypeHelper = new SMG.DB.Helpper.ReportHelper();
                string query = string.Empty;
                query += " ORDER BY CREATE_TIME DESC";
                var rs = reportTypeHelper.LoadReportsFromDatabaseAsync(null, null, query);
                if (rs != null)
                {
                    this.listReport = rs.Result;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                txtCode.Text = txtName.Text = "";
                cboType.EditValue = null;
                this.actionType = GlobalVariables.ActionType.ACCTION__ADD;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private bool ValiadtionControlRequired(Control control)
        {
            bool result = true;
            try
            {
                if (control != null)
                {
                    if (control.Text == null || string.IsNullOrEmpty(control.Text))
                    {
                        dxErrorProvider1.SetError(control, "Trường dữ liệu bắt buộc", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                        result = false;
                    }
                    else
                    {
                        dxErrorProvider1.SetError(control, "", DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        private void gridViewReportTemplate_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var data = (SMG.Models.Report)gridViewReportTemplate.GetRow(e.ListSourceRowIndex);
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = SMG.DateTimeHelpper.Convert.TimeNumberToDateTime(data.CREATE_TIME)?.ToString("dd/MM/yyyy HH:mm:ss");
                    }
                    if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = SMG.DateTimeHelpper.Convert.TimeNumberToDateTime(data.MODIFY_TIME??0)?.ToString("dd/MM/yyyy HH:mm:ss");
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewReportTemplate_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = (SMG.Models.Report)gridViewReportTemplate.GetFocusedRow();
                if (data != null)
                {
                    FillDataToEditControl(data);
                    this.actionType = GlobalVariables.ActionType.ACCTION__EDIT;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataToEditControl(Models.Report data)
        {
            try
            {
                txtCode.Text = data.REPORT_CODE;
                txtName.Text = data.REPORT_NAME;
                cboType.EditValue = data.REPORT_TYPE_CODE;

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if(e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboType.EditValue = null;   
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValiadtionControlRequired(txtCode) || !ValiadtionControlRequired(txtName) || !ValiadtionControlRequired(cboType)) return;
                ProcessSave();
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
                SMG.Models.Report report = new SMG.Models.Report();
                report.REPORT_CODE = txtCode.Text;
                report.REPORT_NAME = txtName.Text;
                report.REPORT_TYPE_CODE = cboType.EditValue.ToString();
                if (TokenManager.TokenManager.IsLoggedIn())
                {

                    if (this.actionType == GlobalVariables.ActionType.ACCTION__EDIT)
                        report.MODIFIER = TokenManager.TokenManager.GetUsername();
                    else
                        report.CREATOR = TokenManager.TokenManager.GetUsername();
                }
                else
                {

                    if (this.actionType == GlobalVariables.ActionType.ACCTION__EDIT)
                        report.MODIFIER = "ADMIN";
                    else
                        report.CREATOR = "ADMIN";
                }
                SMG.DB.Helpper.ReportHelper reportHelper = new SMG.DB.Helpper.ReportHelper();
                string error = string.Empty;
                var result = this.actionType == GlobalVariables.ActionType.ACCTION__ADD ? reportHelper.AddReportAsync(report) : reportHelper.UpdateReportAsync(report);
                if (result != null && result.Result.Item1)
                {

                    MessageBox.Show("Xử lý thành công!");
                    SetDefaultControl();
                    InitDataToGridControl();

                }
                else
                {
                    MessageBox.Show(result.Result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repoDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (SMG.Models.Report)gridViewReportTemplate.GetFocusedRow();
                if (data != null)
                {
                    if (MessageBox.Show("Bạn có chắc chắn muốn xóa bản ghi này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SMG.DB.Helpper.ReportHelper reportHelper = new SMG.DB.Helpper.ReportHelper();
                        string error = string.Empty;
                        var result = reportHelper.DeleteReportAsync(data.ID);
                        if (result != null && result.Result.Item1)
                        {
                            MessageBox.Show("Xử lý thành công!");
                            SetDefaultControl();
                            InitDataToGridControl();
                        }
                        else
                        {
                            MessageBox.Show(result.Result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repoLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (SMG.Models.Report)gridViewReportTemplate.GetFocusedRow();
                if (data != null)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();


                    openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                    openFileDialog.Title = "Select a file";


                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {

                        string fullFilePath = openFileDialog.FileName;
                        string fileName = Path.GetFileName(fullFilePath);

                        data.REPORT_FILE_NAME = fileName;
                        SMG.DB.Helpper.ReportHelper reportHelper = new SMG.DB.Helpper.ReportHelper();
                        string error = string.Empty;
                        var result = reportHelper.UpdateReportAsync(data);
                        if (result != null)
                        {
                            if (result.Result.Item1 && SaveFileToFolder(fileName, fullFilePath))
                            {
                                MessageBox.Show("Xử lý thành công!");
                                SetDefaultControl();
                                InitDataToGridControl();
                            }
                            else
                            {
                                MessageBox.Show(result.Result.Item2.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private bool SaveFileToFolder(string filename, string filePath)
        {
            bool result = true;
            try
            {
                string fullPath = Path.Combine(Application.StartupPath, "Plugins", "Report", "Tmp", filename);

                // Ghi đè file nếu đã tồn tại
                File.Copy(filePath, fullPath, true);
            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Error(ex);
            }
            return result;
        }


        private void repoDownFile_Click(object sender, EventArgs e)
        {

        }
    }
}
