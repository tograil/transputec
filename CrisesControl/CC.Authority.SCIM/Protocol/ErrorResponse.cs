//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Net;
using System.Runtime.Serialization;
using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Protocol
{
    [DataContract]
    public sealed class ErrorResponse : Schematized
    {
        private ErrorType errorType;

        [DataMember(Name = ProtocolAttributeNames.ErrorType)]
        private string errorTypeValue;

        private Response response;

        public ErrorResponse()
        {
            this.Initialize();
            this.AddSchema(ProtocolSchemaIdentifiers.Version2Error);
        }

        [DataMember(Name = ProtocolAttributeNames.Detail)]
        public string Detail
        {
            get;
            set;
        }

        public ErrorType ErrorType
        {
            get
            {
                return this.errorType;
            }

            set
            {
                this.errorType = value;
                this.errorTypeValue = Enum.GetName(typeof(ErrorType), value);
            }
        }

        public HttpStatusCode Status
        {
            get
            {
                return this.response.Status;
            }

            set
            {
                this.response.Status = value;
            }
        }

        private void Initialize()
        {
            this.response = new Response();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}