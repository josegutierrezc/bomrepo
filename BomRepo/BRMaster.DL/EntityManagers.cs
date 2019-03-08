using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRMaster.DL
{
    public class EntityManager
    {
        protected BRMasterModel db;
        public EntityManager(BRMasterModel db) {
            this.db = db;
        }
    }
}
