﻿using AutoMapper;
using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartments;
using CrisesControl.Api.Application.ViewModels.Department;
using CrisesControl.Core.DepartmentAggregate;

namespace CrisesControl.Api.Application.Maps
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, GetDepartmentResponse > ()
            .ForMember(x => x.DepartmentId, m => m.MapFrom(x => x.DepartmentId))
            .ForMember(x => x.CompanyId, m => m.MapFrom(x => x.CompanyId))
            .ForMember(x => x.DepartmentName, m => m.MapFrom(x => x.DepartmentName))
            .ForMember(x => x.UpdatedBy, m => m.MapFrom(x => x.UpdatedBy))
            .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => x.UpdatedOn))
            .ForMember(x => x.CreatedBy, m => m.MapFrom(x => x.CreatedBy))
            .ForMember(x => x.Status, m => m.MapFrom(x => x.Status));


        }
    }
}
