using System;

namespace Touch.Persistence
{
    sealed class NHibernateTransaction : ITransaction
    {
        #region .ctor
        public NHibernateTransaction(NHibernate.ITransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            _transaction = transaction;
        }
        #endregion

        #region Data
        private readonly NHibernate.ITransaction _transaction;
        #endregion

        #region ITransaction members
        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            if (_transaction.IsActive)
                Rollback();

            _transaction.Dispose();
        } 
        #endregion
    }
}
