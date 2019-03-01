using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.BRMaster.DTO
{
    public class UserAccessTokensDTO
    {
        public int UserId { get; set; }
        public string App { get; set; }
        public string AccessToken { get; set; }
    }
}
