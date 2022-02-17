using System;
using System.Collections.Generic;
using System.IO;

namespace ClientInspectionSystem {
    public class Logmanager {
        private static readonly string clientLog = Path.Combine(Environment.CurrentDirectory, @"Data\", "clientIS.log");
        private static readonly object lck = new object();
        private static Logmanager instance = null;
        public bool writeLogEnabled { get; set; }

        public static Logmanager Instance {
            get
            {
                lock (lck) {
                    if (instance == null) {
                        instance = new Logmanager();
                    }
                    return instance;
                }
            }
        }

        private Logmanager() { }

        public void writeLog(string content) {
            try {
                if (writeLogEnabled) {
                    lock (clientLog) {
                        using (StreamWriter sw = File.AppendText(clientLog)) {
                            sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff tt") + "  " + content + "\n");
                        }
                    }

                }
            }
            catch (Exception e) {
                lock (clientLog) {
                    using (StreamWriter sw = File.AppendText(clientLog)) {
                        sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff tt") + "  " + "==========EXCEPTION WRITE READER LOG========== " + e.ToString() + "\n");
                    }
                }
            }
        }
    }
}
