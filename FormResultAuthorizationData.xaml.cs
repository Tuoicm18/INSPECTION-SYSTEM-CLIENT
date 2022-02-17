using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientInspectionSystem.Models;
using MahApps.Metro.Controls;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormResultAuthorizationData.xaml
    /// </summary>
    public partial class FormResultAuthorizationData : MetroWindow {
        public FormResultAuthorizationData() {
            InitializeComponent();
            listBoxMultiSelect.IsEnabled = false;
            listBoxSigleSelect.IsEnabled = false;
        }

        private void btnOk_Click(object sender, System.Windows.RoutedEventArgs e) {
            DialogResult = true;
        }

        public void initListBoxMultipleSelected(List<string> multipleSelected) {
            for(int i = 0; i < multipleSelected.Count; i++) {
                Logmanager.Instance.writeLog("MULTI LIST " + multipleSelected[i]);
                //AuthDataListValue.MultiValues.Add(multipleSelected[i]);
            }
            listBoxMultiSelect.ItemsSource = AuthDataListValue.MultiValues;
        }

        public void initListBoxSingleSelected(string singleSelected) {
            Logmanager.Instance.writeLog("SINGLE LIST " + singleSelected);
            AuthDataListValue.SingleValues.Add(singleSelected);
            listBoxSigleSelect.ItemsSource = AuthDataListValue.SingleValues;
        }

        public void setHeaderGroupMultiple(string header) {
            groupBoxMultiSelect.Header = header;
        }

        public void setHeaderGroupSingle(string header) {
            groupBoxSingleSelect.Header = header;
        }
    }
}
