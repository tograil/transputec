//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Newtonsoft.Json;

namespace CC.Authority.SCIM.Schemas
{
    public class TrustedJsonFactory : JsonFactory
    {
        public override Dictionary<string, object> Create(string json)
        {
            Dictionary<string, object> result =
                (Dictionary<string, object>)JsonConvert.DeserializeObject(
                    json);
            return result;
        }

        public override string Create(string[] json)
        {
            string result = JsonConvert.SerializeObject(json);
            return result;
        }

        public override string Create(Dictionary<string, object> json)
        {
            string result = JsonConvert.SerializeObject(json);
            return result;
        }

        public override string Create(IDictionary<string, object> json)
        {
            string result = JsonConvert.SerializeObject(json);
            return result;
        }

        public override string Create(IReadOnlyDictionary<string, object> json)
        {
            string result = JsonConvert.SerializeObject(json);
            return result;
        }
    }
}
