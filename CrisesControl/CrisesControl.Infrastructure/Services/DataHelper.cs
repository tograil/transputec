﻿using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public static class DataHelper
    {
        private static IDBCommonRepository DBC;
        public static  bool CreateImportResult(List<ImportDump> importData, string impFilePath, string reportType)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(impFilePath));
                using (StreamWriter SW = new StreamWriter(impFilePath, false, Encoding.UTF8))
                {

                    string headerRow = string.Empty;
                    if (reportType.ToUpper() == "USERIMPORTCOMPLETE")
                    {
                        headerRow = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\"",
                            "First Name", "Last Name", "Email", "ISD", "Phone", "Landline", "User Role", "Status", "Location Name", "Address", "Group", "Security", "Action", "Import Status", "Comments");

                    }
                    else if (reportType.ToUpper() == "LOCATIONIMPORTONLY")
                    {
                        headerRow = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                                               "Location Name", "Address", "Status", "Action", "Import Status", "Comments");

                    }
                    else if (reportType.ToUpper() == "GROUPIMPORTONLY")
                    {
                        headerRow = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                              "Group Name", "Status", "Action", "Import Status", "Comments");
                    }
                    else if (reportType.ToUpper() == "DEPARTMENTIMPORTONLY")
                    {
                        headerRow = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                              "Department Name", "Status", "Action", "Import Status", "Comments");
                    }
                    SW.WriteLine(headerRow);


                    foreach (ImportDump UIT in importData)
                    {
                        string record = string.Empty;

                        if (reportType.ToUpper() == "USERIMPORTCOMPLETE")
                        {
                            record = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\"",
                                Replaceln(UIT.FirstName), Replaceln(UIT.Surname), Replaceln(UIT.Email), Replaceln(UIT.Isd), Replaceln(UIT.Phone), Replaceln(UIT.Landline),
                                Replaceln(UIT.UserRole), UIT.Status, Replaceln(UIT.Location), Replaceln(UIT.LocationAddress), Replaceln(UIT.Group),
                                Replaceln(UIT.Security), Replaceln(UIT.Action), UIT.ImportAction, Replaceln(UIT.ValidationMessage));

                        }
                        else if (reportType.ToUpper() == "LOCATIONIMPORTONLY")
                        {

                            record = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                                                   Replaceln(UIT.Location), Replaceln(UIT.LocationAddress), UIT.LocationStatus, Replaceln(UIT.Action), UIT.ImportAction, Replaceln(UIT.ValidationMessage));
                        }
                        else if (reportType.ToUpper() == "GROUPIMPORTONLY")
                        {
                            record = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                                  Replaceln(UIT.Group), UIT.GroupStatus, Replaceln(UIT.Action), UIT.ImportAction, Replaceln(UIT.ValidationMessage));
                        }
                        else if (reportType.ToUpper() == "DEPARTMENTIMPORTONLY")
                        {
                            record = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                                  Replaceln(UIT.Department), UIT.GroupStatus, Replaceln(UIT.Action), UIT.ImportAction, Replaceln(UIT.ValidationMessage));
                        }

                        if (!string.IsNullOrEmpty(record))
                            SW.WriteLine(record);
                    }

                    DBC.connectUNCPath();

                    if (File.Exists(impFilePath))
                        return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static string Replaceln(string str, string rplcwith = " ")
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Replace(Environment.NewLine, rplcwith).Trim();
            }
            return "";
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
    }
}
