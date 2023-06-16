using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Register.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.Index
{
    public class IndexHandler : IRequestHandler<IndexRequest, IndexResponse>
    {
        private readonly ILogger<IndexHandler> _logger;
        //private readonly IRegisterQuery _registerQuery;
        private readonly IRegisterRepository _registerQuery;
        public IndexHandler(ILogger<IndexHandler> logger, IRegisterRepository registerQuery)
        {
            _logger = logger;
            _registerQuery = registerQuery;
        }
        public async Task<IndexResponse> Handle(IndexRequest request, CancellationToken cancellationToken)
        {
            var registrations = await _registerQuery.GetAllRegistrations();
            var result = new IndexResponse();
            if (registrations != null)
            {
                result.Data = registrations;
                result.Message = "Data has been Loaded";
            }
            else
            {
                result.Data = new List<Registration>();
                result.Message = "No record Found.";
            }
            return result;
        }
    }
}
