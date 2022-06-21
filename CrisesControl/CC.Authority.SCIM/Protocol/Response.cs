//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Globalization;
using System.Net;

namespace CC.Authority.SCIM.Protocol
{
    internal class Response : IResponse
    {
        private readonly object thisLock = new object();

        private HttpResponseClass responseClass;
        private HttpStatusCode statusCode;
        private string statusCodeValue;

        private enum HttpResponseClass
        {
            Informational = 1,
            Success = 2,
            Redirection = 3,
            ClientError = 4,
            ServerError = 5
        }

        public HttpStatusCode Status
        {
            get
            {
                return this.statusCode;
            }

            set
            {
                this.StatusCodeValue = ((int)value).ToString(CultureInfo.InvariantCulture);
            }
        }

        public string StatusCodeValue
        {
            get
            {
                return this.statusCodeValue;
            }

            set
            {
                lock (this.thisLock)
                {
                    this.statusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), value);
                    this.statusCodeValue = value;
                    char responseClassSignifier = this.statusCodeValue.First();
                    double responseClassNumber = char.GetNumericValue(responseClassSignifier);
                    int responseClassCode = Convert.ToInt32(responseClassNumber);
                    this.responseClass = (HttpResponseClass)Enum.ToObject(typeof(HttpResponseClass), responseClassCode);
                }
            }
        }

        public bool IsError()
        {
            bool result = HttpResponseClass.ClientError == this.responseClass
                            || HttpResponseClass.ServerError == this.responseClass;
            return result;
        }
    }
}