using BomRepo.ErrorsCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class UserBranchesManager : EntityManagerBase
    {
        public UserBranchesManager(BRXXXXXModel db) : base(db) {
        }
        public List<UserBranch> Get(string username, string projectnumber)
        {
            if (username == string.Empty & projectnumber == string.Empty)
            {
                return db.UserBranches.ToList();
            }
            else if (username != string.Empty & projectnumber == string.Empty)
            {
                return db.UserBranches.Where(e => e.Username == username).ToList();
            }
            else if (username == string.Empty & projectnumber != string.Empty) {
                var ent = (from ub in db.UserBranches
                                join p in db.Projects on ub.ProjectId equals p.Id
                                where p.Number == projectnumber
                                select ub).ToList();
                return ent;
            }

            var entities = (from ub in db.UserBranches
                            join p in db.Projects on ub.ProjectId equals p.Id
                            where ub.Username == username & p.Number == projectnumber
                            select ub).ToList();
            return entities;
        }
        public override object Add(object entity) {
            //Typecast
            UserBranch userbranch = entity as UserBranch;
            if (userbranch == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "UserBranch object cannot be null");
                return null;
            }
            if (userbranch.Username == null | userbranch.Username == string.Empty) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Username cannot be null or empty");
                return null;
            }

            //Search for a branch that meet these requirements
            var current = db.UserBranches.Where(e => e.Username == userbranch.Username & e.ProjectId == userbranch.ProjectId).FirstOrDefault();

            //If no branch was found then add it
            if (current == null) {
                //Search for project
                var project = db.Projects.Where(e => e.Id == userbranch.ProjectId).FirstOrDefault();
                if (project == null)
                {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Project does not exist");
                    return null;
                }

                UserBranch newentity = new UserBranch() {
                    Username = userbranch.Username,
                    ProjectId = userbranch.ProjectId,
                    CreatedOn = DateTime.UtcNow,
                };
                db.UserBranches.Add(newentity);
                db.SaveChanges();

                //Return it
                return newentity;
            }

            //If a branch was found then return it
            return current;
        }
        public bool Update(string username, string projectnumber) {
            var entity = (from ub in db.UserBranches
                           join p in db.Projects on ub.ProjectId equals p.Id
                           where p.Number == projectnumber
                           select ub).FirstOrDefault();

            if (entity == null) return false;

            entity.ModifiedOn = DateTime.UtcNow;
            db.SaveChanges();
            return true;
        }
        
        public override object GetAll()
        {
            throw new NotImplementedException();
        }

        public override object Get(int entityid)
        {
            throw new NotImplementedException();
        }

        public override bool Update(object entity)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(int entityid)
        {
            throw new NotImplementedException();
        }
    }
}
