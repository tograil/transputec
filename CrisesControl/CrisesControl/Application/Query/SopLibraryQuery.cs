using AutoMapper;
using CrisesControl.Api.Application.Commands.SopLibrary.DeleteSOPLib;
using CrisesControl.Api.Application.Commands.SopLibrary.GetSopSection;
using CrisesControl.Api.Application.Commands.SopLibrary.GetSopSections;
using CrisesControl.Api.Application.Commands.SopLibrary.SaveLibSection;
using CrisesControl.Api.Application.Commands.SopLibrary.UseLibraryText;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.SopLibrary;
using CrisesControl.Core.SopLibrary.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class SopLibraryQuery : ISopLibraryQuery
    {
        private readonly ISopLibraryRepository _sopLibraryRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<SopLibraryQuery> _logger;
        public SopLibraryQuery(ISopLibraryRepository sopLibraryRepository, ICurrentUser currentUser, IMapper mapper, ILogger<SopLibraryQuery> logger)
        {
            this._sopLibraryRepository = sopLibraryRepository;
            this._currentUser = currentUser;
            this._mapper = mapper;
            this._logger = logger;
            
        }
        public async Task<DeleteSOPLibResponse> DeleteSOPLib(DeleteSOPLibRequest request)
        {
            try
            {
                var delete = await _sopLibraryRepository.DeleteSOPLib(request.SOPHeaderID);
                var result = _mapper.Map<bool>(delete);
                var response = new DeleteSOPLibResponse();
                if (result)
                {
                    response.Message = "Deleted";
                }
                else
                {
                    response.Message = "No Data Found";
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
                
                var sopSection = await _sopLibraryRepository.GetSOPLibrarySection(request.SOPHeaderID, _currentUser.CompanyId);
                var result = _mapper.Map<SopSection>(sopSection);
                var response = new GetSopSectionResponse();
                if (request.SOPHeaderID>0) { 
                if (result!= null)
                {
                    response.SopSection = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.SopSection = result;
                    response.Message = "No Data Found";
                }
                return response;
                }
                return null;
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
                var sopSections = await _sopLibraryRepository.GetSOPLibrarySections( _currentUser.CompanyId);
                var result = _mapper.Map<List<SopSectionList>>(sopSections);
                var response = new GetSopSectionsResponse();
                
                    if (result != null)
                    {
                        response.SopSection = result;
                        response.Message = "Data loaded Successfully";
                    }
                    else
                    {
                        response.SopSection = result;
                        response.Message = "No Data Found";
                    }
                    return response;
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SaveLibSectionResponse> SaveLibSection(SaveLibSectionRequest request)
        {
            try
            {
                var SaveLib = await _sopLibraryRepository.AU_Section(request.SOPHeaderID, request.IncidentID, request.SOPVersion, request.ReviewDate, request.ContentID, request.ContentSectionID,
                        request.SectionName, request.SectionDescription, request.SectionStatus, request.SOPContentTags,_currentUser.UserId,_currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<int>(SaveLib);
                var response = new SaveLibSectionResponse();

                if (result > 0)
                {
                    response.result = result;
                    
                }
                else
                {
                    response.result = result;
                    
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UseLibraryTextResponse> UseLibraryText(UseLibraryTextRequest request)
        {
            try
            {
                var SaveLib = await _sopLibraryRepository.RecordLibraryUsage(request.SOPHeaderID);
                var result = _mapper.Map<SopSection>(SaveLib);
                var response = new UseLibraryTextResponse();

                if (result != null)
                {
                    response.SopSection = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.SopSection = result;
                    response.Message = "No Data Found";
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
