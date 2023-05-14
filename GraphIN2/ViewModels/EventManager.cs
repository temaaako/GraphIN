using LiveChartsCore.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphIN2.ViewModels
{
    public class EventManager
    {
        private static EventManager _instance;

        private EventManager()
        {
        }

        public static EventManager Instance => _instance ??= new EventManager();

        public event Action<ObservablePoint> DataRecieved;

        public void SendDataRecieved(ObservablePoint data)
        {
            DataRecieved?.Invoke(data);
        }
    }
}
