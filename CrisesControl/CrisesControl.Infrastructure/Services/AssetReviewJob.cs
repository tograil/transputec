using CrisesControl.Core.Assets;
using CrisesControl.Core.Assets.Respositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Core.DBCommon.Repositories;
using Microsoft.AspNetCore.Http;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class AssetReviewJob : IJob
    {
        private readonly CrisesControlContext _context;
        private readonly IDBCommonRepository DBC;
        private readonly ISenderEmailService _SDE;
        private readonly IAssetRepository _assetRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssetReviewJob(CrisesControlContext context, IAssetRepository assetRepository, IHttpContextAccessor httpContextAccessor, ISenderEmailService SDE, IDBCommonRepository _DBC)
        {
            _assetRepository = assetRepository;
            this._context = context;            
            this._httpContextAccessor = httpContextAccessor;
            this.DBC = _DBC;
            _SDE = SDE;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            int AssetId = context.JobDetail.JobDataMap.GetInt("AssetId");
            int Counter = context.JobDetail.JobDataMap.GetInt("Counter");

            try
            {
                var asset =  _context.Set<Assets>().Where(A=> A.AssetId == AssetId).FirstOrDefault();
                if (asset != null)
                {
                    if (asset.Status == 1)
                    {
                        
                       await  _SDE.SendAssetReviewAlert(AssetId, asset.CompanyId);

                        asset.ReminderCount = Counter;
                        _context.Update(asset);
                       await _context.SaveChangesAsync();

                      
                       await _assetRepository.CreateAssetReviewReminder(AssetId, asset.CompanyId, (DateTimeOffset)asset.ReviewDate, asset.ReviewFrequency, Counter);

                    }
                    else
                    {
                      await  DBC.DeleteScheduledJob("ASSET_REVIEW_" + AssetId, "REVIEW_REMINDER");
                    }
                }
                else
                {
                  await  DBC.DeleteScheduledJob("ASSET_REVIEW_" + AssetId, "REVIEW_REMINDER");
                }
                await Task.WhenAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
    }
}
