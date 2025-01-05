using FlexCel.Report;
using FlexCel.XlsAdapter;
using SMG.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMN.Report
{
    public class CreateDataToExcel
    {
        public bool ExportData<T>(List<T> data, string reportFileName, string reportTypeCode, string reportCode, string reportName,string outputFile, string key)
        {
            bool result = true;
            try
            {
                string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string templatePath = Path.Combine(baseDirectory, "Report","Tmp", reportFileName);
                string outputPath = Path.Combine(baseDirectory,"Report", "Data", reportTypeCode, outputFile);
                string outputDirectory = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                FlexCelReport report = new FlexCelReport();
                XlsFile xls = new XlsFile(templatePath, true);

                // Load the configuration sheet
                int configSheetIndex = xls.GetSheetIndex("<#Config1>", false);
                if (configSheetIndex != -1)
                {
                    xls.ActiveSheet = configSheetIndex;

                    int row = 10;
                    while (!string.IsNullOrEmpty(xls.GetCellValue(row, 1).ToString())) 
                    {
                        string tableName = xls.GetCellValue(row, 1)?.ToString(); // Cell A(row) (Tên bảng)
                        string filter = xls.GetCellValue(row, 2)?.ToString(); // Cell B(row) (Filter)
                        string format = xls.GetCellValue(row, 7)?.ToString(); // Cell G(row) (Format)

                        // Reading configuration data from the sheet
                        string relationshipSource = xls.GetCellValue(row, 3)?.ToString(); // Cell C10 (Relationship Source)
                        string relatedFields = xls.GetCellValue(row, 4)?.ToString(); // Cell D10 (Related Fields)

                        // Here you could define a dynamic filter or modify your query based on this data
                        if (!string.IsNullOrEmpty(filter))
                        {
                            if (filter.StartsWith("DISTINCT"))
                            {
                                string column = filter.Replace("DISTINCT(", "").Replace(")", "").Trim(); // Lấy tên cột từ DISTINCT
                                data = ApplyDistinctFilter(data, column);  // Áp dụng DISTINCT
                            }
                            else
                            {
                                data = ApplyFilter(data, filter);  // Dùng dynamic LINQ
                            }
                        }

                        // If the report has relationships to consider, apply them
                        if (!string.IsNullOrEmpty(relationshipSource))
                        {
                            // Parse relationshipSource and relatedFields and apply the relationship in the report
                            string[] relationshipParts = relationshipSource.Split(new string[] { "->" }, StringSplitOptions.None);
                            // Sử dụng ký tự phân cách
                            string masterTable = relationshipParts[0];
                            string detailTable = relationshipParts[1];
                            
                        }

                        row++;
                    }
                }

                report.AddTable(key, data);  
                report.Run(xls);
                xls.Save(outputPath);
            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Error(ex);
            }
            return result;
        }
        private List<T> ApplyFilter<T>(List<T> data, string filter)
        {
            // Dùng dynamic LINQ để áp dụng filter
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, filter.Split('=')[0].Trim());
            var constant = Expression.Constant(Convert.ChangeType(filter.Split('=')[1].Trim(), property.Type));
            var equals = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            return data.AsQueryable().Where(lambda).ToList();
        }

        private List<T> ApplyDistinctFilter<T>(List<T> data, string column)
        {
            // Lấy các giá trị của cột cần DISTINCT
            var distinctValues = data.Select(x => x.GetType().GetProperty(column).GetValue(x, null)).Distinct().ToList();

            // Chuyển lại từ List<object> về List<T>
            var result = data.Where(x => distinctValues.Contains(x.GetType().GetProperty(column).GetValue(x, null))).ToList();

            return result;
        }

    }
}
