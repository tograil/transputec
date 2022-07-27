using AutoMapper;
using CrisesControl.Api.Application.Commands.Sop.AttachSOPToIncident;
using CrisesControl.Api.Application.Commands.Sop.DeleteSOP;
using CrisesControl.Api.Application.Commands.Sop.GetCompanySOP;
using CrisesControl.Api.Application.Commands.Sop.GetSopSection;
using CrisesControl.Api.Application.Commands.Sop.GetSOPSectionLibrary;
using CrisesControl.Api.Application.Commands.Sop.GetSopSections;
using CrisesControl.Api.Application.Commands.Sop.GetTagList;
using CrisesControl.Api.Application.Commands.Sop.LibraryTextModel;
using CrisesControl.Api.Application.Commands.Sop.RemoveSection;
using CrisesControl.Api.Application.Commands.Sop.ReorderSection;
using CrisesControl.Api.Application.Commands.Sop.SaveSOPHeader;
using CrisesControl.Api.Application.Commands.Sop.SaveSopSection;
using CrisesControl.Api.Application.Commands.Sop.UpdateSOPAsset;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Models;
using CrisesControl.Core.Sop;
using CrisesControl.Core.Sop.Respositories;


namespace CrisesControl.Api.Application.Query
{
    public class SopQuery : ISopQuery
    {
        private readonly ISopRepository _sopRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<SopQuery> _logger;
        //private readonly CrisesControlContext _context;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        
        public SopQuery(ISopRepository sopRepository, ICurrentUser currentUser, ILogger<SopQuery> logger, IMapper _mapper)
        {
            this._currentUser = currentUser;
            this._sopRepository = sopRepository;
            this._logger = logger;
           
        }

        public async Task<AttachSOPToIncidentResponse> AttachSOPToIncident(AttachSOPToIncidentRequest request)
        {
            try
            {
                var sop = await _sopRepository.AttachSOPToIncident(request.SOPHeaderID, request.SOPFileName, _currentUser.UserId,_currentUser.CompanyId,_currentUser.TimeZone);
                var result = _mapper.Map<bool>(sop);
                var response = new AttachSOPToIncidentResponse();
                if (result)
                {
                    response.result = result;
                   
                }
                else
                {
                    response.result = false;
                 
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeleteSOPResponse> DeleteSOP(DeleteSOPRequest request)
        {
            try
            {
                var sop = await _sopRepository.DeleteSOP(request.SOPHeaderID, _currentUser.UserId,_currentUser.CompanyId,_currentUser.TimeZone);
                var result = _mapper.Map<bool>(sop);
                var response = new DeleteSOPResponse();
                if (result)
                {
                    response.result = result;
                    response.Message = "Deleted ";
                }
                else
                {
                    response.result = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetCompanySOPResponse> GetCompanySOP(GetCompanySOPRequest request)
        {
            try
            {
                var sop = await _sopRepository.GetSOPSectionLibrary();
                var result = _mapper.Map<List<LibSopSection>>(sop);
                var response = new GetCompanySOPResponse();
                if (result != null)
                {
                    response.data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetSopSectionResponse> GetSopSection(GetSopSectionRequest request)
        {
            try
            {
                var sop = await _sopRepository.GetSOPSections(request.SOPHeaderID, _currentUser.CompanyId, request.ContentSectionID);
                var result = _mapper.Map<ContentSectionData>(sop); 
                var response = new GetSopSectionResponse();
                if (result != null)
                {
                    response.data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetSOPSectionLibraryResponse> GetSOPSectionLibrary(GetSOPSectionLibraryRequest request)
        {
            try
            {
                var sop = await _sopRepository.GetSOPSectionLibrary();
                var result = _mapper.Map<List<LibSopSection>>(sop);
                var response = new GetSOPSectionLibraryResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetSopSectionsResponse> GetSopSections(GetSopSectionsRequest request)
        {
            try
            {
                var sop = await _sopRepository.GetSOPSections(request.SOPHeaderID, _currentUser.CompanyId);
                var result = _mapper.Map<ContentSectionData>(sop);
                var response = new GetSopSectionsResponse();
                if (result != null)
                {
                    response.data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTagListResponse> GetTagList(GetTagListRequest request)
        {
            try
            {
                var sop = await _sopRepository.GetContentTags();
                var result = _mapper.Map<List<ContentTags>>(sop);
                var response = new GetTagListResponse();
                if (result != null)
                {
                    response.data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LibraryTextModelResponse> LibraryTextModel(LibraryTextModelRequest request)
        {
            try
            {
                var libraryTexts = await _sopRepository.GetLibraryText(request.IncidentName, request.IncidentType, request.Sector, request.SectionTitle);
                var result = _mapper.Map<List<LibraryText>>(libraryTexts);
                var response = new LibraryTextModelResponse();
                if (result != null)
                {

                    response.data = result;
                }
                else
                {

                    response.data = result;
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RemoveSectionResponse> RemoveSection(RemoveSectionRequest request)
        {
            try
            {
               
                var response = new RemoveSectionResponse();
                if (request.ContentSectionID > 0) 
                { 
                await _sopRepository.DeleteSections(request.ContentSectionID);
                response.data = true;
                response.Message = "Deleted";
                }
                else
                {
                    response.data = false;
                    response.Message = "No data found";
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ReorderSectionResponse> ReorderSection(ReorderSectionRequest request)
        {
            try
            {
                var sop = await _sopRepository.ReorderSection(request.SectionOrder);
                var result = _mapper.Map<int>(sop);
                var response = new ReorderSectionResponse();
                if (result > 0)
                {
                   
                    response.Message = "Reordered";
                }
                else
                {
                   
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SaveSOPHeaderResponse> SaveSOPHeader(SaveSOPHeaderRequest request)
        {
            try
            {
                var sop = await _sopRepository.AU_SOPHeader(request.SOPHeaderID, request.IncidentID, request.SOPVersion, request.SOPOwner, request.ContentText, request.ReviewDate, request.ReviewFrequency, request.ContentID, request.ContentSectionID,_currentUser.UserId,_currentUser.CompanyId);
                var result = _mapper.Map<int>(sop);
                var response = new SaveSOPHeaderResponse();
                if (result > 0)
                {
                    response.SopHeaderId = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.SopHeaderId = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SaveSopSectionResponse> SaveSopSection(SaveSopSectionRequest request)
        {
            try
            {
                var section_id = await _sopRepository.AU_Section(request.SOPHeaderID, request.ContentID, request.ContentSectionID, request.SectionType, request.SectionName, request.SectionDescription, request.SectionStatus, request.SectionOrder, request.SOPGroups, request.SOPContentTags, _currentUser.UserId);
                var section = await _sopRepository.GetSOPSections(request.SOPHeaderID,_currentUser.CompanyId, section_id);
                var result = _mapper.Map<ContentSectionData>(section);
                var response = new SaveSopSectionResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateSOPAssetResponse> UpdateSOPAsset(UpdateSOPAssetRequest request)
        {
            try
            {
                var sopAssest= await _sopRepository.UpdateSOPAsset(request.SOPHeaderID, request.AssetID, _currentUser.UserId,_currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<bool>(sopAssest);
                var response = new UpdateSOPAssetResponse();
                if (result)
                {
                    
                    response.result = true;
                    response.Message = "Updated";
                }
                else
                {
                    response.result = false;
                    response.Message = "No data found";
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
