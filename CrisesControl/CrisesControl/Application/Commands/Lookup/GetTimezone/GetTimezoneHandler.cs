using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTimezone
{
    public class GetTimezoneHandler: IRequestHandler<GetTimezoneRequest,GetTimezoneResponse>
    {
        private readonly ILookupQuery _lookupQuery;
        public GetTimezoneHandler(ILookupQuery lookupQuery)
        {
            _lookupQuery = lookupQuery;
        }

        public async Task<GetTimezoneResponse> Handle(GetTimezoneRequest request, CancellationToken cancellationToken)
        {
            var response = await _lookupQuery.GetTimezone();
            return response;
        }
    }
}
