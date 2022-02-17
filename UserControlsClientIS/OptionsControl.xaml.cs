using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientInspectionSystem.UserControlsClientIS {
    /// <summary>
    /// Interaction logic for OptionsControl.xaml
    /// </summary>
    public partial class OptionsControl : UserControl {
        public OptionsControl() {
            InitializeComponent();
        }

        #region HANDLE TEXT BOX SOCKET PORT
        private void txtPortSocket_MouseLeave(object sender, MouseEventArgs e) {
            if (txtPortSocket.Text.Equals("9505") || string.IsNullOrEmpty(txtPortSocket.Text)) {
                txtPortSocket.Text = "9505";
            }
            else {
                txtPortSocket.Text = txtPortSocket.Text;
            }
        }
        //Clear Text
        private void txtPortSocket_MouseEnter(object sender, MouseEventArgs e) {
            if (txtPortSocket.Text.Equals("9505") || string.IsNullOrEmpty(txtPortSocket.Text)) {
                txtPortSocket.Text = "9505";
            }
        }

        private void txtPortSocket_TextInput(object sender, TextCompositionEventArgs e) {
            if (string.IsNullOrEmpty(txtPortSocket.Text)) {
                txtPortSocket.Text = "9505";
            }
            else {
                txtPortSocket.Text = string.Empty;
                txtPortSocket.Text += txtPortSocket.Text;
            }
        }

        private void txtPortSocket_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region HANDLE BUTTON CONNECT
        private void btnConnectSocket_Click(object sender, RoutedEventArgs e) {

        }
        #endregion
    }
}
