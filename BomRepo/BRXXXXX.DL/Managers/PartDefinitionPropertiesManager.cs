using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BomRepo.BRXXXXX.DL
{
    public class PartDefinitionPropertiesManager : EntityManagerBase
    {
        public PartDefinitionPropertiesManager(BRXXXXXModel db) : base(db) { }

        public override object Add(object entity)
        {
            throw new NotImplementedException();
        }

        public List<PartDefinitionProperty> Get(string projectnumber, int? entityid) {
            var query = from p in db.Projects
                        join pe in db.ProjectPartDefinitions on p.Id equals pe.ProjectId
                        join eprop in db.PartDefinitionProperties on pe.PartDefinitionId equals eprop.PartDefinitionId
                        join prop in db.Properties on eprop.PropertyId equals prop.Id
                        where p.Number == projectnumber
                        orderby eprop.ShowOrder
                        select new PartDefinitionProperty {
                            PartDefinitionId = eprop.PartDefinitionId,
                            PropertyId = eprop.PropertyId,
                            Property = prop
                        };
            if (entityid == null) return query.ToList();

            return query.Where(e => e.PartDefinitionId == entityid).ToList();
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
