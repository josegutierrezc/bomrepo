using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.Autocad.API.Models
{
    public class Project
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedByUsername { get; set; }
        public DateTime? InactiveSince { get; set; }
        public string CostumerNumber { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}
