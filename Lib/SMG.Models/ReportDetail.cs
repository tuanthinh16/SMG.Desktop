using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class ReportDetail : ModelBase
    {
        public string REPORT_TYPE_CODE { get; set; }
        public string REPORT_CODE { get; set; }
        public string REPORT_NAME { get; set; }
        public string REPORT_JSON_FILTER { get; set; }
        public string REPORT_DETAIL_CODE { get; set; }
        public string OUTPUT_FILE_NAME { get; set; }
    }
}
