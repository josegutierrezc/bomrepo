using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace REST.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        // POST api/v1/values
        [HttpPost]
        public ActionResult<IEnumerable<string>> Post()
        {
            return new string[] { "value1", "value2" };
        }

    }
}