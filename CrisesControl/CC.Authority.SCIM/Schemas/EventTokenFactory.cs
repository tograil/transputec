﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;

namespace CC.Authority.SCIM.Schemas
{
    public abstract class EventTokenFactory
    {
        protected EventTokenFactory(string issuer, JwtHeader header)
        {
            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ArgumentNullException(nameof(issuer));
            }

            this.Issuer = issuer;
            this.Header = header ?? throw new ArgumentNullException(nameof(header));
        }

        public JwtHeader Header
        {
            get;
            private set;
        }

        public string Issuer
        {
            get;
            private set;
        }

        public abstract IEventToken Create(IDictionary<string, object> events);
    }
}