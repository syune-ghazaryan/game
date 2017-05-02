using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Racing
{
    public class GameMatrix
    {
        public int[,] Matrix{get; set;}
        public GameMatrix(int row,int col)
        {
            this.Matrix =new int[row, col];
        }
    }
}
