using AutoMapper;
using CrisesControl.Core.Billing.Repositories;
using CrisesControl.Core.Common;
using CrisesControl.Core.Companies;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Groups.Repositories;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Locations.Services;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
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
        private readonly IDBCommonRepository _DBC;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBillingRepository _billing;
        private readonly ISenderEmailService _SDE;
        private readonly IMapper _mapper;
        private string UserRole = "ADMIN";
        private int userId;
        private int companyId;

        string NewCheck = "NewAdded";
        string OverrideCheck = "Updated";
        string SkipCheck = "Skiped";
        string DeletedCheck = "Deleted";
        string TimeZoneId = "GMT Standard Time";
        private string UpdateRole = "false";
        public int ImportUserId = -1;
        public ImportRepository(CrisesControlContext context, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ImportRepository> logger,
            IDBCommonRepository DBC, 
            IDepartmentRepository departmentRepository, 
            IMapper mapper, ISenderEmailService SDE,
            IGroupRepository groupRepository, IBillingRepository billing,
            ILocationRepository locationRepository, IUserRepository userRepository)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _DBC = DBC;
            _mapper = mapper;
            _departmentRepository = departmentRepository;
            _groupRepository = groupRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _billing = billing;
            _SDE = SDE;
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
                                if (await _DBC.IsPropertyExist(chkdept, "ErrorId"))
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
                                    depatUpdate.UpdatedOn =await _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
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
                                if (await _DBC.IsPropertyExist(chkdept, "ErrorId"))
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
                                    depatUpdate.UpdatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
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
                            LatLng LL =await _DBC.GetCoordinates(uploadData.LocationAddress);

                            string TZone = Convert.ToString(await _context.Set<Company>().Where(c=> c.CompanyId == uploadData.CompanyId).Select(c=> c.TimeZone).FirstOrDefaultAsync());

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
                               await _locationRepository.DeleteLocation((int)uploadData.LocationId, cancellationToken);
                                UpdateActionMessage += "Location is deleted" + Environment.NewLine;
                                uploadData.ActionCheck = DeletedCheck;

                            }
                            else if (uploadData.Action.ToUpper() == "UPDATE" || uploadData.Action.ToUpper() == "ADD")
                            {  //update an existing location

                                LatLng LL =await _DBC.GetCoordinates(uploadData.LocationAddress);

                                var locUpdate = await _context.Set<Location>().Where(t=>t.LocationId == uploadData.LocationId).FirstOrDefaultAsync();
                                if (locUpdate != null)
                                {
                                    locUpdate.Status = Status;
                                    locUpdate.PostCode = _DBC.Left(uploadData.LocationAddress, 150);
                                    locUpdate.Lat = _DBC.Left(LL.Lat, 15);
                                    locUpdate.Long = _DBC.Left(LL.Lng, 15);
                                    locUpdate.UpdatedBy = userId;
                                    locUpdate.UpdatedOn =await _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
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
               await  CreateImportHeader(processImport.SessionId, companyId, "TOBEIMPORTED", userId, "NA", "NA", processImport.SendInvite, 0, false, "FULL");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task CreateImportHeader(string sessionId, int companyId, string status, int userId, string dataFile = "NOFILE", string mappingFile = "NOFILE",
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

                var UserOnlyrec =await _context.Set<JsonResult>().FromSqlRaw("EXEC Pro_ImportUser_CreateHeader @SessionID, @CompanyID, @MappingFileName, @FileName, @Status, " +
                    "@SendInvite, @AutoForceVerify, @JobType, @ImportTriggerID, @LoggedInUserID",
                    pSessionId, pCompanyId, pMappingFile, pDataFile, pStatus, pSendInvite, pAutoForceVerify, pJobType, pImportTriggerID, pCurrentUserId).ToListAsync();

            }
            catch (Exception ex)
            {
            }
        }

        public async Task<dynamic> QueueImportJob(QueueImport queueImport, CancellationToken cancellationToken)
        {
            try
            {
                CreateImportHeader(queueImport.SessionId, companyId, "DUMPING", userId, queueImport.DataFileName, queueImport.MappingFileName, queueImport.SendInvite, jobType: queueImport.JobType);

                string path =await _DBC.LookupWithKey("UPLOAD_PATH");

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

        private async Task<string> QueueImportTask(string sessionId, int companyId, bool sendInvite, int currentUserId, string jobType = "FULL")
        {
            string queue_status = "DUMPED";
            string timeZone = "GMT Standard Time";
            DateTimeOffset dateTime = await _DBC.GetDateTimeOffset(DateTime.Now);
            DateTimeOffset dtNow = dateTime.AddHours(-1);
            var running_import = await _context.Set<ImportDumpHeader>()
                                    .Where(IM=> IM.CompanyId == companyId && IM.JobType == "FULL"
                                    && (IM.Status == "IMPORTING" || IM.Status == "TOBEIMPORTED" || IM.Status == "VALIDATING" ||
                                    IM.Status == "VALIDATED" || IM.Status == "DUMPED" || IM.Status == "DUMPING") && IM.CreatedOn > dtNow
                                    && IM.SessionId != sessionId && IM.FileName != "NOFILE"
                                    ).AnyAsync();
            if (running_import)
            {
               await CreateImportHeader(sessionId, companyId, "WAITING", currentUserId, "NA", "NA", sendInvite, jobType: jobType);
                queue_status = "WAITING";
            }
            else
            {
               await CreateImportHeader(sessionId, companyId, "DUMPED", currentUserId, "NA", "NA", sendInvite, jobType: jobType);
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

        public async Task<CommonDTO> UserCompleteUpload(int importRecId, int companyId, CancellationToken cancellationToken, int currentUserId = 0, string timeZoneId = "GMT Standard Time", bool sendInvite = false, bool autoForceVerify = false)
        {
            CommonDTO Result = new CommonDTO();
            try
            {

                //DBC.CreateLog("INFO", "ImportHelper: Import to the desination table", null, "ImportHelper", "ImportUsers", CompanyId);
                
                int UserIdToUpdate = -1;
                bool ValidPhone = true;

                var UploadData = await _context.Set<ImportDump>().Where(UIT=> UIT.ImportDumpId == importRecId).FirstOrDefaultAsync();
                if (UploadData != null)
                {

                    UploadData.Email = UploadData.Email.ToLower();
                    UserIdToUpdate = UploadData.UserId;

                    int UserStatus = autoForceVerify == true ? 1 : 2;
                    bool FirstLogin = autoForceVerify == true ? false : true;

                    

                    if (UploadData.ActionCheck != "ERROR" && UploadData.EmailCheck != null)
                    {

                        bool IsNew = false;
                        int GroupToUpdate = 0, DepartmentToUpdate = 0, LocationToUpdate = 0;
                        string UniqId = Guid.NewGuid().ToString();
                        string UpdateActionMessage = string.Empty;

                        int Status = DataHelper.GetStatusValue(UploadData.Status);

                        if (UploadData.EmailCheck.ToUpper() == "NEW" &&
                            (UploadData.Action.ToUpper() == "ADD" || UploadData.Action.ToUpper() == "UPDATE"))
                        { //Create new user and get userid

                            IsNew = true;

                            string newPwd =await _DBC.RandomPassword();


                            //DBC.CreateLog("INFO", "ImportHelper: Inserting the user to db", null, "ImportHelper", "ImportUsers", CompanyId);

                            //Add the user and get the userid
                            UserIdToUpdate = await _userRepository.CreateUsers(UploadData.CompanyId, false, UploadData.FirstName, UploadData.Email, newPwd, UserStatus,
                                currentUserId, TimeZoneId, UploadData.Surname, UploadData.Phone, UploadData.UserRole, string.Empty, UploadData.Isd,
                                UploadData.Llisd, UploadData.Landline, "", UniqId, "", "", "", true, "", false, FirstLogin);

                            ValidPhone = _userRepository.IsValidMobile;

                            //DBC.CreateLog("INFO", "ImportHelper: New user created userid " + UserIdToUpdate.ToString(), null, "ImportHelper", "ImportUsers", CompanyId);

                            if (UserIdToUpdate <= 0)
                            {
                                Result.ErrorId = 110;
                                Result.Message = "Email id already exist";
                                return Result;
                            }

                            //Creating default relation with the ALL groups
                            await _DBC.CreateObjectRelationship(UserIdToUpdate, 0, "LOCATION", UploadData.CompanyId, currentUserId, TimeZoneId, "ALL");
                            await _DBC.CreateObjectRelationship(UserIdToUpdate, 0, "GROUP", UploadData.CompanyId, currentUserId, TimeZoneId, "ALL");

                            UploadData.ActionCheck = NewCheck;
                            //DBC.UpdateUserRoleChangeLog("ADD", UploadData.CompanyId, UserIdToUpdate, CurrentUserId, UploadData.UserRole);

                            UpdateActionMessage += "New Account has been created|" + Environment.NewLine;

                            if (!ValidPhone)
                                UpdateActionMessage += "Invalid mobile number|" + Environment.NewLine;


                          await  _billing.AddUserRoleChange(UploadData.CompanyId, UserIdToUpdate, UploadData.UserRole, TimeZoneId);


                        }
                        else if (UploadData.EmailCheck.ToUpper() == "DUPLICATE" || UploadData.EmailCheck.ToUpper() == "OK")
                        { //Existing user update

                            var userUpdate = await  _context.Set<User>()
                                              .Where(U=> U.PrimaryEmail == UploadData.Email
                                              && U.CompanyId == UploadData.CompanyId
                                             ).FirstOrDefaultAsync();

                            if (userUpdate != null)
                            {
                                UserIdToUpdate = userUpdate.UserId;

                                if (UploadData.Action.ToUpper() == "DELETE")
                                {
                                    if (userUpdate.RegisteredUser != true)
                                    {
                                       await _userRepository.DeleteUser(userUpdate, cancellationToken);
                                        UploadData.ActionCheck = DeletedCheck;
                                        UpdateActionMessage += "User has been deleted|" + Environment.NewLine;
                                    }
                                }
                                else if (UploadData.Action.ToUpper() == "UPDATE" || UploadData.Action.ToUpper() == "ADD")
                                {  //update an existing user
                                    string ISDCode = UploadData.Isd;
                                    string MobileNo = UploadData.Phone;
                                    string LLISDCode = UploadData.Llisd;
                                    string LandLine = UploadData.Landline;


                                    if (!string.IsNullOrEmpty(UploadData.FirstName))
                                        userUpdate.FirstName = UploadData.FirstName;

                                    if (!string.IsNullOrEmpty(UploadData.Surname))
                                        userUpdate.LastName = UploadData.Surname;

                                    if (!string.IsNullOrEmpty(ISDCode) && !string.IsNullOrEmpty(MobileNo))
                                    {
                                        await _DBC.GetFormatedNumber(MobileNo,  ISDCode, MobileNo, ISDCode);
                                        ValidPhone = _DBC.IsValidPhone;
                                    }

                                    if (!string.IsNullOrEmpty(ISDCode))
                                        userUpdate.Isdcode = _DBC.Left(ISDCode, 1) != "+" ? "+" + ISDCode : ISDCode;

                                    string DummyNumber =await _DBC.GetCompanyParameter("DUMMY_PHONE_NUMBER", UploadData.CompanyId);
                                    if (string.IsNullOrEmpty(UploadData.Phone) || UploadData.Phone == DummyNumber)
                                    { // when source is empty or dummy
                                        if (string.IsNullOrWhiteSpace(userUpdate.MobileNo) || userUpdate.MobileNo == DummyNumber)
                                        {
                                            userUpdate.MobileNo = await _DBC.FixMobileZero(DummyNumber);
                                        }
                                    }
                                    else
                                    { //When the source has valid number
                                        string OverridePhone =await _DBC.GetCompanyParameter("OVERRIDE_PHONE_NUMBER", UploadData.CompanyId);
                                        string OverrideDummy =await _DBC.GetCompanyParameter("OVERRIDE_DUMMY_NUMBER_ONLY", UploadData.CompanyId);
                                        if (OverridePhone == "true")
                                        {
                                            if (OverrideDummy == "true")
                                            { //only update dummy numbers when turned on.
                                                if (string.IsNullOrEmpty(userUpdate.MobileNo) || userUpdate.MobileNo == DummyNumber)
                                                {
                                                    userUpdate.MobileNo =await _DBC.FixMobileZero(MobileNo);
                                                }
                                            }
                                            else
                                            { // update the non number for all customers.
                                                userUpdate.MobileNo =await _DBC.FixMobileZero(MobileNo);
                                            }
                                        } // skip mobile number update.
                                    }

                                    if (!string.IsNullOrEmpty(LLISDCode) && !string.IsNullOrEmpty(LandLine))
                                    {
                                       await _DBC.GetFormatedNumber(LandLine,  LLISDCode, LandLine, LLISDCode);
                                    }

                                    if (!string.IsNullOrEmpty(LLISDCode))
                                        userUpdate.Llisdcode = _DBC.Left(LLISDCode, 1) != "+" ? "+" + LLISDCode : LLISDCode;

                                    if (!string.IsNullOrEmpty(UploadData.Landline))
                                        userUpdate.Landline =await _DBC.FixMobileZero(LandLine);


                                    if (!userUpdate.RegisteredUser)
                                    {
                                        if (!string.IsNullOrEmpty(UploadData.UserRole))
                                        {
                                            var roles =await _DBC.CCRoles();
                                            if (UpdateRole == "true" && roles.Contains(UserRole))
                                            {
                                                userUpdate.UserRole = UploadData.UserRole.ToUpper().Replace("STAFF", "USER");
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(UploadData.FirstName))
                                                    UpdateActionMessage += "User Role change skipped|" + Environment.NewLine;
                                            }

                                            //if(UserRole!="ADMIN" && new string[] {"KEHOLDER","ADMIN" }.Contains(userUpdate.UserRole.ToUpper()) && 
                                            //    UploadData.UserRole=="USER" && UpdateRole=="false" || (userUpdate.UserRole.ToUpper()=="ADMIN" && UserRole != "ADMIN")) {

                                            //    UpdateActionMessage += "User Role change skipped" + Environment.NewLine;
                                            //} else if((UserRole != "ADMIN" && UpdateRole == "true" && userUpdate.UserRole.ToUpper() != "ADMIN") ||
                                            //    (UserRole=="ADMIN")) {
                                            //    userUpdate.UserRole = UploadData.UserRole.ToUpper().Replace("STAFF", "USER");
                                            //}
                                        }

                                        if (autoForceVerify == true && !string.IsNullOrEmpty(UploadData.Status))
                                            userUpdate.Status = Status;

                                        if (userUpdate.Status != 2 && autoForceVerify == false && !string.IsNullOrEmpty(UploadData.Status))
                                            userUpdate.Status = Status;

                                        if (Status == 0 && !string.IsNullOrEmpty(UploadData.Status))
                                           await _DBC.RemoveUserDevice(userUpdate.UserId);
                                    }

                                    if (currentUserId > 0)
                                        userUpdate.UpdatedBy = currentUserId;

                                    userUpdate.IsValidNumber = ValidPhone;
                                    userUpdate.UpdatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                                    await _context.SaveChangesAsync();

                                   await _userRepository.CreateUserSearch(userUpdate.UserId, userUpdate.FirstName, userUpdate.LastName, userUpdate.Isdcode, userUpdate.MobileNo, userUpdate.PrimaryEmail, companyId);

                                    _userRepository.CreateSMSTriggerRight(companyId, userUpdate.UserId, userUpdate.UserRole, false, userUpdate.Isdcode, userUpdate.MobileNo, true);

                                    UploadData.ActionCheck = OverrideCheck;
                                    UpdateActionMessage += "Account details updated|" + Environment.NewLine;

                                    if (!ValidPhone)
                                        UpdateActionMessage += "Mobile number is invalid";

                                }
                                else if (UploadData.Action.ToUpper() == "UPDATEPROFILE")
                                {
                                    UploadData.ActionCheck = OverrideCheck;
                                }
                            }
                        }

                        if (UserIdToUpdate > 0)
                        {

                            //Location Handeling
                            if (!string.IsNullOrEmpty(UploadData.LocationCheck))
                            {
                                //Location Handeling
                                if ((UploadData.LocationAction.ToUpper() == "DELETE" || UploadData.LocationAction.ToUpper() == "REMOVE") &&
                                        UploadData.LocationCheck.ToUpper() == "DUPLICATE")
                                {
                                    //Remove the user from the location assignment
                                    await _DBC.RemoveUserObjectRelation("LOCATION", UserIdToUpdate, (int)UploadData.LocationId, companyId, currentUserId, TimeZoneId);
                                    UpdateActionMessage += "Location removed from user profile|" + Environment.NewLine;

                                }
                                else if (UploadData.LocationAction.ToUpper() == "ADD" || UploadData.LocationAction.ToUpper() == "UPDATE")
                                {

                                    //get the location coordinates
                                    LatLng LL = new LatLng();
                                    int LocStatus = (UploadData.LocationStatus == "INACTIVE" ? 0 : 1);
                                    if (string.IsNullOrEmpty(UploadData.LocLat) && string.IsNullOrEmpty(UploadData.LocLng))
                                    {
                                        LL =await _DBC.GetCoordinates(UploadData.LocationAddress);
                                    }
                                    else
                                    {
                                        LL.Lat = UploadData.LocLat;
                                        LL.Lng = UploadData.LocLng;
                                        LocStatus = 1;
                                    }

                                    if (LL.Lat == "0" && LL.Lng == "0")
                                    {
                                        LocStatus = 0;
                                    }

                                    if (UploadData.LocationCheck.ToUpper() == "NEW")
                                    {
                                       
                                        //Create New Location if do not exist
                                        Location NewLocation = new Location()
                                        {
                                            CompanyId = UploadData.CompanyId,
                                            LocationName = UploadData.Location,
                                            Lat = LL.Lat,
                                            Long = LL.Lng,
                                            Desc = UploadData.Location,
                                            PostCode = UploadData.LocationAddress,
                                            Status = LocStatus,
                                            CreatedBy = currentUserId,
                                            CreatedOn = DateTime.Now,
                                            UpdatedBy = currentUserId,
                                            UpdatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId)
                                        };
                                        LocationToUpdate = await _locationRepository.CreateLocation(NewLocation,cancellationToken);
                                        UpdateActionMessage += "New location created and assigned to user|" + Environment.NewLine;
                                    }
                                    else if (UploadData.LocationCheck.ToUpper() == "DUPLICATE")
                                    {
                                        LocationToUpdate = (int)UploadData.LocationId;
                                        UpdateActionMessage += "Location assigned to user|" + Environment.NewLine;
                                    }
                                    else if (UploadData.LocationCheck.ToUpper() == "ERROR")
                                    {
                                        UpdateActionMessage += "Location was not added|" + Environment.NewLine;
                                    }

                                    if (UploadData.LocationCheck.ToUpper() != "ERROR")
                                       await _DBC.CreateObjectRelationship(UserIdToUpdate, LocationToUpdate, "LOCATION", UploadData.CompanyId, currentUserId, TimeZoneId);
                                }
                            }

                            //Group Handeling
                            if (!string.IsNullOrEmpty(UploadData.GroupCheck))
                            {
                                if ((UploadData.GroupAction.ToUpper() == "DELETE" || UploadData.GroupAction.ToUpper() == "REMOVE") &&
                                UploadData.GroupCheck.ToUpper() == "DUPLICATE")
                                {
                                    //Remove the user from the location assignment
                                    await _DBC.RemoveUserObjectRelation("GROUP", UserIdToUpdate, (int)UploadData.GroupId, companyId, currentUserId, TimeZoneId);
                                    UpdateActionMessage += "Group removed from user profile|" + Environment.NewLine;

                                }
                                else if (UploadData.GroupAction.ToUpper() == "ADD" || UploadData.GroupAction.ToUpper() == "UPDATE")
                                {


                                    if (UploadData.GroupCheck.ToUpper() == "NEW")
                                    {
                                        //Create New Location if do not exist
                                       CrisesControl.Core.Groups.Group group = new CrisesControl.Core.Groups.Group() {
                                            CompanyId = UploadData.CompanyId,
                                            GroupName= UploadData.Group,
                                            Status=1,
                                            CreatedBy= currentUserId,
                                            CreatedOn= await _DBC.GetDateTimeOffset(DateTime.Now,timeZoneId)

                                        };
                                        GroupToUpdate =await _groupRepository.CreateGroup(group,cancellationToken);

                                        UpdateActionMessage += "New department created and assigned to user|" + Environment.NewLine;
                                    }
                                    else if (UploadData.GroupCheck.ToUpper() == "DUPLICATE")
                                    {
                                        GroupToUpdate = (int)UploadData.GroupId;
                                        UpdateActionMessage += "Group assigned to user|" + Environment.NewLine;
                                    }
                                    else if (UploadData.GroupCheck.ToUpper() == "ERROR")
                                    {
                                        UpdateActionMessage += "Group was not added|" + Environment.NewLine;
                                    }

                                    if (UploadData.GroupCheck.ToUpper() != "ERROR")
                                       await _DBC.CreateObjectRelationship(UserIdToUpdate, GroupToUpdate, "GROUP", UploadData.CompanyId, currentUserId, TimeZoneId);
                                }
                            }

                            //Department Handeling
                            if (!string.IsNullOrEmpty(UploadData.DepartmentCheck))
                            {
                                if ((UploadData.DepartmentAction.ToUpper() == "DELETE" || UploadData.DepartmentAction.ToUpper() == "REMOVE") &&
                                UploadData.DepartmentCheck.ToUpper() == "DUPLICATE")
                                {
                                    //Remove the user from the location assignment
                                    await _DBC.RemoveUserObjectRelation("DEPARTMENT", UserIdToUpdate, (int)UploadData.GroupId, companyId, currentUserId, TimeZoneId);
                                    UpdateActionMessage += "Department removed from user profile|" + Environment.NewLine;
                                }
                                else if (UploadData.DepartmentAction.ToUpper() == "ADD" || UploadData.DepartmentAction.ToUpper() == "UPDATE")
                                {

                                    if (UploadData.DepartmentCheck.ToUpper() == "NEW")
                                    {
                                        //Create New Department if do not exist
                                        Department department = new Department() {
                                            CompanyId= UploadData.CompanyId,
                                            CreatedBy=currentUserId,
                                            CreatedOn=await _DBC.GetDateTimeOffset(DateTime.Now,timeZoneId),
                                            Status=1,
                                            DepartmentName= UploadData.Department,
                                            UpdatedBy= currentUserId,
                                            UpdatedOn= await _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId)

                                        };
                                        DepartmentToUpdate =await _departmentRepository.CreateDepartment(department,cancellationToken);

                                        UpdateActionMessage += "New department created and assigned to user|" + Environment.NewLine;
                                    }
                                    else if (UploadData.DepartmentCheck.ToUpper() == "DUPLICATE")
                                    {
                                        DepartmentToUpdate = (int)UploadData.DepartmentId;
                                        UpdateActionMessage += "Department assigned to user|" + Environment.NewLine;
                                    }
                                    else if (UploadData.DepartmentCheck.ToUpper() == "ERROR")
                                    {
                                        UpdateActionMessage += "Department was not added|" + Environment.NewLine;
                                    }

                                    if (UploadData.DepartmentCheck.ToUpper() != "ERROR")
                                       await _DBC.CreateObjectRelationship(UserIdToUpdate, DepartmentToUpdate, "DEPARTMENT", UploadData.CompanyId, currentUserId, TimeZoneId);
                                }
                            }

                            //Security Group handeling
                            if (!string.IsNullOrEmpty(UploadData.SecurityCheck))
                            {
                                var roles = await _DBC.CCRoles(true);
                                if (UploadData.SecurityCheck.ToUpper() == "DUPLICATE" && roles.Contains(UserRole))
                                {

                                    if (UploadData.EmailCheck.ToUpper() == "NEW")
                                    {
                                        await _userRepository.CreateUserSecurityGroup(UserIdToUpdate, (int)UploadData.SecurityGroupId, currentUserId, companyId, "Standard");
                                    }
                                    else
                                    {
                                        if (UpdateRole == "true")
                                        {
                                            var UpdateSecurity = await _context.Set<UserSecurityGroup>()
                                                                  .Where(USG=> USG.UserId == UserIdToUpdate).ToListAsync();
                                            bool secItemExist = false;
                                            if (UpdateSecurity != null)
                                            {
                                                foreach (var secitem in UpdateSecurity)
                                                {
                                                    if (secitem.SecurityGroupId != UploadData.SecurityGroupId)
                                                    {
                                                        _context.Remove(secitem);
                                                       await _context.SaveChangesAsync();
                                                    }
                                                    else
                                                    {
                                                        secItemExist = true;
                                                    }
                                                }
                                                if (!secItemExist)
                                                    await _userRepository.CreateUserSecurityGroup(UserIdToUpdate, (int)UploadData.SecurityGroupId, currentUserId, companyId, "Standard");

                                            }
                                            else
                                            {
                                                await _userRepository.CreateUserSecurityGroup(UserIdToUpdate, (int)UploadData.SecurityGroupId, currentUserId, companyId, "Standard");
                                            }
                                            UpdateActionMessage += "Menu Access assigned to user|" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            UpdateActionMessage += "Menu Access change skipped|" + Environment.NewLine;
                                        }
                                    }
                                }
                            }

                            //Handle the Comms Method
                            await _userRepository.UpdateUserComms(UploadData.CompanyId, UserIdToUpdate, currentUserId, timeZoneId, UploadData.PingMethods, UploadData.IncidentMethods, IsNew);

                            if (sendInvite && UploadData.EmailCheck.ToUpper() == "NEW")
                            {
                                
                                _SDE.NewUserAccount(UploadData.Email, UploadData.FirstName + " " + UploadData.Surname, UploadData.CompanyId, UniqId);
                                await _DBC.AddUserInvitationLog(UploadData.CompanyId, UserIdToUpdate, "INVITE", currentUserId, TimeZoneId);
                            }
                        } // 
                        UploadData.ValidationMessage = UpdateActionMessage;

                        UploadData.ImportAction = "Imported";
                        UploadData.UserId = UserIdToUpdate;

                        await _context.SaveChangesAsync();
                        Result.ErrorId = 0;
                        Result.Message = UpdateActionMessage;
                        ImportUserId = UserIdToUpdate;
                    }
                    else
                    {  //Record has error, report it back.
                        Result.ErrorId = 100;
                        Result.Message = UploadData.ValidationMessage;
                    }
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

        public async Task<bool> CreateTempDepartment(List<ImportDumpInput> data, string sessionId, int companyId, int userId, string timeZoneId)
        {

            try
            {
                foreach (ImportDumpInput Dep in data)
                {

                    string Action = !string.IsNullOrEmpty(Dep.Action) ? Dep.Action : "ADD";
                   await ImportToDump(userId, companyId, sessionId,
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
                IMPDump.CreatedOn =await _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                if (createdUpdatedBy > 0)
                    IMPDump.UpdatedBy = createdUpdatedBy;
                IMPDump.UpdatedOn =await _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                await _context.AddAsync(IMPDump);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<bool> CreateTempUsers(List<ImportDumpInput> userData, string sessionId, int companyId, string jobType, int userId = 0, string timeZoneId = "GMT Standard Time")
        {

            try
            {

               await CreateImportHeader(sessionId, companyId, "DUMPING", userId, "NOFILE", "NOFILE", false, 0, false, jobType);

                foreach (ImportDumpInput Usr in userData)
                {
                   await ImportToDump(0, companyId, sessionId, Usr.FirstName, Usr.Surname, Usr.Email, Usr.MobileISD, Usr.Mobile, Usr.ISDLandline, Usr.Landline,
                        Usr.UserRole, Usr.Status, Usr.Action, Usr.Group, Usr.GroupStatus, Usr.Department, Usr.DepartmentStatus, Usr.Location, Usr.LocationAddress, Usr.LocationStatus,
                    Usr.MenuAccess, Usr.MenuAccess, Usr.PingMethods, Usr.IncidentMethods, Usr.LocationAction, Usr.GroupAction, Usr.DepartmentAction,
                    "", "", "", "", "", "", "USERIMPORTCOMPLETE", userId, TimeZoneId);
                }

               await QueueImportTask(sessionId, companyId, false, userId, jobType);

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

        public async Task<bool> CreateTempLocation(List<ImportDumpInput> locData, string sessionId, int companyId, int userId = 0, string timeZoneId = "GMT Standard Time")
        {

            try
            {
                foreach (ImportDumpInput Loc in locData)
                {

                    string Action = !string.IsNullOrEmpty(Loc.Action) ? Loc.Action : "ADD";

                   await ImportToDump(userId, companyId, sessionId, "", "", "", "", "", "", "", "", "0",
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

        public async Task<bool> CreateTempGroup(List<ImportDumpInput> data, string sessionId, int companyId, int userId, string timeZoneId)
        {

            try
            {
                foreach (ImportDumpInput Dep in data)
                {

                    string Action = !string.IsNullOrEmpty(Dep.Action) ? Dep.Action : "ADD";
                   await ImportToDump(userId, companyId, sessionId,
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

                var ImportActionCheck = await _context.Set<ImportDump>()
                                         .Where(UIT=> UIT.SessionId == sessionId)
                                         .Select(UIT=> new { ActionCheck = UIT.ActionCheck, ActionType = UIT.ActionType }).ToListAsync();
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

                    var DelImprtData = (from UIT in _context.Set<ImportDump>()
                                        where UIT.SessionId == sessionId
                                        select UIT).ToList();
                    try
                    {

                        string ResultFilePath =await _DBC.Getconfig("ImportResultPath");
                        string FileName = outUserCompanyId + "\\" + sessionId.TrimStart('{').TrimEnd('}') + "\\import_log_report.csv";
                        bool fileCreated = DataHelper.CreateImportResult(DelImprtData, ResultFilePath + FileName, ImportType);
                        if (fileCreated)
                            ImpDTO.ResultFile = FileName;

                        var DelRecs = (from UIT in _context.Set<ImportDump>()
                                       where UIT.CreatedOn <= DelTime
                                       select UIT).ToList();
                        _context.Set<ImportDump>().RemoveRange(DelRecs);
                        _context.SaveChanges();

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
