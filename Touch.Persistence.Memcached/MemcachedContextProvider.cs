using System;
using Enyim.Caching;
using Touch.Logging;

namespace Touch.Persistence
{
    sealed public class MemcachedContextProvider : IContextProvider, IDisposable
    {
        #region .ctor
        public MemcachedContextProvider(ILoggerProvider loggerProvider)
        {
            if (loggerProvider == null) throw new ArgumentNullException("loggerProvider");
            _loggerProvider = loggerProvider;

            _client = new MemcachedClient();
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
