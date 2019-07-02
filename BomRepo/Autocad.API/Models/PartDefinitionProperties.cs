using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.Autocad.API.Models
{
    public class PartDefinitionProperty
    {
        public int ProjectId { get; set; }
        public int PartDefinitionId { get; set; }
        public int PropertyId { get; set; }
    }
}
