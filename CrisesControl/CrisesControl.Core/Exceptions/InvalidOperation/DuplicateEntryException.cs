using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Exceptions.InvalidOperation
{
    public class DuplicateEntryException : InvalidOperationException
    {
        public DuplicateEntryException(string message)
            : base(message)
        {

        }

        public override string Message => "Duplicate Entry!";
    }
}
