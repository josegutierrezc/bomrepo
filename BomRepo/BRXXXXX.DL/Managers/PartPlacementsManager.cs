using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomRepo.ErrorsCatalog;
using FluentValidation;
using FluentValidation.Results;

namespace BomRepo.BRXXXXX.DL
{
    public class PartPlacementsManager : EntityManagerBase
    {
        public PartPlacementsManager(BRXXXXXModel db) : base(db) { }

        public override object Add(object entity)
        {
            try
            {
                //Validation
                PartPlacement placement = entity as PartPlacement;
                if (placement == null)
                {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "UserBranchPartPlacement object required");
                    return null;
                }

                PartPlacementValidator validator = new PartPlacementValidator();
                ValidationResult result = validator.Validate(placement);
                if (!result.IsValid)
                {
                    errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, result.Errors.ToString());
                    return null;
                }

                //Verify existency
                var newplacement = db.PartPlacements.Where(e => e.ParentPartId == placement.ParentPartId & e.ChildPartId == placement.ChildPartId).FirstOrDefault();
                if (newplacement == null)
                {
                    //Create it
                    newplacement = new PartPlacement()
                    {
                        ParentPartId = placement.ParentPartId,
                        ChildPartId = placement.ChildPartId,
                        Qty = placement.Qty
                    };
                    db.PartPlacements.Add(newplacement);
                    db.SaveChanges();
                }
                else
                {
                    newplacement.Qty = placement.Qty;
                    db.SaveChanges();
                }

                return newplacement;
            }
            catch (Exception e)
            {
                return null;
            }
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

        public bool Add(List<PartPlacement> placements)
        {
            foreach (PartPlacement placement in placements)
                db.PartPlacements.Add(placement);
            db.SaveChanges();
            return true;
        }
    }
}
