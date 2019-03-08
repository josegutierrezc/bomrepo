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
        public UserBranch Add(UserBranch userbranch) {
            if (userbranch == null) {
                errorDescription = "UserBranch object cannot be null";
                return null;
            }
            if (userbranch.Username == null | userbranch.Username == string.Empty) {
                errorDescription = "Username cannot be null or empty";
                return null;
            }

            //Search for a branch that meet these requirements
            var entity = db.UserBranches.Where(e => e.Username == userbranch.Username & e.ProjectId == userbranch.ProjectId).FirstOrDefault();

            //If no branch was found then add it
            if (entity == null) {
                //Search for project
                var project = db.Projects.Where(e => e.Id == userbranch.ProjectId).FirstOrDefault();
                if (project == null)
                {
                    errorDescription = "Project does not exist";
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
            return entity;
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
        public bool Commit(string username, string projectnumber, string assemblyname, List<UserBranchAssemblyPart> partreferences) {
            //Get user branch
            var userbranch = (from ub in db.UserBranches
                              join p in db.Projects on ub.ProjectId equals p.Id
                              where ub.Username == username & p.Number == projectnumber
                              select ub).FirstOrDefault();
            if (userbranch == null) return false;

            //Get assembly if it exists in user branch and if it does not then create it
            UserBranchAssembly assembly = db.UserBranchAssemblies.Where(e => e.UserBranchId == userbranch.Id & e.Name == assemblyname).FirstOrDefault();
            if (assembly == null) {
                assembly = new UserBranchAssembly()
                {
                    CreatedOn = DateTime.UtcNow,
                    UserBranchId = userbranch.Id,
                    Name = assemblyname
                };
                db.UserBranchAssemblies.Add(assembly);
                db.SaveChanges();
            }

            //If there is any part associated with this assembly then delete it
            var partsexists = db.UserBranchAssemblyParts.Where(e => e.UserBranchAssemblyId == assembly.Id);
            if (partsexists.Count() != 0) {
                db.UserBranchAssemblyParts.RemoveRange(partsexists);
                db.SaveChanges();
            }

            //Add associated parts
            foreach (UserBranchAssemblyPart pr in partreferences) {
                UserBranchAssemblyPart ap = new UserBranchAssemblyPart()
                {
                    UserBranchAssemblyId = assembly.Id,
                    Name = pr.Name,
                    Qty = pr.Qty
                };
                db.UserBranchAssemblyParts.Add(ap);
            }
            db.SaveChanges();

            return true;
        }
    }
}
