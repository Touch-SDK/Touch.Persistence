using System;
using Amazon.ElastiCacheCluster;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Touch.Logging;

namespace Touch.Persistence
{
    sealed public class AwsMemcachedContextProvider : IContextProvider, IDisposable
    {
        #region .ctor
        public AwsMemcachedContextProvider(ILoggerProvider loggerProvider, string connectionString)
        {
            if (loggerProvider == null) throw new ArgumentNullException("loggerProvider");
            _loggerProvider = loggerProvider;

            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");
            var config = new AwsMemcachedConnectionStringBuilder { ConnectionString = connectionString };

            if (config.LoggingEnabled)
                LogManager.AssignFactory(new EnyimLogFactory(loggerProvider));

            ElastiCacheClusterConfig clientConfig;

            if (!string.IsNullOrEmpty(config.ConfigurationEndpoint))
            {
                Uri endpoint;
                if (!Uri.TryCreate("http://" + config.ConfigurationEndpoint, UriKind.Absolute, out endpoint))
                    throw new ArgumentException("Invalid configuration endpoint.", "connectionString");

                clientConfig = new ElastiCacheClusterConfig(endpoint.Host, endpoint.Port);
            }
            else
            {
                clientConfig = new ElastiCacheClusterConfig();
            }

            _client = new MemcachedClient(clientConfig);
        }
        #endregion

        #region Data
        private readonly MemcachedClient _client;
        private readonly ILoggerProvider _loggerProvider;
        #endregion

        #region IContextProvider implementation
        public IContext GetContext()
        {
            return new MemcachedContext(_client, _loggerProvider.Get<MemcachedContext>());
        }
        #endregion

        #region IDisposable methods
        public void Dispose()
        {
            _client.Dispose();
        } 
        #endregion
    }
}
