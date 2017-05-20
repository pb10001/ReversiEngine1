using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkingEngineBase;
using Reversi.Core;

namespace ReversiEngine1
{
    public class ThinkingEngine : IThinkingEngine
    {
        public string Name
        {
            get
            {
                return "弱いエンジン";
            }
        }

        public void SetTimeLimit(int milliSecond)
        {
            throw new NotImplementedException();
        }

        public async Task<ReversiMove> Think(ReversiBoard board, StoneType player)
        {
            return await Task.Run(() =>
            {
                return board.SearchLegalMoves(player).First();
            });
        }
    }
}
