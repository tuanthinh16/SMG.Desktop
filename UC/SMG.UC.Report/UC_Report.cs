using DevExpress.Data.XtraReports.Wizard;
using SMG.DB.Helpper;
using SMG.Logging;
using SMG.Models;
using SMG.Module;
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
using ReportType = SMG.Models.ReportType;

namespace SMG.UC.Report
{
    public partial class UC_Report : UserControl
    {
        ModuleData moduleData = null;
        List<ReportType> reportTypes = new List<ReportType>();
        public UC_Report(ModuleData data)
        {
            this.moduleData = data;
            InitializeComponent();
        }

        private void UC_Report_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultTime();
                FillDataToControlType();
                FillDataToControlReport();

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #region load data
        private void SetDefaultTime()
        {
            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime endOfDay = DateTime.Today.AddDays(1).AddMilliseconds(-1);

                dtTimeFrom.DateTime = currentDate;
                dtTimeTo.DateTime = endOfDay;

                dtTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                dtTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;

                dtTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                dtTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }


        private void FillDataToControlReport()
        {
            try
            {
                ReportDetailHelper db = new ReportDetailHelper();
                string subQuery = string.Empty;
                UpdateFilter(ref subQuery);
                var data = db.LoadReportDetailsAsync(0, 100, subQuery);
                if (data != null)
                {
                    var dataList = data.Result;
                    gridControlListCreated.DataSource = dataList;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void UpdateFilter(ref string subQuery)
        {
            try
            {
                if (dtTimeFrom.DateTime != null && dtTimeTo.DateTime != null)
                    subQuery += string.Format("AND CREATE_TIME BETWEEN {0} AND {1}", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(dtTimeFrom.DateTime), SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(dtTimeTo.DateTime));
                subQuery += " ORDER BY CREATE_TIME DESC";
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataToControlType()
        {
            try
            {
                ReportTypeHelper db = new ReportTypeHelper();
                var data = db.LoadReportTypeFromDatabaseAsync(0, 100, null);
                if (data != null)
                {
                    var dataList = data.Result;
                    reportTypes = dataList;
                    gridControlListReport.DataSource = dataList;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion
        

        #region show pdf
        private void repoView_Click(object sender, EventArgs e)
        {
            try
            {
                var data = gridViewListCreated.GetFocusedRow();
                if (data != null)
                {
                    SMG.Models.ReportDetail report = data as SMG.Models.ReportDetail;
                    ShowResultToPdf(report);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void ShowResultToPdf(SMG.Models.ReportDetail reportData)
        {
            try
            {
                if (reportData == null)
                {
                    MessageBox.Show("Không tìm thấy báo cáo", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string outputPath = Path.Combine(baseDirectory, "Report", "Data", reportData.REPORT_TYPE_CODE, reportData.OUTPUT_FILE_NAME);
                if (File.Exists(outputPath))
                {
                    SMG.PdfViewer.frmViewer frm = new SMG.PdfViewer.frmViewer(outputPath);
                    frm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy file báo cáo", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion
        private void btnFind1_Click(object sender, EventArgs e)
        {
            FillDataToControlType();
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            FillDataToControlReport();
        }
        #region repo button click
        private void repoCopy_Click(object sender, EventArgs e)
        {

            try
            {
                var data = gridViewListCreated.GetFocusedRow();
                if (data != null)
                {
                    ReportDetail reportDetail = data as ReportDetail;
                    if (reportDetail != null)
                    {
                        var reportType = this.reportTypes.Where(s => s.REPORT_TYPE_CODE == reportDetail.REPORT_TYPE_CODE).FirstOrDefault();
                        frmCreateReport frm = new frmCreateReport(reportType);
                        frm.SetOldFilter(reportDetail.REPORT_JSON_FILTER);
                        frm.FormClosed += (s, args) =>
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(FillDataToControlReport));
                            }
                            else
                            {
                                FillDataToControlReport();
                            }
                        };

                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

        }
        private void repoCreateReport_Click(object sender, EventArgs e)
        {
            try
            {
                var data = gridViewListReport.GetFocusedRow();
                if (data != null)
                {
                    ReportType reportType = data as ReportType;
                    frmCreateReport frm = new frmCreateReport(reportType);

                    frm.FormClosed += (s, args) =>
                    {
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(FillDataToControlReport));
                        }
                        else
                        {
                            FillDataToControlReport();
                        }
                    };

                    frm.ShowDialog();
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
                var data = gridViewListCreated.GetFocusedRow();
                if (data != null)
                {
                    if (MessageBox.Show("Bạn có chắc chắn muốn xóa báo cáo này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ReportDetail reportDetail = data as ReportDetail;
                        if (reportDetail != null)
                        {
                            ReportDetailHelper reportDetailHelper = new ReportDetailHelper();
                            var rs = reportDetailHelper.DeleteReportDetailAsync(reportDetail.ID);
                            if (rs != null)
                            {
                                if (rs.Result.Item1)
                                {
                                    FillDataToControlReport();
                                    MessageBox.Show("Xử lý thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Xóa báo cáo không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
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
        #endregion
        #region grid view custom unbound column data
        private void gridViewListCreated_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var data = (SMG.Models.ReportDetail)gridViewListCreated.GetRow(e.ListSourceRowIndex);
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    if(e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = SMG.DateTimeHelpper.Convert.TimeNumberToDateTime(data.CREATE_TIME).ToString("dd/MM/yyyy HH:mm:ss");
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewListReport_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var data = (SMG.Models.ReportDetail)gridViewListCreated.GetRow(e.ListSourceRowIndex);
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
