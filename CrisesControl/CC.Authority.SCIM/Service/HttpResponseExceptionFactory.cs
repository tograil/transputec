// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using System.Net;
using System.Web.Http;

namespace CC.Authority.SCIM.Service
{
    internal abstract class HttpResponseExceptionFactory<T>
    {
        public abstract HttpResponseMessage ProvideMessage(HttpStatusCode statusCode, T content);

        public HttpResponseException CreateException(HttpStatusCode statusCode, T content)
        {
            HttpResponseMessage message = null;
            try
            {
                message = this.ProvideMessage(statusCode, content);
                HttpResponseException result = new HttpResponseException(message);
                result = null;
                return result;
            }
            finally
            {
                if (message != null)
                {
                    message.Dispose();
                    message = null;
                }
            }
        }
    }
}
