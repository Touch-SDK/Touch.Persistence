using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;

namespace Touch.Persistence
{
    /// <summary>
    /// DynamoDB context provider.
    /// </summary>
    sealed public class DynamoContextProvider : IContextProvider
    {
        #region .ctor
        public DynamoContextProvider(AWSCredentials credentials, RegionEndpoint region, bool consistentReads = false)
        {
            if (credentials == null) throw new ArgumentNullException("credentials");
            if (region == null) throw new ArgumentNullException("region");

            var client = new AmazonDynamoDBClient(credentials, region);
            _context = new DynamoContext(client, consistentReads);
        }
        #endregion

        #region Data
        /// <summary>
        /// DynamoDB context.
        /// </summary>
        private readonly DynamoContext _context;
        #endregion

        #region IContextProvider implementation
        public IContext GetContext()
        {
            return _context;
        }
        #endregion
    }
}
