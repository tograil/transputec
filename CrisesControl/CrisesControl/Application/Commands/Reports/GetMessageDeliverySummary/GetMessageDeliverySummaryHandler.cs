
using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Maintenance;
using CrisesControl.Core.Exceptions.NotFound;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageDeliverySummary
{
    public class GetMessageDeliverySummaryHandler : IRequestHandler<GetMessageDeliverySummaryRequest, GetMessageDeliverySummaryResponse>
    {
        private readonly GetMessageDeliverySummaryValidator _getMessageDeliverySummaryValidator;
        private readonly ILogger<GetMessageDeliverySummaryHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        private readonly ICurrentUser _currentUser;   
    
        public GetMessageDeliverySummaryHandler(GetMessageDeliverySummaryValidator getMessageDeliverySummaryValidator, ILogger<GetMessageDeliverySummaryHandler> logger, IReportsQuery reportsQuery, ICurrentUser currentUser)
        {
            this._getMessageDeliverySummaryValidator = getMessageDeliverySummaryValidator;
            this._reportsQuery=reportsQuery;
            this._logger = logger;
            this._currentUser = currentUser;
          
        }
        public async Task<GetMessageDeliverySummaryResponse> Handle(GetMessageDeliverySummaryRequest request, CancellationToken cancellationToken)
        {
            
                Guard.Against.Null(request, nameof(GetMessageDeliverySummaryRequest));
                await _getMessageDeliverySummaryValidator.ValidateAndThrowAsync(request, cancellationToken);
                var result = await _reportsQuery.GetMessageDeliverySummary(request);
            if(result!= null) {
                return result;
            }
            throw new MessageNotFoundException(_currentUser.CompanyId, _currentUser.UserId);

        }
    }
}
