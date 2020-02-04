using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GameLogic;
using QLearning;
using System.Windows.Threading;

namespace QLearningGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Coordinate initialPlayerCoordinates;
        private readonly Coordinate initialGoalCoordinates;
        private readonly Agent agent;

        /// <summary>
        /// Sets the initial state of the game and configures the timer
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Left = 0;
            Top = 0;
            Coordinate player = new Coordinate(Grid.GetColumn(playerBorder), Grid.GetRow(playerBorder));
            Coordinate goal = new Coordinate(Grid.GetColumn(goalBorder), Grid.GetRow(goalBorder));
            initialPlayerCoordinates = player;
            initialGoalCoordinates = goal;            
            Game.Instance.InitializeField(player, goal, gameGrid.ColumnDefinitions.Count, gameGrid.RowDefinitions.Count - 1);            
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Start();
            agent = new Agent(initialPlayerCoordinates.X, initialPlayerCoordinates.Y, initialGoalCoordinates.X, initialGoalCoordinates.Y, gameGrid.ColumnDefinitions.Count, gameGrid.RowDefinitions.Count - 1);
        }

        /// <summary>
        /// Takes the players input and hands it to the game logic for evaluation
        /// Resets the game if it is finished and the player inputs "Enter"
        /// </summary>
        /// <param name="sender">Event Handler Parameter</param>
        /// <param name="e">Event Handler Parameter</param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (agent.IsRunning==false)
            {
                if (Game.Instance.IsRunning == false)
                {
                    if (e.Key == Key.Enter)
                    {
                        ResetGameState();
                        return;
                    }
                }                
                Coordinate result = null;
                result = Game.Instance.ProcessInput(e.Key.ToString());
                Move(result);
            }                       
        }
        
        /// <summary>
        /// Updates the timer label
        /// </summary>
        /// <param name="sender">Event Handler Parameter</param>
        /// <param name="e">Event Handler Parameter</param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            timerLabel.Content = Game.Instance.Timer.Elapsed.ToString().Substring(3,8);
        }

        /// <summary>
        /// Starts the QLearning Agent and evaluates the result
        /// </summary>
        /// <param name="sender">Event Handler Parameter</param>
        /// <param name="e">Event Handler Parameter</param>
        private async void StartMachineLearningButton_Click(object sender, RoutedEventArgs e)
        {            
            if (agent.IsRunning == false)
            {
                if (!(InputManager.Current.MostRecentInputDevice is MouseDevice))
                {
                    ResetGameState();
                    return;
                }
                Task<(IEnumerable<int> path, IEnumerable<int> dataToVisualize)> t = Task.Run(() => agent.Start());
                ResetGameState();
                int currentState = 0;
                for (int i = 0; i < initialPlayerCoordinates.Y; i++)
                {
                    currentState += gameGrid.ColumnDefinitions.Count;
                }
                currentState += initialPlayerCoordinates.X;
                var result = await t;
                IEnumerable<int> path = result.path;
                await Task.Delay(500);
                string input = String.Empty;
                foreach (int i in path)
                {
                    if (currentState - 1 == i)
                    {
                        input = "A";
                        currentState -= 1;
                    }
                    if (currentState + 1 == i)
                    {
                        input = "D";
                        currentState += 1;
                    }
                    if (currentState - gameGrid.ColumnDefinitions.Count == i)
                    {
                        input = "W";
                        currentState -= gameGrid.ColumnDefinitions.Count;
                    }
                    if (currentState + gameGrid.ColumnDefinitions.Count == i)
                    {
                        input = "S";
                        currentState += gameGrid.ColumnDefinitions.Count;
                    }
                    await SimulateInput(input);
                }
                await SimulateInput("Enter");       
                ShowDataVisualization(result.dataToVisualize);
            }            
        }

        /// <summary>
        /// Moves the player to the specified coordinates
        /// </summary>
        /// <param name="coord">Coordinates the player is moved to</param>
        private void Move(Coordinate coord)
        {
            if (coord!=null)
            {
                Grid.SetColumn(playerBorder, coord.X);
                Grid.SetRow(playerBorder, coord.Y);
            }            
        }

        /// <summary>
        /// Resets the games logic and UI
        /// </summary>
        private void ResetGameState()
        {
            Game.Instance.ResetLogic();
            Game.Instance.InitializeField(initialPlayerCoordinates, initialGoalCoordinates, gameGrid.ColumnDefinitions.Count, gameGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(playerBorder, initialPlayerCoordinates.X);
            Grid.SetRow(playerBorder, initialPlayerCoordinates.Y);
            Grid.SetColumn(goalBorder, initialGoalCoordinates.X);
            Grid.SetRow(goalBorder, initialGoalCoordinates.Y);
        }

        /// <summary>
        /// Simulates inputs with a delay
        /// </summary>
        /// <param name="key">Input to simulate</param>
        /// <returns>Task to await</returns>
        private Task SimulateInput(string key) {            
            if (Game.Instance.IsRunning == false)
            {
                if (key == "Enter")
                {
                    ResetGameState();
                    return Task.Delay(500);
                }
            }
            Coordinate result = null;
            result = Game.Instance.ProcessInput(key);
            Move(result);
            return Task.Delay(500);
        }

        /// <summary>
        /// Shows the visual representation of the data
        /// </summary>
        /// <param name="data">Data to be visualized</param>
        private async void ShowDataVisualization(IEnumerable<int> data)
        {
            GeneralDataWindow generalDataWindow = new GeneralDataWindow(data);
            DataChartWindow dataChartWindow = new DataChartWindow(data);
            generalDataWindow.Left = Left + Width;
            generalDataWindow.Top = Top;
            dataChartWindow.Left = Left;
            dataChartWindow.Top = Top + Height;
            generalDataWindow.Show();
            dataChartWindow.Show();
            await Task.Delay(30000);
            generalDataWindow.Close();
            dataChartWindow.Close();
        }
    }
}
