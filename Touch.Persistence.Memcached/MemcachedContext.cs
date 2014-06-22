using System;
using Enyim.Caching;
using Enyim.Caching.Configuration;

namespace Touch.Persistence
{
    sealed public class MemcachedContext : IContext
    {
        #region .ctor
        internal MemcachedContext(IMemcachedClientConfiguration config)
        {
            if (config == null) throw new ArgumentNullException("config");

            _client = new MemcachedClient(config);
        }
        #endregion

        #region Data
        private readonly MemcachedClient _client;
        #endregion

        #region IContext implementation
        public T Get<T>(string hashKey) where T : class, IDocument
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T target) where T : class, IDocument
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(string hashKey) where T : class, IDocument
        {
            throw new NotImplementedException();
        }

        public void Store(IDocument target)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
