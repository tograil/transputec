using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Support
{
    public class IncidentDataByActivationRefResponse : IncidentsData
    {
        public string ActivatedByFirstName
        {
            get
            {
                return ActivatedBy?.Firstname ?? null;
            }
            set
            {
                if (ActivatedBy == null)
                {
                    ActivatedBy = new Users.UserFullName();
                }
                if (ActivatedBy.Firstname != value)
                {
                    ActivatedBy.Firstname = value;
                }
            }
        }
        public string ActivatedByLastName
        {
            get
            {
                return ActivatedBy?.Lastname ?? null;
            }
            set
            {
                if (ActivatedBy == null)
                {
                    ActivatedBy = new Users.UserFullName();
                }
                if (ActivatedBy.Lastname != value)
                {
                    ActivatedBy.Lastname = value;
                }
            }
        }
        public string LaunchedByFirstName
        {
            get
            {
                return LaunchedBy?.Firstname ?? null;
            }
            set
            {
                if (LaunchedBy == null)
                {
                    LaunchedBy = new Users.UserFullName();
                }
                if (LaunchedBy.Firstname != value)
                {
                    LaunchedBy.Firstname = value;
                }
            }
        }
        public string LaunchedByLastName
        {
            get
            {
                return LaunchedBy?.Lastname ?? null;
            }
            set
            {
                if (LaunchedBy == null)
                {
                    LaunchedBy = new Users.UserFullName();
                }
                if (LaunchedBy.Lastname != value)
                {
                    LaunchedBy.Lastname = value;
                }
            }
        }
        public string DeActivatedByFirstName
        {
            get
            {
                return DeactivatedBy?.Firstname ?? null;
            }
            set
            {
                if (DeactivatedBy == null)
                {
                    DeactivatedBy = new Users.UserFullName();
                }
                if (DeactivatedBy.Firstname != value)
                {
                    DeactivatedBy.Firstname = value;
                }
            }
        }
        public string DeActivatedByLastName
        {
            get
            {
                return DeactivatedBy?.Lastname ?? null;
            }
            set
            {
                if (DeactivatedBy == null)
                {
                    DeactivatedBy = new Users.UserFullName();
                }
                if (DeactivatedBy.Lastname != value)
                {
                    DeactivatedBy.Lastname = value;
                }
            }
        }
        public string ClosedByFirstName
        {
            get
            {
                return ClosedBy?.Firstname ?? null;
            }
            set
            {
                if (ClosedBy == null)
                {
                    ClosedBy = new Users.UserFullName();
                }
                if (ClosedBy.Firstname != value)
                {
                    ClosedBy.Firstname = value;
                }
            }
        }
        public string ClosedByLastName
        {
            get
            {
                return ClosedBy?.Lastname ?? null;
            }
            set
            {
                if (ClosedBy == null)
                {
                    ClosedBy = new Users.UserFullName();
                }
                if (ClosedBy.Lastname != value)
                {
                    ClosedBy.Lastname = value;
                }
            }
        }
    }
}
