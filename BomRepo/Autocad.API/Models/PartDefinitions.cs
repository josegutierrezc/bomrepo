using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.Autocad.API.Models
{
    public class PartDefinition
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedByUsername { get; set; }
        public DateTime? InactiveSince { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Pattern { get; set; }
        public bool IsContainer { get; set; }
    }
}
