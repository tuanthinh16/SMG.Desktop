using SMG.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMN.Report
{
    public class CreateReport
    {
        string reportCode;
        string jsonFilter;
        string outputFile;
        string error;
        public CreateReport(string reportCode, string jsonFilter)
        {
            this.reportCode = reportCode;
            this.jsonFilter = jsonFilter;
            LoadReportDll.LoadData(reportCode,jsonFilter,ref error,ref outputFile);
        }
        public string GetOutputFile()
        {
            return outputFile;
        }
        public string GetError()
        {
            return error;
        }

    }
}
