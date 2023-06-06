using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphIN2.ViewModels
{
    public partial class GraphSettingsVM : ObservableObject, INotifyPropertyChanged
    {
        public GraphVM ParentObject { get; set; }
        private ObservableCollection<string> _items;
        private ObservableCollection<string> _selectedItems;

        public GraphSettingsVM(GraphVM parent) {
            ParentObject = parent;
            parent.PropertyChanged += SeriesPropertyChanged;

            Items = new ObservableCollection<string>();
            foreach (var item in parent.SeriesList)
            {
                Items.Add(item.Key);
            }

            SelectedItems = new ObservableCollection<string>();
        }



        public ObservableCollection<string> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }
        public ObservableCollection<string> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                _selectedItems = value;
                OnPropertyChanged(nameof(SelectedItems));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected new virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SeriesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ParentObject.SeriesList))
            {
                Items = new ObservableCollection<string>();
                foreach (var item in ParentObject.SeriesList)
                {
                    Items.Add(item.Key);
                }
            }
        }

    }
}
