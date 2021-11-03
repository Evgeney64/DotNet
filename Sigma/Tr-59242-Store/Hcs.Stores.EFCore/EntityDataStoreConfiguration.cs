using Hcs.Configuration;

namespace Hcs.Stores
{
    public class EntityDataStoreConfiguration
    {
        public string ConnectionStringName { get; set; }
        public int CommandTimeout { get; set; }
        public LogConfiguration Log { get; set; }
    }
}
