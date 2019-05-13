using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomRepo.EntityNamePattern;
using BomRepo.ErrorsCatalog;
using FluentValidation;
using FluentValidation.Results;

namespace BomRepo.BRXXXXX.DL
{
    public class PartDefinitionsManager : EntityManagerBase
    {
        public PartDefinitionsManager(BRXXXXXModel db) : base(db) {
        }
        public override object Add(object entity) {
            //Typecast
            PartDefinition myentity = entity as PartDefinition;
            if (myentity == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity cannot be null");
                return null;
            }

            //Validate Entity
            EntityValidator validator = new EntityValidator();
            ValidationResult result = validator.Validate(myentity);
            if (!result.IsValid) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, result.Errors.ToString());
                return null;
            }
            if (myentity.CreatedByUsername == null || myentity.CreatedByUsername == string.Empty) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "CreatedByUsername cannot be null or empty");
                return null;
            }

            //Verify that an entity with the same name or pattern does not exist
            var samenameentity = db.PartDefinitions.Where(e => e.Name.ToLower() == myentity.Name.ToLower() | e.NamePattern.ToLower() == myentity.NamePattern.ToLower()).FirstOrDefault();
            if (samenameentity != null) return samenameentity;

            //Create it
            PartDefinition newentity = new PartDefinition()
            {
                CreatedOn = DateTime.UtcNow,
                CreatedByUsername = myentity.CreatedByUsername,
                Name = myentity.Name,
                Description = myentity.Description,
                NamePattern = myentity.NamePattern
            };

            //Add and save it
            db.PartDefinitions.Add(newentity);
            db.SaveChanges();

            return true;
        }
        public override object GetAll() {
            return db.PartDefinitions.ToList();
        }
        public override object Get(int entityid) {
            return db.PartDefinitions.Where(e => e.Id == entityid).FirstOrDefault();
        }
        public List<PartDefinition> GetByProject(string projectnumber) {
            var entities = from pe in db.ProjectPartDefinitions
                           join e in db.PartDefinitions on pe.PartDefinitionId equals e.Id
                           join p in db.Projects on pe.ProjectId equals p.Id
                           where p.Number == projectnumber
                           select e;
            return entities.ToList();
        }
        public PartDefinition GetByProject(string projectnumber, int entityid) {
            var entities = from pe in db.ProjectPartDefinitions
                           join e in db.PartDefinitions on pe.PartDefinitionId equals e.Id
                           join p in db.Projects on pe.ProjectId equals p.Id
                           where p.Number == projectnumber & e.Id == entityid
                           select e;
            return entities.FirstOrDefault();
        }
        public bool LinkToProject(string projectnumber, int entityid) {
            //Verify project does exists
            var project = db.Projects.Where(e => e.Number == projectnumber).FirstOrDefault();
            if (project == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Project does not exists");
                return false;
            }

            //Verify entity does exists
            var entity = db.PartDefinitions.Where(e => e.Id == entityid).FirstOrDefault();
            if (entity == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity does not exists");
                return false;
            }

            //Verify entity is not assigned to project already. If is assigned then we do not need to do anything else
            var assigment = db.ProjectPartDefinitions.Where(e => e.ProjectId == project.Id & e.PartDefinitionId == entity.Id).FirstOrDefault();
            if (assigment != null) return true;

            //Created it
            ProjectPartDefinition pe = new ProjectPartDefinition()
            {
                PartDefinitionId = entity.Id,
                ProjectId = project.Id
            };

            //Add and save it
            db.ProjectPartDefinitions.Add(pe);
            db.SaveChanges();

            return true;
        }
        public bool UnlinkFromProject(string projectnumber, int entityid) {
            var entities = from pe in db.ProjectPartDefinitions
                         join p in db.Projects on pe.ProjectId equals p.Id
                         where p.Number == projectnumber & pe.PartDefinitionId == entityid
                         select pe;

            if (entities.Count() == 0) return true;

            db.ProjectPartDefinitions.RemoveRange(entities);
            db.SaveChanges();
            return true;
        }
        public override bool Remove(int entityid) {
            //Verify that entity is not linked to any project
            var projectlinked = db.ProjectPartDefinitions.Where(e => e.PartDefinitionId == entityid);
            if (projectlinked.Count() != 0) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity is linked with one or many projects");
                return false;
            }

            //Verify that entity is not linked with any general or user part
            var userlinked = db.UserBranchParts.Where(e => e.PartDefinitionId == entityid);
            var generallinked = db.Parts.Where(e => e.PartDefinitionId == entityid);
            if (userlinked.Count() != 0)
            {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity is linked with one or many user parts");
                return false;
            }
            if (generallinked.Count() != 0)
            {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity is linked with one or many general parts");
                return false;
            }
            
            //Get entity
            var entity = db.PartDefinitions.Where(e => e.Id == entityid).FirstOrDefault();
            if (entity == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity does not exists");
                return false;
            }

            //Remove it
            db.PartDefinitions.Remove(entity);
            db.SaveChanges();

            return true;
        }
        public override bool Update(object entity)
        {
            //Typecast
            PartDefinition myentity = entity as PartDefinition;
            if (myentity == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity cannot be null");
                return false;
            }

            //Validate
            EntityValidator validator = new EntityValidator();
            ValidationResult result = validator.Validate(myentity);
            if (!result.IsValid) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, result.Errors.ToString());
                return false;
            }
            if (myentity.ModifiedByUsername == null || myentity.ModifiedByUsername == string.Empty) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "ModifiedByUsername cannot be null or empty");
                return false;
            }

            //Get current entity
            PartDefinition current = db.PartDefinitions.Where(e => e.Id == myentity.Id).FirstOrDefault();
            if (current == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity was not found");
                return false;
            }

            //If name is changed then verify no other entity exist with this new name
            if (current.Name.ToLower() != myentity.Name.ToLower()) {
                var search = db.PartDefinitions.Where(e => e.Name.ToLower() == myentity.Name.ToLower()).FirstOrDefault();
                if (search != null) {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity name cannot be changed because one with the same name exist already");
                    return false;
                }
            }

            //If name pattern was changed then verify no other entity exist with the new pattern
            if (current.NamePattern.ToLower() != myentity.NamePattern.ToLower()) {
                var search = db.PartDefinitions.Where(e => e.NamePattern.ToLower() == myentity.NamePattern.ToLower()).FirstOrDefault();
                if (search != null)
                {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity pattern cannot be changed because one with the same pattern already exist");
                    return false;
                }
            }

            //Update it
            current.ModifiedByUsername = myentity.ModifiedByUsername;
            current.ModifiedOn = DateTime.UtcNow;
            current.Name = myentity.Name;
            current.Description = myentity.Description;
            current.NamePattern = myentity.NamePattern;
            db.SaveChanges();

            return true;
        }
    }
}
