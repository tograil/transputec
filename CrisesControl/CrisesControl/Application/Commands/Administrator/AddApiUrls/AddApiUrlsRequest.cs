using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.AddApiUrls
{
    public class AddApiUrlsRequest:IRequest<AddApiUrlsResponse>
    {
        public string ApiUrl
        {
            get;
            set;
        }

        public string ApiHost
        {
            get;
            set;
        }

        public bool IsCurrent
        {
            get;
            set;
        }

        public int Status
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public string AppVersion
        {
            get;
            set;
        }

        public string ApiMode
        {
            get;
            set;
        }

        public string Platform
        {
            get;
            set;
        }
    }
}
