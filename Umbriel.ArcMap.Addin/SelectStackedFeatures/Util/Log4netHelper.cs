

namespace SelectStackedFeatures.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public static class Log4netHelper
    {
        #region Fields

        /// <summary>
        /// log4net log
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        #endregion



        #region Methods


        /// <summary>
        /// Gets the log4 net directory.
        /// </summary>
        /// <returns></returns>
        private static string GetLog4NetDirectory()
        {
            string dirpath = string.Empty;

            if (!System.IO.Directory.Exists(dirpath))
            {
                dirpath = System.IO.Path.GetTempPath();
            }

            return dirpath;
            //#if DEBUG
            //            return System.IO.Path.GetTempPath();    
            //#else

            //#endif  
        }

        /// <summary>
        /// Configures the logging.
        /// </summary>
        private static void ConfigureLogging()
        {
            Trace.WriteLine("Enter ConfigureLogging");

            string logdirectoryPath = GetLog4NetDirectory();

            ConfigureLogging(logdirectoryPath);

        }


        /// <summary>
        /// Configures the logging.
        /// </summary>
        /// <param name="logdirectoryPath">The logdirectory path.</param>
        private static void ConfigureLogging(string logdirectoryPath)
        {
            Trace.WriteLine(string.Format("Enter ConfigureLogging. logdirectoryPath={0}", logdirectoryPath));

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "log4net.config");

            log4net.Config.XmlConfigurator.Configure(new FileInfo(path));

            log.Info("Enter");

            log.Info(SoftwareVersionString());

            log.Info(string.Format("{1}: {0}", ThisAddIn.Version, ThisAddIn.Name));
            log.Info(ThisAddIn.Name);
            log.Info(ThisAddIn.Date);
            log.Info(ThisAddIn.Description);
            log.Info(ThisAddIn.AddInID);

            log.Info(System.Environment.UserDomainName);
            log.Info(System.Environment.UserName);
            log.Info(System.Environment.MachineName);

        }

        /// <summary>
        /// Returns the Softwares the version string.
        /// </summary>
        /// <returns></returns>
        public static string SoftwareVersionString()
        {
            log.Debug("Enter");

            Version v = Assembly.GetExecutingAssembly().GetName().Version;

            return string.Format(@"VDOT Centerline Editor - v.{0}.{1}.{2} (r{3})", v.Major, v.Minor, v.Build, v.Revision);
        }

        #endregion



    }
}
