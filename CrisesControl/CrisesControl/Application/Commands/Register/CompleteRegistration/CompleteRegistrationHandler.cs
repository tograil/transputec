using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Register.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.CompleteRegistration
{
    public class CompleteRegistrationHandler:IRequestHandler<CompleteRegistrationRequest,CompleteRegistrationResponse>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<CompleteRegistrationHandler> _logger;
        public CompleteRegistrationHandler(IRegisterRepository registerRepository, ILogger<CompleteRegistrationHandler> logger)
        {
            _registerRepository = registerRepository;
            _logger = logger;
        }

        public async Task<CompleteRegistrationResponse> Handle(CompleteRegistrationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CompleteRegistrationRequest));
            //var mappedRequest = _mapper.Map<Core.Register.TempRegister>(request);
            Core.Register.TempRegister tempRegister = new Core.Register.TempRegister()
            {
                AddressLine1=request.AddressLine1,
                AddressLine2=request.AddressLine2,
                City=request.City,
                CompanyName=request.CompanyName,
                CountryCode=request.CountryCode,
                CustomerId=request.CustomerId,
                Email=request.Email,
                FirstName=request.FirstName,
                LastName=request.LastName,
                MobileISD=request.MobileISD,
                MobileNo=request.MobileNo,
                NewRegEmail=request.NewRegEmail,
                PackagePlanId=request.PackagePlanId,
                Password=request.Password,
                PaymentMethod=request.PaymentMethod,
                Postcode=request.Postcode,
                RegAction=request.RegAction,
                Sector=request.Sector,
                State=request.State,
                Status=1,
                UniqueRef=request.UniqueRef,
                VerificationCode=request.VerificationCode
                
            };
            
            var result = await _registerRepository.CompleteRegistration(tempRegister);
            // var response = _mapper.Map<CompleteRegistrationResponse>(result);
            var response = new CompleteRegistrationResponse();
            response.UserValidatedDTO = result;
            return response;
        }
    }
}
