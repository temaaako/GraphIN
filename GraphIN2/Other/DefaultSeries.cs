using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphIN2.Other
{
    internal class DefaultSeries<TModel> : LineSeries<TModel, CircleGeometry, LabelGeometry>
    {
        public DefaultSeries() : base(){
            Fill = null;
            GeometrySize = 0;
            LineSmoothness = 0;
        }
    }
}
