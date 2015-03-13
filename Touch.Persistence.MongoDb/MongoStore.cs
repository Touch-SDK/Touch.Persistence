using System;

namespace Touch.Persistence
{
    /// <summary>
    /// MongoDB store.
    /// </summary>
    /// <typeparam name="T">Document type.</typeparam>
    sealed public class MongoStore<T> : AbstractStore<T>
        where T : class, IDocument
    {
        #region .ctor
        public MongoStore(MongoContextProvider contextProvider)
            : base(contextProvider)
        {
            contextProvider.RegisterType<T>();
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
