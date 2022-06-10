//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace CC.Authority.SCIM.Schemas
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "The long inheritence hieararchy reflects the the System for Cross-Domain Identity Management inheritence mechanism.")]
    [DataContract(Name = Core2EnterpriseUser.DataContractName)]
    public sealed class Core2EnterpriseUser : Core2EnterpriseUserBase
    {
        private const string DataContractName = "Core2EnterpriseUser";

        public Core2EnterpriseUser()
            : base()
        {
        }
    }
}