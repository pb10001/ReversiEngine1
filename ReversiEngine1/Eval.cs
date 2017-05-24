using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiEngine1
{
    /// <summary>
    /// 評価関数
    /// </summary>
    public class Eval
    {
        public Eval()
        {
            FromParamsString("89,7,-56,56,-10,-88,-73,18,-28,-98,24,-5,65,-100,-4,-21");
        }
        public void FromParamsString(string paramsString)
        {
            var paramsArray = paramsString.Split(',').Select(x => int.Parse(x)).ToArray();
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    var param = paramsArray[4 * row + col];

                    evalBoard[    row,     col] = param;
                    evalBoard[7 - row,     col] = param;
                    evalBoard[    row, 7 - col] = param;
                    evalBoard[7 - row, 7 - col] = param;

                }
            }
        }
        private int[,] evalBoard = new int[8, 8];
        public int Execute(bool[,] black,bool[,] white)
        {
            var blackValue = 0;
            var whiteValue = 0;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (black[row, col])
                    {
                        blackValue += evalBoard[row, col];
                    }
                }
            }
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (white[row, col])
                    {
                        whiteValue += evalBoard[row, col];
                    }
                }
            }
            return blackValue-whiteValue;
        }
    }
}
