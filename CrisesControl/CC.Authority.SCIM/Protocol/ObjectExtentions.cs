//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CC.Authority.SCIM.Protocol
{
    public static class ObjectExtentions
    {
        public static bool IsResourceType(this object json, string scheme)
        {
            if (null == json)
            {
                throw new ArgumentNullException(nameof(json));
            }
            if (string.IsNullOrWhiteSpace(scheme))
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            dynamic operationDataJson = JsonConvert.DeserializeObject(json.ToString());
            bool result = false;

            switch (operationDataJson.schemas)
            {
                case JArray schemas:
                    string[] schemasList = schemas.ToObject<string[]>();
                    result =
                        schemasList
                        .Any(
                            (string item) =>
                                string.Equals(item, scheme, StringComparison.OrdinalIgnoreCase));
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
