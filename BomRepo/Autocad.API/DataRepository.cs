using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomRepo.Autocad.API.Models;

namespace BomRepo.Autocad.API.Models
{
    public class DataRepository
    {
        #region Singleton
        private static DataRepository singleton;
        private bool frommemory;
        public static DataRepository Instance(bool FromMemory) {
            if (singleton == null) singleton = new DataRepository();
            singleton.frommemory = FromMemory;
            return singleton;
        }
        #endregion

        #region Public Properties
        public string ErrorDescription { get; set; }
        public bool ErrorOccurred {
            get {
                return ErrorDescription != string.Empty;
            }
        }
        #endregion

        #region Memory Repository Data Definition
        private List<Project> projects { get; set; }
        private List<Property> properties { get; set; }
        private List<PartDefinition> partdefinitions { get; set; }
        private List<PartDefinitionProperty> partdefinitionproperties { get; set; }
        #endregion

        #region Load Memory Objects
        private void InitializeMemoryRepository() {
            if (projects == null) {
                projects = new List<Project>() {
                        new Project() { Id = 1, CostumerNumber = "0000", Name = "Test project 1", Number = "0000", CreatedOn = DateTime.Now, CreatedByUsername = "devuser" },
                        new Project() { Id = 2, CostumerNumber = "0000", Name = "Test project 2", Number = "0001", CreatedOn = DateTime.Now, CreatedByUsername = "devuser" }
                };
            }
            if (properties == null) {
                properties = new List<Property>() {
                    new Property() { Id = 1, ProjectId = 1, Name = "Length", IsDouble = true },
                    new Property() { Id = 2, ProjectId = 1, Name = "Width", IsDouble = true },
                    new Property() { Id = 3, ProjectId = 1, Name = "Height", IsDouble = true },
                };
            }
            if (partdefinitions == null) {
                partdefinitions = new List<PartDefinition>() {
                    new PartDefinition() { Id = 1, CreatedOn = DateTime.Now, CreatedByUsername = "devuser", ProjectId = 1, Name = "Extrusion", Pattern = "E###@###-###", IsContainer = false },
                    new PartDefinition() { Id = 2, CreatedOn = DateTime.Now, CreatedByUsername = "devuser", ProjectId = 1, Name = "Aluminum Accessory", Pattern = "AA%%%%", IsContainer = false },
                    new PartDefinition() { Id = 3, CreatedOn = DateTime.Now, CreatedByUsername = "devuser", ProjectId = 1, Name = "Aluminum sub assembly", Pattern = "SA%%%%%%", IsContainer = true }
                };
            };
            if (partdefinitionproperties == null) {
                partdefinitionproperties = new List<PartDefinitionProperty>() {
                    new PartDefinitionProperty() { ProjectId = 1, PartDefinitionId = 1, PropertyId = 1 },
                    new PartDefinitionProperty() { ProjectId = 1, PartDefinitionId = 2, PropertyId = 1 },
                    new PartDefinitionProperty() { ProjectId = 1, PartDefinitionId = 2, PropertyId = 2 },
                    new PartDefinitionProperty() { ProjectId = 1, PartDefinitionId = 2, PropertyId = 3 }
                };
            }
        }
        #endregion

        #region Projects
        public List<Project> GetAllProjects(string CostumerNumber) {
            if (frommemory) {
                if (projects == null) InitializeMemoryRepository();
                return projects.Where(e => e.CostumerNumber == CostumerNumber).ToList();
            }
            return new List<Project>();
        }
        public Project GetProjectByNumber(string CostumerNumber, string Number) {
            if (frommemory) return GetAllProjects(CostumerNumber).Where(e => e.Number == Number).FirstOrDefault();
            return null;
        }
        #endregion

