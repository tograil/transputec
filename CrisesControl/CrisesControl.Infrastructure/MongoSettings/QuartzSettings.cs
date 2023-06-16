using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.MongoSettings {
    public class QuartzSettings {

        [DisplayName("quartz.scheduler.instanceName")]
        public string PropertySchedulerInstanceName { get; set; }

        [DisplayName("quartz.scheduler.instanceId")]
        public string PropertySchedulerInstanceId { get; set; }

        [DisplayName("quartz.threadPool.type")]
        public string PropertyThreadPoolType { get; set; }

        [DisplayName("quartz.serializer.type")]
        public string PropertySerializerType { get; set; }

        [DisplayName("tablePrefix")]
        public string PropertyTablePrefix { get; set; }

        [DisplayName("provider")]
        public string PropertyDataSourceProvider { get; set; }

        [DisplayName("connectionStringName")]
        public string PropertyDataSourceConnectionStringName { get; set; }
    }
}
