﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class Coordinate
    {
        public int X { get; internal set; }
        public int Y { get; internal set; }

        public Coordinate()
        {

        }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
