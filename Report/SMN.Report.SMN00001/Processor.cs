using FlexCel.Report;
using FlexCel.XlsAdapter;
using SMG.Logging;
using SMG.Models;
using SMG.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SMN.Report.SMN00001
{
    public class Processor : IReport
    {
        List<SMG.Models.User> listRdo = new List<SMG.Models.User>();
        public bool GetData(string jsonFilter)
        {
            bool result = true;
            try
            {
                SMG.DB.Helpper.UserHelper plugin = new SMG.DB.Helpper.UserHelper();

                // Gọi phương thức bất đồng bộ LoadPluginsFromDatabaseAsync
                string query = "SELECT * FROM SMN_USER";
                var rs = plugin.FreeQueryAsync(query);

                if (rs != null)
                {
                    listRdo.AddRange(rs.Result);
                }
                

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
            return result;
        }



        public bool ProcessData()
        {
            bool result = true;
            try
            {

            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Error(ex);
            }
            return result;
        }

        public bool ExportData(SMG.Models.Report reportData,string outputFile)
        {
            bool result = true;
            try
            {
                CreateDataToExcel exporter = new CreateDataToExcel();
                result = exporter.ExportData(
                    listRdo,
                    reportData.REPORT_FILE_NAME,
                    reportData.REPORT_TYPE_CODE,
                    reportData.REPORT_CODE,
                    reportData.REPORT_NAME,
                    outputFile,
                    "Report"
                );


            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Error(ex);
            }
            return result;
        }
        public static List<SMG.Models.User> ApplyFilter(List<SMG.Models.User> listRdo, string filter)
        {
            // Đảm bảo chuỗi lọc của bạn có định dạng đúng
            if (!string.IsNullOrEmpty(filter) && filter.Contains("="))
            {
                var parts = filter.Split('=');
                var propertyName = parts[0].Trim();
                var value = parts[1].Trim();

                // Tạo biểu thức động
                var parameter = Expression.Parameter(typeof(SMG.Models.User), "x");
                var property = Expression.Property(parameter, propertyName);
                var constant = Expression.Constant(Convert.ChangeType(value, property.Type));

                var equals = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<SMG.Models.User, bool>>(equals, parameter);

                // Áp dụng điều kiện lọc vào danh sách
                return listRdo.AsQueryable().Where(lambda).ToList();
            }

            return listRdo;
        }

    }
}
