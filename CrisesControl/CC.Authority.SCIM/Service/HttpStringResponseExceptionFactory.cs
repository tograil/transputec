// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using System.Net;

namespace CC.Authority.SCIM.Service
{
    internal class HttpStringResponseExceptionFactory : HttpResponseExceptionFactory<string>
    {
        private const string ArgumentNameContent = "content";

        private static readonly Lazy<HttpResponseMessageFactory<string>> ResponseMessageFactory =
            new Lazy<HttpResponseMessageFactory<string>>(
                () =>
                    new HttpStringResponseMessageFactory());

        public override HttpResponseMessage ProvideMessage(HttpStatusCode statusCode, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(HttpStringResponseExceptionFactory.ArgumentNameContent);
            }

            HttpResponseMessage result = null;
            try
            {
                result = HttpStringResponseExceptionFactory.ResponseMessageFactory.Value.CreateMessage(statusCode, content);
                return result;
            }
            catch
            {
                if (result != null)
                {
                    result.Dispose();
                    result = null;
                }

                throw;
            }
        }
    }
}
