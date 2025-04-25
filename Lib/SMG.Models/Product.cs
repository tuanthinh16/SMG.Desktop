using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class Product :ModelBase
    {
        public string  PRODUCT_NAME { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string PRODUCT_GROUP_ID { get; set; }
        public long PRODUCT_TYPE_ID { get; set; }
        public string PRODUCT_UNIT_ID { get; set; }
        public string PRODUCT_PRICE { get; set; }
        public decimal AMOUNT { get; set; }
        public string PRODUCT_DESCRIPTION { get; set; }
        public string PRODUCT_IMAGE { get; set; }

    }
}
