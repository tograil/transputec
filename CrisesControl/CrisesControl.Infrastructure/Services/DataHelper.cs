using CrisesControl.Api.Application.Helpers;
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
        private static DBCommon DBC;
        public static bool CreateImportResult(List<ImportDump> ImportData, string impFilePath, string ReportType)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(impFilePath));
                using (StreamWriter SW = new StreamWriter(impFilePath, false, Encoding.UTF8))
                {

                    string headerRow = string.Empty;
                    if (ReportType.ToUpper() == "USERIMPORTCOMPLETE")
                    {
                        headerRow = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\"",
                            "First Name", "Last Name", "Email", "ISD", "Phone", "Landline", "User Role", "Status", "Location Name", "Address", "Group", "Security", "Action", "Import Status", "Comments");

                    }
                    else if (ReportType.ToUpper() == "LOCATIONIMPORTONLY")
                    {
                        headerRow = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                                               "Location Name", "Address", "Status", "Action", "Import Status", "Comments");

                    }
                    else if (ReportType.ToUpper() == "GROUPIMPORTONLY")
                    {
                        headerRow = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                              "Group Name", "Status", "Action", "Import Status", "Comments");
                    }
                    else if (ReportType.ToUpper() == "DEPARTMENTIMPORTONLY")
                    {
                        headerRow = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                              "Department Name", "Status", "Action", "Import Status", "Comments");
                    }
                    SW.WriteLine(headerRow);


                    foreach (ImportDump UIT in ImportData)
                    {
                        string record = string.Empty;

                        if (ReportType.ToUpper() == "USERIMPORTCOMPLETE")
                        {
                            record = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\"",
                                Replaceln(UIT.FirstName), Replaceln(UIT.Surname), Replaceln(UIT.Email), Replaceln(UIT.Isd), Replaceln(UIT.Phone), Replaceln(UIT.Landline),
                                Replaceln(UIT.UserRole), UIT.Status, Replaceln(UIT.Location), Replaceln(UIT.LocationAddress), Replaceln(UIT.Group),
                                Replaceln(UIT.Security), Replaceln(UIT.Action), UIT.ImportAction, Replaceln(UIT.ValidationMessage));

                        }
                        else if (ReportType.ToUpper() == "LOCATIONIMPORTONLY")
                        {

                            record = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                                                   Replaceln(UIT.Location), Replaceln(UIT.LocationAddress), UIT.LocationStatus, Replaceln(UIT.Action), UIT.ImportAction, Replaceln(UIT.ValidationMessage));
                        }
                        else if (ReportType.ToUpper() == "GROUPIMPORTONLY")
                        {
                            record = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                                  Replaceln(UIT.Group), UIT.GroupStatus, Replaceln(UIT.Action), UIT.ImportAction, Replaceln(UIT.ValidationMessage));
                        }
                        else if (ReportType.ToUpper() == "DEPARTMENTIMPORTONLY")
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
    }
}
