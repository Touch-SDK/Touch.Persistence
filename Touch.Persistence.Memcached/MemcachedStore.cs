using System;

namespace Touch.Persistence
{
    /// <summary>
    /// Memcached store.
    /// </summary>
    /// <typeparam name="T">Document type.</typeparam>
    public sealed class MemcachedStore<T> : AbstractStore<T>
        where T : class, IDocument
    {
        #region .ctor
        public MemcachedStore(AwsMemcachedContextProvider contextProvider)
            : base(contextProvider)
        {
        }
        #endregion

        #region Abstract methods implementation
        protected override TR TryCatch<TR>(Func<TR> func)
        {
            try
            {
                return func();
            }
            catch (Exception exception)
            {
                throw HandleException(exception);
            }
        }

        private static Exception HandleException(Exception exception)
        {
            return exception;
        }
        #endregion
    }
}
