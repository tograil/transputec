using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace CrisesControl.Core.CompanyAggregate.Handlers.TempRegister;

public class TempRegisterHandler : IRequestHandler<TempRegisterRequest, TempRegisterResponse>
{
    public Task<TempRegisterResponse> Handle(TempRegisterRequest request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}