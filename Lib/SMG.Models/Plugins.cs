using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SMG.Models
{
    public class Plugins :ModelBase
    {
        public string PLUGIN_NAME { get; set; }
        public string PLUGIN_LINK { get; set; }

        public long? PLUGIN_GROUP_ID { get; set; }
        public long? PLUGIN_TYPE_ID { get; set; }
        public string ICON { get; set; }
    }
}
