using log4net;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Response.ConnectToDevice;
using PluginICAOClientSDK.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ClientInspectionSystem.UserControlsClientIS {
    /// <summary>
    /// Interaction logic for ConfigConnectToDeviceControl.xaml
    /// </summary>
    public partial class ConfigConnectToDeviceControl : UserControl {
        private IniFile iniFile = new IniFile("Data\\clientIS.ini");
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ConfigConnectToDeviceControl() {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e) {
            Window parentWindow = Window.GetWindow(this);
            MainWindow mainWindow = (MainWindow)parentWindow.FindName("mainWindow");
            try {
                string clientName = getClientName();
                bool comfirmEnable = getConfirmEnabled();
                string comfirmCode = getConfirmCode();

                ConfigConnect configConnect = new ConfigConnect();
                configConnect.automaticEnabled = getAutomaticReadEnabled();
                configConnect.mrzEnabled = getMRZEnabled();
                configConnect.imageEnabled = getImgaeEnabled();
                configConnect.dataGroupEnabled = getDataGroupEnabled();
                configConnect.optionalDetailsEnabled = getOptionalDetailsEnabled();

                ConnectToDeviceResp baseConnectToDeviceResp = mainWindow.connectionSocket.getConnectToDevice(comfirmEnable, comfirmCode,
                                                                                                             clientName, configConnect,
                                                                                                             mainWindow.timeoutSocket, mainWindow.timeoutInterval);
                logger.Debug("RESP CONNECT TO DEVICE " + JsonConvert.SerializeObject(baseConnectToDeviceResp, Formatting.Indented));

                if(null != baseConnectToDeviceResp) {
                    FormResultConfigDevice formResultConfigDevice = new FormResultConfigDevice();
                    if(baseConnectToDeviceResp.errorCode == 0) {
                        formResultConfigDevice.setContentStatus(baseConnectToDeviceResp.errorMessage);
                        formResultConfigDevice.setContentDeviceName(baseConnectToDeviceResp.data.deviceName);
                        formResultConfigDevice.setContentDeviceSN(baseConnectToDeviceResp.data.deviceSN);
                        formResultConfigDevice.setContentDeviceIP(baseConnectToDeviceResp.data.deviceIP);
                    } else {
                        formResultConfigDevice.setContentStatus(baseConnectToDeviceResp.errorMessage);
                        formResultConfigDevice.contentForError();
                    }
                    if (formResultConfigDevice.ShowDialog() == true) { }
                }

            } catch (Exception eConfigConenct) {
                if(eConfigConenct is ISPluginException) {
                    ISPluginException pluginException = (ISPluginException)eConfigConenct;
                    mainWindow.Dispatcher.Invoke(async () => {
                        ProgressDialogController progressDialog = await mainWindow.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, pluginException.errMsg);
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await progressDialog.CloseAsync();
                    });
                } else {
                    mainWindow.Dispatcher.Invoke(async () => {
                        ProgressDialogController progressDialog = await mainWindow.ShowProgressAsync(InspectionSystemContanst.TITLE_MESSAGE_BOX, InspectionSystemContanst.CONTENT_FALIL);
                        await Task.Delay(InspectionSystemContanst.DIALOG_TIME_OUT_2k);
                        await progressDialog.CloseAsync();
                    });
                }
                logger.Error(eConfigConenct);
            } finally {
                this.Visibility = Visibility.Collapsed;
            }
        }

        private void btnCancelConnect_Click(object sender, RoutedEventArgs e) {
            this.Visibility = Visibility.Collapsed;
        }

        public string getClientName() {
            return txtClientName.Text;
        }

        public string getConfirmCode() {
            return txtConfirmCode.Text;
        }

        public bool getConfirmEnabled() {
            return (bool)cbConfirmEnabled.IsChecked;
        }

        public bool getAutomaticReadEnabled() {
            return (bool)cbAutoRead.IsChecked;
        }

        public bool getMRZEnabled() {
            return (bool)cbMRZEnabled.IsChecked;
        }

        public bool getImgaeEnabled() {
            return (bool)cbImageEnabled.IsChecked;
        }

        public bool getDataGroupEnabled() {
            return (bool)cbDataGroupEnabled.IsChecked;
        }

        public bool getOptionalDetailsEnabled() {
            return (bool)cbOptionalDetailsEnabled.IsChecked;
        }
    }
}
