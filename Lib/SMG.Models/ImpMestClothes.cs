using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class ImpMestClothes:ModelBase
    {
        public string IMP_MEST_TYPE_ID { get; set; }
        public string IMP_MEST_STT_ID { get; set; }
        public string IMP_MEST_CODE { get; set; }
        public long IMP_MEST_TIME { get; set; }
        public string IMP_MEST_STATUS { get; set; }
        public string IMP_MEST_DESCRIPTION { get; set; }
        public long CLOTHES_TYPE_ID { get; set; }
        public long CLOTHES_ID { get; set; }

        public string LOGINNAME { get; set; }
        public string IMP_TIME { get; set; }
        public string REQUEST_TIME { get; set; }
        public string REQUEST_LOGINNAME { get; set; }

    }
}
