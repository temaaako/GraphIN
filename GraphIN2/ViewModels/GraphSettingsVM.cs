using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphIN2.Other;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GraphIN2.ViewModels
{
    public partial class GraphSettingsVM : ObservableObject, INotifyPropertyChanged
    {
        public GraphVM ParentObject { get; set; }


        public ObservableCollection<ISeries> Items { get; set; }
        public GraphSettingsVM(GraphVM parent) {
            ParentObject = parent;
            Items = parent.Series;
        }


       
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
