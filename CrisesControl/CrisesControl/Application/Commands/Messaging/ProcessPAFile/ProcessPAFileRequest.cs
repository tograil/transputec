using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.ProcessPAFile
{
    public class ProcessPAFileRequest : IRequest<ProcessPAFileResponse>
    {
        public string UserListFile { get; set; }
        public bool HasHeader { get; set; }
        public int EmailColIndex {get;set;}
        public int PhoneColIndex {get;set;}
        public int PostcodeColIndex {get;set;}
        public int LatColIndex {get;set;}
        public int LongColIndex {get;set;}
        public string SessionId {get;set;}
    }
}
