using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace F1TelemetryUi.ViewModels
{
    public class TimingOverlayViewModel
    {
        private IWindowManager _windowManager;
        private IEventAggregator _eventAggregator;

        public TimingOverlayViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
        }
    }
}
