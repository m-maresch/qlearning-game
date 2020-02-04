using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace GameLogic
{    
    public sealed class Game
    {
        private static readonly Game instance = new Game();
        private Game() {
            IsRunning = true;
            Timer.Start();
        }
        public static Game Instance => instance;

        private FieldType[,] field;    
        private readonly Stopwatch timer = new Stopwatch();
        public Stopwatch Timer => timer;
        public bool IsRunning { get; set; }

        /// <summary>
        /// Evaluates the players input, checks if the player has won and returns the coordinates the player will be moved to
        /// </summary>
        /// <param name="key">Players input</param>
        /// <returns>Coordinates the player will be moved to</returns>
        public Coordinate ProcessInput(string key) {
            if (IsRunning == false)
            {
                return null;
            }
            Coordinate result = new Coordinate();
            MovementKey parsedKey;
            try {
                parsedKey = (MovementKey)Enum.Parse(typeof(MovementKey), key);
            }
            catch (ArgumentException) {
                return null;
            }
            Coordinate currentPlayerCoords = GetPlayerCoordinates();
            switch (parsedKey)
            {
                case MovementKey.W:
                    result.X = currentPlayerCoords.X;
                    result.Y += currentPlayerCoords.Y - 1;
                    break;
                case MovementKey.A:
                    result.X = currentPlayerCoords.X - 1;
                    result.Y += currentPlayerCoords.Y;
                    break;
                case MovementKey.S:
                    result.X = currentPlayerCoords.X;
                    result.Y += currentPlayerCoords.Y + 1;
                    break;
                case MovementKey.D:
                    result.X = currentPlayerCoords.X + 1;
                    result.Y += currentPlayerCoords.Y;
                    break;                
            }
            if (result.X > field.GetLength(1) - 1 || result.X < 0)
            {
                result.X = currentPlayerCoords.X;
            }
            if (result.Y > field.GetLength(0) - 1 || result.Y < 0)
            {
                result.Y = currentPlayerCoords.Y;
            }            
            if (field[result.Y, result.X] == FieldType.Goal)
            {
                if (Timer.IsRunning==true)
                {
                    Timer.Stop();            
                }
                if (IsRunning==true)
                {
                    IsRunning = false;
                }                
            }            
            field[currentPlayerCoords.Y, currentPlayerCoords.X] = FieldType.Field;
            field[result.Y, result.X] = FieldType.Player;           
            return result;
        }

        /// <summary>
        /// Initializes the game logics representation of the field
        /// </summary>
        /// <param name="player">Coordinates of the player</param>
        /// <param name="goal">Coordinates of the goal</param>
        /// <param name="fieldWidth">Field width</param>
        /// <param name="fieldHeight">Field height</param>
        public void InitializeField(Coordinate player, Coordinate goal, int fieldWidth, int fieldHeight)
        {
            field = new FieldType[fieldHeight, fieldWidth];           

            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    field[i, j] = FieldType.Field;
                }
            }

            field[player.Y, player.X] = FieldType.Player;
            field[goal.Y, goal.X] = FieldType.Goal;
        }

        /// <summary>
        /// Resets the game logics state
        /// </summary>
        public void ResetLogic() {
            IsRunning = true;
            if (Timer.IsRunning == true)
            {
                Timer.Stop();
            }          
            Timer.Reset();
            Timer.Start();
        }

        /// <summary>
        /// Gets the players current coordinates
        /// </summary>
        /// <returns>Players current coordinates</returns>
        private Coordinate GetPlayerCoordinates() {            
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i,j]==FieldType.Player)
                    {
                        return new Coordinate(j,i);
                    }
                }
            }
            throw new IndexOutOfRangeException();
        }
    }   
}
