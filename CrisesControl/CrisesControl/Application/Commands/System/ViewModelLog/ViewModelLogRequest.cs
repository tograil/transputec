using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.ViewModelLog
{
    public class ViewModelLogRequest:IRequest<ViewModelLogResponse>
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public List<Order> order { get; set; }
        public Search search { get; set; }
        public int draw { get; set; }
    }
}
