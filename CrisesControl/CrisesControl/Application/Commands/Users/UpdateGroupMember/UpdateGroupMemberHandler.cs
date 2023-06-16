using Ardalis.GuardClauses;
using CrisesControl.Core.Users.Repositories;
using MediatR;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Users.UpdateGroupMember
{
    public class UpdateGroupMemberHandler : IRequestHandler<UpdateGroupMemberRequest, UpdateGroupMemberResponse>
    {
        private readonly ILogger<UpdateGroupMemberHandler> _logger;
        private readonly IUserRepository _userRepository;
        public UpdateGroupMemberHandler(ILogger<UpdateGroupMemberHandler> logger, IUserRepository userRepository)
        {
            this._userRepository= userRepository;
            this._logger = logger;
        }
        public async Task<UpdateGroupMemberResponse> Handle(UpdateGroupMemberRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Guard.Against.Null(request, nameof(UpdateGroupMemberRequest));
                var member = await _userRepository.UpdateGroupMember(request.TargetID, request.UserID, request.ObjMapID,request.Action);
                if (member)
                {
                    return new UpdateGroupMemberResponse
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "Update Successfully",
                        result = member
                    };
                }
                return new UpdateGroupMemberResponse
                {
                    StatusCode = HttpStatusCode.IMUsed,
                    Message = "Not data Found",
                    result = member
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException, ex.StackTrace);
                return new UpdateGroupMemberResponse { };
            }
        }
    }
}
