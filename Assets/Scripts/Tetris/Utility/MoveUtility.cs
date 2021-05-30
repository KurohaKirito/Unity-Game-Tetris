using System;
using System.Linq;
using Managers;
using Tetris.Manager;
using UnityEngine;

namespace Tetris.Utility
{
    public static class MoveUtility
    {
        /// <summary>
        /// 让特定形状下落
        /// </summary>
        /// <param name="shape">形状变量</param>
        /// <param name="backColor">背景颜色</param>
        /// <typeparam name="T">形状类型</typeparam>
        public static void MoveDown<T>(T shape, Sprite backColor) where T : Shape.TetrisShape
        {
            Move(shape, backColor, EM_ACTION_TYPE.Down, shape.MoveDown);
        }

        /// <summary>
        /// 让特定形状左移
        /// </summary>
        /// <param name="shape">形状变量</param>
        /// <param name="backColor">背景颜色</param>
        /// <typeparam name="T">形状类型</typeparam>
        public static void MoveLeft<T>(T shape, Sprite backColor) where T : Shape.TetrisShape
        {
            Move(shape, backColor, EM_ACTION_TYPE.Left, shape.MoveLeft);
        }

        /// <summary>
        /// 让特定形状右移
        /// </summary>
        /// <param name="shape">形状变量</param>
        /// <param name="backColor">背景颜色</param>
        /// <typeparam name="T">形状类型</typeparam>
        public static void MoveRight<T>(T shape, Sprite backColor) where T : Shape.TetrisShape
        {
            Move(shape, backColor, EM_ACTION_TYPE.Right, shape.MoveRight);
        }
        
        /// <summary>
        /// 移动判断
        /// </summary>
        /// <param name="shape">形状变量</param>
        /// <param name="moveType">移动类型</param>
        /// <param name="backColor">背景色</param>
        /// <typeparam name="T">形状的类型</typeparam>
        /// <returns></returns>
        private static bool MoveJudge<T>(T shape, EM_ACTION_TYPE moveType, Sprite backColor) where T : Shape.TetrisShape
        {
            // 如果移动后越界, 则不允许移动
            if (shape.GetNodesInfo().Any(node =>
            {
                return moveType switch
                {
                    EM_ACTION_TYPE.Up => false,
                    EM_ACTION_TYPE.Down => node.position.x <= NodesManager.RowIndex.min,
                    EM_ACTION_TYPE.Left => node.position.y <= NodesManager.ColumnIndex.min,
                    EM_ACTION_TYPE.Right => node.position.y >= NodesManager.ColumnIndex.max,
                    _ => false
                };
            }))
            {
                return false;
            }

            // 如果移动后导致重叠, 则不允许移动
            for (var i = 0; i < shape.NodeCount; i++)
            {
                int nextX;
                int nextY;
                var nodeInfo = shape.GetNodesInfo()[i];

                switch (moveType)
                {
                    case EM_ACTION_TYPE.Up:
                        nextX = nodeInfo.position.x + 1;
                        nextY = nodeInfo.position.y;
                        break;
                    case EM_ACTION_TYPE.Down:
                        nextX = nodeInfo.position.x - 1;
                        nextY = nodeInfo.position.y;
                        break;
                    case EM_ACTION_TYPE.Left:
                        nextX = nodeInfo.position.x;
                        nextY = nodeInfo.position.y - 1;
                        break;
                    case EM_ACTION_TYPE.Right:
                        nextX = nodeInfo.position.x;
                        nextY = nodeInfo.position.y + 1;
                        break;
                    default: // Down
                        nextX = nodeInfo.position.x - 1;
                        nextY = nodeInfo.position.y;
                        break;
                }

                if (NodesManager.GetNodeColor(nextX, nextY).sprite.Equals(backColor))
                {
                    continue;
                }
                
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// 移动特定形状
        /// </summary>
        /// <param name="shape">形状</param>
        /// <param name="backColor">背景色</param>
        /// <param name="moveType">移动行为类型</param>
        /// <param name="move">刷新时执行的移动行为</param>
        /// <typeparam name="T">形状类型</typeparam>
        private static void Move<T>(T shape, Sprite backColor, EM_ACTION_TYPE moveType, Action move) where T : Shape.TetrisShape
        {
            // 擦除旧形状
            foreach (var node in shape.GetNodesInfo())
            {
                NodesManager.GetNodeColor(node.position.x, node.position.y).sprite = backColor;
            }

            // 判断是否可以移动
            var isCanMove = MoveJudge(shape, moveType, backColor);
            
            // 如果可以移动
            if (isCanMove)
            {
                move?.Invoke();
            }
            
            // 绘制新形状
            foreach (var node in shape.GetNodesInfo())
            {
                NodesManager.GetNodeColor(node.position.x, node.position.y).sprite = node.color;
            }

            // 如果发生了结点堆叠, 则执行以下操作
            if (!isCanMove && moveType == EM_ACTION_TYPE.Down)
            {
                NodeStacked(shape, backColor);
            }
        }
        
        /// <summary>
        /// 堆叠后需要进行的判断操作
        /// </summary>
        /// <param name="shape">当前形状</param>
        /// <param name="backColor">背景色</param>
        /// <typeparam name="T"></typeparam>
        private static void NodeStacked<T>(T shape, Sprite backColor) where T : Shape.TetrisShape
        {
            // TODO 堆叠后需要进行的判断操作
            
            // 1. 清除判定
            ClearUtility.ClearJudge(shape, backColor, ref NodesManager.clearRowIndexList);
            if (NodesManager.clearRowIndexList.Count > 0)
            {
                ClearUtility.ClearRows();
            }
            
            // 2. Game Over 判断
            if (NodesUtility.GameOverJudge(shape))
            {
                GameManager.Instance.GameOver();
                return;
            }

            // 3. 生成下一个结点
            RandomManager.NextShape();
            GameManager.Instance.tetrisShape = RandomManager.currentTetrisShape;
            
            // 4. 存档
            DataManager.SaveData();
            
            // 5. 刷新三个结点的显示
            TipsManager.RefreshTipOneDisplay(backColor);
            TipsManager.RefreshTipTwoDisplay(backColor);
            NodesUtility.RefreshNodeDisplay(GameManager.Instance.tetrisShape.shape, backColor);
        }
    }
}