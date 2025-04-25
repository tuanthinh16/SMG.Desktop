using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class ImpMest:ModelBase
    {
        public string IMP_MEST_TYPE_ID { get; set; }
        public string IMP_MEST_STT_ID { get; set; }
        public string IMP_MEST_CODE { get; set; }
        public long IMP_MEST_TIME { get; set; }
        public string IMP_MEST_STATUS { get; set; }
        public string IMP_MEST_DESCRIPTION { get; set; }
        public long PRODUCT_TYPE_ID { get; set; }
        public long PRODUCT_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal VAT { get; set; }
        public string LOGINNAME { get; set; }
        public long? IMP_TIME { get; set; }
        public string REQUEST_TIME { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string BILL_NUMBER { get; set; }
        public string  SUPPLIER { get; set; }

    }
}
