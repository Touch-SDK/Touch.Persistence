using System;
using Enyim.Caching;
using Touch.Logging;

namespace Touch.Persistence
{
    internal class EnyimLog : ILog
    {
        public EnyimLog(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            _logger = logger;
        }

        private readonly ILogger _logger;

        public void Debug(object message, Exception exception)
        {
            _logger.Debug(message.ToString(), exception);
        }

        public void Debug(object message)
        {
            _logger.Debug(message.ToString());
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.DebugFormat(provider, format, args);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.DebugFormat(format, arg0, arg1, arg2);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            _logger.DebugFormat(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0)
        {
            _logger.DebugFormat(format, arg0);
        }

        public void Error(object message, Exception exception)
        {
            _logger.Error(message.ToString(), exception);
        }

        public void Error(object message)
        {
            _logger.Error(message.ToString());
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.ErrorFormat(provider, format, args);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _logger.ErrorFormat(format, args);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.ErrorFormat(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            _logger.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0)
        {
            _logger.ErrorFormat(format, arg0);
        }

        public void Fatal(object message, Exception exception)
        {
            _logger.Fatal(message.ToString(), exception);
        }

        public void Fatal(object message)
        {
            _logger.Fatal(message.ToString());
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.FatalFormat(provider, format, args);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _logger.FatalFormat(format, args);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.FatalFormat(format, arg0, arg1, arg2);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            _logger.FatalFormat(format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0)
        {
            _logger.FatalFormat(format, arg0);
        }

        public void Info(object message, Exception exception)
        {
            _logger.Info(message.ToString(), exception);
        }

        public void Info(object message)
        {
            _logger.Info(message.ToString());
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.InfoFormat(provider, format, args);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.InfoFormat(format, arg0, arg1, arg2);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            _logger.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0)
        {
            _logger.InfoFormat(format, arg0);
        }

        public bool IsDebugEnabled
        {
            get { return _logger.IsDebugEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return _logger.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return _logger.IsFatalEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return _logger.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return _logger.IsWarnEnabled; }
        }

        public void Warn(object message, Exception exception)
        {
            _logger.Warn(message.ToString(), exception);
        }

        public void Warn(object message)
        {
            _logger.Warn(message.ToString());
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.WarnFormat(provider, format, args);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.WarnFormat(format, arg0, arg1, arg2);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            _logger.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0)
        {
            _logger.WarnFormat(format, arg0);
        }
    }
}
