using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomRepo.ErrorsCatalog;
using BomRepo.EntityNamePattern;
using FluentValidation;
using FluentValidation.Results;

namespace BomRepo.BRXXXXX.DL
{
    public class PartsManager : EntityManagerBase
    {
        public PartsManager(BRXXXXXModel db) : base(db) { }

        public override object Add(object entity)
        {
            //Validate
            Part part = entity as Part;
            if (part == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Part cannot be null or empty");
                return null;
            }
            PartValidator validator = new PartValidator();
            ValidationResult result = validator.Validate(part);
            if (!result.IsValid) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, result.Errors.ToString());
                return null;
            }

            //Get Entity
            Entity ent = db.Entities.Where(e => e.Id == part.EntityId).FirstOrDefault();
            if (ent == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity referenced in part object was not found");
                return null;
            }

            //Check part name match with pattern
            if (!EntityNamePattern.EntityNamePattern.MatchPattern(ent.NamePattern, part.Name))
            {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Part name does not match with entity name pattern");
                return null;
            }

            //Check if Project exist
            Project p = db.Projects.Where(e => e.Id == part.ProjectId).FirstOrDefault();
            if (p == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Project referenced in part object was not found");
                return null;
            }

            //Check if it exists already
            Part newpart = db.Parts.Where(e => e.ProjectId == part.ProjectId & e.Name.ToUpper() == part.Name.ToUpper()).FirstOrDefault();
            if (newpart == null)
            {
                //Create it
                newpart = new Part()
                {
                    CreatedOn = DateTime.UtcNow,
                    CreatedByUsername = part.CreatedByUsername,
                    ProjectId = part.ProjectId,
                    EntityId = ent.Id,
                    Name = part.Name.ToUpper(),
                    Description = part.Description
                };
                db.Parts.Add(newpart);
                db.SaveChanges();
            }
            else {
                //Change modification date
                if (part.ModifiedByUsername == null || part.ModifiedByUsername == string.Empty) {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "ModifiedByUsername in part object cannot be null or empty");
                    return null;
                }
                newpart.ModifiedOn = DateTime.UtcNow;
                newpart.ModifiedByUsername = part.ModifiedByUsername;
                newpart.EntityId = part.EntityId;
                newpart.Name = part.Name.ToUpper();
                newpart.Description = part.Description;
                db.SaveChanges();
            }

            return newpart;
        }

        public override object Get(int entityid)
        {
            throw new NotImplementedException();
        }

        public override object GetAll()
        {
            throw new NotImplementedException();
        }

        public override bool Remove(int entityid)
        {
            throw new NotImplementedException();
        }

        public override bool Update(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
