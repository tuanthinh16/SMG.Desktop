using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class Bill:ModelBase
    {
        public string BILL_CODE { get; set; }
        public long BILL_TYPE_ID { get; set; }
        public long? BILL_STATUS { get; set; }
        public long BILL_TIME { get; set; }
        public string BILL_DESCRIPTION { get; set; }
        public string LOGINNAME { get; set; }
        public long? BILL_ID
        {
            get; set;
        }
        public long? EXP_MEST_ID { get; set; }
        public long? IMP_MEST_ID { get; set; }
    }
}
