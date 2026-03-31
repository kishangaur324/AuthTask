using AuthTask.Application.DTOs;
using AuthTask.Domain.Entities;
using AutoMapper;

namespace AuthTask.MapperProfiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>();
            CreateMap<Employee, EmployeeDto>();
        }
    }
}
