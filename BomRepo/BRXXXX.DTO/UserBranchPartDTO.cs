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
        public int EntityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
