using FlexCel.Report;
using FlexCel.XlsAdapter;
using Newtonsoft.Json;
using SMG.DB.Helper;
using SMG.DB.Helpper;
using SMG.Logging;
using SMG.Plugins;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMN.Report
{
    internal class LoadReportDll
    {
        public LoadReportDll()
        {
        }
        public static bool LoadData(string reportTypeCode, string jsonFilter, ref string error, ref string outputFileName)
        {
            bool isSuccessful = true;
            try
            {
                string pluginLink = "SMN.Report." + reportTypeCode;
                if (!string.IsNullOrEmpty(reportTypeCode) && reportTypeCode.StartsWith("TKB"))
                {
                    var rs = ProcessTKB(reportTypeCode, jsonFilter);
                    if (rs != null)
                    {
                        isSuccessful = rs.Result.Item1;
                        error = rs.Result.Item2;
                    }
                }
                else
                {
                    AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
                    // Get the directory where the executable is running
                    string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    // Construct the full path to the plugin
                    string pluginPath = Path.Combine(baseDirectory, "Report", pluginLink + ".dll");

                    // Check if the file exists
                    // Kiểm tra plugin có tồn tại không
                    if (File.Exists(pluginPath))
                    {
                        // Load plugin assembly (DLL)
                        Assembly pluginAssembly = Assembly.LoadFrom(pluginPath);

                        // Tìm các lớp triển khai IPlugin trong assembly
                        var pluginTypes = pluginAssembly.GetTypes()
                            .Where(t => typeof(IReport).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                        var filter = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFilter);
                        SMG.Models.Report reportDataCreate = new SMG.Models.Report();
                        if (filter != null)
                        {
                            if (filter.ContainsKey("REPORT_TYPE_CODE"))
                            {
                                reportDataCreate.REPORT_TYPE_CODE = filter["REPORT_TYPE_CODE"].ToString();

                            }
                            if (filter.ContainsKey("REPORT_CODE"))
                            {
                                reportDataCreate.REPORT_CODE = filter["REPORT_CODE"].ToString();

                            }
                            if (filter.ContainsKey("REPORT_NAME"))
                            {
                                reportDataCreate.REPORT_NAME = filter["REPORT_NAME"].ToString();

                            }
                        }

                        SMG.Models.Report reportData = LoadReport(reportTypeCode, reportDataCreate.REPORT_CODE);
                        string generateCode = Guid.NewGuid().ToString().Substring(0, 10);
                        foreach (var pluginType in pluginTypes)
                        {
                            // Tạo instance của lớp plugin
                            var pluginInstance = Activator.CreateInstance(pluginType);

                            // Gọi phương thức Execute của plugin
                            if (pluginInstance is IReport report)
                            {
                                outputFileName = reportData.REPORT_CODE + "_" + reportData.REPORT_NAME + "_" + generateCode + ".xlsx";
                                bool isGetData = report.GetData(jsonFilter);
                                bool isProcessData = report.ProcessData();
                                bool isExportData = report.ExportData(reportData, outputFileName);
                                isSuccessful = isSuccessful && isExportData && isGetData && isProcessData;

                            }
                        }

                        if (!SaveReportData(reportData, jsonFilter, generateCode))
                        {
                            error = "Có lỗi xảy ra trong quá trình lưu báo cáo";
                            isSuccessful = false;
                        }

                    }
                    else
                    {
                        error = $"Plugin không tồn tại tại: {pluginPath}";

                        isSuccessful = false;
                    }
                    if (!isSuccessful)
                    {
                        error = "Có lỗi xảy ra trong quá trình xử lý báo cáo";
                    }
                }


            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                isSuccessful = false;
            }
            return isSuccessful;
        }

        private static async Task<(bool,string)> ProcessTKB(string reportTypeCode, string jsonFilter)
        {
            string error = string.Empty;
            try
            {
                var filter = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFilter);
                SMG.Models.Report reportDataCreate = new SMG.Models.Report();
                if (filter != null)
                {
                    if (filter.ContainsKey("REPORT_TYPE_CODE"))
                    {
                        reportDataCreate.REPORT_TYPE_CODE = filter["REPORT_TYPE_CODE"].ToString();

                    }
                    if (filter.ContainsKey("REPORT_CODE"))
                    {
                        reportDataCreate.REPORT_CODE = filter["REPORT_CODE"].ToString();

                    }
                    if (filter.ContainsKey("REPORT_NAME"))
                    {
                        reportDataCreate.REPORT_NAME = filter["REPORT_NAME"].ToString();

                    }
                }

                var report = LoadReport(reportTypeCode, reportDataCreate.REPORT_CODE);
                string generateCode = Guid.NewGuid().ToString().Substring(0, 10);
                if (report != null)
                {
                    string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string templatePath = Path.Combine(baseDirectory, "Report", "Tmp", reportDataCreate.REPORT_FILE_NAME);
                    string outputPath = Path.Combine(baseDirectory, "Report", "Data", reportTypeCode, report.REPORT_CODE + "_" + report.REPORT_NAME + "_" + generateCode + ".xlsx");
                    string outputDirectory = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    FlexCelReport rp = new FlexCelReport();
                    XlsFile xls = new XlsFile(templatePath, true);

                    xls.ActiveSheet = 1;


                    int colCount = xls.ColCount;
                    DBHelper dbHelper = new DBHelper(); 

                    for (int col = 1; col <= colCount; col++) 
                    {
                        string sqlQuery = xls.GetCellValue(1, col)?.ToString();
                        if (!string.IsNullOrWhiteSpace(sqlQuery))
                        {
                            var data = await dbHelper.ExecuteQueryAsync(sqlQuery, reader =>
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                                return row;
                            });
                            DataTable dataTable = ConvertToDataTable(data);
                            rp.AddTable($"Report{col}", dataTable);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                error = ex.Message;
                
                LogSystem.Error(ex);
                return (false, error);
            }
            return (true, error);
        }

        private static DataTable ConvertToDataTable(List<Dictionary<string, object>> data)
        {
            DataTable table = new DataTable();

            if (data.Count > 0)
            {
                // Thêm cột vào DataTable
                foreach (var column in data[0].Keys)
                {
                    table.Columns.Add(column);
                }

                // Thêm dữ liệu vào DataTable
                foreach (var row in data)
                {
                    var newRow = table.NewRow();
                    foreach (var column in row.Keys)
                    {
                        newRow[column] = row[column] ?? DBNull.Value;
                    }
                    table.Rows.Add(newRow);
                }
            }
            return table;
        }

        private static bool SaveReportData(SMG.Models.Report reportData, string Filter, string generatecode)
        {
            bool result = true;
            try
            {
                ReportDetailHelper reportTypeHelper = new ReportDetailHelper();
                SMG.Models.ReportDetail report = new SMG.Models.ReportDetail();
                report.REPORT_CODE = reportData.REPORT_CODE;
                report.REPORT_TYPE_CODE = reportData.REPORT_TYPE_CODE;
                report.REPORT_NAME = reportData.REPORT_NAME;
                report.REPORT_JSON_FILTER = Filter;
                report.REPORT_DETAIL_CODE = generatecode;
                report.OUTPUT_FILE_NAME = reportData.REPORT_CODE + "_" + reportData.REPORT_NAME + "_" + report.REPORT_DETAIL_CODE + ".xlsx";
                report.IS_ACTIVE = (short)1;
                var rs = reportTypeHelper.AddReportDetailAsync(report);
                if (rs != null)
                {
                    result = rs.Result.Item1;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
            return result;
        }

        private static SMG.Models.Report LoadReport(string reportTypeCode, string reportcode)
        {
            SMG.Models.Report result = new SMG.Models.Report();
            try
            {
                ReportHelper reportTypeHelper = new ReportHelper();
                var rs = reportTypeHelper.FreeQueryAsync("SELECT * FROM SMN_REPORT WHERE REPORT_TYPE_CODE = '" + reportTypeCode + "'");
                if (rs != null)
                {
                    var data = rs.Result;
                    if (data != null && data.Count > 0)
                    {

                        result = data.FirstOrDefault(s => s.REPORT_CODE == reportcode);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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
