/*
Copyright 2015 Upper Setting Corporation

This file is part of DotNetOpenServer SDK.

DotNetOpenServer SDK is free software: you can redistribute it and/or modify it
under the terms of the GNU General Public License as published by the Free
Software Foundation, either version 3 of the License, or (at your option) any
later version.

DotNetOpenServer SDK is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License along with
DotNetOpenServer SDK. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Reflection;
using log4net;
using log4net.Core;

namespace US.OpenServer
{
    /// <summary>
    /// Class that logs to log4net.
    /// </summary>
    public class Log4NetLogger : Logger
    {
        #region Private Variables
        /// <summary>
        /// The log4Net logger.
        /// </summary>
        private log4net.ILog log;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Creates a Logger object.
        /// </summary>
        /// <remarks>
        /// Reads the log4net configuration from the app.config file.  For example:
        /// <code>
        /// <![CDATA[
        /// <?xml version="1.0"?>
        /// <configuration>
        ///   <configSections>
        /// 
        ///     ...
        /// 
        ///     <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler,Log4net"/>
        /// 
        ///     ...
        /// 
        ///   </configSections>
        /// 
        ///   ...
        /// 
        ///   <log4net>
        ///     <root>
        ///       <level value="ALL"/>
        ///       <appender-ref ref="ConsoleAppender" />
        ///       <appender-ref ref="RollingFileAppender"/>
        ///     </root>
        /// 
        ///     <appender name="ConsoleAppender" type="log4net.Appender.ConsoleAppender">
        ///       <layout type="log4net.Layout.PatternLayout">
        ///         <conversionPattern value="%date %thread %level - %message%newline"/>
        ///       </layout>
        ///     </appender>
        ///     
        ///     <appender name="RollingFileAppender" type="log4net.Appender.RollingFileAppender">
        ///       <file value="application.log"/>
        ///       <appendToFile value="true"/>
        ///       <rollingStyle value="Size"/>
        ///       <maxSizeRollBackups value="5"/>
        ///       <maximumFileSize value="10MB"/>
        ///       <staticLogFileName value="true"/>
        ///       <layout type="log4net.Layout.PatternLayout">
        ///         <conversionPattern value="%date %thread %level - %message%newline"/>
        ///       </layout>
        ///     </appender>
        ///   </log4net>
        /// 
        ///   ...
        /// 
        /// </configuration>
        /// ]]>
        /// </code>
        /// 
        /// Then Logs a message stating the application has started and then another
        /// message showing the name of the user the process is running under.</remarks>
        /// <param name="name">A string that specifies the name of the application.</param>
        public Log4NetLogger(string name)
            : base ()
        {
            log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            Log(Level.Info, string.Format("{0} started", name));
            Log(Level.Info, string.Format("Running as {0}", Environment.UserName));
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">The level of the message.</param>
        /// <param name="message">The message.</param>
        public override void Log(US.OpenServer.Level level, string message)
        {
            base.Log(level, message);

            if (level == US.OpenServer.Level.Debug && !LogDebug)
                return;

            if (log == null)
                return;
            
            LoggingEventData d = new LoggingEventData();
            d.TimeStamp = DateTime.Now;
            d.Level = GetLevelFromLevel(level);
            d.Message = message;
            LoggingEvent le = new LoggingEvent(d);
            log.Logger.Log(le);
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Converts a <see cref="US.OpenServer.Level"/> enumeration to log4net.Core.Level object.
        /// </summary>
        /// <param name="level">The Level to convert.</param>
        /// <returns>A log4net.Core.Level object.</returns>
        private log4net.Core.Level GetLevelFromLevel(US.OpenServer.Level level)
        {
            switch (level)
            {
                case US.OpenServer.Level.Debug:
                    return log4net.Core.Level.Debug;
                case US.OpenServer.Level.Info:
                    return log4net.Core.Level.Info;
                case US.OpenServer.Level.Notice:
                    return log4net.Core.Level.Notice;
                case US.OpenServer.Level.Critical:
                    return log4net.Core.Level.Critical;
                case US.OpenServer.Level.Error:
                    return log4net.Core.Level.Error;
                default:
                    return log4net.Core.Level.Info;
            }
        }
        #endregion
    }
}