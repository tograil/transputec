using CrisesControl.Core.CompanyParameters;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.UpdateProfile
{
    public class UpdateProfileRequest : IRequest<UpdateProfileResponse>
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string? UserPhoto { get; set; }
        public string? Isdcode { get; set; }
        public string? Llisdcode { get; set; }
        public string? Landline { get; set; }
        public string? UserLanguage { get; set; }
        public int[] PingMethod { get; set; }
        //[Required(ErrorMessage = "At least one incident comm method is required")]
        public int[] IncidentMethod { get; set; }
        public List<CommsMethodPriority> CommsMethod { get; set; }
        public int TimezoneId { get; set; }
        public string? MobileNo { get; set; }
    }
}
