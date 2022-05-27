using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports.SP_Response
{
    public class GetIncidentDataByActivationRefIncidentAssetsResponse
    {
        public int AssetId { get; set; }
        public int AssetTypeId { get; set; }
        public string AssetTitle { get; set; }
        public string AssetDescription { get; set; }
        public string FilePath { get; set; }
        public string AssetType { get; set; }
    }
}
