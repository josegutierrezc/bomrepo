using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomRepo.EntityNamePattern;
using FluentValidation;
using FluentValidation.Results;

namespace BomRepo.BRXXXXX.DL
{
    public class EntitiesManager : EntityManagerBase
    {
        public EntitiesManager(BRXXXXXModel db) : base(db) {
        }
        public bool Add(Entity entity) {
            //Validate Entity
            EntityValidator validator = new EntityValidator();
            ValidationResult result = validator.Validate(entity);
            if (!result.IsValid) {
                errorDescription = result.Errors.ToString();
                return false;
            }

            //Verify that an entity with the same name or pattern does not exist
            var samenameentity = db.Entities.Where(e => e.Name.ToLower() == entity.Name.ToLower() | e.NamePattern.ToLower() == entity.NamePattern.ToLower()).FirstOrDefault();
            if (samenameentity != null) return true;

            //Create it
            Entity newentity = new Entity()
            {
                CreatedOn = DateTime.UtcNow,
                CreatedByUsername = entity.CreatedByUsername,
                Name = entity.Name,
                Description = entity.Description,
                NamePattern = entity.NamePattern
            };

            //Add and save it
            db.Entities.Add(newentity);
            db.SaveChanges();

            return true;
        }
        public List<Entity> GetAll() {
            return db.Entities.ToList();
        }
        public List<Entity> GetByProject(string projectnumber) {
            var entities = from pe in db.ProjectEntities
                           join e in db.Entities on pe.EntityId equals e.Id
                           join p in db.Projects on pe.ProjectId equals p.Id
                           where p.Number == projectnumber
                           select e;
            return entities.ToList();
        }
        public Entity GetByProject(string projectnumber, int entityid) {
            var entities = from pe in db.ProjectEntities
                           join e in db.Entities on pe.EntityId equals e.Id
                           join p in db.Projects on pe.ProjectId equals p.Id
                           where p.Number == projectnumber & e.Id == entityid
                           select e;
            return entities.FirstOrDefault();
        }
        public bool LinkToProject(string projectnumber, int entityid) {
            //Verify project does exists
            var project = db.Projects.Where(e => e.Number == projectnumber).FirstOrDefault();
            if (project == null) {
                errorDescription = "Project does not exists.";
                return false;
            }

            //Verify entity does exists
            var entity = db.Entities.Where(e => e.Id == entityid).FirstOrDefault();
            if (entity == null) {
                errorDescription = "Entity does not exists.";
                return false;
            }

            //Verify entity is not assigned to project already. If is assigned then we do not need to do anything else
            var assigment = db.ProjectEntities.Where(e => e.ProjectId == project.Id & e.EntityId == entity.Id).FirstOrDefault();
            if (assigment != null) return true;

            //Created it
            ProjectEntity pe = new ProjectEntity()
            {
                EntityId = entity.Id,
                ProjectId = project.Id
            };

            //Add and save it
            db.ProjectEntities.Add(pe);
            db.SaveChanges();

            return true;
        }
        public bool UnlinkFromProject(string projectnumber, int entityid) {
            var entities = from pe in db.ProjectEntities
                         join p in db.Projects on pe.ProjectId equals p.Id
                         where p.Number == projectnumber & pe.EntityId == entityid
                         select pe;

            if (entities.Count() == 0) return true;

            db.ProjectEntities.RemoveRange(entities);
            db.SaveChanges();
            return true;
        }
        public bool Remove(int entityid) {
            //Verify that entity is not linked to any project
            var projectlinked = db.ProjectEntities.Where(e => e.EntityId == entityid);
            if (projectlinked.Count() != 0) {
                errorDescription = "Entity is linked with one or many projects.";
                return false;
            }

            //Verify that entity is not linked with any assembly
            var userassemblylinked = db.UserBranchAssemblies.Where(e => e.EntityId == entityid);
            var assemblylinked = db.Assemblies.Where(e => e.EntityId == entityid);
            if (userassemblylinked.Count() != 0)
            {
                errorDescription = "Entity is linked with one or many user assemblies.";
                return false;
            }
            if (assemblylinked.Count() != 0)
            {
                errorDescription = "Entity is linked with one or many general assemblies.";
                return false;
            }

            //Verify that entity is not linked with any user or general part
            var userpartlinked = db.UserBranchAssemblyParts.Where(e => e.EntityId == entityid);
            var partlinked = db.Parts.Where(e => e.EntityId == entityid);
            if (userpartlinked.Count() != 0)
            {
                errorDescription = "Entity is linked with one or many user parts.";
                return false;
            }
            if (partlinked.Count() != 0)
            {
                errorDescription = "Entity is linked with one or many general parts.";
                return false;
            }

            //Get entity
            var entity = db.Entities.Where(e => e.Id == entityid).FirstOrDefault();
            if (entity == null) {
                errorDescription = "Entity does not exists.";
                return false;
            }

            //Remove it
            db.Entities.Remove(entity);
            db.SaveChanges();

            return true;
        }
    }
}
