using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using NHibernate;
using NHibernate.Context;

namespace Touch.Persistence
{
    sealed public class NHibernateServiceBehavior : IServiceBehavior
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factory">Session factory.</param>
        /// <param name="commitOnly">If <c>false</c>, automatically opens a new session and starts a transaction.
        /// If <c>true</c>, doesn't start any sessions.</param>
        public NHibernateServiceBehavior(NHibernateSessionFactory factory, bool commitOnly = true)
        {
            _factory = factory;
            _commitOnly = commitOnly;
        }

        private readonly NHibernateSessionFactory _factory;
        private readonly bool _commitOnly;

        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (var endpoint in channelDispatcher.Endpoints)
                {
                    endpoint.DispatchRuntime.MessageInspectors.Add(new NHibernateWcfContextInitializer(_factory.Factory, _commitOnly));
                }
            }
        }

        public void Validate(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase) { }
    }

    sealed public class NHibernateWcfContextInitializer : IDispatchMessageInspector
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factory">Session factory.</param>
        /// <param name="commitOnly">If <c>false</c>, automatically opens a new session and starts a transaction.
        /// If <c>true</c>, doesn't start any sessions.</param>
        public NHibernateWcfContextInitializer(ISessionFactory factory, bool commitOnly = true)
        {
            _factory = factory;
            _commitOnly = commitOnly;
        }

        private readonly ISessionFactory _factory;
        private readonly bool _commitOnly;

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (!_commitOnly)
            {
                var session = _factory.OpenSession();
                session.BeginTransaction();

                CurrentSessionContext.Bind(session);
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (!CurrentSessionContext.HasBind(_factory)) return;

            try
            {
                var session = _factory.GetCurrentSession();

                if (session.Transaction.IsActive && session.IsDirty())
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
            finally
            {
                var session = CurrentSessionContext.Unbind(_factory);
                session.Dispose();
            }
        }
    }
}
