using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Collections.Generic;

namespace BomRepo.BRMaster.DL
{
    public class CostumerUsersManager : EntityManager
    {
        public CostumerUsersManager(BRMasterModel db) : base(db) {
        }

        public List<Costumer> GetCostumers(string username) {
            var costumers = from cu in db.CostumerUsers
                            join u in db.Users on cu.UserId equals u.Id
                            join c in db.Costumers on cu.CostumerId equals c.Id
                            where u.Username == username
                            select c;
            return costumers.ToList();
        }
    }
}
