using System;
using System.Diagnostics;
using NHibernate;

namespace Touch.Persistence
{
    /// <summary>
    /// NHibernate repository.
    /// </summary>
    /// <typeparam name="T">Repository entity type.</typeparam>
    sealed public class NHibernateRepository<T> : AbstractRepository<T>
        where T : class, IEntity, new()
    {
        #region .ctor
        public NHibernateRepository(IPersistenceProvider provider)
            : base(provider)
        {
        }
        #endregion

        #region Abstract methods implementation
        [DebuggerStepThrough]
        override protected T TryCatch(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (UnresolvableObjectException exception)
            {
                throw new Exceptions.ObjectNotFoundException(exception.Message, exception);
            }
            catch (NonUniqueObjectException exception)
            {
                throw new Exceptions.ObjectNotUniqueException(exception.Message, exception);
            }
            catch (Exception exception)
            {
                if (!String.IsNullOrEmpty(exception.Message) && exception.Message.Contains("Cannot insert duplicate key in object"))
                {
                    throw new Exceptions.ObjectNotUniqueException(exception.Message, exception);
                }

                if (exception.InnerException != null && !String.IsNullOrEmpty(exception.InnerException.Message)
                    && exception.InnerException.Message.Contains("Cannot insert duplicate key in object"))
                {
                    throw new Exceptions.ObjectNotUniqueException(exception.Message, exception);
                }

                throw;
            }
        }
        #endregion
    }

    /// <summary>
    /// NHibernate business repository.
    /// </summary>
    /// <typeparam name="T">Repository business entity type.</typeparam>
    sealed public class NHibernateBusinessRepository<T> : AbstractBusinessRepository<T>
        where T : class, IBusinessEntity, new()
    {
        #region .ctor
        public NHibernateBusinessRepository(IPersistenceProvider provider)
            : base(provider)
        {
        }
        #endregion

        #region Abstract methods implementation
        [DebuggerStepThrough]
        override public void Destroy(Guid token)
        {
            var entity = (BusinessEntity)Activator.CreateInstance(typeof(T));
            entity.Id = token;
            Destroy(entity as T);
        }

        [DebuggerStepThrough]
        override protected T TryCatch(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (UnresolvableObjectException exception)
            {
                throw new Exceptions.ObjectNotFoundException(exception.Message, exception);
            }
            catch (NonUniqueObjectException exception)
            {
                throw new Exceptions.ObjectNotUniqueException(exception.Message, exception);
            }
            catch (Exception exception)
            {
                if (!String.IsNullOrEmpty(exception.Message) && exception.Message.Contains("Cannot insert duplicate key in object"))
                {
                    throw new Exceptions.ObjectNotUniqueException(exception.Message, exception);
                }

                if (exception.InnerException != null && !String.IsNullOrEmpty(exception.InnerException.Message)
                    && exception.InnerException.Message.Contains("Cannot insert duplicate key in object"))
                {
                    throw new Exceptions.ObjectNotUniqueException(exception.Message, exception);
                }

                throw;
            }
        }
        #endregion
    }
}
