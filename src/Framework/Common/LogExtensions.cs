using System;
using log4net;

namespace Kinect.Common
{
    /// <summary>
    /// Extension methods for the log4net logging framework
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Logs a message object with the <see cref="log4net.Core.Level.Info"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="message">The message object to log</param>
        public static void IfInfo(this ILog log, object message)
        {
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
        }

        /// <summary>
        /// Logs a message object with the <see cref="log4net.Core.Level.Info"/> level, 
        /// including the stack trace of the System.Exception passed with
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="message">The message object to log</param>
        /// <param name="exception">The exception to log, including its stack trace</param>
        public static void IfInfo(this ILog log, object message, Exception exception)
        {
            if (log.IsInfoEnabled)
            {
                log.Info(message, exception);
            }
        }

        /// <summary>
        /// Logs a formatted message string with the <see cref="log4net.Core.Level.Info"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void IfInfoFormat(this ILog log, string format, params object[] args)
        {
            if (log.IsInfoEnabled)
            {
                log.InfoFormat(format, args);
            }
        }

        /// <summary>
        /// Logs a formatted message string with the <see cref="log4net.Core.Level.Info"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="provider">An <see cref="System.IFormatProvider"/> that supplies culture-specific format information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void IfInfoFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
        {
            if (log.IsInfoEnabled)
            {
                log.InfoFormat(provider, format, args);
            }
        }

        /// <summary>
        /// Logs a message object with the <see cref="log4net.Core.Level.Debug"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="message">The message object to log</param>
        public static void IfDebug(this ILog log, object message)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(message);
            }
        }

        /// <summary>
        /// Logs a message object with the <see cref="log4net.Core.Level.Debug"/> level, 
        /// including the stack trace of the System.Exception passed with
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="message">The message object to log</param>
        /// <param name="exception">The exception to log, including its stack trace</param>
        public static void IfDebug(this ILog log, object message, Exception exception)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(message, exception);
            }
        }

        /// <summary>
        /// Logs a formatted message string with the <see cref="log4net.Core.Level.Debug"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void IfDebugFormat(this ILog log, string format, params object[] args)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat(format, args);
            }
        }

        /// <summary>
        /// Logs a formatted message string with the <see cref="log4net.Core.Level.Debug"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="provider">An <see cref="System.IFormatProvider"/> that supplies culture-specific format information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void IfDebugFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat(provider, format, args);
            }
        }

        /// <summary>
        /// Logs a message object with the <see cref="log4net.Core.Level.Warn"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="message">The message object to log</param>
        public static void IfWarn(this ILog log, object message)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn(message);
            }
        }

        /// <summary>
        /// Logs a message object with the <see cref="log4net.Core.Level.Warn"/> level, 
        /// including the stack trace of the System.Exception passed with
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="message">The message object to log</param>
        /// <param name="exception">The exception to log, including its stack trace</param>
        public static void IfWarn(this ILog log, object message, Exception exception)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn(message, exception);
            }
        }

        /// <summary>
        /// Logs a formatted message string with the <see cref="log4net.Core.Level.Warn"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void IfWarnFormat(this ILog log, string format, params object[] args)
        {
            if (log.IsWarnEnabled)
            {
                log.WarnFormat(format, args);
            }
        }

        /// <summary>
        /// Logs a formatted message string with the <see cref="log4net.Core.Level.Warn"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="provider">An <see cref="System.IFormatProvider"/> that supplies culture-specific format information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void IfWarnFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
        {
            if (log.IsWarnEnabled)
            {
                log.WarnFormat(provider, format, args);
            }
        }

        /// <summary>
        /// Logs a message object with the <see cref="log4net.Core.Level.Error"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="message">The message object to log</param>
        public static void IfError(this ILog log, object message)
        {
            if (log.IsErrorEnabled)
            {
                log.Error(message);
            }
        }

        /// <summary>
        /// Logs a message object with the <see cref="log4net.Core.Level.Error"/> level, 
        /// including the stack trace of the System.Exception passed with
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="message">The message object to log</param>
        /// <param name="exception">The exception to log, including its stack trace</param>
        public static void IfError(this ILog log, object message, Exception exception)
        {
            if (log.IsErrorEnabled)
            {
                log.Error(message, exception);
            }
        }

        /// <summary>
        /// Logs a formatted message string with the <see cref="log4net.Core.Level.Error"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void IfErrorFormat(this ILog log, string format, params object[] args)
        {
            if (log.IsErrorEnabled)
            {
                log.ErrorFormat(format, args);
            }
        }

        /// <summary>
        /// Logs a formatted message string with the <see cref="log4net.Core.Level.Error"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="provider">An <see cref="System.IFormatProvider"/> that supplies culture-specific format information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void IfErrorFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
        {
            if (log.IsErrorEnabled)
            {
                log.ErrorFormat(provider, format, args);
            }
        }

        /// <summary>
        /// Logs a message object with the <see cref="log4net.Core.Level.Fatal"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="message">The message object to log</param>
        public static void IfFatal(this ILog log, object message)
        {
            if (log.IsFatalEnabled)
            {
                log.Fatal(message);
            }
        }

        /// <summary>
        /// Logs a message object with the <see cref="log4net.Core.Level.Fatal"/> level, 
        /// including the stack trace of the System.Exception passed with
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="message">The message object to log</param>
        /// <param name="exception">The exception to log, including its stack trace</param>
        public static void IfFatal(this ILog log, object message, Exception exception)
        {
            if (log.IsFatalEnabled)
            {
                log.Fatal(message, exception);
            }
        }

        /// <summary>
        /// Logs a formatted message string with the <see cref="log4net.Core.Level.Fatal"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void IfFatalFormat(this ILog log, string format, params object[] args)
        {
            if (log.IsFatalEnabled)
            {
                log.FatalFormat(format, args);
            }
        }

        /// <summary>
        /// Logs a formatted message string with the <see cref="log4net.Core.Level.Fatal"/> level
        /// </summary>
        /// <param name="log">Instance of <see cref="log4net.ILog"/></param>
        /// <param name="provider">An <see cref="System.IFormatProvider"/> that supplies culture-specific format information</param>
        /// <param name="format">A string containing zero or more format items</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void IfFatalFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
        {
            if (log.IsFatalEnabled)
            {
                log.FatalFormat(provider, format, args);
            }
        }
    }
}