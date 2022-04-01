using PluginICAOClientSDK;
using PluginICAOClientSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetConnectToDevice {
        private ISPluginClient clientPlugin;
        private bool confirmEnabled;
        private string confirmCode;
        private string clientName;
        private ConfigConnect configConnect;
        private int timeOutInterVal;
        public TimeSpan timeOutResp;

        public GetConnectToDevice(ISPluginClient pluginClient, bool confirmEnabled,
                                  string confirmCode, string clientName,
                                  ConfigConnect configConnect, int timeOutInterVal,
                                  TimeSpan timeOutResp) {
            this.clientPlugin = pluginClient;
            this.confirmEnabled = confirmEnabled;
            this.confirmCode = confirmCode;
            this.clientName = clientName;
            this.configConnect = configConnect;
            this.timeOutInterVal = timeOutInterVal;
            this.timeOutResp = timeOutResp;
        }

        public PluginICAOClientSDK.Response.ConnectToDevice.BaseConnectToDeviceResp getConnectToDevice() {
            return clientPlugin.connectToDevice(this.confirmEnabled, this.confirmCode,
                                                this.clientName, this.configConnect,
                                                this.timeOutResp, this.timeOutInterVal);
        }
    }
}
