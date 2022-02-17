using System;
using System.IO;
using System.Windows;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            // Process unhandled exception do stuff below  
            //Enable Write Log
            Logmanager.Instance.writeLogEnabled = true;
            Exception theException = e.Exception;
            Logmanager.Instance.writeLog("FULL STACK LOGGING\n" + theException.ToString());
            e.Handled = true;
        }
    }
}
