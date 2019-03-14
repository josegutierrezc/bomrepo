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
    public class UserBranchPartPlacementsManager : EntityManagerBase
    {
        public UserBranchPartPlacementsManager(BRXXXXXModel db) : base(db) {
        }

        public override object Add(object entity)
        {
            try
            {
                //Validation
                UserBranchPartPlacement placement = entity as UserBranchPartPlacement;
                if (placement == null)
                {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "UserBranchPartPlacement object required");
                    return null;
                }

                UserBranchPartPlacementValidator validator = new UserBranchPartPlacementValidator();
                ValidationResult result = validator.Validate(placement);
                if (!result.IsValid)
                {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, result.Errors.ToString());
                    return null;
                }

                //Verify existency
                var newplacement = db.UserBranchPartPlacements.Where(e => e.UserBranchId == placement.UserBranchId & e.ParentUserBranchPartId == placement.ParentUserBranchPartId & e.ChildUserBranchPartId == placement.ChildUserBranchPartId).FirstOrDefault();
                if (newplacement == null)
                {
                    //Create it
                    newplacement = new UserBranchPartPlacement()
                    {
                        UserBranchId = placement.UserBranchId,
                        ParentUserBranchPartId = placement.ParentUserBranchPartId,
                        ChildUserBranchPartId = placement.ChildUserBranchPartId,
                        Qty = placement.Qty
                    };
                    db.UserBranchPartPlacements.Add(newplacement);
                    db.SaveChanges();
                }
                else
                {
                    newplacement.Qty = placement.Qty;
                    db.SaveChanges();
                }

                return newplacement;
            }
            catch (Exception e) {
                return null;
            }
        }

        public override object Get(int entityid)
        {
            throw new NotImplementedException();
        }

        public List<UserBranchPart> GetContainerPlacements(int parentid) {
            var query = from ubpp in db.UserBranchPartPlacements
                        join child in db.UserBranchParts on ubpp.ChildUserBranchPartId equals child.Id
                        where ubpp.ParentUserBranchPartId == parentid
                        select child;
            return query.ToList();            
        }

        public override object GetAll()
        {
            throw new NotImplementedException();
        }

        public override bool Remove(int entityid)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAll(int parentid) {
            var placements = db.UserBranchPartPlacements.Where(e => e.ParentUserBranchPartId == parentid);
            db.UserBranchPartPlacements.RemoveRange(placements);
            db.SaveChanges();
            return true;
        }

        public bool Add(List<UserBranchPartPlacement> placements) {
            foreach (UserBranchPartPlacement placement in placements)
                db.UserBranchPartPlacements.Add(placement);
            db.SaveChanges();
            return true;
        }

        public override bool Update(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
