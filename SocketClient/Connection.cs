using ClientInspectionSystem.SocketClient.Response;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using PluginICAOClientSDK;
using PluginICAOClientSDK.Request;
using PluginICAOClientSDK.Response;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInspectionSystem.SocketClient {
    public delegate void DeleagteConnect(bool isConnectWS);
    public delegate void DelegateAutoGetDoc(BaseDocumentDetailsResp  documentDetailsResp);
    public class Connection {
        #region VARIABLE
        //private static readonly string endPointUrlWSS = "wss://192.168.3.170:9505/ISPlugin";
        //private static readonly string endPointUrlWS = "ws://192.168.3.170:9505/ISPlugin";
        //private static readonly string endPointUrl = "wss://192.168.1.8:9505/ISPlugin";
        private readonly int FIND_CONNECT_INTEVAL = 2000;
        private readonly int MAX_PING = 10;
        private DeleagteConnect deleagteConnect;
        private DelegateAutoDocument delegateAutoGetDoc;
        private bool checkConnection;
        public bool CheckConnection {
            get { return checkConnection; }
        }
        private ISPluginClient wsClient;

        public ISPluginClient WSClient {
            get { return wsClient; }
        }
        private System.Timers.Timer timeFindConnectWs;
        #endregion

        #region CONSTRUCTOR
        public Connection(DeleagteConnect dlgConnect) {
            deleagteConnect = dlgConnect;
        }
        public Connection(DeleagteConnect dlgConnect, bool secureConnect,
                          string ip, string port, DelegateAutoDocument dlgAutoGetDoc) {
            deleagteConnect = dlgConnect;
            delegateAutoGetDoc = dlgAutoGetDoc;
            if (secureConnect) {
                //"wss://192.168.3.170:9505/ISPlugin";
                string endPointUrlWSS = "wss://" + ip + ":" + port + InspectionSystemContanst.SUB_URL;
                wsClient = new ISPluginClient(endPointUrlWSS, secureConnect, delegateAutoGetDoc, null);
            }
            else {
                string endPointUrlWS = "ws://" + ip + ":" + port + InspectionSystemContanst.SUB_URL;
                wsClient = new ISPluginClient(endPointUrlWS, secureConnect, delegateAutoGetDoc, null);
            }
        }
        #endregion

        #region FIND CONNECTION AND CONNECT SOCKET
        public void findConnect(MetroWindow metroWindow) {
            try {
                MainWindow mainWindow = (MainWindow)metroWindow;
                mainWindow.Dispatcher.Invoke(async () => {
                    ProgressDialogController controllerWSClient = await mainWindow.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                                                     InspectionSystemContanst.CONTENT_CONNECTING_MESSAGE_BOX);
                    controllerWSClient.SetIndeterminate();

                    timeFindConnectWs = new System.Timers.Timer();

                    bool isConnectWs = false;
                    int countFailConnect = 0;

                    timeFindConnectWs.Interval = FIND_CONNECT_INTEVAL;
                    timeFindConnectWs.Elapsed += (sendFindConnect, eFindConnect) => {
                        isConnectWs = wsClient.checkConnect();
                        checkConnection = wsClient.checkConnect();
                        //Logmanager.Instance.writeLog("AFTER FIND CONNECTION SOCKET [" + isConnectWs + "]");

                        deleagteConnect(isConnectWs);
                        if (isConnectWs) {
                            mainWindow.Dispatcher.Invoke(async () => {
                                mainWindow.btnDisconnect.IsEnabled = true;
                                mainWindow.loadingConnectSocket.Visibility = System.Windows.Visibility.Collapsed;
                                mainWindow.lbSocketConnectionStatus.Content = "SOCKET CONNECTED";
                                mainWindow.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/success-icon.png",
                                                                                                                    mainWindow.imgSocketConnectionStatus);
                                if (controllerWSClient.IsOpen) {
                                    controllerWSClient.SetMessage(InspectionSystemContanst.CONTENT_CONNECTED_MESSAGE_BOX);
                                    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_3k);
                                    await controllerWSClient.CloseAsync();
                                    mainWindow.btnIDocument.IsEnabled = true;
                                }
                            });
                            //Logmanager.Instance.writeLog("STOP TIMER FIND CONNECTION");
                            //timeFindConnectWs.Stop();
                        }
                        else {
                            countFailConnect++;
                            mainWindow.Dispatcher.Invoke(() => {
                                mainWindow.loadingConnectSocket.Visibility = System.Windows.Visibility.Visible;
                                mainWindow.lbSocketConnectionStatus.Content = "CONNECTING...";
                                mainWindow.lbSocketConnectionStatus.FontWeight = System.Windows.FontWeights.Bold;
                                mainWindow.lbSocketConnectionStatus.Foreground = System.Windows.Media.Brushes.White;
                                mainWindow.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/Button-warning-icon.png",
                                                                                                                    mainWindow.imgSocketConnectionStatus);
                                mainWindow.btnConnect.IsEnabled = true;
                            });
                        }
                        if (countFailConnect == MAX_PING) {
                            mainWindow.Dispatcher.Invoke(async () => {
                                await controllerWSClient.CloseAsync();
                                ProgressDialogController controllerWSClientFail = await mainWindow.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                                                                     InspectionSystemContanst.CONTENT_FALIL);
                                await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_4k);
                                await controllerWSClientFail.CloseAsync();
                                mainWindow.IsEnabled = true;
                                mainWindow.loadingConnectSocket.Visibility = System.Windows.Visibility.Collapsed;
                                mainWindow.lbSocketConnectionStatus.Content = "SOCKET CONNECT FAIL";
                            });
                            timeFindConnectWs.Stop();
                            countFailConnect = 0;
                            wsClient.shutDown();
                        }
                    };
                    timeFindConnectWs.Start();
                });
            }
            catch (Exception eFindConnect) {
                Logmanager.Instance.writeLog("FIND CONNECT ERROR " + eFindConnect.ToString());
            }
        }
        #endregion

        #region DISCONNCET SOCKET
        //Shuttdown
        public void shuttdown(MetroWindow metroWindow) {
            checkConnection = wsClient.checkConnect();
            MainWindow mainWindow = (MainWindow)metroWindow;
            mainWindow.Dispatcher.Invoke(() => {
                mainWindow.loadingConnectSocket.Visibility = System.Windows.Visibility.Collapsed;
                mainWindow.lbSocketConnectionStatus.Content = "STATUS SOCKET";
                mainWindow.lbSocketConnectionStatus.FontWeight = System.Windows.FontWeights.Bold;
                mainWindow.lbSocketConnectionStatus.Foreground = System.Windows.Media.Brushes.White;
                mainWindow.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/Button-warning-icon.png",
                                                                                                    mainWindow.imgSocketConnectionStatus);
                mainWindow.btnDisconnect.IsEnabled = false;
                mainWindow.btnConnect.IsEnabled = true;
            });
            timeFindConnectWs.Stop();
            wsClient.shutDown();
        }
        #endregion

        #region GET DEVICE DETAILS
        //Get Device Details
        public PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp getDeviceDetails(bool deviceDetailsEnabled, bool presenceEnabled, TimeSpan timeOutResp) {
            try {
                GetDeviceDetails getDeviceDetailsResp = new GetDeviceDetails(wsClient, deviceDetailsEnabled, presenceEnabled,
                                                                     timeOutResp);
                PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp deviceDetailsResp = getDeviceDetailsResp.getDeviceDetails();
                Logmanager.Instance.writeLog("<DEBUG> GET DEVICE DETAILS " + JsonConvert.SerializeObject(deviceDetailsResp));
                return deviceDetailsResp;
            }
            catch (Exception e) {
                Logmanager.Instance.writeLog("GET DEVICE DETAILS ERROR " + e.ToString());
                //return null;
                throw e;
            }
        }
        #endregion

        #region GET DOCUMENT DETAILS
        public BaseDocumentDetailsResp getDocumentDetails(bool mrzEnabled, bool imageEnabled,
                                                      bool dataGroupEnabled, bool optionalEnabled,
                                                      TimeSpan timeOutResp, ISPluginClient.DocumentDetailsListener documentDetailsListener) {
            try {
                GetDocumentDetails getDocumentDetails = new GetDocumentDetails(wsClient, mrzEnabled, imageEnabled,
                                                                               dataGroupEnabled, optionalEnabled, timeOutResp,
                                                                               documentDetailsListener);
                BaseDocumentDetailsResp documentDetailsResp = getDocumentDetails.getDocumentDetails();
                return documentDetailsResp;
            }
            catch (Exception eDoc) {
                Logmanager.Instance.writeLog("GET DOCUMENT DETAILS ERROR " + eDoc.ToString());
                //return null;
                throw eDoc;
            }
        }
        #endregion

        #region GET RESULT BIOMETRIC AUTH
        public BaseBiometricAuthResp getResultBiometricAuth(string biometricType, AuthorizationData authorizationData, TimeSpan timeOutResp) {
            try {
                GetBiometricAuthentication getBiometricAuthentication = new GetBiometricAuthentication(wsClient, biometricType,
                                                                                                       authorizationData, timeOutResp);
                BaseBiometricAuthResp biometricAuthenticationResp = getBiometricAuthentication.getResultBiometricAuth();
                return biometricAuthenticationResp;
            }
            catch (Exception eBiometricAuth) {
                Logmanager.Instance.writeLog("GET RESULT BIOMETRIC ERROR " + eBiometricAuth.ToString());
                throw eBiometricAuth;
            }
        }
        #endregion
    }
}
