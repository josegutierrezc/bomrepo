using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class PartProperty
    {
        public int PartId { get; set; }
        public int PropertyId { get; set; }
        public string Value { get; set; }
        [NotMapped]
        public string PropertyName { get; set; }
    }
}
