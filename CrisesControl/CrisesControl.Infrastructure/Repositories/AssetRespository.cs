using AutoMapper;
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
        private readonly IMapper _mapper;

        public AssetRespository(CrisesControlContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            var result = _context.Set<Assets>().Where(t => t.AssetId == asset.AssetId).FirstOrDefault();

            if (result == null)
            {
                return default;
            }
            else
            {
                result.AssetTitle = asset.AssetTitle;
                result.AssetDescription = asset.AssetDescription;
                result.AssetType = asset.AssetType;
                result.AssetTypeId = asset.AssetTypeId;
                result.AssetPath = asset.AssetPath;
                result.UpdatedBy = asset.UpdatedBy;
                result.AssetSize = asset.AssetSize;
                result.Status = asset.Status;
                result.SourceObjectId = asset.SourceObjectId;
                result.AssetOwner = asset.AssetOwner;
                result.ReminderCount = asset.ReminderCount;
                result.ReviewDate = asset.ReviewDate;
                result.ReviewFrequency = asset.ReviewFrequency;
                result.SourceFileName = asset.SourceFileName;
                await _context.SaveChangesAsync(cancellationToken);
                return result.AssetId;
            }
        }

        public bool CheckDuplicate(Assets asset)
        {
            return _context.Set<Assets>().Where(t=>t.AssetTitle.Equals(asset.AssetTitle)).Any();
        }

        public bool CheckForExistance(int assetId)
        {
            return _context.Set<Assets>().Where(t => t.AssetId.Equals(assetId)).Any();
        }
    }
}
