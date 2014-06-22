using System;
using System.Runtime.Caching;

namespace Touch.Persistence
{
    sealed class InMemoryContext : IContext
    {
        #region .ctor
        internal InMemoryContext()
        {
            _cache = MemoryCache.Default;

            _defaultPolicy = new CacheItemPolicy
                                 {
                                     SlidingExpiration = TimeSpan.FromDays(1)
                                 };
        }
        #endregion

        #region Data
        /// <summary>
        /// Object cache.
        /// </summary>
        private readonly ObjectCache _cache;

        private readonly CacheItemPolicy _defaultPolicy;
        #endregion

        #region IContext implementation
        public T Get<T>(string hashKey) where T : class, IDocument
        {
            var value = _cache.Get(hashKey);

            return value != null 
                ? (T)value
                : null;
        }

        public void Delete<T>(T target) where T : class, IDocument
        {
            _cache.Remove(target.HashKey);
        }

        public void Delete<T>(string hashKey) where T : class, IDocument
        {
            _cache.Remove(hashKey);
        }

        void IContext.Store(IDocument target)
        {
            var value = new CacheItem(target.HashKey, target);

            _cache.Set(value, _defaultPolicy);
        }
        #endregion
    }
}
