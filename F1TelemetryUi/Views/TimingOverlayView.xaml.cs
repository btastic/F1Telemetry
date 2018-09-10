using System;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace F1TelemetryUi.Views
{
    /// <summary>
    /// Interaction logic for TimingOverlayView.xaml
    /// </summary>
    public partial class TimingOverlayView : MetroWindow
    {
        public TimingOverlayView()
        {
            InitializeComponent();
            MouseDown += Window_MouseDown;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