        #region Properties
        public List<Property> GetAllProperties(string CostumerNumber, string ProjectNumber) {
            if (frommemory) {
                if (properties == null) InitializeMemoryRepository();
                var query = from property in properties
                            join project in projects on property.ProjectId equals project.Id
                            where project.CostumerNumber == CostumerNumber & project.Number == ProjectNumber
                            select property;
                return query.ToList();
            }

            return new List<Property>();
        }
        public Property GetPropertyByName(string CostumerNumber, string ProjectNumber, string Name) {
            if (frommemory) {
                if (properties == null) InitializeMemoryRepository();
                var query = from property in properties
                            join project in projects on property.ProjectId equals project.Id
                            where project.CostumerNumber == CostumerNumber & project.Number == ProjectNumber & property.Name.ToLower() == Name.ToLower()
                            select property;
                return query.FirstOrDefault();
            }

            return null;
        }
        public int AddProperty(string CostumerNumber, string ProjectNumber, Property Entity) {
            if (frommemory) {
                ErrorDescription = string.Empty;
                if (properties == null) InitializeMemoryRepository();
                var search = from property in properties
                             join project in projects on property.ProjectId equals project.Id
                             where project.CostumerNumber == CostumerNumber & project.Number == ProjectNumber & property.Name.ToLower() == Entity.Name.ToLower()
                             select property;
                if (search.FirstOrDefault() != null) {
                    ErrorDescription = "A property with the same name exists already.";
                    return -1;
                }

                Entity.Id = properties.Count() + 1;
                Entity.ProjectId = projects.Where(e => e.Number == ProjectNumber).FirstOrDefault().Id;
                properties.Add(Entity);
                return Entity.Id;
            }

            return -1;
        }
        public bool RemoveProperty(int PropertyId) {
            if (frommemory) {
                ErrorDescription = string.Empty;
                if (properties == null) InitializeMemoryRepository();
                var property = properties.Where(e => e.Id == PropertyId).FirstOrDefault();
                if (property == null) {
                    ErrorDescription = "Property was not found.";
                    return false;
                };

                var query = from partdefinition in partdefinitions
                            join partdefinitionproperty in partdefinitionproperties on partdefinition.Id equals partdefinitionproperty.PartDefinitionId
                            where partdefinitionproperty.PropertyId == PropertyId
                            select partdefinition;

                if (query.Count() != 0) {
                    string usedin = string.Empty;
                    foreach (var elem in query) usedin += elem.Name + ", ";
                    usedin = usedin.Substring(0, usedin.Length - 2);

                    ErrorDescription = "It is being used in definition(s): " + usedin;
                    return false;
                }

                properties.Remove(property);
                return true;
            }

            return true;
        }
        public List<Property> GetPropertiesByPartDefinitionId(int PartDefinitionId) {
            if (frommemory) {
                if (partdefinitionproperties == null) InitializeMemoryRepository();
                var query = from partdefinitionproperty in partdefinitionproperties
                            join partdefinition in partdefinitions on partdefinitionproperty.PartDefinitionId equals partdefinition.Id
                            join property in properties on partdefinitionproperty.PropertyId equals property.Id
                            where partdefinition.Id == PartDefinitionId
                            select property;
                return query.ToList();
            }

            return new List<Property>();
        }
        #endregion

        #region PartDefinitions
        public List<PartDefinition> GetAllPartDefinitions(string CostumerNumber, string ProjectNumber) {
            if (frommemory) {
                if (partdefinitions == null) InitializeMemoryRepository();
                var query = from partdefinition in partdefinitions
                            join project in projects on partdefinition.ProjectId equals project.Id
                            where project.CostumerNumber == CostumerNumber & project.Number == ProjectNumber
                            select partdefinition;
                return query.ToList();
            }
            return new List<PartDefinition>();
        }
        public List<PartDefinition> GetAllWherePropertyDoesNotBelong(string CostumerNumber, string ProjectNumber, int PropertyId) {
            if (frommemory) {
                if (partdefinitions == null) InitializeMemoryRepository();
                var pids = (from partdefinitionproperty in partdefinitionproperties
                            join partdefinition in partdefinitions on partdefinitionproperty.PartDefinitionId equals partdefinition.Id
                            join property in properties on partdefinitionproperty.PropertyId equals property.Id
                            join project in projects on partdefinition.ProjectId equals project.Id
                            where project.CostumerNumber == CostumerNumber & project.Number == ProjectNumber & property.Id == PropertyId
                            select partdefinition.Id).ToList();
                var defs = from partdefinition in partdefinitions
                           where !pids.Contains(partdefinition.Id)
                           select partdefinition;
                return defs.ToList();
            }
            return new List<PartDefinition>();
        }
        #endregion

        #region PartDefinitionProperties
        
        #endregion
    }
}
