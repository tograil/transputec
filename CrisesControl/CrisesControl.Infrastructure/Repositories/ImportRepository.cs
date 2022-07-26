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

                            Core.Locations.Location newLocation = new Core.Locations.Location();
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
                                _locationRepository.DeleteLocation((int)uploadData.LocationId, cancellationToken);
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
                                    _context.SaveChanges();
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
                        _context.SaveChanges();
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

        public void CreateImportHeader(string SessionID, int CompanyID, string Status, int UserID, string DataFile = "NOFILE", string MappingFile = "NOFILE",
            bool SendInvite = false, int ImportTriggerID = 0, bool AutoForceVerify = false, string JobType = "FULL")
        {
            try
            {
                var pSessionId = new SqlParameter("@SessionID", SessionID);
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var pMappingFile = new SqlParameter("@MappingFileName", MappingFile);
                var pDataFile = new SqlParameter("@FileName", DataFile);
                var pStatus = new SqlParameter("@Status", Status);
                var pSendInvite = new SqlParameter("@SendInvite", SendInvite);
                var pImportTriggerID = new SqlParameter("@ImportTriggerID", ImportTriggerID);
                var pCurrentUserId = new SqlParameter("@LoggedInUserID", UserID);
                var pAutoForceVerify = new SqlParameter("@AutoForceVerify", AutoForceVerify);
                var pJobType = new SqlParameter("@JobType", JobType);

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
                CreateImportHeader(queueImport.SessionId, companyId, "DUMPING", userId, queueImport.DataFileName, queueImport.MappingFileName, queueImport.SendInvite, JobType: queueImport.JobType);

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

                    var queuStatus = QueueImportTask(queueImport.SessionId, companyId, queueImport.SendInvite, userId, JobType: queueImport.JobType);

                    return queuStatus;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string QueueImportTask(string SessionId, int CompanyId, bool SendInvite, int CurrentUserId, string JobType = "FULL")
        {
            string queue_status = "DUMPED";
            DateTimeOffset dtNow = _DBC.GetDateTimeOffset(DateTime.Now).AddHours(-1);
            var running_import = (from IM in _context.Set<ImportDumpHeader>()
                                    where IM.CompanyId == CompanyId && IM.JobType == "FULL"
                                    && (IM.Status == "IMPORTING" || IM.Status == "TOBEIMPORTED" || IM.Status == "VALIDATING" ||
                                    IM.Status == "VALIDATED" || IM.Status == "DUMPED" || IM.Status == "DUMPING") && IM.CreatedOn > dtNow
                                    && IM.SessionId != SessionId && IM.FileName != "NOFILE"
                                    select IM).Any();
            if (running_import)
            {
                CreateImportHeader(SessionId, CompanyId, "WAITING", CurrentUserId, "NA", "NA", SendInvite, JobType: JobType);
                queue_status = "WAITING";
            }
            else
            {
                CreateImportHeader(SessionId, CompanyId, "DUMPED", CurrentUserId, "NA", "NA", SendInvite, JobType: JobType);
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

        public async Task<bool> createDepartmentData(string SessionId, int CompanyId, int UserId)
        {

            try
            {
                var impDepData = await _context.Set<ImportDump>().Where(t => t.SessionId == SessionId).ToListAsync();
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

                        Rec = ImportCheckDepartmentExist(CompanyId, item.Department, SessionId, out DepartmentId);

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
    }
}
