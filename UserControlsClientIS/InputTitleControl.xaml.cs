using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ClientInspectionSystem.UserControlsClientIS {
    /// <summary>
    /// Interaction logic for InputTitleControl.xaml
    /// </summary>
    public partial class InputTitleControl : UserControl {
        public FormAuthenticationDataNew formAuthDataNew { get; set; }
        public InputTitleControl() {
            InitializeComponent();
        }

        private void btnTitleSubmit_Click(object sender, RoutedEventArgs e) {
            this.Visibility = Visibility.Hidden;
            formAuthDataNew.enableButtonForInitTitleForm();
            formAuthDataNew.Title = getTitleInput();
        }

        public string getTitleInput() {
            return txtTitleForm.Text;
        }
    }
}
