using System;
using Enyim.Caching;
using Touch.Logging;

namespace Touch.Persistence
{
    internal class EnyimLogFactory : ILogFactory
    {
        public EnyimLogFactory(ILoggerProvider provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            _provider = provider;
        }

        private readonly ILoggerProvider _provider;

        public ILog GetLogger(Type type)
        {
            var logger = _provider.Get(type);
            return new EnyimLog(logger);
        }

        public ILog GetLogger(string name)
        {
            var logger = _provider.Get<AwsMemcachedContextProvider>();
            return new EnyimLog(logger);
        }
    }
}
