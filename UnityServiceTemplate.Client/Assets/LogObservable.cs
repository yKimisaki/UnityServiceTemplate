using System;
using UniRx;
using UniRx.Operators;
using UnityEngine;
using UniRxLogger = UniRx.Diagnostics.Logger;

namespace Tonari.UniRx.Diagnostics
{
    internal class LogObservable<T> : OperatorObservableBase<T>
    {
        private readonly IObservable<T> _source;
        private readonly ILogger _logger;
        private readonly Func<T, string> _formatter;

        public LogObservable(IObservable<T> source, ILogger logger, Func<T, string> formatter) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this._source = source;
            this._logger = logger;
            this._formatter = formatter;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Log(this, observer, cancel).Run();
        }

        private class Log : OperatorObserverBase<T, T>
        {
            private static readonly UniRxLogger _defaultLogger = new UniRxLogger("Log");

            private readonly LogObservable<T> _parent;

            public Log(LogObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this._parent = parent;
            }

            public IDisposable Run()
            {
                return this._parent._source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
#if DEBUG
                try
                {
                    if (this._parent._logger != null)
                    {
                        if (this._parent._formatter != null)
                            this._parent._logger.Log(this._parent._formatter(value));
                        else
                            this._parent._logger.Log(value);
                    }
                    else
                    {
                        if (this._parent._formatter != null)
                            _defaultLogger.Log(this._parent._formatter(value));
                        else
                            _defaultLogger.Log(value);
                    }
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { this.Dispose(); };
                    return;
                }
#endif
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
#if DEBUG
                try
                {
                    if (this._parent._logger != null)
                    {
                        this._parent._logger.LogException(error);

                        return;
                    }

                    _defaultLogger.Exception(error);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { this.Dispose(); };
                    return;
                }
#endif
                try { observer.OnError(error); } finally { this.Dispose(); };
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { this.Dispose(); };
            }
        }
    }

    public static class LogObservableExtensions
    {
        public static IObservable<T> Log<T>(this IObservable<T> source)
        {
            return new LogObservable<T>(source, null, null);
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, Color color)
        {
            return new LogObservable<T>(source, null, x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">{0}</COLOR>", x));
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, ILogger logger)
        {
            return new LogObservable<T>(source, logger, null);
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, UniRxLogger logger)
        {
            return new LogObservable<T>(source, new UniRxILoggerWrapper(logger), null);
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, ILogger logger, Color color)
        {
            return new LogObservable<T>(source, logger, x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">{0}</COLOR>", x));
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, UniRxLogger logger, Color color)
        {
            return new LogObservable<T>(source, new UniRxILoggerWrapper(logger), x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">{0}</COLOR>", x));
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, string format)
        {
            return new LogObservable<T>(source, null, x => string.Format(format, x));
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, string format, Color color)
        {
            return new LogObservable<T>(source, null, x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + format + "</COLOR>", x));
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, ILogger logger, string format)
        {
            return new LogObservable<T>(source, logger, x => string.Format(format, x));
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, UniRxLogger logger, string format)
        {
            return new LogObservable<T>(source, new UniRxILoggerWrapper(logger), x => string.Format(format, x));
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, ILogger logger, string format, Color color)
        {
            return new LogObservable<T>(source, logger, x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + format + "</COLOR>", x));
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, UniRxLogger logger, string format, Color color)
        {
            return new LogObservable<T>(source, new UniRxILoggerWrapper(logger), x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + format + "</COLOR>", x));
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, Func<T, string> formatter)
        {
            return new LogObservable<T>(source, null, formatter);
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, ILogger logger, Func<T, string> formatter)
        {
            return new LogObservable<T>(source, logger, formatter);
        }

        public static IObservable<T> Log<T>(this IObservable<T> source, UniRxLogger logger, Func<T, string> formatter)
        {
            return new LogObservable<T>(source, new UniRxILoggerWrapper(logger), formatter);
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source)
        {
            return new LogObservable<T>(source, null, null).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, Color color)
        {
            return new LogObservable<T>(source, null, x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">{0}</COLOR>", x)).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, ILogger logger)
        {
            return new LogObservable<T>(source, logger, null).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, UniRxLogger logger)
        {
            return new LogObservable<T>(source, new UniRxILoggerWrapper(logger), null).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, ILogger logger, Color color)
        {
            return new LogObservable<T>(source, logger, x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">{0}</COLOR>", x)).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, UniRxLogger logger, Color color)
        {
            return new LogObservable<T>(source, new UniRxILoggerWrapper(logger), x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">{0}</COLOR>", x)).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, string format)
        {
            return new LogObservable<T>(source, null, x => string.Format(format, x)).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, string format, Color color)
        {
            return new LogObservable<T>(source, null, x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + format + "</COLOR>", x)).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, ILogger logger, string format)
        {
            return new LogObservable<T>(source, logger, x => string.Format(format, x)).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, UniRxLogger logger, string format)
        {
            return new LogObservable<T>(source, new UniRxILoggerWrapper(logger), x => string.Format(format, x)).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, ILogger logger, string format, Color color)
        {
            return new LogObservable<T>(source, logger, x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + format + "</COLOR>", x)).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, UniRxLogger logger, string format, Color color)
        {
            return new LogObservable<T>(source, new UniRxILoggerWrapper(logger), x => string.Format("<COLOR=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + format + "</COLOR>", x)).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, Func<T, string> formatter)
        {
            return new LogObservable<T>(source, null, formatter).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, ILogger logger, Func<T, string> formatter)
        {
            return new LogObservable<T>(source, logger, formatter).Subscribe();
        }

        public static IDisposable SubscribeToLog<T>(this IObservable<T> source, UniRxLogger logger, Func<T, string> formatter)
        {
            return new LogObservable<T>(source, new UniRxILoggerWrapper(logger), formatter).Subscribe();
        }
    }
    
    public class UniRxILoggerWrapper : ILogger
    {
        public ILogHandler logHandler { get; set; }
        public bool logEnabled { get; set; }
        public LogType filterLogType { get; set; }

        private UniRxLogger _originalLogger;

        public UniRxILoggerWrapper(UniRxLogger originalLogger)
        {
            this._originalLogger = originalLogger;
        }

        public bool IsLogTypeAllowed(LogType logType)
        {
            return logType == LogType.Log
                || logType == LogType.Warning
                || logType == LogType.Error
                || logType == LogType.Exception;
        }

        public void Log(LogType logType, object message)
        {
            if (!logEnabled)
                return;

            if (logType == LogType.Log)
                this._originalLogger.Log(message);
            else if (logType == LogType.Warning)
                this._originalLogger.Warning(message);
            else if (logType == LogType.Error)
                this._originalLogger.Error(message);
            else if (logType == LogType.Exception)
                this._originalLogger.Exception(new Exception(message.ToString()));
        }

        public void Log(LogType logType, object message, UnityEngine.Object context)
        {
            if (!logEnabled)
                return;

            if (logType == LogType.Log)
                this._originalLogger.Log(message, context);
            else if (logType == LogType.Warning)
                this._originalLogger.Warning(message, context);
            else if (logType == LogType.Error)
                this._originalLogger.Error(message, context);
            else if (logType == LogType.Exception)
                this._originalLogger.Exception(new Exception(message.ToString()), context);
        }

        public void Log(LogType logType, string tag, object message)
        {
            if (!logEnabled)
                return;

            if (logType == LogType.Log)
                this._originalLogger.Log("[" + tag + "]" + message);
            else if (logType == LogType.Warning)
                this._originalLogger.Warning("[" + tag + "]" + message);
            else if (logType == LogType.Error)
                this._originalLogger.Error("[" + tag + "]" + message);
            else if (logType == LogType.Exception)
                this._originalLogger.Exception(new Exception("[" + tag + "]" + message));
        }

        public void Log(LogType logType, string tag, object message, UnityEngine.Object context)
        {
            if (!logEnabled)
                return;

            if (logType == LogType.Log)
                this._originalLogger.Log("[" + tag + "]" + message, context);
            else if (logType == LogType.Warning)
                this._originalLogger.Warning("[" + tag + "]" + message, context);
            else if (logType == LogType.Error)
                this._originalLogger.Error("[" + tag + "]" + message, context);
            else if (logType == LogType.Exception)
                this._originalLogger.Exception(new Exception("[" + tag + "]" + message), context);
        }

        public void Log(object message)
        {
            if (logEnabled)
                this._originalLogger.Log(message);
        }

        public void Log(string tag, object message)
        {
            if (logEnabled)
                this._originalLogger.Log("[" + tag + "]" + message);
        }

        public void Log(string tag, object message, UnityEngine.Object context)
        {
            if (logEnabled)
                this._originalLogger.Log("[" + tag + "]" + message, context);
        }

        public void LogError(string tag, object message)
        {
            if (logEnabled)
                this._originalLogger.Error("[" + tag + "]" + message);
        }

        public void LogError(string tag, object message, UnityEngine.Object context)
        {
            if (logEnabled)
                this._originalLogger.Error("[" + tag + "]" + message, context);
        }

        public void LogException(Exception exception)
        {
            if (logEnabled)
                this._originalLogger.Exception(exception);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            if (logEnabled)
                this._originalLogger.Exception(exception, context);
        }

        public void LogFormat(LogType logType, string format, params object[] args)
        {
            if (!logEnabled)
                return;

            if (logType == LogType.Log)
                this._originalLogger.LogFormat(format, args);
            else if (logType == LogType.Warning)
                this._originalLogger.WarningFormat(format, args);
            else if (logType == LogType.Error)
                this._originalLogger.ErrorFormat(format, args);
            else if (logType == LogType.Exception)
                this._originalLogger.Exception(new Exception(string.Format(format, args)));
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            if (!logEnabled)
                return;

            if (logType == LogType.Log)
                this._originalLogger.LogFormat(format, args, context);
            else if (logType == LogType.Warning)
                this._originalLogger.WarningFormat(format, args, context);
            else if (logType == LogType.Error)
                this._originalLogger.ErrorFormat(format, args, context);
            else if (logType == LogType.Exception)
                this._originalLogger.Exception(new Exception(string.Format(format, args)), context);
        }

        public void LogWarning(string tag, object message)
        {
            if (logEnabled)
                this._originalLogger.Warning("[" + tag + "]" + message);
        }

        public void LogWarning(string tag, object message, UnityEngine.Object context)
        {
            if (logEnabled)
                this._originalLogger.Warning("[" + tag + "]" + message, context);
        }
    }
}
