using ClientInspectionSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ClientInspectionSystem.UserControlsClientIS {
    /// <summary>
    /// Interaction logic for AddKeyValueControl.xaml
    /// </summary>
    public partial class AddKeyValueControl : UserControl {
        public AddKeyValueControl() {
            InitializeComponent();
            if (txtKey.Text.Equals(string.Empty) || txtValue.Text.Equals(string.Empty)) {
                if (btnAdd != null) {
                    btnAdd.IsEnabled = false;
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            FormAuthenticationDataNew formAuthorizationData = findFormAuthData();
            //List<AuthDataKeyValue> keyValueTest = new List<AuthDataKeyValue>();
            AuthDataKeyValue authDataKeyValue = null;
            if (!txtKey.Text.Equals(string.Empty) && !txtValue.Text.Equals(string.Empty)) {
                authDataKeyValue = new AuthDataKeyValue { KEY = txtKey.Text, VALUE = txtValue.Text };
            }
            if (formAuthorizationData != null) {
                formAuthorizationData.Dispatcher.Invoke(() => {
                    if (null != authDataKeyValue) {
                        //formAuthorizationData.dataGridKeyValues.Items.Refresh();
                        //formAuthorizationData.dataGridKeyValues.Items.Add(authDataKeyValue);
                    }
                });
            }
        }

        private void btnCacelAdd_Click(object sender, RoutedEventArgs e) {
            FormAuthenticationDataNew formAuthorizationData = findFormAuthData();
            if (null != formAuthorizationData) {
                //formAuthorizationData.btnSendBiometricAuth.IsEnabled = true;
            }
            this.Visibility = Visibility.Collapsed;
        }

        private void txtValue_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtValue.Text.Equals(string.Empty)) {
                if (btnAdd != null) {
                    btnAdd.IsEnabled = false;
                }
            } else {
                if (btnAdd != null) {
                    btnAdd.IsEnabled = true;
                }
            }
        }

        private void txtKey_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtKey.Text.Equals(string.Empty)) {
                if (btnAdd != null) {
                    btnAdd.IsEnabled = false;
                }
            } else {
                if (btnAdd != null) {
                    btnAdd.IsEnabled = true;
                }
            }
        }

        private FormAuthenticationDataNew findFormAuthData() {
            FormAuthenticationDataNew formAuthorizationData;
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null) {
                formAuthorizationData = (FormAuthenticationDataNew)parentWindow.FindName("authDataWindow");
                if (formAuthorizationData != null) {
                    return formAuthorizationData;
                }
                else {
                    return null;
                }
            }
            else {
                return null;
            }
        }
    }
}
