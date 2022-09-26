using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetCountry
{
    public class GetCountryHandler : IRequestHandler<GetCountryRequest, GetCountryResponse>
    {
        private readonly ILookupQuery _lookupQuery;
        public GetCountryHandler(ILookupQuery lookupQuery)
        {
            _lookupQuery = lookupQuery;
        }

        public Task<GetCountryResponse> Handle(GetCountryRequest request, CancellationToken cancellationToken)
        {
            var result = _lookupQuery.GetCountry();
            return result;
        }
    }
}
