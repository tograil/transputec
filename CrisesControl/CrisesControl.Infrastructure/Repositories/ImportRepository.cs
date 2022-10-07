using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Common;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Groups.Repositories;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Locations.Services;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class ImportRepository : IImportRepository
    {
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ImportRepository> _logger;
        private readonly DBCommon _DBC;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        private int userId;
        private int companyId;

        string NewCheck = "NewAdded";
        string OverrideCheck = "Updated";
        string SkipCheck = "Skiped";
        string DeletedCheck = "Deleted";
        string TimeZoneId = "GMT Standard Time";


        public ImportRepository(CrisesControlContext context, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ImportRepository> logger, 
            DBCommon DBC, 
            IDepartmentRepository departmentRepository, 
            IMapper mapper, 
            IGroupRepository groupRepository,
            ILocationRepository locationRepository)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _DBC = DBC;
            _mapper = mapper;
            _departmentRepository = departmentRepository;
            _groupRepository = groupRepository;
            _locationRepository = locationRepository;

            userId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
            companyId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

        }

        public async Task<dynamic> DepartmentOnlyImport(GroupOnlyImportModel groupOnlyImportModel, CancellationToken cancellationToken)
        {
            var ResultDTO = new CommonDTO();
            try
            {
                bool dep_created = await createDepartmentData(groupOnlyImportModel.SessionId, companyId, userId);

                var rec = (from UIT in _context.Set<ImportDump>()
                           where UIT.CompanyId == companyId && UIT.SessionId == groupOnlyImportModel.SessionId
                           select new
                           {
                               UserImportTotalId = UIT.ImportDumpId,
                               UserId = UIT.UserId,
                               CompanyId = UIT.CompanyId,
                               SessionId = UIT.SessionId,
                               DepartmentName = UIT.Department,
                               DepartmentStatus = UIT.DepartmentStatus,
                               DepartmentCheck = UIT.DepartmentCheck,
                               ImportAction = UIT.ImportAction,
                               ActionType = UIT.ActionType,
                               Action = UIT.Action,
                               UIT.ActionCheck,
                               UIT.ValidationMessage
                           }).ToList();
                if (rec.Count > 0)
                {
                    return rec;
                }
                else
                {
                    ResultDTO.ErrorId = 135;
                    ResultDTO.Message = "No department record ready for import.";
                }
                return ResultDTO;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<CommonDTO> DepartmentOnlyUpload(GroupOnlyUploadModel groupOnlyUploadModel, CancellationToken cancellationToken)
        {
            var result = new CommonDTO();
            try
            {
                string UpdateActionMessage = string.Empty;
                var uploadData = await _context.Set<ImportDump>().Where(t => t.ImportDumpId == groupOnlyUploadModel.UserImportTotalId).FirstOrDefaultAsync();
                if (uploadData != null)
                {
                    if (uploadData.ActionCheck != "ERROR")
                    {
                        int NewAddedId = 0;
                        int Status = ImportService.GetStatusValue(uploadData.DepartmentStatus, "DEPARTMENT");

                        if (uploadData.DepartmentCheck.ToUpper() == "NEW" &&
                            (uploadData.Action.ToUpper() == "ADD" || uploadData.Action.ToUpper() == "UPDATE"))
                        {
                            Department newDepartment = new Department();
                            newDepartment.CompanyId = uploadData.CompanyId;
                            newDepartment.DepartmentName = uploadData.Department;
                            newDepartment.Status = Status;
                            newDepartment.CreatedBy = userId;
                            NewAddedId = await _departmentRepository.CreateDepartment(newDepartment, cancellationToken);
                            uploadData.ActionCheck = NewCheck;

                            UpdateActionMessage += "New department created" + Environment.NewLine;

                        }
                        else if (uploadData.DepartmentCheck.ToUpper() == "DUPLICATE" || uploadData.DepartmentCheck.ToUpper() == "OK")
                        { //Existing department update

                            if (uploadData.Action?.ToUpper() == "DELETE")
                            {
                                var chkdept = await _departmentRepository.DeleteDepartment((int)uploadData.DepartmentId!,cancellationToken);
                                if (_DBC.IsPropertyExist(chkdept, "ErrorId"))
                                {
                                    UpdateActionMessage += "Department associated with incident task and cannot be deleted." + Environment.NewLine;
                                    uploadData.ActionCheck = SkipCheck;
                                }
                                else
                                {
                                    UpdateActionMessage += "Department is deleted" + Environment.NewLine;
                                    uploadData.ActionCheck = DeletedCheck;
                                }

                            }
                            else if (uploadData.Action?.ToUpper() == "UPDATE" || uploadData.Action?.ToUpper() == "ADD")
                            {  //update an existing department

                                var depatUpdate = await _context.Set<Department>().Where(t => t.DepartmentId == uploadData.DepartmentId).FirstOrDefaultAsync(); ;
                                if (depatUpdate != null)
                                {
                                    depatUpdate.Status = Status;
                                    depatUpdate.UpdatedBy = userId;
                                    depatUpdate.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                                    await _context.SaveChangesAsync(cancellationToken);
                                    NewAddedId = depatUpdate.DepartmentId;
                                    uploadData.ActionCheck = OverrideCheck;
                                    UpdateActionMessage += "Department details updated" + Environment.NewLine;
                                }

                            }
                            else if (uploadData.ImportAction?.ToUpper() == "SKIP")
                            {
                                uploadData.ActionCheck = SkipCheck;
                                UpdateActionMessage += "Department update skipped" + Environment.NewLine;
                            }

                        }

                        uploadData.ValidationMessage = UpdateActionMessage;

                        uploadData.ImportAction = "Imported";
                        await _context.SaveChangesAsync();
                        result.ErrorId = 0;
                        result.Message = UpdateActionMessage;

                    }
                    else
                    {
                        result.ErrorId = 100;
                        result.Message = uploadData.ValidationMessage;
                    }
                }
                else
                {
                    result.ErrorId = 100;
                    result.Message = "Record not found to import";
                }
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorId = 100;
                result.Message = ex.Message.ToString();
                return result;
            }
        }

        public List<ImportDumpResult> DownloadImportResult(int companyId, int sessionId, CancellationToken cancellationToken)
        {
            try
            {
                var pSessionId = new SqlParameter("@SessionId", sessionId);
                var pCompanyId = new SqlParameter("@CompanyId", companyId);
                //var pUserID = new SqlParameter("@CreatedBy", UserID);

                _context.Database.SetCommandTimeout(1 * 60 * 60);
                var result = _context.Set<ImportDump>().FromSqlRaw("EXEC Pro_ImportUser_GetImportResult @SessionId, @CompanyId", pSessionId, pCompanyId).ToList();
                var response = _mapper.Map<List<ImportDumpResult>>(result);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ImportDumpResult>> GetImportResult(ProcessImport processImport, CancellationToken cancellationToken)
        {
            try
            {
                var pSessionId = new SqlParameter("@SessionId", processImport.SessionId);
                var pCompanyId = new SqlParameter("@CompanyId", companyId);

                _context.Database.SetCommandTimeout (1 * 60 * 60);
                var result = await _context.Set<ImportDump>().FromSqlRaw("EXEC Pro_ImportUser_GetImportResult @SessionId, @CompanyId", pSessionId, pCompanyId).ToListAsync();
                var response = _mapper.Map<List<ImportDumpResult>>(result);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ImportDumpResult>> GetValidationResult(ProcessImport processImport, CancellationToken cancellationToken)
        {
            try
            {

                var pSessionId = new SqlParameter("@SessionId", processImport.SessionId);
                var pCompanyId = new SqlParameter("@CompanyId", companyId);

                _context.Database.SetCommandTimeout(1 * 60 * 60);
                var result = await _context.Set<ImportDump>().FromSqlRaw("EXEC Pro_ImportUser_GetValidationResult @SessionId, @CompanyId", pSessionId, pCompanyId).ToListAsync();
                var response = _mapper.Map<List<ImportDumpResult>>(result);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> GroupOnlyImport(GroupOnlyImportModel groupOnlyImportModel, CancellationToken cancellationToken)
        {
            var ResultDTO = new CommonDTO();
            try
            {
                bool dep_created = await createGroupData(groupOnlyImportModel.SessionId, companyId, userId);

                var DeptOnlyrec = await (from UIT in _context.Set<ImportDump>()
                                   where UIT.CompanyId == companyId && UIT.SessionId == groupOnlyImportModel.SessionId
                                   select new GroupOnlyImportResult()
                                   {
                                       UserImportTotalId = UIT.ImportDumpId,
                                       UserId = UIT.UserId,
                                       CompanyId = UIT.CompanyId,
                                       SessionId = UIT.SessionId,
                                       GroupName = UIT.Group,
                                       GroupStatus = UIT.GroupStatus,
                                       GroupCheck = UIT.GroupCheck,
                                       ImportAction = UIT.ImportAction,
                                       ActionType = UIT.ActionType,
                                       Action = UIT.Action,
                                       ActionCheck = UIT.ActionCheck,
                                       ValidationMessage = UIT.ValidationMessage
                                   }).ToListAsync();
                if (DeptOnlyrec.Count > 0)
                {
                    return DeptOnlyrec;
                }
                else
                {
                    ResultDTO.ErrorId = 135;
                    ResultDTO.Message = "No Group record ready for import.";
                }
                return ResultDTO;
            }
            catch (Exception ex)
            {
                return ResultDTO;
            }
        }

        public async Task<bool> createGroupData(string sessionId, int companyId, int userId)
        {

            try
            {
                var impDepData = await _context.Set<ImportDump>().Where(t => t.SessionId == sessionId).ToListAsync();
                if (impDepData.Count > 0)
                {

                    foreach (var item in impDepData)
                    {
                        string GroupRec = string.Empty;
                        string ValidationMessage = string.Empty;
                        int GroupId;
                        int HasError = 0;
                        bool DataError = false;
                        bool MandatoryError = false;

                        //validateion the group all.

                        List<UserImportDataValidation> UIDV = new List<UserImportDataValidation>();

                        string deptformat = ImportService.ExtendedNames("Group", item.Group, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Group", MandatoryError = MandatoryError, DataError = DataError, Message = deptformat });
                        string dept = ImportService.ValidateLength("Group", item.Group, 3, 100, out MandatoryError, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Group", MandatoryError = MandatoryError, DataError = DataError, Message = dept });

                        string action = ImportService.ValidateAction("Action", item.Action, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Action", MandatoryError = false, DataError = DataError, Message = action });

                        string status = ImportService.ValidateStatus("Status", item.GroupStatus, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Status", MandatoryError = false, DataError = DataError, Message = status });


                        string MultiGroup = string.Empty;
                        if (MultiOptionCheck(item.Group))
                        {
                            HasError++;
                            MultiGroup = "Multiple groups found, seprate them per line." + Environment.NewLine;
                        }

                        GroupRec = ImportCheckGroupExist(companyId, item.Group, sessionId, out GroupId);

                        string UpdateActionMessage = string.Empty;

                        int totalMandatoryErrors = 0;
                        int totalDataErrors = 0;

                        foreach (UserImportDataValidation valErrors in UIDV)
                        {
                            if (valErrors.MandatoryError == true || valErrors.DataError == true)
                            {
                                ValidationMessage = ValidationMessage + valErrors.Message;
                                if (valErrors.MandatoryError)
                                    totalMandatoryErrors++;

                                if (valErrors.DataError)
                                    totalDataErrors++;

                                HasError++;
                            }
                        }

                        if (GroupRec == "NEW" && item.Action == "DELETE")
                        {
                            ValidationMessage = "Group do not exist in database. Cannot delete." + Environment.NewLine;
                            HasError++;
                        }

                        if (totalMandatoryErrors > 0 && totalDataErrors == 0 && GroupRec == "DUPLICATE" && item.Action == "DELETE")
                        {
                            HasError = -100;
                        }


                        if (HasError > 0)
                        {
                            item.ValidationMessage = ValidationMessage;
                            item.ActionCheck = "ERROR";
                            item.ImportAction = "ERROR";
                        }
                        else
                        {
                            if (GroupRec == "DUPLICATE")
                            {
                                if (HasError != -100)
                                {
                                    UpdateActionMessage += "Group details will be updated." + Environment.NewLine;
                                }
                                else
                                {
                                    if (item.Action.ToUpper() == "DELETE")
                                    {
                                        UpdateActionMessage += "Group will be deleted." + Environment.NewLine;
                                    }
                                }
                            }
                            else
                            {
                                if (item.Action.ToUpper() == "DELETE")
                                {
                                    UpdateActionMessage += "Group does not exist in database. Cannot delete." + Environment.NewLine;
                                }
                                else
                                {
                                    UpdateActionMessage += "New group will be created." + Environment.NewLine;
                                }
                            }
                            item.ValidationMessage = UpdateActionMessage;
                        }

                        item.GroupId = GroupId;
                        item.GroupCheck = GroupRec;
                        await _context.SaveChangesAsync();

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string ImportCheckGroupExist(int companyId, string groupName, string sessionId, out int groupId)
        {
            groupId = 0;
            if (!string.IsNullOrEmpty(groupName))
            {
                var isGroupExist = _context.Set<Group>().Where(t => t.GroupName == groupName && t.CompanyId == companyId).FirstOrDefault();

                if (isGroupExist != null)
                {
                    groupId = isGroupExist.GroupId;
                    return "DUPLICATE";
                }
                else
                {
                    return "NEW";
                }
            }
            else
            {
                return "EMPTY";
            }
        }

        public async Task<CommonDTO> GroupOnlyUpload(GroupOnlyUploadModel groupOnlyUploadModel, CancellationToken cancellationToken)
        {
            CommonDTO result = new CommonDTO();
            try
            {
                string UpdateActionMessage = string.Empty;
                var uploadData = await _context.Set<ImportDump>().Where(t => t.ImportDumpId == groupOnlyUploadModel.UserImportTotalId).FirstOrDefaultAsync();
                if (uploadData != null)
                {
                    if (uploadData.ActionCheck != "ERROR")
                    {
                        int NewAddedGroupId = 0;
                        int Status = ImportService.GetStatusValue(uploadData.GroupStatus, "GROUP");

                        if (uploadData.GroupCheck.ToUpper() == "NEW" &&
                           (uploadData.Action.ToUpper() == "ADD" || uploadData.Action.ToUpper() == "UPDATE"))
                        {
                            Core.Groups.Group newGroup = new Core.Groups.Group();
                            newGroup.CompanyId = uploadData.CompanyId;
                            newGroup.GroupName = uploadData.Group;
                            newGroup.Status = Status;
                            newGroup.CreatedBy = userId;
                            NewAddedGroupId = await _groupRepository.CreateGroup(newGroup, cancellationToken);
                            uploadData.ActionCheck = NewCheck;

                            UpdateActionMessage += "New group created" + Environment.NewLine;

                        }
                        else if (uploadData.GroupCheck.ToUpper() == "DUPLICATE" || uploadData.GroupCheck.ToUpper() == "OK")
                        { //Existing group update

                            if (uploadData.Action.ToUpper() == "DELETE")
                            {
                                var chkdept = await _groupRepository.DeleteGroup((int)uploadData.GroupId, cancellationToken);
                                if (_DBC.IsPropertyExist(chkdept, "ErrorId"))
                                {
                                    UpdateActionMessage += "Group associated with incident task and cannot be deleted." + Environment.NewLine;
                                    uploadData.ActionCheck = SkipCheck;
                                }
                                else
                                {
                                    UpdateActionMessage += "Group is deleted" + Environment.NewLine;
                                    uploadData.ActionCheck = DeletedCheck;
                                }

                            }
                            else if (uploadData.Action.ToUpper() == "UPDATE" || uploadData.Action.ToUpper() == "ADD")
                            {  //update an existing group

                                var depatUpdate = await _context.Set<Group>().Where(t=>t.GroupId == uploadData.GroupId).FirstOrDefaultAsync();
                                if (depatUpdate != null)
                                {
                                    depatUpdate.Status = Status;
                                    depatUpdate.UpdatedBy = userId;
                                    depatUpdate.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                                    await _context.SaveChangesAsync(cancellationToken);
                                    NewAddedGroupId = depatUpdate.GroupId;
                                    uploadData.ActionCheck = OverrideCheck;
                                    UpdateActionMessage += "Group details updated" + Environment.NewLine;
                                }

                            }
                            else if (uploadData.ImportAction.ToUpper() == "SKIP")
                            {
                                uploadData.ActionCheck = SkipCheck;
                                UpdateActionMessage += "Group update skipped" + Environment.NewLine;
                            }

                        }

                        uploadData.ValidationMessage = UpdateActionMessage;

                        uploadData.ImportAction = "Imported";
                        await _context.SaveChangesAsync();
                        result.ErrorId = 0;
                        result.Message = UpdateActionMessage;

                    }
                    else
                    {
                        result.ErrorId = 100;
                        result.Message = uploadData.ValidationMessage;
                    }
                }
                else
                {
                    result.ErrorId = 100;
                    result.Message = "Record not found to import";
                }
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorId = 100;
                result.Message = ex.Message.ToString();
                return result;
            }
        }

        public async Task<dynamic> LocationOnlyImport(LocationOnlyImportModel locationOnlyImportModel, CancellationToken cancellationToken)
        {
            CommonDTO ResultDTO = new CommonDTO();
            try
            {
                await createLocationData(locationOnlyImportModel.SessionId, companyId, userId);

                var LocationOnlyrec = (from UIT in _context.Set<ImportDump>()
                                       where UIT.CompanyId == companyId && UIT.SessionId == locationOnlyImportModel.SessionId
                                       select new LocationOnlyImportResult()
                                       {
                                           UserImportTotalId = UIT.ImportDumpId,
                                           UserId = UIT.UserId,
                                           CompanyId = UIT.CompanyId,
                                           SessionId = UIT.SessionId,
                                           LocationName = UIT.Location,
                                           LocationAddress = UIT.LocationAddress,
                                           LocationStatus = UIT.LocationStatus,
                                           LocationCheck = UIT.LocationCheck,
                                           ImportAction = UIT.ImportAction,
                                           ActionType = UIT.ActionType,
                                           Action = UIT.Action,
                                           ActionCheck = UIT.ActionCheck,
                                           ValidationMessage = UIT.ValidationMessage

                                       }).ToList();
                if (LocationOnlyrec.Count > 0)
                {
                    return LocationOnlyrec;
                }
                else
                {
                    ResultDTO.ErrorId = 135;
                    ResultDTO.Message = "No Location only record imported.";
                }
                return ResultDTO;
            }
            catch (Exception ex)
            {
                return ResultDTO;
            }
        }

        public async Task<bool> createLocationData(string sessionId, int companyId, int currentUserId)
        {
            try
            {
                var impLocData = await _context.Set<ImportDump>().Where(t => t.SessionId == sessionId).ToListAsync();

                if (impLocData.Count > 0)
                {
                    foreach (var item in impLocData)
                    {
                        string locationName = item.Location;
                        string locationRec = string.Empty;
                        string validationMessage = string.Empty;
                        int locationId;
                        int hasError = 0;
                        bool dataError = false;
                        bool mandatoryError = false;

                        //validateion the location all.
                        List<UserImportDataValidation> UIDV = new List<UserImportDataValidation>();

                        string locnameformat = ImportService.ExtendedNames("Location", item.Location, out dataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Location", MandatoryError = mandatoryError, DataError = dataError, Message = locnameformat });
                        string locname = ImportService.ValidateLength("Location", item.Location, 3, 150, out mandatoryError, out dataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Location", MandatoryError = mandatoryError, DataError = dataError, Message = locname });

                        string locaddformat = ImportService.ExtendedNames("Location Address", item.LocationAddress, out dataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Location Address", MandatoryError = mandatoryError, DataError = dataError, Message = locaddformat });
                        string locadd = ImportService.ValidateLength("Location Address", item.LocationAddress, 3, 250, out mandatoryError, out dataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Location Address", MandatoryError = mandatoryError, DataError = dataError, Message = locadd });

                        string action = ImportService.ValidateAction("Action", item.Action, out dataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Action", MandatoryError = false, DataError = dataError, Message = action });

                        string status = ImportService.ValidateStatus("Status", item.LocationStatus, out dataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Status", MandatoryError = false, DataError = dataError, Message = status });

                        string multiLocation = string.Empty;
                        if (MultiOptionCheck(item.Location))
                        {
                            hasError++;
                            multiLocation = "Multiple locations found, seprate them per line" + Environment.NewLine;
                        }

                        locationRec = ImportCheckLocationExist(companyId, locationName, sessionId, out locationId);

                        string UpdateActionMessage = string.Empty;

                        int totalMandatoryErrors = 0;
                        int totalDataErrors = 0;

                        foreach (UserImportDataValidation valErrors in UIDV)
                        {
                            if (valErrors.MandatoryError == true || valErrors.DataError == true)
                            {
                                validationMessage = validationMessage + valErrors.Message;
                                if (valErrors.MandatoryError)
                                    totalMandatoryErrors++;

                                if (valErrors.DataError)
                                    totalDataErrors++;

                                hasError++;
                            }
                        }

                        if (locationRec == "NEW" && item.Action == "DELETE")
                        {
                            validationMessage = "Location does not exist in database. Cannot delete." + Environment.NewLine;
                            hasError++;
                        }

                        if (totalMandatoryErrors > 0 && totalDataErrors == 0 && locationRec == "DUPLICATE" && item.Action == "DELETE")
                        {
                            hasError = -100;
                        }

                        if (hasError > 0)
                        {
                            item.ValidationMessage = validationMessage;
                            item.ActionCheck = "ERROR";
                            item.ImportAction = "ERROR";
                        }
                        else
                        {
                            if (locationRec == "DUPLICATE")
                            {
                                if (hasError != -100)
                                {
                                    UpdateActionMessage += "Location details will be updated." + Environment.NewLine;
                                }
                                else
                                {
                                    if (item.Action.ToUpper() == "DELETE")
                                    {
                                        UpdateActionMessage += "Location will be deleted." + Environment.NewLine;
                                    }
                                }
                            }
                            else
                            {
                                if (item.Action.ToUpper() == "DELETE")
                                {
                                    UpdateActionMessage += "Location does not exist in database. Cannot delete." + Environment.NewLine;
                                }
                                else
                                {
                                    UpdateActionMessage += "New location will be created." + Environment.NewLine;
                                }
                            }
                            item.ValidationMessage = UpdateActionMessage;
                        }

                        item.LocationId = locationId;
                        item.LocationCheck = locationRec;
                        await _context.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string ImportCheckLocationExist(int companyId, string locationName, string sessionId, out int locationId)
        {
            locationId = 0;
            if (!string.IsNullOrEmpty(locationName))
            {
                var isLocationExist = _context.Set<Core.Locations.Location>().Where(t => t.LocationName == locationName && t.CompanyId == companyId).FirstOrDefault();

                if (isLocationExist != null)
                {
                    locationId = isLocationExist.LocationId;
                    return "DUPLICATE";
                }
                else
                {
                    return "NEW";
                }
            }
            else
            {
                return "EMPTY";
            }
        }

        public async Task<CommonDTO> LocationOnlyUpload(LocationOnlyUploadModel locationOnlyUploadModel, CancellationToken cancellationToken)
        {
            CommonDTO Result = new CommonDTO();
            try
            {
                string UpdateActionMessage = string.Empty;
                var uploadData = await _context.Set<ImportDump>().Where(t => t.ImportDumpId == locationOnlyUploadModel.UserImportTotalId).FirstOrDefaultAsync();
                if (uploadData != null)
                {
                    if (uploadData.ActionCheck != "ERROR")
                    {
                        int NewAddedLocationId = 0;

                        int Status = ImportService.GetStatusValue(uploadData.LocationStatus, "LOCATION");


                        if (uploadData.LocationCheck.ToUpper() == "NEW" &&
                            (uploadData.Action.ToUpper() == "ADD" || uploadData.Action.ToUpper() == "UPDATE"))
                        {
                            LatLng LL = _DBC.GetCoordinates(uploadData.LocationAddress);

                            string TZone = Convert.ToString((from c in _context.Set<Company>() where c.CompanyId == uploadData.CompanyId select c.TimeZone).FirstOrDefault());

                            Location newLocation = new Location();
                            newLocation.CompanyId = uploadData.CompanyId;
                            newLocation.LocationName = uploadData.Location;
                            newLocation.Lat = LL.Lat;
                            newLocation.Long = LL.Lng;
                            newLocation.Desc = uploadData.Location;
                            newLocation.PostCode = uploadData.LocationAddress;
                            newLocation.Status = Status;
                            newLocation.CreatedBy = userId;
                            newLocation.UpdatedBy = userId;
                            NewAddedLocationId = await _locationRepository.CreateLocation(newLocation, cancellationToken);
                            uploadData.ActionCheck = NewCheck;

                            UpdateActionMessage += "New location created" + Environment.NewLine;
                        }
                        else if (uploadData.LocationCheck.ToUpper() == "DUPLICATE" || uploadData.LocationCheck.ToUpper() == "OK")
                        { //Existing location update

                            if (uploadData.Action.ToUpper() == "DELETE")
                            {
                                await _locationRepository.DeleteLocation((int)uploadData.LocationId, cancellationToken);
                                UpdateActionMessage += "Location is deleted" + Environment.NewLine;
                                uploadData.ActionCheck = DeletedCheck;

                            }
                            else if (uploadData.Action.ToUpper() == "UPDATE" || uploadData.Action.ToUpper() == "ADD")
                            {  //update an existing location

                                LatLng LL = _DBC.GetCoordinates(uploadData.LocationAddress);

                                var locUpdate = await _context.Set<Location>().Where(t=>t.LocationId == uploadData.LocationId).FirstOrDefaultAsync();
                                if (locUpdate != null)
                                {
                                    locUpdate.Status = Status;
                                    locUpdate.PostCode = _DBC.Left(uploadData.LocationAddress, 150);
                                    locUpdate.Lat = _DBC.Left(LL.Lat, 15);
                                    locUpdate.Long = _DBC.Left(LL.Lng, 15);
                                    locUpdate.UpdatedBy = userId;
                                    locUpdate.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                                    await _context.SaveChangesAsync();
                                    uploadData.ActionCheck = OverrideCheck;
                                    UpdateActionMessage += "Location details updated" + Environment.NewLine;
                                }
                            }
                            else if (uploadData.ImportAction.ToUpper() == "SKIP")
                            {
                                uploadData.ActionCheck = SkipCheck;
                                UpdateActionMessage += "Location update skipped" + Environment.NewLine;
                            }

                        }

                        uploadData.ValidationMessage = UpdateActionMessage;

                        uploadData.ImportAction = "Imported";
                        await _context.SaveChangesAsync();
                        Result.ErrorId = 0;
                        Result.Message = UpdateActionMessage;
                    }
                    else
                    {
                        Result.ErrorId = 100;
                        Result.Message = uploadData.ValidationMessage;
                    }
                }
                else
                {
                    Result.ErrorId = 100;
                    Result.Message = "Record not found to import";
                }
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorId = 100;
                Result.Message = ex.Message.ToString();
                return Result;
            }
        }

        public async Task<bool> ProcessUserImport(ProcessImport processImport, CancellationToken cancellationToken)
        {
            try
            {
                CreateImportHeader(processImport.SessionId, companyId, "TOBEIMPORTED", userId, "NA", "NA", processImport.SendInvite, 0, false, "FULL");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void CreateImportHeader(string sessionId, int companyId, string status, int userId, string dataFile = "NOFILE", string mappingFile = "NOFILE",
            bool sendInvite = false, int importTriggerId = 0, bool autoForceVerify = false, string jobType = "FULL")
        {
            try
            {
                var pSessionId = new SqlParameter("@SessionID", sessionId);
                var pCompanyId = new SqlParameter("@CompanyID", companyId);
                var pMappingFile = new SqlParameter("@MappingFileName", mappingFile);
                var pDataFile = new SqlParameter("@FileName", dataFile);
                var pStatus = new SqlParameter("@Status", status);
                var pSendInvite = new SqlParameter("@SendInvite", sendInvite);
                var pImportTriggerID = new SqlParameter("@ImportTriggerID", importTriggerId);
                var pCurrentUserId = new SqlParameter("@LoggedInUserID", userId);
                var pAutoForceVerify = new SqlParameter("@AutoForceVerify", autoForceVerify);
                var pJobType = new SqlParameter("@JobType", jobType);

                var UserOnlyrec = _context.Set<JsonResult>().FromSqlRaw("EXEC Pro_ImportUser_CreateHeader @SessionID, @CompanyID, @MappingFileName, @FileName, @Status, " +
                    "@SendInvite, @AutoForceVerify, @JobType, @ImportTriggerID, @LoggedInUserID",
                    pSessionId, pCompanyId, pMappingFile, pDataFile, pStatus, pSendInvite, pAutoForceVerify, pJobType, pImportTriggerID, pCurrentUserId).ToList();

            }
            catch (Exception ex)
            {
            }
        }

        public dynamic QueueImportJob(QueueImport queueImport, CancellationToken cancellationToken)
        {
            try
            {
                CreateImportHeader(queueImport.SessionId, companyId, "DUMPING", userId, queueImport.DataFileName, queueImport.MappingFileName, queueImport.SendInvite, jobType: queueImport.JobType);

                string path = _DBC.LookupWithKey("UPLOAD_PATH");

                string newSesid = Guid.NewGuid().ToString();
                string filePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ImportFilesPath"]);
                string uploadPath = filePath + companyId.ToString() + "\\" + queueImport.SessionId + "\\";
                string temp_map_file = path + queueImport.MappingFileName;
                string temp_data_file = path + queueImport.DataFileName;

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                if (File.Exists(temp_map_file))
                {
                    File.Move(temp_map_file, uploadPath + queueImport.MappingFileName);
                }
                if (File.Exists(temp_data_file))
                {
                    File.Move(temp_data_file, uploadPath + queueImport.DataFileName);
                }

                if (File.Exists(uploadPath + queueImport.MappingFileName) && File.Exists(uploadPath + queueImport.DataFileName))
                {
                    NewImport NI = new NewImport(companyId, userId, queueImport.SessionId, "USER", uploadPath + queueImport.DataFileName, "CSV",
                        uploadPath + queueImport.MappingFileName, "XML", true, ",", queueImport.SendInvite, JobType: queueImport.JobType);

                    NI.BulkInsert();

                    var queuStatus = QueueImportTask(queueImport.SessionId, companyId, queueImport.SendInvite, userId, jobType: queueImport.JobType);

                    return queuStatus;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string QueueImportTask(string sessionId, int companyId, bool sendInvite, int currentUserId, string jobType = "FULL")
        {
            string queue_status = "DUMPED";
            DateTimeOffset dtNow = _DBC.GetDateTimeOffset(DateTime.Now).AddHours(-1);
            var running_import = (from IM in _context.Set<ImportDumpHeader>()
                                    where IM.CompanyId == companyId && IM.JobType == "FULL"
                                    && (IM.Status == "IMPORTING" || IM.Status == "TOBEIMPORTED" || IM.Status == "VALIDATING" ||
                                    IM.Status == "VALIDATED" || IM.Status == "DUMPED" || IM.Status == "DUMPING") && IM.CreatedOn > dtNow
                                    && IM.SessionId != sessionId && IM.FileName != "NOFILE"
                                    select IM).Any();
            if (running_import)
            {
                CreateImportHeader(sessionId, companyId, "WAITING", currentUserId, "NA", "NA", sendInvite, jobType: jobType);
                queue_status = "WAITING";
            }
            else
            {
                CreateImportHeader(sessionId, companyId, "DUMPED", currentUserId, "NA", "NA", sendInvite, jobType: jobType);
                queue_status = "DUMPED";
            }
            return queue_status;
        }

        public void UploadSingleFile()
        {
            throw new NotImplementedException();
        }

        public void UserCompleteImport(UserCompleteImportModel userCompleteImportModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void UserCompleteUpload(UserCompleteUploadModel userCompleteUploadModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> createDepartmentData(string sessionId, int companyId, int userId)
        {

            try
            {
                var impDepData = await _context.Set<ImportDump>().Where(t => t.SessionId == sessionId).ToListAsync();
                if (impDepData.Count > 0)
                {

                    foreach (var item in impDepData)
                    {
                        string Rec = string.Empty;
                        string ValidationMessage = string.Empty;
                        int DepartmentId, HasError = 0;
                        bool DataError = false, MandatoryError = false;

                        //validateion the department all.
                        List<UserImportDataValidation> UIDV = new List<UserImportDataValidation>();

                        string deptformat = ImportService.ExtendedNames("Department", item.Department, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Department", MandatoryError = MandatoryError, DataError = DataError, Message = deptformat });
                        string dept = ImportService.ValidateLength("Department", item.Department, 3, 100, out MandatoryError, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Department", MandatoryError = MandatoryError, DataError = DataError, Message = dept });

                        string action = ImportService.ValidateAction("Action", item.Action, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Action", MandatoryError = false, DataError = DataError, Message = action });

                        string status = ImportService.ValidateStatus("Status", item.GroupStatus, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Status", MandatoryError = false, DataError = DataError, Message = status });


                        string MultiGroup = string.Empty;
                        if (MultiOptionCheck(item.Department))
                        {
                            HasError++;
                            MultiGroup = "Multiple departments found, seprate them per line." + Environment.NewLine;
                        }

                        Rec = ImportCheckDepartmentExist(companyId, item.Department, sessionId, out DepartmentId);

                        string UpdateActionMessage = string.Empty;

                        int totalMandatoryErrors = 0;
                        int totalDataErrors = 0;

                        foreach (UserImportDataValidation valErrors in UIDV)
                        {
                            if (valErrors.MandatoryError == true || valErrors.DataError == true)
                            {
                                ValidationMessage = ValidationMessage + valErrors.Message;
                                if (valErrors.MandatoryError)
                                    totalMandatoryErrors++;

                                if (valErrors.DataError)
                                    totalDataErrors++;

                                HasError++;
                            }
                        }

                        if (Rec == "NEW" && item.Action == "DELETE")
                        {
                            ValidationMessage = "Department do not exist in database. Cannot delete." + Environment.NewLine;
                            HasError++;
                        }

                        if (totalMandatoryErrors > 0 && totalDataErrors == 0 && Rec == "DUPLICATE" && item.Action == "DELETE")
                        {
                            HasError = -100;
                        }


                        if (HasError > 0)
                        {
                            item.ValidationMessage = ValidationMessage;
                            item.ActionCheck = "ERROR";
                            item.ImportAction = "ERROR";
                        }
                        else
                        {
                            if (Rec == "DUPLICATE")
                            {
                                if (HasError != -100)
                                {
                                    UpdateActionMessage += "Department details will be updated." + Environment.NewLine;
                                }
                                else
                                {
                                    if (item.Action.ToUpper() == "DELETE")
                                    {
                                        UpdateActionMessage += "Department will be deleted." + Environment.NewLine;
                                    }
                                }
                            }
                            else
                            {
                                if (item.Action.ToUpper() == "DELETE")
                                {
                                    UpdateActionMessage += "Department does not exist in database. Cannot delete." + Environment.NewLine;
                                }
                                else
                                {
                                    UpdateActionMessage += "New department will be created." + Environment.NewLine;
                                }
                            }
                            item.ValidationMessage = UpdateActionMessage;
                        }

                        item.DepartmentId = DepartmentId;
                        item.DepartmentCheck = Rec;
                        await _context.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool MultiOptionCheck(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Contains("|"))
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        private string ImportCheckDepartmentExist(int companyId, string departmentName, string dessionId, out int departmentId)
        {
            departmentId = 0;
            if (!string.IsNullOrEmpty(departmentName))
            {
                var isExist = _context.Set<Department>().Where(d=> d.DepartmentName == departmentName && d.CompanyId == companyId).FirstOrDefault();
                if (isExist != null)
                {
                    departmentId = isExist.DepartmentId;
                    return "DUPLICATE";
                }
                else
                {
                    return "NEW";
                }
            }
            else
            {
                return "EMPTY";
            }
        }

        public bool CreateTempDepartment(List<ImportDumpInput> data, string sessionId, int companyId, int userId, string timeZoneId)
        {

            try
            {
                foreach (ImportDumpInput Dep in data)
                {

                    string Action = !string.IsNullOrEmpty(Dep.Action) ? Dep.Action : "ADD";
                    _ = ImportToDump(userId, companyId, sessionId,
                        "", "", "", "", "", "", "", "", "0", Action, "", "0", Dep.Department, Dep.DepartmentStatus, "", "", "0", "", "", "",
                        "", "", "", "", "", "", "", "", "", "", "DepartmentImportOnly", userId, TimeZoneId);

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task ImportToDump(int userId, int companyId, string sessionId,
           string firstName, string surname, string email, string ISD, string phone, string LLISD, string landline, string userRole, string status, string action,
           string group, string groupStatus,
           string department, string departmentStatus,
           string location, string locationAddress, string locationStatus,
           string security, string securityDescription,
           string pingMethods, string incidentMethods,
           string locationAction, string groupAction, string departmentAction,
           string emailCheck, string locationCheck, string groupCheck, string departmentCheck, string securityCheck, string importAction, string actionType, int createdUpdatedBy, string timeZoneId)
        {
            try
            {

                ImportDump IMPDump = new ImportDump();

                IMPDump.UserId = userId;
                IMPDump.CompanyId = companyId;
                IMPDump.SessionId = sessionId;

                if (actionType.ToUpper() == "USERIMPORTONLY" || actionType.ToUpper() == "USERIMPORTCOMPLETE")
                {
                    IMPDump.FirstName = firstName;
                    IMPDump.Surname = surname;
                    IMPDump.Email = (!string.IsNullOrEmpty(email) ? email.ToLower() : "");
                    IMPDump.EncryptedEmail = (!string.IsNullOrEmpty(email) ? email.ToLower() : "");
                    IMPDump.Status = status;
                    IMPDump.Isd = ISD;
                    IMPDump.Phone = phone;
                    IMPDump.Llisd = LLISD;
                    IMPDump.Landline = landline;
                    IMPDump.UserRole = (!string.IsNullOrEmpty(userRole) ? userRole.ToUpper() : "");
                    IMPDump.EmailCheck = emailCheck;
                    IMPDump.PingMethods = pingMethods;
                    IMPDump.IncidentMethods = incidentMethods;
                    IMPDump.LocationAction = locationAction;
                    IMPDump.GroupAction = groupAction;
                    IMPDump.ActionCheck = "";
                }

                if (actionType.ToUpper() == "GROUPIMPORTONLY" || actionType.ToUpper() == "USERIMPORTCOMPLETE")
                {
                    IMPDump.Group = group;
                    IMPDump.GroupStatus = groupStatus;
                    IMPDump.GroupCheck = groupCheck;
                }
                if (actionType.ToUpper() == "DEPARTMENTIMPORTONLY" || actionType.ToUpper() == "USERIMPORTCOMPLETE")
                {
                    IMPDump.Department = department;
                    IMPDump.DepartmentStatus = departmentStatus;
                    IMPDump.DepartmentCheck = departmentCheck;
                }
                if (actionType.ToUpper() == "LOCATIONIMPORTONLY" || actionType.ToUpper() == "USERIMPORTCOMPLETE")
                {
                    IMPDump.Location = location;
                    IMPDump.LocationAddress = locationAddress;
                    IMPDump.LocationStatus = locationStatus;
                    IMPDump.LocationCheck = locationCheck;

                }
                if (actionType.ToUpper() == "SECURITYIMPORTONLY" || actionType.ToUpper() == "USERIMPORTCOMPLETE")
                {
                    IMPDump.Security = security;
                    IMPDump.SecurityCheck = securityCheck;
                }

                IMPDump.ImportAction = importAction;
                IMPDump.ActionType = actionType;
                IMPDump.Action = action;
                if (createdUpdatedBy > 0)
                    IMPDump.CreatedBy = createdUpdatedBy;
                IMPDump.CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                if (createdUpdatedBy > 0)
                    IMPDump.UpdatedBy = createdUpdatedBy;
                IMPDump.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                await _context.Set<ImportDump>().AddAsync(IMPDump);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex; }
        }

        public bool CreateTempUsers(List<ImportDumpInput> userData, string sessionId, int companyId, string jobType, int userId = 0, string timeZoneId = "GMT Standard Time")
        {

            try
            {

                CreateImportHeader(sessionId, companyId, "DUMPING", userId, "NOFILE", "NOFILE", false, 0, false, jobType);

                foreach (ImportDumpInput Usr in userData)
                {
                    _ = ImportToDump(0, companyId, sessionId, Usr.FirstName, Usr.Surname, Usr.Email, Usr.MobileISD, Usr.Mobile, Usr.ISDLandline, Usr.Landline,
                        Usr.UserRole, Usr.Status, Usr.Action, Usr.Group, Usr.GroupStatus, Usr.Department, Usr.DepartmentStatus, Usr.Location, Usr.LocationAddress, Usr.LocationStatus,
                    Usr.MenuAccess, Usr.MenuAccess, Usr.PingMethods, Usr.IncidentMethods, Usr.LocationAction, Usr.GroupAction, Usr.DepartmentAction,
                    "", "", "", "", "", "", "USERIMPORTCOMPLETE", userId, TimeZoneId);
                }

                QueueImportTask(sessionId, companyId, false, userId, jobType);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<CommonDTO> RefreshTmpTable(int companyId, int userId, string sessionId)
        {
            CommonDTO ResultDTO = new CommonDTO();
            try
            {
                DateTime DelTime = DateTime.Now.AddHours(-1);

                var Imp = (from I in _context.Set<ImportDump>()
                            where (I.CompanyId == companyId && I.UserId == userId && I.SessionId == sessionId) ||
                            I.CreatedOn <= DelTime
                            select I).ToList();
                _context.Set<ImportDump>().RemoveRange(Imp);
                await _context.SaveChangesAsync();
                ResultDTO.ErrorId = 1;
                ResultDTO.Message = "Table Refreshed";
                return ResultDTO;
            }
            catch (Exception ex)
            {
                return ResultDTO;
            }
        }

        public bool CreateTempLocation(List<ImportDumpInput> locData, string sessionId, int companyId, int userId = 0, string timeZoneId = "GMT Standard Time")
        {

            try
            {
                foreach (ImportDumpInput Loc in locData)
                {

                    string Action = !string.IsNullOrEmpty(Loc.Action) ? Loc.Action : "ADD";

                    _ = ImportToDump(userId, companyId, sessionId, "", "", "", "", "", "", "", "", "0",
                        Action, "", "0", "", "0", Loc.Location, Loc.LocationAddress, Loc.LocationStatus, "", "", "", "", "", "", "", "",
                        "", "", "", "", "", "LocationImportOnly", userId, timeZoneId);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CreateTempGroup(List<ImportDumpInput> data, string sessionId, int companyId, int userId, string timeZoneId)
        {

            try
            {
                foreach (ImportDumpInput Dep in data)
                {

                    string Action = !string.IsNullOrEmpty(Dep.Action) ? Dep.Action : "ADD";
                    _ = ImportToDump(userId, companyId, sessionId,
                        "", "", "", "", "", "", "", "", "0", Action, Dep.Group, Dep.Status, "", "0", "", "", "0", "", "", "", "", "", "",
                        "", "", "", "", "", "", "", "GroupImportOnly", userId, timeZoneId);

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<object> GetCountActionCheck(string sessionId, int outUserCompanyId)
        {
            CommonDTO ResultDTO = new CommonDTO();
            try
            {

                var ImportActionCheck = await (from UIT in _context.Set<ImportDump>()
                                         where UIT.SessionId == sessionId
                                         select new { ActionCheck = UIT.ActionCheck, ActionType = UIT.ActionType }).ToListAsync();
                if (ImportActionCheck.Count > 0)
                {
                    int TotalNewImported = 0;
                    int TotalUpdated = 0;
                    int TotalSkiped = 0;
                    int TotalDeleted = 0;
                    string ImportType = string.Empty;
                    foreach (var item in ImportActionCheck)
                    {
                        if (!string.IsNullOrEmpty(item.ActionCheck))
                        {
                            if (item.ActionCheck.ToUpper() == NewCheck.ToUpper())
                            {
                                TotalNewImported += 1;
                            }
                            else if (item.ActionCheck.ToUpper() == OverrideCheck.ToUpper())
                            {
                                TotalUpdated += 1;
                            }
                            else if (item.ActionCheck.ToUpper() == SkipCheck.ToUpper() || item.ActionCheck == "ERROR")
                            {
                                TotalSkiped += 1;
                            }
                            else if (item.ActionCheck.ToUpper() == DeletedCheck.ToUpper())
                            {
                                TotalDeleted += 1;
                            }
                        }
                        else
                        {
                            TotalSkiped += 1;
                        }
                        ImportType = item.ActionType;
                    }

                    ImportResult ImpDTO = new ImportResult();
                    ImpDTO.TotalImport = TotalNewImported;
                    ImpDTO.TotalUpdate = TotalUpdated;
                    ImpDTO.TotalSkip = TotalSkiped;
                    ImpDTO.TotalDelete = TotalDeleted;

                    DateTime DelTime = DateTime.Now.AddHours(-1);

                    var DelImprtData = await (from UIT in _context.Set<ImportDump>()
                                        where UIT.SessionId == sessionId
                                        select UIT).ToListAsync();
                    try
                    {

                        string ResultFilePath = _DBC.Getconfig("ImportResultPath");
                        string FileName = outUserCompanyId + "\\" + sessionId.TrimStart('{').TrimEnd('}') + "\\import_log_report.csv";
                        bool fileCreated = DataHelper.CreateImportResult(DelImprtData, ResultFilePath + FileName, ImportType);
                        if (fileCreated)
                            ImpDTO.ResultFile = FileName;

                        var DelRecs = await (from UIT in _context.Set<ImportDump>()
                                       where UIT.CreatedOn <= DelTime
                                       select UIT).ToListAsync();
                        _context.Set<ImportDump>().RemoveRange(DelRecs);
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                    }
                    return ImpDTO;
                }
                else
                {
                    ResultDTO.ErrorId = 110;
                    ResultDTO.Message = "Record not found";
                }
                return ResultDTO;
            }
            catch (Exception ex)
            {
                return ResultDTO;
            }

        }

    }
}
