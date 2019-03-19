using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRXXXXX.DTO
{
    public class EntityPropertyDTO
    {
        public int EntityId { get; set; }
        public int PropertyId { get; set; }
        public int ShowOrder { get; set; }
        public PropertyDTO Property { get; set; }
    }
}
