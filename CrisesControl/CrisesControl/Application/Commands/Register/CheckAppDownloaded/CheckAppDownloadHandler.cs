using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Register.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.CheckAppDownloaded
{
    public class CheckAppDownloadHandler : IRequestHandler<CheckAppDownloadRequest, CheckAppDownloadResponse>
    {
        private readonly IRegisterQuery _registerQuery;
        public CheckAppDownloadHandler(IRegisterQuery registerQuery)
        {
            _registerQuery = registerQuery;
        }
        public async Task<CheckAppDownloadResponse> Handle(CheckAppDownloadRequest request, CancellationToken cancellationToken)
        {
            var result = await  _registerQuery.CheckAppDownload(request);
            return result;
        }
    }
}
