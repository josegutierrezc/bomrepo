using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public partial class UserBranchAssembly
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public int EntityId { get; set; }
        public int UserBranchId { get; set; }
        public string Name { get; set; }
    }
}
