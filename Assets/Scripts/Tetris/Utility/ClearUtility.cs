using System.Collections.Generic;
using Tetris.Manager;
using UnityEngine;

namespace Tetris.Utility
{
    public static class ClearUtility
    {
        /// <summary>
        /// 消除判断
        /// </summary>
        /// <param name="shape">当前正在下落的形状</param>
        /// <param name="backColor">背景色</param>
        /// <param name="clearRows">存储待清除的行索引</param>
        /// <typeparam name="T">当前正在下落的形状的类型</typeparam>
        public static void ClearJudge<T>(T shape, Sprite backColor, ref List<int> clearRows) where T : Shape.TetrisShape
        {
            clearRows ??= new List<int>();
            
            var nodeInfos = shape.GetNodesInfo();
            
            for (var index = 0; index < nodeInfos.Length; index++)
            {
                var rowIndex = nodeInfos[index].position.x;
                
                if (!JudgeOneRowCompleted(rowIndex, backColor))
                {
                    continue;
                }
                
                if (!clearRows.Exists(n => n == rowIndex))
                {
                    clearRows.Add(rowIndex);
                }
            }
            
            // 由于消除时的逻辑必须按照顺序依次消除, 因此必须进行排序
            clearRows.Sort();
        }
        
        /// <summary>
        /// 判断特定行是否可以被消除
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <param name="backColor">背景色</param>
        /// <returns></returns>
        private static bool JudgeOneRowCompleted(int rowIndex, Sprite backColor)
        {
            var completed = true;
            
            for (var columnIndex = 0; columnIndex < NodesManager.ColumnCount; columnIndex++)
            {
                if (NodesManager.GetNodeColor(rowIndex, columnIndex).sprite.Equals(backColor))
                {
                    completed = false;
                }
            }

            return completed;
        }
        
        /// <summary>
        /// 消除所有等待消除的行
        /// </summary>
        public static void ClearRows()
        {
            // 重点:
            // 1. foreach 中不允许对被遍历对象进行更新, 即使调用的方法对遍历者进行了更新也不行
            // 2. 由于消除会导致上方全部行下移, 因此必须由上向下消除
            
            var endIndex = NodesManager.clearRowIndexList.Count - 1;
            
            for (var index = endIndex; index >= 0; index--)
            {
                ClearOneRow(NodesManager.clearRowIndexList[index]);
                
                // TODO 制定更加详细的加分规则, 当前的太简陋了, 所有一律加 100
                DataManager.UpdateScoreLevel(100);
            }
                        
            NodesManager.clearRowIndexList.Clear();
        }
        
        /// <summary>
        /// 消除指定的一行
        /// </summary>
        /// <param name="clearRowIndex">行索引</param>
        private static void ClearOneRow(int clearRowIndex)
        {
            for (var rowIndex = clearRowIndex; rowIndex < NodesManager.RowIndex.max; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < NodesManager.ColumnCount; columnIndex++)
                {
                    // 上面的行覆盖下面的行
                    NodesManager.GetNodeColor(rowIndex, columnIndex).sprite =
                        NodesManager.GetNodeColor(rowIndex + 1, columnIndex).sprite;
                }
            }
        }
    }
}