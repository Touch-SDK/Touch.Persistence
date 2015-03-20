using MongoDB.Driver;

namespace Touch.Persistence
{
    /// <summary>
    /// MongoDB context provider.
    /// </summary>
    sealed public class MongoContextProvider : IContextProvider
    {
        #region .ctor
        public MongoContextProvider(string connectionString, string database)
        {
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            
            var db = server.GetDatabase(database);
            
            _context = new MongoContext(db);
        }
        #endregion

        #region Data
        private readonly MongoContext _context;
        #endregion

        public void RegisterType<T>()
            where T : class, IDocument
        {
            _context.RegisterType<T>();
        }

        #region IContextProvider implementation
        public IContext GetContext()
        {
            return _context;
        }
        #endregion
    }
}
