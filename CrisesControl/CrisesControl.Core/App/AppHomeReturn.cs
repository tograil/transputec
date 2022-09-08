using CrisesControl.Core.Incidents;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.App
{
    public class AppHomeReturn
    {
        public int PingCount { get; set; }
        public int IncidentCount { get; set; }
        public int PingStat { get; set; }
        public int IncidentStat { get; set; }
        [NotMapped]
        public UsefulText UsefulText { get; set; }
        public string TutorialPath { get; set; }
        public string TermsAndConditionsUrl { get; set; }
        public string PrivacyPolicyUrl { get; set; }
        public string WhatsNewUrl { get; set; }
        public string CopyRightInfo { get; set; }
        public string CompanyText { get; set; }
        public string MyTaskURL { get; set; }
        public bool HasTask { get; set; }
        public List<CompanyParam> CompanyParams { get; set; }
        public List<CommsMethods> MessageMethods { get; set; }
        public bool TrackMeTravel { get; set; }
        public bool TrackMeIncident { get; set; }
        public List<AppIconURL> AppIconURL { get; set; }
        public string InviteUrl { get; set; }
        public string ApiVersion { get; set; }
        public DateTimeOffset CCMessageLastUpdate { get; set; }
        public DateTimeOffset CompanyMessageLastUpdate { get; set; }
        public string PhoneVerifier { get; set; }
        public DateTimeOffset LangLastUpdated { get; set; }
        public string PhoneContactLogo { get; set; }
        public int PingListThreshold { get; set; }
        public int PingListLimit { get; set; }
    }
}
