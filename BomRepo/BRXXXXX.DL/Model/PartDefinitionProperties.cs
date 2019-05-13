using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class PartDefinitionProperty
    {
        public int PartDefinitionId { get; set; }
        public int PropertyId { get; set; }
        public int ShowOrder { get; set; }
        [NotMapped]
        public Property Property { get; set; }
    }
}
