using ClientInspectionSystem.SocketClient.Response;
using log4net;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using PluginICAOClientSDK;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Request;
using PluginICAOClientSDK.Response;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.ConnectToDevice;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using PluginICAOClientSDK.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInspectionSystem.SocketClient {
    public delegate void DelegateAutoGetDoc(BaseDocumentDetailsResp documentDetailsResp);
    public delegate void DelegateAutoGetBiometric(BaseBiometricAuthResp baseBiometricAuthResp);
    public class Connection {

        #region VARIABLE
        //private static readonly string endPointUrlWSS = "wss://192.168.3.170:9505/ISPlugin";
        //private static readonly string endPointUrlWS = "ws://192.168.3.170:9505/ISPlugin";
        //private static readonly string endPointUrl = "wss://192.168.1.8:9505/ISPlugin";
        //private readonly int FIND_CONNECT_INTEVAL = 1000;
        //private readonly long MAX_PING = long.MaxValue;
        private DelegateConnect deleagteConnect;
        private DelegateAutoDocument delegateAutoGetDoc;
        private DelegateAutoBiometricResult delegateAutoBiometric;
        private DelegateCardDetectionEvent delegateCardDetectionEvent;
        private IniFile iniFile = new IniFile("Data\\clientIS.ini");
        private ISPluginClient wsClient;
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion 

        #region CONSTRUCTOR
        public Connection(bool secureConnect, string ip,
                          string port,
                          DelegateAutoDocument dlgAutoGetDoc, DelegateAutoBiometricResult delegateAutoBiometricResult,
                          DelegateCardDetectionEvent dlgCardEvent, DelegateConnect delegateConnectSDK) {
            this.delegateAutoGetDoc = dlgAutoGetDoc;
            this.delegateAutoBiometric = delegateAutoBiometricResult;
            this.delegateCardDetectionEvent = dlgCardEvent;
            this.deleagteConnect = delegateConnectSDK;

            if (secureConnect) {
                //"wss://192.168.3.170:9505/ISPlugin";
                string endPointUrlWSS = "wss://" + ip + ":" + port + InspectionSystemContanst.SUB_URL;
                wsClient = new ISPluginClient(endPointUrlWSS, secureConnect,
                                              null,
                                              this.delegateAutoGetDoc, this.delegateAutoBiometric,
                                              this.delegateCardDetectionEvent, this.deleagteConnect);
            }
            else {
                string endPointUrlWS = "ws://" + ip + ":" + port + InspectionSystemContanst.SUB_URL;
                wsClient = new ISPluginClient(endPointUrlWS, secureConnect,
                                              null,
                                              this.delegateAutoGetDoc, this.delegateAutoBiometric,
                                              this.delegateCardDetectionEvent, this.deleagteConnect);
            }
        }
        #endregion

        #region FIND CONNECTION AND CONNECT SOCKET
        public void connectSocketServer(MetroWindow metroWindow) {
            try {
                MainWindow mainWindow = (MainWindow)metroWindow;
                mainWindow.Dispatcher.Invoke(async () => {
                    ProgressDialogController controllerWSClient = await mainWindow.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                                                    InspectionSystemContanst.CONTENT_CONNECTING_MESSAGE_BOX);
                    mainWindow.btnDisconnect.IsEnabled = true;
                    mainWindow.btnConnectToDevice.IsEnabled = true;
                    mainWindow.btnConnect.IsEnabled = false;
                    mainWindow.loadingConnectSocket.Visibility = System.Windows.Visibility.Collapsed;
                    mainWindow.lbSocketConnectionStatus.Content = "SOCKET CONNECTED";
                    mainWindow.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/success-icon.png",
                                                                                                        mainWindow.imgSocketConnectionStatus);
                    if (controllerWSClient.IsOpen) {
                        controllerWSClient.SetMessage(InspectionSystemContanst.CONTENT_CONNECTED_MESSAGE_BOX);
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerWSClient.CloseAsync();
                        mainWindow.btnIDocument.IsEnabled = true;
                    }
                });
            }
            catch (Exception eFindConnect) {
                logger.Error(eFindConnect);
            }
        }
        #endregion

        #region CONNECT SOCKET
        public void connect() {
            wsClient.connectSocketServer();
        }
        #endregion

        #region DISCONNCET SOCKET
        //Shuttdown
        public void shuttdown(MetroWindow metroWindow) {
            MainWindow mainWindow = (MainWindow)metroWindow;
            mainWindow.Dispatcher.Invoke(() => {
                mainWindow.loadingConnectSocket.Visibility = System.Windows.Visibility.Collapsed;
                mainWindow.lbSocketConnectionStatus.Content = "STATUS SOCKET";
                mainWindow.lbSocketConnectionStatus.Foreground = System.Windows.Media.Brushes.White;
                mainWindow.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/Button-warning-icon.png",
                                                                                                    mainWindow.imgSocketConnectionStatus);
                mainWindow.btnDisconnect.IsEnabled = false;
                mainWindow.btnConnectToDevice.IsEnabled = false;
                mainWindow.btnConnect.IsEnabled = true;
            });
            wsClient.shutDown();
        }
        #endregion

        #region GET DEVICE DETAILS
        //Get Device Details
        public PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp getDeviceDetails(bool deviceDetailsEnabled, bool presenceEnabled,
                                                                                                 TimeSpan timeOutResp, int timeOutInterVal) {
            PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp deviceDetailsResp = null;
            try {
                GetDeviceDetails getDeviceDetailsResp = new GetDeviceDetails(wsClient, deviceDetailsEnabled, presenceEnabled, timeOutResp, timeOutInterVal);
                deviceDetailsResp = getDeviceDetailsResp.getDeviceDetails();
                logger.Debug("GET DEVICE DETAILS " + JsonConvert.SerializeObject(deviceDetailsResp));
                return deviceDetailsResp;
            }
            catch (Exception e) {
                logger.Error(e);
                //return null;
                throw e;
            }
        }
        #endregion

        #region GET DOCUMENT DETAILS
        public BaseDocumentDetailsResp getDocumentDetails(bool mrzEnabled, bool imageEnabled,
                                                          bool dataGroupEnabled, bool optionalEnabled,
                                                          TimeSpan timeOutResp, ISPluginClient.DocumentDetailsListener documentDetailsListener,
                                                          int timeOutInterVal, string canValue,
                                                          string challenge, bool caEnabled,
                                                          bool taEnabled) {
            try {
                GetDocumentDetails getDocumentDetails = new GetDocumentDetails(wsClient, mrzEnabled, imageEnabled,
                                                                               dataGroupEnabled, optionalEnabled, timeOutResp,
                                                                               documentDetailsListener, timeOutInterVal,
                                                                               canValue, challenge,
                                                                               caEnabled, taEnabled);
                BaseDocumentDetailsResp documentDetailsResp = getDocumentDetails.getDocumentDetails();
                return documentDetailsResp;
            }
            catch (Exception eDoc) {
                logger.Error(eDoc);
                //return null;
                throw eDoc;
            }
        }
        #endregion

        #region GET RESULT BIOMETRIC AUTH
        public BaseBiometricAuthResp getResultBiometricAuth(string biometricType, object challenge,
                                                            TimeSpan timeOutResp, int timeOutInterVal,
                                                            string challengeType, bool livenessEnabled,
                                                            string cardNo) {
            try {
                GetBiometricAuthentication getBiometricAuthentication = new GetBiometricAuthentication(wsClient, biometricType,
                                                                                                       challenge, timeOutResp,
                                                                                                       timeOutInterVal, challengeType,
                                                                                                       livenessEnabled, cardNo);
                BaseBiometricAuthResp biometricAuthenticationResp = getBiometricAuthentication.getResultBiometricAuth();
                return biometricAuthenticationResp;
            }
            catch (Exception eBiometricAuth) {
                logger.Error(eBiometricAuth);
                throw eBiometricAuth;
            }
        }
        #endregion

        #region GET RESULT CONNECT TO DEVICE
        public BaseConnectToDeviceResp getConnectToDevice(bool confirmEnabled, string confirmCode,
                                                          string clientName, PluginICAOClientSDK.Models.ConfigConnect configConnect,
                                                          TimeSpan timeOutResp, int timeOutInterVal) {
            try {
                GetConnectToDevice getConnectToDevice = new GetConnectToDevice(wsClient, confirmEnabled,
                                                                               confirmCode, clientName,
                                                                               configConnect, timeOutInterVal,
                                                                               timeOutResp);
                BaseConnectToDeviceResp baseConnectToDeviceResp = getConnectToDevice.getConnectToDevice();
                return baseConnectToDeviceResp;
            }
            catch (Exception eConnectToDevice) {
                logger.Error(eConnectToDevice);
                throw eConnectToDevice;
            }
        }
        #endregion
    }
}
