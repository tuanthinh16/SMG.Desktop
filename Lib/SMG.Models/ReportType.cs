using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public  class ReportType :ModelBase
    {
        public string REPORT_TYPE_CODE { get; set; }
        public string REPORT_TYPE_NAME { get; set; }
        public long? REPORT_TYPE_GROUP_ID { get; set; }
    }
}
