using Ardalis.GuardClauses;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.MemberShipList
{
    public class MembershipListHandler : IRequestHandler<MemberShipListRequest, MemberShipListResponse>
    {
        private readonly IUserRepository _userRepository;
        private ILogger<MembershipListHandler> _logger;
        public MembershipListHandler(IUserRepository userRepository,ILogger<MembershipListHandler> logger)
        {
            this._userRepository = userRepository;
            this._logger = logger;
        }
        public async Task<MemberShipListResponse> Handle(MemberShipListRequest request, CancellationToken cancellationToken)
        {
            try
            {

                Guard.Against.Null(request, nameof(MemberShipListRequest));            
               var membership = await _userRepository.MembershipList(request.ObjMapID, request.MemberShipType, request.TargetID, request.Start, request.Length, request.search, request.order, request.ActiveOnly, request.CompanyKey);
                DataTablePaging rtn = new DataTablePaging();
                rtn.recordsFiltered = membership.Count();
                rtn.data = membership;
                int totalRecord = membership.Count();
                rtn.draw = request.Draw;
                    rtn.recordsTotal = totalRecord;
               
               
                return new MemberShipListResponse
                {
                    recordsFiltered = rtn.recordsFiltered,
                    data = rtn.data,
                    draw = request.Draw,
                    recordsTotal = rtn.recordsTotal,

                };

            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured whikle trying to seed the database {0},{1},{2},{3}",ex.Message, ex.InnerException,ex.StackTrace, ex.Source);
            }
            return new MemberShipListResponse { };
     

          
        }
    }
}
