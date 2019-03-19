using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.BRXXXXX.DTO
{
    public class PartsContainerDTO
    {
        public int ParentPartId { get; set; }
        public string ParentPartName { get; set; }
        public List<PartPropertyDTO> ParentProperties { get; set; }
        public List<PartPlacementDTO> Placements { get; set; }
    }
}
