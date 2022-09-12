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
using System.Windows.Shapes;
using ControlzEx.Theming;
using MahApps.Metro.Controls;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormChoiceReadDocument.xaml
    /// </summary>
    public partial class FormChoiceReadDocument : MetroWindow {
        public bool isClose = false;
        public FormChoiceReadDocument() {
            InitializeComponent();
            // Set the window theme to Dark Mode
            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
        }

        public string getCanValue() {
            return txtCanValue.Text;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void cmbChoices_DropDownClosed(object sender, EventArgs e) {
            if(cmbChoices.SelectedIndex == 1) {
                //Show CAN
                lbCanValue.Visibility = Visibility.Visible;
                txtCanValue.Visibility = Visibility.Visible;
                if(null != btnOK) {
                    btnOK.IsEnabled = false;
                }
            } else {
                //Hidden CAN
                lbCanValue.Visibility = Visibility.Collapsed;
                txtCanValue.Visibility = Visibility.Collapsed;
                btnOK.IsEnabled = true;
            }
        }

        private void cmbChoices_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!IsInitialized) return;
            ComboBoxItem item = cmbChoices.SelectedItem as ComboBoxItem;
            if(null != item) {
                if(item.Content.ToString().Equals("MRZ")) {
                    if(null != btnOK) {
                        btnOK.IsEnabled = true;
                    }
                } else {
                    if(null != btnOK) {
                        btnOK.IsEnabled = false;
                    }
                }
            }
        }

        //Is only Number Text box CAN VALUE
        private void txtCanValue_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //txt Can Value Text input
        private void txtCanValue_TextInput(object sender, TextCompositionEventArgs e) {
            if (String.IsNullOrEmpty(txtCanValue.Text)) {
                txtCanValue.Text = "CAN VALUE";
            }
            else {
                txtCanValue.Text = String.Empty;
                txtCanValue.Text += txtCanValue.Text;
            }
        }

        //Pre text Can Value Text Box
        private void txtCanValue_MouseLeave(object sender, MouseEventArgs e) {
            if (txtCanValue.Text.Equals("CAN VALUE") || String.IsNullOrEmpty(txtCanValue.Text)) {
                txtCanValue.Text = "CAN VALUE";
            }
            else {
                txtCanValue.Text = txtCanValue.Text;
            }
        }

        //Clear Text in text box Can Value
        private void txtCanValue_MouseEnter(object sender, MouseEventArgs e) {
            if (txtCanValue.Text.Equals("CAN VALUE") || String.IsNullOrEmpty(txtCanValue.Text)) {
                txtCanValue.Text = String.Empty;
            }
        }

        private void txtCanValue_TextChanged(object sender, TextChangedEventArgs e) {
            //lengthCANValue = e.Changes.ElementAt(0).AddedLength;
            //lengthCANValue = txtCanValue.Text.Length;

            if (!txtCanValue.Equals(string.Empty) || txtCanValue != null) {
                if (txtCanValue.Text.Equals("CAN VALUE")) {
                    if (btnOK != null) {
                        btnOK.IsEnabled = false;
                    }
                }
                else {
                    if (btnOK != null) {
                        btnOK.IsEnabled = true;
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            isClose = true;
            this.Close();
        }

        public bool getValueCheckBoxCA() {
            return (bool)cbCa.IsChecked;
        }

        public bool getValueCheckBoxTA() {
            return (bool)cbTa.IsChecked;
        }

        public bool getValueCheckBoxPA() {
            return (bool)cbPa.IsChecked;
        }

        public bool getValueCheckBoxLiveness() {
            return (bool)cbLiveness.IsChecked;
        }
    }
}
