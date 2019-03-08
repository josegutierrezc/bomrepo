using System;
using System.Collections.Generic;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class EntityManagerBase
    {
        protected BRXXXXXModel db;
        protected string errorDescription;
        public bool ErrorOccurred {
            get { return errorDescription != string.Empty; }
        }
        public string ErrorDescription {
            get { return errorDescription; }
        }
        public EntityManagerBase(BRXXXXXModel db)
        {
            this.db = db;
            errorDescription = string.Empty;
        }
    }
}
