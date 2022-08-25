using AutoMapper;
using CrisesControl.Api.Application.Commands.App.GetPrivacyPolicy;
using CrisesControl.Api.Application.Commands.App.GetTnC;
using CrisesControl.Api.Application.Commands.App.ValidatePin;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.App.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class AppQuery : IAppQuery
    {
        private readonly IAppRepository _appRepository;
        private readonly ILogger<AppQuery> _logger;
        private readonly DBCommon _DBC;
        private readonly IMapper _mapper;
        public AppQuery(IAppRepository appRepository, ILogger<AppQuery> logger, DBCommon DBC, IMapper mapper)
        {
            this._appRepository = appRepository;
            this._logger = logger;
            this._mapper = mapper;
        }

        public async Task<GetPrivacyPolicyResponse> GetPrivacyPolicy(GetPrivacyPolicyRequest request)
        {
            try
            {
                var tncText = _DBC.LookupWithKey("PRIVACY_POLICY");
                var result = _mapper.Map<string>(tncText);
                var response = new GetPrivacyPolicyResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.Result = result;
                }
                else
                {
                    response.Result = "No record found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Terms and Condition text/link for App
        /// </summary>
        /// <returns></returns>
        public async Task<GetTnCResponse> GetTnC(GetTnCRequest request)
        {
            try
            {
                var tncText = _DBC.LookupWithKey("TNC"); 
                var result = _mapper.Map<string>(tncText);
                var response = new GetTnCResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.Result = result;                    
                }
                else
                {
                    response.Result = "No record found";                   
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ValidatePinResponse> ValidatePin(ValidatePinRequest request)
        {
            try
            {
                int ValidPin = 12345;
                var response = new ValidatePinResponse();
                if (request.PinNumber == ValidPin)
                {
                    response.PinExpire = 10;
                }
                else
                {
                  
                    response.Message = "Wrong pin entered, please try again";
                   
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
