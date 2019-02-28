using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.REST.Client
{
    public class Connector
    {
        #region Singleton
        private static Connector singleton;
        public static Connector Instance {
            get {
                if (singleton == null) singleton = new Connector();
                return singleton;
            }
        }
        #endregion
    }
}
