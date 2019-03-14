using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class ProjectStatusesManager : EntityManagerBase
    {
        public ProjectStatusesManager(BRXXXXXModel db) : base(db)
        {
        }

        public override object Add(object entity)
        {
            throw new NotImplementedException();
        }

        public override object Get(int entityid)
        {
            throw new NotImplementedException();
        }

        public override object GetAll() {
            return db.ProjectStatuses.ToList();
        }

        public override bool Remove(int entityid)
        {
            throw new NotImplementedException();
        }

        public override bool Update(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
