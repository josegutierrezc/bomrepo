using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public partial class Project
    {
        public int Id { get; set; }
        public int ProjectStatusId { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        [NotMapped]
        public string ProjectStatusName { get; set; }
    }
}
