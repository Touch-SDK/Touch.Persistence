using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

namespace Touch.Persistence
{
    sealed public class MemcachedContextProvider : IContextProvider, IDisposable
    {
        #region .ctor
        public MemcachedContextProvider()
        {
            const string configEndpoint = @"touchcache.tdl6kj.cfg.euw1.cache.amazonaws.com";

            Uri endpoint;
            
            if (!Uri.TryCreate(configEndpoint, UriKind.Absolute, out endpoint))
                throw new ArgumentException();

            var endpointIp = Dns.GetHostAddresses(endpoint.DnsSafeHost).First();

            var config = new MemcachedClientConfiguration();
            config.Servers.Add(new IPEndPoint(endpointIp, endpoint.Port));
            config.Protocol = MemcachedProtocol.Text;

            _configurationClient = new MemcachedClient(config);
        }
        #endregion

        #region Data

        private readonly MemcachedClient _configurationClient;

        /// <summary>
        /// Memcached context.
        /// </summary>
        private MemcachedContext _context;
        #endregion

        #region IContextProvider implementation
        public IContext GetContext()
        {
            return _context;
        }
        #endregion

        #region Private methods
        private void ResolveEndpoints()
        {
            using (var client = new TcpClient("test.tdl6kj.cfg.euw1.cache.amazonaws.com", 11211))
            {
                var data = Encoding.ASCII.GetBytes("config get cluster"); 

                using (var stream = client.GetStream())
                {
                    stream.Write(data, 0, data.Length);

                    using (var reader = new StreamReader(stream))
                    {
                        var result = reader.ReadToEnd();
                    }
                }
            }
        }
        #endregion

        #region IDisposable methods
        public void Dispose()
        {
            _configurationClient.Dispose();
        } 
        #endregion
    }
}
