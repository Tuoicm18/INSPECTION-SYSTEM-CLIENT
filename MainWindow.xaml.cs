using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows.Navigation;
using System.IO;
using ClientInspectionSystem.Models;
using MahApps.Metro.Controls.Dialogs;
using ClientInspectionSystem;
using System.Threading.Tasks;
using ClientInspectionSystem.SocketClient;
using System.Windows.Controls;
using ClientInspectionSystem.LoadData;
using PluginICAOClientSDK.Response;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClientInspectionSystem.SocketClient.Response;
using PluginICAOClientSDK.Request;
using Newtonsoft.Json;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.ConnectToDevice;
using ClientInspectionSystem.UserControlsClientIS;
using PluginICAOClientSDK.Util;
using PluginICAOClientSDK.Response.CardDetectionEvent;
using System.Timers;
using log4net;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK;

/// <summary>
/// Main Window Class.cs
/// </summary>
/// <author>
/// TuoiCM
/// </author>
/// <date>
/// 21.06.2021
/// </date>
namespace ClientInspectionSystem {
    public partial class MainWindow : MetroWindow {

        #region VARIABLE

        #region DELEGATE
        public ISPluginClientDelegate pluginClientDelegate = new ISPluginClientDelegate();
        #endregion

        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly string endPointUrl = "ws://192.168.3.170:9505/ISPlugin";
        public Connection connectionSocket;
        public bool isWSS;
        public ClientListener clientListener = new ClientListener();
        private BrushConverter brushConverter = new BrushConverter();
        private bool manualReadDoc = false;
        private IniFile iniFile = new IniFile("Data\\clientIS.ini");
        public int timeoutInterval { get; set; }
        private ProgressDialogController dialogControllerAutoRead = null;
        //Update 2022.03.11 For Can Value Request Read Document
        private bool isReadByCanValue = false;
        //Update 2022.05.20 set liveness
        private bool liveness;
        public bool isConnectDenied;
        public bool isConnectSocket { get; set; }
        public bool isDisConnectSocket = false;
        private int countTotalTimesReconnect = 5;


        //Update 2022.07.28
        private Task<ProgressDialogController> dialogControllerReConnect = null;
        private bool isDenied = false;
        #endregion

        #region CONSTRUCTOR
        public MainWindow() {
            InitializeComponent();

            //Version APP
            lbVersion.Content = "V" + ClientExtentions.getCurrentVersion();

            ClientLogger.Instance.writeLogEnabled = true;

            string procName = Process.GetCurrentProcess().ProcessName;
            Process[] pname = Process.GetProcessesByName(procName);
            if (pname.Length > 1) {
                MessageBox.Show("CLIENT PLUGIN IS RUNNING", "WARING", MessageBoxButton.OK);
                System.Windows.Application.Current.Shutdown();
            }

            // Set the window theme to Dark Mode
            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");

            //Find Connection Socket Server
            try {
                //this.timeoutSocket = long.Parse(iniFile.IniReadValue(ClientContants.SECTION_OPTIONS_SOCKET, ClientContants.KEY_OPTIONS_SOCKET_TIME_OUT_RESP));
                //this.timeoutInterval = int.Parse(iniFile.IniReadValue(ClientContants.SECTION_OPTIONS_SOCKET, ClientContants.KEY_OPTIONS_SOCKET_TIME_OUT_INTERVAL));
                this.timeoutInterval = Convert.ToInt32(iniFile.IniReadValue(ClientContants.SECTION_OPTIONS_SOCKET, ClientContants.KEY_OPTIONS_SOCKET_TIME_OUT_INTERVAL));

                //this.timeoutInterval = 60000;

                //Update 2022.07.28
                pluginClientDelegate.dlgConnected = new DelegateDefault<bool>(handleConnect);
                pluginClientDelegate.dlgDisConnected = new DelegateDefault<bool>(handleDisconnected);
                pluginClientDelegate.dlgConnectDenied = new DelegateDefault<bool>(handleConnectDenied);
                pluginClientDelegate.dlgReConnect = new DelegateDefault<bool>(handleReConnect);
                pluginClientDelegate.dlgReceivedDocument = new DelegateDefault<DocumentDetailsResp>(autoGetDocumentDetails);
                pluginClientDelegate.dlgReceivedBiometricResult = new DelegateDefault<BiometricAuthResp>(autoGetBiometricAuth);
                pluginClientDelegate.dlgReceviedCardDetectionEvent = new DelegateDefault<CardDetectionEventResp>(delegateCardDetection);
                pluginClientDelegate.dlgSend = new DelegateDefaultSend<object>(handleDoSend);
                pluginClientDelegate.dlgReceive = new DelegateDefaultReceive<object>(handleReceive);
            }
            catch (Exception eConnection) {
                logger.Error(eConnection);
            }

            btnDisconnect.IsEnabled = false;
            btnConnectToDevice.IsEnabled = false;

            //Test Biometric Auth With cardNo
            btnRFID.IsEnabled = true;
            btnLeftFinger.IsEnabled = false;
            btnRightFinger.IsEnabled = false;
            btnRefresh.IsEnabled = false;
            btnScanDocument.IsEnabled = false;
        }
        #endregion

