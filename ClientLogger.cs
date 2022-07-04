using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInspectionSystem {
    public class ClientLogger {
        #region VARIABLE 2022.05.17
        private static readonly object lck = new object();
        private static ClientLogger instance = null;
        public bool writeLogEnabled { get; set; }
        public ILog Log { get; set; }
        #endregion

        #region CONSTRUCTOR 2022.05.17
        public static ClientLogger Instance {
            get
            {
                lock (lck) {
                    if (instance == null) {
                        instance = new ClientLogger();
                    }
                    return instance;
                }
            }
        }

        private ClientLogger() { }
        #endregion

        #region SETUP LOGGER 2022.05.17
        public void setup() {
            if (!writeLogEnabled) {
                Log.Logger.Repository.Threshold = log4net.Core.Level.Off;
            }
            else {
                Log.Logger.Repository.Threshold = log4net.Core.Level.All;
            }
        }
        #endregion

        #region LOG IN NEW THREAD 2022.05.28
        //Info
        public void InfoTask(ILog logger, object msg) {
            Task.Factory.StartNew(() => { logger.Info(msg); });
        }
        //Debug
        public void DebugTask(ILog logger, object msg) {
            Task.Factory.StartNew(() => { logger.Debug(msg); });
        }

        //Debug
        public void WarnTask(ILog logger, object msg) {
            Task.Factory.StartNew(() => { logger.Warn(msg); });
        }
        //Error
        public void ErrorTask(ILog logger, object msg = null, Exception e = null) {
            Task.Factory.StartNew(() => { logger.Error(msg, e); });
        }
        #endregion
    }
}
