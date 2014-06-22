using System;

namespace Touch.Persistence
{
    /// <summary>
    /// In-memory store.
    /// </summary>
    /// <typeparam name="T">Document type.</typeparam>
    sealed public class InMemoryStore<T> : AbstractStore<T>
        where T : class, IDocument
    {
        #region .ctor
        public InMemoryStore(InMemoryContextProvider contextProvider)
            : base(contextProvider)
        {
        }
        #endregion

        #region Abstract methods implementation
        protected override T TryCatch(Func<T> func)
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
