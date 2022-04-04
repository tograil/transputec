using CrisesControl.Core.AssetAggregate.Respositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asset = CrisesControl.Core.AssetAggregate.Asset;


namespace CrisesControl.Infrastructure.Repositories
{
    public class AssetRespository: IAssetRepository
    {
        private readonly CrisesControlContext _context;

        public AssetRespository(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsset(Asset asset, CancellationToken cancellationToken)
        {
            await _context.AddAsync(asset, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return asset.AssetId;
        }

        public async Task<int> DeleteAsset(int assetId, CancellationToken cancellationToken)
        {
            await _context.AddAsync(assetId, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return assetId;
        }

        public async Task<IEnumerable<Asset>> GetAllAssets(int companyId)
        {
            return await _context.Set<Asset>().Where(t => t.CompanyId == companyId).ToListAsync();
        }

        public async Task<Asset> GetAsset(int companyId, int assetId)
        {
            return await _context.Set<Asset>().Where(t => t.CompanyId == companyId && t.AssetId == assetId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsset(Asset asset, CancellationToken cancellationToken)
        {
            await _context.AddAsync(asset, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return asset.AssetId;
        }

        public bool CheckDuplicate(Asset asset)
        {
            return _context.Set<Asset>().Where(t=>t.AssetTitle.Equals(asset.AssetTitle)).Any();
        }
    }
}
