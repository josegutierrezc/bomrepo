using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public partial class UserBranchAssemblyPart
    {
        public int Id { get; set; }
        public int UserBranchAssemblyId { get; set; }
        public int EntityId { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
    }
}
