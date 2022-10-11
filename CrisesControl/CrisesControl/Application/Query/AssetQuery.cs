using AutoMapper;
using CrisesControl.Api.Application.Commands.Assets.CreateAsset;
using CrisesControl.Api.Application.Commands.Assets.DeleteAsset;
using CrisesControl.Api.Application.Commands.Assets.DeleteAssetLink;
using CrisesControl.Api.Application.Commands.Assets.GetAllAssets;
using CrisesControl.Api.Application.Commands.Assets.GetAsset;
using CrisesControl.Api.Application.Commands.Assets.GetAssetLink;
using CrisesControl.Api.Application.Commands.Assets.GetAssets;
using CrisesControl.Api.Application.Commands.Assets.UpdateAssets;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Assets;
using CrisesControl.Core.Assets.Respositories;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query {
    public class AssetQuery : IAssetQuery {
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;
        private readonly IPaging _paging;
        private readonly ICurrentUser _currentUser;
        private readonly IDBCommonRepository _DBC;

        public AssetQuery(IAssetRepository assetRepository, IMapper mapper, IPaging paging, ICurrentUser currentUser, IDBCommonRepository DBC) {
            _assetRepository = assetRepository;
            _mapper = mapper;
            _paging = paging;
            _currentUser = currentUser;
            _DBC = DBC;
        }

        public async Task<GetAllAssetsResponse> GetAllAssets(GetAllAssetsRequest request) {

            var response = new GetAllAssetsResponse();

            string OrderBy = string.IsNullOrEmpty(_paging.OrderBy) ? "AssetTitle" : _paging.OrderBy;
            int AssetFilter = request.AssetFilter != null ? request.AssetFilter : 0;

            int totalRecord = 0;
            DataTablePaging rtn = new DataTablePaging();
            rtn.Draw = _paging.Draw;

            var Assets = await _assetRepository.GetAssets(_currentUser.CompanyId, _paging.Start, _paging.Length, _paging.Search, OrderBy, _paging.Dir, AssetFilter, _currentUser.UserId);

            if (Assets != null) {
                totalRecord = Assets.Count;
                rtn.RecordsFiltered = Assets.Count;
                rtn.Data = Assets;
            }

            var TotalList = await _assetRepository.GetAssets(_currentUser.CompanyId, 0, int.MaxValue, "", "AssetId", "asc", AssetFilter, _currentUser.UserId);

            if (TotalList != null) {
                totalRecord = TotalList.Count;
            }

            rtn.RecordsTotal = totalRecord;
            var result = _mapper.Map<DataTablePaging>(rtn);
            if (result != null) {
                response.Data = rtn;
            } else {
                response.Data = null;
                response.Message = "No record found.";
            }
            return response;
        }

        public async Task<GetAssetResponse> GetAsset(GetAssetRequest request, CancellationToken cancellationToken) {
            var asset = await _assetRepository.GetAsset(_currentUser.CompanyId, request.AssetId);
            var result = _mapper.Map<AssetsDetails>(asset);
            var response = new GetAssetResponse();
            response.Data = result;
            return response;
        }

        public async Task<GetAssetLinkResponse> GetAssetLink(GetAssetLinkRequest request) {
            try {
                var assets = await _assetRepository.GetAssetLink(request.AssetId);
                var result = _mapper.Map<List<AssetLink>>(assets);
                var response = new GetAssetLinkResponse();
                if (result != null) {
                    response.Data = result;
                } else {
                    response.Data = null;
                }

                return response;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<DeleteAssetLinkResponse> DeleteAssetLink(DeleteAssetLinkRequest request) {
            try {
                var assets = await _assetRepository.DeleteAsset(request.AssetId, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<bool>(assets);
                var response = new DeleteAssetLinkResponse();
                if (result) {
                    response.DeleteAssetLink = result;
                    response.Message = "Asset link deleted";
                } else {
                    response.DeleteAssetLink = false;
                    response.Message = "Not deleted";
                }

                return response;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<GetAssetsResponse> GetAssets(GetAssetsRequest request, CancellationToken cancellationToken) {
            try {
                var assets = await _assetRepository.GetAllAssets(_currentUser.CompanyId);
                var result = _mapper.Map<List<Assets>>(assets.ToList());
                var response = new GetAssetsResponse();
                if (result != null) {
                    response.Data = result;
                } else {
                    response.Data = null;
                }

                return response;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<CreateAssetResponse> CreateAsset(CreateAssetRequest request, CancellationToken cancellationToken) {
            try {
                Assets asset = new Assets() {
                    AssetPath = request.AssetPath,
                    AssetTitle = request.AssetTitle,
                    AssetOwner = _currentUser.UserId,
                    AssetSize = request.AssetSize,
                    SourceFileName = request.SourceFileName,
                    ReminderCount = 0,
                    AssetType = request.AssetType,
                    ReviewDate = request.ReviewDate != null ? request.ReviewDate : DateTime.MinValue,
                    SourceObjectId = request.SourceObjectId,
                    CompanyId = _currentUser.CompanyId,
                    ReviewFrequency = request.ReviewFrequency,
                    FilePath = request.FilePath,
                    AssetTypeId = request.AssetTypeId,
                    CreatedBy = _currentUser.UserId,
                    CreatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
                    AssetDescription = request.AssetDescription,
                    Status = 1,
                    UpdatedBy = _currentUser.UserId,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
                };
                //var existeing = await _assetRepository.CheckDuplicate(asset);
                //if (!existeing) {
                var assetRtn = await _assetRepository.CreateAsset(asset, cancellationToken);

                if (asset.AssetOwner > 0 && asset.ReviewDate > DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone).Date && !string.IsNullOrEmpty(asset.ReviewFrequency)) {
                    await _assetRepository.CreateAssetReviewReminder(assetRtn, _currentUser.CompanyId, asset.ReviewDate, asset.ReviewFrequency, 0);
                } else {
                    await _DBC.DeleteScheduledJob("ASSET_REVIEW_" + assetRtn, "REVIEW_REMINDER");
                }

                var result = _mapper.Map<int>(assetRtn);
                var response = new CreateAssetResponse();
                if (result > 0) {
                    response.AssetId = result;
                    response.Message = "Added";

                } else {
                    response.AssetId = 0;
                    response.Message = "Not added";
                    response.ErrorId = 110;
                }
                return response;
                //}
                //return new CreateAssetResponse() {
                //    AssetId = asset.AssetId,
                //    Message = "Asset already exist"

                //};

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<UpdateAssetsResponse> UpdateAssets(UpdateAssetsRequest request, CancellationToken cancellationToken) {
            try {
                Assets asset = new Assets() {
                    AssetId = request.AssetId,
                    AssetPath = request.AssetPath,
                    AssetTitle = request.AssetTitle,
                    AssetSize = request.AssetSize,
                    SourceFileName = request.SourceFileName,
                    ReminderCount = 0,
                    AssetType = request.AssetType,
                    ReviewDate = request.ReviewDate != null ? request.ReviewDate : DateTime.MinValue,
                    SourceObjectId = request.SourceObjectId,
                    CompanyId = request.CompanyId,
                    ReviewFrequency = request.ReviewFrequency,
                    FilePath = request.FilePath,
                    AssetTypeId = request.AssetTypeId,
                    AssetDescription = request.AssetDescription,
                    Status = 1,
                    UpdatedBy = _currentUser.UserId,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone)
                };
                //if (!await _assetRepository.CheckDuplicate(asset)) {
                var assetRtn = await _assetRepository.UpdateAsset(asset, cancellationToken);

                if (asset.AssetOwner > 0 && asset.ReviewDate > DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone).Date && !string.IsNullOrEmpty(asset.ReviewFrequency)) {
                    await _assetRepository.CreateAssetReviewReminder(assetRtn, _currentUser.CompanyId, asset.ReviewDate, asset.ReviewFrequency, 0);
                } else {
                    await _DBC.DeleteScheduledJob("ASSET_REVIEW_" + assetRtn, "REVIEW_REMINDER");
                }

                var result = _mapper.Map<int>(asset);
                var response = new UpdateAssetsResponse();
                if (result > 0) {
                    response.AssetId = result;
                    response.Message = "Updated";

                } else {
                    response.AssetId = 0;
                    response.Message = "Not updated";
                    response.ErrorId = 110;

                }
                return response;
                //}
                //return new UpdateAssetsResponse() {
                //    AssetId = asset.AssetId,
                //    Message = "Asset title already exist"

                //};

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<DeleteAssetResponse> DeleteAsset(DeleteAssetRequest request, CancellationToken cancellationToken) {
            try {

                var asset = await _assetRepository.DeleteAsset(request.AssetId, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<bool>(asset);
                var response = new DeleteAssetResponse();
                if (result) {
                    response.Asset = result;
                    response.Message = "Deleted";

                } else {
                    response.Asset = false;
                    response.Message = "Not Deleted";

                }
                return response;


            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
