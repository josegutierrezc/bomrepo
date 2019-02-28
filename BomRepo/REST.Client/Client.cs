using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.REST.Client
{
    public class Client
    {
        #region Singleton
        private static Client singleton;
        public static Client Instance {
            get {
                if (singleton == null) singleton = new Client();
                return singleton;
            }
        }
        #endregion
    }
}
