using System;

namespace CrisesControl.Core.Models
{
    public partial class TaskActiveCheckListUserResponse
    {
        public int UserResponseId { get; set; }
        public int ActiveCheckListId { get; set; }
        public int ActiveReponseId { get; set; }
        public string Comment { get; set; } = null!;
        public bool Done { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
