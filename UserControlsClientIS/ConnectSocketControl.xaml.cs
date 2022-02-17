using ClientInspectionSystem.LoadData;
using ClientInspectionSystem.SocketClient.Response;
using PluginICAOClientSDK.Response;
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

namespace ClientInspectionSystem.UserControlsClientIS {
    /// <summary>
    /// Interaction logic for ConnectSocketControl.xaml
    /// </summary>
    public partial class ConnectSocketControl : UserControl {
        public ConnectSocketControl() {
            InitializeComponent();
            if (txtIP.Text.Equals(string.Empty) || txtPort.Text.Equals(string.Empty)) {
                if (btnOkConnect != null) {
                    btnOkConnect.IsEnabled = false;
                }
            }
        }

        #region HANDLE BUTTON CONNECT SOCKET
        private void btnOkConnect_Click(object sender, RoutedEventArgs e) {
            Window parentWindow;
            MainWindow mainWindow = null;
            try {
                parentWindow = Window.GetWindow(this);
                mainWindow = (MainWindow)parentWindow.FindName("mainWindow");
                if (mainWindow.isWSS) {
                    needForConnectSocket(mainWindow);
                }
                else {
                    needForConnectSocket(mainWindow);
                }
                this.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex) {
                Logmanager.Instance.writeLog("ERROR BUTTON CONNECT SOCKET " + ex.ToString());
                this.Visibility = Visibility.Collapsed;
            }
        }

        private void needForConnectSocket(MainWindow mainWindow) {
            mainWindow.connectionSocket = new SocketClient.Connection(mainWindow.deleagteConnect, mainWindow.isWSS, 
                                                                      txtIP.Text, txtPort.Text, mainWindow.delegateAutoGetDoc);
            //Find Connect
            mainWindow.connectionSocket.findConnect(mainWindow);
            mainWindow.Dispatcher.Invoke(async() => {
                try {
                    //Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_1k);
                    await Task.Factory.StartNew(() => {
                        PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp deviceDetailsResp = mainWindow.connectionSocket.getDeviceDetails(true, true,
                                                                                   TimeSpan.FromSeconds(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_15S));
                        mainWindow.Dispatcher.Invoke(() => {
                            LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(mainWindow.dataGridDetails, deviceDetailsResp.data.deviceSN,
                                                                                deviceDetailsResp.data.deviceName, deviceDetailsResp.data.lastScanTime,
                                                                                deviceDetailsResp.data.totalPreceeded.ToString());
                        });
                    });
                }
                catch (Exception eDeviceDetails) {
                    Logmanager.Instance.writeLog("GET DEVICE DETAILS ERROR " + eDeviceDetails.ToString());
                    LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(mainWindow.dataGridDetails, string.Empty,
                                                                        string.Empty, string.Empty, string.Empty);
                }
            });
        }
        #endregion

        #region HANDLE TEXT BOX IP
        private void txtIP_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtIP.Text.Equals(string.Empty)) {
                if (btnOkConnect != null) {
                    btnOkConnect.IsEnabled = false;
                }
            }
            else {
                if (btnOkConnect != null) {
                    btnOkConnect.IsEnabled = true;
                }
            }
        }

        private void txtIP_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            //Regex regex = new Regex("[^0-9]+");
            //e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region HANDLE TEXT BOX PORT
        private void txtPort_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtPort.Text.Equals(string.Empty)) {
                if (btnOkConnect != null) {
                    btnOkConnect.IsEnabled = false;
                }
            }
            else {
                if (btnOkConnect != null) {
                    btnOkConnect.IsEnabled = true;
                }
            }
        }

        private void txtPort_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region HADNLE BUTTON CANCEL CONNECT SOCKET
        //CANCEL CONNECT
        private void btnCancelConnect_Click(object sender, RoutedEventArgs e) {
            this.Visibility = Visibility.Collapsed;
            Window parentWindow = Window.GetWindow(this);
            MainWindow mainWindow = (MainWindow)parentWindow.FindName("mainWindow");
            mainWindow.btnConnect.IsEnabled = true;
        }
        #endregion
    }
}
