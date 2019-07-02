using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class EntityPropertyDTO
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public bool IsString { get; set; }
        public bool IsInteger { get; set; }
        public bool IsDouble { get; set; }
        public bool IsBoolean { get; set; }
        public string Name { get; set; }
    }
}
