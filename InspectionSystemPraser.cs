using log4net;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClientInspectionSystem {
    public class InspectionSystemPraser {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static System.Windows.Media.ImageSource setImageSource(string path, Image img) {
            return img.Source = new BitmapImage(new Uri(path, UriKind.Relative));
        }

        public static ContentControl setContentLabel(string content, Label label) {
            return (ContentControl)(label.Content = content);
        }

        public static BitmapImage base64ToImage(string base64) {
            try {
                if (base64.Equals(string.Empty)) {
                    throw new Exception("BASE 64 IS NULL");
                }
                else {
                    byte[] binaryData = Convert.FromBase64String(base64);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = new MemoryStream(binaryData);
                    bi.EndInit();
                    return bi;
                }
            } catch(Exception e) {
                logger.Error(e);
                return null;
            }
        }
    }
}
