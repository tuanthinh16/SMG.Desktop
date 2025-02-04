using Oracle.ManagedDataAccess.Client;
using SMG.DB.Helper;
using SMG.Logging;
using SMG.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SMG.DB.Helpper.PluginHelper;

namespace SMG.DB.Helpper
{
    public class ReportTypeHelper
    {
        graphqlHelper client = new graphqlHelper();
        public ReportTypeHelper() : base() { }

        // Semaphore để giới hạn số lượng thread kết nối đồng thời
        private static SemaphoreSlim semaphore = new SemaphoreSlim(5); // Ví dụ: tối đa 5 luồng

        // Phương thức lấy plugin từ cơ sở dữ liệu
        

        // Thêm Reportype mới
        public async Task<(bool, string)> AddReportTypeAsync(ReportType reportType)
        {
            string error = string.Empty;
            try
            {
                string mutation = @"
                mutation CreateReportType($reportTypeCode: String, $reportTypeGroupId: Int, $reportTypeName: String) {
                    createReportType(
                        reportTypeCode: $reportTypeCode, 
                        reportTypeGroupId: $reportTypeGroupId, 
                        reportTypeName: $reportTypeName
                    ) {
                        success
                    }
                }";

                // Định nghĩa các tham số để truyền vào mutation
                var variables = new
                {
                    reportTypeCode = reportType.REPORT_TYPE_CODE,
                    reportTypeGroupId = reportType.REPORT_TYPE_GROUP_ID,
                    reportTypeName = reportType.REPORT_TYPE_NAME
                };

                // Gửi mutation và lấy kết quả
                var result = client.ExecuteQuery<CreateReportTypeResponse>(mutation, variables);

                // Kiểm tra kết quả và trả về
                if (result != null && result.CreateReportType != null && result.CreateReportType.Success)
                {
                    return (true, error);
                }
                else
                {
                    error = "Failed to create report type.";
                    return (false, error);
                }
            }
            catch (OracleException ex)
            {
                LogSystem.Error("Error adding report type: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }


        // Cập nhật Reportype
        public async Task<(bool, string)> UpdateReportTypeAsync(ReportType reportType)
        {
            string error = string.Empty;
            try
            {
                string mutation = @"
                mutation UpdateReportType($id: Int, $modifier: String, $reportTypeCode: String, $reportTypeGroupId: Int, $reportTypeName: String) {
                    updateReportType(
                        id: $id,
                        modifier: $modifier,
                        reportTypeCode: $reportTypeCode,
                        reportTypeGroupId: $reportTypeGroupId,
                        reportTypeName: $reportTypeName
                    ) {
                        success
                    }
                }";

                var variables = new
                {
                    id = reportType.ID,
                    modifier = reportType.MODIFIER,
                    reportTypeCode = reportType.REPORT_TYPE_CODE,
                    reportTypeGroupId = reportType.REPORT_TYPE_GROUP_ID,
                    reportTypeName = reportType.REPORT_TYPE_NAME
                };

                var result = client.ExecuteQuery<UpdateReportTypeResponse>(mutation, variables);

                return (result != null && result.UpdateReportType != null && result.UpdateReportType.Success, error);
            }
            catch (Exception ex)
            {
                LogSystem.Error("Error updating report type: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }
        // xoa Reportype
        public async Task<(bool, string)> DeleteReportTypeAsync(long id)
        {
            string error = string.Empty;
            try
            {
                string mutation = @"
                mutation DeleteReportType($id: Int) {
                    deleteReportType(id: $id) {
                        success
                    }
                }";

                var variables = new { id };

                var result = client.ExecuteQuery<DeleteReportTypeResponse>(mutation, variables);

                return (result != null && result.DeleteReportType != null && result.DeleteReportType.Success, error);
            }
            catch (Exception ex)
            {
                LogSystem.Error("Error deleting report type: " + ex.Message);
                error = ex.Message;
                return (false, error);
            }
        }
        //get reportType
        public async Task<List<ReportType>> GetReportTypesAsync()
        {
            try
            {
                string query = @"
                            query ReportTypes {
                                reportTypes {
                                    ID
                                    REPORT_TYPE_CODE
                                    REPORT_TYPE_NAME
                                    REPORT_TYPE_GROUP_ID
                                    CREATE_TIME
                                    CREATOR
                                    MODIFIER
                                    MODIFY_TIME
                                    IS_ACTIVE
                                }
                            }";

                var result = client.ExecuteQuery<ReportTypesResponse>(query);

                return result?.ReportTypes ?? new List<ReportType>();
            }
            catch (Exception ex)
            {
                LogSystem.Error("Error fetching report types: " + ex.Message);
                return new List<ReportType>();
            }
        }


        //response
        public class CreateReportTypeResponse
        {
            public CreateReportTypeResult CreateReportType { get; set; }
        }

        public class CreateReportTypeResult
        {
            public bool Success { get; set; }
        }

        public class UpdateReportTypeResponse
        {
            public UpdateReportTypeResult UpdateReportType { get; set; }
        }

        public class UpdateReportTypeResult
        {
            public bool Success { get; set; }
        }

        public class DeleteReportTypeResponse
        {
            public DeleteReportTypeResult DeleteReportType { get; set; }
        }

        public class DeleteReportTypeResult
        {
            public bool Success { get; set; }
        }

        public class ReportTypesResponse
        {
            public List<ReportType> ReportTypes { get; set; }
        }


    }
}
