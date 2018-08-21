using System.Windows;
using MahApps.Metro.Controls;

namespace F1TelemetryUi.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
            Left = SystemParameters.PrimaryScreenWidth - Width;
        }
    }
}
