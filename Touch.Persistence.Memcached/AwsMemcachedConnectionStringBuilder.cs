using System;
using System.Data.Common;
using Amazon;

namespace Touch.Persistence
{
    sealed public class AwsMemcachedConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Properties
        public string ConfigurationEndpoint
        {
            get { return ContainsKey("ConfigurationEndpoint") ? this["ConfigurationEndpoint"] as string : null; }
            set { this["ConfigurationEndpoint"] = value; }
        }

        public bool LoggingEnabled
        {
            get { return ContainsKey("LoggingEnabled") && Convert.ToBoolean(this["LoggingEnabled"]); }
            set { this["LoggingEnabled"] = value; }
        }
        #endregion
    }
}
