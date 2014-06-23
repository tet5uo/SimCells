using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace CellularSim.ViewModel
{
    internal static class BitmapHelpers
    {
        private static Random rng = new Random();

        internal static void DrawCell(WriteableBitmap ImageSource, int xPos, int yPos, bool state, int scale)
        {
            int xLeft = xPos * scale, 
                xRight = (xPos * scale) + scale, 
                yTop = (yPos * scale), 
                yBottom = (yPos * scale) + scale;

            var cellColor = state ? Colors.Orange : Colors.Black;
            using (ImageSource.GetBitmapContext())
            {
                ImageSource.FillRectangle(xLeft, yTop, xRight, yBottom, cellColor);
            }
        }

        internal static void ClearImage(WriteableBitmap ImageSource)
        {
            using (ImageSource.GetBitmapContext())
            {
                ImageSource.Clear(Colors.DarkBlue);
            }
        }

        internal static void DrawGridLines(WriteableBitmap bmp, int gridLength, int Scale)
        {
            var lineColor = Color.FromArgb(200, 0, 0, 0);
            using (bmp.GetBitmapContext())
            {
                for (int i = 0; i < 640; i = i + Scale)
                {
                    bmp.DrawLine(0, i, 640, i, lineColor);
                    bmp.DrawLine(i, 0, i, 640, lineColor);
                }
            }
        }
    }
}
