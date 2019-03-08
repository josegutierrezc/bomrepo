using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using BomRepo.BRMaster.DL;
using BomRepo.BRMaster.DTO;

namespace REST.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET api/v1/users
        [HttpGet]
        public ActionResult<List<UserDTO>> Get()
        {
            using (var db = new BRMasterModel()) {
                UsersManager usersMan = new UsersManager(db);
                return Mapper.Map<List<User>, List<UserDTO>>(usersMan.GetAll());
            }    
        }

        // GET api/v1/users/username
        [HttpGet("{username}")]
        public ActionResult<UserDTO> Get(string username)
        {
            using (var db = new BRMasterModel()) {
                UsersManager usersMan = new UsersManager(db);
                CostumerUsersManager cosMan = new CostumerUsersManager(db);
                UserDTO dto = Mapper.Map<User, UserDTO>(usersMan.Get(username));

                if (dto == null) return NotFound();

                dto.Costumers = Mapper.Map<List<Costumer>, List<CostumerDTO>>(cosMan.GetCostumers(username));
                return dto;
            }
        }

        // POST api/v1/users
        [HttpPost]
        public void Post([FromBody] UserDTO user)
        {
            
        }

        // PUT api/v1/users/username
        [HttpPut("{username}")]
        public ActionResult Put(string username, [FromBody] UserDTO user)
        {
            User entity = Mapper.Map<UserDTO, User>(user);
            if (entity.Username != username) return BadRequest("Username does not match.");

            using (var db = new BRMasterModel()) {
                UsersManager usersMan = new UsersManager(db);
                usersMan.Update(entity);
                return Ok();
            }
        }

        // DELETE api/v1/users/username
        [HttpDelete("{username}")]
        public ActionResult Delete(string username)
        {
            using (var db = new BRMasterModel())
            {
                UsersManager usersMan = new UsersManager(db);
                usersMan.Delete(username);
                return Ok();
            }
        }
    }
}
