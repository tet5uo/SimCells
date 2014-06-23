using System.Collections.Generic;
using System.Linq;

namespace CellularSim.Model
{
    public static class GameRules
    {
        public static bool ConwayRules(WrappingGrid<bool> state, int x, int y)
        {
            var neighbors = CountLivingNeighbors(state, x, y);
            if (state[x, y])
            {
                if (neighbors < 2 || neighbors > 3)
                {
                    return false;
                }
                else if (neighbors == 2 || neighbors == 3)
                {
                    return true;
                }
            }
            else
            {
                if (neighbors == 3) { return true; }
            }
            return false;
        }

        private static int CountLivingNeighbors(WrappingGrid<bool> state, int x, int y)
        {
            return new List<bool>() 
            { 
                (state[x - 1, y]),
                (state[x + 1, y]),
                (state[x, y - 1]),
                (state[x, y + 1]),
                (state[x - 1, y - 1]),
                (state[x + 1, y + 1]),
                (state[x + 1, y - 1]),
                (state[x - 1, y + 1]),
            }.Where(e => e).Count();
        }

    }
}
