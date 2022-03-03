using AutoMapper;
using CrisesControl.Api.Application.ViewModels.Department;
using CrisesControl.Core.DepartmentAggregate;

namespace CrisesControl.Api.Application.Maps
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<DepartmentInfo, Department>()
            .ForMember(x => x.DepartmentId, m => m.MapFrom(x => Guid.NewGuid()))
            .ForMember(x => x.CompanyId, m => m.MapFrom(x => x.CompanyId))
            .ForMember(x => x.DepartmentName, m => m.MapFrom(x => x.DepartmentName))
            .ForMember(x => x.UpdatedBy, m => m.MapFrom(m => m.UpdatedBy))
            .ForMember(x => x.UpdatedOn, m => m.MapFrom(m => m.UpdatedOn))
            .ForMember(x => x.CreatedOn, m => m.MapFrom(m => m.CreatedOn))
            .ForMember(x => x.CreatedBy, m => m.MapFrom(m => m.CreatedBy))
            .ForMember(x => x.Status, m => m.MapFrom(m => m.Status));
        }
    }
}
