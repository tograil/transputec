using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services.Jobs
{
    public class Update
    {
        public delegate void UpdateHandler(object sender, UpdateEventArgs e);
        public class UpdateEventArgs : EventArgs
        {
            public UpdateEventArgs(string _Msg)
            {
                Message = _Msg;
            }
            public string Message { get; }
        }
    }
}
