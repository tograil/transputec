﻿using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.CheckDepartment
{
    public class CheckDepartmentHandler : IRequestHandler<CheckDepartmentRequest, CheckDepartmentResponse>
    {
        private readonly IDepartmentQuery _departmentQuery;
        private readonly ILogger<CheckDepartmentHandler> _logger;
        private readonly CheckDepartmentValidator _departmentValidator;
        public CheckDepartmentHandler(ILogger<CheckDepartmentHandler> logger, IDepartmentQuery departmentQuery, CheckDepartmentValidator departmentValidator)
        {
            this._departmentQuery = departmentQuery;
            this._logger = logger;
            this._departmentValidator = departmentValidator;
        }
        public async Task<CheckDepartmentResponse> Handle(CheckDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CheckDepartmentRequest));
            await _departmentValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _departmentQuery.CheckDepartment(request);
            return result;
        }
    }
}
