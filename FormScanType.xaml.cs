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
using log4net;
using MahApps.Metro.Controls;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormScanType.xaml
    /// </summary>
    public partial class FormScanType : MetroWindow {
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool isCancelScanDoc { get; set; } = false;
        public FormScanType() {
            InitializeComponent();
            // Set the window theme to Dark Mode
            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e) {
            try {
                DialogResult = true;
                isCancelScanDoc = false;
                this.Topmost = false;
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            try {
                isCancelScanDoc = true;
                this.Topmost = false;
                this.Close();
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }

        public string getTextCmbScanType() {
            try {
                return cmbScanType.Text;
            }
            catch (Exception e) {
                logger.Error(e);
                return string.Empty;
            }
        }

        public bool getCbSaveEnable() {
            try {
                return (bool)cbSaveEnable.IsChecked;
            }
            catch (Exception ex) {
                logger.Error(ex);
                return false;
            }
        }
    }
}
