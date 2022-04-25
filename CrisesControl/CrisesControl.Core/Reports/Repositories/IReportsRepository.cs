using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports.Repositories {
    public interface IReportsRepository {
        public Task<List<SOSItem>> GetSOSItems();
    }
}
