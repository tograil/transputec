using CrisesControl.Core.TablePaging;
using System;
using System.ComponentModel.DataAnnotations;

namespace CrisesControl.Core.Common;
public class CCBase : DataTableAjaxPostModel
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