using BomRepo.ErrorsCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class PartPropertiesManager : EntityManagerBase
    {
        public PartPropertiesManager(BRXXXXXModel db) : base(db) { }

        public override object Add(object entity)
        {
            PartProperty partproperty = entity as PartProperty;
            if (partproperty == null)
            {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "PartProperty object required.");
                return null;
            }

            //Search for part
            Part part = db.Parts.Where(e => e.Id == partproperty.PartId).FirstOrDefault();
            if (part == null)
            {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Part referenced in PartProperty.PartId was not found.");
                return null;
            }

            //Search for Property
            Property property = db.Properties.Where(e => e.Id == partproperty.PropertyId).FirstOrDefault();
            if (property == null)
            {
                errorDefinition = ErrorCatalog.CreateFrom(ErrorCatalog.ValidationFailed, "Property referenced in PartProperty.PropertyId was not found.");
                return null;
            }

            //Search for PartProperty
            PartProperty newpartproperty = db.PartProperties.Where(e => e.PartId == partproperty.PartId & e.PropertyId == partproperty.PropertyId).FirstOrDefault();
            if (newpartproperty == null)
            {
                //If it does not exist then add it
                newpartproperty = new PartProperty()
                {
                    PartId = partproperty.PartId,
                    PropertyId = partproperty.PropertyId,
                    Value = partproperty.Value
                };
                db.PartProperties.Add(newpartproperty);
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

        public override bool Remove(int entityid)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll(int partid) {
            db.PartProperties.RemoveRange(db.PartProperties.Where(e => e.PartId == partid));
            db.SaveChanges();
        }

        public override bool Update(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
