using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class Report :ModelBase
    {
        public string REPORT_CODE { get; set; }
        public string REPORT_NAME { get; set; }
        public string REPORT_GROUP_ID { get; set; }
        public string REPORT_FILE_NAME { get; set; }
        public string REPORT_TYPE_CODE { get; set; }
    }
}
