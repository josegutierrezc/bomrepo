using System;
using System.Collections.Generic;
using System.Text;
using BomRepo.ErrorsCatalog;

namespace BomRepo.BRXXXXX.DL
{
    public abstract class EntityManagerBase
    {
        protected BRXXXXXModel db;
        protected ErrorDefinition errorDefinition;
        public bool ErrorOccurred {
            get { return errorDefinition != null; }
        }
        public ErrorDefinition ErrorDefinition {
            get { return errorDefinition; }
        }
        public EntityManagerBase(BRXXXXXModel db)
        {
            this.db = db;
            errorDefinition = null;
        }

        public abstract object GetAll();
        public abstract object Add(object entity);
        public abstract object Get(int entityid);
        public abstract bool Update(object entity);
        public abstract bool Remove(int entityid);
    }
}
