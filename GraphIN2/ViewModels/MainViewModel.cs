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
    public partial class MainViewModel : ObservableObject, INotifyPropertyChanged
    {
        private TimeSpan _elapsedTime;
        private DispatcherTimer _timer;
        private float _stepTime = 0.1f;

        SerialPort _serialPort;
        public string _comPortName = "COM11";
        public int _baudRate = 9600;
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
                byte[] buffer = new byte[_serialPort.BytesToRead];
                _serialPort.Read(buffer, 0, buffer.Length);

                StringBuilder binaryString = new StringBuilder();
                foreach (byte b in buffer)
                {
                    binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                }

                for (int i = 0; i < binaryString.Length; i += 8)
                {
                    string binaryNumber = binaryString.ToString().Substring(i, 8);
                    int decimalNumber = Convert.ToInt32(binaryNumber, 2);
                    double numToAdd = decimalNumber;
                    Debug.WriteLine(_elapsedTime.TotalSeconds);

                    EventManager.Instance.SendDataRecieved(new ObservablePoint(_elapsedTime.TotalSeconds, numToAdd));
                }
            });
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            _elapsedTime += TimeSpan.FromSeconds(_stepTime);
            Debug.WriteLine(_elapsedTime);
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
                Debug.WriteLine($"Elapsed time: {_elapsedTime.TotalSeconds}");
                // Do something with elapsed time...
                Thread.Sleep((int)(_stepTime * 1000f)); // Sleep for interval in seconds
            }
        }
    }
}


