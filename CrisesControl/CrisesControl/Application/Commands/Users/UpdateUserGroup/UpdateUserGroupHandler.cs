using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUserGroup
{
    public class UpdateUserGroupHandler: IRequestHandler<UpdateUserGroupRequest, UpdateUserGroupResponse>
    {
     
        private readonly ILogger<UpdateUserGroupHandler> _logger;
        private readonly IUserRepository _userRepository;
      
        public UpdateUserGroupHandler( ILogger<UpdateUserGroupHandler> logger, IUserRepository userRepository)
        {
         this._logger = logger;
         this._userRepository = userRepository;
        }

        public async Task<UpdateUserGroupResponse> Handle(UpdateUserGroupRequest request, CancellationToken cancellationToken)
        {

            try
            {
                Guard.Against.Null(request, nameof(UpdateUserGroupRequest));
                var results = await _userRepository.UpdateUserMsgGroups(request.UserGroups);
                if (results)
                {
                    return new UpdateUserGroupResponse
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Message = "Update Successfully",
                        result = results
                    };
                }
                return new UpdateUserGroupResponse
                {
                    StatusCode = System.Net.HttpStatusCode.IMUsed,
                    Message = "Not data Found",
                    result = results
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException, ex.StackTrace);
                return new UpdateUserGroupResponse { };
             }
        }
    }
}
