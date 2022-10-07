using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Assets;
using CrisesControl.Core.Assets.Respositories;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assets = CrisesControl.Core.Assets.Assets;


namespace CrisesControl.Infrastructure.Repositories {
    public class AssetRespository : IAssetRepository {
        private readonly CrisesControlContext _context;
        private readonly IMapper _mapper;
        private readonly DBCommon DBC;
        public AssetRespository(CrisesControlContext context, IMapper mapper, DBCommon _DBC) {
            _context = context;
            _mapper = mapper;
            DBC = _DBC;
        }

        public async Task<int> CreateAsset(Assets asset, CancellationToken cancellationToken) {
            await _context.AddAsync(asset, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return asset.AssetId;
        }

        public async Task<bool> DeleteAsset(int assetId, int currentUserId, int companyId, string timeZoneId) {
            var Assetsdata = await _context.Set<Assets>().Where(a => a.AssetId == assetId && a.CompanyId == companyId).FirstOrDefaultAsync();
            if (Assetsdata != null) {
                var result = await DeleteAssetLink(assetId);

                Assetsdata.Status = 3;
                Assetsdata.UpdatedBy = currentUserId;
                Assetsdata.UpdatedOn = DBC.GetLocalTime(timeZoneId, System.DateTime.Now);
                await _context.SaveChangesAsync();

                DBC.DeleteScheduledJob("ASSET_REVIEW_" + assetId, "REVIEW_REMINDER");

                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Assets>> GetAllAssets(int companyId) {
            return await _context.Set<Assets>().Where(t => t.CompanyId == companyId).ToListAsync();
        }

        public async Task<AssetsDetails> GetAsset(int companyId, int assetId) {
            var pCompanyId = new SqlParameter("@CompanyID", companyId);
            var pAssetId = new SqlParameter("@AssetID", assetId);

            var details = _context.Set<AssetsDetails>().FromSqlRaw("exec Pro_Assets_GetAssetDetails @CompanyID, @AssetID", pCompanyId, pAssetId).AsEnumerable();

            if (details != null) {
                var result = details.FirstOrDefault();
                result.AssetOwnerName = new UserFullName { Firstname = result.AssetOwnerFirstName, Lastname = result.AssetOwnerLastName };
                result.CreatedByName = new UserFullName { Firstname = result.CreatedByFirstName, Lastname = result.CreatedByLastName };
                result.UpdatedByName = new UserFullName { Firstname = result.UpdatedByFirstName, Lastname = result.UpdatedByLastName };
                return result;
            }
            return null;
        }

        public async Task<int> UpdateAsset(Assets asset, CancellationToken cancellationToken) {
            var result = _context.Set<Assets>().Where(t => t.AssetId == asset.AssetId).FirstOrDefault();

            if (result == null) {
                return default;
            } else {
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
                _context.Update(result);
                await _context.SaveChangesAsync(cancellationToken);
                return result.AssetId;
            }
        }

        public async Task<bool> CheckDuplicate(Assets asset) {
            return await _context.Set<Assets>().Where(t => t.AssetTitle.Equals(asset.AssetTitle)).AnyAsync();
        }

        public async Task<bool> CheckForExistance(int assetId) {
            return _context.Set<Assets>().Where(t => t.AssetId.Equals(assetId)).Any();
        }

        public async Task CreateAssetReviewReminder(int assetId, int companyID, DateTimeOffset nextReviewDate, string reviewFrequency, int reminderCount) {
            try {

                DBC.DeleteScheduledJob("ASSET_REVIEW_" + assetId, "REVIEW_REMINDER");

                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                string jobName = "ASSET_REVIEW_" + assetId;
                string taskTrigger = "ASSET_REVIEW_" + assetId;

                var jobDetail = new Quartz.Impl.JobDetailImpl(jobName, "REVIEW_REMINDER", typeof(AssetReviewJob));
                jobDetail.JobDataMap["AssetId"] = assetId;

                int Counter = 0;
                DateTimeOffset DateCheck = DBC.GetNextReviewDate(nextReviewDate, companyID, reminderCount, out Counter);
                jobDetail.JobDataMap["Counter"] = Counter;

                string TimeZoneVal = DBC.GetTimeZoneByCompany(companyID);

                if (DateTimeOffset.Compare(DateCheck, DBC.GetDateTimeOffset(DateTime.Now, TimeZoneVal)) >= 0) {

                    if (DateCheck < DateTime.Now)
                        DateCheck = DateTime.Now.AddMinutes(5);

                    ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                                                              .WithIdentity(taskTrigger, "REVIEW_REMINDER")
                                                              .StartAt(DateCheck)
                                                              .ForJob(jobDetail)
                                                              .Build();
                    await _scheduler.ScheduleJob(jobDetail, trigger);
                } else {
                    DateTimeOffset NewReviewDate = DBC.GetNextReviewDate(nextReviewDate, reviewFrequency);
                    var asset = _context.Set<Assets>().Where(A => A.AssetId == assetId).FirstOrDefault();
                    if (asset != null) {
                        asset.ReviewDate = NewReviewDate;
                        _context.Update(asset);
                        await _context.SaveChangesAsync();
                        await CreateAssetReviewReminder(assetId, companyID, NewReviewDate, reviewFrequency, reminderCount);
                    }
                }

            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<AssetLink>> GetAssetLink(int assetId) {
            try {
                var pAssetId = new SqlParameter("@AssetID", assetId);

                var result = await _context.Set<AssetLink>().FromSqlRaw("exec Pro_Get_Asset_Links @AssetID", pAssetId).ToListAsync();

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<int> DeleteAssetLink(int assetId) {
            try {
                var pAssetId = new SqlParameter("@AssetID", assetId);

                var result = await _context.Database.ExecuteSqlRawAsync("exec Pro_Delete_Asset_Links @AssetID", pAssetId);

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<AssetList>> GetAssets(int companyID, int recordStart = 0, int recordLength = 100, string searchString = "",
    string orderBy = "Name", string orderDir = "asc", int assetFilter = 0, int userID = 0) {
            try {

                var pCompanyID = new SqlParameter("@CompanyID", companyID);
                var pUserID = new SqlParameter("@UserID", userID);
                var pRecordStart = new SqlParameter("@RecordStart", recordStart);
                var pRecordLength = new SqlParameter("@RecordLength", recordLength);
                var pSearchString = new SqlParameter("@SearchString", searchString);
                var pOrderBy = new SqlParameter("@OrderBy", orderBy);
                var pOrderDir = new SqlParameter("@OrderDir", orderDir);
                var pAssetFilter = new SqlParameter("@AssetFilter", assetFilter);

                var result = await _context.Set<AssetList>().FromSqlRaw("exec Pro_Asset_SelectAll @CompanyID, @UserID, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @AssetFilter",
                    pCompanyID, pUserID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pAssetFilter).ToListAsync();
                result.Select(c => {
                    c.AssetOwnerName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                    return c;
                }).ToList();

                return result;

            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
