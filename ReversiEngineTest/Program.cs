using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReversiEngine1;
using Reversi.Core;

namespace ReversiEngineTest
{
    class Program
    {
        static ThinkingEngine engine = new ThinkingEngine();
        static ReversiBoard board = ReversiBoard.InitBoard();
        static void Main(string[] args)
        {
            Think();
            Console.ReadKey();
        }
        static async void Think()
        {
            var res = await engine.Think(board, StoneType.Sente);
            board = board.AddStone(res.Row,res.Col,StoneType.Sente);
            var res2 = await engine.Think(board, StoneType.Gote);
            Console.WriteLine(string.Format("{0},{1}", res.Row, res.Col));
        }
    }
}
