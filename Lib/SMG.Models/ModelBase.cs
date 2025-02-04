using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class ModelBase
    {
        [Key]
        public long ID { get; set; }
        public long CREATE_TIME { get; set; }
        public string CREATOR { get; set; }
        public string MODIFIER { get; set; }
        public long? MODIFY_TIME { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
}
