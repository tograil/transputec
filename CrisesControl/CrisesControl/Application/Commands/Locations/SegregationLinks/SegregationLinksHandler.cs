using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Groups;
using CrisesControl.Core.Locations.Services;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.SegregationLinks
{
    public class SegregationLinksHandler : IRequestHandler<SegregationLinksRequest, SegregationLinksResponse>
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public SegregationLinksHandler(ILocationRepository locationRepository, IMapper mapper, ICurrentUser currentUser)
        {
            this._locationRepository = locationRepository;
            this._mapper = mapper;
            this._currentUser = currentUser;
        }
        public async Task<SegregationLinksResponse> Handle(SegregationLinksRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _locationRepository.SegregationLinks(request.TargetID,request.MemberShipType,request.LinkType, _currentUser.UserId, _currentUser.CompanyId);
                var result = _mapper.Map<List<GroupLink>>(location);
                var response = new SegregationLinksResponse();
                if (result!=null)
                {
                    response.Data = result;
                }
                else
                {
                    response.Data = new List<GroupLink>();
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