        #region DISABLE ENTER EVENT BUTTON
        private void btnIDocument_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Space) {
                e.Handled = true;
            }
        }
        #endregion

        #region <FOR TEST> HANDLE DELEGATE SDK TEST OLD

        #region AUTO GET DOCUMENT DETAILS
        public void autoGetDocumentDetails(DocumentDetailsResp documentDetailsResp) {
            //ProgressDialogController controllerReadChip = null;
            if (manualReadDoc == false) {
                this.Dispatcher.Invoke(async () => {
                    try {
                        btnIDocument.IsEnabled = false;
                        clearLayout(true);
                        showMain();
                        optionsControl.Visibility = Visibility.Collapsed;
                        //controllerReadChip = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                        //                                                  InspectionSystemContanst.CONTENT_READING_CHIP_MESSAGE_BOX);
                        //controllerReadChip.SetIndeterminate();

                        await Task.Factory.StartNew(async () => {
                            bool getDocSuccess = autoGetDocumentToLayout(documentDetailsResp);
                            if (getDocSuccess) {
                                //controllerReadChip.CloseAsync();
                                if (null != dialogControllerAutoRead) {
                                    await this.dialogControllerAutoRead.CloseAsync();
                                }
                            }
                            else {
                                //controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                                //Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_3k);
                                //controllerReadChip.CloseAsync();
                            }
                        });
                        btnRFID.IsEnabled = true;
                        btnLeftFinger.IsEnabled = true;
                        btnRightFinger.IsEnabled = true;

                    }
                    catch (Exception eReadChip) {
                        //controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                        //await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_4k);
                        //await controllerReadChip.CloseAsync();
                        clearLayout(false);
                        logger.Error(eReadChip);
                    } finally {
                        PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp deviceDetailsResp = null;
                        await Task.Factory.StartNew(() => {
                            //Update Device Details
                            deviceDetailsResp = connectionSocket.getDeviceDetails(true, true, timeoutInterval);
                            logger.Debug("[UPDATE DEVICE DETAILS 1]");
                        });

                        if (null != deviceDetailsResp) {
                            this.Dispatcher.Invoke(() => {
                                LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(dataGridDetails, deviceDetailsResp.data.deviceSN,
                                                                                    deviceDetailsResp.data.deviceName, deviceDetailsResp.data.lastScanTime,
                                                                                    deviceDetailsResp.data.totalPreceeded.ToString());
                            });
                        }
                        btnIDocument.IsEnabled = true;
                    }
                });
            }
        }

        private bool autoGetDocumentToLayout(DocumentDetailsResp documentDetailsResp) {
            try {
                if (null != documentDetailsResp) {
                    //Logmanager.Instance.writeLog("<DEBUG> AUTO GET DOCUMENT RESPONSE " + JsonConvert.SerializeObject(documentDetailsResp));
                    //2022.05.11 Update get jwt PA from server
                    logger.Debug("DEBUG (AUTO READ) [JWT PA FROM SERVER] " + documentDetailsResp.data.jwt);

                    this.Dispatcher.Invoke(() => {
                        if (!documentDetailsResp.data.mrzString.Equals(string.Empty)) {
                            string mrzSubString = documentDetailsResp.data.mrzString.Substring(0, 30) + "\n" +
                                                  documentDetailsResp.data.mrzString.Substring(30, 30) + "\n" +
                                                  documentDetailsResp.data.mrzString.Substring(60);
                            btnCopyMRZ.Visibility = Visibility.Visible;
                            lbMRZ.Content = mrzSubString;
                        }


                        string imgBse64 = Convert.ToBase64String(documentDetailsResp.data.image);
                        System.Windows.Media.Imaging.BitmapImage imgSource = InspectionSystemPraser.base64ToImage(imgBse64);
                        if (imgSource != null) {
                            imgAvatar.Source = imgSource;
                        }

                        LoadDataForDataGrid.loadDataGridMain(dataGridInputDevice, documentDetailsResp.data.optionalDetails);
                        //Update 2022.02.28 TIME OUT INI FILE
                    });


                    //Background Button
                    if (!documentDetailsResp.data.mrzString.Equals(string.Empty)) { updateBackgroundBtnDG(btnMRZ, 2); }
                    if (documentDetailsResp.data.bacEnabled) { updateBackgroundBtnDG(btnBAC, 2); }
                    if (documentDetailsResp.data.paceEnabled) { updateBackgroundBtnDG(btnSAC, 2); }
                    if (documentDetailsResp.data.activeAuthenticationEnabled) { updateBackgroundBtnDG(btnAA, 2); }
                    if (documentDetailsResp.data.chipAuthenticationEnabled) { updateBackgroundBtnDG(btnCA, 2); }
                    if (documentDetailsResp.data.terminalAuthenticationEnabled) { updateBackgroundBtnDG(btnTA, 2); }
                    if (documentDetailsResp.data.passiveAuthenticationEnabled) { updateBackgroundBtnDG(btnSOD, 2); }
                    if (!documentDetailsResp.data.efCom.Equals(string.Empty)) { updateBackgroundBtnDG(btnEF, 2); }
                    if (!documentDetailsResp.data.efCardAccess.Equals(string.Empty)) { updateBackgroundBtnDG(btnCSC, 2); }

                    logger.Info("AUTO GET DOCUMENT DETAILS SUCCESS");
                    //if (null != this.dialogControllerAutoRead) {
                    //    this.Dispatcher.Invoke(() => {
                    //        Logmanager.Instance.writeLog("<DEBUG> CLOSE DIALOG AUTO READ (AUTO GET DOCUMENT SUCCESS)");
                    //        this.dialogControllerAutoRead.CloseAsync();
                    //        this.dialogControllerAutoRead = null;
                    //    });
                    //}
                    return true;
                }
                else {
                    logger.Debug("AUTO GET DOCUMENT DETAILS FAILURE <DATA IS NULL>");
                    //if (null != this.dialogControllerAutoRead) {
                    //    this.Dispatcher.Invoke(() => {
                    //        Logmanager.Instance.writeLog("<DEBUG> CLOSE DIALOG AUTO READ (AUTO GET DOCUMENT FALIURE)");
                    //        this.dialogControllerAutoRead.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                    //        Task.Delay(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_1S);
                    //        this.dialogControllerAutoRead.CloseAsync();
                    //    });
                    //}
                    return false;
                }
            }
            catch (Exception eAutoDoc) {
                logger.Error("ERROR AUTO GET DOCUMENT " + eAutoDoc);
                //if (null != this.dialogControllerAutoRead) {
                //    this.Dispatcher.Invoke(() => {
                //        Logmanager.Instance.writeLog("<DEBUG> CLOSE DIALOG AUTO READ EXCEPTION AUTO GET DOCUMENT " + eAutoDoc.ToString());
                //        this.dialogControllerAutoRead.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                //        Task.Delay(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_1S);
                //        this.dialogControllerAutoRead.CloseAsync();
                //    });
                //}
                throw eAutoDoc;
            }
        }
        #endregion

        #region AUTO GET RESULT BIOMETRIC AUTH
        public void autoGetBiometricAuth(BiometricAuthResp baseBiometricAuthResp) {
            try {
                if (null != baseBiometricAuthResp) {
                    logger.Debug("<AUTO> BIOMETRIC AUTH RESP " + JsonConvert.SerializeObject(baseBiometricAuthResp, Formatting.Indented));
                    if (baseBiometricAuthResp.data.biometricType.Equals(BiometricType.faceID)) {
                        this.Dispatcher.Invoke(() => {
                            FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                            formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FACE);
                            formBiometricAuth.renderResultBiometricAuht(baseBiometricAuthResp);
                            formBiometricAuth.Topmost = true;
                            if (formBiometricAuth.ShowDialog() == true) { }
                        });
                    }
                    else {
                        this.Dispatcher.Invoke(() => {
                            FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                            formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                            formBiometricAuth.renderResultBiometricAuht(baseBiometricAuthResp);
                            formBiometricAuth.Topmost = true;
                            if (formBiometricAuth.ShowDialog() == true) { }
                        });
                    }
                }
            }
            catch (Exception eAutoBiometric) {
                logger.Error(eAutoBiometric);
            }
        }
        #endregion

        #region AUTO READ NOTIFY MESSAGE
        public void autoReadNotify(string json) {
            try {
                if (!string.IsNullOrEmpty(json)) {
                    BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(json);
                    if (null != baseResponse) {
                        isConnectDenied = false;
                        if (baseResponse.errorCode == PluginICAOClientSDK.Utils.ERR_FOR_DENIED_CONNECTION) {
                            isConnectDenied = true;
                        }
                    }
                }
            }
            catch (Exception e) {
                logger.Error(e);
            }
        }
        #endregion

        #region UPDATE 2022.05.10 TEST CARD DETECTION EVENT
        private void delegateCardDetection(CardDetectionEventResp baseCardDetectionEventResp) {
            try {
                if (null != baseCardDetectionEventResp) {
                    if (baseCardDetectionEventResp.errorCode == 0) {
                        this.Dispatcher.Invoke(async () => {
                            if (baseCardDetectionEventResp.data.cardDetected) {
                                //await this.ShowMessageAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_CARD_DETECTION_EVENT);
                                //Update 2022.02.28
                                manualReadDoc = true;

                                ProgressDialogController controllerReadChip = null;
                                //Show Form Choices Read Document
                                FormChoiceReadDocument formChoiceReadDocument = new FormChoiceReadDocument();
                                string txtCanValue = string.Empty;
                                //Update 2022.05.20 TA, CA ENABLED
                                bool caEnableEd = false;
                                bool taEnabled = false;
                                if (formChoiceReadDocument.ShowDialog() == true) {
                                    if (!formChoiceReadDocument.txtCanValue.Text.Equals("CAN VALUE") && !formChoiceReadDocument.txtCanValue.Text.Equals(string.Empty)) {
                                        isReadByCanValue = true;
                                        txtCanValue = formChoiceReadDocument.getCanValue();
                                    }
                                    caEnableEd = formChoiceReadDocument.getValueCheckBoxCA();
                                    taEnabled = formChoiceReadDocument.getValueCheckBoxTA();
                                    this.liveness = formChoiceReadDocument.getValueCheckBoxLiveness();
                                }
                                try {
                                    if (formChoiceReadDocument.isClose) {
                                        return;
                                    }
                                    btnIDocument.IsEnabled = false;
                                    clearLayout(true);
                                    showMain();
                                    optionsControl.Visibility = Visibility.Collapsed;
                                    controllerReadChip = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                                      InspectionSystemContanst.CONTENT_READING_CHIP_MESSAGE_BOX);
                                    controllerReadChip.SetIndeterminate();
                                    //Call socket server
                                    //bool mrzEnabled = true;
                                    //bool imageEnabled = true;
                                    //bool dataGroupEnabled = false;
                                    //bool optionalDetailsEnabled = true;

                                    bool mrzEnabled = configConnectToDeviceControl.getMRZEnabled();
                                    bool imageEnabled = configConnectToDeviceControl.getImgaeEnabled();
                                    bool dataGroupEnabled = configConnectToDeviceControl.getDataGroupEnabled();
                                    bool optionalDetailsEnabled = configConnectToDeviceControl.getOptionalDetailsEnabled();

                                    //await Task.Delay(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_10S);
                                    await Task.Factory.StartNew(() => {
                                        try {
                                            bool getDocSuccess = getDocumentDetailsToLayout(mrzEnabled, imageEnabled,
                                                                                            dataGroupEnabled, optionalDetailsEnabled,
                                                                                            txtCanValue, "C# CLIENT",
                                                                                            caEnableEd, taEnabled,
                                                                                            timeoutInterval);
                                            if (getDocSuccess) {
                                                controllerReadChip.CloseAsync();
                                            }
                                            else {
                                                controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                                                Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_5k);
                                                controllerReadChip.CloseAsync();
                                            }
                                        }
                                        catch (Exception exx) {
                                            //controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                                            //Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_5k);
                                            //controllerReadChip.CloseAsync();
                                            throw exx;
                                        }
                                    });
                                    btnRFID.IsEnabled = true;
                                    btnLeftFinger.IsEnabled = true;
                                    btnRightFinger.IsEnabled = true;
                                }
                                catch (Exception eReadChip) {
                                    //Check if auto reviced data.
                                    //if (null == lbMRZ.Content) {
                                    //    controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                                    //    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_5k);
                                    //    await controllerReadChip.CloseAsync();
                                    //    clearLayout(false);
                                    //    Logmanager.Instance.writeLog("BUTTON READ CHIP EXCEPTION " + eReadChip.ToString());
                                    //}
                                    //else {
                                    //    Logmanager.Instance.writeLog("BUTTON READ CHIP EXCEPTION " + eReadChip.ToString());
                                    //}

                                    if (eReadChip is ISPluginException) {
                                        ISPluginException pluginException = (ISPluginException)eReadChip;
                                        controllerReadChip.SetMessage(pluginException.errMsg.ToUpper());
                                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                        await controllerReadChip.CloseAsync();
                                    }
                                    else {
                                        controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                        await controllerReadChip.CloseAsync();
                                    }

                                    clearLayout(false);
                                    logger.Error(eReadChip);
                                } finally {
                                    //Update 2022.02.28
                                    manualReadDoc = false;
                                    btnIDocument.IsEnabled = true;

                                    PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp deviceDetailsResp = null;
                                    await Task.Factory.StartNew(() => {
                                        //Update Device Details
                                        deviceDetailsResp = connectionSocket.getDeviceDetails(true, true, timeoutInterval);
                                        logger.Debug("[UPDATE DEVICE DETAILS 2]");
                                    });

                                    if (null != deviceDetailsResp) {
                                        this.Dispatcher.Invoke(() => {
                                            LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(dataGridDetails, deviceDetailsResp.data.deviceSN,
                                                                                                deviceDetailsResp.data.deviceName, deviceDetailsResp.data.lastScanTime,
                                                                                                deviceDetailsResp.data.totalPreceeded.ToString());
                                        });
                                    }
                                }
                            }
                            else {
                                //await this.ShowMessageAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_CARD_NOT_DETECTED_EVENT);
                                clearLayout(false);
                            }
                        });
                    }
                }
            }
            catch (Exception e) {
                throw e;
            }
        }
        #endregion

        #region AUTO CONNECT
        private void handleDlgConnect(bool isConnect) {
            try {
                bool isConnected = isConnect;
                isConnectSocket = isConnected;
                //logger.Debug("IS CONNECT => " + isConnected);
                if (!isConnect) {
                    if (!isConnectDenied && !isDisConnectSocket) {
                        this.Dispatcher.Invoke(() => {
                            this.loadingConnectSocket.Visibility = Visibility.Visible;
                            this.lbSocketConnectionStatus.Content = "RE-CONNECT...";
                            this.lbSocketConnectionStatus.Foreground = Brushes.White;
                            this.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/Button-warning-icon.png",
                                                                                                               this.imgSocketConnectionStatus);
                            this.IsEnabled = false;
                        });

                        //timerDead.Reset();
                        //timerConnect.Interval = 2000;
                        //timerConnect.Elapsed += Timer_Elapsed;
                        //timerConnect.Start();

                        //if(countReConnect == 5) {
                        //    StopTimer(); 
                        //}

                        //connectionSocket.reConnect(INTERVAL_RECONNECT_SOCKET, 5);
                        this.countTotalTimesReconnect--;
                        logger.Warn("RE-CONNECT TIEMS => " + countTotalTimesReconnect);
                        if (this.countTotalTimesReconnect == 0) {
                            connectionSocket.shuttdown(this);
                        }
                    }
                    else {
                        this.Dispatcher.Invoke(() => {
                            this.lbSocketConnectionStatus.Content = "CONNECTION DENIED";
                            this.btnDisconnect.IsEnabled = false;
                            this.btnConnectToDevice.IsEnabled = false;
                            this.btnConnect.IsEnabled = true;
                            this.loadingConnectSocket.Visibility = Visibility.Collapsed;
                            this.lbSocketConnectionStatus.Foreground = Brushes.White;
                            this.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/Button-warning-icon.png",
                                                                                                                this.imgSocketConnectionStatus);
                        });
                        connectionSocket.shuttdown(this);
                    }
                }
                else {
                    this.Dispatcher.Invoke(() => {
                        this.btnDisconnect.IsEnabled = true;
                        this.btnConnectToDevice.IsEnabled = true;
                        this.btnConnect.IsEnabled = false;
                        this.loadingConnectSocket.Visibility = System.Windows.Visibility.Collapsed;
                        this.lbSocketConnectionStatus.Content = "SOCKET CONNECTED";
                        this.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/success-icon.png",
                                                                                                            this.imgSocketConnectionStatus);
                        this.IsEnabled = true;

                        //countReConnect = 0;
                        //timerDead.Set();
                        this.countTotalTimesReconnect = 5;
                    });
                }
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            //lock (locker) {
            //    if (timerDead.WaitOne(0)) return;
            //    // etc...
            //    countReConnect++;
            //    connectionSocket.connect();
            //    logger.Warn(countReConnect);
            //}
        }

        private void StopTimer() {
            //lock (locker) {
            //    timerDead.Set();
            //    timerConnect.Stop();
            //    countReConnect = 0;
            //    logger.Debug("STOP TIMER");
            //    connectionSocket.shuttdown(this);
            //}
        }

        #endregion

        #endregion

        #region HANDLE DELEGATE SDK NEW

        #region CONNECT
        private void handleConnect(bool isConnected) {
            try {
                logger.Debug("[CONNECTED SOCKET] " + this.isConnectDenied);
                if (isConnected) {
                    this.Dispatcher.Invoke(async () => {
                        try {
                            //ProgressDialogController controllerConnectOK = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, "CONNECT SUCCESSFULLY");
                            //await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                            //await controllerConnectOK.CloseAsync();

                            this.loadingConnectSocket.Visibility = Visibility.Collapsed;
                            this.lbSocketConnectionStatus.Content = "SOCKET CONNECTED";
                            this.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/success-icon.png",
                                                                                                                this.imgSocketConnectionStatus);

                            btnDisconnect.IsEnabled = true;
                            btnConnectToDevice.IsEnabled = true;
                            btnRefresh.IsEnabled = true;
                            btnScanDocument.IsEnabled = true;
                            btnConnect.IsEnabled = false;
                            btnIDocument.IsEnabled = true;

                            this.IsEnabled = true;

                            this.isConnectDenied = false;
                        }
                        catch (Exception ex) {
                            logger.Error(ex);
                        }
                    });
                }
                else {
                    logger.Warn("[CONNECTED SOCKET => FALSE] " + this.isConnectDenied);
                    if (this.isConnectDenied) {
                        mainWindow.Dispatcher.Invoke(() => {
                            this.lbSocketConnectionStatus.Content = "CONNECTION DENIED";
                            this.btnDisconnect.IsEnabled = false;
                            this.btnConnectToDevice.IsEnabled = false;
                            this.btnScanDocument.IsEnabled = false;
                            this.btnRefresh.IsEnabled = false;
                            this.btnConnect.IsEnabled = true;
                            this.loadingConnectSocket.Visibility = Visibility.Collapsed;
                            this.lbSocketConnectionStatus.Foreground = Brushes.White;
                            this.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/Button-warning-icon.png",
                                                                                                                this.imgSocketConnectionStatus);
                        });
                    }
                }
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion

        #region DISCONNECTED
        public void handleDisconnected(bool isDisconnect) {
            try {
                if (isDisconnect) {
                    logger.Debug("[DISCONNECTED SOCKET]");

                    mainWindow.Dispatcher.Invoke(async () => {
                        if (!this.isDisConnectSocket) {
                            ProgressDialogController controllerConnectFail = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, "CONNECTION FAIL");
                            await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                            await controllerConnectFail.CloseAsync();

                            this.loadingConnectSocket.Visibility = Visibility.Collapsed;
                            this.lbSocketConnectionStatus.Content = "STATUS SOCKET";
                            mainWindow.lbSocketConnectionStatus.Foreground = Brushes.White;
                            mainWindow.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/Button-warning-icon.png",
                                                                                                                mainWindow.imgSocketConnectionStatus);
                            this.btnDisconnect.IsEnabled = false;
                            this.btnConnectToDevice.IsEnabled = false;
                            this.btnRefresh.IsEnabled = false;
                            this.btnScanDocument.IsEnabled = false;
                            this.btnConnect.IsEnabled = true;
                            this.IsEnabled = true;

                            this.isDisConnectSocket = true;
                        }
                    });
                }
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion

        #region CONNECTION DENIED
        private void handleConnectDenied(bool isDenied) {
            try {
                this.isConnectDenied = isDenied;
                if (isDenied) {
                    logger.Warn("[CONNECTION SOCKET DENIED]");

                    this.Dispatcher.Invoke(async () => {
                        try {
                            ProgressDialogController controllerDenied = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, "CONNECTION DENIED");
                            await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                            await controllerDenied.CloseAsync();

                            this.lbSocketConnectionStatus.Content = "CONNECTION DENIED";
                            this.btnDisconnect.IsEnabled = false;
                            this.btnConnectToDevice.IsEnabled = false;
                            this.btnScanDocument.IsEnabled = false;
                            this.btnRefresh.IsEnabled = false;
                            this.btnConnect.IsEnabled = true;
                            this.loadingConnectSocket.Visibility = Visibility.Collapsed;
                            this.lbSocketConnectionStatus.Foreground = Brushes.White;
                            this.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/Button-warning-icon.png",
                                                                                                                this.imgSocketConnectionStatus);
                        }
                        catch (Exception ex) {
                            logger.Error(ex);
                        }
                    });
                }
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion

        #region RE-CONNECT
        private void handleReConnect(bool isReConncet) {
            try {
                if (isReConncet) {
                    logger.Info("[RE-CONNECT SOCKET...]");
                    if (!this.isDisConnectSocket) {
                        this.Dispatcher.Invoke(async () => {
                            //ProgressDialogController controllerReConnect = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, "RE-CONNECT...");
                            //await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                            //await controllerReConnect.CloseAsync();

                            this.loadingConnectSocket.Visibility = Visibility.Visible;
                            this.lbSocketConnectionStatus.Content = "RE-CONNECT...";
                            this.lbSocketConnectionStatus.Foreground = Brushes.White;
                            this.imgSocketConnectionStatus.Source = InspectionSystemPraser.setImageSource("/Resource/Button-warning-icon.png",
                                                                                                               this.imgSocketConnectionStatus);

                            this.IsEnabled = false;
                        });
                    }
                }
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion

        #region DO SEND
        public void handleDoSend(object cmd, object id, object data) {
            try {
                logger.Debug("DO SEND " + cmd as string);
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion

        #region RECEIVE
        public void handleReceive(object cmd, object id, object error, object data) {
            try {
                logger.Debug("ON RECEIVE " + cmd as string + "/" + error as string);
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion

        #endregion

        #region HYPER LINK QUEQUE TEXT
        //Hyperlink
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            // for .NET Core you need to add UseShellExecute = true
            // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        #endregion

        #region DATA GRID DEVICE DETAILS HANDLE
        private void dataGridInputDevice_Loaded(object sender, RoutedEventArgs e) {
            LoadDataForDataGrid.loadDataGridMain(dataGridInputDevice, null);
        }

        private void dataGridInputDevice_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            dataGridInputDevice.Items.Refresh();
        }
        #endregion

        #region BUTTON_CLICK COPY DESCRIPTION DATA GRID MAIN
        //Btn Coppy Data Grid MAIN
        private void btnCoppy_Click(object sender, RoutedEventArgs e) {
            DataInputFromDevice obj = ((FrameworkElement)sender).DataContext as DataInputFromDevice;
            System.Windows.Clipboard.SetText(obj.DESCRIPTION);
            dataGridInputDevice.Items.Refresh();
        }
        #endregion

        #region BUTTON_CLICK COPY MRZ STRING 
        //Copy MRZ String
        private void btnCopyMRZ_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(lbMRZ.Content.ToString().Replace(Environment.NewLine, "").TrimStart().TrimEnd());
        }
        #endregion

        #region BUTTON_CLICK READ DOCUMENT
        private void btnIDocument_Click(object sender, RoutedEventArgs e) {
            //Update 2022.02.28
            manualReadDoc = true;

            ProgressDialogController controllerReadChip = null;
            this.Dispatcher.Invoke(async () => {
                //Show Form Choices Read Document
                FormChoiceReadDocument formChoiceReadDocument = new FormChoiceReadDocument();
                string txtCanValue = string.Empty;
                //Update 2022.05.20 TA, CA ENABLED
                bool caEnableEd = false;
                bool taEnabled = false;
                if (formChoiceReadDocument.ShowDialog() == true) {
                    if (!formChoiceReadDocument.txtCanValue.Text.Equals("CAN VALUE") && !formChoiceReadDocument.txtCanValue.Text.Equals(string.Empty)) {
                        isReadByCanValue = true;
                        txtCanValue = formChoiceReadDocument.getCanValue();
                    }
                    caEnableEd = formChoiceReadDocument.getValueCheckBoxCA();
                    taEnabled = formChoiceReadDocument.getValueCheckBoxTA();
                    this.liveness = formChoiceReadDocument.getValueCheckBoxLiveness();
                }
                try {
                    if (formChoiceReadDocument.isClose) {
                        return;
                    }
                    btnIDocument.IsEnabled = false;
                    clearLayout(true);
                    showMain();
                    optionsControl.Visibility = Visibility.Collapsed;
                    controllerReadChip = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                      InspectionSystemContanst.CONTENT_READING_CHIP_MESSAGE_BOX);
                    controllerReadChip.SetIndeterminate();
                    //Call socket server
                    //bool mrzEnabled = true;
                    //bool imageEnabled = true;
                    //bool dataGroupEnabled = false;
                    //bool optionalDetailsEnabled = true;

                    bool mrzEnabled = configConnectToDeviceControl.getMRZEnabled();
                    bool imageEnabled = configConnectToDeviceControl.getImgaeEnabled();
                    bool dataGroupEnabled = configConnectToDeviceControl.getDataGroupEnabled();
                    bool optionalDetailsEnabled = configConnectToDeviceControl.getOptionalDetailsEnabled();

                    //await Task.Delay(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_10S);
                    await Task.Factory.StartNew(() => {
                        try {
                            bool getDocSuccess = getDocumentDetailsToLayout(mrzEnabled, imageEnabled,
                                                                            dataGroupEnabled, optionalDetailsEnabled,
                                                                            txtCanValue, "C# CLIENT",
                                                                            caEnableEd, taEnabled,
                                                                            this.timeoutInterval);
                            if (getDocSuccess) {
                                controllerReadChip.CloseAsync();
                            }
                            else {
                                controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                                Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_5k);
                                controllerReadChip.CloseAsync();
                            }
                        }
                        catch (Exception exx) {
                            //controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                            //Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_5k);
                            //controllerReadChip.CloseAsync();
                            throw exx;
                        }
                    });
                    btnRFID.IsEnabled = true;
                    btnLeftFinger.IsEnabled = true;
                    btnRightFinger.IsEnabled = true;
                }
                catch (Exception eReadChip) {
                    //Check if auto reviced data.
                    //if (null == lbMRZ.Content) {
                    //    controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                    //    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_5k);
                    //    await controllerReadChip.CloseAsync();
                    //    clearLayout(false);
                    //    Logmanager.Instance.writeLog("BUTTON READ CHIP EXCEPTION " + eReadChip.ToString());
                    //}
                    //else {
                    //    Logmanager.Instance.writeLog("BUTTON READ CHIP EXCEPTION " + eReadChip.ToString());
                    //}

                    if (eReadChip is ISPluginException) {
                        ISPluginException pluginException = (ISPluginException)eReadChip;
                        controllerReadChip.SetMessage(pluginException.errMsg.ToUpper());
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerReadChip.CloseAsync();
                    }
                    else {
                        controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerReadChip.CloseAsync();
                    }

                    clearLayout(false);
                    logger.Error(eReadChip);
                } finally {
                    //Update 2022.02.28
                    manualReadDoc = false;
                    btnIDocument.IsEnabled = true;

                    PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp deviceDetailsResp = null;
                    await Task.Factory.StartNew(() => {
                        //Update Device Details
                        deviceDetailsResp = connectionSocket.getDeviceDetails(true, true, timeoutInterval);
                        logger.Debug("[UPDATE DEVICE DETAILS 3]");
                    });

                    if (null != deviceDetailsResp) {
                        this.Dispatcher.Invoke(() => {
                            LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(dataGridDetails, deviceDetailsResp.data.deviceSN,
                                                                                deviceDetailsResp.data.deviceName, deviceDetailsResp.data.lastScanTime,
                                                                                deviceDetailsResp.data.totalPreceeded.ToString());
                        });
                    }
                }
            });
        }

        private bool getDocumentDetailsToLayout(bool mrzEnabled, bool imageEnabled,
                                                bool dataGroupEnabled, bool optionalDetailsEnabled,
                                                string canValue, string challenge,
                                                bool caEnabled, bool taEnabled,
                                                int timeoutInterval) {
            try {

                DocumentDetailsResp documentDetailsResp = connectionSocket.getDocumentDetails(mrzEnabled, imageEnabled,
                                                                                              dataGroupEnabled, optionalDetailsEnabled,
                                                                                              canValue, challenge,
                                                                                              caEnabled, taEnabled,
                                                                                              timeoutInterval);
                logger.Info(JsonConvert.SerializeObject(documentDetailsResp));
                if (null != documentDetailsResp) {
                    //2022.05.11 Update get jwt PA from server
                    logger.Debug("(MANUAL READ) [JWT PA FROM SERVER] " + documentDetailsResp.data.jwt);
                    this.Dispatcher.Invoke(() => {
                        if (mrzEnabled) {
                            if (!documentDetailsResp.data.mrzString.Equals(string.Empty)) {
                                string mrzSubString = documentDetailsResp.data.mrzString.Substring(0, 30) + "\n" +
                                                      documentDetailsResp.data.mrzString.Substring(30, 30) + "\n" +
                                                      documentDetailsResp.data.mrzString.Substring(60);
                                btnCopyMRZ.Visibility = Visibility.Visible;
                                lbMRZ.Content = mrzSubString;
                            }
                        }
                        if (imageEnabled) {
                            string imgBse64 = Convert.ToBase64String(documentDetailsResp.data.image);
                            System.Windows.Media.Imaging.BitmapImage imgSource = InspectionSystemPraser.base64ToImage(imgBse64);
                            if (imgSource != null) {
                                imgAvatar.Source = imgSource;
                            }
                        }
                        if (optionalDetailsEnabled) {
                            LoadDataForDataGrid.loadDataGridMain(dataGridInputDevice, documentDetailsResp.data.optionalDetails);
                        }

                        if (configConnectToDeviceControl.getMRZEnabled()) {
                            if (!documentDetailsResp.data.mrzString.Equals(string.Empty) && !isReadByCanValue) { updateBackgroundBtnDG(btnMRZ, 2); }
                        }
                    });

                    if (documentDetailsResp.data.bacEnabled) { updateBackgroundBtnDG(btnBAC, 2); }
                    if (documentDetailsResp.data.paceEnabled) { updateBackgroundBtnDG(btnSAC, 2); }
                    if (documentDetailsResp.data.activeAuthenticationEnabled) { updateBackgroundBtnDG(btnAA, 2); }
                    if (documentDetailsResp.data.chipAuthenticationEnabled) { updateBackgroundBtnDG(btnCA, 2); }
                    if (documentDetailsResp.data.terminalAuthenticationEnabled) { updateBackgroundBtnDG(btnTA, 2); }
                    if (documentDetailsResp.data.passiveAuthenticationEnabled) { updateBackgroundBtnDG(btnSOD, 2); }
                    if (!documentDetailsResp.data.efCom.Equals(string.Empty)) { updateBackgroundBtnDG(btnEF, 2); }
                    if (!documentDetailsResp.data.efCardAccess.Equals(string.Empty)) { updateBackgroundBtnDG(btnCSC, 2); }

                    logger.Info("MANUAL GET DOCUMENT DETAILS SUCCESS");
                    return true;
                }
                else {
                    logger.Info("MANUAL GET DOCUMENT DETAILS FAILURE <DATA IS NULL>");
                    return false;
                }
            }
            catch (Exception e) {
                logger.Error(e);
                throw e;
            } finally {
                isReadByCanValue = false;
            }
        }
        #endregion

        #region CHECK CONNECTION FOR TEST
        private void checkConnect(bool isConnect) {
            //Connect to socket server
            ProgressDialogController controllerWSClient = null;
            _ = this.Dispatcher.Invoke(async () => {
                try {
                    controllerWSClient = await this.ShowProgressAsync("CLIENT INSPECTION SYSTEM", "WAITING CONNECT SOCKET");
                    controllerWSClient.SetIndeterminate();
                    if (isConnect) {
                        controllerWSClient.SetMessage("CONNECT SOCET SERVER SUCCESSFULLY");
                        await Task.Delay(4000);
                        await controllerWSClient.CloseAsync();
                    }
                }
                catch (Exception eMain) {
                    controllerWSClient.SetMessage("CONNECT SOCET SERVER ERROR");
                    await Task.Delay(4000);
                    await controllerWSClient.CloseAsync();
                    logger.Error(eMain);
                }
            });
        }
        #endregion

        #region BUTTON OPTONS HANDLE
        private void btnOption_Click(object sender, RoutedEventArgs e) {
            hideMain();
            optionsControl.Visibility = Visibility.Visible;
        }
        #endregion

        #region HIDE MAIN WINDOW
        private void hideMain() {
            gridAvatar.Visibility = Visibility.Hidden;
            gridOptionalDetails.Visibility = Visibility.Hidden;
            btnCopyMRZ.Visibility = Visibility.Hidden;
            imgAvatar.Visibility = Visibility.Hidden;
            lbMRZ.Visibility = Visibility.Hidden;
        }
        #endregion

        #region SHOW MAIN WINDOW
        private void showMain() {
            optionsControl.Visibility = Visibility.Hidden;

            gridAvatar.Visibility = Visibility.Visible;
            gridOptionalDetails.Visibility = Visibility.Visible;
            btnCopyMRZ.Visibility = Visibility.Visible;
            imgAvatar.Visibility = Visibility.Visible;
            lbMRZ.Visibility = Visibility.Visible;
        }
        #endregion

        #region HANDLE BUTTON CONNECT WEBSOCKET SERVER
        private void btnConnect_Click(object sender, RoutedEventArgs e) {
            isDisConnectSocket = false;
            var button = sender as Button;
            ContextMenu contextMenu = button.ContextMenu;
            contextMenu.PlacementTarget = button;
            contextMenu.IsOpen = true;
            e.Handled = true;
        }

        //Connect Normal Socket
        private void MenuItemWS_Click(object sender, RoutedEventArgs e) {
            //connectionSocket.findConnect(this, false);
            try {
                this.Dispatcher.Invoke(() => {
                    btnConnect.IsEnabled = false;
                    isWSS = false;
                    conncetSocketControl.lbTitleConnectSocket.Content = "NORMAL SOCKET CONNECTION";
                    conncetSocketControl.Visibility = Visibility.Visible;
                });
            }
            catch (Exception eWS) {
                logger.Error(eWS);
            }
        }
        //Conncet Secure Connect
        private void MenuItemWSS_Click(object sender, RoutedEventArgs e) {
            try {
                //connectionSocket.findConnect(this, true);
                this.Dispatcher.Invoke(() => {
                    btnConnect.IsEnabled = false;
                    isWSS = true;
                    conncetSocketControl.lbTitleConnectSocket.Content = "SECURE SOCKET CONNECTION";
                    conncetSocketControl.Visibility = Visibility.Visible;
                });
            }
            catch (Exception eWSS) {
                logger.Error(eWSS);
            }
        }
        #endregion

        #region HANDLE BUTTON DISCONNECT 
        private void btnDisconnect_Click(object sender, RoutedEventArgs e) {
            connectionSocket.shuttdown(this);
            clearLayout(false);
            disabledAllButton();
        }
        #endregion

        #region CLEAR LAYOUT
        public void clearLayout(bool isUpdate) {
            imgAvatar.Source = new BitmapImage(new Uri("/Resource/15_RFID.jpg", UriKind.Relative));
            lbMRZ.Content = string.Empty;
            btnCopyMRZ.Visibility = Visibility.Collapsed;
            if (btnLeftFinger.IsEnabled) {
                btnLeftFinger.IsEnabled = false;
            }
            if (btnRightFinger.IsEnabled) {
                btnRightFinger.IsEnabled = false;
            }
            if (btnRFID.IsEnabled) {
                btnRFID.IsEnabled = false;
            }

            if (null != dataGridInputDevice.ItemsSource) {
                LoadDataForDataGrid.loadDataGridMain(dataGridInputDevice, null);
            }

            if (isUpdate) {
                //After Update
                LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(dataGridDetails, "...",
                                                                    "...", "...", "...");
            }
            else {
                if (null != dataGridDetails.ItemsSource) {
                    LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(dataGridDetails, string.Empty,
                                                                        string.Empty, string.Empty, string.Empty);
                }
            }

            btnPassAll.Background = new SolidColorBrush(Colors.White);
            btnMRZ.Background = new SolidColorBrush(Colors.White);
            btnSAC.Background = new SolidColorBrush(Colors.White);
            btnBAC.Background = new SolidColorBrush(Colors.White);
            btnAA.Background = new SolidColorBrush(Colors.White);
            btnCA.Background = new SolidColorBrush(Colors.White);
            btnTA.Background = new SolidColorBrush(Colors.White);
            btnEF.Background = new SolidColorBrush(Colors.White);
            btnSOD.Background = new SolidColorBrush(Colors.White);
            btnCSC.Background = new SolidColorBrush(Colors.White);
            btnSF.Background = new SolidColorBrush(Colors.White);
            btnFA.Background = new SolidColorBrush(Colors.White);
        }
        #endregion

        #region ENABLED ALL BUTTON
        public void enabledAllButton() {
            this.Dispatcher.Invoke(() => {
                btnIDocument.IsEnabled = true;
                btnRFID.IsEnabled = true;
                btnLeftFinger.IsEnabled = true;
                btnRightFinger.IsEnabled = true;
            });
        }
        #endregion

        #region DISABLED ALL BUTTON
        public void disabledAllButton() {
            btnIDocument.IsEnabled = false;
            btnRFID.IsEnabled = false;
            btnLeftFinger.IsEnabled = false;
            btnRightFinger.IsEnabled = false;
        }
        #endregion

        #region UPDATE BACKGROUND BUTTON 
        public void updateBackgroundBtnDG(System.Windows.Controls.Button btnDG, int res) {
            BrushConverter bc = new BrushConverter();
            try {
                switch (res) {
                    case -1:
                        this.Dispatcher.Invoke(() => {
                            btnDG.Background = new SolidColorBrush(Colors.Red);
                        });
                        break;
                    case 0:
                        this.Dispatcher.Invoke(() => {
                            btnDG.Background = null;
                        });
                        break;
                    case 1:
                        this.Dispatcher.Invoke(() => {
                            btnDG.Background = new SolidColorBrush(Colors.White);
                        });
                        break;
                    case 2:
                        this.Dispatcher.Invoke(() => {
                            btnDG.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0767b3");
                            //btnDG.Background = new SolidColorBrush(Colors.Green);
                        });
                        break;
                    case 10:
                        this.Dispatcher.Invoke(() => {
                            btnDG.Background = new SolidColorBrush(Colors.Brown);
                        });
                        break;
                }
            }
            catch (Exception e) {
                logger.Error(e);
                return;
            }
        }
        #endregion

        #region BUTTON_CLICK FACE AUTH
        private void btnRFID_Click(object sender, RoutedEventArgs e) {
            ProgressDialogController controllerFaceAuth = null;
            this.Dispatcher.Invoke(async () => {
                try {
                    btnRFID.IsEnabled = false;
                    showMain();
                    FormAuthenticationDataNew formAuthorizationData = new FormAuthenticationDataNew();
                    FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                    if (formAuthorizationData.ShowDialog() == true) {
                        //Form Watting
                        //controllerFaceAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_WATTING_BIOMETRIC_RESULT_MESSAGE_BOX);
                        //controllerFaceAuth.SetIndeterminate();
                        BiometricAuthResp resultFaceAuth = null;
                        await Task.Factory.StartNew(() => {
                            try {
                                resultFaceAuth = resultBiometricAuth(formAuthorizationData, BiometricType.faceID, string.Empty); // 2022.05.12 Update challenge 079094012066
                            }
                            catch (Exception ex) {
                                ProgressDialogController controllerFaceAuthErr = null;
                                if (ex is ISPluginException) {
                                    this.Dispatcher.InvokeAsync(async () => {
                                        ISPluginException pluginException = (ISPluginException)ex;
                                        controllerFaceAuthErr = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg.ToUpper());
                                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                        await controllerFaceAuth.CloseAsync();
                                    });
                                }
                                else {
                                    this.Dispatcher.InvokeAsync(async () => {
                                        controllerFaceAuthErr = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                        await controllerFaceAuth.CloseAsync();
                                    });
                                }
                            }
                        });

                        if (null != resultFaceAuth) {
                            if (resultFaceAuth.errorCode == ClientContants.SOCKET_RESP_CODE_BIO_AUTH_DENIED) { // Cancel Auth
                                                                                                               //controllerFaceAuth.CloseAsync();
                                this.Dispatcher.Invoke(() => {
                                    formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FACE);
                                    formBiometricAuth.Topmost = true;
                                    formBiometricAuth.hideLabelForDeniedAuth();
                                    formBiometricAuth.setContentLabelResponseCode(resultFaceAuth.errorCode.ToString());
                                    formBiometricAuth.setContentLabelResponseMsg(resultFaceAuth.errorMessage.ToString());
                                    if (formBiometricAuth.ShowDialog() == true) { }
                                });
                            }
                            else if (resultFaceAuth.errorCode == 0) {
                                if (resultFaceAuth.data.result) {
                                    //controllerFaceAuth.CloseAsync();
                                    this.Dispatcher.Invoke(() => {
                                        //Init Form Result Biometric Auth
                                        initFormResultBiometricAuth(resultFaceAuth, PluginICAOClientSDK.Utils.ToDescription(BiometricType.faceID));
                                        logger.Debug("GET RESPONSE BIOMETRIC AUTH FACE" + JsonConvert.SerializeObject(resultFaceAuth, Formatting.Indented));

                                        updateBackgroundBtnDG(btnFA, 2);
                                        //Button Pass All
                                        SolidColorBrush btnPassAllBackground = btnPassAll.Background as SolidColorBrush;
                                        if (null != btnPassAllBackground) {
                                            Color colorPassAll = btnPassAllBackground.Color;
                                            if (Colors.Red.Equals(colorPassAll)) {
                                                btnPassAll.Background = (Brush)brushConverter.ConvertFrom(InspectionSystemContanst.SET_BACKGROUND_BTN_PASSALL);
                                            }
                                            else {
                                                btnPassAll.Background = (Brush)brushConverter.ConvertFrom(InspectionSystemContanst.SET_BACKGROUND_BTN_PASSALL);
                                            }
                                        }
                                    });
                                }
                                else {
                                    //controllerFaceAuth.CloseAsync();
                                    this.Dispatcher.Invoke(() => {
                                        //Init Form Result Biometric Auth
                                        initFormResultBiometricAuth(resultFaceAuth, PluginICAOClientSDK.Utils.ToDescription(BiometricType.faceID));
                                        logger.Debug("GET RESPONSE BIOMETRIC AUTH FACE" + JsonConvert.SerializeObject(resultFaceAuth, Formatting.Indented));

                                        updateBackgroundBtnDG(btnFA, -1);
                                        //Button Pass All
                                        SolidColorBrush btnPassAllBackground = btnPassAll.Background as SolidColorBrush;
                                        if (null != btnPassAllBackground) {
                                            Color colorPassAll = btnPassAllBackground.Color;
                                            if (Colors.White.Equals(colorPassAll) || Colors.Red.Equals(colorPassAll)) {
                                                btnPassAll.Background = new SolidColorBrush(Colors.Red);
                                            }
                                            else {
                                                btnPassAll.Background = (Brush)brushConverter.ConvertFrom(InspectionSystemContanst.SET_BACKGROUND_BTN_PASSALL);
                                            }
                                        }
                                    });
                                }
                            }
                            else {
                                await this.Dispatcher.InvokeAsync(async () => {
                                    controllerFaceAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, resultFaceAuth.errorMessage.ToUpper());
                                    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                    await controllerFaceAuth.CloseAsync();
                                    logger.Warn("RESPONSE FACE AUTH\n" + JsonConvert.SerializeObject(resultFaceAuth));
                                });
                            }
                        }
                        else {
                            logger.Warn("RESPONSE NULL");
                        }
                        btnRFID.IsEnabled = true;
                    }
                }
                catch (Exception ex) {
                    logger.Error(ex);
                    if (ex is ISPluginException) {
                        ISPluginException pluginException = (ISPluginException)ex;
                        controllerFaceAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg.ToUpper());
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerFaceAuth.CloseAsync();
                    }
                    else {
                        controllerFaceAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerFaceAuth.CloseAsync();
                    }
                    btnRFID.IsEnabled = true;
                }
            });
        }

        private BiometricAuthResp resultBiometricAuth(FormAuthenticationDataNew formAuthorizationData, BiometricType biometricType, string cardNo) {
            BiometricAuthResp resultBiometricResp = null;
            try {
                BiometricAuthResp resultBiometric = null;

                ChallengeBiometricAuth challenge = new ChallengeBiometricAuth();
                string challengeString = string.Empty;

                challenge.challengeValue = string.Empty;
                TransactionDataBiometricAuth transactionDataInput = null;
                //bool livenessEnabled = optionsControl.toggleSwitchLivenessTest.IsOn;

                this.Dispatcher.Invoke(() => {
                    if (formAuthorizationData.CheckImportString) {
                        challengeString = formAuthorizationData.getImportJson();
                        resultBiometric = connectionSocket.getResultBiometricAuth(biometricType, challengeString,
                                                                                  ChallengeType.TYPE_STRING, this.liveness,
                                                                                  cardNo, this.timeoutInterval);
                    }
                    else {
                        if (formAuthorizationData.CheckImportJson) {
                            string jsonImport = formAuthorizationData.getImportJson();
                            transactionDataInput = ISExtentions.deserializeJsonAuthorizationData(jsonImport);
                            challenge.transactionData = transactionDataInput;
                        }
                        else {
                            transactionDataInput = new TransactionDataBiometricAuth();
                            transactionDataInput.transactionTitle = formAuthorizationData.Title;
                            transactionDataInput.authContentList = formAuthorizationData.getDataContentList();
                            transactionDataInput.multipleSelectList = formAuthorizationData.getDataMultipleChoices();
                            transactionDataInput.singleSelectList = formAuthorizationData.getDataSingleChoices();
                            transactionDataInput.nameValuePairList = formAuthorizationData.getDataNVP();
                            challenge.transactionData = transactionDataInput;
                        }
                    }
                });

                resultBiometric = connectionSocket.getResultBiometricAuth(biometricType, challenge,
                                                                          ChallengeType.TYPE_OBJECT, this.liveness,
                                                                          cardNo, this.timeoutInterval);

                if (null != resultBiometric) {
                    //resultAuthFace = resultBiometric.result;
                    resultBiometricResp = resultBiometric;
                }

                return resultBiometricResp;
            }
            catch (Exception ex) {
                logger.Error(ex);
                //resultAuthFace = false;
                throw ex;
            }
        }
        #endregion

        #region BUTTON_CLICK LEFT FINGER
        private void btnLeftFinger_Click(object sender, RoutedEventArgs e) {
            ProgressDialogController controllerLeftFingerAuth = null;
            this.Dispatcher.Invoke(async () => {
                try {
                    btnLeftFinger.IsEnabled = false;
                    showMain();
                    FormAuthenticationDataNew formAuthorizationData = new FormAuthenticationDataNew();
                    FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                    if (formAuthorizationData.ShowDialog() == true) {
                        //Form Watting
                        //controllerLeftFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_WATTING_BIOMETRIC_RESULT_MESSAGE_BOX);
                        //controllerLeftFingerAuth.SetIndeterminate();
                        BiometricAuthResp resultLeftFingerAuth = null;

                        await Task.Factory.StartNew(async () => {
                            try {
                                resultLeftFingerAuth = resultBiometricAuth(formAuthorizationData, BiometricType.fingerLeftIndex, string.Empty); // 2022.05.12 Update challenge 079094012066
                            }
                            catch (Exception ex) {
                                logger.Warn("CHECK " + ex);
                                ProgressDialogController controllerLeftErr = null;
                                if (ex is ISPluginException) {
                                    await this.Dispatcher.InvokeAsync(async () => {
                                        ISPluginException pluginException = (ISPluginException)ex;
                                        controllerLeftErr = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg.ToUpper());
                                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                        await controllerLeftErr.CloseAsync();
                                    });
                                }
                                else {
                                    await this.Dispatcher.InvokeAsync(async () => {
                                        controllerLeftErr = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                        await controllerLeftErr.CloseAsync();
                                    });
                                }
                                throw ex;
                            }
                        });

                        if (null != resultLeftFingerAuth) {
                            if (resultLeftFingerAuth.errorCode == ClientContants.SOCKET_RESP_CODE_BIO_AUTH_DENIED) { // Cancel Auth
                                                                                                                     //controllerLeftFingerAuth.CloseAsync();
                                await this.Dispatcher.InvokeAsync(() => {
                                    formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                    formBiometricAuth.Topmost = true;
                                    formBiometricAuth.hideLabelForDeniedAuth();
                                    formBiometricAuth.setContentLabelResponseCode(resultLeftFingerAuth.errorCode.ToString());
                                    formBiometricAuth.setContentLabelResponseMsg(resultLeftFingerAuth.errorMessage.ToString());
                                    if (formBiometricAuth.ShowDialog() == true) { }
                                });
                            }
                            else if (resultLeftFingerAuth.errorCode == 0) {
                                if (resultLeftFingerAuth.data.result) {
                                    //controllerLeftFingerAuth.CloseAsync();
                                    await this.Dispatcher.InvokeAsync(() => {
                                        //Init Form Result Biometric Auth
                                        initFormResultBiometricAuth(resultLeftFingerAuth, PluginICAOClientSDK.Utils.ToDescription(BiometricType.fingerLeftIndex));
                                        logger.Debug("GET RESPONSE BIOMETRIC AUTH LEFT FINGER\n" + JsonConvert.SerializeObject(resultLeftFingerAuth, Formatting.Indented));

                                        updateBackgroundBtnDG(btnSF, 2);
                                        //Button Pass All
                                        SolidColorBrush btnPassAllBackground = btnPassAll.Background as SolidColorBrush;
                                        if (null != btnPassAllBackground) {
                                            Color colorPassAll = btnPassAllBackground.Color;
                                            if (Colors.Red.Equals(colorPassAll)) {
                                                btnPassAll.Background = (Brush)brushConverter.ConvertFrom(InspectionSystemContanst.SET_BACKGROUND_BTN_PASSALL);
                                            }
                                            else {
                                                btnPassAll.Background = (Brush)brushConverter.ConvertFrom(InspectionSystemContanst.SET_BACKGROUND_BTN_PASSALL);
                                            }
                                        }
                                    });
                                }
                                else {
                                    //controllerLeftFingerAuth.CloseAsync();
                                    await this.Dispatcher.InvokeAsync(() => {
                                        //Init Form Result Biometric Auth
                                        initFormResultBiometricAuth(resultLeftFingerAuth, PluginICAOClientSDK.Utils.ToDescription(BiometricType.fingerLeftIndex));
                                        logger.Debug("GET RESPONSE BIOMETRIC AUTH LEFT FINGER" + JsonConvert.SerializeObject(resultLeftFingerAuth, Formatting.Indented));

                                        updateBackgroundBtnDG(btnSF, -1);
                                        //Button Pass All
                                        SolidColorBrush btnPassAllBackground = btnPassAll.Background as SolidColorBrush;
                                        if (null != btnPassAllBackground) {
                                            Color colorPassAll = btnPassAllBackground.Color;
                                            if (Colors.White.Equals(colorPassAll) || Colors.Red.Equals(colorPassAll)) {
                                                btnPassAll.Background = new SolidColorBrush(Colors.Red);
                                            }
                                            else {
                                                btnPassAll.Background = (Brush)brushConverter.ConvertFrom(InspectionSystemContanst.SET_BACKGROUND_BTN_PASSALL);
                                            }
                                        }
                                    });
                                }
                            }
                            else {
                                await this.Dispatcher.InvokeAsync(async () => {
                                    controllerLeftFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, resultLeftFingerAuth.errorMessage.ToUpper());
                                    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                    await controllerLeftFingerAuth.CloseAsync();
                                    logger.Warn("RESPONSE LEFT FINGER\n" + JsonConvert.SerializeObject(resultLeftFingerAuth));
                                });
                            }
                        }
                        else {
                            logger.Warn("RESPONSE NULL LEFT");
                        }
                        btnLeftFinger.IsEnabled = true;
                    }
                }
                catch (Exception eLeft) {
                    logger.Error(eLeft);
                    if (eLeft is ISPluginException) {
                        ISPluginException pluginException = (ISPluginException)eLeft;
                        controllerLeftFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg.ToUpper());
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerLeftFingerAuth.CloseAsync();
                    }
                    else {
                        controllerLeftFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerLeftFingerAuth.CloseAsync();
                    }
                }
            });
        }
        #endregion

        #region BUTTON_CLICK RIGHT FINGER
        private void btnRightFinger_Click(object sender, RoutedEventArgs e) {
            ProgressDialogController controllerRightFingerAuth = null;
            this.Dispatcher.Invoke(async () => {
                try {
                    btnRightFinger.IsEnabled = false;
                    showMain();
                    FormAuthenticationDataNew formAuthorizationData = new FormAuthenticationDataNew();
                    FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                    if (formAuthorizationData.ShowDialog() == true) {
                        //Show Form Watting
                        //controllerRightFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,InspectionSystemContanst.CONTENT_WATTING_BIOMETRIC_RESULT_MESSAGE_BOX);
                        //controllerRightFingerAuth.SetIndeterminate();
                        BiometricAuthResp resultFingerRightAuth = null;
                        await Task.Factory.StartNew(() => {
                            try {
                                resultFingerRightAuth = resultBiometricAuth(formAuthorizationData, BiometricType.fingerRightIndex, string.Empty); // 2022.05.12 Update challenge 079094012066
                            }
                            catch (Exception ex) {
                                ProgressDialogController controllerRightFingerAuthErr = null;
                                if (ex is ISPluginException) {
                                    this.Dispatcher.InvokeAsync(async () => {
                                        ISPluginException pluginException = (ISPluginException)ex;
                                        controllerRightFingerAuthErr = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg.ToUpper());
                                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                        await controllerRightFingerAuth.CloseAsync();
                                    });
                                }
                                else {
                                    this.Dispatcher.InvokeAsync(async () => {
                                        controllerRightFingerAuthErr = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                        await controllerRightFingerAuth.CloseAsync();
                                    });
                                }
                            }
                        });

                        if (null != resultFingerRightAuth) {

                            if (resultFingerRightAuth.errorCode == ClientContants.SOCKET_RESP_CODE_BIO_AUTH_DENIED) { // Cancel Auth
                                                                                                                      //controllerRightFingerAuth.CloseAsync();
                                this.Dispatcher.Invoke(() => {
                                    formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                    formBiometricAuth.Topmost = true;
                                    formBiometricAuth.hideLabelForDeniedAuth();
                                    formBiometricAuth.setContentLabelResponseCode(resultFingerRightAuth.errorCode.ToString());
                                    formBiometricAuth.setContentLabelResponseMsg(resultFingerRightAuth.errorMessage.ToString());
                                    if (formBiometricAuth.ShowDialog() == true) { }
                                });
                            }
                            else if (resultFingerRightAuth.errorCode == 0) {
                                if (resultFingerRightAuth.data.result) {
                                    //controllerRightFingerAuth.CloseAsync();
                                    this.Dispatcher.Invoke(() => {
                                        //Init Form Result Biometric Auth
                                        initFormResultBiometricAuth(resultFingerRightAuth, PluginICAOClientSDK.Utils.ToDescription(BiometricType.fingerRightIndex));
                                        logger.Debug("GET RESPONSE BIOMETRIC AUTH RIGHT FINGER " +
                                                                     JsonConvert.SerializeObject(resultFingerRightAuth, Formatting.Indented));

                                        updateBackgroundBtnDG(btnSF, 2);
                                        //Button Pass All
                                        SolidColorBrush btnPassAllBackground = btnPassAll.Background as SolidColorBrush;
                                        if (null != btnPassAllBackground) {
                                            Color colorPassAll = btnPassAllBackground.Color;
                                            if (Colors.Red.Equals(colorPassAll)) {
                                                btnPassAll.Background = (Brush)brushConverter.ConvertFrom(InspectionSystemContanst.SET_BACKGROUND_BTN_PASSALL);
                                            }
                                            else {
                                                btnPassAll.Background = (Brush)brushConverter.ConvertFrom(InspectionSystemContanst.SET_BACKGROUND_BTN_PASSALL);
                                            }
                                        }
                                    });
                                }
                                else {
                                    //controllerRightFingerAuth.CloseAsync();
                                    this.Dispatcher.Invoke(() => {
                                        //Init Form Result Biometric Auth
                                        initFormResultBiometricAuth(resultFingerRightAuth, PluginICAOClientSDK.Utils.ToDescription(BiometricType.fingerRightIndex));
                                        logger.Debug("GET RESPONSE BIOMETRIC AUTH RIGHT FINGER\n" + JsonConvert.SerializeObject(resultFingerRightAuth, Formatting.Indented));

                                        updateBackgroundBtnDG(btnSF, 1);
                                        //Button Pass All
                                        SolidColorBrush btnPassAllBackground = btnPassAll.Background as SolidColorBrush;
                                        if (null != btnPassAllBackground) {
                                            Color colorPassAll = btnPassAllBackground.Color;
                                            if (Colors.White.Equals(colorPassAll) || Colors.Red.Equals(colorPassAll)) {
                                                btnPassAll.Background = new SolidColorBrush(Colors.Red);
                                            }
                                            else {
                                                btnPassAll.Background = (Brush)brushConverter.ConvertFrom(InspectionSystemContanst.SET_BACKGROUND_BTN_PASSALL);
                                            }
                                        }
                                    });
                                }
                            }
                            else {
                                await this.Dispatcher.InvokeAsync(async () => {
                                    controllerRightFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, resultFingerRightAuth.errorMessage.ToUpper());
                                    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                    await controllerRightFingerAuth.CloseAsync();
                                    logger.Warn("RESPONSE RIGHT FINGER\n" + JsonConvert.SerializeObject(resultFingerRightAuth));
                                });
                            }
                        }
                        else {
                            logger.Warn("RESPONSE RIGHT FINGER NULL");
                        }

                        btnRightFinger.IsEnabled = true;
                    }
                }
                catch (Exception eLeft) {
                    logger.Error(eLeft);
                    if (eLeft is ISPluginException) {
                        ISPluginException pluginException = (ISPluginException)eLeft;
                        controllerRightFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg.ToUpper());
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerRightFingerAuth.CloseAsync();
                    }
                    else {
                        controllerRightFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerRightFingerAuth.CloseAsync();
                    }
                }
            });
        }
        #endregion

        #region INIT FORM RESULT BIOMETRIC AUTH
        public void initFormResultBiometricAuth(BiometricAuthResp baseBiometricAuthResp, string biometricType) {
            logger.Debug("INI FORM RESULT BIOMETRIC AUTH...");
            if (null != baseBiometricAuthResp) {
                if (null != baseBiometricAuthResp.data.challenge) {
                    ChallengeBiometricAuth challengeBiometricAuth = baseBiometricAuthResp.data.challenge;
                    if (null != challengeBiometricAuth.transactionData) {
                        TransactionDataBiometricAuth transactionDataResp = challengeBiometricAuth.transactionData;
                        FormResultAuthorizationData formResultAuthorization = new FormResultAuthorizationData();
                        formResultAuthorization.Topmost = true;
                        if (checkNullReslutTransactionData(transactionDataResp) == false) {
                            //Render Title Form Result Biometric Auth
                            formResultAuthorization.renderTitleForm(transactionDataResp.transactionTitle);

                            Dictionary<int, AuthorizationElement> dicRenderResult = new Dictionary<int, AuthorizationElement>();
                            //Content List
                            if (null != transactionDataResp.authContentList) {
                                foreach (var v in transactionDataResp.authContentList) {
                                    v.type = AuthElementType.Content;
                                    dicRenderResult.Add(v.ordinary, v);
                                }
                            }
                            //Multiple
                            if (null != transactionDataResp.multipleSelectList) {
                                foreach (var v in transactionDataResp.multipleSelectList) {
                                    v.type = AuthElementType.Multiple;
                                    dicRenderResult.Add(v.ordinary, v);
                                }
                            }
                            //Single
                            if (null != transactionDataResp.singleSelectList) {
                                foreach (var v in transactionDataResp.singleSelectList) {
                                    v.type = AuthElementType.Single;
                                    dicRenderResult.Add(v.ordinary, v);
                                }
                            }
                            //NVP
                            if (null != transactionDataResp.nameValuePairList) {
                                foreach (var v in transactionDataResp.nameValuePairList) {
                                    v.type = AuthElementType.NVP;
                                    dicRenderResult.Add(v.ordinary, v);
                                }
                            }

                            int maxLoop = 1000;
                            int count = 0;
                            for (int i = 0; i < maxLoop; i++) {
                                if (dicRenderResult.ContainsKey(i)) {
                                    AuthorizationElement element = dicRenderResult[i];
                                    if (null == element) {
                                        continue;
                                    }
                                    //Render Layout
                                    switch (element.type) {
                                        case AuthElementType.Content:
                                            formResultAuthorization.renderToLayoutResultContentList(element);
                                            break;
                                        case AuthElementType.Multiple:
                                            formResultAuthorization.renderToLayoutReslutMultiple(element);
                                            break;
                                        case AuthElementType.Single:
                                            formResultAuthorization.renderToLayoutResultSingle(element);
                                            break;
                                        case AuthElementType.NVP:
                                            formResultAuthorization.renderToLayoutNVP(element);
                                            break;
                                    }
                                    if (++count >= dicRenderResult.Count) {
                                        break;
                                    }
                                }
                            }
                            //Render Result Biometric Auth
                            formResultAuthorization.renderResultBiometricAuht(baseBiometricAuthResp);
                            if (formResultAuthorization.ShowDialog() == true) { }
                        }
                    }
                }
                else {
                    if (baseBiometricAuthResp.errorCode == ClientContants.SOCKET_RESP_CODE_BIO_SUCCESS) {
                        if (BiometricType.fingerLeftIndex.Equals(biometricType)) {
                            try {
                                //controllerLeftFingerAuth.CloseAsync();
                                FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                                this.Dispatcher.Invoke(() => {
                                    formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                    formBiometricAuth.renderResultBiometricAuht(baseBiometricAuthResp);
                                    formBiometricAuth.Topmost = true;
                                    if (formBiometricAuth.ShowDialog() == true) { }
                                });
                            }
                            catch (Exception eLeft) {
                                logger.Error(eLeft);
                            }
                        }
                        else if (BiometricType.fingerRightIndex.Equals(biometricType)) {
                            //controllerLeftFingerAuth.CloseAsync();
                            FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                            this.Dispatcher.Invoke(() => {
                                formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                formBiometricAuth.renderResultBiometricAuht(baseBiometricAuthResp);
                                formBiometricAuth.Topmost = true;
                                if (formBiometricAuth.ShowDialog() == true) { }
                            });
                        }
                        else {
                            //controllerLeftFingerAuth.CloseAsync();
                            FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                            this.Dispatcher.Invoke(() => {
                                formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FACE);
                                formBiometricAuth.renderResultBiometricAuht(baseBiometricAuthResp);
                                formBiometricAuth.Topmost = true;
                                if (formBiometricAuth.ShowDialog() == true) { }
                            });
                        }
                    }
                }
            }
        }

        //Check Null Transaction Data
        private bool checkNullReslutTransactionData(TransactionDataBiometricAuth transaction) {
            List<AuthorizationElement> elementContent = transaction.authContentList;
            List<AuthorizationElement> elementMultiple = transaction.multipleSelectList;
            List<AuthorizationElement> elementSingle = transaction.singleSelectList;
            List<AuthorizationElement> elementNVP = transaction.nameValuePairList;
            return elementContent.Count == 0 && elementMultiple.Count == 0
                && elementSingle.Count == 0 && elementNVP.Count == 0;

        }
        #endregion

        #region CLOSE MAIN WINDOW
        private void mainWindow_Closed(object sender, EventArgs e) {
            string procName = Process.GetCurrentProcess().ProcessName;
            Process[] pname = Process.GetProcessesByName(procName);
            foreach (var p in pname) {
                p.Kill();
            }
        }
        #endregion

        #region FOR TEST
        //private void btnOption_Click(object sender, RoutedEventArgs e) {
        //    //this.Visibility = Visibility.Collapsed;
        //    FormChoiceReadDocument formChoiceReadDocument = new FormChoiceReadDocument();

        //    if (formChoiceReadDocument.ShowDialog() == true) {
        //        //this.Visibility = Visibility.Visible;
        //    }
        //}
        private void btnDG1_Click(object sender, RoutedEventArgs e) {
            try {
                //connectionSocket = new Connection("127.0.0.1", 9505, true);
                logger.Debug("DG1 CLICKED!!!");
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion

        #region EVENT MAIN 
        //Update 2021.11.25 For Copy Serial Number
        private void btnCopySNDevice_Click(object sender, RoutedEventArgs e) {
            DeviceDetails obj = ((FrameworkElement)sender).DataContext as DeviceDetails;
            System.Windows.Clipboard.SetText(obj.CONTENT);
            //dataGridDetails.Items.Refresh();
        }

        //Update 2021.11.29 For Multiple Click
        private void btnIDocument_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.ClickCount >= 2) {
                btnIDocument.IsEnabled = false;
                e.Handled = true;
            }
        }

        //Disable Enter or Space Keyboard Button Left Verify Finger
        private void btnLeftFinger_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Space) {
                e.Handled = true;
            }
        }

        //Disable Enter or Space Keyboard Button RFID
        private void btnRFID_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Space) {
                e.Handled = true;
            }
        }

        //Disable Enter or Space Keyboard Button Left Verify Right
        private void btnRightFinger_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Space) {
                e.Handled = true;
            }
        }
        #endregion

        #region CONNECT TO DEVICE
        private void btnConnectToDevice_Click(object sender, RoutedEventArgs e) {
            configConnectToDeviceControl.Visibility = Visibility.Visible;
        }
        #endregion

        #region BUTTON_CLICK REFRESH 2022.07.05
        private void btnRefresh_Click(object sender, RoutedEventArgs e) {
            Task.Factory.StartNew(() => {
                try {
                    try {
                        //Update Device Details Then Refresh
                        PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp deviceDetailsRefresh = connectionSocket.refreshReader(true, true, timeoutInterval);
                        if (null != deviceDetailsRefresh) {
                            mainWindow.Dispatcher.Invoke(async () => {
                                LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(dataGridDetails, deviceDetailsRefresh.data.deviceSN,
                                                                                    deviceDetailsRefresh.data.deviceName, deviceDetailsRefresh.data.lastScanTime,
                                                                                    deviceDetailsRefresh.data.totalPreceeded.ToString());
                                ProgressDialogController controllerRefreshSuccess = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_REFRESH_SUCCESS_MESSAGE_BOX);
                                await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                await controllerRefreshSuccess.CloseAsync();
                            });
                        }
                    }
                    catch (Exception ex) {
                        this.Dispatcher.Invoke(async () => {
                            ProgressDialogController controllerErrRefresh = null;
                            if (ex is ISPluginException) {
                                ISPluginException pluginException = (ISPluginException)ex;
                                controllerErrRefresh = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg.ToUpper());
                                await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                await controllerErrRefresh.CloseAsync();
                            }
                            else {
                                controllerErrRefresh = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                                await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                await controllerErrRefresh.CloseAsync();
                            }
                            LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(dataGridDetails, string.Empty,
                                                                                string.Empty, string.Empty,
                                                                                string.Empty);
                        });
                        logger.Error(ex);
                    }
                }
                catch (Exception ex) {
                    this.Dispatcher.Invoke(async () => {
                        ProgressDialogController controllerErrRefresh = null;
                        if (ex is ISPluginException) {
                            ISPluginException pluginException = (ISPluginException)ex;
                            controllerErrRefresh = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg.ToUpper());
                            await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                            await controllerErrRefresh.CloseAsync();
                        }
                        else {
                            controllerErrRefresh = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                            await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                            await controllerErrRefresh.CloseAsync();
                        }
                    });
                    logger.Error(ex);
                }
            });
        }
        #endregion

        #region BUTTON_CLICK SCAN DOCUMENT 2022.07.05
        private void btnScanDocument_Click(object sender, RoutedEventArgs e) {
            mainWindow.Dispatcher.Invoke(async () => {
                try {
                    FormScanType formScanType = new FormScanType();
                    if (formScanType.ShowDialog() == true) {
                        if (!formScanType.isCancelScanDoc) {
                            string scanTypeInput = formScanType.getTextCmbScanType();
                            bool saveEnable = formScanType.getCbSaveEnable();
                            PluginICAOClientSDK.Response.ScanDocument.ScanDocumentResp scanDocumentResp = null;
                            await Task.Factory.StartNew(async () => {
                                try {
                                    switch (scanTypeInput) {
                                        case "JPG":
                                            scanDocumentResp = connectionSocket.scanDocumentResp(ScanType.JPG, saveEnable, timeoutInterval);
                                            break;
                                        case "PDF":
                                            scanDocumentResp = connectionSocket.scanDocumentResp(ScanType.PDF, saveEnable, timeoutInterval);
                                            break;
                                    }
                                }
                                catch (Exception ex) {
                                    ProgressDialogController controllerErrScanDoc = null;
                                    if (ex is ISPluginException) {
                                        await this.Dispatcher.InvokeAsync(async () => {
                                            ISPluginException pluginException = (ISPluginException)ex;
                                            controllerErrScanDoc = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg.ToUpper());
                                            await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                            await controllerErrScanDoc.CloseAsync();
                                        });
                                    }
                                    else {
                                        await this.Dispatcher.InvokeAsync(async () => {
                                            controllerErrScanDoc = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                                            await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                            await controllerErrScanDoc.CloseAsync();
                                        });
                                    }
                                    logger.Error(ex);
                                }
                            });

                            if (null != scanDocumentResp) {
                                ProgressDialogController controllerRefreshSuccess = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_SCAN_SUCCESS_MESSAGE_BOX);
                                await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                                await controllerRefreshSuccess.CloseAsync();
                                //logger.Debug(JsonConvert.SerializeObject(scanDocumentResp));
                                FormResultScanDocument formResultScanDocument = new FormResultScanDocument();
                                formResultScanDocument.scanType = scanDocumentResp.data.scanType;
                                switch (formResultScanDocument.scanType) {
                                    case "JPG":
                                        formResultScanDocument.setJpgScanDoc(scanDocumentResp.data.document);
                                        if (formResultScanDocument.ShowDialog() == true) { }
                                        break;
                                    case "PDF":
                                        formResultScanDocument.setPdfScanDoc(scanDocumentResp.data.document);
                                        break;
                                }
                            }
                        }
                        else {
                            logger.Warn("CANCLE SCAN DOCUMENT");
                            return;
                        }

                    }
                }
                catch (Exception ex) {
                    ProgressDialogController controllerErrScanDoc = null;
                    if (ex is ISPluginException) {
                        ISPluginException pluginException = (ISPluginException)ex;
                        controllerErrScanDoc = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg.ToUpper());
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerErrScanDoc.CloseAsync();
                    }
                    else {
                        controllerErrScanDoc = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await controllerErrScanDoc.CloseAsync();
                    }
                    logger.Error(ex);
                }
            });
        }
        #endregion
    }

    #region ANIMATION HEADER TEXT MARQUEE CLASS 
    //Animation Header Text Marquee
    public class NegatingConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is double) {
                return -((double)value);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is double) {
                return +(double)value;
            }
            return value;
        }
    }
    #endregion
}

