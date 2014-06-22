using NHibernate;
using NHibernate.Context;

namespace Touch.Persistence
{
    /// <summary>
    /// NHibernate persistence provider.
    /// </summary>
    sealed public class NHibernatePersistenceProvider : IPersistenceProvider
    {
        #region .ctor
        public NHibernatePersistenceProvider(NHibernateSessionFactory factory)
        {
            _factory = factory;
        }
        #endregion

        #region Data
        /// <summary>
        /// NHibernate session factory.
        /// </summary>
        private readonly NHibernateSessionFactory _factory;
        #endregion

        #region IPersistenceProvider implementation
        public IPersistence GetSession()
        {
            return new NHibernatePersistence(GetCurrentSession);
        }
        #endregion

        #region Private methods
        private ISession GetCurrentSession()
        {
            if (!CurrentSessionContext.HasBind(_factory.Factory))
            {
                var session = _factory.OpenSession();
                session.BeginTransaction();

                CurrentSessionContext.Bind(session);
            }

            return _factory.GetCurrentSession();
        }
        #endregion
    }
}
