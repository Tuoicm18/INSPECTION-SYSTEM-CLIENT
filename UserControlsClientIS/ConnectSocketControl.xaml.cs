using ClientInspectionSystem.LoadData;
using ClientInspectionSystem.SocketClient.Response;
using log4net;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
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

        private IniFile iniFile = new IniFile("Data\\clientIS.ini");
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private int timeOutSocket;

        public ConnectSocketControl() {
            InitializeComponent();

            this.Dispatcher.Invoke(() => {
                //txtIP.Text = ClientExtentions.GetLocalIPAddress();
                txtIP.Text = "127.0.0.1";
                if (txtIP.Text.Equals(string.Empty) || txtPort.Text.Equals(string.Empty)) {
                    if (btnOkConnect != null) {
                        btnOkConnect.IsEnabled = false;
                    }
                }
            });
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
                logger.Error(ex);
                this.Visibility = Visibility.Collapsed;
            }
        }

        private void needForConnectSocket(MainWindow mainWindow) {
            mainWindow.connectionSocket = new SocketClient.Connection(mainWindow.isWSS, txtIP.Text,
                                          txtPort.Text,
                                          mainWindow.delegateAutoGetDoc, mainWindow.delegateAutoBiometric,
                                          mainWindow.delegateCardDetectionEvent, mainWindow.delegateConnectSDK,
                                          mainWindow.delegateNotifyMessage);

            //Find Connect
            mainWindow.connectionSocket.connectSocketServer(mainWindow);

            if (!mainWindow.isConnectDenied) {
                mainWindow.Dispatcher.Invoke(async () => {
                    try {
                        //Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_1k);
                        //Update 2022.02.28 TIME OUT INI FILE
                        timeOutSocket = int.Parse(iniFile.IniReadValue(ClientContants.SECTION_OPTIONS_SOCKET, ClientContants.KEY_OPTIONS_SOCKET_TIME_OUT));

                        await Task.Factory.StartNew(() => {
                            try {
                                PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp deviceDetailsResp = mainWindow.connectionSocket.getDeviceDetails(true, true,
                                                                                                                                  TimeSpan.FromSeconds(timeOutSocket),
                                                                                                                                  timeOutSocket);

                                 mainWindow.Dispatcher.Invoke(() => {
                                    LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(mainWindow.dataGridDetails, deviceDetailsResp.data.deviceSN,
                                                                                        deviceDetailsResp.data.deviceName, deviceDetailsResp.data.lastScanTime,
                                                                                        deviceDetailsResp.data.totalPreceeded.ToString());
                                });
                            }
                            catch (Exception ex) {
                                mainWindow.Dispatcher.Invoke(() => {
                                    LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(mainWindow.dataGridDetails, string.Empty,
                                                                                        string.Empty, string.Empty, string.Empty);
                                });
                                return;
                            }
                        });
                    }
                    catch (Exception eDeviceDetails) {
                        logger.Error(eDeviceDetails);
                        LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(mainWindow.dataGridDetails, string.Empty,
                                                                             string.Empty, string.Empty, string.Empty);
                    }
                });
            }
            else {
                logger.Debug("=> XXXX");
            }
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

        #region HADNLE ENTER KEY
        private void txtIP_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                btnOkConnect_Click(sender, e);
            }
        }

        private void txtPort_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                btnOkConnect_Click(sender, e);
            }
        }
        #endregion
    }
}
