using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomRepo.ErrorsCatalog;

namespace BomRepo.BRXXXXX.DL
{
    public class UserBranchPartPropertiesManager : EntityManagerBase
    {
        public UserBranchPartPropertiesManager(BRXXXXXModel db) : base(db) { }

        public override object Add(object entity)
        {
            UserBranchPartProperty partproperty = entity as UserBranchPartProperty;
            if (partproperty == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "UserBranchPartProperty object required.");
                return null;
            }

            //Search for part
            UserBranchPart part = db.UserBranchParts.Where(e => e.Id == partproperty.UserBranchPartId).FirstOrDefault();
            if (part == null) {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Part referenced in UserBranchPartProperty.UserBranchPartId was not found.");
                return null;
            }

            //Search for Property
            Property property = db.Properties.Where(e => e.Id == partproperty.PropertyId).FirstOrDefault();
            if (property == null)
            {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Property referenced in UserBranchPartProperty.PropertyId was not found.");
                return null;
            }

            //Search for PartProperty
            UserBranchPartProperty newpartproperty = db.UserBranchPartProperties.Where(e => e.UserBranchPartId == partproperty.UserBranchPartId & e.PropertyId == partproperty.PropertyId).FirstOrDefault();
            if (newpartproperty == null) {
                //If it does not exist then add it
                newpartproperty = new UserBranchPartProperty()
                {
                    UserBranchPartId = partproperty.UserBranchPartId,
                    PropertyId = partproperty.PropertyId,
                    Value = partproperty.Value
                };
                db.UserBranchPartProperties.Add(newpartproperty);
                db.SaveChanges();
                return newpartproperty;
            }

            //If it does exist then update its value and return it
            newpartproperty.Value = partproperty.Value;
            db.SaveChanges();
            return newpartproperty;
        } 

        public override object Get(int entityid)
        {
            throw new NotImplementedException();
        }

        public override object GetAll()
        {
            throw new NotImplementedException();
        }

        public List<UserBranchPartProperty> GetAll(int userbranchpartid) {
            var query = from ubpp in db.UserBranchPartProperties
                        join prop in db.Properties on ubpp.PropertyId equals prop.Id
                        where ubpp.UserBranchPartId == userbranchpartid
                        select new UserBranchPartProperty
                        {
                            UserBranchPartId = ubpp.UserBranchPartId,
                            PropertyId = ubpp.PropertyId,
                            Value = ubpp.Value,
                            PropertyName = prop.Name,
                        };
            return query.ToList();
        }

        public override bool Remove(int entityid)
        {
            throw new NotImplementedException();
        }
        public void RemoveAll(int userbranchpartid) {
            db.UserBranchPartProperties.RemoveRange(db.UserBranchPartProperties.Where(e => e.UserBranchPartId == userbranchpartid));
            db.SaveChanges();
        }

        public override bool Update(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
