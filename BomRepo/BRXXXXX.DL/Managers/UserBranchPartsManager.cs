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
    public class UserBranchPartsManager : EntityManagerBase
    {
        public UserBranchPartsManager(BRXXXXXModel db) : base(db) { }

        public override object Add(object entity)
        {
            //Typecast
            UserBranchPart userpart = entity as UserBranchPart;
            if (userpart == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "UserBrachPart object required");
                return null;
            }

            //Validate
            UserBranchPartValidator validator = new UserBranchPartValidator();
            ValidationResult result = validator.Validate(userpart);
            if (!result.IsValid) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, result.Errors.ToString());
                return null;
            }

            //Check for entity existency if entityid <> -1
            Entity ent = null;
            if (userpart.EntityId == -1)
            {
                //It means that we need to find the entity id by part name
                foreach (Entity e in db.Entities)
                    if (EntityNamePattern.EntityNamePattern.MatchPattern(e.NamePattern, userpart.Name))
                    {
                        ent = e;
                        break;
                    }

                if (ent == null)
                {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "No entity which name pattern match with part name was found");
                    return null;
                }
            }
            else
            {
                //It means that an Entity id was already provided
                ent = db.Entities.Where(e => e.Id == userpart.EntityId).FirstOrDefault();
                if (ent == null) {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Entity referencedd in UserBranchPart was not found");
                    return null;
                }

                //Validate part name
                if (!EntityNamePattern.EntityNamePattern.MatchPattern(ent.NamePattern, userpart.Name))
                {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "User part name does not match with entity name pattern");
                    return null;
                }
            }

            //Check if part exists already. If does not then create it
            var part = db.UserBranchParts.Where(e => e.UserBranchId == userpart.UserBranchId & e.Name.ToLower() == userpart.Name.ToLower()).FirstOrDefault();
            if (part == null) {
                //Create it
                part = new UserBranchPart() {
                    CreatedOn = DateTime.UtcNow,
                    UserBranchId = userpart.UserBranchId,
                    EntityId = ent.Id,
                    Name = userpart.Name.ToUpper(),
                    Description = userpart.Description
                };
                db.UserBranchParts.Add(part);
                db.SaveChanges();
            }

            return part;
        }
        
        public override object Get(int entityid)
        {
            throw new NotImplementedException();
        }

        public override object GetAll()
        {
            throw new NotImplementedException();
        }
        public List<KeyValuePair<int, UserBranchPart>> GetPartPlacements(int parentid) {
            var query = from ubpp in db.UserBranchPartPlacements
                        join ubp in db.UserBranchParts on ubpp.ChildUserBranchPartId equals ubp.Id
                        where ubpp.ParentUserBranchPartId == parentid
                        select new
                        {
                            UserBranchPart = ubp,
                            ubpp.Qty
                        };
            List<KeyValuePair<int, UserBranchPart>> result = new List<KeyValuePair<int, UserBranchPart>>();
            foreach (var elem in query) {
                result.Add(new KeyValuePair<int, UserBranchPart>(elem.Qty, elem.UserBranchPart));
            }
            return result;
        }

        public override bool Remove(int entityid)
        {
            var entity = db.UserBranchParts.Where(e => e.Id == entityid).FirstOrDefault();
            if (entity == null) return true;
            db.UserBranchParts.Remove(entity);
            db.SaveChanges();
            return true;
        }

        public override bool Update(object entity)
        {
            throw new NotImplementedException();
        }

        public bool IsContainerPart(int partid) {
            var entity = (from p in db.UserBranchParts
                         join e in db.Entities on p.EntityId equals e.Id
                         select e).FirstOrDefault();
            if (entity == null) return false;
            return entity.IsContainer;
        }
        public List<UserBranchPart> GetContainers(string username, string projectnumber) {
            var query = from ubp in db.UserBranchParts
                        join ub in db.UserBranches on ubp.UserBranchId equals ub.Id
                        join e in db.Entities on ubp.EntityId equals e.Id
                        where e.IsContainer & ub.Username == username
                        select ubp;
            if (projectnumber != string.Empty) {
                query = from ubp in query
                        join ub in db.UserBranches on ubp.UserBranchId equals ub.Id
                        join p in db.Projects on ub.ProjectId equals p.Id
                        where p.Number == projectnumber
                        select ubp;
            }
            return query.ToList();
        }
    }
}
