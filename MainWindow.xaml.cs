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
using PluginICAOClientSDK;
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
using PluginICAOClientSDK.Models;

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
        private string logPath = Path.Combine(Environment.CurrentDirectory, @"Data\", "clientIS.log");
        //private static readonly string endPointUrl = "ws://192.168.3.170:9505/ISPlugin";
        public DeleagteConnect deleagteConnect;
        public DelegateAutoDocument delegateAutoGetDoc;
        public Connection connectionSocket;
        public bool isWSS;
        public ClientListener clientListener = new ClientListener();
        private BrushConverter brushConverter = new BrushConverter();
        private bool manualReadDoc = false;
        private IniFile iniFile = new IniFile("Data\\clientIS.ini");
        private int timeOutSocket;
        #endregion

        #region MAIN
        public MainWindow() {
            InitializeComponent();

            string procName = Process.GetCurrentProcess().ProcessName;
            Logmanager.Instance.writeLog(procName);
            Process[] pname = Process.GetProcessesByName(procName);
            if (pname.Length > 1) {
                MessageBox.Show("CLIENT PLUGIN IS RUNNING", "WARING", MessageBoxButton.OK);
                System.Windows.Application.Current.Shutdown();
            }

            //Create Log File If Not Exists
            if (!File.Exists(logPath)) {
                using (FileStream fs = File.Create(logPath)) { };
            }
            //Enable Write Log
            Logmanager.Instance.writeLogEnabled = true;

            // Set the window theme to Dark Mode
            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");

            //Find Connection Socket Server
            try {
                deleagteConnect = new DeleagteConnect(delegateFindConnect);
                delegateAutoGetDoc = new DelegateAutoDocument(autoGetDocumentDetails);
                connectionSocket = new Connection(deleagteConnect);
                //connectionSocket.findConnect(this);
            }
            catch (Exception eConnection) {
                Logmanager.Instance.writeLog("CONNECTION SOCET SERVER ERROR " + eConnection);
            }

            btnDisconnect.IsEnabled = false;
        }
        #endregion

        #region DISABLE ENTER EVENT BUTTO
        private void btnIDocument_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Space) {
                e.Handled = true;
            }
        }
        #endregion

        #region DELEGATE
        private void delegateFindConnect(bool isConnect) {
            try {
                this.Dispatcher.Invoke(() => {
                    if (!isConnect) {
                        this.IsEnabled = false;
                    }
                    else {
                        this.IsEnabled = true;
                    }
                });
            }
            catch (Exception e) {
                Logmanager.Instance.writeLog("RE-CONNECT ERROR " + e.ToString());
            }
        }

        public void autoGetDocumentDetails(BaseDocumentDetailsResp documentDetailsResp) {
            //ProgressDialogController controllerReadChip = null;
            if(manualReadDoc == false) {
                this.Dispatcher.Invoke(async () => {
                    try {
                        btnIDocument.IsEnabled = false;
                        clearLayout(true);
                        showMain();
                        optionsControl.Visibility = Visibility.Collapsed;
                        //controllerReadChip = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                        //                                                  InspectionSystemContanst.CONTENT_READING_CHIP_MESSAGE_BOX);
                        //controllerReadChip.SetIndeterminate();

                        await Task.Factory.StartNew(() => {
                            bool getDocSuccess = autoGetDocumentToLayout(documentDetailsResp);
                            if (getDocSuccess) {
                                //controllerReadChip.CloseAsync();
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
                        Logmanager.Instance.writeLog("BUTTON READ CHIP EXCEPTION " + eReadChip.ToString());
                    } finally {
                        btnIDocument.IsEnabled = true;
                    }
                });
            }
        }

        private bool autoGetDocumentToLayout(BaseDocumentDetailsResp documentDetailsResp) {
            try {
                if (null != documentDetailsResp) {
                    //Logmanager.Instance.writeLog("<DEBUG> AUTO GET DOCUMENT RESPONSE " + JsonConvert.SerializeObject(documentDetailsResp));
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
                        timeOutSocket = int.Parse(iniFile.IniReadValue(ClientContants.SECTION_OPTIONS_SOCKET, ClientContants.KEY_OPTIONS_SOCKET_TIME_OUT));
                        TimeSpan timeOutGetDeviceDetails = TimeSpan.FromSeconds(timeOutSocket);

                        //Update Device Details
                        PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp deviceDetailsResp = connectionSocket.getDeviceDetails(true, true, timeOutGetDeviceDetails, timeOutSocket);

                        if (null != deviceDetailsResp) {
                            LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(dataGridDetails, deviceDetailsResp.data.deviceSN,
                                                    deviceDetailsResp.data.deviceName, deviceDetailsResp.data.lastScanTime,
                                                    deviceDetailsResp.data.totalPreceeded.ToString());
                        }
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

                    Logmanager.Instance.writeLog("AUTO GET DOCUMENT DETAILS SUCCESS");
                    return true;
                }
                else {
                    Logmanager.Instance.writeLog("AUTO GET DOCUMENT DETAILS FAILURE <DATA IS NULL>");
                    return false;
                }
            }
            catch (Exception eAutoDoc) {
                Logmanager.Instance.writeLog("ERROR AUTO GET DOCUMENT " + eAutoDoc);
                throw eAutoDoc;
            }
        }
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
                try {
                    btnIDocument.IsEnabled = false;
                    clearLayout(true);
                    showMain();
                    optionsControl.Visibility = Visibility.Collapsed;
                    controllerReadChip = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                      InspectionSystemContanst.CONTENT_READING_CHIP_MESSAGE_BOX);
                    controllerReadChip.SetIndeterminate();
                    //Call socket server
                    bool mrzEnabled = true;
                    bool imageEnabled = true;
                    bool dataGroupEnabled = false;
                    bool optionalDetailsEnabled = true;

                    timeOutSocket = int.Parse(iniFile.IniReadValue(ClientContants.SECTION_OPTIONS_SOCKET, ClientContants.KEY_OPTIONS_SOCKET_TIME_OUT));
                    TimeSpan timeOutForResp = TimeSpan.FromSeconds(timeOutSocket);

                    //await Task.Delay(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_10S);
                    await Task.Factory.StartNew(() => {
                        try {
                            bool getDocSuccess = getDocumentDetailsToLayout(mrzEnabled, imageEnabled,
                        dataGroupEnabled, optionalDetailsEnabled, timeOutForResp);
                            if (getDocSuccess) {
                                controllerReadChip.CloseAsync();
                            }
                            else {
                                controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                                Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_5k);
                                controllerReadChip.CloseAsync();
                            }
                        } catch ( Exception exx) {
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

                    controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_5k);
                    await controllerReadChip.CloseAsync();
                    clearLayout(false);
                    Logmanager.Instance.writeLog("BUTTON READ CHIP EXCEPTION " + eReadChip.ToString());
                } finally {
                    //Update 2022.02.28
                    manualReadDoc = false;
                    btnIDocument.IsEnabled = true;
                }
            });
        }

        private bool getDocumentDetailsToLayout(bool mrzEnabled, bool imageEnabled,
                                                bool dataGroupEnabled, bool optionalEnabled,
                                                TimeSpan timeOutResp) {
            try {
                BaseDocumentDetailsResp documentDetailsResp = connectionSocket.getDocumentDetails(mrzEnabled, imageEnabled,
                                                                                        dataGroupEnabled, optionalEnabled,
                                                                                        timeOutResp, null,
                                                                                        timeOutSocket);
                if (null != documentDetailsResp) {
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
                        if (optionalEnabled) {
                            LoadDataForDataGrid.loadDataGridMain(dataGridInputDevice, documentDetailsResp.data.optionalDetails);
                        }

                        timeOutSocket = int.Parse(iniFile.IniReadValue(ClientContants.SECTION_OPTIONS_SOCKET, ClientContants.KEY_OPTIONS_SOCKET_TIME_OUT));

                        //Update Device Details
                        PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp deviceDetailsResp = connectionSocket.getDeviceDetails(true, true,
                                                                                                                                               TimeSpan.FromSeconds(timeOutSocket),
                                                                                                                                               timeOutSocket);

                        if (null != deviceDetailsResp) {
                            LoadDataForDataGrid.loadDataDetailsDeviceNotConnect(dataGridDetails, deviceDetailsResp.data.deviceSN,
                                                    deviceDetailsResp.data.deviceName, deviceDetailsResp.data.lastScanTime,
                                                    deviceDetailsResp.data.totalPreceeded.ToString());
                        }
                    });

                    if (!documentDetailsResp.data.mrzString.Equals(string.Empty)) { updateBackgroundBtnDG(btnMRZ, 2); }
                    if (documentDetailsResp.data.bacEnabled) { updateBackgroundBtnDG(btnBAC, 2); }
                    if (documentDetailsResp.data.paceEnabled) { updateBackgroundBtnDG(btnSAC, 2); }
                    if (documentDetailsResp.data.activeAuthenticationEnabled) { updateBackgroundBtnDG(btnAA, 2); }
                    if (documentDetailsResp.data.chipAuthenticationEnabled) { updateBackgroundBtnDG(btnCA, 2); }
                    if (documentDetailsResp.data.terminalAuthenticationEnabled) { updateBackgroundBtnDG(btnTA, 2); }
                    if (documentDetailsResp.data.passiveAuthenticationEnabled) { updateBackgroundBtnDG(btnSOD, 2); }
                    if (!documentDetailsResp.data.efCom.Equals(string.Empty)) { updateBackgroundBtnDG(btnEF, 2); }
                    if (!documentDetailsResp.data.efCardAccess.Equals(string.Empty)) { updateBackgroundBtnDG(btnCSC, 2); }

                    Logmanager.Instance.writeLog("MANUAL GET DOCUMENT DETAILS SUCCESS");
                    return true;
                }
                else {
                    Logmanager.Instance.writeLog("MANUAL GET DOCUMENT DETAILS FAILURE <DATA IS NULL>");
                    return false;
                }
            }
            catch (Exception e) {
                Logmanager.Instance.writeLog("GET DOCUMENT DETAILS IN MAIN WINDOW ERROR " + e.ToString());
                throw e;
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
                    Logmanager.Instance.writeLog(eMain.ToString());
                }
            });
        }
        #endregion

        #region BUTTON OPTONS HANDLE
        //private void btnOption_Click(object sender, RoutedEventArgs e) {
        //    hideMain();
        //    optionsControl.Visibility = Visibility.Visible;
        //}
        #endregion

        #region HIDE MAIN WINDOW
        private void hideMain() {
            stackPanelMainContent.Visibility = Visibility.Hidden;
            stackPanelAvatar.Visibility = Visibility.Hidden;
            btnCopyMRZ.Visibility = Visibility.Hidden;
            imgAvatar.Visibility = Visibility.Hidden;
            lbMRZ.Visibility = Visibility.Hidden;
        }
        #endregion

        #region SHOW MAIN WINDOW
        private void showMain() {
            optionsControl.Visibility = Visibility.Hidden;
            stackPanelMainContent.Visibility = Visibility.Visible;
            stackPanelAvatar.Visibility = Visibility.Visible;
            //btnCopyMRZ.Visibility = Visibility.Visible;
            imgAvatar.Visibility = Visibility.Visible;
            lbMRZ.Visibility = Visibility.Visible;
        }
        #endregion

        #region HANDLE BUTTON CONNECT WEBSOCKET SERVER
        private void btnConnect_Click(object sender, RoutedEventArgs e) {
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
                btnConnect.IsEnabled = false;
                isWSS = false;
                conncetSocketControl.lbTitleConnectSocket.Content = "NORMAL SOCKET CONNECTION";
                conncetSocketControl.Visibility = Visibility.Visible;
            }
            catch (Exception eWS) {
                Logmanager.Instance.writeLog("CONNECT WS ERROR " + eWS.ToString());
            }
        }
        //Conncet Secure Connect
        private void MenuItemWSS_Click(object sender, RoutedEventArgs e) {
            try {
                //connectionSocket.findConnect(this, true);
                btnConnect.IsEnabled = false;
                isWSS = true;
                conncetSocketControl.lbTitleConnectSocket.Content = "SECURE SOCKET CONNECTION";
                conncetSocketControl.Visibility = Visibility.Visible;
            }
            catch (Exception eWSS) {
                Logmanager.Instance.writeLog("CONNECT WSS ERROR " + eWSS.ToString());
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
                Logmanager.Instance.writeLog("EXCEPTION UPDATE BACKGROUND BUTTON " + e.ToString());
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
                        controllerFaceAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                          InspectionSystemContanst.CONTENT_WATTING_BIOMETRIC_RESULT_MESSAGE_BOX);
                        controllerFaceAuth.SetIndeterminate();

                        await Task.Factory.StartNew(() => {
                            BaseBiometricAuthResp resultFaceAuth = resultBiometricAuth(formAuthorizationData, BiometricType.TYPE_FACE);
                            if (null != resultFaceAuth) {
                                if (resultFaceAuth.errorCode == ClientContants.SOCKET_RESP_CODE_BIO_AUTH_DENIED) { // Cancel Auth
                                    controllerFaceAuth.CloseAsync();
                                    this.Dispatcher.Invoke(() => {
                                        formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FACE);
                                        formBiometricAuth.Topmost = true;
                                        formBiometricAuth.hideLabelForDeniedAuth();
                                        formBiometricAuth.setContentLabelResponseCode(resultFaceAuth.errorCode + "-" + resultFaceAuth.errorMessage);
                                        if (formBiometricAuth.ShowDialog() == true) { }
                                    });
                                }
                                else {
                                    if (resultFaceAuth.data.result) {
                                        controllerFaceAuth.CloseAsync();
                                        this.Dispatcher.Invoke(() => {
                                            //Init Form Result Biometric Auth
                                            initFormResultBiometricAuth(resultFaceAuth, controllerFaceAuth, BiometricType.TYPE_FACE);
                                            Logmanager.Instance.writeLog("<DEBUG> GET RESPONSE BIOMETRIC AUTH FACE" + JsonConvert.SerializeObject(resultFaceAuth, Formatting.Indented));

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
                                        controllerFaceAuth.CloseAsync();
                                        this.Dispatcher.Invoke(() => {
                                            //Init Form Result Biometric Auth
                                            initFormResultBiometricAuth(resultFaceAuth, controllerFaceAuth, BiometricType.TYPE_FACE);
                                            Logmanager.Instance.writeLog("<DEBUG> GET RESPONSE BIOMETRIC AUTH FACE" + JsonConvert.SerializeObject(resultFaceAuth, Formatting.Indented));

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
                            }
                        });
                        btnRFID.IsEnabled = true;
                    }
                }
                catch (Exception ex) {
                    Logmanager.Instance.writeLog("ERROR FACE AUTH " + ex.ToString());
                    controllerFaceAuth.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_3k);
                    await controllerFaceAuth.CloseAsync();
                    btnRFID.IsEnabled = true;
                }
            });
        }

        private BaseBiometricAuthResp resultBiometricAuth(FormAuthenticationDataNew formAuthorizationData, string biometricType) {
            BaseBiometricAuthResp resultBiometricResp = null;
            try {
                //Set Time Out
                timeOutSocket = int.Parse(iniFile.IniReadValue(ClientContants.SECTION_OPTIONS_SOCKET, ClientContants.KEY_OPTIONS_SOCKET_TIME_OUT));
                TimeSpan timeOutResp = TimeSpan.FromSeconds(timeOutSocket);

                AuthorizationData authorizationData = null;
                this.Dispatcher.Invoke(() => {
                    if (formAuthorizationData.CheckImportJson) {
                        string jsonImport = formAuthorizationData.getImportJson();
                        authorizationData = ISExtentions.deserializeJsonAuthorizationData(jsonImport);
                    }
                    else {
                        authorizationData = new AuthorizationData();
                        authorizationData.authorizationTitle = formAuthorizationData.Title;
                        authorizationData.authContentList = formAuthorizationData.getDataContentList();
                        authorizationData.multipleSelectList = formAuthorizationData.getDataMultipleChoices();
                        authorizationData.singleSelectList = formAuthorizationData.getDataSingleChoices();
                        authorizationData.nameValuePairList = formAuthorizationData.getDataNVP();
                    }
                });

                BaseBiometricAuthResp resultBiometric = connectionSocket.getResultBiometricAuth(biometricType, authorizationData, timeOutResp, timeOutSocket);
                if (null != resultBiometric) {
                    //resultAuthFace = resultBiometric.result;
                    resultBiometricResp = resultBiometric;
                }
                return resultBiometricResp;
            }
            catch (Exception ex) {
                Logmanager.Instance.writeLog("ERROR GET RESULT BIOMETRIC " + ex.ToString());
                //resultAuthFace = false;
                throw ex;
            }
        }
        #endregion

        #region BUTTON_CLICK LEFT FINGER
        private void btnLeftFinger_Click(object sender, RoutedEventArgs e) {
            ProgressDialogController controllerLeftFingerAuth = null;
            BaseBiometricAuthResp resultLeftFingerAuth;
            this.Dispatcher.Invoke(async () => {
                try {
                    btnLeftFinger.IsEnabled = false;
                    showMain();
                    FormAuthenticationDataNew formAuthorizationData = new FormAuthenticationDataNew();
                    FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                    if (formAuthorizationData.ShowDialog() == true) {
                        //Form Watting
                        controllerLeftFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                                InspectionSystemContanst.CONTENT_WATTING_BIOMETRIC_RESULT_MESSAGE_BOX);
                        controllerLeftFingerAuth.SetIndeterminate();
                        await Task.Factory.StartNew(() => {
                            resultLeftFingerAuth = resultBiometricAuth(formAuthorizationData, BiometricType.TYPE_FINGER_LEFT);
                            if (null != resultLeftFingerAuth) {
                                if (resultLeftFingerAuth.errorCode == ClientContants.SOCKET_RESP_CODE_BIO_AUTH_DENIED) { // Cancel Auth
                                    controllerLeftFingerAuth.CloseAsync();
                                    this.Dispatcher.Invoke(() => {
                                        formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                        formBiometricAuth.Topmost = true;
                                        formBiometricAuth.hideLabelForDeniedAuth();
                                        formBiometricAuth.setContentLabelResponseCode(resultLeftFingerAuth.errorCode + "-" + resultLeftFingerAuth.errorMessage);
                                        if (formBiometricAuth.ShowDialog() == true) { }
                                    });
                                }
                                else {
                                    if (resultLeftFingerAuth.data.result) {
                                        controllerLeftFingerAuth.CloseAsync();
                                        this.Dispatcher.Invoke(() => {
                                            //Init Form Result Biometric Auth
                                            initFormResultBiometricAuth(resultLeftFingerAuth, controllerLeftFingerAuth, BiometricType.TYPE_FINGER_LEFT);
                                            Logmanager.Instance.writeLog("<DEBUG> GET RESPONSE BIOMETRIC AUTH LEFT FINGER" + JsonConvert.SerializeObject(resultLeftFingerAuth, Formatting.Indented));

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
                                        controllerLeftFingerAuth.CloseAsync();
                                        this.Dispatcher.Invoke(() => {
                                            //Init Form Result Biometric Auth
                                            initFormResultBiometricAuth(resultLeftFingerAuth, controllerLeftFingerAuth, BiometricType.TYPE_FINGER_LEFT);
                                            Logmanager.Instance.writeLog("<DEBUG> GET RESPONSE BIOMETRIC AUTH LEFT FINGER" + JsonConvert.SerializeObject(resultLeftFingerAuth, Formatting.Indented));

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
                            }
                        });
                        btnLeftFinger.IsEnabled = true;
                    }
                }
                catch (Exception eLeft) {
                    Logmanager.Instance.writeLog("ERROR GET RESULT BIOMETRIC LEFT FIGNER " + eLeft.ToString());
                    controllerLeftFingerAuth.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_5k);
                    await controllerLeftFingerAuth.CloseAsync();
                    btnLeftFinger.IsEnabled = true;
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
                        controllerRightFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                           InspectionSystemContanst.CONTENT_WATTING_BIOMETRIC_RESULT_MESSAGE_BOX);
                        controllerRightFingerAuth.SetIndeterminate();

                        await Task.Factory.StartNew(() => {
                            BaseBiometricAuthResp resultFingerRightAuth = resultBiometricAuth(formAuthorizationData, BiometricType.TYPE_FINGER_RIGHT);
                            if (null != resultFingerRightAuth) {

                                if (resultFingerRightAuth.errorCode == ClientContants.SOCKET_RESP_CODE_BIO_AUTH_DENIED) { // Cancel Auth
                                    controllerRightFingerAuth.CloseAsync();
                                    this.Dispatcher.Invoke(() => {
                                        formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                        formBiometricAuth.Topmost = true;
                                        formBiometricAuth.hideLabelForDeniedAuth();
                                        formBiometricAuth.setContentLabelResponseCode(resultFingerRightAuth.errorCode + "-" + resultFingerRightAuth.errorMessage);
                                        if (formBiometricAuth.ShowDialog() == true) { }
                                    });
                                }
                                else {
                                    if (resultFingerRightAuth.data.result) {
                                        controllerRightFingerAuth.CloseAsync();
                                        this.Dispatcher.Invoke(() => {
                                            //Init Form Result Biometric Auth
                                            initFormResultBiometricAuth(resultFingerRightAuth, controllerRightFingerAuth, BiometricType.TYPE_FINGER_RIGHT);
                                            Logmanager.Instance.writeLog("<DEBUG> GET RESPONSE BIOMETRIC AUTH RIGHT FINGER " +
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
                                        controllerRightFingerAuth.CloseAsync();
                                        this.Dispatcher.Invoke(() => {
                                            //Init Form Result Biometric Auth
                                            initFormResultBiometricAuth(resultFingerRightAuth, controllerRightFingerAuth, BiometricType.TYPE_FINGER_RIGHT);
                                            Logmanager.Instance.writeLog("<DEBUG> GET RESPONSE BIOMETRIC AUTH RIGHT FINGER " +
                                                                         JsonConvert.SerializeObject(resultFingerRightAuth, Formatting.Indented));

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
                            }
                        });
                        btnRightFinger.IsEnabled = true;
                    }
                }
                catch (Exception eLeft) {
                    Logmanager.Instance.writeLog("ERROR GET RESULT BIOMETRIC LEFT FIGNER " + eLeft.ToString());
                    controllerRightFingerAuth.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_3k);
                    await controllerRightFingerAuth.CloseAsync();
                    btnRightFinger.IsEnabled = true;
                }
            });
        }
        #endregion

        #region INIT FORM RESULT BIOMETRIC AUTH
        public void initFormResultBiometricAuth(BaseBiometricAuthResp baseBiometricAuthResp, ProgressDialogController controllerLeftFingerAuth, string biometricType) {
            if (null != baseBiometricAuthResp) {
                if (null != baseBiometricAuthResp.data.authorizationData) {
                    AuthorizationData authorizedData = baseBiometricAuthResp.data.authorizationData;
                    FormResultAuthorizationData formResultAuthorization = new FormResultAuthorizationData();
                    formResultAuthorization.Topmost = true;
                    if (checkNullReslutTransactionData(authorizedData) == false) {
                        //Render Title Form Result Biometric Auth
                        formResultAuthorization.renderTitleForm(baseBiometricAuthResp.data.authorizationData.authorizationTitle);

                        Dictionary<int, AuthorizationElement> dicRenderResult = new Dictionary<int, AuthorizationElement>();
                        //Content List
                        if (null != authorizedData.authContentList) {
                            foreach (var v in authorizedData.authContentList) {
                                v.type = AuthElementType.Content;
                                dicRenderResult.Add(v.ordinary, v);
                            }
                        }
                        //Multiple
                        if (null != authorizedData.multipleSelectList) {
                            foreach (var v in authorizedData.multipleSelectList) {
                                v.type = AuthElementType.Multiple;
                                dicRenderResult.Add(v.ordinary, v);
                            }
                        }
                        //Single
                        if (null != authorizedData.singleSelectList) {
                            foreach (var v in authorizedData.singleSelectList) {
                                v.type = AuthElementType.Single;
                                dicRenderResult.Add(v.ordinary, v);
                            }
                        }
                        //NVP
                        if (null != authorizedData.nameValuePairList) {
                            foreach (var v in authorizedData.nameValuePairList) {
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
                else {
                    if (baseBiometricAuthResp.errorCode == ClientContants.SOCKET_RESP_CODE_BIO_SUCCESS) {
                        if (BiometricType.TYPE_FINGER_LEFT.Equals(biometricType)) {
                            controllerLeftFingerAuth.CloseAsync();
                            FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                            this.Dispatcher.Invoke(() => {
                                formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                formBiometricAuth.renderResultBiometricAuht(baseBiometricAuthResp);
                                formBiometricAuth.Topmost = true;
                                if (formBiometricAuth.ShowDialog() == true) { }
                            });
                        }
                        else if (BiometricType.TYPE_FINGER_RIGHT.Equals(biometricType)) {
                            controllerLeftFingerAuth.CloseAsync();
                            FormBiometricAuth formBiometricAuth = new FormBiometricAuth();
                            this.Dispatcher.Invoke(() => {
                                formBiometricAuth.setTitleForm(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                formBiometricAuth.renderResultBiometricAuht(baseBiometricAuthResp);
                                formBiometricAuth.Topmost = true;
                                if (formBiometricAuth.ShowDialog() == true) { }
                            });
                        }
                        else {
                            controllerLeftFingerAuth.CloseAsync();
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
        private bool checkNullReslutTransactionData(AuthorizationData authorizedData) {
            List<AuthorizationElement> elementContent = authorizedData.authContentList;
            List<AuthorizationElement> elementMultiple = authorizedData.multipleSelectList;
            List<AuthorizationElement> elementSingle = authorizedData.singleSelectList;
            List<AuthorizationElement> elementNVP = authorizedData.nameValuePairList;
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

        #region TEST FORM AUTHORIZATION DATA
        private void btnOption_Click(object sender, RoutedEventArgs e) {
            this.Visibility = Visibility.Collapsed;
            FormAuthenticationDataNew formAuthorizationDataNew = new FormAuthenticationDataNew();

            if (formAuthorizationDataNew.ShowDialog() == true) {
                this.Visibility = Visibility.Visible;
            }
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

