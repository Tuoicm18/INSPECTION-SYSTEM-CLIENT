using log4net;
using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        #region VARIABLE
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region OVERRIDE ON STARTUP
        protected override void OnStartup(StartupEventArgs e) {
            log4net.Config.XmlConfigurator.Configure();
            logger.Info("[START LOG " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff tt ") + "]");
            base.OnStartup(e);
        }
        #endregion

        public void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            // Process unhandled exception do stuff below  
            Exception theException = e.Exception;
            logger.Error("[SYSTEM LOG]", theException);
            // Prevent default unhandled exception processing 
            e.Handled = true;
        }
    }
}
