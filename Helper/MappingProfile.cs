using AutoMapper;
using TodoWebApi.Dto;
using TodoWebApi.Models;

namespace TodoWebApi.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Tasks, TasksDto>();
            CreateMap<TasksDto, Tasks>();
            CreateMap<Owner, OwnerDto>();
            CreateMap<OwnerDto, Owner>();
        }
    }
}
