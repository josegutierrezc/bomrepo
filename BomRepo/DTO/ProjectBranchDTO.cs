using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ProjectBranchDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedByUserId { get; set; }
        public string Name { get; set; }
    }
}
