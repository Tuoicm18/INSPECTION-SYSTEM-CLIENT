using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ControlzEx.Theming;
using log4net;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormResultScanDocument.xaml
    /// </summary>
    public partial class FormResultScanDocument : MetroWindow {

        #region VARIABLE
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string scanType { get; set; } = string.Empty;
        #endregion
        public FormResultScanDocument() {
            try {
                InitializeComponent();
                // Set the window theme to Dark Mode
                ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }

        #region SHOW JPG SCAN DOCUMENT
        public void setJpgScanDoc(string base64Img) {
            try {
                imgScan.Source = ClientExtentions.base64ToBitmapImage(base64Img);
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion

        #region SHOW PDF SCAN DOCUMENT
        public void setPdfScanDoc() {
            try {
                //byte[] pdfDecode = Convert.FromBase64String(base64Doc);
                //File.WriteAllBytes("test.pdf", pdfDecode);
                //pdfScan.CoreWebView2.Navigate("https://docs.microsoft.com/");
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion

        #region EVENT BUTTON CLICK
        private void btnOK_Click(object sender, RoutedEventArgs e) {
            try {
                DialogResult = true;
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            try {
                switch (scanType) {
                    case "JPG":
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "Images|*.jpg;*.bmp;*.png";
                        saveFileDialog.FileName = ClientExtentions.generateUUID();
                        ImageFormat format = ImageFormat.Jpeg;
                        if (saveFileDialog.ShowDialog() == true) {
                            var encoder = new JpegBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgScan.Source));
                            using (var stream = saveFileDialog.OpenFile()) {
                                encoder.Save(stream);
                            }
                        }
                        break;
                    case "PDF":
                        break;
                }
            }
            catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion
    }
}
