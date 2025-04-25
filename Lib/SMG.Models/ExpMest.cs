using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class ExpMest : ModelBase
    {
        public string EXP_MEST_TYPE_ID { get; set; }
        public string EXP_MEST_STT_ID { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public long EXP_MEST_TIME { get; set; }
        public string EXP_MEST_STATUS { get; set; }
        public string EXP_MEST_DESCRIPTION { get; set; }
        public long PRODUCT_TYPE_ID { get; set; }
        public long PRODUCT_ID { get; set; }
        public long AMOUNT { get; set; }
        public decimal VAT { get; set; }
        public string LOGINNAME { get; set; }
        public string EXP_TIME { get; set; }
        public string REQUEST_TIME { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
    }
}
