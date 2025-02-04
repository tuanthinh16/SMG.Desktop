using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class ClothesType :ModelBase
    {
        public string CLOTHES_TYPE_CODE { get; set; }
        public string CLOTHES_TYPE_NAME { get; set; }
        public string PARENT_ID { get; set; }
        public string BRANCH_NAME { get; set; }
        public string COUNTRY_NAME { get; set; }
        public string CLOTHES_TYPE_DESCRIPTION { get; set; }
        public string CLOTHES_TYPE_IMAGE { get; set; }

    }
}
