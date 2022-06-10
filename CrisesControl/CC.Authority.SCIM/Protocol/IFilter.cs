//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Protocol
{
    public interface IFilter
    {
        IFilter AdditionalFilter { get; set; }
        string AttributePath { get; }
        string ComparisonValue { get; }
        string ComparisonValueEncoded { get; }
        AttributeDataType? DataType { get; set; }
        ComparisonOperator FilterOperator { get; }

        string Serialize();
    }
}