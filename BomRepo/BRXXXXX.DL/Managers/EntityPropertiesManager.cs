using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BomRepo.BRXXXXX.DL
{
    public class EntityPropertiesManager : EntityManagerBase
    {
        public EntityPropertiesManager(BRXXXXXModel db) : base(db) { }

        public override object Add(object entity)
        {
            throw new NotImplementedException();
        }

        public List<EntityProperty> Get(string projectnumber, int? entityid) {
            var query = from p in db.Projects
                        join pe in db.ProjectEntities on p.Id equals pe.ProjectId
                        join eprop in db.EntityProperties on pe.EntityId equals eprop.EntityId
                        join prop in db.Properties on eprop.PropertyId equals prop.Id
                        where p.Number == projectnumber
                        orderby eprop.ShowOrder
                        select new EntityProperty {
                            EntityId = eprop.EntityId,
                            PropertyId = eprop.PropertyId,
                            Property = prop
                        };
            if (entityid == null) return query.ToList();

            return query.Where(e => e.EntityId == entityid).ToList();
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
