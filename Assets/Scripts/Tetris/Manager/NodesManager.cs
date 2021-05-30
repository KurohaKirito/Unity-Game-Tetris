using System.Collections.Generic;
using Tetris.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris.Manager
{
    using TetrisRow = List<Image>;

    /// <summary>
    /// 结点管理类
    /// 管理所有的结点
    /// </summary>
    public static class NodesManager
    {
        /// <summary>
        /// 所有的玩家区域结点
        /// </summary>
        private static List<TetrisRow> playerAreaNodes;

        /// <summary>
        /// 当前需要消除的行索引
        /// </summary>
        public static List<int> clearRowIndexList;
        
        /// <summary>
        /// 行数
        /// </summary>
        public static int RowCount => playerAreaNodes.Count;

        /// <summary>
        /// 列数
        /// </summary>
        public static int ColumnCount => playerAreaNodes[0].Count;
        
        /// <summary>
        /// 行边界索引
        /// </summary>
        public static Border2 RowIndex => new Border2(0, RowCount - 1);

        /// <summary>
        /// 列边界索引
        /// </summary>
        public static Border2 ColumnIndex => new Border2(0, ColumnCount - 1);
        
        /// <summary>
        /// 出生点坐标
        /// </summary>
        public static Vector2Int BirthPosition => new Vector2Int(RowIndex.max - 1, ColumnCount / 2);

        /// <summary>
        /// 初始化玩家区域和随机区域
        /// </summary>
        /// <param name="backColor">背景色</param>
        /// <param name="playerArea">玩家区域</param>
        /// <param name="randomArea">随机区域</param>
        public static void Init(Sprite backColor, Transform playerArea, Transform randomArea)
        {
            playerAreaNodes = NodesUtility.InitAreaNodes(backColor, playerArea, randomArea);
        }

        /// <summary>
        /// 获取指定坐标结点的颜色
        /// </summary>
        /// <param name="rowIndex">行坐标</param>
        /// <param name="columnIndex">列坐标</param>
        /// <returns></returns>
        public static Image GetNodeColor(int rowIndex, int columnIndex)
        {
            return playerAreaNodes[rowIndex][columnIndex];
        }
    }
}