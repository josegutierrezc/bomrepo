using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRXXXXX.DTO
{
    public partial class UserBranchDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
