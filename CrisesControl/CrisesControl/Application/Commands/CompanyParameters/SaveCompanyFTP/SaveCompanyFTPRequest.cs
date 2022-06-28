using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SaveCompanyFTP
{
    public class SaveCompanyFTPRequest:IRequest<SaveCompanyFTPResponse>
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string SecurityKey { get; set; }
        public string Protocol { get; set; }
        public int Port { get; set; }
        public string RemotePath { get; set; }
        public string LogonType { get; set; }
        public bool DeleteSourceFile { get; set; }
        public string SHAFingerPrint { get; set; }
    }
}
