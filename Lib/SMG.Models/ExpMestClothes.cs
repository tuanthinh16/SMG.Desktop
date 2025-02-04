using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class ExpMestClothes : ModelBase
    {
        public string EXP_MEST_TYPE_ID { get; set; }
        public string EXP_MEST_STT_ID { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public long EXP_MEST_TIME { get; set; }
        public string EXP_MEST_STATUS { get; set; }
        public string EXP_MEST_DESCRIPTION { get; set; }
        public long CLOTHES_TYPE_ID { get; set; }
        public long CLOTHES_ID { get; set; }

        public string LOGINNAME { get; set; }
        public string EXP_TIME { get; set; }
        public string REQUEST_TIME { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
    }
}
