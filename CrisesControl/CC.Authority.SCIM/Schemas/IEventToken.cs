//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;

namespace CC.Authority.SCIM.Schemas
{
    public interface IEventToken
    {
        IReadOnlyCollection<string> Audience { get; set; }
        IDictionary<string, object> Events { get; }
        DateTime? Expiration { get; set; }
        JwtHeader Header { get; }
        string Identifier { get; }
        DateTime IssuedAt { get; }
        string Issuer { get; }
        DateTime? NotBefore { get; }
        string Subject { get; set; }
        string Transaction { get; set; }
    }
}