using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRXXXXX.DTO
{
    public partial class UserBranchPartDTO
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserBranchId { get; set; }
        public int PartDefinitionId { get; set; }
        public string Name { get; set; }
    }
}
