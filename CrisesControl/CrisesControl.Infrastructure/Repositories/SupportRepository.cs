using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Support;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class SupportRepository
    {
        private readonly CrisesControlContext _context;

        public SupportRepository(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<IncidentDataByActivationRefResponse> GetIncidentData(int incidentActivationId, int companyId)
        {
            try
            {
                var pIncidentActivationID = new SqlParameter("@IncidentActivationID", incidentActivationId);
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var incidentReportData = await _context.Set<IncidentDataByActivationRefResponse>().FromSqlRaw(
                    "EXEC Pro_Admin_Report_GetIncidentData_ByActivationRef @IncidentActivationID", pIncidentActivationID).FirstOrDefaultAsync();
                if (incidentReportData != null)
                {
                    var pIncidentActivationID2 = new SqlParameter("@IncidentActivationID", incidentActivationId);
                    var pCompanyID2 = new SqlParameter("@CompanyID", incidentReportData.CompanyId);
                    var list = await _context.Set<IncidentDataByActivationRefKeyContactsResponse>().FromSqlRaw(
                        "EXEC Pro_Report_GetIncidentData_ByActivationRef_KeyContacts @IncidentActivationID, @CompanyID", pIncidentActivationID2,
                        pCompanyID2).ToListAsync();
                    incidentReportData.KeyContacts = list.ConvertAll(x => x.ToKeyContacts());

                    var pIncidentActivationID3 = new SqlParameter("@IncidentActivationID", incidentActivationId);
                    var pCompanyID3 = new SqlParameter("@CompanyID", incidentReportData.CompanyId);
                    incidentReportData.IncidentAssets = await _context.Set<IncidentAssets>().FromSqlRaw(
                        "EXEC Pro_Report_GetIncidentData_ByActivationRef_IncidentAssets @IncidentActivationID, @CompanyID", pIncidentActivationID3,
                        pCompanyID3).ToListAsync();

                    return incidentReportData;
                }
                return new IncidentDataByActivationRefResponse();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SupportUserResponse> GetUser(int userId)
        {
            try
            {
                var Userlist = await (from Usersval in _context.Set<User>()
                                      join UC in _context.Set<User>() on Usersval.CreatedBy equals UC.UserId
                                      join UU in _context.Set<User>() on Usersval.UpdatedBy equals UU.UserId
                                      where Usersval.UserId == userId
                                      select new SupportUserResponse()
                                      {
                                          CompanyId = Usersval.CompanyId,
                                          UserId = Usersval.UserId,
                                          FirstName = Usersval.FirstName,
                                          LastName = Usersval.LastName,
                                          MobileISDCode = Usersval.Isdcode,
                                          MobileNo = Usersval.MobileNo,
                                          LLISDCode = Usersval.Llisdcode,
                                          Landline = Usersval.Landline,
                                          Primary_Email = Usersval.PrimaryEmail,
                                          UserPassword = Usersval.Password,
                                          UserPhoto = Usersval.UserPhoto,
                                          UserRole = Usersval.UserRole,
                                          UniqueGuiID = Usersval.UniqueGuiId,
                                          RegisteredUser = Usersval.RegisteredUser,
                                          Status = Usersval.Status,
                                          PasswordChangeDate = Usersval.PasswordChangeDate,
                                          ExpirePassword = Usersval.ExpirePassword,
                                          UserLanguage = Usersval.UserLanguage,
                                          FirstLogin = Usersval.FirstLogin,
                                          TrackMe = false,
                                          TrackMeLocation = 0,
                                          Smstrigger = Usersval.Smstrigger,
                                          CreatedByName = new UserFullName { Firstname = UC.FirstName, Lastname = UC.LastName },
                                          UpdatedByName = new UserFullName { Firstname = UU.FirstName, Lastname = UU.LastName },
                                          CreatedOn = Usersval.CreatedOn,
                                          UpdatedOn = Usersval.UpdatedOn,
                                          DepartmentId = Usersval.DepartmentId,
                                          ActiveOffDuty = Usersval.ActiveOffDuty,
                                          TimezoneId = Usersval.TimezoneId,
                                          CommsMethod = (from UC in _context.Set<UserComm>()
                                                         join CM in _context.Set<Core.Models.CommsMethod>() on UC.MethodId equals CM.CommsMethodId
                                                         where UC.UserId == Usersval.UserId && UC.Status == 1
                                                         select new CommsMethodModel()
                                                         {
                                                             MethodId = UC.MethodId,
                                                             MessageType = UC.MessageType,
                                                             Priority = UC.Priority,
                                                             MethodName = CM.MethodName
                                                         }).FirstOrDefault()!,
                                          SecGroup = (from SG in _context.Set<SecurityGroup>()
                                                      join USG in _context.Set<UserSecurityGroup>() on SG.SecurityGroupId equals USG.SecurityGroupId
                                                      where SG.CompanyId == Usersval.CompanyId && USG.UserId == Usersval.UserId
                                                      select SG.SecurityGroupId).FirstOrDefault(),
                                          OBJMap = (from UOR in _context.Set<ObjectRelation>()
                                                    join OBM in _context.Set<ObjectMapping>() on UOR.ObjectMappingId equals OBM.ObjectMappingId
                                                    join OBJ in _context.Set<Core.Models.Object>() on OBM.TargetObjectId equals OBJ.ObjectId
                                                    where (OBM.CompanyId == Usersval.CompanyId || OBM.CompanyId == null) && UOR.TargetObjectPrimaryId == Usersval.UserId
                                                    select new OBJMap
                                                    {
                                                        ObjectTableName = OBJ.ObjectTableName,
                                                        SourceObjectPrimaryId = UOR.SourceObjectPrimaryId,
                                                        ObjectMappingId = OBM.ObjectMappingId
                                                    }).FirstOrDefault()!,
                                      }).FirstOrDefaultAsync();
                if (Userlist != null)
                {
                    return Userlist;
                }
                else
                {
                    return new SupportUserResponse();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<IncidentMessagesRtn>> GetIncidentReportDetails(int incidentActivationId, int companyId)
        {
            try
            {
                var pIncidentActivationID = new SqlParameter("@IncidentActivationID", incidentActivationId);
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var response = await _context.Set<IncidentMessagesRtn>().FromSqlRaw("Pro_Admin_Get_Incident_Messages @IncidentActivationID, @CompanyID",
                  pIncidentActivationID, pCompanyID)
                  .ToListAsync();
                var incimsgs = response.Select(c =>
                {
                    c.SentBy = new UserFullName { Firstname = c.SentByFirst, Lastname = c.SentByLast };
                    c.Notes = (from N in _context.Set<IncidentTaskNotes>()
                               where (N.ObjectId == incidentActivationId && N.NoteType == "TASK")
                               || N.ObjectId == incidentActivationId && N.NoteType == "INCIDENT" && c.MessageType == "Ping"
                               select N).FirstOrDefault();
                    return c;
                }).ToList();

                if (incimsgs != null)
                {
                    return incimsgs;
                }
                else
                {
                    return new List<IncidentMessagesRtn>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
