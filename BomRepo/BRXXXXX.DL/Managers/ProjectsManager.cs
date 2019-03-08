﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomRepo.BRXXXXX.DL
{
    public class ProjectsManager : EntityManagerBase
    {
        public ProjectsManager(BRXXXXXModel db) : base(db) {
        }

        public List<Project> GetAll() {
            var all = from p in db.Projects
                      join ps in db.ProjectStatuses on p.ProjectStatusId equals ps.Id
                      select new Project
                      {
                          Id = p.Id,
                          ProjectStatusId = p.ProjectStatusId,
                          Number = p.Number,
                          Name = p.Name,
                          Address = p.Address,
                          City = p.City,
                          State = p.State,
                          ZipCode = p.ZipCode,
                          ProjectStatusName = ps.Name
                      };
            return all.ToList();
        }

        public Project Get(string number) {
            return db.Projects.Where(e => e.Number == number).FirstOrDefault();
        }
    }
}