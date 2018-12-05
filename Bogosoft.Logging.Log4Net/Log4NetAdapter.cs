using System.Threading;
using System.Threading.Tasks;
#if NETSTANDARD2_0
using System;
using System.IO;
using System.Reflection;
using System.Xml;
#endif

namespace Bogosoft.Logging.Log4Net
{
    /// <summary>
    /// An <see cref="ILogger"/> adapter for a log4net logger.
    /// </summary>
    public class Log4NetAdapter : ILogger
    {
#if NETSTANDARD2_0
        /// <summary>
        /// Create a new log4net adapter from configuration settings in the given file.
        /// </summary>
        /// <typeparam name="T">The type the new logger is to be associated with.</typeparam>
        /// <param name="file">A reference to a file containing log4net configuration information.</param>
        /// <param name="watch">True if log4net is to respond to changes in the file; false otherwise.</param>
        /// <returns>A fully configured log4net adapter.</returns>
        public static ILogger Create<T>(FileInfo file, bool watch = true)
        {
            var repository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());

            if (watch)
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(repository, file);
            }
            else
            {
                log4net.Config.XmlConfigurator.Configure(repository, file);
            }

            return new Log4NetAdapter(log4net.LogManager.GetLogger(typeof(T)));
        }

        /// <summary>
        /// Create a new log4net adapter from configuration settings in the given file.
        /// </summary>
        /// <typeparam name="T">The type the new logger is to be associated with.</typeparam>
        /// <param name="filename">The name of a file containing log4net configuration information.</param>
        /// <param name="watch">True if log4net is to respond to changes in the file; false otherwise.</param>
        /// <returns>A fully configured log4net adapter.</returns>
        public static ILogger Create<T>(string filename, bool watch = true)
        {
            return Create<T>(new FileInfo(filename));
        }

        /// <summary>
        /// Create a new log4net adapter from configuration settings in the given stream.
        /// </summary>
        /// <typeparam name="T">The type the new logger is to be associated with.</typeparam>
        /// <param name="configStream">A stream containing log4net configuration information.</param>
        /// <returns>A fully configured log4net adapter.</returns>
        public static ILogger Create<T>(Stream configStream)
        {
            var repository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());

            log4net.Config.XmlConfigurator.Configure(repository, configStream);

            return new Log4NetAdapter(log4net.LogManager.GetLogger(typeof(T)));
        }

        /// <summary>
        /// Create a new log4net adapter from configuration settings in the given stream.
        /// </summary>
        /// <typeparam name="T">The type the new logger is to be associated with.</typeparam>
        /// <param name="configUri">A URI which contains log4net configuration information.</param>
        /// <returns>A fully configured log4net adapter.</returns>
        public static ILogger Create<T>(Uri configUri)
        {
            var repository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());

            log4net.Config.XmlConfigurator.Configure(repository, configUri);

            return new Log4NetAdapter(log4net.LogManager.GetLogger(typeof(T)));
        }

        /// <summary>
        /// Create a new log4net adapter from configuration settings in the given stream.
        /// </summary>
        /// <typeparam name="T">The type the new logger is to be associated with.</typeparam>
        /// <param name="element">An XML element containing log4net configuration information.</param>
        /// <returns>A fully configured log4net adapter.</returns>
        public static ILogger Create<T>(XmlElement element)
        {
            var repository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());

            log4net.Config.XmlConfigurator.Configure(repository, element);

            return new Log4NetAdapter(log4net.LogManager.GetLogger(typeof(T)));
        }
#endif
        readonly log4net.ILog log;

        /// <summary>
        /// Create a new instance of the <see cref="Log4NetAdapter"/> class.
        /// </summary>
        /// <param name="log">A log4net log to be adapted.</param>
        public Log4NetAdapter(log4net.ILog log)
        {
            this.log = log;
        }

        /// <summary>
        /// Log a given message.
        /// </summary>
        /// <param name="message">A message to log.</param>
        public void Log(IMessage message)
        {
            switch (message.Severity)
            {
                case MessageSeverity.Debug:
                    log.DebugFormat(message.Format, message.Values);
                    break;
                case MessageSeverity.Error:
                    log.ErrorFormat(message.Format, message.Values);
                    break;
                case MessageSeverity.Informational:
                    log.InfoFormat(message.Format, message.Values);
                    break;
                case MessageSeverity.Warning:
                    log.WarnFormat(message.Format, message.Values);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Log a given message.
        /// </summary>
        /// <param name="message">A message to log.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>A task representing a possibly asynchronous operation.</returns>
        public Task LogAsync(IMessage message, CancellationToken token)
        {
            return Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                Log(message);
            });
        }
    }
#if NET45
    /// <summary>
    /// A set of static members for dealing with log4net-specific implementations of <see cref="ILogger"/> types.
    /// </summary>
    /// <typeparam name="T">The type to associate to a logger.</typeparam>
    public static class Log4NetAdapter<T>
    {
        /// <summary>
        /// Get a log4net-adapted logger using log4net settings specified in the local XML configuration.
        /// </summary>
        public static ILogger AsConfigured
        {
            get
            {
                return new Log4NetAdapter(log4net.LogManager.GetLogger(typeof(T)));
            }
        }
    }
#endif
}