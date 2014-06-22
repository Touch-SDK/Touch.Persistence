using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace Touch.Persistence
{
    /// <summary>
    /// NHibernate persistence.
    /// </summary>
    sealed public class NHibernatePersistence : IPersistence
    {
        #region .ctor
        public NHibernatePersistence(Func<ISession> sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }
        #endregion

        #region Data
        /// <summary>
        /// NHibernate session provider.
        /// </summary>
        private readonly Func<ISession> _sessionProvider;

        /// <summary>
        /// NHibernate session instance.
        /// </summary>
        public ISession Session { get { return _sessionProvider.Invoke(); } }
        #endregion

        #region IPersistence implementation
        public T Get<T>(object id) where T : class, IEntity
        {
            return Session.Get<T>(id);
        }

        public void Delete<T>(T obj) where T : class, IEntity
        {
            Session.Delete(obj);
        }

        public void Save<T>(T obj) where T : class, IEntity
        {
            Session.Save(obj);
        }

        public void Update<T>(T obj) where T : class, IEntity
        {
            Session.Update(obj);
        }

        public T Proxy<T>(object id) where T : class, IEntity
        {
            return Session.Load<T>(id);
        }

        public IQueryable<T> Query<T>() where T : class, IEntity
        {
            return Session.Query<T>();
        }

        public ITransaction BeginTransaction()
        {
            return new NHibernateTransaction(Session.BeginTransaction());
        }
        #endregion
    }
}
