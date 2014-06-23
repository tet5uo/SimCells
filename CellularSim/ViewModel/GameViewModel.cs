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

        public int Scale { get; private set; }
        public int SimWidth { get { return (int)SimSize; } }
        private GameModel model;
        private DispatcherTimer timer = new DispatcherTimer();

        private WrappingGrid<bool> lastGenModelState;

        public WriteableBitmap ImageSource { get; private set; }

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
        private RelayCommand newGameCommand;
        public RelayCommand NewGameCommand
        {
            get
            {
                return newGameCommand ??
                    (newGameCommand = new RelayCommand(this.NewGame, (arg) => !timer.IsEnabled ? true : false));
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
            }
            
        }

        public GameViewModel()
        {
            SimSize = GameSize.VeryLarge;
            ImageSource = BitmapFactory.New(640, 640);
            using (ImageSource.GetBitmapContext())
            {
                ImageSource.Clear(Colors.DarkBlue);
            }
            RaisePropertyChanged("ImageSource");
            PercentRandomCells = 15;
            model = new GameModel(rng);
            model.PropertyChanged += model_PropertyChanged;
            timer.Interval = TimeSpan.FromMilliseconds(64);
            timer.Tick += timer_Tick;
        }

        private void NewGame(Object obj)
        {
            model.NewGame(SimSize);
            lastGenModelState = model.GameArea;
        }

        private void EnableRandomCells(Object obj)
        {
            model.ActivateRandomCells(PercentRandomCells);
            BitmapHelpers.ClearImage(ImageSource);
            lastGenModelState = null;
            RedrawDrawCells();
        }

        private void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RedrawDrawCells();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void RaisePropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));

        }

        public void SetScale()
        {
            Scale = 640 / ((int)SimSize);
        }

        private void RunTimer(Object obj) 
        {
            timer.Start();
            timerRunning = true;
            RaisePropertyChanged("HitTestEnabled");
        }

        void timer_Tick(object sender, object e)
        {
            model.UpdateCells(GameRules.ConwayRules);
            lastGenModelState = model.GameArea;
        }

        private void RedrawDrawCells()
        {
            for (int x = 0; x < model.GameArea.Width; x++)
            {
                for (int y = 0; y < model.GameArea.Height; y++)
                {
                    if (lastGenModelState == null || lastGenModelState[x, y] != model.GameArea[x, y])
                    {
                        BitmapHelpers.DrawCell(ImageSource, x, y, model.GameArea[x, y], Scale);
                    }
                }
            }
            BitmapHelpers.DrawGridLines(ImageSource, model.GameArea.Width, Scale);
            RaisePropertyChanged("ImageSource");
        }
    }
}
