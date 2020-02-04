using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLearning
{
    public class Agent
    {
        public bool IsRunning { get; internal set; }               

        private int initialPlayerX;
        private int initialPlayerY;
        private int goalX;
        private int goalY;
        private int fieldWidth;
        private int fieldHeight;

        private double[,] qMatrix;
        private double[,] rMatrix;

        private const double gamma = 0.8;

        public Agent(int initialPlayerX, int initialPlayerY, int goalX, int goalY, int fieldWidth, int fieldHeight)
        {                       
            IsRunning = false;
            this.initialPlayerX = initialPlayerX;
            this.initialPlayerY = initialPlayerY;
            this.goalX = goalX;
            this.goalY = goalY;
            this.fieldWidth = fieldWidth;
            this.fieldHeight = fieldHeight;
            qMatrix = new double[fieldHeight * fieldWidth, fieldHeight * fieldWidth];
            rMatrix = new double[fieldHeight * fieldWidth, fieldHeight * fieldWidth];
            
            for (int i = 0; i < qMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < qMatrix.GetLength(1); j++)
                {
                    qMatrix[i, j] = 0;
                }
            }

            for (int i = 0; i < rMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < rMatrix.GetLength(1); j++)
                {
                    rMatrix[i, j] = -1;
                }
            }

            for (int i = 0; i < rMatrix.GetLength(0); i++)
            {
                if (i % fieldWidth != 0 && i - 1 > -1)
                {
                    rMatrix[i, i - 1] = 0;
                }
                if ((i - (fieldWidth - 1)) % fieldWidth != 0 && i + 1 < fieldHeight * fieldWidth)
                {
                    rMatrix[i, i + 1] = 0;
                }
                if (i - fieldWidth >= 0)
                {
                    rMatrix[i, i - fieldWidth] = 0;
                }
                if (i + fieldWidth < fieldHeight * fieldWidth)
                {
                    rMatrix[i, i + fieldWidth] = 0;
                }               
            }

            for (int i = 0; i < rMatrix.GetLength(0); i++)
            {
                int goal = 0;
                for (int j = 0; j < goalY; j++)
                {
                    goal += fieldWidth;
                }
                goal += goalX;
                if (rMatrix[i, goal] == 0)
                {
                    rMatrix[i, goal] = 100;
                }
            }
        }

        /// <summary>
        /// Starts the agent
        /// </summary>
        /// <returns>Sequence of actions leading to the desired result and the related data to be visualized</returns>
        public (IEnumerable<int> path, IEnumerable<int> dataToVisualize) Start()
        {           
            IsRunning = true;
            Random rand = new Random();
            List<int> result = new List<int>();
            List<int> dataToVisualize = new List<int>();
          
            int goalState = 0;
            for (int j = 0; j < goalY; j++)
            {
                goalState += fieldWidth;
            }
            goalState += goalX;
            
            for (int i = 0; i < 1000; i++)
            {
                int numOfActions = 0;
                int state = rand.Next(0, rMatrix.GetLength(1));
                do
                {
                    Dictionary<int, double> allActions = new Dictionary<int, double>();
                    for (int j = 0; j < rMatrix.GetLength(1); j++)
                    {
                        allActions.Add(j, rMatrix[state, j]);
                    }
                    Dictionary<int, double> possibleActions = new Dictionary<int, double>();
                    foreach (KeyValuePair<int, double> item in allActions)
                    {
                        if (item.Value != -1)
                        {
                            possibleActions.Add(item.Key, item.Value);
                        }
                    }
                    int action = possibleActions.Keys.ToArray()[rand.Next(0, possibleActions.Count)];
                    int nextState = action;
                    double maxQ = -1;
                    for (int j = 0; j < qMatrix.GetLength(1); j++)
                    {
                        if (qMatrix[nextState, j] > maxQ)
                        {
                            maxQ = qMatrix[nextState, j];
                        }                        
                    }                    
                    qMatrix[state, action] = rMatrix[state, action] + gamma * maxQ;
                    state = nextState;
                    numOfActions++;
                } while (state != goalState);    
                dataToVisualize.Add(numOfActions);
            }

            int currentState = 0;
            for (int j = 0; j < initialPlayerY; j++)
            {
                currentState += fieldWidth;
            }
            currentState += initialPlayerX;

            do
            {                
                double maxQ = -1;
                int nextState = -1;
                for (int j = 0; j < qMatrix.GetLength(1); j++)
                {
                    if (qMatrix[currentState, j] > maxQ)
                    {
                        maxQ = qMatrix[currentState, j];
                        nextState = j;
                    }
                }
                currentState = nextState;
                result.Add(currentState);
            } while (currentState != goalState);
            
            IsRunning = false;
            return (result.AsEnumerable(), dataToVisualize.AsEnumerable());
        }        
    }
}
