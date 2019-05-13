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
                return Mapper.Map<List<Project>, List<ProjectDTO>>(proMan.GetAll() as List<Project>);
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
                return Mapper.Map<List<ProjectStatus>, List<ProjectStatusDTO>>(proMan.GetAll() as List<ProjectStatus>);
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
                UserBranch newbranch = branchMan.Add(Mapper.Map<UserBranchDTO, UserBranch>(userbranch)) as UserBranch;
                if (branchMan.ErrorOccurred) return BadRequest(branchMan.ErrorDefinition);

                return Mapper.Map<UserBranch, UserBranchDTO>(newbranch);
            }
        }
        #endregion

        #region UserBranchParts

        /// <summary>
        /// Get all container parts and its content for an specific user and project. If projecnumber is not assigned then
        /// method will return all container parts and its content for all projects for this user.
        /// </summary>
        /// <param name="costumernumber">Required string indicating the costumer number</param>
        /// <param name="username">Required string indicating the username</param>
        /// <param name="projectnumber">No required string indicating the project number</param>
        /// <returns></returns>
        [HttpGet("{costumernumber}/branches/{username}")]
        public ActionResult<List<PartsContainerDTO>> GetUserParts(string costumernumber, string username, [FromQuery]string projectnumber) {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            if (GetUser(username) == null) return NotFound(ErrorCatalog.UserDoesNotExist);

            projectnumber = projectnumber == null ? string.Empty : projectnumber;
            if (projectnumber != string.Empty && GetProject(costumernumber, projectnumber) == null) return NotFound(ErrorCatalog.ProjectDoesNotExist);

            using (var db = new BRXXXXXModel(costumernumber)) {
                UserBranchPartsManager partMan = new UserBranchPartsManager(db);
                UserBranchPartPropertiesManager userPropMan = new UserBranchPartPropertiesManager(db);

                //Get All container parts
                List<PartsContainerDTO> result = new List<PartsContainerDTO>();
                foreach (UserBranchPart container in partMan.GetContainers(username, projectnumber)) {
                    PartsContainerDTO newdto = new PartsContainerDTO();
                    newdto.ParentPartId = container.Id;
                    newdto.ParentPartName = container.Name;
                    newdto.ParentProperties = new List<PartPropertyDTO>();
                    newdto.Placements = new List<PartPlacementDTO>();
                    foreach (KeyValuePair<int, UserBranchPart> kvp in partMan.GetPartPlacements(container.Id)) {
                        PartPlacementDTO placement = new PartPlacementDTO();
                        placement.PartId = kvp.Value.Id;
                        placement.PartName = kvp.Value.Name;
                        placement.Qty = kvp.Key;
                        placement.PartProperties = Mapper.Map<List<UserBranchPartProperty>, List<PartPropertyDTO>>(userPropMan.GetAll(kvp.Value.Id));
                        newdto.Placements.Add(new PartPlacementDTO() { PartId = kvp.Value.Id, PartName = kvp.Value.Name, Qty = kvp.Key });
                    }
                        
                    result.Add(newdto);
                }
                
                return Ok(result);
            }
        }

        /// <summary>
        /// Add a container part and its content to a user and project branch. If container exists then it
        /// and all placements will be replaced for this new commit.
        /// </summary>
        /// <param name="costumernumber">Required string indicating the costumer number</param>
        /// <param name="username">Required string indicating the username</param>
        /// <param name="projectnumber">Required string indicating the project number</param>
        /// <param name="dto">Required object of type PartsContainerDTO</param>
        /// <returns></returns>
        [HttpPost("{costumernumber}/branches/{username}")]
        public ActionResult PostUserPart(string costumernumber, string username, [FromQuery] string projectnumber, [FromBody] PartsContainerDTO dto)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            if (GetUser(username) == null) return NotFound(ErrorCatalog.UserDoesNotExist);

            Project project = GetProject(costumernumber, projectnumber);
            if (project == null) return NotFound(ErrorCatalog.ProjectDoesNotExist);

            if (dto == null) return BadRequest(ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "PartsContainerDTO object is required"));
            
            using (var db = new BRXXXXXModel(costumernumber))
            {
                //Prepare manager
                UserBranchesManager branchMan = new UserBranchesManager(db);
                UserBranchPartsManager partMan = new UserBranchPartsManager(db);
                UserBranchPartPlacementsManager placeMan = new UserBranchPartPlacementsManager(db);

                //Get User branches
                UserBranch userbranch = null;
                List<UserBranch> userbranches = branchMan.Get(username, projectnumber) as List<UserBranch>;
                if (userbranches.Count() == 0)
                {
                    //Create one
                    userbranch = branchMan.Add(new UserBranch()
                    {
                        Username = username,
                        ProjectId = project.Id,
                    }) as UserBranch;
                }
                else userbranch = userbranches[0];
                    
                //Add parent
                UserBranchPart parent = new UserBranchPart();
                parent.PartDefinitionId = -1;
                parent.UserBranchId = userbranch.Id;
                parent.Name = dto.ParentPartName;
                parent = partMan.Add(parent) as UserBranchPart;
                if (parent == null) return BadRequest(partMan.ErrorDefinition);

                //Remove all existing parent properties
                UserBranchPartPropertiesManager propMan = new UserBranchPartPropertiesManager(db);
                propMan.RemoveAll(parent.Id);

                //Add parent properties
                foreach (PartProperty property in Mapper.Map<List<PartPropertyDTO>, List<PartProperty>>(dto.ParentProperties)) {
                    UserBranchPartProperty prop = new UserBranchPartProperty();
                    prop.PropertyId = property.PropertyId;
                    prop.PropertyName = property.PropertyName;
                    prop.UserBranchPartId = parent.Id;
                    prop.Value = property.Value;
                    propMan.Add(prop);
                }
                    

                //Verify parent is container
                if (!partMan.IsContainerPart(parent.Id)) {
                    ErrorDefinition newerror = ErrorCatalog.CreateFrom(ErrorCatalog.ContainerPartRequired, string.Empty);
                    newerror.ReplaceParameterValueInUserDescription("@1", parent.Name);
                    return BadRequest(newerror);
                }

                //Remove all existing placements
                bool removed = placeMan.RemoveAll(parent.Id);

                //Add Placements
                List<UserBranchPartPlacement> placements = new List<UserBranchPartPlacement>();
                foreach (PartPlacementDTO pp in dto.Placements)
                {
                    //Verify if part has the same name of its parent
                    if (pp.PartName.ToUpper() == parent.Name) {
                        ErrorDefinition newerror = ErrorCatalog.CreateFrom(ErrorCatalog.SelfContainedError, string.Empty);
                        newerror.ReplaceParameterValueInUserDescription("@1", parent.Name);
                        return BadRequest(newerror);
                    }

                    //Add part to db
                    UserBranchPart child = partMan.Add(new UserBranchPart()
                    {
                        UserBranchId = parent.UserBranchId,
                        PartDefinitionId = -1,
                        Name = pp.PartName,
                    }) as UserBranchPart;
                    if (child == null) return BadRequest(partMan.ErrorDefinition);

                    //Remove current part properties
                    propMan.RemoveAll(child.Id);

                    //Add part properties
                    foreach (PartProperty property in Mapper.Map<List<PartPropertyDTO>, List<PartProperty>>(pp.PartProperties)) {
                        UserBranchPartProperty prop = new UserBranchPartProperty();
                        prop.UserBranchPartId = child.Id;
                        prop.PropertyId = property.PropertyId;
                        prop.PropertyName = property.PropertyName;
                        prop.Value = property.Value;
                        propMan.Add(prop);
                    }

                    //Add placement to list
                    placements.Add(new UserBranchPartPlacement()
                    {
                        UserBranchId = parent.UserBranchId,
                        ParentUserBranchPartId = parent.Id,
                        ChildUserBranchPartId = child.Id,
                        Qty = pp.Qty
                    });
                }
                bool added = placeMan.Add(placements);

                //Return data
                return Ok();
            }
        }

        /// <summary>
        /// Push all user parts related with the specified project to the main branch. Once done, user project branch will be cleaned.
        /// </summary>
        /// <param name="costumernumber">Required string indicating the costumer number</param>
        /// <param name="username">REquired string indicating the username</param>
        /// <param name="projectnumber">Required string indicating the project number</param>
        /// <returns></returns>
        [HttpPut("{costumernumber}/branches/{username}")]
        public ActionResult PutUserParts(string costumernumber, string username, [FromQuery] string projectnumber) {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            if (GetUser(username) == null) return NotFound(ErrorCatalog.UserDoesNotExist);

            Project project = GetProject(costumernumber, projectnumber);
            if (project == null) return NotFound(ErrorCatalog.ProjectDoesNotExist);

            using (var db = new BRXXXXXModel(costumernumber)) {
                UserBranchPartsManager usersPartMan = new UserBranchPartsManager(db);
                PartsManager partsMan = new PartsManager(db);
                PartPropertiesManager propMan = new PartPropertiesManager(db);
                PartPlacementsManager placeMan = new PartPlacementsManager(db);
                UserBranchPartPropertiesManager userpropMan = new UserBranchPartPropertiesManager(db);

                //Get All containers for this user and project
                foreach (UserBranchPart container in usersPartMan.GetContainers(username, projectnumber)) {
                    //Add the part to the main branch
                    Part newcontainerpart = partsMan.Add(new Part() {
                        CreatedOn = container.CreatedOn,
                        CreatedByUsername = username,
                        ModifiedByUsername = username, //Required in case part exists already
                        ProjectId = project.Id,
                        PartDefinitionId = container.PartDefinitionId,
                        Name = container.Name,
                    }) as Part;
                    if (newcontainerpart == null) return BadRequest(partsMan.ErrorDefinition);

                    //Remove all current part properties
                    propMan.RemoveAll(newcontainerpart.Id);

                    //Add container properties
                    foreach (UserBranchPartProperty property in userpropMan.GetAll(container.Id)) {
                        PartProperty prop = new PartProperty();
                        prop.PartId = newcontainerpart.Id;
                        prop.PropertyId = property.PropertyId;
                        prop.Value = property.Value;
                        propMan.Add(prop);
                    }

                    //Remove all current part placements
                    placeMan.RemoveAll(newcontainerpart.Id);

                    //Get all placements
                    List<PartPlacement> placements = new List<PartPlacement>();
                    foreach (KeyValuePair<int, UserBranchPart> kvp in usersPartMan.GetPartPlacements(container.Id)) {
                        //Add part
                        Part part = partsMan.Add(new Part() {
                            CreatedOn = kvp.Value.CreatedOn,
                            CreatedByUsername = username,
                            ModifiedByUsername = username, //Required in case part exist already
                            ProjectId = project.Id,
                            PartDefinitionId = kvp.Value.PartDefinitionId,
                            Name = kvp.Value.Name,
                        }) as Part;
                        if (part == null) return BadRequest(partsMan.ErrorDefinition);

                        //Remove all properties
                        propMan.RemoveAll(part.Id);

                        //Add properties
                        foreach (UserBranchPartProperty property in userpropMan.GetAll(kvp.Value.Id)) {
                            PartProperty prop = new PartProperty();
                            prop.PartId = part.Id;
                            prop.PropertyId = property.PropertyId;
                            prop.Value = property.Value;
                            propMan.Add(prop);
                        }

                        //Add placement
                        PartPlacement placement = new PartPlacement() {
                            ParentPartId = newcontainerpart.Id,
                            ChildPartId = part.Id,
                            Qty = kvp.Key
                        };
                        placements.Add(placement);
                    }

                    //Add placements
                    bool added = placeMan.Add(placements);
                    if (!added) return BadRequest(placeMan.ErrorDefinition);
                }

                //If everything went well ... and at this point it make sense then remove everything
                UserBranchPartPlacementsManager userPlaceMan = new UserBranchPartPlacementsManager(db);
                foreach (UserBranchPart container in usersPartMan.GetContainers(username, projectnumber)) {
                    //Remove all part properties and itself
                    foreach (KeyValuePair<int, UserBranchPart> kvp in usersPartMan.GetPartPlacements(container.Id)) {
                        //Remove all properties
                        userpropMan.RemoveAll(kvp.Value.Id);

                        //Remove the part
                        usersPartMan.Remove(kvp.Value.Id);
                    }

                    //Remove all placements
                    userPlaceMan.RemoveAll(container.Id);

                    //Remove all container properties
                    userpropMan.RemoveAll(container.Id);

                    //Remove container
                    usersPartMan.Remove(container.Id);
                }
            }
            
            return Ok();
        }

        #endregion

        #region PartDefinitionProperties

        // GET api/v1/costumers/{costumernumber}/entityproperties?projectnumber=projectnumber&entityid=entityid}
        [HttpGet("{costumernumber}/entityproperties")]
        public ActionResult<List<PartDefinitionDTO>> GetEntityProperties(string costumernumber, [FromQuery]string projectnumber,[FromQuery]int? entityid)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            Project project = GetProject(costumernumber, projectnumber);
            if (project == null) return NotFound(ErrorCatalog.ProjectDoesNotExist);

            using (var db = new BRXXXXXModel(costumernumber))
            {
                PartDefinitionPropertiesManager propMan = new PartDefinitionPropertiesManager(db);
                List<PartDefinitionProperty> properties = propMan.Get(costumernumber, entityid);
                return Ok(Mapper.Map<List<PartDefinitionProperty>, List<PartDefinitionPropertyDTO>>(properties));
            }
        }

        #endregion

        #region Entities

        // GET api/v1/costumers/{costumernumber}/entities?projectnumber=projectnumber}
        [HttpGet("{costumernumber}/entities")]
        public ActionResult<List<PartDefinitionDTO>> GetEntities(string costumernumber, [FromQuery]string projectnumber)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            using (var db = new BRXXXXXModel(costumernumber))
            {
                PartDefinitionsManager entMan = new PartDefinitionsManager(db);
                if (projectnumber == null)
                    return Ok(Mapper.Map<List<PartDefinition>, List<PartDefinitionDTO>>(entMan.GetAll() as List<PartDefinition>));
                else
                    return Ok(Mapper.Map<List<PartDefinition>, List<PartDefinitionDTO>>(entMan.GetByProject(projectnumber)));
            }
        }

        // POST api/v1/costumers/{costumernumber}/entities
        [HttpPost("{costumernumber}/entities")]
        public ActionResult PostEntity(string costumernumber, [FromBody]PartDefinitionDTO dto)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            if (GetUser(dto.CreatedByUsername) == null) return NotFound(ErrorCatalog.UserDoesNotExist);
            using (var db = new BRXXXXXModel(costumernumber))
            {
                PartDefinitionsManager entMan = new PartDefinitionsManager(db);
                PartDefinition result = entMan.Add(Mapper.Map<PartDefinitionDTO, PartDefinition>(dto)) as PartDefinition;

                if (result == null) return BadRequest(entMan.ErrorDefinition);

                return Ok();
            }
        }

        // POST api/v1/costumers/{costumernumber}/entities/entityid
        [HttpGet("{costumernumber}/entities/{entityid:int}")]
        public ActionResult<PartDefinitionDTO> GetEntity(string costumernumber, int entityid)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            using (var db = new BRXXXXXModel(costumernumber))
            {
                PartDefinitionsManager entMan = new PartDefinitionsManager(db);
                PartDefinition entity = entMan.Get(entityid) as PartDefinition;

                if (entity == null) return NotFound(ErrorCatalog.EntityDoesNotExist);

                return Ok(Mapper.Map<PartDefinition, PartDefinitionDTO>(entity));
            }
        }

        // POST api/v1/costumers/{costumernumber}/entities/entityid
        [HttpDelete("{costumernumber}/entities/{entityid}")]
        public ActionResult DeleteEntity(string costumernumber, int entityid)
        {
            if (GetCostumer(costumernumber) == null) return NotFound(ErrorCatalog.CostumerDoesNotExist);
            using (var db = new BRXXXXXModel(costumernumber))
            {
                PartDefinitionsManager entMan = new PartDefinitionsManager(db);
                bool deleted = entMan.Remove(entityid);

                if (!deleted) return NotFound(ErrorCatalog.EntityDoesNotExist);

                return Ok();
            }
        }

        #endregion

        #region Parts

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