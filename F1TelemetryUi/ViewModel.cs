using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts;

namespace F1TelemetryUi
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TimeSpan _currentLapTime;
        public TimeSpan CurrentLapTime
        {
            get { return _currentLapTime; }
            set
            {
                _currentLapTime = value;
                OnPropertyChanged();
            }
        }

        public SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set
            {
                _seriesCollection = value;
                OnPropertyChanged();
            }
        }

        public int _sector;
        public int Sector
        {
            get { return _sector; }
            set
            {
                _sector = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan[] _sectorTimes = new TimeSpan[3];
        public TimeSpan[] SectorTimes
        {
            get { return _sectorTimes; }
            set
            {
                _sectorTimes = value;
                OnPropertyChanged();
            }
        }

        public Func<double, string> Formatter { get; set; }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
