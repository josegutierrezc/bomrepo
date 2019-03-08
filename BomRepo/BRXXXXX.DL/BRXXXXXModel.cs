using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BomRepo.BRXXXXX.DL
{
    public class BRXXXXXModel : BRXXXXXContext
    {
        private string costumernumber;

        public BRXXXXXModel(string CostumerNumber) : base()
        {
            costumernumber = CostumerNumber;
        }

        public BRXXXXXModel(DbContextOptions<BRXXXXXContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string sqlconnection = "Server=ITDEV\\SQLDEVELOPMENT;Database=@DBNAME;User Id=brsysadmin;Password=1KillsAll";
                sqlconnection = sqlconnection.Replace("@DBNAME", "BR" + costumernumber);
                optionsBuilder.UseSqlServer(sqlconnection);
            }
        }
    }
}
