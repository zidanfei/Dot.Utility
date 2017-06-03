using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Log
{
    /// <summary>
    /// http://www.cnblogs.com/ryanding/archive/2011/05/10/2040561.html
    /// </summary>
    public class LogConfig
    {

        static ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

        internal static ILoggerRepository BaseRepository
        {
            get
            {
                return rep;
            }
        }
        public static void SetConfig(string configFile)
        {
            XmlConfigurator.Configure(rep, new FileInfo(configFile));
        }
        public static void SetConfig(System.IO.FileInfo configFile)
        {
           XmlConfigurator.Configure(rep, configFile);
        }


        /// <summary>
        /// 使用SQLSERVER记录异常日志
        /// </summary>
        /// <Author>Ryanding</Author>
        /// <date>2011-05-01</date>
        static void LoadADONetAppender(string conString)
        {
            LoadFileAppender();

            Hierarchy hier =
              LogManager.GetLoggerRepository(rep.Name) as Hierarchy;

            //if (hier != null)
            //{
            //    AdoNetAppender adoAppender = new AdoNetAppender();
            //    adoAppender.Name = "AdoNetAppender";
            //    adoAppender.CommandType = CommandType.Text;
            //    adoAppender.BufferSize = 1;
            //    adoAppender.ConnectionType = "System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
            //    adoAppender.ConnectionString = conString;
            //    adoAppender.CommandText = @"INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message],[Exception]) VALUES (@log_date, @thread, @log_level, @logger, @message, @exception)";
            //    adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@log_date", DbType = System.Data.DbType.DateTime, Layout = new log4net.Layout.RawTimeStampLayout() });
            //    adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@thread", DbType = System.Data.DbType.String, Size = 255, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%thread")) });
            //    adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@log_level", DbType = System.Data.DbType.String, Size = 50, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%level")) });
            //    adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@logger", DbType = System.Data.DbType.String, Size = 255, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%logger")) });
            //    adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@message", DbType = System.Data.DbType.String, Size = 4000, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message")) });
            //    adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@exception", DbType = System.Data.DbType.String, Size = 4000, Layout = new Layout2RawLayoutAdapter(new ExceptionLayout()) });
            //    adoAppender.ActivateOptions();
            //    BasicConfigurator.Configure(adoAppender);
            //}


        }

        /// <summary>
        /// 使用文本记录异常日志
        /// </summary>
        /// <Author>Ryanding</Author>
        /// <date>2011-05-01</date>
        static void LoadFileAppender()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            string txtLogPath = string.Empty;
            string iisBinPath = AppDomain.CurrentDomain.RelativeSearchPath;

            if (!string.IsNullOrEmpty(iisBinPath))
                txtLogPath = Path.Combine(iisBinPath, "ErrorLog.txt");
            else
                txtLogPath = Path.Combine(currentPath, "ErrorLog.txt");

           Hierarchy hier =
             LogManager.GetLoggerRepository(rep.Name) as Hierarchy;

            FileAppender fileAppender = new FileAppender();
            fileAppender.Name = "LogFileAppender";
            fileAppender.File = txtLogPath;
            fileAppender.AppendToFile = true;

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "记录时间：%date 线程ID:[%thread] 日志级别：%-5level 出错类：%logger property:[%property{NDC}] - 错误描述：%message%newline";
            patternLayout.ActivateOptions();
            fileAppender.Layout = patternLayout;

            //选择UTF8编码，确保中文不乱码。
            fileAppender.Encoding = Encoding.UTF8;

            fileAppender.ActivateOptions();
            //BasicConfigurator.Configure(fileAppender);

        }
    }
}
