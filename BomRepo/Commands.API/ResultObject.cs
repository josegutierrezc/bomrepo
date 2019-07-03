using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.API
{
    public class ResultObject
    {
        public bool Success { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public string ErrorDescription { get; set; }
        public ResultObject() {
            Success = true;
            Data = new Dictionary<string, object>();
            ErrorDescription = string.Empty;
        }
    }

}
