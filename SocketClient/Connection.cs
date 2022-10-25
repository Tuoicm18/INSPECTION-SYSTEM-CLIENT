using ClientInspectionSystem.SocketClient.Response;
using log4net;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using PluginICAOClientSDK;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.ConnectToDevice;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using System;
using System.Threading.Tasks;

namespace ClientInspectionSystem.SocketClient {
    public delegate void DelegateAutoGetDoc(DocumentDetailsResp documentDetailsResp);
    public delegate void DelegateAutoGetBiometric(BiometricAuthResp baseBiometricAuthResp);
    public class Connection {

        #region VARIABLE
        private ISPluginClient wsClient;
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion 

        #region CONSTRUCTOR     
        public Connection(string ip, int port, bool secureConnect) {
            try {
                wsClient = new ISPluginClient(ip, port, secureConnect, new TestListenner());
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }

        public Connection(string ip, int port, bool secureConnect, ISPluginClientDelegate pluginClientDelegate) {
            try {
                wsClient = new ISPluginClient(ip, port, secureConnect, pluginClientDelegate);
                wsClient.setTotalOfTimesReConnect(5);
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion

        #region CONNECT SOCKET & LAYOUT
        public void connectSocketServer(MetroWindow metroWindow) {
            try {
                MainWindow mainWindow = (MainWindow)metroWindow;
                mainWindow.Dispatcher.Invoke(async () => {
                    ProgressDialogController controllerWSClient = await mainWindow.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                                                    InspectionSystemContanst.CONTENT_CONNECTING_MESSAGE_BOX);
                    if (controllerWSClient.IsOpen) {
                        if (mainWindow.isConnectDenied) {
                            controllerWSClient.SetMessage(InspectionSystemContanst.CONTENT_CONNECTED_DENIED_MESSAGE_BOX);
                            await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                            await controllerWSClient.CloseAsync();
                            mainWindow.btnIDocument.IsEnabled = true;
                        }
                        else {
                            if (mainWindow.isConnectSocket) {
                                mainWindow.lbSocketConnectionStatus.Content = "SOCKET CONNECTED";
                                mainWindow.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/success-icon.png",
                                                                                                                    mainWindow.imgSocketConnectionStatus);
                                mainWindow.btnDisconnect.IsEnabled = true;
                                mainWindow.btnConnectToDevice.IsEnabled = true;
                                mainWindow.btnRefresh.IsEnabled = true;
                                mainWindow.btnScanDocument.IsEnabled = true;
                                mainWindow.btnConnect.IsEnabled = false;
                                mainWindow.loadingConnectSocket.Visibility = System.Windows.Visibility.Collapsed;

                                controllerWSClient.SetMessage(InspectionSystemContanst.CONTENT_CONNECTED_MESSAGE_BOX);
                                await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                await controllerWSClient.CloseAsync();
                                mainWindow.btnIDocument.IsEnabled = true;
                            }
                            else {
                                controllerWSClient.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                                await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_3k);
                                await controllerWSClient.CloseAsync();
                                shuttdown(mainWindow);
                            }
                        }
                    }
                });
            }
            catch (Exception eFindConnect) {
                logger.Error(eFindConnect);
            }
        }
        #endregion

        #region CONNECT SOCKET FUNC
        public void connect() {
            wsClient.connect();
        }
        #endregion

        #region DISCONNCET SOCKET FUNC
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
                mainWindow.btnRefresh.IsEnabled = false;
                mainWindow.btnScanDocument.IsEnabled = false;
                mainWindow.btnConnect.IsEnabled = true;
                mainWindow.IsEnabled = true;

