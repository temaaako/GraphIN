using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using LiveChartsCore.Defaults;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics.Metrics;
using System.Threading;
using LiveChartsCore.Measure;
using System.IO.Ports;
using System;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using GraphIN2.Other;

namespace GraphIN2.ViewModels
{
    public partial class ViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly System.Random _random = new();
        private SerialPort _serialPort;
        private readonly ObservableCollection<ObservablePoint> _ObservablePoints;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> ZoomModeNames { get;} = new ObservableCollection<string>{"X","Y","Both"}; 


        public Axis[] XAxes { get; }

        public Axis[] YAxes { get; }
        private string _debugText;
        public string DebugText
        {
            get { return _debugText; }
            set
            {
                _debugText = value;
                OnPropertyChanged(nameof(DebugText));
            }
        }

        


        private string _selectedZoomMode;

        public string SelectedZoomMode
        {
            get => _selectedZoomMode;
            set
            {
                _selectedZoomMode = value;
                switch (_selectedZoomMode)
                {
                    case "X":
                        ZoomMode = ZoomAndPanMode.X;
                        break;
                    case "Y":
                        ZoomMode = ZoomAndPanMode.Y;
                        break;
                    case "Both":
                        ZoomMode = ZoomAndPanMode.Both;
                        break;
                    case "None":
                        ZoomMode = ZoomAndPanMode.None; 
                        break;
                    default:
                        break;
                }
            }
        } 
        private ZoomAndPanMode _zoomMode;
        public ZoomAndPanMode ZoomMode
        {
            get => _zoomMode;
            set
            {
                _zoomMode = value;
                OnPropertyChanged(nameof(ZoomMode));
               
            }
        }

        public ObservableCollection<ISeries> Series { get; set; }


        private Task _task1;
        private bool _isRunning = false;
        private int _rowCounter = 0;
        private string[] _time;
        private string[] _values;
        private int _step = 1;
        private int _sleepTime = 50;
        

        public ViewModel()
        {
           

            DebugText = "123";

            SelectedZoomMode = "Both";

            //_serialPort = new SerialPort("COM11", 9600, Parity.None, 8, StopBits.One);
            //_serialPort.DataReceived += SerialPort_DataReceived;
            //_serialPort.Open();

            EventManager.Instance.DataRecieved += OnDataRecieved;

            _ObservablePoints = new ObservableCollection<ObservablePoint>
        {
            new ObservablePoint(1,4.2),
            new(2,5.3),
            new(3,4.8),
            new(4,5.9),
            new(5,2),
            new(6,6),
            new(7,6),
                new(8,6),
        };
            //_time = ExcelToArrayConverter.ConvertColumnToArray("C:\\Users\\MI\\Desktop\\данные.xlsx", 1);
            //_values = ExcelToArrayConverter.ConvertColumnToArray("C:\\Users\\MI\\Desktop\\данные.xlsx", 18);

            XAxes = new[] { new Axis() };
            YAxes = new[] { new Axis() };

            Series = new ObservableCollection<ISeries>
        {
            new DefaultSeries<ObservablePoint>
            {
                Values = _ObservablePoints,
            }
        };

        }

        


        [RelayCommand]
        public void AddItem()
        {
            var randomValue = _random.Next(1, 10);
            _ObservablePoints.Add(new(_ObservablePoints.Count + 1, randomValue));
        }

        public void AddItem(double itemToAdd)
        {
            _ObservablePoints.Add(new(_ObservablePoints.Count + 1, itemToAdd));
        }

        [RelayCommand]
        public void RemoveItem()
        {
            if (_ObservablePoints.Count == 0) return;
            _ObservablePoints.RemoveAt(0);
        }

        [RelayCommand]
        public void UpdateItem()
        {
            var randomValue = _random.Next(1, 10);

            // we grab the last instance in our collection
            var lastInstance = _ObservablePoints[_ObservablePoints.Count - 1];

            // finally modify the value property and the chart is updated!
            lastInstance.Y = randomValue;
        }

        [RelayCommand]
        public void ReplaceItem()
        {
            var randomValue = _random.Next(1, 10);
            var randomIndex = _random.Next(0, _ObservablePoints.Count - 1);
            _ObservablePoints[randomIndex] = new(_ObservablePoints.Count + 1, randomValue);
        }

        [RelayCommand]
        public void AddSeries()
        {
            //  for this sample only 5 series are supported.
            if (Series.Count == 5) return;
            LineSeries<double> newSeries = new LineSeries<double>();
            newSeries.Values = new List<double>
                    {
                    _random.Next(0, 10),
                    _random.Next(0, 10),
                    _random.Next(0, 10)
                    };
            newSeries.Name = "new";
            Series.Add(newSeries);
        }

        [RelayCommand]
        public void RemoveSeries()
        {
            if (Series.Count == 1) return;

            Series.RemoveAt(Series.Count - 1);
        }

        [RelayCommand]
        public void SeeAll()
        {
            var xAxis = XAxes[0];
            xAxis.MinLimit = null;
            xAxis.MaxLimit = null;

            var yAxis = YAxes[0];
            yAxis.MinLimit = null;
            yAxis.MaxLimit = null;
        }

        [RelayCommand]
        public void Clear()
        {
            _ObservablePoints.Clear();

        }
        [RelayCommand]
        public void Stop()
        {

        }

        private void OnDataRecieved(double data)
        {
            AddItem(data);
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine("efw");
        }

    }
}
