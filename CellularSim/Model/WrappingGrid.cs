
namespace CellularSim.Model
{
    public class WrappingGrid<T> 
    {
        private T[,] gridState;

        public int Width { get { return gridState.GetLength(0); } }
        public int Height { get { return gridState.GetLength(1); } }

        public WrappingGrid(int sizeX, int sizeY)
        {
            this.gridState = new T[sizeX, sizeY];
        }
        public T this[int x, int y]
        {
            get 
            {
                return gridState[WrapX(x), WrapY(y)];
            }
            set 
            { 
                gridState[WrapX(x), WrapY(y)] = value;
            }
        }

        private int WrapY(int y)
        {
            return mod(y, Height);
        }

        private int WrapX(int x)
        {
            return mod(x, Width);
        }

        private int mod(int index, int maxSize)
        {
            while (index < 0) index += maxSize;
            return index % maxSize;
        }

    }
}