                mainWindow.isDisConnectSocket = true;
            });
            wsClient.shutdown();
        }
        #endregion

        #region GET DEVICE DETAILS FUNC
        //Get Device Details
        public PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp getDeviceDetails(bool deviceDetailsEnabled, bool presenceEnabled,
                                                                                             int timeoutInterval) {
            PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp deviceDetailsResp = null;
            try {
                GetDeviceDetails getDeviceDetailsResp = new GetDeviceDetails(deviceDetailsEnabled, presenceEnabled, timeoutInterval, wsClient);
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

        #region GET DOCUMENT DETAILS FUNC
        public DocumentDetailsResp getDocumentDetails(bool mrzEnabled, bool imageEnabled,
                                                      bool dataGroupEnabled, bool optionalDetailsEnabled,
                                                      string canValue, string challenge,
                                                      bool caEnabled, bool taEnabled,
                                                      bool paEnabled, int timeoutInterval) {
            try {
                GetDocumentDetails getDocumentDetails = new GetDocumentDetails(mrzEnabled, imageEnabled,
                                                                               dataGroupEnabled, optionalDetailsEnabled,
                                                                               canValue, challenge,
                                                                               caEnabled, taEnabled,
                                                                               paEnabled, timeoutInterval, 
                                                                               wsClient);
                DocumentDetailsResp documentDetailsResp = getDocumentDetails.getDocumentDetails();
                return documentDetailsResp;
            }
            catch (Exception eDoc) {
                logger.Error(eDoc);
                //return null;
                throw eDoc;
            }
        }
        #endregion

        #region GET RESULT BIOMETRIC AUTH FUNC
        public BiometricAuthResp getResultBiometricAuth(BiometricType biometricType, object challenge,
                                                       ChallengeType challengeType, bool livenessEnabled,
                                                       string cardNo, int timeoutInterval,
                                                       bool biometricEvidence) {
            try {
                GetBiometricAuthentication getBiometricAuthentication = new GetBiometricAuthentication(biometricType, challenge,
                                                                                                       challengeType, livenessEnabled,
                                                                                                       cardNo, timeoutInterval,
                                                                                                       biometricEvidence, wsClient);
                BiometricAuthResp biometricAuthenticationResp = getBiometricAuthentication.getResultBiometricAuth();
                return biometricAuthenticationResp;
            }
            catch (Exception eBiometricAuth) {
                logger.Error(eBiometricAuth);
                throw eBiometricAuth;
            }
        }
        #endregion

        #region GET RESULT CONNECT TO DEVICE FUNC
        public ConnectToDeviceResp getConnectToDevice(bool confirmEnabled, string confirmCode,
                                                      string clientName, ConfigConnect configConnect,
                                                      int timeoutInterval) {
            try {
                GetConnectToDevice getConnectToDevice = new GetConnectToDevice(confirmEnabled, confirmCode,
                                                                               clientName, configConnect,
                                                                               timeoutInterval, wsClient);
                ConnectToDeviceResp baseConnectToDeviceResp = getConnectToDevice.getConnectToDevice();
                return baseConnectToDeviceResp;
            }
            catch (Exception eConnectToDevice) {
                logger.Error(eConnectToDevice);
                throw eConnectToDevice;
            }
        }
        #endregion

        #region REFRESH FUNC
        public PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp refreshReader(bool deviceDetailsEnabled, bool presenceEnabled,
                                                                                          int timeOutInterval) {
            PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp respRefresh = null;
            try {
                Refresh refresh = new Refresh(deviceDetailsEnabled, presenceEnabled, timeOutInterval, wsClient);
                respRefresh = refresh.refreshReader();
                logger.Debug("REFRESH READER " + JsonConvert.SerializeObject(refresh));
                return respRefresh;
            }
            catch (Exception ex) {
                logger.Error(ex);
                throw ex;
            }
        }
        #endregion

        #region SCAN DOCUMENT
        public PluginICAOClientSDK.Response.ScanDocument.ScanDocumentResp scanDocumentResp(ScanType scanType, bool saveEnabled,
                                                                                           int timeoutInterval) {
            PluginICAOClientSDK.Response.ScanDocument.ScanDocumentResp scanDocResp = null;
            try {
                GetScanDocument scanDocument = new GetScanDocument(scanType, saveEnabled, timeoutInterval, wsClient);
                scanDocResp = scanDocument.scanDocumentResp();
                return scanDocResp;
            }
            catch (Exception ex) {
                logger.Error(ex);
                throw ex;
            }
        }
        #endregion

        #region RE-CONNECT SOCKET
        public void reConnect() {
            try {
                wsClient.reConnectSocket();
            }
            catch (Exception ex) {
                logger.Error(ex);
                throw ex;
            }
        }
        #endregion
    }
}
