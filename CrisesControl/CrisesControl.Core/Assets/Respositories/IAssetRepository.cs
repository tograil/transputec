using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Assets.Respositories
{
    public interface IAssetRepository
    {
        public Task<int> CreateAsset(Assets asset, CancellationToken cancellationToken);

        public Task<int> DeleteAsset(int assetId, CancellationToken cancellationToken);

        public Task<IEnumerable<Assets>> GetAllAssets(int companyId);

        public Task<Assets> GetAsset(int companyId, int assetId);

        public Task<int> UpdateAsset(Assets asset, CancellationToken cancellationToken);

        public bool CheckDuplicate(Assets asset);
    }
}
