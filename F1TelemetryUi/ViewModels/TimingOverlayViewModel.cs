using Caliburn.Micro;
using F1Telemetry;
using F1Telemetry.Models.Raw.F12018;

namespace F1TelemetryUi.ViewModels
{
    public class TimingOverlayViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly F1Manager _f1Manager;
        private readonly IWindowManager _windowManager;

        public TimingOverlayViewModel(IWindowManager windowManager,
            IEventAggregator eventAggregator,
            F1Manager f1Manager)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _f1Manager = f1Manager;

            _f1Manager.CarTelemetryReceived += _f1Manager_CarTelemetryReceived;
            _f1Manager.CarStatusReceived += _f1Manager_CarStatusReceived;
        }

        private void _f1Manager_CarStatusReceived(object sender, PacketReceivedEventArgs<PacketCarStatusData> e)
        {
            
        }

        private void _f1Manager_CarTelemetryReceived(object sender, PacketReceivedEventArgs<PacketCarTelemetryData> e)
        {
            
        }
    }
}
