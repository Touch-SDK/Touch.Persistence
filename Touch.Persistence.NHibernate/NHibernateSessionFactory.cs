using System;
using NHibernate;
using NHibernate.Cfg;

namespace Touch.Persistence
{
    /// <summary>
    /// NHibernate session factory wrapper.
    /// </summary>
    sealed public class NHibernateSessionFactory
    {
        #region .ctor
        public NHibernateSessionFactory(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory");
            _sessionFactory = sessionFactory;
        }
        #endregion

        #region Data
        /// <summary>
        /// Session factory.
        /// </summary>
        private readonly ISessionFactory _sessionFactory;
        #endregion

        #region Public methods
        /// <summary>
        /// Session factory instance.
        /// </summary>
        public ISessionFactory Factory
        {
            get { return _sessionFactory; }
        }
        
        /// <summary>
        /// Open new session.
        /// </summary>
        /// <returns></returns>
        public ISession OpenSession()
        {
            return _sessionFactory.OpenSession();
        }

        /// <summary>
        /// Get current session.
        /// </summary>
        /// <returns></returns>
        public ISession GetCurrentSession()
        {
            return _sessionFactory.GetCurrentSession();
        }
        #endregion
    }
}
