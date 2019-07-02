using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.Autocad.API.Models
{
    public class Costumer
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? InactiveSince { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
    }
}
