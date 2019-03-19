using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class EntityProperty
    {
        public int EntityId { get; set; }
        public int PropertyId { get; set; }
        public int ShowOrder { get; set; }
        [NotMapped]
        public Property Property { get; set; }
    }
}
