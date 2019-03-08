using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class Assembly
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedByUsername { get; set; }
        public int EntityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
