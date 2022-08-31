using MediatR;

namespace CrisesControl.Api.Application.Commands.Payments.AddRemoveModule
{
    public class AddRemoveModuleRequest:IRequest<AddRemoveModuleResponse>
    {
        public int ModuleID { get; set; }
        public string ActionValue { get; set; }
    }
}
