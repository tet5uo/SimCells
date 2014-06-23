using CellularSim.Model;
using CellularSim.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace CellularSim.ViewModel
{
    class GameViewModel : INotifyPropertyChanged
    {
       
        private bool timerRunning = false;
        public bool HitTestEnabled { get { return !timerRunning; } }
        private Random rng = new Random();
        public GameSize SimSize { get; set; }
        private int percentRandomCells;
        public int PercentRandomCells { get { return percentRandomCells; }
            set
            {
                percentRandomCells = value;
                RaisePropertyChanged();
            }
        }
        private Color cellColor = Colors.YellowGreen;

        public Color CellColor { get { return cellColor; } }
        public UInt64 Generation { get; private set; }
        public double Scale { get; private set; }
        public int SimWidth { get { return (int)SimSize; } }
        private GameModel model;
        private DispatcherTimer timer = new DispatcherTimer();

        private WrappingGrid<bool> lastGenModelState;

        public WriteableBitmap ImageSource { get; private set; }

        public GameViewModel()
        {
            SimSize = GameSize.Large;
            SetScale();
            ImageSource = BitmapFactory.New(640, 640);
            BitmapHelpers.ClearImage(ImageSource);
            BitmapHelpers.DrawGridLines(ImageSource, (int)SimSize, (int)Scale);
            PercentRandomCells = 15;
            model = new GameModel(rng);
            model.NewGame(SimSize);
            timer.Interval = TimeSpan.FromMilliseconds(40);
            timer.Tick += timer_Tick;
        }

        #region RelayCommands

        private RelayCommand changeSizeCommand;
        public RelayCommand ChangeSizeCommand
        {
            get
            {
                return changeSizeCommand ??
                    (changeSizeCommand = new RelayCommand(this.ChangeGameSize, (arg) => !timer.IsEnabled));
            }
        }

        
        private RelayCommand startSimulationCommand;
        public RelayCommand StartSimulationCommand
        {
            get
            {
                return startSimulationCommand ?? 
                    (startSimulationCommand = new RelayCommand(this.RunTimer, (arg) =>
                {
                    if (timer.IsEnabled) { return false; }
                    return true;
                }));
            }
        }
        private RelayCommand randomizeCellsCommand;
        public RelayCommand RandomizeCellsCommand
        {
            get
            {
                return randomizeCellsCommand ??
                    (randomizeCellsCommand = new RelayCommand(this.EnableRandomCells, (arg) => 
                {
                    if (timer.IsEnabled)
                    {
                        return false;
                    }
                    return true;
                }));
            }
        }
        private RelayCommand clearCommand;
        public RelayCommand ClearCommand
        {
            get
            {
                return clearCommand ??
                    (clearCommand = new RelayCommand(this.NewGame, (arg) => !timer.IsEnabled ? true : false));
            }
        }

        private RelayCommand stopSimCommand;
        public RelayCommand StopSimCommand
        {
            get
            {
                return stopSimCommand ??
                    (stopSimCommand = new RelayCommand(this.StopTimer, (arg) => timer.IsEnabled ? true : false));
            }
        }
        #endregion

        private void StopTimer(object obj)
        {
            timer.Stop();
            timerRunning = false;
            RaisePropertyChanged("HitTestEnabled");
        }

        private void ChangeGameSize(object obj)
        {
            string param = obj as String;
            if (param != null)
            {
                switch (param.ToLower())
                {
                    case "small":
                        SimSize = GameSize.Small;
                        break;
                    case "medium":
                        SimSize = GameSize.Medium;
                        break;
                    case "large":
                        SimSize = GameSize.Large;
                        break;
                    case "verylarge":
                        SimSize = GameSize.VeryLarge;
                        break;
                    default:
                        break;
                }
                BitmapHelpers.ClearImage(ImageSource);
                SetScale();
                model.NewGame(SimSize);
                Generation = 0;
                RaisePropertyChanged("Generation");
                lastGenModelState = null;
                RedrawDrawCells();
            }
            
        }


        private void NewGame(Object obj)
        {
            Generation = 0;
            RaisePropertyChanged("Generation");
            lastGenModelState = model.GameArea;
            model.NewGame(SimSize);
            RedrawDrawCells();
        }

        private void EnableRandomCells(Object obj)
        {
            model.ActivateRandomCells(PercentRandomCells);
            Generation = 0;
            RaisePropertyChanged("Generation");
            lastGenModelState = null;
            RedrawDrawCells();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void RaisePropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));

        }

        public void SetScale()
        {
            Scale = 640d / (double)SimSize;
        }

        private void RunTimer(Object obj) 
        {
            timer.Start();
            timerRunning = true;
            RaisePropertyChanged("HitTestEnabled");
        }

        void timer_Tick(object sender, object e)
        {
            lastGenModelState = model.GameArea;
            model.UpdateCells(GameRules.ConwayRules);
            RedrawDrawCells();
            Generation++;
            RaisePropertyChanged("Generation");
        }

        private void RedrawDrawCells([CallerMemberName] string whoCalled = "")
        {
            if (whoCalled == "OnCellClicked")
            {
                DrawAllCells();
            }
            if (lastGenModelState == null)
            {
                DrawAllCells();
            }
            else
            {
                DrawChangedCells();
            }
            BitmapHelpers.DrawGridLines(ImageSource, model.GameArea.Width, (int)Scale);
            RaisePropertyChanged("ImageSource");
        }

        private void DrawChangedCells()
        {
            for (int x = 0; x < model.GameArea.Width; x++)
            {
                for (int y = 0; y < model.GameArea.Height; y++)
                {
                    if (lastGenModelState[x, y] != model.GameArea[x, y])
                    {
                        BitmapHelpers.DrawCell(ImageSource, x, y, model.GameArea[x, y], (int)Scale, CellColor);
                    }
                }
            }
        }

        private void DrawAllCells()
        {
            for (int x = 0; x < model.GameArea.Width; x++)
            {
                for (int y = 0; y < model.GameArea.Height; y++)
                {
                    BitmapHelpers.DrawCell(ImageSource, x, y, model.GameArea[x, y], (int)Scale, CellColor);
                }
            }
        }

        internal void OnCellClicked(Point point)
        {
            double x1 = point.X,
                   y1 = point.Y;
            int x, y;
            if (x1 <= Scale)
            {
                x = 0;
            }
            else
            {
                x = (int)(Math.Floor(x1 / Scale));
            }
            if (y1 <= Scale)
            {
                y = 0;
            }
            else
            {
                y = (int)(Math.Floor(y1 / Scale));
            }
            model.GameArea.FlipCell((int)x, (int)y );
            RedrawDrawCells();
        }
    }
}
