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
        private int timeoutInterval;

        public GetConnectToDevice(bool confirmEnabled, string confirmCode, 
                                  string clientName, ConfigConnect configConnect,
                                  int timeoutInterval, ISPluginClient pluginClient) {
            this.confirmEnabled = confirmEnabled;
            this.confirmCode = confirmCode;
            this.clientName = clientName;
            this.configConnect = configConnect;
            this.timeoutInterval = timeoutInterval;
            this.clientPlugin = pluginClient;
        }

        public PluginICAOClientSDK.Response.ConnectToDevice.ConnectToDeviceResp getConnectToDevice() {
            return clientPlugin.connectToDevice(confirmEnabled, confirmCode,
                                                clientName, configConnect, timeoutInterval);
        }
    }
}
