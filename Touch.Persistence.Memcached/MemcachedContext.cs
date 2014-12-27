using System;
using System.Collections.Generic;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Touch.Logging;

namespace Touch.Persistence
{
    sealed public class MemcachedContext : IContext
    {
        #region .ctor
        internal MemcachedContext(MemcachedClient client, ILogger logger)
        {
            if (client == null) throw new ArgumentNullException("client");
            _client = client;

            if (logger == null) throw new ArgumentNullException("logger");
            _logger = logger;
        }
        #endregion

        #region Data
        private readonly MemcachedClient _client;
        private readonly ILogger _logger;
        #endregion

        #region IContext implementation
        public T Get<T>(string hashKey) where T : class, IDocument
        {
            _logger.DebugFormat("Getting value for key {0}", hashKey);

            var result = _client.Get<T>(hashKey);

            _logger.DebugFormat("Get value result for key {0}: {1}", hashKey, result);

            return result;
        }

        public void Delete<T>(T target) where T : class, IDocument
        {
            _logger.DebugFormat("Deleting value for key {0}", target.HashKey);

            var result = _client.Remove(target.HashKey);

            _logger.DebugFormat("Delete value result for key {0}: {1}", target.HashKey, result);
        }

        public void Delete<T>(string hashKey) where T : class, IDocument
        {
            _logger.DebugFormat("Deleting value for key {0}", hashKey);

            var result = _client.Remove(hashKey);

            _logger.DebugFormat("Delete value result for key {0}: {1}", hashKey, result);
        }

        public void Store(IDocument target)
        {
            _logger.DebugFormat("Storing value for key {0}", target.HashKey);

            var result = _client.Store(StoreMode.Set, target.HashKey, target);

            _logger.DebugFormat("Store value result for key {0}: {1}", target.HashKey, result);
        }

        public IEnumerable<T> Query<T>(KeyValuePair<string, string> condition) where T : class, IDocument
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
