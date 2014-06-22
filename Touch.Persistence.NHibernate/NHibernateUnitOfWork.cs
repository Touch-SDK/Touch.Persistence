using System;
using System.Data;
using NHibernate.Context;
using Touch.Persistence;

namespace Touch.ServiceModel
{
    /// <summary>
    /// NHibernate service unit of work.
    /// </summary>
    sealed public class NHibernateUnitOfWork : IUnitOfWork
    {
        #region .ctor
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factory">Session factory.</param>
        /// <param name="commitOnly">If <c>false</c>, automatically opens a new session and starts a transaction.
        /// If <c>true</c>, doesn't start any sessions.</param>
        public NHibernateUnitOfWork(NHibernateSessionFactory factory, bool commitOnly=true)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            _factory = factory;

            if (!commitOnly)
            {
                var session = _factory.OpenSession();
                session.BeginTransaction();

                CurrentSessionContext.Bind(session);
            }
        }
        #endregion

        #region Data
        /// <summary>
        /// Session factory.
        /// </summary>
        private readonly NHibernateSessionFactory _factory;
        #endregion

        #region IUnitOfWork implementation
        public void Commit()
        {
            if (!CurrentSessionContext.HasBind(_factory.Factory)) return;

            var session = _factory.GetCurrentSession();

            if (session.Transaction.IsActive)
            {
                try
                {
                    session.Transaction.Commit();
                }
                finally
                {
                    session.Transaction.Dispose();
                }
            }
        }

        public void Dispose()
        {
            if (!CurrentSessionContext.HasBind(_factory.Factory)) return;

            var session = CurrentSessionContext.Unbind(_factory.Factory);
            session.Dispose();
        }
        #endregion
    }
}
