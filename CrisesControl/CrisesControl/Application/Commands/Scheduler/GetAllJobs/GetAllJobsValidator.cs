using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Scheduler.GetAllJobs
{
    public class GetAllJobsValidator:AbstractValidator<GetAllJobsRequest>
    {
       public GetAllJobsValidator()
       {
            RuleFor(x => x.UserID).GreaterThan(0);
            RuleFor(x => x.CompanyID).GreaterThan(0);
        }
    }
}
