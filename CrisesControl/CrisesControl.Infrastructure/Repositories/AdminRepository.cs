
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.Companies;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class AdminRepository: IAdminRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<AdminRepository> _logger;
        private readonly SendEmail _SDE;
        public AdminRepository(CrisesControlContext context, ILogger<AdminRepository> logger, SendEmail SDE)
        {
            this._context=context;
            this._logger=logger;
            this._SDE = SDE;
        }
        public async Task<List<LibIncident>> GetAllLibIncident()
        {
            try
            {
                var allLibIncidents = await _context.Set<LibIncident>("exec Pro_Admin_GetAllLibIncidents").ToListAsync();

                if (allLibIncidents != null)
                {
                    return allLibIncidents;
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new List<LibIncident>();
        }
        public async Task<DataTable> GetReportData(int ReportID, List<ReportParam> sqlParams, string rFilePath,  string rFileName)
        {
            DataTable dt = new DataTable();
            rFilePath = string.Empty;
            rFileName = string.Empty;
            try
            {

                string ResultFilePath = Getconfig("ImportResultPath");
                string ExportPath = ResultFilePath + "DataExport\\";

                connectUNCPath();

                var reports = await  GetReportList(ReportID);

                if (reports != null)
                {

                    List<AdminReport> objdata = JsonConvert.DeserializeObject<List<AdminReport>>(reports.Result);
                    var ReportSP = objdata.FirstOrDefault().ReportSource;
                    var SourceType = objdata.FirstOrDefault().SourceType;
                    string FileName = objdata.FirstOrDefault().DownloadFileName + ".csv";

                    if (objdata.FirstOrDefault().DownloadFileName.ToUpper() == "RANDOM")
                        FileName = Guid.NewGuid().ToString() + ".csv";

                    if (SourceType == "VIEW" || SourceType == "TABLE")
                    {
                        ReportSP = "SELECT * FROM " + ReportSP;
                    }

                    string FilePath = ExportPath + FileName;
                    rFilePath = FilePath;
                    rFileName = FileName;

                    if (!Directory.Exists(ExportPath))
                    {
                        Directory.CreateDirectory(ExportPath);
                        DeleteOldFiles(ExportPath);
                    }

                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }

                    string constr = System.Configuration.ConfigurationManager.ConnectionStrings.ToString();
                    using (SqlConnection con = new SqlConnection(constr))
                    {

                        if (SourceType == "SP")
                        {
                            string delim = "";
                            ReportSP += " ";
                            foreach (ReportParam param in sqlParams)
                            {
                                ReportSP += delim + "@" + param.ParamName;
                                delim = ", ";
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(ReportSP))
                        {

                            foreach (ReportParam param in sqlParams)
                            {
                                cmd.Parameters.AddWithValue("@" + param.ParamName, param.ParamValue);
                            }

                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.Connection = con;
                                con.Open();
                                sda.SelectCommand = cmd;
                                sda.Fill(dt);
                            }
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
                return dt;
            }

        }

      
        private void DeleteOldFiles(string dirName)
        {
            string[] files = Directory.GetFiles(@dirName);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddDays(-1))
                    fi.Delete();
            }
        }
        private string Getconfig(string key, string DefaultVal = "")
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings[key];
                if (value != null)
                {
                    return value;
                }
                else
                {
                    return DefaultVal;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                return DefaultVal;
            }
        }

        public bool connectUNCPath(string UNCPath = "", string strUncUsername = "", string strUncPassword = "", string UseUNC = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(UNCPath))
                    UNCPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UNCPath"]);
                if (!string.IsNullOrEmpty(UseUNC))
                    UseUNC = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UseUNC"]);
                if (!string.IsNullOrEmpty(strUncUsername))
                    strUncUsername = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UncUserName"]);
                if (!string.IsNullOrEmpty(strUncPassword))
                    strUncPassword = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UncPassword"]);

                if (UseUNC == "true")
                {
                    UNCAccessWithCredentials.disconnectRemote(@UNCPath);
                    if (string.IsNullOrEmpty(UNCAccessWithCredentials.connectToRemote(@UNCPath, strUncUsername, strUncPassword)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
           
            }
           
        }
       
        public async Task<string> ToCSVHighPerformance(DataTable dataTable, bool includeHeaderAsFirstRow = true, string separator = ",")
        {
            //DataTable dataTable = new DataTable();
            StringBuilder csvRows = new StringBuilder();
            string row = "";
            int columns;
            try
            {
                //dataTable.Load(dataReader);
                columns = dataTable.Columns.Count;
                //Create Header
                if (includeHeaderAsFirstRow)
                {
                    for (int index = 0; index < columns; index++)
                    {
                        row += (dataTable.Columns[index]);
                        if (index < columns - 1)
                            row += (separator);
                    }
                    row += (Environment.NewLine);
                }
                csvRows.Append(row);

                //Create Rows
                for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
                {
                    row = "";
                    //Row
                    for (int index = 0; index < columns; index++)
                    {
                        string value = dataTable.Rows[rowIndex][index].ToString();

                        //If type of field is string
                        if (dataTable.Rows[rowIndex][index] is string)
                        {
                            //If double quotes are used in value, ensure each are replaced by double quotes.
                            if (value.IndexOf("\"") >= 0)
                                value = value.Replace("\"", "\"\"");

                            //If separtor are is in value, ensure it is put in double quotes.
                            if (value.IndexOf(separator) >= 0)
                                value = "\"" + value + "\"";

                            //If string contain new line character
                            while (value.Contains("\r"))
                            {
                                value = value.Replace("\r", "");
                            }
                            while (value.Contains("\n"))
                            {
                                value = value.Replace("\n", "");
                            }
                        }
                        row += value;
                        if (index < columns - 1)
                            row += separator;
                    }
                    dataTable.Rows[rowIndex][columns - 1].ToString().ToString().Replace(separator, " ");
                    row += Environment.NewLine;
                    csvRows.Append(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            dataTable.Dispose();
            return csvRows.ToString();
        }
        public async Task<AdminResult> GetReportList(int ReportID)
        {
            try
            {
                var pReportID = new SqlParameter("@ReportID", ReportID);
                var result = await _context.Set<AdminResult>().FromSqlRaw("exec Report_Admin_Get @ReportID", pReportID).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }
        public async Task<LibIncidentType> GetLibIncidentTypeByName(string Name)
        {
            var LibIncidentTypeExist = await _context.Set<LibIncidentType>().Where(LIT => LIT.Name == Name).FirstOrDefaultAsync();
            return LibIncidentTypeExist;
        }
        public async Task<LibIncident> GetLibIncidentByName(string Name)
        {
            var LibIncidentTypeExist = await _context.Set<LibIncident>().Where(LIT=> LIT.Name == Name).FirstOrDefaultAsync();
            return LibIncidentTypeExist;
        }
        public async Task<int> UpdateLibIncidentType(LibIncidentType libIncidentType)
        {
            try
            {
                _context.Update(libIncidentType);
                await _context.SaveChangesAsync();
                return libIncidentType.LibIncidentTypeId;
                              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddLibIncidentType(LibIncidentType libIncidentType)
        {
            try
            {
                await _context.AddAsync(libIncidentType);
                await _context.SaveChangesAsync();
                return libIncidentType.LibIncidentTypeId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateLibIncident(LibIncident libIncident)
          {
            try
            {
                _context.Update(libIncident);
               await _context.SaveChangesAsync();
                
                return libIncident.LibIncidentId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddLibIncident(LibIncident libIncident)
        {
            try
            {
                await _context.AddAsync(libIncident);
                await _context.SaveChangesAsync();
                return libIncident.LibIncidentId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteLibIncidentType(LibIncidentType libIncidentType )
        {
            try
            {
                
              _context.Remove(libIncidentType);
              await _context.SaveChangesAsync();               
              return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }
        public async Task<AdminLibIncident> GetLibIncident(int LibIncidentId)
        {

            try
            {
                var pLibIncidentId = new SqlParameter("@LibIncidentID", LibIncidentId);
                var libIncident = await _context.Set<AdminLibIncident>().FromSqlRaw("exec Pro_Admin_GetLibIncident @LibIncidentID", pLibIncidentId).FirstOrDefaultAsync();

                return libIncident;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LibIncident> GetLibIncidentById(int LibIncidentId)
        {
            var LibIncident = await _context.Set<LibIncident>().Where(LIT => LIT.LibIncidentId == LibIncidentId).FirstOrDefaultAsync();
            return LibIncident;
        }
        public async Task<bool> DeleteLibIncident(LibIncident libIncident)
        {
            try
            {
               
               _context.Remove(libIncident);
               await _context.SaveChangesAsync();
                    return true;
                
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<LibIncidentType>> GetAllLibIncidentType()
        {
            try
            {
                //Use: [dbo].[Pro_Admin_GetAllLibraryIncidentType]
                var allLibIncidentType = await _context.Set<LibIncidentType>().FromSqlRaw("exec Pro_Admin_GetAllLibraryIncidentType").ToListAsync();
                return allLibIncidentType;
        
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<LibIncidentType> GetLibIncidentType(int LibIncidentTypeId)
        {
            try
            {
                //Use: EXEC [dbo].[Pro_Admin_GetLibraryIncidentType] @LibIncidentTypeId
                var pLibIncidentTypeID = new SqlParameter("@LibIncidentTypeId", LibIncidentTypeId);
                var libIncidentType = await _context.Set<LibIncidentType>().FromSqlRaw("exec Pro_Admin_GetLibraryIncidentType @LibIncidentTypeId", pLibIncidentTypeID).FirstOrDefaultAsync();

                return libIncidentType;
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LibIncidentType> GetLibIncidentTypeById(int LibIncidentTypeId)
        {
            var LibIncident = await _context.Set<LibIncidentType>().Where(LIT => LIT.LibIncidentTypeId == LibIncidentTypeId).FirstOrDefaultAsync();
            return LibIncident;
        }
        public async Task<List<CompanyPackageFeatureList>> GetCompanyPackageFeatures(int OutUserCompanyId)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", OutUserCompanyId);
                var transactions = await _context.Set<CompanyPackageFeatureList>().FromSqlRaw(" exec Pro_Company_PackageFeature @CompanyID", pCompanyID).ToListAsync();

                    return transactions;
               
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CompanyPackageFeatureList>> GetCompanyModules(int OutUserCompanyId)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", OutUserCompanyId);
                var transactions = await _context.Set<CompanyPackageFeatureList>().FromSqlRaw("exec Pro_Company_Module @CompanyID", pCompanyID).ToListAsync();

               
                    return transactions;
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<EmailTemplateList>> GetEmailTemplate(string Code, string Locale, int TemplateID, int Status, int CompanyID, string TimeZoneId)
        {
            try
            {

                var pCode = new SqlParameter("@Code", Code);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pTemplateID = new SqlParameter("@TemplateID", TemplateID);
                var pStatus = new SqlParameter("@Status", Status);
                var pLocale = new SqlParameter("@Locale", Locale);
                var pTimeZoneId = new SqlParameter("@TimeZoneId", TimeZoneId);

                var getList = await _context.Set<EmailTemplateList>().FromSqlRaw("exec Pro_Company_Email_Template @Code, @CompanyID, @TemplateID, @Status, @Locale, @TimeZoneId",
                    pCode, pCompanyID, pTemplateID, pStatus, pLocale, pTimeZoneId).ToListAsync();


               
                return getList;
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }

        public async Task<int> SaveEmailTemplate(int TemplateID, string Type, string Code, string Name, string Description, string HtmlData, string EmailSubject,
            string Locale, int Status, int CurrentUserID, int CompanyID = 0, string TimeZoneId = "GMT Standard Time")
        {
            try
            {
                if (TemplateID > 0)
                {
                    var item = await _context.Set<EmailTemplate>().Where(T=> T.TemplateId == TemplateID && T.CompanyId == CompanyID).FirstOrDefaultAsync();
                    if (item != null)
                    {
                        if (!string.IsNullOrEmpty(Type))
                            item.Type = Type;

                        if (!string.IsNullOrEmpty(Code))
                            item.Code = Code;

                        if (!string.IsNullOrEmpty(Name))
                            item.Name = Name;

                        if (!string.IsNullOrEmpty(Locale))
                            item.Locale = Locale;

                        if (!string.IsNullOrEmpty(Description))
                            item.Description = Description;

                        item.HtmlData = HtmlData;

                        if (Status > 0)
                            item.Status = Status;

                        if (!string.IsNullOrEmpty(EmailSubject))
                            item.EmailSubject = EmailSubject;

                        item.CompanyId = CompanyID;
                        item.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                        item.UpdatedBy = CurrentUserID;
                        _context.Update(item);
                       await _context.SaveChangesAsync();
                        return item.TemplateId;
                    }
                    else
                    {
                        return await CreateEmailTemplate("COMPANY", Code, Name, Description, HtmlData, EmailSubject, Locale, Status, CurrentUserID, CompanyID, TimeZoneId);
                    }
                }
                else
                {
                    var item = await _context.Set<EmailTemplate>().Where(T=> T.Code == Code && T.CompanyId == CompanyID ).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        var citem = await _context.Set<EmailTemplate>().Where(T=> T.Code == Code && T.CompanyId == 0).FirstOrDefaultAsync();
                        if (citem != null)
                        {
                         return  await CreateEmailTemplate(citem.Type, citem.Code, citem.Name, citem.Description, HtmlData, EmailSubject, citem.Locale, citem.Status, CurrentUserID, CompanyID, TimeZoneId);
                        }
                        else
                        {
                            return await CreateEmailTemplate(Type, Code, Name, Description, HtmlData, EmailSubject, Locale, Status, CurrentUserID, CompanyID, TimeZoneId);
                        }
                       
                    }
                  
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<EmailTemplate> GetEmailTemplateById(int TemplateId, int CompanyID)
        {
            var item = await _context.Set<EmailTemplate>().Where(T => T.TemplateId == TemplateId && T.CompanyId == CompanyID).FirstOrDefaultAsync();
            return item;
        }

        public async Task<int> CreateEmailTemplate(string Type, string Code, string Name, string Description, string HtmlData, string EmailSubject, string Locale,
            int Status, int CurrentUserID, int CompanyID, string TimeZoneId)
        {
            try
            {
                EmailTemplate ET = new EmailTemplate();
                ET.Type = Type;
                ET.Code = Code;
                ET.Name = Name;
                ET.Locale = Locale;
                ET.Description = Description;
                ET.HtmlData = HtmlData;
                ET.EmailSubject = EmailSubject;
                ET.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                ET.UpdatedBy = CurrentUserID;
                ET.CompanyId = CompanyID;
                ET.Status = Status;
                await _context.AddAsync(ET);
                await _context.SaveChangesAsync();
                return ET.TemplateId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        public async Task<List<EmailFieldLookup>> GetEmailFields(string TemplateCode, int FieldType = 1)
        {
            try
            {
                if (TemplateCode.ToUpper() == "ALL")
                {
                    var fieldlist = await _context.Set<EmailFieldLookup>()
                                     .Where(FL=> FL.FieldType >= FieldType)
                                     .OrderBy(o => o.FieldName).ToListAsync();
                    return fieldlist;
                }
                else
                {

                    var fieldlist = await _context.Set<EmailFieldLookup>()
                                     .Where(FL=> FL.FieldType >= FieldType && FL.FieldCode == TemplateCode)
                                     .OrderBy(o => o.FieldName).ToListAsync();
                    return fieldlist;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
            
        }
        public async Task<EmailTemplate> GetEmailTemplateByCode(string TemplateCode,int CompanyID)
        {
            var ctemplate = await _context.Set<EmailTemplate>()
                                     .Where(T => T.Code == TemplateCode && T.CompanyId == CompanyID)
                                     .FirstOrDefaultAsync();
            return ctemplate;
        }
        public async Task<bool> RestoreTemplate(EmailTemplate ctemplate)
        {
            try
            {
                
                    
                    if (ctemplate != null)
                    {
                        _context.Remove(ctemplate);
                        await _context.SaveChangesAsync();
                        return true;
                     
                    }
                    return false;
                   
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<List<AdminUsersList>> SendCustomerNotice(string EmailContent, string EmailSubject, List<string> ExtraEmailList)
        {
            try
            {

                string Domain = await LookupWithKey("DOMAIN");
                string Portal = await LookupWithKey("PORTAL");
                string CCimage = await LookupWithKey("CCLOGO");
                string EmailFrom = await LookupWithKey("ALERT_EMAILFROM");
                string SMTPHost = await LookupWithKey("SMTPHOST");
                string EmailSub =await LookupWithKey("EMAILSUB");

                var result = await _context.Set<AdminUsersList>().FromSqlRaw("exec Pro_Admin_Email_List").ToListAsync();

                
                foreach (AdminUsersList user in result)
                {
                    string messagebody = EmailContent;
                    messagebody = messagebody.Replace("{FIRST_NAME}", user.FirstName).Replace("{LAST_NAME}", user.LastName).Replace("{COMPANY_NAME}", user.CompanyName);
                    string[] useremail = { user.PrimaryEmail };
                    _SDE.Email(useremail, messagebody, EmailFrom, SMTPHost, EmailSubject);
                }

                foreach (string user in ExtraEmailList)
                {
                    string messagebody = EmailContent;
                    messagebody = messagebody.Replace("{FIRST_NAME}", "Valued").Replace("{LAST_NAME}", "Customer").Replace("{COMPANY_NAME}", "Transputec");
                    string[] useremail = { user };
                    _SDE.Email(useremail, EmailContent, EmailFrom, SMTPHost, EmailSubject);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
                
            }
        }
        public async Task<string> LookupWithKey(string Key, string Default = "")
        {
            try
            {
                Dictionary<string, string> Globals = CCConstants.GlobalVars;
                if (Globals.ContainsKey(Key))
                {
                    return Globals[Key];
                }

                var LKP = await _context.Set<SysParameter>()
                           .Where(L=> L.Name == Key).FirstOrDefaultAsync();
                if (LKP != null)
                {
                    Default = LKP.Value;
                }
                return Default;
            }
            catch (Exception ex)
            {
                throw ex;
               
            }
        }
        public async Task<List<TransactionType>> GetTransactionType()
        {
            try
            {
                var TransactinTypes = await _context.Set<TransactionType>().ToListAsync();                
                return TransactinTypes;               
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> AddTransaction(UpdateTransactionDetailsModel IP, int CurrentUserId, int CompanyId, string TimeZoneId)
        {
            try
            {
                var ttype = await _context.Set<TransactionType>().Where(TT=> TT.TransactionTypeId == IP.TransactionTypeId).FirstOrDefaultAsync();
                if (ttype != null)
                {

                    //Make a transaction now
                    int Transid =await UpdateTransactionDetails(0, CompanyId, IP.TransactionTypeId, IP.TransactionRate, IP.MinimumPrice, IP.Quantity,
                        IP.Cost, IP.LineValue, IP.Vat, IP.Total, 0, IP.TransactionDate, CurrentUserId, IP.TransactionReference, TimeZoneId,
                        IP.TransactionStatus, 0, IP.DRCR);

                    if (Transid > 0)
                    {
                        var comp_pp = await _context.Set<CompanyPaymentProfile>().Where(CPP=> CPP.CompanyId == CompanyId).FirstOrDefaultAsync();
                        if (comp_pp != null)
                        {
                            
                            //Handle the TOPUP Transaction
                            if (ttype.TransactionCode == "TOPUP")
                            {
                                decimal newBalance = comp_pp.CreditBalance + IP.Total;
                                comp_pp.CreditBalance = newBalance;
                                comp_pp.LastCreditDate = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                                comp_pp.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                                comp_pp.UpdatedBy = CurrentUserId;
                                _context.Update(comp_pp);
                                _context.SaveChanges();

                               await GetSetCompanyComms(CompanyId);

                                if (IP.PaymentMethod == "SELFTOPUP")
                                {

                                    _SDE.SendPaymentTransactionAlert(CompanyId, IP.Total, TimeZoneId);
                                }
                            }

                            //Handle the Storage Transaction
                            if (ttype.TransactionCode == "STORAGE")
                            {
                                comp_pp.StorageLimit = comp_pp.StorageLimit + IP.Storage;
                                comp_pp.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                                comp_pp.UpdatedBy = CurrentUserId;
                                _context.Update(comp_pp);
                                await _context.SaveChangesAsync();
                                return Transid = comp_pp.CompanyId;
                            }
                        }
                        //ResultDTO.ErrorId = 0;
                        //ResultDTO.Message = "Transaction entry is successful";
                        return Transid;
                    }
                    //else
                    //{
                    //    ResultDTO.ErrorId = 100;
                    //    ResultDTO.Message = "Transaction was not successfull";
                    //}
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<int> UpdateTransactionDetails(int TransactionHeaderId, int CompanyId, int TransactionTypeId, decimal TransactionRate, decimal MinimumPrice,
           int Quantity, decimal Cost, decimal LineValue, decimal LineVAT, decimal Total, int MessageId, DateTimeOffset TransactionDate, int currntUserId = 0,
           string TransactionReference = "", string TimeZoneId = "GMT Standard Time", int TransactionStatus = 1, int TransactionDetailsId = 0, string TrType = "DR")
        {

            int TDId = 0;
            try
            {
                if (Total > 0)
                {
                    if (TransactionDetailsId == 0)
                    {

                        TransactionDetail NewTransactionDetails = new TransactionDetail()
                        {
                            TransactionHeaderId = TransactionHeaderId,
                            CompanyId = CompanyId,
                            TransactionReference = TransactionReference,
                            TransactionTypeId = TransactionTypeId,
                            TransactionRate = TransactionRate,
                            MinimumPrice = MinimumPrice,
                            TransactionDate = TransactionDate,
                            Quantity = Quantity,
                            Cost = Cost,
                            LineValue = LineValue,
                            LineVat = LineVAT,
                            Total = Total,
                            TransactionStatus = TransactionStatus,
                            MessageId = MessageId,
                            CreatedBy = currntUserId,
                            CreatedOn = DateTime.Now,
                            UpdatedBy = currntUserId,
                            UpdateOn = DateTime.Now,
                            Drcr = TrType,
                            IsPaid = false
                        };
                       await _context.AddAsync(NewTransactionDetails);
                        await _context.SaveChangesAsync();
                        TDId = NewTransactionDetails.TransactionDetailsId;

                    }
                    else
                    {
                        var newTransactionDetails = await _context.Set<TransactionDetail>().Where(TD=> TD.TransactionDetailsId == TransactionDetailsId).FirstOrDefaultAsync();
                        if (newTransactionDetails != null)
                        {
                            newTransactionDetails.TransactionHeaderId = TransactionHeaderId;
                            newTransactionDetails.TransactionReference = TransactionReference;
                            newTransactionDetails.TransactionTypeId = TransactionTypeId;
                            newTransactionDetails.TransactionRate = TransactionRate;
                            newTransactionDetails.MinimumPrice = MinimumPrice;
                            newTransactionDetails.Quantity = Quantity;
                            newTransactionDetails.LineValue = LineValue;
                            newTransactionDetails.Cost = Cost;
                            newTransactionDetails.LineVat = LineVAT;
                            newTransactionDetails.Total = Total;
                            newTransactionDetails.TransactionDate = TransactionDate;
                            newTransactionDetails.TransactionStatus = TransactionStatus;
                            newTransactionDetails.UpdatedBy = currntUserId;
                            newTransactionDetails.UpdateOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                            newTransactionDetails.Drcr = TrType;
                            _context.Update(newTransactionDetails);
                            await _context.SaveChangesAsync();
                            TDId = newTransactionDetails.TransactionDetailsId;
                        }
                    }
                }
                return TDId;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            
        }
        public async Task CreateCompanyPackageFeature(int SecurityObjectID, int CompanyID, int Status)
        {
            try
            {

                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pSecurityObjectID = new SqlParameter("@SecurityObjectID", SecurityObjectID);
                var pStatus = new SqlParameter("@Status", Status);

               await _context.Set<CompanyPackageFeature>().FromSqlRaw(" exec Pro_Admin_Save_Company_Feature @CompanyID, @SecurityObjectID, @Status",
                    pCompanyID, pSecurityObjectID, pStatus).FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SubscribeModule(int TransactionTypeId, string PaymentPeriod, int CurrentUserId, int CompanyId, string TimeZoneId)
        {
            try
            {
                var ttype = await _context.Set<TransactionType>().Where(TT=> TT.TransactionTypeId == TransactionTypeId).FirstOrDefaultAsync();
                if (ttype != null)
                {
                    var comp_pp =  _context.Set<CompanyPaymentProfile>().Where(CPP=> CPP.CompanyId == CompanyId).FirstOrDefaultAsync();
                    if (comp_pp != null)
                    {
                        //Handle the Subscription Transaction
                        //Check if the transaction type is a chargeable feature module
                        var check_feature = await _context.Set<ChargeableFeature>().Where(CF=> CF.TransactionTypeId == ttype.TransactionTypeId).FirstOrDefaultAsync();

                        if (check_feature != null)
                        {
                            int SecurityObjID = check_feature.SecurityObjectId;
                            var SecurityItems = await _context.Set<SecurityObject>()
                                                 .Where(SO=> SO.SecurityObjectId == SecurityObjID || SO.ParentId == SecurityObjID
                                                 ).ToListAsync();

                            foreach (var secitem in SecurityItems)
                            {
                               await CreateCompanyPackageFeature(secitem.SecurityObjectId, CompanyId, 1);
                            }
                            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            DateTimeOffset NextRun = _SDE.GetNextRunDate(firstDayOfMonth, PaymentPeriod);
                            decimal rate = ttype.Rate;
                            if (PaymentPeriod == "YEARLY")
                            {
                                rate = ttype.Rate * 12;
                            }
                            //int id = 0; UpdateCompanyTranscationType(CompanyId, CurrentUserId, TimeZoneId, ttype.TransactionTypeID, ttype.Rate, 0, IP.PaymentPeriod, null, IP.PaymentMethod);

                            //this will be added to monthly transaction by the billing service
                            //BH.AddMonthTransaction(0, OutUserCompanyId, 0, "", ttype.Rate, ttype.TransactionTypeID);

                            if (ttype.TransactionCode == "INCIDENTTASKMGRLICENSE")
                            {
                                await UpdateCompanyParameter("ACTIVE_ISOP", "true", CompanyId, CurrentUserId, TimeZoneId, 1);

                                decimal user_rate = 0.10M;
                                decimal.TryParse(await GetPackageItem("USER_RATE", CompanyId), out user_rate);
                                decimal keyholder_rate = 10M;
                                decimal.TryParse(await GetPackageItem("ADMIN_USER_RATE", CompanyId), out keyholder_rate);

                                decimal task_manager_charge_rate = 2M;
                                decimal.TryParse(await LookupWithKey("TASK_MANAGER_EXTRA_USER_CHARGE_RATE"), out task_manager_charge_rate);

                                user_rate = user_rate * task_manager_charge_rate;
                                keyholder_rate = keyholder_rate * task_manager_charge_rate;

                               await UpdateCompanyPackageItem("USER_RATE", user_rate.ToString(), 1, CompanyId, CurrentUserId, TimeZoneId);
                               await UpdateCompanyPackageItem("ADMIN_USER_RATE", keyholder_rate.ToString(), 1, CompanyId, CurrentUserId, TimeZoneId);
                                return true;
                            }
                            return true;
                          
                        }
                        return true;
                    }
                  
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateCompanyParameter(string Name, string Value, int CompanyId, int CurrentUserId, string TimeZoneId, int Status = 4)
        {
            var cp = await _context.Set<CompanyParameter>().Where(CP=> CP.Name == Name && CP.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (cp != null)
            {
                cp.Value = Value;

                if (Status != 4)
                    cp.Status = Status;

                cp.UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                cp.UpdatedBy = CurrentUserId;
                await _context.SaveChangesAsync();
            }
            else
            {
                await AddCompanyParameter(Name, Value, CompanyId, CurrentUserId, TimeZoneId);
            }
        }

        public async Task<TransactionList> GetCompanyTransactions(int CompanyId, DateTimeOffset StartDate, DateTimeOffset EndDate)
        {
            try
            {
                TransactionList TransactionList = new TransactionList();
                if (StartDate != EndDate)
                {
                    if (EndDate >= StartDate)
                    {
                        DateTime FinalStatDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0);
                        DateTime FinalEndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 23, 59, 59);

                        var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
                        var pStartDate = new SqlParameter("@StartDate", FinalStatDate);
                        var pEndDate = new SqlParameter("@EndDate", FinalEndDate);

                        var get_list = await _context.Set<TransactionDtls>().FromSqlRaw("exec Pro_Company_Transactions @CompanyID, @StartDate, @EndDate",
                            pCompanyID, pStartDate, pEndDate).ToListAsync();

                        TransactionList.transactionDtls = get_list;
                    }
                }
                else
                {
                    var get_list = await _context.Set<TransactionDetail>()
                                    .Where(TR=> TR.CompanyId == CompanyId)                                   
                                    .OrderByDescending(TR=>TR.TransactionDate).Take(10).ToListAsync();
                    TransactionList.TransactionDetail = get_list;
                }
                return TransactionList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<List<UnpaidTransaction>> GetUnpaidTransactions(int TransactionId, DateTimeOffset StartDate, DateTimeOffset EndDate)
        {
            try
            {
                var pTransactionId = new SqlParameter("@TransactionID", TransactionId);
                var pStartDate = new SqlParameter("@StartDate", StartDate);
                var pEndDate = new SqlParameter("@EndDate", EndDate);
               var result= await _context.Set<UnpaidTransaction>().FromSqlRaw(" exec Pro_Admin_GetUnpaidTransactions @TransactionID,@StartDate,@EndDate", pTransactionId, pStartDate, pEndDate)
                               .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateCompanyPackageItem(string ItemCode, string ItemValue, int Status, int CompanyID, int CurrentUserId, string TimeZoneId)
        {
            try
            {
                var item = await _context.Set<CompanyPackageItem>().Where(CP=> CP.ItemCode == ItemCode && CP.CompanyId == CompanyID ).FirstOrDefaultAsync();
                if (item != null)
                {
                    item.ItemValue = ItemValue;
                    item.Status = Status;
                    item.UpdatedBy = CurrentUserId;
                    item.UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                    _context.Update(item);
                   await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> GetPackageItem(string ItemCode, int CompanyId)
        {
            string retVal = string.Empty;
            ItemCode = ItemCode.Trim();
            var ItemRec = await _context.Set<CompanyPackageItem>().Where(PI=> PI.ItemCode == ItemCode && PI.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (ItemRec != null)
            {
                retVal = ItemRec.ItemValue;
            }
            else
            {
                var LibItemRec = await _context.Set<LibPackageItem>().Where(PI=> PI.ItemCode == ItemCode).FirstOrDefaultAsync();
                retVal = LibItemRec.ItemValue;
            }
            return retVal;
        }
        public async Task<int> AddCompanyParameter(string Name, string Value, int CompanyId, int CurrentUserId, string TimeZoneId)
        {
            try
            {
                var comp_param = await _context.Set<CompanyParameter>().Where(CP=> CP.CompanyId == CompanyId && CP.Name == Name).AnyAsync();
                if (!comp_param)
                {
                    CompanyParameter NewCompanyParameters = new CompanyParameter()
                    {
                        CompanyId = CompanyId,
                        Name = Name,
                        Value = Value,
                        Status = 1,
                        CreatedBy = CurrentUserId,
                        UpdatedBy = CurrentUserId,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId)
                    };
                    await _context.AddAsync(NewCompanyParameters);
                    await _context.SaveChangesAsync();
                    return NewCompanyParameters.CompanyParametersId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }
        public async Task GetSetCompanyComms(int CompanyID)
        {
            try
            {
                var comp_pp = (from CPP in _context.Set<CompanyPaymentProfile>() where CPP.CompanyId == CompanyID select CPP).FirstOrDefault();
                var comp = (from C in _context.Set<Company>() where C.CompanyId == CompanyID select C).FirstOrDefault();
                if (comp_pp != null && comp != null)
                {

                    if (comp.Status == 1)
                    {
                        bool sendAlert = false;

                        DateTimeOffset LastUpdate = comp_pp.UpdatedOn;

                        List<string> stopped_comms = new List<string>();

                        if (comp_pp.MinimumEmailRate > 0)
                        {
                            stopped_comms.Add("EMAIL");
                        }
                        if (comp_pp.MinimumPhoneRate > 0)
                        {
                            stopped_comms.Add("PHONE");
                        }
                        if (comp_pp.MinimumTextRate > 0)
                        {
                            stopped_comms.Add("TEXT");
                        }
                        if (comp_pp.MinimumPushRate > 0)
                        {
                            stopped_comms.Add("PUSH");
                        }

                        if (comp_pp.CreditBalance > comp_pp.MinimumBalance)
                        { //Have positive balance + More than the minimum balance required.
                            comp.CompanyProfile = "SUBSCRIBED";
                          await  _set_comms_status(CompanyID, stopped_comms, true);
                        }
                        else if (comp_pp.CreditBalance < -comp_pp.CreditLimit)
                        { //Used the overdraft amount as well, so stop their SMS and Phone
                            comp.CompanyProfile = "STOP_MESSAGING";
                            sendAlert = true;
                           await _set_comms_status(CompanyID, stopped_comms, false);
                        }
                        else if (comp_pp.CreditBalance < 0 && comp_pp.CreditBalance > -comp_pp.CreditLimit)
                        { //Using the overdraft facility, can still use the system
                            comp.CompanyProfile = "ON_CREDIT";
                            sendAlert = true;
                           await _set_comms_status(CompanyID, stopped_comms, true);
                        }
                        else if (comp_pp.CreditBalance < comp_pp.MinimumBalance)
                        { //Less than the minimum balance, just send an alert, can still use the system.
                            comp.CompanyProfile = "LOW_BALANCE";
                            sendAlert = true;
                           await _set_comms_status(CompanyID, stopped_comms, true);
                        }
                        comp_pp.UpdatedOn = DateTime.Now.GetDateTimeOffset();
                        _context.Update(comp_pp);
                        _context.SaveChanges();

                        if (DateTimeOffset.Now.Subtract(LastUpdate).TotalHours < 24)
                        {
                            sendAlert = false;
                        }

                        string CommsDebug = await LookupWithKey("COMMS_DEBUG_MODE");

                        if (sendAlert && CommsDebug == "false")
                        {
                            
                           await _SDE.UsageAlert(CompanyID);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task _set_comms_status(int CompanyId, List<string> methods, bool status)
        {
            try
            {
                (from CM in _context.Set<CompanyComm>()
                 join CO in _context.Set<CommsMethod>() on CM.MethodId equals CO.CommsMethodId
                 where CM.CompanyId == CompanyId && methods.Contains(CO.MethodCode)
                 select CM).ToList().ForEach(x => x.ServiceStatus = status);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LanguageItem> GetLanguageById(int lanuageId)
        {
            var item = await _context.Set<LanguageItem>().Where(L=> L.LanguageItemId == lanuageId).FirstOrDefaultAsync();
            return item;
        }

        public async Task<int> SaveLanguageItem(LanguageItem lanuage)
        {
            await _context.AddAsync(lanuage);
            await _context.SaveChangesAsync();
            return lanuage.LanguageItemId;
        }

        public async Task<int> UpdateLanguageItem(LanguageItem lanuage)
        {
            _context.Update(lanuage);
            await _context.SaveChangesAsync();
            return lanuage.LanguageItemId;
        }
        public async Task<AppLanguages> GetAppLanguage(string LangKey, string Locale, int LanguageItemID, string ObjectType = "APP")
        {
            try
            {
                AppLanguages app = new AppLanguages();
                List<string> objtype = new List<string>();
                if (ObjectType == "APP")
                {
                    objtype.Add("LABEL");
                    objtype.Add("MESSAGE");
                }
                else
                {
                    objtype.Add(ObjectType);
                }

                if (LanguageItemID > 0)
                {
                    var item = await _context.Set<LanguageItem>().Where(L=> L.LanguageItemId == LanguageItemID && objtype.Contains(L.ObjectType)).FirstOrDefaultAsync();
                    if (item != null)
                        app.languageItem = item;
                }
                if (!string.IsNullOrEmpty(LangKey) && LangKey == "ALL")
                {
                    var list =await _context.Set<LanguageItem>().Where(L=> L.Locale == Locale && objtype.Contains(L.ObjectType)).ToListAsync();
                    app.languageItems = list;
                }
                else
                {
                    var list = await _context.Set<LanguageItem>().Where(L => objtype.Contains(L.ObjectType)).ToListAsync();
                    app.languageItems = list;
                }
                return app;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CompanyPackageItem> GetCompanyPackageById(int PackageItemId)
        {
            var cp = await _context.Set<CompanyPackageItem>().Where(CP=> CP.CompanyPackageItemId == PackageItemId).FirstOrDefaultAsync();
            return cp;
        }

        public async Task<int> UpdateCompanyPackageItem(CompanyPackageItem packageItem)
        {
            try
            {
                _context.Update(packageItem);
                await _context.SaveChangesAsync();

                return packageItem.CompanyPackageItemId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<CompanyPackageItems> GetCompanyPackageItems(int CompanyID, int PackageItemId)
        {
            try
            {
                if (PackageItemId <= 0)
                {
                    //var CompanyItem = (from PI in _context.Set<CompanyPackageItem>()
                    //                   join C in _context.Set<Company>() on PI.CompanyId equals C.CompanyId
                    //                   from LPI in _context.Set<LibPackageItem>().Where(LPI => LPI.ItemCode == PI.ItemCode && LPI.PackagePlanId == C.PackagePlanId).DefaultIfEmpty()
                    //                   where PI.CompanyId == CompanyID && PI.Status == 1 && LPI.PackagePlanId == C.PackagePlanId
                    //                   select new
                    //                   {
                    //                       PI.CompanyPackageItemId,
                    //                       PI.ItemCode,
                    //                       LPI.ItemName,
                    //                       LPI.ItemDescription,
                    //                       PI.ItemValue,
                    //                       PI.Status,
                    //                   }).Distinct();

                    var CompanyItem =  _context.Set<Company>().Include(x => x.CompanyPackageItem).Include(x => x.CompanyPackageItem.LibPackageItem)
                         .Where(PI => PI.CompanyPackageItem.CompanyId == CompanyID && PI.CompanyPackageItem.Status == 1 && PI.CompanyPackageItem.LibPackageItem.PackagePlanId == PI.PackagePlanId)
                         .Select(a => new CompanyPackageItems()
                         {
                          CompanyPackageItemId=   a.CompanyPackageItem.CompanyPackageItemId,
                          ItemCode=   a.CompanyPackageItem.ItemCode,
                          ItemName=   a.CompanyPackageItem.LibPackageItem.ItemName,
                          ItemDescription=   a.CompanyPackageItem.LibPackageItem.ItemDescription,
                          ItemValue=   a.CompanyPackageItem.ItemValue,
                          Status=   a.CompanyPackageItem.Status,
                         }).Distinct();
                    return (CompanyPackageItems)CompanyItem;
                }
                else
                {
                    //var item = (from PI in _context.Set<CompanyPackageItem>()
                    //            join C in _context.Set<Company>() on PI.CompanyId equals C.CompanyId
                    //            from LPI in _context.Set<LibPackageItem>().Where(LPI => LPI.ItemCode == PI.ItemCode && LPI.PackagePlanId == C.PackagePlanId).DefaultIfEmpty()
                    //            where PI.CompanyId == CompanyID && PI.Status == 1 && PI.CompanyPackageItemId == PackageItemId
                    //            && LPI.PackagePlanId == C.PackagePlanId
                    //            select new
                    //            {
                    //                PI.CompanyPackageItemId,
                    //                PI.ItemCode,
                    //                LPI.ItemName,
                    //                LPI.ItemDescription,
                    //                PI.ItemValue,
                    //                PI.Status,
                    //            }).FirstOrDefault();
                    var item = await _context.Set<Company>().Include(x => x.CompanyPackageItem).Include(x => x.CompanyPackageItem.LibPackageItem)
                         .Where(PI => PI.CompanyPackageItem.CompanyId == CompanyID && PI.CompanyPackageItem.Status == 1 && PI.CompanyPackageItem.LibPackageItem.PackagePlanId == PI.PackagePlanId)
                         .Select(a => new CompanyPackageItems()
                         {
                             CompanyPackageItemId = a.CompanyPackageItem.CompanyPackageItemId,
                             ItemCode = a.CompanyPackageItem.ItemCode,
                             ItemName = a.CompanyPackageItem.LibPackageItem.ItemName,
                             ItemDescription = a.CompanyPackageItem.LibPackageItem.ItemDescription,
                             ItemValue = a.CompanyPackageItem.ItemValue,
                             Status = a.CompanyPackageItem.Status,
                         }).FirstOrDefaultAsync();
                    return item;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
