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

                        //Update Device Details
                        PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp deviceDetailsResp = connectionSocket.getDeviceDetails(true, true, TimeSpan.FromSeconds(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_10S));

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
                    TimeSpan timeOutForResp = TimeSpan.FromSeconds(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_20S);

                    //await Task.Delay(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_10S);
                    await Task.Factory.StartNew(() => {
                        bool getDocSuccess = getDocumentDetailsToLayout(mrzEnabled, imageEnabled,
                                                dataGroupEnabled, optionalDetailsEnabled, timeOutForResp);
                        if (getDocSuccess) {
                            controllerReadChip.CloseAsync();
                        }
                        else {
                            controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                            Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_3k);
                            controllerReadChip.CloseAsync();
                        }
                    });
                    btnRFID.IsEnabled = true;
                    btnLeftFinger.IsEnabled = true;
                    btnRightFinger.IsEnabled = true;
                }
                catch (Exception eReadChip) {
                    //Check if auto reviced data.
                    if (null == lbMRZ.Content) {
                        controllerReadChip.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_4k);
                        await controllerReadChip.CloseAsync();
                        clearLayout(false);
                        Logmanager.Instance.writeLog("BUTTON READ CHIP EXCEPTION " + eReadChip.ToString());
                    }
                    else {
                        await controllerReadChip.CloseAsync();
                    }
                } finally {
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
                                                                                        timeOutResp, null);
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

                        //Update Device Details
                        PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp deviceDetailsResp = connectionSocket.getDeviceDetails(true, true,
                                                                                       TimeSpan.FromSeconds(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_1M));

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
                        controllerFaceAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                          InspectionSystemContanst.CONTENT_WATTING_BIOMETRIC_RESULT_MESSAGE_BOX);
                        controllerFaceAuth.SetIndeterminate();
                        await Task.Factory.StartNew(() => {
                            BaseBiometricAuthResp resultFaceAuth = resultBiometricAuth(formAuthorizationData, BiometricType.TYPE_FACE);
                            if (null != resultFaceAuth) {
                                if (resultFaceAuth.data.result) {
                                    controllerFaceAuth.CloseAsync();
                                    this.Dispatcher.Invoke(() => {
                                        formBiometricAuth.setTitleLabel(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FACE);
                                        formBiometricAuth.setContenLabelResult(InspectionSystemContanst.RESUT_FORM_BIOMETRIC_AUTH_SUCCESS);
                                        formBiometricAuth.setImageSource(InspectionSystemContanst.PATH_IMG_FACE_SUCCESS);
                                        formBiometricAuth.Topmost = true;
                                        formBiometricAuth.Show();
                                        formBiometricAuth.StartCloseTimer(3d);
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
                                        formBiometricAuth.setTitleLabel(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FACE);
                                        formBiometricAuth.setContenLabelResult(InspectionSystemContanst.RESUT_FORM_BIOMETRIC_AUTH_FAILURE);
                                        formBiometricAuth.setImageSource(InspectionSystemContanst.PATH_IMG_FACE_FAILURE);
                                        formBiometricAuth.Topmost = true;
                                        formBiometricAuth.Show();
                                        formBiometricAuth.StartCloseTimer(3d);
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
                            this.Dispatcher.Invoke(() => {
                                //Init Form Result Biometric Auth
                                //initFormResultBiometricAuth(resultFaceAuth);
                            });
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
            //bool resultAuthFace = false;
            BaseBiometricAuthResp resultBiometricResp = null;
            try {
                TimeSpan timeOutResp = TimeSpan.FromMinutes(InspectionSystemContanst.TIME_OUT_RESP_SOCKET_1M);
                PrepareAuthData prepareAuthData = new PrepareAuthData();
                prepareAuthData.authContentList = formAuthorizationData.getDataContentList();
                prepareAuthData.multipleSelectList = formAuthorizationData.getDataMultipleChoices();
                prepareAuthData.singleSelectList = formAuthorizationData.getDataSingleChoices();
                prepareAuthData.nameValuePairList = formAuthorizationData.getDataNVP();

                AuthorizationData authorizationData = new AuthorizationData();
                authorizationData.authorizationData = prepareAuthData;

                BaseBiometricAuthResp resultBiometric = connectionSocket.getResultBiometricAuth(biometricType, authorizationData, timeOutResp);
                if (null != resultBiometric) {
                    //resultAuthFace = resultBiometric.result;
                    resultBiometricResp = resultBiometric;
                }
                return resultBiometricResp;
            }
            catch (Exception ex) {
                Logmanager.Instance.writeLog("ERROR GET RESULT BIOMETRIC FACE " + ex.ToString());
                //resultAuthFace = false;
                throw ex;
            }
            //return resultAuthFace;
            //return resultBiometricResp;
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
                        controllerLeftFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                                InspectionSystemContanst.CONTENT_WATTING_BIOMETRIC_RESULT_MESSAGE_BOX);
                        controllerLeftFingerAuth.SetIndeterminate();
                        await Task.Factory.StartNew(() => {
                            BaseBiometricAuthResp resultLeftFingerAuth = resultBiometricAuth(formAuthorizationData, BiometricType.TYPE_FINGER_LEFT);
                            if (null != resultLeftFingerAuth) {
                                if (resultLeftFingerAuth.data.result) {
                                    controllerLeftFingerAuth.CloseAsync();
                                    this.Dispatcher.Invoke(() => {
                                        formBiometricAuth.setTitleLabel(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                        formBiometricAuth.setContenLabelResult(InspectionSystemContanst.RESUT_FORM_BIOMETRIC_AUTH_SUCCESS);
                                        formBiometricAuth.setImageSource(InspectionSystemContanst.PATH_IMG_FINGER_LEFT_SUCCESS);
                                        formBiometricAuth.Topmost = true;
                                        formBiometricAuth.Show();
                                        formBiometricAuth.StartCloseTimer(3d);
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
                                        //Check Score
                                        if (resultLeftFingerAuth.data.score > 0) {
                                            formBiometricAuth.setTitleLabel(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                            formBiometricAuth.setContenLabelResult(InspectionSystemContanst.RESUT_FORM_BIOMETRIC_AUTH_FAILURE);
                                            formBiometricAuth.setImageSource(InspectionSystemContanst.PATH_IMG_FINGER_LEFT_FAILURE);
                                            formBiometricAuth.Topmost = true;
                                            formBiometricAuth.Show();
                                            formBiometricAuth.StartCloseTimer(3d);
                                            updateBackgroundBtnDG(btnSF, -1);
                                        }
                                        else {
                                            formBiometricAuth.setTitleLabel(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                            formBiometricAuth.setContenLabelResult(InspectionSystemContanst.RESUT_FORM_BIOMETRIC_AUTH_NOT_FOUND_FINGER);
                                            formBiometricAuth.setImageSource(InspectionSystemContanst.PATH_IMG_FINGER_LEFT_NOT_FOUND);
                                            formBiometricAuth.Topmost = true;
                                            formBiometricAuth.Show();
                                            formBiometricAuth.StartCloseTimer(3d);
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
                                        }
                                    });
                                }
                            }
                            this.Dispatcher.Invoke(() => {
                                //Init Form Result Biometric Auth
                                //initFormResultBiometricAuth(resultLeftFingerAuth);
                                Logmanager.Instance.writeLog("<DEBUG> GET RESPONSE BIOMETRIC AUTH " + JsonConvert.SerializeObject(resultLeftFingerAuth));
                            });
                        });
                        btnLeftFinger.IsEnabled = true;
                    }
                }
                catch (Exception eLeft) {
                    Logmanager.Instance.writeLog("ERROR GET RESULT BIOMETRIC LEFT FIGNER " + eLeft.ToString());
                    controllerLeftFingerAuth.SetMessage(InspectionSystemContanst.CONTENT_FALIL);
                    await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_3k);
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
                        controllerRightFingerAuth = await this.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX,
                                                                           InspectionSystemContanst.CONTENT_WATTING_BIOMETRIC_RESULT_MESSAGE_BOX);
                        controllerRightFingerAuth.SetIndeterminate();
                        await Task.Factory.StartNew(() => {
                            BaseBiometricAuthResp resultFingerRightAuth = resultBiometricAuth(formAuthorizationData, BiometricType.TYPE_FINGER_RIGHT);
                            if (null != resultFingerRightAuth) {
                                if (resultFingerRightAuth.data.result) {
                                    controllerRightFingerAuth.CloseAsync();
                                    this.Dispatcher.Invoke(() => {
                                        formBiometricAuth.setTitleLabel(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                        formBiometricAuth.setContenLabelResult(InspectionSystemContanst.RESUT_FORM_BIOMETRIC_AUTH_SUCCESS);
                                        formBiometricAuth.setImageSource(InspectionSystemContanst.PATH_IMG_FINGER_RIGHT_SUCCESS);
                                        formBiometricAuth.Topmost = true;
                                        formBiometricAuth.Show();
                                        formBiometricAuth.StartCloseTimer(3d);
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
                                        //Check Score
                                        if (resultFingerRightAuth.data.score > 0) {
                                            formBiometricAuth.setTitleLabel(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                            formBiometricAuth.setContenLabelResult(InspectionSystemContanst.RESUT_FORM_BIOMETRIC_AUTH_FAILURE);
                                            formBiometricAuth.setImageSource(InspectionSystemContanst.PATH_IMG_FINGER_RIGHT_FAILURE);
                                            formBiometricAuth.Topmost = true;
                                            formBiometricAuth.Show();
                                            formBiometricAuth.StartCloseTimer(3d);
                                            updateBackgroundBtnDG(btnSF, -1);
                                        }
                                        else {
                                            formBiometricAuth.setTitleLabel(InspectionSystemContanst.TITLE_FORM_BIOMETRIC_AUTH_FINGER);
                                            formBiometricAuth.setContenLabelResult(InspectionSystemContanst.RESUT_FORM_BIOMETRIC_AUTH_NOT_FOUND_FINGER);
                                            formBiometricAuth.setImageSource(InspectionSystemContanst.PATH_IMG_FINGER_RIGHT_NOT_FOUND);
                                            formBiometricAuth.Topmost = true;
                                            formBiometricAuth.Show();
                                            formBiometricAuth.StartCloseTimer(3d);
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
                                        }
                                    });
                                }
                            }
                            this.Dispatcher.Invoke(() => {
                                //Init Form Result Biometric Auth
                                //initFormResultBiometricAuth(resultFingerRightAuth);
                            });
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
        /*private void initFormResultBiometricAuth(BaseBiometricAuthResp baseBiometricAuthResp) {
            //Show Form Result Biometric Type
            MultipleSelected multipleSelected = baseBiometricAuthResp.data.multipleSelected;
            string titleMultiple = multipleSelected.title;
            List<string> multipleContent = multipleSelected.multipleContent;
            SingleSelected singleSelected = baseBiometricAuthResp.data.singleSelected;
            string titleSingle = singleSelected.title;
            string singleContent = singleSelected.singleContent;
            FormResultAuthorizationData formResultAuthorizationData = new FormResultAuthorizationData();
            if (null != multipleSelected || null != singleSelected) {
                if (!titleMultiple.Equals(string.Empty) && null != multipleContent) {
                    formResultAuthorizationData.initListBoxMultipleSelected(multipleContent);
                    formResultAuthorizationData.setHeaderGroupMultiple(titleMultiple);
                }
                if (!titleSingle.Equals(string.Empty) && !singleContent.Equals(string.Empty)) {
                    formResultAuthorizationData.initListBoxSingleSelected(singleContent);
                    formResultAuthorizationData.setHeaderGroupSingle(titleSingle);
                }
                if (formResultAuthorizationData.ShowDialog() == true) { }
            }
        }*/
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

