using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomRepo.BRMaster.DL
{
    public class CostumersManager : EntityManager
    {
        public CostumersManager(BRMasterModel db) : base(db) {
        }

        public List<Costumer> GetAll() {
            return db.Costumers.ToList();
        }

        public Costumer Get(string number) {
            return db.Costumers.Where(e => e.Number == number).FirstOrDefault();
        }
    }
}
