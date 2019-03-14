using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BomRepo.BRMaster.DL;
using BomRepo.BRMaster.DTO;
using BomRepo.BRXXXXX.DL;
using BomRepo.BRXXXXX.DTO;
using AutoMapper;

namespace BomRepo.REST.API
{
    public static class AutoMapperConfig
    {
        public static void Register()
        {
            Mapper.Initialize(cfg => {
                cfg.AddProfile(new BRMasterProfile());
                cfg.AddProfile(new BRXXXXProfile());
            });
        }
    }

    public class BRMasterProfile : Profile
    {
        public BRMasterProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<Costumer, CostumerDTO>();
        }
    }

    public class BRXXXXProfile : Profile
    {
        public BRXXXXProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<Part, PartDTO>();
            CreateMap<Project, ProjectDTO>();
            CreateMap<ProjectStatus, ProjectStatusDTO>();
            CreateMap<UserBranch, UserBranchDTO>();
            CreateMap<UserBranchPart, UserBranchPartDTO>();
        }
    }
}
