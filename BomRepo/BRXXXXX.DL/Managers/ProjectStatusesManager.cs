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

        public List<ProjectStatus> GetAll() {
            return db.ProjectStatuses.ToList();
        }
    }
}
