using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CellularSim.View
{
    public static class ViewHelpers
    {
        public static Dictionary<Point, FrameworkElement> CreateGridOfCells(int sideLength, double scale)
        {
            var map = new Dictionary<Point, FrameworkElement>();
            for (int x = 0; x < sideLength; x++)
            {
                for (int y = 0; y < sideLength; y++)
                {
                    var cell = new CellControl() { Width = scale, Height = scale, Visibility = Visibility.Collapsed };
                    Canvas.SetLeft(cell, x * scale);
                    Canvas.SetTop(cell, y * scale);
                    map.Add(new Point(x, y), cell);
                }
            }
            return map;
        }
    }
}
