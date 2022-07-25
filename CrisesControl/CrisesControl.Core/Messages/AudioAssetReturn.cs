using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class AudioAssetReturn
    {
        public int AssetId { get; set; }
        public string AssetTitle { get; set; }
        public string AssetPath { get; set; }
    }
}
