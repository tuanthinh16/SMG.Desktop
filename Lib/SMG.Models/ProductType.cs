using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class ProductType :ModelBase
    {
        public string PRODUCT_TYPE_NAME { get; set; }
        public string PRODUCT_TYPE_CODE { get; set; }
        public string PRODUCT_TYPE_GROUP_ID { get; set; }
        public string PRODUCT_TYPE_DESCRIPTION { get; set; }
    }
}
