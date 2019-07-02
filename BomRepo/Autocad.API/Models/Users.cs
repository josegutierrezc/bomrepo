using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.Autocad.API.Models
{
    public class User
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? InactiveSince { get; set; }
        public string CostumerNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
    }
}
