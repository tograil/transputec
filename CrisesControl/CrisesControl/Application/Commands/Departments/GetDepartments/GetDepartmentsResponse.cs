﻿using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Core.Departments;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartments
{
    public class GetDepartmentsResponse
    {
        public List<Department> Data { get; set; }
    }
}
