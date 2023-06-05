﻿using LiveChartsCore.SkiaSharpView;
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

namespace GraphIN2.ViewModels
{
    public partial class ViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly System.Random _random = new();
        private readonly ObservableCollection<ObservablePoint> _ObservablePoints;
        public event PropertyChangedEventHandler PropertyChanged;

        private double? xAxisMinLimit = null;
        private double? xAxisMaxLimit = null;

        

        public ObservableCollection<string> ZoomModeNames { get;} = new ObservableCollection<string>{"X","Y","Both"};

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


        public bool IsFixed { get; set; }

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


       
        

        public ViewModel()
        {

            DebugText = "123";
            XAxisSize = "5";
            SelectedZoomMode = "Both";

            //_serialPort = new SerialPort("COM11", 9600, Parity.None, 8, StopBits.One);
            //_serialPort.DataReceived += SerialPort_DataReceived;
            //_serialPort.Open();

            EventManager.Instance.DataRecieved += OnDataRecieved;

            _ObservablePoints = new ObservableCollection<ObservablePoint> { new ObservablePoint(0,0)        };
            

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
            var xValue = _ObservablePoints.Count + 1;
            AddItem(new(xValue, randomValue));

            
        }

        public void AddItem(ObservablePoint itemToAdd)
        {
            Task.Run(() =>
            {
                _ObservablePoints.Add(itemToAdd);
                if (xAxisMaxLimit != null && xAxisMinLimit != null)
                {
                    SetLast();
                }
                ShowActualPoints();
            });
        }

        [RelayCommand]
        public void RemoveItem()
        {
            if (_ObservablePoints.Count == 0) return;
            _ObservablePoints.RemoveAt(0);
        }



        private double _lastFixedX = 0;

        [RelayCommand]
        public void SetLast()
        {
            if (IsFixed)
            {
                while (_ObservablePoints.Last().X>_lastFixedX+ _xAxisSize)
                {
                    _lastFixedX += _xAxisSize;
                }
                Debug.WriteLine($"От {_lastFixedX} до {_lastFixedX + _xAxisSize}");
                SetXAxisLimits(_lastFixedX, _lastFixedX + _xAxisSize);
            }
            else
            {
                SetXAxisLimits(_ObservablePoints.Last().X - _xAxisSize, _ObservablePoints.Last().X);
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
            yAxis.MinLimit = null;
            yAxis.MaxLimit = null;
        }

        public void SetXAxisLimits(double? minLim, double? maxLim)
        {
            xAxisMaxLimit = maxLim;
            xAxisMinLimit = minLim;
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
            DefaultSeries<double> newSeries = new DefaultSeries<double>();
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
        public void Clear()
        {
            _ObservablePoints.Clear();
        }

        

        private void OnDataRecieved(ObservablePoint data)
        {
            AddItem(data);
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
