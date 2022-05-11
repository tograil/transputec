﻿using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports.Repositories;
using MediatR;
using Serilog;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;

public class GetIndidentMessageNoAckHandler: IRequestHandler<GetIndidentMessageNoAckRequest, GetIndidentMessageNoAckResponse>
{
    private readonly IReportsRepository _reportRepository;
    private readonly GetIndidentMessageNoAckValidator _getIndidentMessageNoAckValidator;
    public GetIndidentMessageNoAckHandler(IReportsRepository reportRepository, GetIndidentMessageNoAckValidator getIndidentMessageNoAckValidator)
    {
        this._reportRepository = reportRepository;
        this._getIndidentMessageNoAckValidator=getIndidentMessageNoAckValidator;
    }

    public async Task<GetIndidentMessageNoAckResponse> Handle(GetIndidentMessageNoAckRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _reportRepository.GetIndidentMessageNoAck(request.draw, request.IncidentActivationId, request.RecordStart, request.RecordLength, request.SearchString, request.OrderBy, request.OrderDir);

            

          
            if (result != null)
            {
                return new GetIndidentMessageNoAckResponse
                {
                    data = result,
                    Message = "No record found.",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            return new GetIndidentMessageNoAckResponse
            {
                data = new List<DataTablePaging>(),
                Message = "Loaded Succesfull.",
                StatusCode = System.Net.HttpStatusCode.NotFound

            };
        }
        catch (Exception ex)
        {
            Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                         ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            return new GetIndidentMessageNoAckResponse { };
        }
        
    }


}
