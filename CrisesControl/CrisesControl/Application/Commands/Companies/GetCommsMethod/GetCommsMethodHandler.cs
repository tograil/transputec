using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetCommsMethod {
    public class GetCommsMethodHandler : IRequestHandler<GetCommsMethodRequest, GetCommsMethodResponse> {

        private readonly ICompanyQuery _companyQuery;

        public GetCommsMethodHandler(ICompanyQuery companyQuery) {
            _companyQuery = companyQuery;
        }

        public async Task<GetCommsMethodResponse> Handle(GetCommsMethodRequest request, CancellationToken cancellationToken) {
            var result = await _companyQuery.GetCommsMethod(cancellationToken);
            return result;
        }
    }
}
