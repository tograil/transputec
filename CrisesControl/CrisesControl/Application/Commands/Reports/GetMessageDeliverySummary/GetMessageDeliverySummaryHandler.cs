
using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Maintenance;
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
        private readonly IExceptionFilter _errorFilter;
        public GetMessageDeliverySummaryHandler(GetMessageDeliverySummaryValidator getMessageDeliverySummaryValidator, ILogger<GetMessageDeliverySummaryHandler> logger, IReportsQuery reportsQuery, ErrorFilter errorFilter)
        {
            this._getMessageDeliverySummaryValidator = getMessageDeliverySummaryValidator;
            this._reportsQuery=reportsQuery;
            this._logger = logger;
            this._errorFilter = errorFilter;
        }
        public async Task<GetMessageDeliverySummaryResponse> Handle(GetMessageDeliverySummaryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Guard.Against.Null(request, nameof(GetMessageDeliverySummaryRequest));
                await _getMessageDeliverySummaryValidator.ValidateAndThrowAsync(request, cancellationToken);

                var result = await _reportsQuery.GetMessageDeliverySummary(request);
                return result;
            }
            catch (Exception ex)
            {
                var producesContentAttribute = new ProducesAttribute("application/json");
               
                var actionContext = new ActionContext()
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor()
                };
                IList<IFilterMetadata> filterMetadatas = new List<IFilterMetadata>()
                { 
                    producesContentAttribute
                
                };
                ExceptionContext custException = new ExceptionContext(actionContext, filterMetadatas);
                _errorFilter.OnException(custException);

                return new GetMessageDeliverySummaryResponse { };

            }
        }
    }
}
