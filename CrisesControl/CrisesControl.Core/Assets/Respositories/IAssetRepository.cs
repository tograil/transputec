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

        Task<bool> DeleteAsset(int assetId, int currentUserId, int companyId, string timeZoneId);
        Task<List<AssetsDetails>> GetAssets(int companyID, int recordStart = 0, int recordLength = 100, string searchString = "",
                                            string orderBy = "Name", string orderDir = "asc", int assetFilter = 0, int userID = 0);

        public Task<IEnumerable<Assets>> GetAllAssets(int companyId);

        public Task<Assets> GetAsset(int companyId, int assetId);

        public Task<int> UpdateAsset(Assets asset, CancellationToken cancellationToken);

        public Task<bool> CheckDuplicate(Assets asset);
        Task CreateAssetReviewReminder(int assetId, int companyID, DateTimeOffset nextReviewDate, string reviewFrequency, int reminderCount);
        Task<List<AssetLink>> GetAssetLink(int assetId);
        Task<int> DeleteAssetLink(int assetId);
        Task<bool> CheckForExistance(int assetId);
    }
}
