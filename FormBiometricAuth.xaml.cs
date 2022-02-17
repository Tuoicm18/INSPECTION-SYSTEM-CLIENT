using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using MahApps.Metro.Controls;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormBiometricAuth.xaml
    /// </summary>
    public partial class FormBiometricAuth : MetroWindow {
        public MainWindow mainWindow = new MainWindow();
        public FormBiometricAuth() {
            InitializeComponent();
        }

        public void setImageSource(string path) {
            imgResult.Source = new BitmapImage(new Uri(path, UriKind.Relative));
        }
        public void setContenLabelResult(string content) {
            lbResult.Content = content;
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
