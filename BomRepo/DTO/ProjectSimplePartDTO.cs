﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ProjectSimplePartDTO
    {
        public int Id { get; set; }
        public int ProjectEntityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
