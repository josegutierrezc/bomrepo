using System;
using System.Collections.Generic;

namespace BomRepo.BRMaster.DL
{
    public partial class User
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string AutocadToken { get; set; }
        public string InventorToken { get; set; }
        public string WebToken { get; set; }
    }
}
