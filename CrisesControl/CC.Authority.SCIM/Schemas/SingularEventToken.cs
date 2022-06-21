//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.ObjectModel;
using Microsoft.SCIM;

namespace CC.Authority.SCIM.Schemas
{
    public abstract class SingularEventToken : EventTokenDecorator
    {
        protected SingularEventToken(IEventToken innerToken)
            : base(innerToken)
        {
            if (this.InnerToken.Events.Count != 1)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementSchemasResources.ExceptionSingleEventExpected);
            }

            KeyValuePair<string, object> singleEvent = this.InnerToken.Events.Single();
            this.SchemaIdentifier = singleEvent.Key;
            this.Event = new ReadOnlyDictionary<string, object>((IDictionary<string, object>)singleEvent.Value);
        }

        protected SingularEventToken(string serialized)
            : this(new EventToken(serialized))
        {
        }

        public IReadOnlyDictionary<string, object> Event
        {
            get;
            private set;
        }

        public string SchemaIdentifier
        {
            get;
            private set;
        }
    }
}