using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.AssetAggregate.Respositories
{
    public interface IAssetRepository
    {
        public Task<int> CreateAsset(Asset asset, CancellationToken cancellationToken);

        public Task<int> DeleteAsset(int assetId, CancellationToken cancellationToken);

        public Task<IEnumerable<Asset>> GetAllAssets(int companyId);

        public Task<Asset> GetAsset(int companyId, int assetId);

        public Task<int> UpdateAsset(Asset asset, CancellationToken cancellationToken);

        public bool CheckDuplicate(Asset asset);
    }
}
