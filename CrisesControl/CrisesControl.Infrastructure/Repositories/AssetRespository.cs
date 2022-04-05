using CrisesControl.Core.AssetAggregate.Respositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assets = CrisesControl.Core.AssetAggregate.Assets;


namespace CrisesControl.Infrastructure.Repositories
{
    public class AssetRespository: IAssetRepository
    {
        private readonly CrisesControlContext _context;

        public AssetRespository(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsset(Assets asset, CancellationToken cancellationToken)
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

        public async Task<IEnumerable<Assets>> GetAllAssets(int companyId)
        {
            return await _context.Set<Assets>().Where(t => t.CompanyId == companyId).ToListAsync();
        }

        public async Task<Assets> GetAsset(int companyId, int assetId)
        {
            return await _context.Set<Assets>().Where(t => t.CompanyId == companyId && t.AssetId == assetId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsset(Assets asset, CancellationToken cancellationToken)
        {
            await _context.AddAsync(asset, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return asset.AssetId;
        }

        public bool CheckDuplicate(Assets asset)
        {
            return _context.Set<Assets>().Where(t=>t.AssetTitle.Equals(asset.AssetTitle)).Any();
        }
    }
}
