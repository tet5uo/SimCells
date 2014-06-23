using System;
using System.Collections.Generic;
using System.Windows;

namespace CellularSim.Model
{
    public static class GridExtension
    {
        public static WrappingGrid<bool> ProcessCells(this WrappingGrid<bool> current, GameModel.CellProcessor proc)
        {
            var newGrid = new WrappingGrid<bool>(current.Width, current.Height);
            for (int x = 0; x < current.Width; x++)
            {
                for (int y = 0; y < current.Height; y++)
                {
                    newGrid[x, y] = proc(current, x, y);
                }
            }
            return newGrid;
        }

        public static List<Cell> ToCellList(this WrappingGrid<bool> grid)
        {
            var list = new List<Cell>();
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var cell = new Cell(new Point((double)x, (double)y));
                    cell.State = grid[x, y];
                    list.Add(cell);
                }
            }
            return list;
        }

        public static WrappingGrid<bool> ToWrappingGrid(this List<Cell> list)
        {
            int size = (int)Math.Sqrt(list.Count);
            var grid = new WrappingGrid<bool>(size, size);
            foreach (var cell in list)
            {
                grid[(int)cell.Location.X, (int)cell.Location.Y] = cell.State;
            }
            return grid;
        }

        public static void FlipCell(this WrappingGrid<bool> state, int x, int y)
        {
            state[x, y] = !state[x, y];
        }
        
    }
}
