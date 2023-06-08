using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphIN2.Other
{
    public class DefaultSeries<TModel> : LineSeries<TModel, CircleGeometry, LabelGeometry>
    {
        public DefaultSeries() : base(){
            Fill = null;
            GeometrySize = 10;
            LineSmoothness = 0;
            EasingFunction = null;
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));

                    // Trigger the event
                    OnIsSelectedChanged();
                }
            }
        }

        public event EventHandler IsSelectedChanged;

        protected virtual void OnIsSelectedChanged()
        {
            IsSelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
