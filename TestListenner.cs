using System;
using log4net;
using Newtonsoft.Json;
using PluginICAOClientSDK;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.CardDetectionEvent;
using PluginICAOClientSDK.Response.GetDocumentDetails;

namespace ClientInspectionSystem {
    public class TestListenner : ISPluginClient.ISListener, ISPluginClient.DetailsListener {
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void doSend(string cmd, string id, ISMessage<object> data) {
            throw new NotImplementedException();
        }

        public void onConnected() {
            logger.Debug("CONNECTED SOCKET");
            //throw new NotImplementedException();
        }

        public void onDisconnected() {
            logger.Debug("DISCONNECTED SOCKET");
            //throw new NotImplementedException();
        }

        public void onError(Exception error) {
            logger.Error(error.ToString());
        }

        public void onPreConnect() {
            throw new NotImplementedException();
        }

        public void onReceive(string cmd, string id, int error, ISMessage<object> data) {
            logger.Debug("ON RECEVICE " + cmd);
            //throw new NotImplementedException();
        }

        public bool onReceivedBiometricResult(BiometricAuthResp baseBiometricAuth) {
            try {
                logger.Debug(JsonConvert.SerializeObject(baseBiometricAuth));
                return true;
            }
            catch (Exception ex) {
                logger.Error(ex);
                return false;
            }
            //throw new NotImplementedException();
        }

        public bool onReceivedDocument(DocumentDetailsResp document) {
            throw new NotImplementedException();
        }

        public bool onReceviedCardDetectionEvent(CardDetectionEventResp baseCardDetectionEvent) {
            try {
                logger.Debug(JsonConvert.SerializeObject(baseCardDetectionEvent));
                return true;
            }
            catch (Exception ex) {
                logger.Error(ex);
                return false;
            }
            //throw new NotImplementedException();
        }
    }
}
