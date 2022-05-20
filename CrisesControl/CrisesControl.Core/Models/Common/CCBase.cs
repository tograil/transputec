using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Models.Common
{
    public class CCBase: DataTableAjaxPost
    {
        [Required(ErrorMessage = "Company ID can not be blank")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "User ID can not be blank")]
        public int CurrentUserId { get; set; }

        [Required(ErrorMessage = "Password can not be blank")]
        [MaxLength(50)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Company key is required")]
        [MaxLength(100)]
        public string CompanyKey { get; set; }
        public int CustomerId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int SalesSource { get; set; }
        public bool FilterVirtual { get; set; }
        public string TOKEN { get; set; }
        public int IncidentId { get; set; }
        public int QUserId { get; set; }
    }
}
