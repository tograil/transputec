using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports.Repositories;
using FluentValidation;
using MediatR;
using Serilog;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentMessageNoAck;

public class GetIncidentMessageNoAckHandler: IRequestHandler<GetIncidentMessageNoAckRequest, GetIncidentMessageNoAckResponse>
{
    private readonly IReportsRepository _reportRepository;
    private readonly GetIncidentMessageNoAckValidator _getIndidentMessageNoAckValidator;
    public GetIncidentMessageNoAckHandler(IReportsRepository reportRepository, GetIncidentMessageNoAckValidator getIndidentMessageNoAckValidator)
    {
        _reportRepository = reportRepository;
        _getIndidentMessageNoAckValidator=getIndidentMessageNoAckValidator;
    }

    public async Task<GetIncidentMessageNoAckResponse> Handle(GetIncidentMessageNoAckRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Guard.Against.Null(request, nameof(GetIncidentMessageNoAckRequest));
            await _getIndidentMessageNoAckValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportRepository.GetIncidentMessageNoAck(request.draw, request.IncidentActivationId, request.RecordStart, request.RecordLength, request.SearchString, request.UniqueKey);

            

          
            if (result != null)
            {
                return new GetIncidentMessageNoAckResponse
                {
                    data = result,
                    Message = "No record found.",
                    StatusCode = HttpStatusCode.OK
                };
            }
            return new GetIncidentMessageNoAckResponse
            {
                data = new List<DataTablePaging>(),
                Message = "Loaded Succesfull.",
                StatusCode =HttpStatusCode.NotFound

            };
        }
        catch (Exception ex)
        {
            Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                         ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            return new GetIncidentMessageNoAckResponse { };
        }
        
    }


}
