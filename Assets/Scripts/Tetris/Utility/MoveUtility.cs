using System;
using System.Linq;
using DG.Tweening;
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
        /// <typeparam name="T">形状的类型</typeparam>
        /// <returns></returns>
        private static bool MoveJudge<T>(T shape, EM_ACTION_TYPE moveType) where T : Shape.TetrisShape
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
                var nodeInfo = shape.GetNodesInfo()[i];

                var nextPosition = moveType switch
                {
                    EM_ACTION_TYPE.Up => nodeInfo.position + new Vector2Int(1, 0),
                    EM_ACTION_TYPE.Down => nodeInfo.position + new Vector2Int(-1, 0),
                    EM_ACTION_TYPE.Left => nodeInfo.position + new Vector2Int(0, -1),
                    EM_ACTION_TYPE.Right => nodeInfo.position + new Vector2Int(0, 1),
                    _ => nodeInfo.position + new Vector2Int(1, 0)
                };

                if (RandomManager.IsNodeColor(NodesManager.GetNodeColor(nextPosition).sprite))
                {
                    return false;
                }
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
            // 取出颜色
            var color = shape.GetNodesInfo()[0].color;

            // 擦除旧形状
            NodesUtility.SetShapeColor(shape, backColor);

            // 判断是否可以移动
            var isCanMove = MoveJudge(shape, moveType);

            // 如果可以移动
            if (isCanMove)
            {
                move?.Invoke();
                PredictManager.ClearPredictShape(backColor);
                PredictManager.UpdatePredictShape(backColor, shape.GetNodesInfo());
            }

            // 绘制新形状
            NodesUtility.SetShapeColor(shape, color);

            // 如果发生了结点堆叠, 则执行以下操作
            if (isCanMove == false && moveType == EM_ACTION_TYPE.Down)
            {
                PredictManager.ClearPredictShape(backColor);
                NodesUtility.SetShapeColor(shape, color);

                foreach (var nodeInfo in shape.GetNodesInfo())
                {
                    var image = NodesManager.GetNodeColor(nodeInfo.position);
                    image.transform.DOScale(new Vector3(0.6f, 0.6f, 1), 0.1f).onComplete = () =>
                    {
                        image.transform.DOScale(Vector3.one, 0.1f);
                    };
                }

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
            // 清除判定
            ClearUtility.ClearJudge(shape, ref NodesManager.clearRowIndexList);
            if (NodesManager.clearRowIndexList.Count > 0)
            {
                // 震动
                if (GameManager.Instance.vibrantEnable)
                {
                    VibrateManager.Instance.TriggerSuccess();
                }

                // 清除
                ClearUtility.ClearRows();
            }

            // Game Over 判断
            if (NodesUtility.GameOverJudge(shape))
            {
                GameManager.Instance.GameOver();
                return;
            }

            // 生成下一个结点
            RandomManager.NextShape();

            // 存档
            DataManager.SaveData();

            // 刷新高亮提示
            PredictManager.UpdatePredictShape(backColor, RandomManager.currentTetrisShape.shape.GetNodesInfo());

            // 刷新三个结点的显示
            TipsManager.RefreshTipOneDisplay(backColor);
            TipsManager.RefreshTipTwoDisplay(backColor);
            NodesUtility.RefreshCurrentShapeDisplay(backColor);
        }
    }
}