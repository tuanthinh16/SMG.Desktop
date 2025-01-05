using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using Newtonsoft.Json;
using SMG.DB.Helpper;
using SMG.Logging;
using SMG.Models;
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

namespace SMG.UC.Report
{
    public partial class frmCreateReport : Form
    {
        ReportType currentReportType = null;
        List<SMG.Models.Report> listCurrentReport = null;
        bool isOldValue = false;
        public frmCreateReport(ReportType reportType)
        {
            this.currentReportType = reportType;
            InitializeComponent();
        }

        private void frmCreateReport_Load(object sender, EventArgs e)
        {
            try
            {
                this.txtReportType.Text = currentReportType.REPORT_TYPE_CODE +" - "+currentReportType.REPORT_TYPE_NAME;
                LoadDataToReport();
                SetDefaultTime();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        public void SetOldFilter(string jsonFilter)
        {
            try
            {

                var filter = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFilter);
                if (filter != null)
                {
                    isOldValue = true;
                    LoadDataToReport();
                    if (filter.ContainsKey("REPORT_CODE"))
                    {
                        if (listCurrentReport == null)
                        {
                            ReportHelper reportHelper = new ReportHelper();
                            string query = "SELECT * FROM SMN_REPORT WHERE REPORT_TYPE_CODE = '" + currentReportType.REPORT_TYPE_CODE + "'";
                            var result = reportHelper.FreeQueryAsync(query);
                            if (result != null)
                            {
                                listCurrentReport = result.Result;
                            }
                        }
                        var report = listCurrentReport.Where(s => s.REPORT_CODE == filter["REPORT_CODE"].ToString()).FirstOrDefault();
                        if (report != null) cboReport.EditValue = report.ID;

                    }
                    if (filter.ContainsKey("TIME_FROM"))
                    {
                        dtTimeFrom.DateTime = SMG.DateTimeHelpper.Convert.TimeNumberToDateTime((long)filter["TIME_FROM"]);

                    }
                    if (filter.ContainsKey("TIME_TO"))
                    {
                        dtTimeTo.DateTime = SMG.DateTimeHelpper.Convert.TimeNumberToDateTime((long)filter["TIME_TO"]);

                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetDefaultTime()
        {
            try
            {
                dtTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                dtTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;

                dtTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                dtTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;

                if (dtTimeFrom.DateTime != null && dtTimeTo.DateTime != null && isOldValue)
                {
                    return;
                }
                DateTime currentDate = DateTime.Today;
                DateTime endOfDay = DateTime.Today.AddDays(1).AddMilliseconds(-1);

                // Thiết lập giá trị mặc định cho dtTimeFrom và dtTimeTo
                dtTimeFrom.DateTime = currentDate;
                dtTimeTo.DateTime = endOfDay;

                // Thiết lập định dạng hiển thị theo kiểu 24 giờ
                
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDataToReport()
        {
            try
            {
                if(listCurrentReport == null)
                {
                    ReportHelper reportHelper = new ReportHelper();
                    string query = "SELECT * FROM SMN_REPORT WHERE REPORT_TYPE_CODE = '" + currentReportType.REPORT_TYPE_CODE + "'";
                    var result = reportHelper.FreeQueryAsync(query);
                    if(result != null)
                    {
                        listCurrentReport = result.Result;
                    }
                }
                
                if (listCurrentReport != null)
                {
                    cboReport.Properties.DataSource = listCurrentReport;
                    cboReport.Properties.DisplayMember = "REPORT_NAME";
                    cboReport.Properties.ValueMember = "ID";
                    cboReport.Properties.View.Columns.Clear();
                    cboReport.Properties.View.Columns.AddField("REPORT_CODE").Visible = false;
                    cboReport.Properties.View.Columns.AddField("REPORT_NAME").Visible = true; 
                    cboReport.Properties.View.OptionsView.ShowColumnHeaders = false; 
                    cboReport.Properties.View.OptionsView.ShowIndicator = false; 
                    cboReport.EditValue = listCurrentReport.FirstOrDefault().ID;
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
                if (!validationRequired(cboReport)) return;
                ProcesSave();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private bool validationRequired(Control control)
        {
            bool result = true;
            try
            {
                if (control != null)
                {
                    if (control.Text == null || string.IsNullOrEmpty(control.Text))
                    {
                        dxErrorProvider1.SetError(control, "Trường dữ liệu bắt buộc");
                        result = false;
                    }
                    else
                    {
                        dxErrorProvider1.SetError(control, "");
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

        private void ProcesSave()
        {
            try
            {
                string error = string.Empty;
                SMG.Models.Report reportCode = listCurrentReport.Where(s => s.ID == Convert.ToInt64(cboReport.EditValue)).FirstOrDefault();
                Dictionary<string, object> filter = new Dictionary<string, object>
                {
                    { "REPORT_TYPE_CODE", this.currentReportType.REPORT_TYPE_CODE },
                    { "REPORT_CODE",  reportCode.REPORT_CODE},
                    { "REPORT_NAME", reportCode.REPORT_NAME },
                    { "TIME_FROM", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(dtTimeFrom.DateTime) },
                    { "TIME_TO", SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(dtTimeTo.DateTime) }
                };
                string jsonFilter = JsonConvert.SerializeObject(filter);
                string outputFile = string.Empty;
                SMN.Report.CreateReport createReport = new SMN.Report.CreateReport(this.currentReportType.REPORT_TYPE_CODE,jsonFilter);
                outputFile = createReport.GetOutputFile();
                error = createReport.GetError();
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if(MessageBox.Show("Tạo báo cáo thành công. Bạn có muốn xem ngay ? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        ShowResultToPdf(listCurrentReport.Where(s=>s.ID == Convert.ToInt64(cboReport.EditValue)).FirstOrDefault(), outputFile);
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void ShowResultToPdf(SMG.Models.Report reportData,string outputFile)
        {
            try
            {
                if(reportData == null)
                {
                    MessageBox.Show("Không tìm thấy báo cáo", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string outputPath = Path.Combine(baseDirectory, "Report","Data", reportData.REPORT_TYPE_CODE, outputFile);
                if(File.Exists(outputPath))
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
    }
}
