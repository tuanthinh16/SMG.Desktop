using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class Transaction : ModelBase
    {
        public string TRANSACTION_CODE { get; set; }
        public string TRANSACTION_NAME { get; set; }
        public long TRANSACTION_TYPE_ID { get; set; }
        public long TRANSACTION_TIME { get; set; }
        public string TRANSACTION_STATUS { get; set; }

        public string TRANSACTION_DESCRIPTION { get; set; }
        public string LOGINNAME { get; set; }
        public short IS_CANCEL { get; set; }
        public short IS_CONFIRMED { get; set; }
        
    }
}
