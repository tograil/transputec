﻿using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.CompleteRegistration
{
    public class CompleteRegistrationRequest : IRequest<CompleteRegistrationResponse>
    {
        public CompleteRegistrationRequest()
        {
            CountryCode = "GBR";
            RegAction = "SAVE";
        }
        public int RegId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NewRegEmail { get; set; }
        public string Password { get; set; }
        public string MobileISD { get; set; }
        public string MobileNo { get; set; }
        public string VerificationCode { get; set; }
        public string CompanyName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public string UniqueRef { get; set; }
        public int Status { get; set; }
        public int PackagePlanId { get; set; }
        public string PaymentMethod { get; set; }
        public string RegAction { get; set; }
        public string CustomerId { get; set; }
        public string Sector { get; set; }
    }
}
