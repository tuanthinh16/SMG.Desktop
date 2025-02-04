using DevExpress.XtraEditors;
using SMG.DB.Helpper;
using SMG.GlobalVariables;
using SMG.Logging;
using SMG.Models;
using SMG.Notify.Toast;
using SMG.TokenManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMG.Plugins.ReportType
{
    public partial class frmListReportType : Form
    {
        List<SMG.Models.ReportType> listReportType = null;
        int actionType = -1;
        public frmListReportType()
        {
            InitializeComponent();
        }

        private void frmListReportType_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                InitDataToGridControl();
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
                
                this.gridControlListReportType.DataSource = this.listReportType;
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
                SMG.DB.Helpper.ReportTypeHelper reportTypeHelper = new SMG.DB.Helpper.ReportTypeHelper();
                string query = string.Empty;
                var rs = reportTypeHelper.GetReportTypesAsync();
                if(rs != null)
                {
                    this.listReportType = rs.Result;
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
                txtCode.Text = txtName.Text = txtGroup.Text = "";
                this.actionType = GlobalVariables.ActionType.ACCTION__ADD;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewListReportType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var data = (SMG.Models.ReportType)gridViewListReportType.GetRow(e.ListSourceRowIndex);
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

        private void gridViewListReportType_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = (SMG.Models.ReportType)gridViewListReportType.GetFocusedRow();
                if(data != null)
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

        private void FillDataToEditControl(Models.ReportType data)
        {
            try
            {
                txtCode.Text = data.REPORT_TYPE_CODE;
                txtName.Text = data.REPORT_TYPE_NAME;
                txtGroup.Text = data.REPORT_TYPE_GROUP_ID.ToString();
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
                if (!ValiadtionControlRequired(txtCode)|| !ValiadtionControlRequired(txtName) || !ValiadtionControlRequired(txtGroup) || this.actionType == -1) return;
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
                SMG.Models.ReportType reportType = new SMG.Models.ReportType();
                reportType.REPORT_TYPE_CODE = txtCode.Text;
                reportType.REPORT_TYPE_NAME = txtName.Text;
                reportType.REPORT_TYPE_GROUP_ID = Convert.ToInt32(txtGroup.Text);
                if (TokenManager.TokenManager.IsLoggedIn())
                {

                    if (this.actionType == GlobalVariables.ActionType.ACCTION__EDIT)
                        reportType.MODIFIER = TokenManager.TokenManager.GetUsername();
                    else
                        reportType.CREATOR = TokenManager.TokenManager.GetUsername();
                }
                else
                {

                    if (this.actionType == GlobalVariables.ActionType.ACCTION__EDIT)
                        reportType.MODIFIER = "ADMIN";
                    else
                        reportType.CREATOR = "ADMIN";
                }
                SMG.DB.Helpper.ReportTypeHelper reportTypeHelper = new SMG.DB.Helpper.ReportTypeHelper();
                string error = string.Empty;
                var result = this.actionType == GlobalVariables.ActionType.ACCTION__ADD ? reportTypeHelper.AddReportTypeAsync(reportType) : reportTypeHelper.UpdateReportTypeAsync(reportType);
                if (result != null && result.Result.Item1)
                {

                    ToastNotifier.ShowSuccess("Xử lý thành công!");
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

        private bool ValiadtionControlRequired(TextEdit control)
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            SetDefaultControl();
        }

        private void repoDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (SMG.Models.ReportType)gridViewListReportType.GetFocusedRow();
                if (data != null)
                {
                    if (MessageBox.Show("Bạn có chắc chắn muốn xóa bản ghi này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SMG.DB.Helpper.ReportTypeHelper reportTypeHelper = new SMG.DB.Helpper.ReportTypeHelper();
                        string error = string.Empty;
                        var result = reportTypeHelper.DeleteReportTypeAsync(data.ID);
                        if (result != null && result.Result.Item1)
                        {
                            ToastNotifier.ShowSuccess("Xử lý thành công!");
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

    }
}
