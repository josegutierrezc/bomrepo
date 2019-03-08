using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using BomRepo.BRXXXXX.DL;
using BomRepo.BRXXXXX.DTO;
using BomRepo.BRMaster.DL;
using BomRepo.BRMaster.DTO;
using BomRepo.ErrorsCatalog;

namespace REST.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CostumersController : ControllerBase
    {
        #region Costumers

        // GET api/v1/costumers
        [HttpGet]
        public ActionResult<List<CostumerDTO>> Get()
        {
            using (var db = new BRMasterModel())
            {
                CostumersManager cosMan = new CostumersManager(db);
                return Mapper.Map<List<Costumer>, List<CostumerDTO>>(cosMan.GetAll());
            }
        }


        // GET api/v1/costumers/{costumernumber}
        [HttpGet("{costumernumber}")]
        public ActionResult<CostumerDTO> Get(string costumernumber)
        {
            using (var db = new BRMasterModel())
            {
                Costumer costumer = GetCostumer(costumernumber);
                if (costumer == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
                return Mapper.Map<Costumer, CostumerDTO>(costumer);
            }
        }

        #endregion

        #region Projects

        // GET api/v1/costumers/{costumernumber}/projects}
        [HttpGet("{costumernumber}/projects")]
        public ActionResult<List<ProjectDTO>> GetProjects(string costumernumber)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            using (var db = new BRXXXXXModel(costumernumber))
            {
                ProjectsManager proMan = new ProjectsManager(db);
                return Mapper.Map<List<Project>, List<ProjectDTO>>(proMan.GetAll());
            }
        }


        // GET api/v1/costumers/{costumernumber}/projects
        [HttpGet("{costumernumber}/projects/{projectnumber}")]
        public ActionResult<List<ProjectDTO>> GetProjects(string costumernumber, string projectnumber)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            Project project = GetProject(costumernumber, projectnumber);
            if (project == null) return NotFound(ErrorCatalog.ProjectDoesNotExist);
            return Ok(Mapper.Map<Project, ProjectDTO>(project));
        }

        #endregion

        #region ProjectStatuses

        // GET api/v1/costumers/{costumernumber}/projectstatuses
        [HttpGet("{costumernumber}/projectstatuses")]
        public ActionResult<List<ProjectStatusDTO>> GetProjectStatuses(string costumernumber)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            using (var db = new BRXXXXXModel(costumernumber))
            {
                ProjectStatusesManager proMan = new ProjectStatusesManager(db);
                return Mapper.Map<List<ProjectStatus>, List<ProjectStatusDTO>>(proMan.GetAll());
            }
        }

        #endregion

        #region Branches

        // GET api/v1/costumers/{costumernumber}/branches
        [HttpGet("{costumernumber}/branches")]
        public ActionResult<List<UserBranchDTO>> GetBranches(string costumernumber, [FromQuery]string username, [FromQuery]string projectnumber)
        {
            username = username == null ? string.Empty : username;
            projectnumber = projectnumber == null ? string.Empty : projectnumber;

            //Verify costumer exists
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);

            //Verify User exists
            if (username != string.Empty)
                if (GetUser(username) == null) return NotFound(ErrorCatalog.UserDoesNotExist);

            using (var db = new BRXXXXXModel(costumernumber))
            {
                UserBranchesManager branchMan = new UserBranchesManager(db);
                return Mapper.Map<List<UserBranch>, List<UserBranchDTO>>(branchMan.Get(username, projectnumber));

            }
        }


        // GET api/v1/costumers/{costumernumber}/branches
        [HttpPut("{costumernumber}/branches")]
        public ActionResult<bool> PutAssembly(string costumernumber, [FromQuery]string username, [FromQuery]string projectnumber, [FromQuery]string assemblyname, [FromBody]List<UserBranchAssemblyPartDTO> partreferencesdto)
        {
            username = username == null ? string.Empty : username;
            projectnumber = projectnumber == null ? string.Empty : projectnumber;

            //Verify costumer exists
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);

            //Verify User exists
            if (username == string.Empty)
                return NotFound(ErrorCatalog.UserDoesNotExist);
            else if (GetUser(username) == null) return NotFound(ErrorCatalog.UserDoesNotExist);

            //Verify Project exists
            if (projectnumber == string.Empty) return NotFound(ErrorCatalog.ProjectDoesNotExist);
            Project project = GetProject(costumernumber, projectnumber);
            if (project == null) return NotFound(ErrorCatalog.ProjectDoesNotExist);

            using (var db = new BRXXXXXModel(costumernumber))
            {
                UserBranchesManager branchMan = new UserBranchesManager(db);

                //Get userbranch, if it does not exist then create it
                UserBranch userbranch = null;
                var userbranches = branchMan.Get(username, projectnumber);
                if (userbranches.Count() == 0)
                {
                    userbranch = branchMan.Add(new UserBranch()
                    {
                        Username = username,
                        ProjectId = project.Id,
                    });
                }
                else userbranch = userbranches[0];

                //Push stuff
                List<UserBranchAssemblyPart> partreferences = Mapper.Map<List<UserBranchAssemblyPartDTO>, List<UserBranchAssemblyPart>>(partreferencesdto);
                bool result = branchMan.Commit(username, projectnumber, assemblyname, partreferences);

                //Return
                if (result) return Created(string.Empty, null); else return BadRequest();
            }
        }


        // POST api/v1/costumers/{costumernumber}/branches
        [HttpPost("{costumernumber}/branches")]
        public ActionResult<UserBranchDTO> PostBranch(string costumernumber, [FromBody] UserBranchDTO userbranch)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            using (var db = new BRXXXXXModel(costumernumber))
            {
                if (userbranch == null) return BadRequest("UserBranch object missing in message body.");
                if (userbranch.Username == null || userbranch.Username == string.Empty) return BadRequest("Username cannot be null or empty.");

                UserBranchesManager branchMan = new UserBranchesManager(db);
                UserBranch newbranch = branchMan.Add(Mapper.Map<UserBranchDTO, UserBranch>(userbranch));
                if (branchMan.ErrorOccurred) return BadRequest(branchMan.ErrorDescription);

                return Mapper.Map<UserBranch, UserBranchDTO>(newbranch);
            }
        }

        #endregion

        #region Entities

        // GET api/v1/costumers/{costumernumber}/entities}
        [HttpGet("{costumernumber}/entities")]
        public ActionResult<List<ProjectDTO>> GetEntities(string costumernumber)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            using (var db = new BRXXXXXModel(costumernumber))
            {
                EntitiesManager entMan = new EntitiesManager(db);
                return Ok(Mapper.Map < List<Entity>, List<EntityDTO>>(entMan.GetAll()));
            }
        }

        // POST api/v1/costumers/{costumernumber}/entities}
        [HttpPost("{costumernumber}/entities")]
        public ActionResult<List<ProjectDTO>> PostEntity(string costumernumber, [FromBody]EntityDTO dto)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            using (var db = new BRXXXXXModel(costumernumber))
            {
                EntitiesManager entMan = new EntitiesManager(db);
                bool result = entMan.Add(Mapper.Map<EntityDTO, Entity>(dto));

                if (!result) return BadRequest(entMan.ErrorDescription);

                return Ok();
            }
        }

        #endregion

        #region Helpers
        public Costumer GetCostumer(string costumernumber)
        {
            using (var db = new BRMasterModel())
            {
                CostumersManager cosMan = new CostumersManager(db);
                return cosMan.Get(costumernumber);
            }
        }
        public User GetUser(string username)
        {
            using (var db = new BRMasterModel())
            {
                UsersManager userMan = new UsersManager(db);
                return userMan.Get(username);
            }
        }
        public Project GetProject(string costumernumber, string projectnumber)
        {
            using (var db = new BRXXXXXModel(costumernumber))
            {
                ProjectsManager proMan = new ProjectsManager(db);
                return proMan.Get(projectnumber);
            }
        }
        #endregion
    }
}