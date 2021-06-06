using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Tetris.Manager;
using Tetris.Shape;

namespace Tetris.Utility
{
    using TetrisRow = List<Image>;

    /// <summary>
    /// 结点处理工具
    /// </summary>
    public static class NodesUtility
    {
        /// <summary>
        /// 初始化一个区域的 Nodes
        /// </summary>
        /// <param name="originColor">初始颜色</param>
        /// <param name="nodeParents">nodes 来源</param>
        public static List<TetrisRow> InitAreaNodes(Sprite originColor, params Transform[] nodeParents)
        {
            // nodes: 记录所有的结点
            var nodes = new List<TetrisRow>();

            // 总行数索引 (行数从 0 开始, 索引从 -1 开始)
            var nodesRowIndex = -1;

            // 每个部分单独处理, 每一个部分称作一个面板
            foreach (var onePanel in nodeParents)
            {
                // 当前面板的行数
                var rowCount = onePanel.childCount;

                for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    // 取出单独的一行并保存
                    var row = onePanel.GetChild(rowIndex);
                    nodes.Add(new TetrisRow());

                    // 保存后, 行索引自增
                    nodesRowIndex++;

                    // 总列数索引, 每遍历一个新行, 索引必须重新计算 (行数从 0 开始, 索引从 -1 开始)
                    var nodesColumnIndex = -1;

                    for (var columnIndex = 0; columnIndex < row.childCount; columnIndex++)
                    {
                        // 取出每一个结点并保存
                        var node = row.GetChild(columnIndex).GetComponent<Image>();
                        nodes[nodesRowIndex].Add(node);

                        // 保存后, 列索引自增
                        nodesColumnIndex++;

                        // 初始化结点的颜色为背景色
                        nodes[nodesRowIndex][nodesColumnIndex].sprite = originColor;
                    }
                }
            }

            return nodes;
        }
        
        /// <summary>
        /// 刷新当前形状的显示
        /// </summary>
        /// <param name="backColor">背景色</param>
        public static void RefreshCurrentShapeDisplay(Sprite backColor)
        {
            var shape = RandomManager.currentTetrisShape.shape;
            var newColor = shape.GetNodesInfo()[0].color;
            SetShapeColor(shape, backColor);
            SetShapeColor(shape, newColor);
        }

        /// <summary>
        /// 游戏结束判断
        /// </summary>
        public static bool GameOverJudge(TetrisShape tetrisShape)
        {
            var isGameOver = false;

            foreach (var node in tetrisShape.GetNodesInfo())
            {
                if (node.position.x > DataManager.DeathLineIndex)
                {
                    isGameOver = true;
                }
            }

            if (isGameOver)
            {
                Debug.Log($"游戏结束!");
            }

            return isGameOver;
        }
        
        /// <summary>
        /// 重置所有的 Nodes
        /// </summary>
        /// <param name="originColor">初始颜色</param>
        public static void ResetAllAreaNodes(Sprite originColor)
        {
            // 玩家区域
            for (var rowIndex = 0; rowIndex < NodesManager.RowCount; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < NodesManager.ColumnCount; columnIndex++)
                {
                    NodesManager.GetNodeColor(rowIndex, columnIndex).sprite = originColor;
                }
            }

            // 提示 One
            for (var rowIndex = 0; rowIndex < TipsManager.RowCount; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < TipsManager.ColumnCount; columnIndex++)
                {
                    TipsManager.GetTipOneNodeColor(rowIndex, columnIndex).sprite = originColor;
                }
            }
            
            // 提示 Two
            for (var rowIndex = 0; rowIndex < TipsManager.RowCount; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < TipsManager.ColumnCount; columnIndex++)
                {
                    TipsManager.GetTipTwoNodeColor(rowIndex, columnIndex).sprite = originColor;
                }
            }
        }

        /// <summary>
        /// 设置形状颜色
        /// </summary>
        /// <param name="shape">形状</param>
        /// <param name="color">颜色</param>
        public static void SetShapeColor(TetrisShape shape, Sprite color)
        {
            foreach (var node in shape.GetNodesInfo())
            {
                NodesManager.GetNodeColor(node.position.x, node.position.y).sprite = color;
            }
        }
    }
}