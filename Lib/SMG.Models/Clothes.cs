using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class Clothes :ModelBase
    {
        public string CLOTHES_CODE { get; set; }
        public string CLOTHES_NAME { get; set; }
        public long CLOTHES_TYPE_ID { get; set; }
        public string PARENT_ID { get; set; }
        public string BRANCH_NAME { get; set; }
        public string COUNTRY_NAME { get; set; }
        public string CLOTHES_SIZE { get; set; }
        public string CLOTHES_COLOR { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal PRICE { get; set; }
        public decimal VAT { get; set; }
        public string CLOTHES_DESCRIPTION { get; set; }
        public string CLOTHES_IMAGE { get; set; }
        public string CLOTHES_STATUS { get; set; }
        public string IMP_TIME { get; set; }
        public string USERNAME { get; set; }
        public string REQUEST_TIME { get; set; }
        public string REQUEST_STATUS { get; set; }
        public string REQUEST_DESCRIPTION { get; set; }


    }
}
