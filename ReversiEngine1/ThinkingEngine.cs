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
        public ThinkingEngine()
        {
            Init();
        }
        public string Name
        {
            get
            {
                return "100万局面読む";
            }
        }
        public int GetEval()
        {
            return eval;
        }
        public void SetTimeLimit(int milliSecond)
        {
            throw new NotImplementedException();
        }

        //public async Task<ReversiMove> Think(ReversiBoard board, StoneType player)
        //{
        //    return await Task.Run(() =>
        //    {
        //        return board.SearchLegalMoves(player).First();
        //    });
        //}

        Dictionary<ReversiBoard, ReversiMove> moveMap = new Dictionary<ReversiBoard, ReversiMove>();
        //探索の深さ
        int depthForDeep;
        ////探索の広さ
        int breadthForDeep;

        //探索の深さ
        int depthForBroad;
        ////探索の広さ
        int breadthForBroad;
        int eval;
        private void Init()
        {
            depthForDeep = 60;
            breadthForDeep = 1;

            depthForBroad = 6;
            breadthForBroad = 6;
            moveMap = new Dictionary<ReversiBoard, ReversiMove>();
        }
        public async Task<ReversiMove> Think(ReversiBoard board, StoneType player)
        {
            var broad = new Dictionary<ReversiMove, int>();
            var deep = new Dictionary<ReversiMove, int>();
            if (board.NumOfBlack() + board.NumOfWhite() >= 59)
            {
                return await new CountingEngine().Think(board, player);
            }
            else if (board.NumOfBlack() + board.NumOfWhite() >= 60 - depthForBroad)
            {
                depthForBroad = 60 - board.NumOfBlack() - board.NumOfWhite() - 1;

            }
            if (board.NumOfBlack() + board.NumOfWhite() >= 60 - depthForDeep)
            {
                depthForDeep = 60 - board.NumOfBlack() - board.NumOfWhite() - 1;
            }
            var childMove = board.SearchLegalMoves(player);
            foreach (var move in childMove)
            {
                var child = board.AddStone(move.Row, move.Col, player);
                moveMap[child] = move;
            }

            broad = await Evaluate(board, player,  depthForBroad, breadthForBroad);
            //deep  = await Evaluate(board, player, depthForDeep, breadthForDeep);
            System.IO.File.AppendAllText("debug.log", string.Join("-", broad.Values));
            System.IO.File.AppendAllText("debug.log", "\r\n");
            System.IO.File.AppendAllText("debug.log", string.Join("-", deep.Values));
            System.IO.File.AppendAllText("debug.log", "\r\n");
            var evalMap = new Dictionary<ReversiMove, int>();
            foreach (var item in moveMap.Values)
            {
                var eval = broad[broad.Keys.First(x => x.Row == item.Row && x.Col == item.Col)];
                //var eval = 2*broad[broad.Keys.First(x => x.Row == item.Row && x.Col == item.Col)] + deep[deep.Keys.First(x => x.Row == item.Row && x.Col == item.Col)];
                evalMap.Add(item,eval);
            }
            if (player == StoneType.Sente)
            {
                var best = evalMap.Values.Max();
                eval = best;
                Init();
                return evalMap.Keys.First(x => evalMap[x] == best);
            }
            else
            {
                var best = evalMap.Values.Min();
                eval = best;
                Init();
                return evalMap.Keys.First(x => evalMap[x] == best);
            }
        }

        /// <summary>
        /// 盤の情報をもとに思考し、次の手を返す
        /// </summary>
        /// <param name="board"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task<Dictionary<ReversiMove,int>> Evaluate(ReversiBoard board, StoneType player, int depth,int breadth)
        {
            List<ReversiBoard>[] moveTree;
            Dictionary<ReversiBoard, int> countMap = new Dictionary<ReversiBoard, int>();
            Dictionary<ReversiBoard, List<ReversiBoard>> childMap = new Dictionary<ReversiBoard, List<ReversiBoard>>();

            return await Task.Run(() =>
            {
                moveTree = new List<ReversiBoard>[depth + 1];
                moveTree[0] = new List<ReversiBoard>() { board };
                for (int i = 0; i < depth; i++)
                {
                    moveTree[i + 1] = new List<ReversiBoard>();
                    foreach (var item in moveTree[i])
                    {
                        childMap.Add(item, new List<ReversiBoard>());
                        var plyr = (i + (int)player) % 2 == 1 ? StoneType.Sente : StoneType.Gote;
                        var tmpList = new List<ReversiBoard>();
                        var childMove = item.SearchLegalMoves(plyr);
                        if (i == 0 && childMove.Count == 0)
                        {
                            throw new InvalidOperationException("合法手がありません");
                        }
                        if (i == 0)
                        {
                            tmpList.AddRange(moveMap.Keys);
                        }
                        else
                        {
                            foreach (var move in childMove)
                            {
                                var child = item.AddStone(move.Row, move.Col, plyr);
                                tmpList.Add(child);
                            }
                        }
                        
                        if ((i + (int)player) % 2 == 1)
                        {
                            tmpList = tmpList.OrderByDescending(x => new Eval().Execute(x.BlackToMat(),x.WhiteToMat())).ToList();
                        }
                        else
                        {
                            tmpList = tmpList.OrderBy(x => new Eval().Execute(x.BlackToMat(),x.WhiteToMat())).ToList();
                        }
                        if (tmpList.Count > breadth && i != 0)
                        {
                            for (int j = 0; j < breadth; j++)
                            {
                                var child = tmpList[j];
                                moveTree[i + 1].Add(child);
                                childMap[item].Add(child);
                            }
                        }
                        else
                        {
                            foreach (var child in tmpList)
                            {
                                moveTree[i + 1].Add(child);
                                childMap[item].Add(child);
                            }
                        }

                    }
                }
                foreach (var item in moveTree[depth])
                {
                    var count = new Eval().Execute(item.BlackToMat(),item.WhiteToMat());
                    countMap[item] = count;
                }
                for (int i = depth - 1; i >= 1; i--)
                {
                    if ((i+(int)player)%2 == 1)
                    {
                        foreach (var item in moveTree[i])
                        {
                            var best = -999999;
                            foreach (var child in childMap[item])
                            {
                                //var count = (i + (int)player)%2 == 1 ? Eval.Execute(child.BlackToMat()) : Eval.Execute(child.WhiteToMat());
                                var count = countMap[child];
                                if (count > best)
                                {
                                    best = count;
                                }
                            }

                            countMap[item] = best;
                        }
                    }
                    else
                    {
                        foreach (var item in moveTree[i])
                        {
                            var best = 999999;
                            foreach (var child in childMap[item])
                            {
                                //var count = (i + (int)player)%2 == 1 ? Eval.Execute(child.BlackToMat()) : Eval.Execute(child.WhiteToMat());
                                var count = countMap[child];
                                if (count < best)
                                {
                                    best = count;
                                }
                            }
                            countMap[item] = best;
                        }
                    }
                    
                }
                var resMap = new Dictionary<ReversiMove, int>();
                foreach (var item in moveTree[1])
                {
                    resMap.Add(moveMap[item],countMap[item]);
                }
                var text = Enumerable.Range(0, moveTree.Count())
                        .Zip(moveTree.Select(x => string.Join(", ", x.Count) + "\n"),
                        (x, y) => x + ": " + y);
                System.IO.File.AppendAllText("debug.log", string.Join("\r\n", text));
                System.IO.File.AppendAllText("debug.log", "\r\n");
                return resMap;
            });

        }
        
    }
}
