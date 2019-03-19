using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class Property
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsString { get; set; }
        public bool IsInteger { get; set; }
        public bool IsDouble { get; set; }
        public bool IsBoolean { get; set; }
        public bool IsDateTime { get; set; }
    }
}
