using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class MapLocationReturn
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LocationType { get; set; }
        public int LocationID { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        [NotMapped]
        public UserFullName UserName
        {
            get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
            set { }
        }
        public int Tracked { get; set; }
    }
}
