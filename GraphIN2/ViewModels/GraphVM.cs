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
using System.Windows.Media.Converters;
using GraphIN2.Windows;
using System.Windows;
using System.Timers;
using Timer = System.Timers.Timer;

namespace GraphIN2.ViewModels
{
    public partial class GraphVM : ObservableObject, INotifyPropertyChanged
    {
        private readonly System.Random _random = new();
        public event PropertyChangedEventHandler PropertyChanged;

        private double? xAxisMinLimit = null;
        private double? xAxisMaxLimit = null;

        private double _lastBiggestX = 0;
        public ObservableCollection<ISeries> Series { get; set; }

        public ObservableCollection<string> ZoomModeNames { get; } = new ObservableCollection<string> { "X", "Y", "Both" };

        private double _xAxisSize;
        public string XAxisSize
        {
            get => _xAxisSize.ToString();
            set
            {
                double potentialValue;
                bool isNumeric = double.TryParse(value, out potentialValue);

                if (isNumeric && potentialValue != _xAxisSize && potentialValue > 1)
                {
                    _xAxisSize = potentialValue;
                    OnPropertyChanged(nameof(XAxisSize));
                }
            }
        }

        public bool IsActive { get; set; } = true;
        public bool IsFixed { get; set; } = true;

        public Axis[] XAxes { get; }

        public Axis[] YAxes { get; }




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




        public GraphVM()
        {

            XAxisSize = "10";
            SelectedZoomMode = "Both";


            EventManager.Instance.DataRecieved += OnDataRecieved;



            XAxes = new[] { new Axis() };
            YAxes = new[] { new Axis() };




            Series = new ObservableCollection<ISeries>();
            AddNewSeries(new DefaultSeries<ObservablePoint>());
            AddNewSeries(new DefaultSeries<ObservablePoint>());
            AddNewSeries(new DefaultSeries<ObservablePoint>());
            AddNewSeries(new DefaultSeries<ObservablePoint>());
            AddNewSeries(new DefaultSeries<ObservablePoint>());
        }


        private void AddNewSeries(DefaultSeries<ObservablePoint> newSeries)
        {

            string name = $"Series {Series.Count + 1}";
            newSeries.Name= name;
            newSeries.Values = new ObservableCollection<ObservablePoint>();
            Series.Add(newSeries);

            OnPropertyChanged(nameof(Series));
        }

        private void RemoveSeries(ISeries seriesToRemove)
        {
            Series.Remove(seriesToRemove);
            OnPropertyChanged(nameof(Series));

        }

    
        private void RemoveSeries(string nameToRemove)
        {
            foreach (var item in Series)
            {
                if (item.Name == nameToRemove)
                {
                    Series.Remove(item);
                    OnPropertyChanged(nameof(Series));
                    break;
                }
            }
        }


        [RelayCommand]
        public void AddItem()
        {
            var randomValue = _random.Next(1, 10);
            var xValue = _lastBiggestX + 1;


            AddItem(_random.Next(0, 5), new(xValue, randomValue));


        }

        private object collectionLock = new object();


        private Timer timer;
        //private object collectionLock = new object();
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Stop the timer after it elapsed and allow adding a new point
            if (timer == null) return;
            timer.Stop();
            timer.Dispose();
            timer = null;
        }

        public void AddItem(int index, ObservablePoint itemToAdd)
        {

            //while (Series.Count <= index) 
            //{
            //    AddNewSeries(new DefaultSeries<ObservablePoint>());
            //}


            if (timer == null || timer.Enabled == false)
            {
                timer = new Timer(1000); // 1000 milliseconds = 1 second
                timer.Elapsed += TimerElapsed;
                timer.Start();
            }
            else
            {
                // Ignore the new point if the timer is still running
                return;
            }

            Task.Run(() =>
            {

                Debug.WriteLine(itemToAdd.Y);
                Debug.WriteLine(index);
                Debug.WriteLine(Series.Count);
                Debug.WriteLine(((ObservableCollection<ObservablePoint>)Series[index].Values).Count);
                if (itemToAdd.Y<100 && itemToAdd.X>=_lastBiggestX-2)
                {
                    lock (collectionLock)
                    {
                        ((ObservableCollection<ObservablePoint>)Series[index].Values).Add(itemToAdd);
                    }
                    

                    _lastBiggestX = (double)itemToAdd.X;
                }
                

                if (xAxisMaxLimit != null && xAxisMinLimit != null)
                {
                    SetLast();
                }
                ShowActualPoints();
            });
        }





        private double _lastFixedX = 0;

        [RelayCommand]
        public void SetLast()
        {
            if (IsFixed)
            {
                while (_lastBiggestX > _lastFixedX + _xAxisSize)
                {
                    _lastFixedX += _xAxisSize;
                }
                Debug.WriteLine($"От {_lastFixedX} до {_lastFixedX + _xAxisSize}");
                SetXAxisLimits(_lastFixedX, _lastFixedX + _xAxisSize);
            }
            else
            {
                SetXAxisLimits(_lastBiggestX - _xAxisSize, _lastBiggestX);
            }
            ShowActualPoints();
        }

        [RelayCommand]
        public void SetFull()
        {
            SetXAxisLimits(null, null);

            ShowActualPoints();
        }

        public void ShowActualPoints()
        {
            var xAxis = XAxes[0];
            xAxis.MaxLimit = xAxisMaxLimit;
            xAxis.MinLimit = xAxisMinLimit;


            var yAxis = YAxes[0];
            if (yAxis.MinLimit != null)
            {
                yAxis.MinLimit = null;
            }
            if (yAxis.MaxLimit != null)
            {
                yAxis.MaxLimit = null;

            }
        }

        public void SetXAxisLimits(double? minLim, double? maxLim)
        {
            xAxisMaxLimit = maxLim;
            xAxisMinLimit = minLim;
        }




        [RelayCommand]
        public void AddSeries()
        {
            //  for this sample only 5 series are supported.
            if (Series.Count == 5) return;
            DefaultSeries<ObservablePoint> newSeries = new DefaultSeries<ObservablePoint>();
            var obsCollection = new ObservableCollection<ObservablePoint>();
            newSeries.Values = obsCollection;
            newSeries.Name = $"Series {Series.Count + 1}";

            for (int i = 0; i < 5; i++)
            {
                var randomValue = _random.Next(1, 10);
                var xValue = i;
                obsCollection.Add(new(xValue, randomValue));
            }

            AddNewSeries(newSeries);
        }
        [RelayCommand]
        public void RemoveSeries()
        {
            if (Series.Count == 1) return;

            RemoveSeries(Series.Last());
        }



        [RelayCommand]
        public void Clear()
        {
            if (Series == null) return;
            foreach (var collection in Series)
            {
                collection.Values = new ObservableCollection<ObservablePoint>();
            }
        }


        GraphSettingsWindow settingsWindow;

        [RelayCommand]
        public void OpenSettings()
        {
            if (settingsWindow != null && settingsWindow.IsVisible) return;
            settingsWindow = new GraphSettingsWindow
            {

                DataContext = new GraphSettingsVM(this)
            };

            settingsWindow.Show();
        }




        private void OnDataRecieved(int index, ObservablePoint data)
        {
            if (IsActive)
            {
                AddItem(index, data);
            }
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
