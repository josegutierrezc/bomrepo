using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BomRepo.BRMaster.DL
{
    public class BRMasterModel : BRMasterContext
    {
        public BRMasterModel() : base() {

        }

        public BRMasterModel(DbContextOptions<BRMasterContext> options) : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=ITDEV\\SQLDEVELOPMENT;Database=BRMaster;User Id=brsysadmin;Password=1KillsAll");
            }
        }
    }
}
