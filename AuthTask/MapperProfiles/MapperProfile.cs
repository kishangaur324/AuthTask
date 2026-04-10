using AuthTask.Application.DTOs;
using AuthTask.Domain.Entities;
using AutoMapper;

namespace AuthTask.MapperProfiles
{
    /// <summary>
    /// Centralized AutoMapper profile for employee-related mappings.
    /// </summary>
    public class MapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperProfile"/> class.
        /// </summary>
        public MapperProfile()
        {
            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>();
            CreateMap<Employee, EmployeeDto>();
        }
    }
}
