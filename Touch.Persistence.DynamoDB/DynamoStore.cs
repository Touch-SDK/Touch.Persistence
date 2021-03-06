﻿using System;

namespace Touch.Persistence
{
    /// <summary>
    /// DynamoDB store.
    /// </summary>
    /// <typeparam name="T">Document type.</typeparam>
    sealed public class DynamoStore<T> : AbstractStore<T>
        where T : class, IDocument
    {
        #region .ctor
        public DynamoStore(DynamoContextProvider contextProvider)
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
                var ex = HandleException(exception);

                if (ex != null)
                    throw ex;

                throw;
            }
        }

        private static Exception HandleException(Exception exception)
        {
            if (!String.IsNullOrEmpty(exception.Message) && exception.Message.Contains("Cannot insert duplicate key in object"))
            {
                return new Exceptions.ObjectNotUniqueException(exception.Message, exception);
            }

            if (exception.InnerException != null && !String.IsNullOrEmpty(exception.InnerException.Message)
                && exception.InnerException.Message.Contains("Cannot insert duplicate key in object"))
            {
                return new Exceptions.ObjectNotUniqueException(exception.Message, exception);
            }

            return null;
        }
        #endregion

        
    }
}
