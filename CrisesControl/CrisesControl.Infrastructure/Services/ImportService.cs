using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Common;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Groups.Repositories;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Services;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Locations.Services;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace CrisesControl.Infrastructure.Services
{
    public class ImportService : IImportService
    {
        string NewCheck = "NewAdded";
        string OverrideCheck = "Updated";
        string SkipCheck = "Skiped";
        string DeletedCheck = "Deleted";

        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserImportJob _UIK;
        private static DBCommon _DBC;
        private static SendEmail _SDE;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private static string OtherNameRegPattern;

        private readonly int companyId;
        private readonly int userId;

        private List<string> UserLocList = new List<string>();
        private List<string> UserDepList = new List<string>();
        private string userRole = "ADMIN";
        private string showAllLoc = "true";
        private string showAllDep = "true";
        private string updateRole = "false";
        private int sesCompanyId = 0;
        public int importUserId = -1;
        public static int colCount = 0;


        public ImportService(CrisesControlContext context,
            IHttpContextAccessor httpContextAccessor,
            DBCommon DBC,
            SendEmail SDE,
            UserImportJob UIK,
            ILocationRepository locationRepository,
            IUserRepository userRepository,
            IGroupRepository groupRepository,
            IDepartmentRepository departmentRepository
            )
        {
            _context = context;
            _DBC = DBC;
            _SDE = SDE;
            _UIK = UIK;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _departmentRepository = departmentRepository;
            //OtherNameRegPattern = _DBC.LookupWithKey("OTHER_NAME_REG_PATTERN");
            userId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
            companyId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
        }
        public static string ExtendedNames(string FieldName, string str, out bool DataError)
        {
            string rtnString = string.Empty;
            DataError = false;
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    if (!Regex.IsMatch(str, @OtherNameRegPattern, RegexOptions.IgnoreCase))
                    {
                        DataError = true;
                        rtnString += FieldName + ": Invalid characters. Only letters numbers and some following -_()&#., are allowed." + Environment.NewLine;
                    }
                    if (!DataError)
                    {
                        bool isLetter = Char.IsLetterOrDigit(str[0]);
                        if (!isLetter)
                        {
                            DataError = true;
                            rtnString += FieldName + ": First letter must be an alphabet" + Environment.NewLine;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return FieldName + ": ERROR";
            }
            return rtnString;
        }

        public static string ValidateLength(string fieldName, string str, int minLength, int maxLength, out bool mandatoryError, out bool dataError)
        {
            try
            {
                string rtnString = string.Empty;
                mandatoryError = dataError = false;

                if (minLength > 0)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = str.Trim();
                        bool lengthCheck = _DBC.verifyLength(str, minLength, maxLength);
                        if (!lengthCheck)
                        {
                            mandatoryError = dataError = true;
                            rtnString += fieldName + ": Invalid length, length should be between " + minLength + " - " + maxLength + Environment.NewLine;
                        }
                    }
                    else
                    {
                        mandatoryError = true;
                        rtnString += fieldName + ": Cannot be left blank" + Environment.NewLine;
                    }
                }

                if (!string.IsNullOrEmpty(str))
                {
                    if (str.Length > maxLength && !mandatoryError)
                    {
                        dataError = true;
                        rtnString += fieldName + ": Invalid length, length should be between " + minLength + " - " + maxLength + Environment.NewLine;
                    }
                }

                return rtnString;
            }
            catch (Exception ex)
            {
                mandatoryError = dataError = true;
                return fieldName + ": ERROR";
            }

        }

        public static string ValidateAction(string fieldName, string str, out bool dataError)
        {
            string rtnString = string.Empty;
            dataError = false;
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    string[] validActions = { "ADD", "UPDATE", "DELETE" };
                    if (!validActions.Contains(str))
                    {
                        dataError = true;
                        rtnString += fieldName + ": (" + str + "): is invalid." + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                dataError = true;
                return fieldName + ": ERROR";
            }

            return rtnString;
        }

        public static string ValidateStatus(string fieldName, string str, out bool dataError)
        {
            string rtnString = string.Empty;
            dataError = false;
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    string[] validActions = { "ACTIVE", "INACTIVE", "0", "1", "NO", "YES", "TRUE", "FALSE", "IN-ACTIVE", "IN ACTIVE", "DELETE", "PENDING VERIFICATION" };
                    if (!validActions.Contains(str.Trim().ToUpper()))
                    {
                        dataError = true;
                        rtnString += fieldName + ": (" + str + "): is invalid." + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                dataError = true;
                return fieldName + ": ERROR";
            }
            return rtnString;
        }

        public static int GetStatusValue(string str, string ModuleType = "USER")
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    string[] postiveActions = { "ACTIVE", "1", "YES", "TRUE" };
                    string[] negetiveActions = { "INACTIVE", "0", "NO", "FALSE", "IN-ACTIVE", "IN ACTIVE" };
                    str = str.Trim().ToUpper();

                    if (postiveActions.Contains(str))
                    {
                        return 1;
                    }
                    else if (negetiveActions.Contains(str))
                    {
                        return 0;
                    }
                    else if (str == "DELETE")
                    {
                        return 3;
                    }
                    else if (str == "PENDING VERIFICATION" && ModuleType == "USER")
                    {
                        return 2;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 1;
        }

        public bool createLocationData(string sessionId, int companyId, int currentUserId)
        {

            try
            {
                var impLocData = _context.Set<ImportDump>().Where(t => t.SessionId == sessionId).ToList();

                if (impLocData.Count > 0)
                {
                    foreach (var item in impLocData)
                    {
                        string LocationName = item.Location;
                        string LocationRec = string.Empty;
                        string ValidationMessage = string.Empty;
                        int LocationId;
                        int HasError = 0;
                        bool DataError = false;
                        bool MandatoryError = false;

                        //validateion the location all.
                        List<UserImportDataValidation> UIDV = new List<UserImportDataValidation>();

                        string locnameformat = ImportService.ExtendedNames("Location", item.Location, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Location", MandatoryError = MandatoryError, DataError = DataError, Message = locnameformat });
                        string locname = ImportService.ValidateLength("Location", item.Location, 3, 150, out MandatoryError, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Location", MandatoryError = MandatoryError, DataError = DataError, Message = locname });

                        string locaddformat = ExtendedNames("Location Address", item.LocationAddress, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Location Address", MandatoryError = MandatoryError, DataError = DataError, Message = locaddformat });
                        string locadd = ValidateLength("Location Address", item.LocationAddress, 3, 250, out MandatoryError, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Location Address", MandatoryError = MandatoryError, DataError = DataError, Message = locadd });

                        string action = ValidateAction("Action", item.Action, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Action", MandatoryError = false, DataError = DataError, Message = action });

                        string status = ValidateStatus("Status", item.LocationStatus, out DataError);
                        UIDV.Add(new UserImportDataValidation { FieldName = "Status", MandatoryError = false, DataError = DataError, Message = status });

                        string MultiLocation = string.Empty;
                        if (MultiOptionCheck(item.Location))
                        {
                            HasError++;
                            MultiLocation = "Multiple locations found, seprate them per line" + Environment.NewLine;
                        }

                        LocationRec = ImportCheckLocationExist(companyId, LocationName, sessionId, out LocationId);

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

                        if (LocationRec == "NEW" && item.Action == "DELETE")
                        {
                            ValidationMessage = "Location does not exist in database. Cannot delete." + Environment.NewLine;
                            HasError++;
                        }

                        if (totalMandatoryErrors > 0 && totalDataErrors == 0 && LocationRec == "DUPLICATE" && item.Action == "DELETE")
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
                            if (LocationRec == "DUPLICATE")
                            {
                                if (HasError != -100)
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

                        item.LocationId = LocationId;
                        item.LocationCheck = LocationRec;
                        _context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private string ImportCheckLocationExist(int CompanyId, string LocationName, string SessionId, out int LocationId)
        {
            LocationId = 0;
            if (!string.IsNullOrEmpty(LocationName))
            {
                var IsLocationExist = (from L in _context.Set<Location>() where L.LocationName == LocationName && L.CompanyId == CompanyId select L).FirstOrDefault();

                if (IsLocationExist != null)
                {
                    LocationId = IsLocationExist.LocationId;
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

        public void processLocation(List<ImportDumpData> locData, string sessionId, int companyId, int userId = 0, string timeZoneId = "GMT Standard Time", CancellationToken cancellationToken = default)
        {
            try
            {
                if (sessionId != null)
                {
                    sesCompanyId = companyId;
                    //Create Temporary location data
                    createTempLocation(locData, sessionId, companyId, userId, timeZoneId);

                    //Call the process for creating the import table record
                    createLocationData(sessionId, companyId, userId);

                    //Start Importing
                    var importLocList = _context.Set<ImportDump>().Where(t => t.SessionId == sessionId).ToList();
                    foreach (var rec in importLocList)
                    {
                        ImportLocation(rec.ImportDumpId, rec.CompanyId, rec.UserId, timeZoneId);
                        _UIK.MoveDumpToLog(rec.ImportDumpId);
                    }

                    _UIK.MoveErrorItemsToLog(sessionId, companyId, userId);
                    //Send the import CSV report to email.
                    _UIK.SendReportToEmail(sessionId, companyId, userId);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void processGroup(List<ImportDumpData> grpData, string SessionId, int CompanyId, int UserId = 0, string TimeZoneId = "GMT Standard Time")
        {

            try
            {
                if (SessionId != null)
                {
                    sesCompanyId = CompanyId;

                    //Create Temporary Groups
                    createTempGroup(grpData, SessionId, CompanyId, UserId, TimeZoneId);

                    //Craete Group Import data
                    createGroupData(SessionId, CompanyId, UserId);

                    //Start Importing
                    var DeptList = (from L in _context.Set<ImportDump>() where L.SessionId == SessionId select L).ToList();
                    foreach (var rec in DeptList)
                    {
                        ImportGroup(rec.ImportDumpId, rec.CompanyId, rec.UserId);
                        _UIK.MoveDumpToLog(rec.ImportDumpId);
                    }

                    _UIK.MoveErrorItemsToLog(SessionId, CompanyId, UserId);
                    //Send the import CSV report to email.
                    _UIK.SendReportToEmail(SessionId, CompanyId, UserId);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<CommonDTO> ImportGroup(int ImportRecId, int CompanyId, int CurrentUserId = 0, string TimeZoneId = "GMT Standard Time")
        {

            CommonDTO Result = new CommonDTO();
            try
            {
                string UpdateActionMessage = string.Empty;
                var UploadData = (from UIT in _context.Set<ImportDump>() where UIT.ImportDumpId == ImportRecId select UIT).FirstOrDefault();
                if (UploadData != null)
                {
                    if (UploadData.ActionCheck != "ERROR")
                    {
                        int NewAddedGroupId = 0;
                        int Status = ImportService.GetStatusValue(UploadData.GroupStatus, "GROUP");

                        if (UploadData.GroupCheck.ToUpper() == "NEW" &&
                            (UploadData.Action.ToUpper() == "ADD" || UploadData.Action.ToUpper() == "UPDATE"))
                        {
                            Core.Groups.Group newGroup = new Core.Groups.Group();
                            newGroup.CompanyId = UploadData.CompanyId;
                            newGroup.GroupName = UploadData.Group;
                            newGroup.Status = Status;
                            newGroup.CreatedBy = userId;
                            newGroup.UpdatedBy = userId;
                            NewAddedGroupId = await _groupRepository.CreateGroup(newGroup, CancellationToken.None);
                            UploadData.ActionCheck = NewCheck;

                            UpdateActionMessage += "New group created" + Environment.NewLine;

                        }
                        else if (UploadData.GroupCheck.ToUpper() == "DUPLICATE" || UploadData.GroupCheck.ToUpper() == "OK")
                        { //Existing group update

                            if (UploadData.Action.ToUpper() == "DELETE")
                            {
                                var chkdept = _groupRepository.DeleteGroup((int)UploadData.GroupId, CancellationToken.None);
                                if (_DBC.IsPropertyExist(chkdept, "ErrorId"))
                                {
                                    UpdateActionMessage += "Group associated with incident task and cannot be deleted." + Environment.NewLine;
                                    UploadData.ActionCheck = SkipCheck;
                                }
                                else
                                {
                                    UpdateActionMessage += "Group is deleted" + Environment.NewLine;
                                    UploadData.ActionCheck = DeletedCheck;
                                }

                            }
                            else if (UploadData.Action.ToUpper() == "UPDATE" || UploadData.Action.ToUpper() == "ADD")
                            {  //update an existing group

                                var depatUpdate = (from D in _context.Set<Core.Groups.Group>()
                                                   where D.GroupId == UploadData.GroupId
                                                   select D).FirstOrDefault();
                                if (depatUpdate != null)
                                {
                                    depatUpdate.Status = Status;
                                    depatUpdate.UpdatedBy = CurrentUserId;
                                    depatUpdate.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                                    await _context.SaveChangesAsync();
                                    NewAddedGroupId = depatUpdate.GroupId;
                                    UploadData.ActionCheck = OverrideCheck;
                                    UpdateActionMessage += "Group details updated" + Environment.NewLine;
                                }

                            }
                            else if (UploadData.ImportAction.ToUpper() == "SKIP")
                            {
                                UploadData.ActionCheck = SkipCheck;
                                UpdateActionMessage += "Group update skipped" + Environment.NewLine;
                            }

                        }

                        UploadData.ValidationMessage = UpdateActionMessage;

                        UploadData.ImportAction = "Imported";
                        await _context.SaveChangesAsync();
                        Result.ErrorId = 0;
                        Result.Message = UpdateActionMessage;

                    }
                    else
                    {
                        Result.ErrorId = 100;
                        Result.Message = UploadData.ValidationMessage;
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

        public bool createTempGroup(List<ImportDumpData> data, string SessionId, int CompanyId, int UserId, string TimeZoneId)
        {

            try
            {
                foreach (ImportDumpData Dep in data)
                {

                    string Action = !string.IsNullOrEmpty(Dep.Action) ? Dep.Action : "ADD";
                    ImportToDump(UserId, CompanyId, SessionId,
                        "", "", "", "", "", "", "", "", "0", Action, Dep.Group, Dep.Status, "", "0", "", "", "0", "", "", "", "", "", "",
                        "", "", "", "", "", "", "", "GroupImportOnly", UserId, TimeZoneId);

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool createGroupData(string SessionId, int CompanyId, int UserId)
        {

            try
            {
                var ImpDepData = (from Dep in _context.Set<ImportDump>() where Dep.SessionId == SessionId select Dep).ToList();
                if (ImpDepData.Count > 0)
                {

                    foreach (var item in ImpDepData)
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

                        GroupRec = ImportCheckGroupExist(CompanyId, item.Group, SessionId, out GroupId);

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
                        _context.SaveChanges();

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string ImportCheckGroupExist(int CompanyId, string GroupName, string SessionId, out int GroupId)
        {
            GroupId = 0;
            if (!string.IsNullOrEmpty(GroupName))
            {
                var IsGroupExist = (from D in _context.Set<Core.Models.Group>() where D.GroupName == GroupName && D.CompanyId == CompanyId select D).FirstOrDefault();

                if (IsGroupExist != null)
                {
                    GroupId = IsGroupExist.GroupId;
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

        public async Task<CommonDTO> ImportLocation(int ImportRecId, int CompanyId, int CurrentUserId = 0, string TimeZoneId = "GMT Standard Time", CancellationToken cancellationToken = default)
        {
            CommonDTO Result = new CommonDTO();
            try
            {
                string UpdateActionMessage = string.Empty;
                var uploadData = _context.Set<ImportDump>().Where(t => t.ImportDumpId == ImportRecId).FirstOrDefault();
                if (uploadData != null)
                {
                    if (uploadData.ActionCheck != "ERROR")
                    {
                        int NewAddedLocationId = 0;

                        int Status = GetStatusValue(uploadData.LocationStatus, "LOCATION");


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
                            newLocation.PostCode = uploadData.LocationAddress;
                            newLocation.Status = Status;
                            newLocation.CreatedBy = userId;
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

                                var locUpdate = (from L in _context.Set<Location>()
                                                 where L.LocationId == uploadData.LocationId
                                                 select L).FirstOrDefault();
                                if (locUpdate != null)
                                {
                                    locUpdate.Status = Status;
                                    locUpdate.PostCode = _DBC.Left(uploadData.LocationAddress, 150);
                                    locUpdate.Lat = _DBC.Left(LL.Lat, 15);
                                    locUpdate.Long = _DBC.Left(LL.Lng, 15);
                                    locUpdate.UpdatedBy = CurrentUserId;
                                    locUpdate.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                                    await _context.SaveChangesAsync(cancellationToken);
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

        public bool createTempLocation(List<ImportDumpData> locData, string SessionId, int CompanyId, int UserId = 0, string TimeZoneId = "GMT Standard Time")
        {

            try
            {
                foreach (ImportDumpData Loc in locData)
                {

                    string Action = !string.IsNullOrEmpty(Loc.Action) ? Loc.Action : "ADD";

                    ImportToDump(UserId, CompanyId, SessionId, "", "", "", "", "", "", "", "", "0",
                        Action, "", "0", "", "0", Loc.Location, Loc.LocationAddress, Loc.LocationStatus, "", "", "", "", "", "", "", "",
                        "", "", "", "", "", "LocationImportOnly", UserId, TimeZoneId);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void ImportToDump(int UserId, int CompanyId, string SessionId,
           string FirstName, string Surname, string Email, string ISD, string Phone, string LLISD, string Landline, string UserRole, string Status, string Action,
           string Group, string GroupStatus,
           string Department, string DepartmentStatus,
           string Location, string LocationAddress, string LocationStatus,
           string Security, string SecurityDescription,
           string PingMethods, string IncidentMethods,
           string LocationAction, string GroupAction, string DepartmentAction,
           string EmailCheck, string LocationCheck, string GroupCheck, string DepartmentCheck, string SecurityCheck, string ImportAction, string ActionType, int createdUpdatedBy, string TimeZoneId)
        {
            try
            {

                ImportDump IMPDump = new ImportDump();

                IMPDump.UserId = UserId;
                IMPDump.CompanyId = CompanyId;
                IMPDump.SessionId = SessionId;

                if (ActionType.ToUpper() == "USERIMPORTONLY" || ActionType.ToUpper() == "USERIMPORTCOMPLETE")
                {
                    IMPDump.FirstName = FirstName;
                    IMPDump.Surname = Surname;
                    IMPDump.Email = (!string.IsNullOrEmpty(Email) ? Email.ToLower() : "");
                    IMPDump.EncryptedEmail = (!string.IsNullOrEmpty(Email) ? Email.ToLower() : "");
                    IMPDump.Status = Status;
                    IMPDump.Isd = ISD;
                    IMPDump.Phone = Phone;
                    IMPDump.Llisd = LLISD;
                    IMPDump.Landline = Landline;
                    IMPDump.UserRole = (!string.IsNullOrEmpty(UserRole) ? UserRole.ToUpper() : "");
                    IMPDump.EmailCheck = EmailCheck;
                    IMPDump.PingMethods = PingMethods;
                    IMPDump.IncidentMethods = IncidentMethods;
                    IMPDump.LocationAction = LocationAction;
                    IMPDump.GroupAction = GroupAction;
                    IMPDump.ActionCheck = "";
                }

                if (ActionType.ToUpper() == "GROUPIMPORTONLY" || ActionType.ToUpper() == "USERIMPORTCOMPLETE")
                {
                    IMPDump.Group = Group;
                    IMPDump.GroupStatus = GroupStatus;
                    IMPDump.GroupCheck = GroupCheck;
                }
                if (ActionType.ToUpper() == "DEPARTMENTIMPORTONLY" || ActionType.ToUpper() == "USERIMPORTCOMPLETE")
                {
                    IMPDump.Department = Department;
                    IMPDump.DepartmentStatus = DepartmentStatus;
                    IMPDump.DepartmentCheck = DepartmentCheck;
                }
                if (ActionType.ToUpper() == "LOCATIONIMPORTONLY" || ActionType.ToUpper() == "USERIMPORTCOMPLETE")
                {
                    IMPDump.Location = Location;
                    IMPDump.LocationAddress = LocationAddress;
                    IMPDump.LocationStatus = LocationStatus;
                    IMPDump.LocationCheck = LocationCheck;

                }
                if (ActionType.ToUpper() == "SECURITYIMPORTONLY" || ActionType.ToUpper() == "USERIMPORTCOMPLETE")
                {
                    IMPDump.Security = Security;
                    IMPDump.SecurityCheck = SecurityCheck;
                }

                IMPDump.ImportAction = ImportAction;
                IMPDump.ActionType = ActionType;
                IMPDump.Action = Action;
                if (createdUpdatedBy > 0)
                    IMPDump.CreatedBy = createdUpdatedBy;
                IMPDump.CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                if (createdUpdatedBy > 0)
                    IMPDump.UpdatedBy = createdUpdatedBy;
                IMPDump.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                _context.Set<ImportDump>().Add(IMPDump);
                _context.SaveChanges();
            }
            catch (Exception ex) { throw ex; }
        }

        public static DataTable ReadCSVFile(string filePath, string columnDelimiter, string columnMappingFilePath, string columnMappingFileType, bool fileHasHeader)
        {
            DataTable csvData = new DataTable();
            try
            {

                _DBC.connectUNCPath();

                Encoding enc = TextFileEncodingDetector.DetectTextFileEncoding(filePath);

                if (enc == null)
                {
                    enc = Encoding.GetEncoding("ISO-8859-1");
                }

                using (TextFieldParser csvReader = new TextFieldParser(filePath, enc, false))
                {
                    csvReader.Delimiters = new string[] { columnDelimiter };
                    csvReader.TrimWhiteSpace = true;
                    csvReader.HasFieldsEnclosedInQuotes = true;

                    //Get column name list from the xml or json as provided
                    List<DataMappingColumn> colfields = _getDataColumns(columnMappingFilePath, columnMappingFileType);
                    colCount = colfields.Count;

                    //Creating Datatable with the data column mapping  fileds
                    foreach (DataMappingColumn column in colfields)
                    {
                        DataColumn datacolumn = new DataColumn(column.ColumnName);
                        csvData.Columns.Add(datacolumn);
                    }
                    bool FileHeaderSkipped = false;

                    //Pushing the csv data in to the data table
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();

                        if (fileHasHeader && !FileHeaderSkipped)
                        {
                            FileHeaderSkipped = true;
                            continue;
                        }

                        DataRow dr = csvData.NewRow();
                        int colcnt = 0;
                        foreach (DataMappingColumn column in colfields)
                        {
                            string dtvalue = "";

                            if (column.ColumnIndex == 9999)
                            {
                                dtvalue = "ADD";
                            }
                            else
                            {
                                if (fieldData[column.ColumnIndex] == "")
                                {
                                    dtvalue = null;
                                }
                                else
                                {
                                    dtvalue = _DBC.PureAscii(fieldData[column.ColumnIndex], true);
                                }
                            }
                            //int dtIntVal = 0;
                            bool dtboolVal = false;

                            if (column.DataType == "bool" && FileHeaderSkipped == true)
                            {
                                Boolean.TryParse(dtvalue, out dtboolVal);
                                dr[colcnt] = dtboolVal;
                            }
                            else if (column.DataType == "int" && FileHeaderSkipped == true)
                            {

                            }
                            else
                            {
                                dr[colcnt] = dtvalue;
                            }
                            colcnt++;
                        }

                        csvData.Rows.Add(dr);
                    }

                }
            }
            catch (Exception ex)
            {
            }
            return csvData;
        }

        private static List<DataMappingColumn> _getDataColumns(string ColumnMappingFilePath, string ColumnMappingFileType)
        {
            List<DataMappingColumn> dc = new List<DataMappingColumn>();
            try
            {

                _DBC.connectUNCPath();

                if (ColumnMappingFileType == "XML")
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(@ColumnMappingFilePath);

                    XmlNode parentnode = doc.DocumentElement.SelectSingleNode("/Columns");

                    if (parentnode != null)
                    {
                        bool HasActionColumn = false;
                        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                        {
                            string ColumnName = node.Attributes["ColumnName"].InnerText != null ? node.Attributes["ColumnName"].InnerText : "";
                            if (ColumnName == "Action")
                                HasActionColumn = true;

                            if (ColumnName != "NOMAP" && !string.IsNullOrEmpty(ColumnName))
                            {
                                string ColumnValue = node.Attributes["ColumnValue"].InnerText != null ? node.Attributes["ColumnValue"].InnerText : "";
                                string DataType = node.Attributes["DataType"].InnerText != null ? node.Attributes["DataType"].InnerText : "";
                                string isRequired = node.Attributes["isRequired"].InnerText != null ? node.Attributes["isRequired"].InnerText : "";
                                string ColumnIndex = node.Attributes["ColumnIndex"].InnerText != null ? node.Attributes["ColumnIndex"].InnerText : "";

                                dc.Add(new DataMappingColumn() { ColumnName = ColumnName, ColumnValue = ColumnValue, DataType = DataType, isRequired = Convert.ToBoolean(isRequired), ColumnIndex = Convert.ToInt16(ColumnIndex) });
                            }
                        }

                        if (!HasActionColumn)
                        {
                            dc.Add(new DataMappingColumn() { ColumnName = "Action", ColumnValue = "Action", DataType = "string", isRequired = false, ColumnIndex = 9999 });
                        }

                    }
                }
                else if (ColumnMappingFileType == "JSON")
                {
                    using (StreamReader file = File.OpenText(@ColumnMappingFilePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        string jsonString = file.ReadToEnd().ToString();
                        dc = JsonConvert.DeserializeObject<List<DataMappingColumn>>(jsonString);
                    }
                }
            }
            catch (XmlException ex)
            {
            }
            return dc;
        }

        public void CheckDataSegregation(int CompanyId, int UserId)
        {
            showAllLoc = _DBC.GetCompanyParameter("SHOW_ALL_LOCATIONS_TO_STAFF", CompanyId);
            showAllDep = _DBC.GetCompanyParameter("SHOW_ALL_GROUPS_TO_STAFF", CompanyId);
            updateRole = _DBC.GetCompanyParameter("ALLOW_KEYHOLDER_DEMOTE", CompanyId);

            if (showAllLoc == "false" || showAllDep == "false")
            {
                var user_role = (from U in _context.Set<User>() where U.UserId == UserId select U.UserRole).FirstOrDefault();
                if (user_role != null)
                {
                    userRole = user_role.ToUpper();

                    if (userRole != "ADMIN")
                    {
                        if (showAllLoc == "false")
                        {
                            UserLocList = (from UL in _context.Set<UserLocation>() where UL.UserId == UserId where UL.LocationName != "ALL" select UL.LocationName).ToList();
                        }

                        if (showAllDep == "false")
                        {
                            UserDepList = (from UL in _context.Set<UserGroup>() where UL.UserId == UserId where UL.GroupName != "ALL" select UL.GroupName).ToList();
                        }
                    }
                }
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

        public void ReleaseImportQueue(int CompanyId, string JobType)
        {
            try
            {

                var queued_import = (from IM in _context.Set<ImportDumpHeader>()
                                     where IM.CompanyId == CompanyId && IM.JobType == JobType && IM.Status == "WAITING"
                                     select IM).FirstOrDefault();
                if (queued_import != null)
                {
                    CreateImportHeader(queued_import.SessionId, CompanyId, "DUMPED", queued_import.CreatedBy, "NA", "NA", (bool)queued_import.SendInvite, JobType: queued_import.JobType);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public async Task<CommonDTO> ImportUsers(int ImportRecId, int CompanyId, int CurrentUserId = 0, string TimeZoneId = "GMT Standard Time", bool SendInvite = false, bool AutoForceVerify = false)
        {

            CommonDTO Result = new CommonDTO();
            try
            {

                //DBC.CreateLog("INFO", "ImportHelper: Import to the desination table", null, "ImportHelper", "ImportUsers", CompanyId);

                int UserIdToUpdate = -1;

                var UploadData = (from UIT in _context.Set<ImportDump>() where UIT.ImportDumpId == ImportRecId select UIT).FirstOrDefault();
                if (UploadData != null)
                {

                    UploadData.Email = UploadData.Email.ToLower();
                    UserIdToUpdate = UploadData.UserId;

                    int UserStatus = AutoForceVerify == true ? 1 : 2;
                    bool FirstLogin = AutoForceVerify == true ? false : true;


                    if (UploadData.ActionCheck != "ERROR" && UploadData.EmailCheck != null)
                    {

                        bool IsNew = false;
                        int GroupToUpdate = 0, DepartmentToUpdate = 0, LocationToUpdate = 0;
                        string UniqId = Guid.NewGuid().ToString();
                        string UpdateActionMessage = string.Empty;

                        int Status = GetStatusValue(UploadData.Status);

                        if (UploadData.EmailCheck.ToUpper() == "NEW" &&
                            (UploadData.Action.ToUpper() == "ADD" || UploadData.Action.ToUpper() == "UPDATE"))
                        { //Create new user and get userid

                            IsNew = true;

                            string newPwd = _DBC.RandomPassword();


                            //DBC.CreateLog("INFO", "ImportHelper: Inserting the user to db", null, "ImportHelper", "ImportUsers", CompanyId);

                            //Add the user and get the userid
                            User newUser = new User();
                            newUser.CompanyId = UploadData.CompanyId;
                            newUser.RegisteredUser = false;
                            newUser.FirstName = UploadData.FirstName;
                            newUser.PrimaryEmail = UploadData.Email;
                            newUser.Password = newPwd;
                            newUser.Status = UserStatus;
                            newUser.CreatedBy = userId;
                            newUser.LastName = UploadData.Surname;
                            newUser.MobileNo = UploadData.Phone;
                            newUser.UserRole = UploadData.UserRole;
                            newUser.Isdcode = UploadData.Isd;
                            newUser.Llisdcode = UploadData.Llisd;
                            newUser.Landline = UploadData.Landline;
                            newUser.UniqueGuiId = UniqId;
                            newUser.ExpirePassword = true;
                            newUser.Smstrigger = false;
                            newUser.FirstLogin = FirstLogin;
                            UserIdToUpdate = await _userRepository.CreateUser(newUser, CancellationToken.None);

                            //DBC.CreateLog("INFO", "ImportHelper: New user created userid " + UserIdToUpdate.ToString(), null, "ImportHelper", "ImportUsers", CompanyId);

                            if (UserIdToUpdate <= 0)
                            {
                                Result.ErrorId = 110;
                                Result.Message = "Email id already exist";
                                return Result;
                            }

                            //Creating default relation with the ALL groups
                            _DBC.CreateObjectRelationship(UserIdToUpdate, 0, "LOCATION", UploadData.CompanyId, CurrentUserId, TimeZoneId, "ALL");
                            _DBC.CreateObjectRelationship(UserIdToUpdate, 0, "GROUP", UploadData.CompanyId, CurrentUserId, TimeZoneId, "ALL");

                            UploadData.ActionCheck = NewCheck;
                            //DBC.UpdateUserRoleChangeLog("ADD", UploadData.CompanyId, UserIdToUpdate, CurrentUserId, UploadData.UserRole);

                            UpdateActionMessage += "New Account has been created" + Environment.NewLine;


                            //_billing.AddUserRoleChange(UploadData.CompanyId, UserIdToUpdate, UploadData.UserRole, TimeZoneId);


                        }
                        else if (UploadData.EmailCheck.ToUpper() == "DUPLICATE" || UploadData.EmailCheck.ToUpper() == "OK")
                        { //Existing user update

                            var userUpdate = (from U in _context.Set<User>()
                                              where U.PrimaryEmail == UploadData.Email
                                              && U.CompanyId == UploadData.CompanyId
                                              select U).FirstOrDefault();

                            if (userUpdate != null)
                            {
                                UserIdToUpdate = userUpdate.UserId;

                                if (UploadData.Action.ToUpper() == "DELETE")
                                {
                                    if (userUpdate.RegisteredUser != true)
                                    {
                                        var user = _context.Set<User>().Where(t => t.UserId == userUpdate.UserId).FirstOrDefault();
                                        _userRepository.DeleteUser(user, CancellationToken.None);
                                        UploadData.ActionCheck = DeletedCheck;
                                        UpdateActionMessage += "User has been deleted" + Environment.NewLine;
                                    }
                                }
                                else if (UploadData.Action.ToUpper() == "UPDATE" || UploadData.Action.ToUpper() == "ADD")
                                {  //update an existing user
                                    if (!string.IsNullOrEmpty(UploadData.FirstName))
                                        userUpdate.FirstName = UploadData.FirstName;

                                    if (!string.IsNullOrEmpty(UploadData.Surname))
                                        userUpdate.LastName = UploadData.Surname;

                                    if (!string.IsNullOrEmpty(UploadData.Isd))
                                        userUpdate.Isdcode = _DBC.Left(UploadData.Isd, 1) != "+" ? "+" + UploadData.Isd : UploadData.Isd;

                                    string DummyNumber = _DBC.GetCompanyParameter("DUMMY_PHONE_NUMBER", UploadData.CompanyId);
                                    if (string.IsNullOrEmpty(UploadData.Phone) || UploadData.Phone == DummyNumber)
                                    { // when source is empty or dummy
                                        if (string.IsNullOrWhiteSpace(userUpdate.MobileNo) || userUpdate.MobileNo == DummyNumber)
                                        {
                                            userUpdate.MobileNo = _DBC.FixMobileZero(DummyNumber);
                                        }
                                    }
                                    else
                                    { //When the source has valid number
                                        string OverridePhone = _DBC.GetCompanyParameter("OVERRIDE_PHONE_NUMBER", UploadData.CompanyId);
                                        string OverrideDummy = _DBC.GetCompanyParameter("OVERRIDE_DUMMY_NUMBER_ONLY", UploadData.CompanyId);
                                        if (OverridePhone == "true")
                                        {
                                            if (OverrideDummy == "true")
                                            { //only update dummy numbers when turned on.
                                                if (string.IsNullOrEmpty(userUpdate.MobileNo) || userUpdate.MobileNo == DummyNumber)
                                                {
                                                    userUpdate.MobileNo = _DBC.FixMobileZero(UploadData.Phone);
                                                }
                                            }
                                            else
                                            { // update the non number for all customers.
                                                userUpdate.MobileNo = _DBC.FixMobileZero(UploadData.Phone);
                                            }
                                        } // skip mobile number update.
                                    }

                                    if (!string.IsNullOrEmpty(UploadData.Llisd))
                                        userUpdate.Llisdcode = _DBC.Left(UploadData.Llisd, 1) != "+" ? "+" + UploadData.Llisd : UploadData.Llisd;

                                    if (!string.IsNullOrEmpty(UploadData.Landline))
                                        userUpdate.Landline = _DBC.FixMobileZero(UploadData.Landline);

                                    if (!userUpdate.RegisteredUser)
                                    {
                                        if (!string.IsNullOrEmpty(UploadData.UserRole))
                                        {
                                            var roles = _DBC.CCRoles();
                                            if (updateRole == "true" && roles.Contains(userRole))
                                            {
                                                userUpdate.UserRole = UploadData.UserRole.ToUpper().Replace("STAFF", "USER");
                                            }
                                            else
                                            {
                                                UpdateActionMessage += "User Role change skipped" + Environment.NewLine;
                                            }

                                            //if(UserRole!="ADMIN" && new string[] {"KEHOLDER","ADMIN" }.Contains(userUpdate.UserRole.ToUpper()) && 
                                            //    UploadData.UserRole=="USER" && UpdateRole=="false" || (userUpdate.UserRole.ToUpper()=="ADMIN" && UserRole != "ADMIN")) {

                                            //    UpdateActionMessage += "User Role change skipped" + Environment.NewLine;
                                            //} else if((UserRole != "ADMIN" && UpdateRole == "true" && userUpdate.UserRole.ToUpper() != "ADMIN") ||
                                            //    (UserRole=="ADMIN")) {
                                            //    userUpdate.UserRole = UploadData.UserRole.ToUpper().Replace("STAFF", "USER");
                                            //}
                                        }

                                        if (AutoForceVerify == true && !string.IsNullOrEmpty(UploadData.Status))
                                            userUpdate.Status = Status;

                                        if (userUpdate.Status != 2 && AutoForceVerify == false && !string.IsNullOrEmpty(UploadData.Status))
                                            userUpdate.Status = Status;

                                        if (Status == 0 && !string.IsNullOrEmpty(UploadData.Status))
                                            _DBC.RemoveUserDevice(userUpdate.UserId);
                                    }

                                    if (CurrentUserId > 0)
                                        userUpdate.UpdatedBy = CurrentUserId;

                                    userUpdate.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                                    _context.SaveChanges();

                                    await _userRepository.CreateUserSearch(userUpdate.UserId, userUpdate.FirstName, userUpdate.LastName, userUpdate.Isdcode, userUpdate.MobileNo, userUpdate.PrimaryEmail, CompanyId);

                                    _userRepository.CreateSMSTriggerRight(CompanyId, userUpdate.UserId, userUpdate.UserRole, false, userUpdate.Isdcode, userUpdate.MobileNo, true);

                                    UploadData.ActionCheck = OverrideCheck;
                                    UpdateActionMessage += "Account details updated" + Environment.NewLine;
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
                                    _DBC.RemoveUserObjectRelation("LOCATION", UserIdToUpdate, (int)UploadData.LocationId, CompanyId, CurrentUserId, TimeZoneId);
                                    UpdateActionMessage += "Location removed from user profile" + Environment.NewLine;

                                }
                                else if (UploadData.LocationAction.ToUpper() == "ADD" || UploadData.LocationAction.ToUpper() == "UPDATE")
                                {

                                    //get the location coordinates
                                    LatLng LL = new LatLng();
                                    int LocStatus = (UploadData.LocationStatus == "INACTIVE" ? 0 : 1);
                                    if (string.IsNullOrEmpty(UploadData.LocLat) && string.IsNullOrEmpty(UploadData.LocLng))
                                    {
                                        LL = _DBC.GetCoordinates(UploadData.LocationAddress);
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
                                        Core.Locations.Location newLocation = new Core.Locations.Location();
                                        newLocation.CompanyId = UploadData.CompanyId;
                                        newLocation.LocationName = UploadData.Location;
                                        newLocation.Lat = LL.Lat;
                                        newLocation.Long = LL.Lng;
                                        newLocation.PostCode = UploadData.LocationAddress;
                                        newLocation.Status = LocStatus;
                                        newLocation.CreatedBy = CurrentUserId;
                                        newLocation.UpdatedBy = CurrentUserId;
                                        LocationToUpdate = await _locationRepository.CreateLocation(newLocation, CancellationToken.None);
                                        UpdateActionMessage += "New location created and assigned to user" + Environment.NewLine;
                                    }
                                    else if (UploadData.LocationCheck.ToUpper() == "DUPLICATE")
                                    {
                                        LocationToUpdate = (int)UploadData.LocationId;
                                        UpdateActionMessage += "Location assigned to user" + Environment.NewLine;
                                    }
                                    else if (UploadData.LocationCheck.ToUpper() == "ERROR")
                                    {
                                        UpdateActionMessage += "Location was not added" + Environment.NewLine;
                                    }

                                    if (UploadData.LocationCheck.ToUpper() != "ERROR")
                                        _DBC.CreateObjectRelationship(UserIdToUpdate, LocationToUpdate, "LOCATION", UploadData.CompanyId, CurrentUserId, TimeZoneId);
                                }
                            }

                            //Group Handeling
                            if (!string.IsNullOrEmpty(UploadData.GroupCheck))
                            {
                                if ((UploadData.GroupAction.ToUpper() == "DELETE" || UploadData.GroupAction.ToUpper() == "REMOVE") &&
                                UploadData.GroupCheck.ToUpper() == "DUPLICATE")
                                {
                                    //Remove the user from the location assignment
                                    _DBC.RemoveUserObjectRelation("GROUP", UserIdToUpdate, (int)UploadData.GroupId, CompanyId, CurrentUserId, TimeZoneId);
                                    UpdateActionMessage += "Group removed from user profile" + Environment.NewLine;

                                }
                                else if (UploadData.GroupAction.ToUpper() == "ADD" || UploadData.GroupAction.ToUpper() == "UPDATE")
                                {


                                    if (UploadData.GroupCheck.ToUpper() == "NEW")
                                    {
                                        //Create New Location if do not exist
                                        Core.Groups.Group newGroup = new Core.Groups.Group();
                                        newGroup.CompanyId = UploadData.CompanyId;
                                        newGroup.GroupName = UploadData.Group;
                                        newGroup.Status = 1;
                                        newGroup.CreatedBy = CurrentUserId;
                                        newGroup.UpdatedBy = CurrentUserId;
                                        GroupToUpdate = await _groupRepository.CreateGroup(newGroup, CancellationToken.None);

                                        UpdateActionMessage += "New department created and assigned to user" + Environment.NewLine;
                                    }
                                    else if (UploadData.GroupCheck.ToUpper() == "DUPLICATE")
                                    {
                                        GroupToUpdate = (int)UploadData.GroupId;
                                        UpdateActionMessage += "Group assigned to user" + Environment.NewLine;
                                    }
                                    else if (UploadData.GroupCheck.ToUpper() == "ERROR")
                                    {
                                        UpdateActionMessage += "Group was not added" + Environment.NewLine;
                                    }

                                    if (UploadData.GroupCheck.ToUpper() != "ERROR")
                                        _DBC.CreateObjectRelationship(UserIdToUpdate, GroupToUpdate, "GROUP", UploadData.CompanyId, CurrentUserId, TimeZoneId);
                                }
                            }

                            //Department Handeling
                            if (!string.IsNullOrEmpty(UploadData.DepartmentCheck))
                            {
                                if ((UploadData.DepartmentAction.ToUpper() == "DELETE" || UploadData.DepartmentAction.ToUpper() == "REMOVE") &&
                                UploadData.DepartmentCheck.ToUpper() == "DUPLICATE")
                                {
                                    //Remove the user from the location assignment
                                    _DBC.RemoveUserObjectRelation("DEPARTMENT", UserIdToUpdate, (int)UploadData.GroupId, CompanyId, CurrentUserId, TimeZoneId);
                                    UpdateActionMessage += "Department removed from user profile" + Environment.NewLine;
                                }
                                else if (UploadData.DepartmentAction.ToUpper() == "ADD" || UploadData.DepartmentAction.ToUpper() == "UPDATE")
                                {

                                    if (UploadData.DepartmentCheck.ToUpper() == "NEW")
                                    {
                                        //Create New Department if do not exist
                                        Department newDepartment = new Department();
                                        newDepartment.CompanyId = UploadData.CompanyId;
                                        newDepartment.DepartmentName = UploadData.Department;
                                        newDepartment.Status = 1;
                                        newDepartment.CreatedBy = CurrentUserId;
                                        newDepartment.UpdatedBy = CurrentUserId;
                                        DepartmentToUpdate = await _departmentRepository.CreateDepartment(newDepartment, CancellationToken.None);

                                        UpdateActionMessage += "New department created and assigned to user" + Environment.NewLine;
                                    }
                                    else if (UploadData.DepartmentCheck.ToUpper() == "DUPLICATE")
                                    {
                                        DepartmentToUpdate = (int)UploadData.DepartmentId;
                                        UpdateActionMessage += "Department assigned to user" + Environment.NewLine;
                                    }
                                    else if (UploadData.DepartmentCheck.ToUpper() == "ERROR")
                                    {
                                        UpdateActionMessage += "Department was not added" + Environment.NewLine;
                                    }

                                    if (UploadData.DepartmentCheck.ToUpper() != "ERROR")
                                        _DBC.CreateObjectRelationship(UserIdToUpdate, DepartmentToUpdate, "DEPARTMENT", UploadData.CompanyId, CurrentUserId, TimeZoneId);
                                }
                            }

                            //Security Group handeling
                            if (!string.IsNullOrEmpty(UploadData.SecurityCheck))
                            {
                                var roles = _DBC.CCRoles(true);
                                if (UploadData.SecurityCheck.ToUpper() == "DUPLICATE" && roles.Contains(userRole))
                                {

                                    if (UploadData.EmailCheck.ToUpper() == "NEW")
                                    {
                                        _userRepository.CreateUserSecurityGroup(UserIdToUpdate, (int)UploadData.SecurityGroupId, CurrentUserId, CompanyId, "Standard");
                                    }
                                    else
                                    {
                                        if (updateRole == "true")
                                        {
                                            var UpdateSecurity = (from USG in _context.Set<UserSecurityGroup>()
                                                                  where USG.UserId == UserIdToUpdate
                                                                  select USG).ToList();
                                            bool secItemExist = false;
                                            if (UpdateSecurity != null)
                                            {
                                                foreach (var secitem in UpdateSecurity)
                                                {
                                                    if (secitem.SecurityGroupId != UploadData.SecurityGroupId)
                                                    {
                                                        _context.Set<UserSecurityGroup>().Remove(secitem);
                                                        _context.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        secItemExist = true;
                                                    }
                                                }
                                                if (!secItemExist)
                                                    _userRepository.CreateUserSecurityGroup(UserIdToUpdate, (int)UploadData.SecurityGroupId, CurrentUserId, CompanyId, "Standard");

                                            }
                                            else
                                            {
                                                _userRepository.CreateUserSecurityGroup(UserIdToUpdate, (int)UploadData.SecurityGroupId, CurrentUserId, CompanyId, "Standard");
                                            }
                                            UpdateActionMessage += "Menu Access assigned to user" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            UpdateActionMessage += "Menu Access change skipped" + Environment.NewLine;
                                        }
                                    }
                                }
                            }

                            //Handle the Comms Method
                            _userRepository.UpdateUserComms(UploadData.CompanyId, UserIdToUpdate, CurrentUserId, TimeZoneId, UploadData.PingMethods, UploadData.IncidentMethods, IsNew);

                            if (SendInvite && UploadData.EmailCheck.ToUpper() == "NEW")
                            {
                                _SDE.NewUserAccount(UploadData.Email, UploadData.FirstName + " " + UploadData.Surname, UploadData.CompanyId, UniqId);
                            }
                        } // 
                        UploadData.ValidationMessage = UpdateActionMessage;

                        UploadData.ImportAction = "Imported";
                        UploadData.UserId = UserIdToUpdate;

                        _context.SaveChanges();
                        Result.ErrorId = 0;
                        Result.Message = UpdateActionMessage;
                        importUserId = UserIdToUpdate;
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
        public static List<ImportDumpData> DataTableToList<T>(DataTable table) where T : class, new()
        {
            try
            {
                List<ImportDumpData> list = new List<ImportDumpData>();

                foreach (var row in table.AsEnumerable())
                {
                    ImportDumpData obj = new ImportDumpData();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }
    }
}
