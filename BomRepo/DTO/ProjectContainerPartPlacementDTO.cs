using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ProjectContainerPartPlacementDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ProjectContainerPartParentId { get; set; }
        public int? ProjectContainerPartChildId { get; set; }
        public int? ProjectSimplePartChildId { get; set; }
        public int Quantity { get; set; }
    }
}
