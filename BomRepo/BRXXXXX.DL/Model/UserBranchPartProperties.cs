using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class UserBranchPartProperty
    {
        public int UserBranchPartId { get; set; }
        public int PropertyId { get; set; }
        public string Value { get; set; }
        [NotMapped]
        public string PropertyName { get; set; }
    }
}
