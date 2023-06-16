using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Locations.Services;
using CrisesControl.SharedKernel.Utils;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.UpdateSegregationLink
{
    public class UpdateSegregationLinkHandler : IRequestHandler<UpdateSegregationLinkRequest, UpdateSegregationLinkResponse>
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<UpdateSegregationLinkHandler> _logger;
        private readonly ICurrentUser _currentUser;
        public UpdateSegregationLinkHandler(ILocationRepository locationRepository, ILogger<UpdateSegregationLinkHandler> logger, ICurrentUser currentUser)
        {
            this._locationRepository = locationRepository;
            this._logger = logger;
            this._currentUser = currentUser;
        }
        public async Task<UpdateSegregationLinkResponse> Handle(UpdateSegregationLinkRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _locationRepository.UpdateSegregationLink(request.SourceID,request.TargetID, request.LinkType, _currentUser.CompanyId);
                //var result = _mapper.Map<bool>(location);
                var response = new UpdateSegregationLinkResponse();
                if (location)
                {
                    response.Result = location;
                    response.Message = "Updated Segregation Link";
                }
                else
                {
                    response.Result = false;
                    response.Message = "No data found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
