﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Core;

namespace ReversiEngine1
{
    /// <summary>
    /// 一番多く返せる手を選ぶエンジン
    /// </summary>
    class CountingEngine
    {
        //オセロの局面
        Reversi.Core.ReversiBoard board;
        //先手または後手
        Reversi.Core.StoneType player;
        //合法手のリスト
        List<ReversiMove> legalMoves = new List<ReversiMove>();

        /// <summary>
        /// 盤の情報をもとに思考し、次の手を返す
        /// </summary>
        /// <param name="board"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task<ReversiMove> Think(ReversiBoard board, StoneType player)
        {
            return await Task.Run(() =>
            {
                this.board = board;
                this.player = player;
                legalMoves = board.SearchLegalMoves(player); //合法手
                if (legalMoves.Count == 0)
                {
                    throw new InvalidOperationException("合法手がありません");
                }
                else
                {
                    var best = 0;
                    var bestMove = default(ReversiMove);
                    foreach (var item in board.SearchLegalMoves(player))
                    {
                        var child = board.AddStone(item.Row, item.Col, player);
                        switch (player)
                        {
                            case StoneType.None:
                                break;
                            case StoneType.Sente:
                                if (best < child.NumOfBlack())
                                {
                                    best = child.NumOfBlack();
                                    bestMove = item;
                                }
                                break;
                            case StoneType.Gote:
                                if (best < child.NumOfWhite())
                                {
                                    best = child.NumOfWhite();
                                    bestMove = item;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    return bestMove;
                }
            });

        }
    }
}