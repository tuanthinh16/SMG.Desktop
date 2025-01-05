using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Models
{
    public class User : ModelBase
    {
        [Required]
        [MaxLength(255)]
        public string USERNAME { get; set; }

        [Required]
        [MaxLength(255)]
        public string LOGINNAME { get; set; }

        [MaxLength(255)]
        public string FULLNAME { get; set; }

        [MaxLength(500)]
        public string ADDRESS { get; set; }

        [MaxLength(20)]
        public string PHONE { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string EMAIL { get; set; }

        [MaxLength(255)]
        public string ROLE { get; set; }
    }
}
