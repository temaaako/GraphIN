﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Defaults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GraphIN2.ViewModels
{
    public enum DataFormat
    {
        DOUBLE, INT8, UINT8, INT16,UINT16, INT32, UINT32, INT64, UINT64
    }
    public partial class MainViewModel : ObservableObject, INotifyPropertyChanged
    {
        private TimeSpan _elapsedTime;
        private DispatcherTimer _timer;
        private float _stepTime = 0.1f;

        SerialPort _serialPort;
        public string _comPortName = "COM11";
        public Parity _portParity = Parity.None;
        public int _dataBits = 8;
        public StopBits _stopBits = StopBits.One;
        public List<string> ComPortsNames
        {
            get
            {
                Debug.WriteLine("ComPortsName invoke");
                return SerialPort.GetPortNames().ToList();
            }
        }


        private int _baudRate = 9600;
        public string BaudRate
        {
            get { return _baudRate.ToString(); }
            set
            {
                int potentialValue;
                bool isNumeric = int.TryParse(value, out potentialValue);

                if (potentialValue != _baudRate && isNumeric  && potentialValue > 0)
                {
                    if (IsRunning)
                    {
                        Stop();
                    }
                    _baudRate = potentialValue;
                    OnPropertyChanged(nameof(BaudRate));
                }
            }
        }
        public List<DataFormat> DataFormats { get; }

        private DataFormat _selectedDataFormat = DataFormat.DOUBLE;
        public DataFormat SelectedDataFormat
        {
            get { return _selectedDataFormat; }
            set
            {
                _selectedDataFormat = value;
                OnPropertyChanged(nameof(SelectedDataFormat));
            }
        }

        private int _savedSeconds=100;

        public int SavedSeconds
        {
            get { return _savedSeconds; }
            set
            {
                _savedSeconds = value;
                OnPropertyChanged(nameof(SavedSeconds));
            }
        }

        private string _selectedComPortName;
        public string SelectedComPortName
        {
            get { return _selectedComPortName; }
            set
            {
                if (_selectedComPortName != value && value != null && value != "No ports")
                {
                    if (IsRunning)
                    {
                        Stop();
                    }
                    _selectedComPortName = value;
                    _serialPort = new SerialPort(_selectedComPortName, _baudRate, _portParity, _dataBits, _stopBits);
                    _serialPort.DataReceived += OnSerialPortDataReceived;
                    _serialPort.Encoding = Encoding.UTF8;


                    OnPropertyChanged(nameof(SelectedComPortName));
                }
            }
        }

        public List<int> ParameterCounts { get; } = Enumerable.Range(1, 8).ToList();

        private int _selectedParameterCount = 1;

        public int SelectedParameterCount
        {
            get { return _selectedParameterCount; }
            set
            {
                _selectedParameterCount = value;
                OnPropertyChanged(nameof(SelectedParameterCount));
            }
        }

        private bool _hasHeader;
        public bool HasHeader
        {
            get { return _hasHeader; }
            set
            {
                _hasHeader = value;
                OnPropertyChanged(nameof(HasHeader));
            }
        }

        public bool IsRunning
        {
            get
            {
                if (_serialPort == null)
                {
                    return false;
                }
                return _serialPort.IsOpen;
            }
            private set
            {
                if (_serialPort == null)
                {
                    return;
                }
                if (value)
                {
                    try
                    {
                        Task.Run(() =>
                        {
                            _serialPort.Open();
                            StartTimer();

                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    _serialPort.Close();
                    StopTimer();
                }
            }
        }




        public MainViewModel()
        {
            SelectedComPortName = ComPortsNames.FirstOrDefault("No ports");
            DataFormats = Enum.GetValues(typeof(DataFormat)).Cast<DataFormat>().ToList();
        }



        [RelayCommand]
        public void Run()
        {
            if (_serialPort != null)
            {
                IsRunning = true;
            }
        }

        [RelayCommand]
        public void Stop()
        {
            if (_serialPort != null)
            {
                IsRunning = false;
            }
        }

        private void OnSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //if (IsRunning == false) return;

            Task.Run(() =>
            {
                try
                {
                    byte[] buffer = new byte[_serialPort.BytesToRead];
                    _serialPort.Read(buffer, 0, buffer.Length);
                    if (buffer.Length == 8)
                    {
                        double decimalNumber = BitConverter.ToDouble(buffer, 0);
                        double numToAdd = decimalNumber;
                        Debug.WriteLine(numToAdd + " E");
                        EventManager.Instance.SendDataRecieved(new ObservablePoint(_elapsedTime.TotalSeconds, numToAdd));
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                




            });
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            _elapsedTime += TimeSpan.FromSeconds(_stepTime);
        }


        private CancellationTokenSource _cancellationTokenSource;
        private Task _timerTask;

        public void StartTimer()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _timerTask = Task.Run(() => TimerLoop(_cancellationTokenSource.Token));
        }

        public void StopTimer()
        {
            _cancellationTokenSource.Cancel();
        }

        private void TimerLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _elapsedTime += TimeSpan.FromSeconds(_stepTime);
                Thread.Sleep((int)(_stepTime * 1000f)); 
            }
        }
    }
}


