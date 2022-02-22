using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using PluginICAOClientSDK.Response.BiometricAuth;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormBiometricAuth.xaml
    /// </summary>
    public partial class FormBiometricAuth : MetroWindow {
        private MainWindow mainWindow = new MainWindow();
        public FormBiometricAuth() {
            InitializeComponent();
        }

        public void setImageSource(string path) {
            imgResult.Source = new BitmapImage(new Uri(path, UriKind.Relative));
        }
        public void setContenLabelResult(string content) {
            lbResult.Content = lbResult.Content.ToString() + "     " + content;
        }

        public void setContentLabelType(string content) {
            lbType.Content = lbType.Content.ToString() + "      " + content;
        }

        public void setContentLabelScore(string score) {
            lbScore.Content = lbScore.Content.ToString() + "      " + score;
        }
        public void setTitleLabel(string content) {
            lbTitleResult.Content = content;
        }

        public void StartCloseTimer(double timeout) {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(timeout);
            timer.Tick += TimerTick;
            timer.Start();
        }

        public void TimerTick(object sender, EventArgs e) {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            this.Close();
            this.Topmost = false;
            //uacISAPP.Hide();
            mainWindow.IsEnabled = true;
        }

    }
}
