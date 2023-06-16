//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;

namespace CC.Authority.SCIM.Schemas
{
    public abstract class UnsecuredEventTokenFactory : EventTokenFactory
    {

        private static readonly Lazy<JwtHeader> UnsecuredTokenHeader =
            new Lazy<JwtHeader>(
                () =>
                    UnsecuredEventTokenFactory.ComposeHeader());

        protected UnsecuredEventTokenFactory(string issuer)
            : base(issuer, UnsecuredEventTokenFactory.UnsecuredTokenHeader.Value)
        {
        }

        private static JwtHeader ComposeHeader()
        {
            JwtHeader result = new JwtHeader();
            result.Add(EventToken.HeaderKeyAlgorithm, EventToken.JwtAlgorithmNone);
            return result;
        }
    }
}