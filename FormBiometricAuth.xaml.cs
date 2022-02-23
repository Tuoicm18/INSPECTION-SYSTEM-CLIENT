using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Response.BiometricAuth;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormBiometricAuth.xaml
    /// </summary>
    public partial class FormBiometricAuth : MetroWindow {
        private MainWindow mainWindow = new MainWindow();
        public FormBiometricAuth() {
            InitializeComponent();
            // Set the window theme to Dark Mode
            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
        }

        //public void setImageSource(string path) {
        //    imgResult.Source = new BitmapImage(new Uri(path, UriKind.Relative));
        //}

        public void setContenLabelResult(string content) {
            lbResult.Content = content;
        }

        public void setContentLabelType(string content) {
            lbTypeResult.Content = content;
        }

        public void setContentLabelScore(string score) {
            lbScoreResult.Content = score;
        }
        public void setTitleForm(string content) {
            this.Dispatcher.Invoke(() => this.Title = content);
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

        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e) {
            DialogResult = true;
        }

        public void showHideLabelForBiometricAuth(string biometricType) {
            if(BiometricType.TYPE_FINGER_LEFT.Equals(biometricType) || BiometricType.TYPE_FINGER_RIGHT.Equals(biometricType)) {
                lbScore.Visibility = System.Windows.Visibility.Visible;
                lbScoreResult.Visibility = System.Windows.Visibility.Visible;
            } else {
                lbScore.Visibility = System.Windows.Visibility.Collapsed;
                lbScoreResult.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        public void hideLabelForDeniedAuth() {
            lbType.Visibility = System.Windows.Visibility.Collapsed;
            lbTypeResult.Visibility = System.Windows.Visibility.Collapsed;
            lbScore.Visibility = System.Windows.Visibility.Collapsed;
            lbScoreResult.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
