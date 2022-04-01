using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ControlzEx.Theming;
using MahApps.Metro.Controls;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormResultConfigDevice.xaml
    /// </summary>
    public partial class FormResultConfigDevice : MetroWindow {
        public FormResultConfigDevice() {
            InitializeComponent();
            // Set the window theme to Dark Mode
            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        public void setContentStatus(string message) {
            lbStatusResult.Content = message;
        }

        public void setContentDeviceName(string deviceName) {
            lbDeviceNameResult.Content = deviceName;
        }

        public void setContentDeviceSN(string deviceSN) {
            lbDeviceSNResult.Content = deviceSN;
        }
        public void setContentDeviceIP(string deviceIP) {
            lbDeviceIPResult.Content = deviceIP;
        }

        public void contentForError() {
            lbDeviceNameResult.Content = "N/A";
            lbDeviceSNResult.Content = "N/A";
            lbDeviceIPResult.Content = "N/A";
        }
    }
}
