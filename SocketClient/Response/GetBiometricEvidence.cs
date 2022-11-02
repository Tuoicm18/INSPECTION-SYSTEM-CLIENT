using PluginICAOClientSDK;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Response.BiometricEvidence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetBiometricEvidence {
        private ISPluginClient clientPlugin;
        private BiometricType biometricType;
        private int timeoutInterval;

        public GetBiometricEvidence(ISPluginClient pluginClient, BiometricType biometricType, int timeoutInterval) {
            this.clientPlugin = pluginClient;
            this.biometricType = biometricType;
            this.timeoutInterval = timeoutInterval;
        }

        public BiometricEvidenceResp getBiometricEvidence() {
            return this.clientPlugin.biometricEvidence(this.biometricType, this.timeoutInterval);
        }
    }
}
