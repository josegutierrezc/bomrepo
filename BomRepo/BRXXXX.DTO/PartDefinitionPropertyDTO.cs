using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRXXXXX.DTO
{
    public class PartDefinitionPropertyDTO
    {
        public int PartDefinitionId { get; set; }
        public int PropertyId { get; set; }
        public int ShowOrder { get; set; }
        public PropertyDTO Property { get; set; }
    }
}
