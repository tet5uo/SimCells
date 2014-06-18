using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CellularSim.Model
{
    public class GameModel : INotifyPropertyChanged
    {
        private Random rng;
        private WrappingGrid<bool> gameArea;
        
        public WrappingGrid<bool> GameArea
        {
            get { return gameArea; }
            set
            {
                gameArea = value;
                RaisePropertyChanged();
            }
        }
        
        public delegate bool CellProcessor(WrappingGrid<bool> grid, int x, int y);

        public GameModel(Random rand)
        {
            rng = rand;

        }

        public void NewGame(GameSize size)
        {
            gameArea = new WrappingGrid<bool>((int)size, (int)size);
            RaisePropertyChanged("GameArea");
        }

        public void UpdateCells(CellProcessor proc)
        {
            var newGen = gameArea.ProcessCells(proc);
            gameArea = newGen;
            RaisePropertyChanged("GameArea");
        }

        public void ActivateRandomCells(int percentActivated)
        {
            gameArea.ProcessCells((grid, x, y) => grid[x, y] = rng.Next(100) < percentActivated ? true : false );
            RaisePropertyChanged("GameArea");
        }
        public void PopulateGridFromInput(List<Cell> input)
        {
            GameArea = input.ToWrappingGrid();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void RaisePropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
