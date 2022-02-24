using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.DepartmentAggregate.Handles.GetDepartment
{
    public class GetDepartmentRequest : IRequest<GetDepartmentResponse>
    {
        public int DepartmentId { get; set; }  
        public int CompanyId { get; set; }
    }
}
