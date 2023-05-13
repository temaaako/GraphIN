using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphIN2.ViewModels
{
    public partial class MainViewModel : ObservableObject, INotifyPropertyChanged
    {
        SerialPort serialPort;
        
        public MainViewModel()
        {
            serialPort = new SerialPort("COM11", 9600, Parity.None, 8, StopBits.One);
            serialPort.DataReceived += OnSerialPortDataReceived;
            serialPort.Encoding = Encoding.UTF8;
            if (serialPort.IsOpen == false)
            {
                Debug.WriteLine("opening");
                serialPort.Open();

            }
        }
        private void OnSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length);

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
                Debug.WriteLine(numToAdd);
                EventManager.Instance.SendDataRecieved(numToAdd);
            }
        }


    }
}
