using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Communication.Repositories;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferenceList
{
    public class GetUserActiveConferenceListHandler : IRequestHandler<GetUserActiveConferenceListRequest, GetUserActiveConferenceListResponse>
    {
        private readonly GetUserActiveConferenceListValidator _communicationValidator;
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ICommunicationRepository _communicationService;

        public GetUserActiveConferenceListHandler(GetUserActiveConferenceListValidator communicationValidator,
            ICommunicationRepository communicationService,
            ICommunicationQuery communicationQuery)
        {

           this._communicationValidator = communicationValidator;
           this._communicationQuery = communicationQuery;
            this._communicationService= communicationService;
        }

        public async Task<GetUserActiveConferenceListResponse> Handle(GetUserActiveConferenceListRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserActiveConferenceListRequest));
            
            await _communicationValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var users = await _communicationQuery.GetUserActiveConferenceList(request);
            return users;
        }
    }
}
