using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Assets.Respositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assets = CrisesControl.Core.Assets.Assets;


namespace CrisesControl.Infrastructure.Repositories
{
    public class AssetRespository: IAssetRepository
    {
        private readonly CrisesControlContext _context;
        private readonly IMapper _mapper;
        private readonly DBCommon DBC;
        public AssetRespository(CrisesControlContext context, IMapper mapper, DBCommon _DBC)
        {
            _context = context;
            _mapper = mapper;
            DBC = _DBC;
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

        public void CreateAssetReviewReminder(int AssetId, int CompanyID, DateTimeOffset NextReviewDate, string ReviewFrequency, int ReminderCount)
        {
            try
            {

                DBC.DeleteScheduledJob("ASSET_REVIEW_" + AssetId, "REVIEW_REMINDER");

                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                string jobName = "ASSET_REVIEW_" + AssetId;
                string taskTrigger = "ASSET_REVIEW_" + AssetId;

                var jobDetail = new Quartz.Impl.JobDetailImpl(jobName, "REVIEW_REMINDER", typeof(AssetReviewJob));
                jobDetail.JobDataMap["AssetId"] = AssetId;

                int Counter = 0;
                DateTimeOffset DateCheck = DBC.GetNextReviewDate(NextReviewDate, CompanyID, ReminderCount, out Counter);
                jobDetail.JobDataMap["Counter"] = Counter;

                string TimeZoneVal = DBC.GetTimeZoneByCompany(CompanyID);

                if (DateTimeOffset.Compare(DateCheck, DBC.GetDateTimeOffset(DateTime.Now, TimeZoneVal)) >= 0)
                {

                    if (DateCheck < DateTime.Now)
                        DateCheck = DateTime.Now.AddMinutes(5);

                    ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                                                              .WithIdentity(taskTrigger, "REVIEW_REMINDER")
                                                              .StartAt(DateCheck)
                                                              .ForJob(jobDetail)
                                                              .Build();
                    _scheduler.ScheduleJob(jobDetail, trigger);
                }
                else
                {
                    DateTimeOffset NewReviewDate = DBC.GetNextReviewDate(NextReviewDate, ReviewFrequency);
                    var asset = _context.Set<Assets>().Where(A=> A.AssetId == AssetId).FirstOrDefault();
                    if (asset != null)
                    {
                        asset.ReviewDate = NewReviewDate;
                        _context.SaveChangesAsync();
                        CreateAssetReviewReminder(AssetId, CompanyID, NewReviewDate, ReviewFrequency, ReminderCount);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
