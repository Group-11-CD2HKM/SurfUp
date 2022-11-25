using SurfUpLibary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels
{
    public class UserAddress
    {
        public int Id { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [MaxLength(4)]
        public string ZipCode { get; set; }
        [Required]
        public string StreetName { get; set; }
        [Required]
        public string StreetNumber { get; set; }
    }
}
