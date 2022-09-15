using MediatR;

namespace CrisesControl.Api.Application.Commands.System.ExportCompanyData
{
    public class ExportCompanyDataRequest:IRequest<ExportCompanyDataResponse>
    {
        public string Entity { get; set; }
        public bool ShowDeleted { get; set; }
    }
}
