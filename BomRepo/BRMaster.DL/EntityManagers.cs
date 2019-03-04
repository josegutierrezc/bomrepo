using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRMaster.DL
{
    public class EntityManagers
    {
        protected BRMasterModel db;
        public EntityManagers(BRMasterModel db) {
            this.db = db;
        }
    }
}
