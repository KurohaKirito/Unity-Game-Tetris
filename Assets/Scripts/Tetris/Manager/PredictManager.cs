using System.Collections.Generic;
using System.Linq;
using Tetris.Shape;
using UnityEngine;

namespace Tetris.Manager
{
    public static class PredictManager
    {
        /// <summary>
        /// [临时变量]高亮结点是否可用
        /// </summary>
        private static List<bool> nodeEnable;

        /// <summary>
        /// 当前高亮的结点信息
        /// </summary>
        private static List<TetrisNodeInfo> predictShape;

        /// <summary>
        /// 所有的预判高亮颜色
        /// </summary>
        private static List<Sprite> predictColors;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="predictColorList">所有预判高亮颜色</param>
        public static void Init(List<Sprite> predictColorList)
        {
            if (predictColorList == null || predictColorList.Count <= 0)
            {
                return;
            }

            predictColors = predictColorList;
        }

        /// <summary>
        /// 判断颜色是否是结点颜色
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public static bool IsPredictColor(Sprite color)
        {
            return Enumerable.Contains(predictColors, color);
        }

        /// <summary>
        /// 更新预判高亮形状
        /// </summary>
        public static void UpdatePredictShape(Sprite backColor, TetrisNodeInfo[] currentShapeNodesInfo)
        {
            // 取出当前颜色
            var colorIndex = RandomManager.GetColorIndexByName(currentShapeNodesInfo[0].color.name);

            // 如果未初始化, 则初始化
            if (predictShape == null)
            {
                predictShape = new List<TetrisNodeInfo>();
                nodeEnable = new List<bool>();
                for (var index = 0; index < currentShapeNodesInfo.Length; index++)
                {
                    predictShape.Add(new TetrisNodeInfo());
                    nodeEnable.Add(false);
                }
            }

            // 获取形状的最低结点
            var lowNode = currentShapeNodesInfo[0];
            foreach (var nodeInfo in currentShapeNodesInfo)
            {
                if (nodeInfo.position.x < lowNode.position.x)
                {
                    lowNode = nodeInfo;
                }
            }
            
            // 定义是否要显示高亮, 默认不显示
            var displayPredictShape = false;

            // 计算得出新的高亮形状
            for (var offset = 1; offset <= lowNode.position.x; ++offset)
            {
                for (var index = 0; index < predictShape.Count; index++)
                {
                    nodeEnable[index] = NodesManager.GetNodeColor(
                        currentShapeNodesInfo[index].position.x - offset,
                        currentShapeNodesInfo[index].position.y).sprite == backColor;
                }

                if (nodeEnable.Exists(enable => enable == false))
                {
                    break;
                }

                // nodeEnable 全部是 true 时说明有可以显示的提示
                displayPredictShape = true;
                for (var index = 0; index < predictShape.Count; index++)
                {
                    predictShape[index] = new TetrisNodeInfo
                    {
                        position = new Vector2Int(
                            currentShapeNodesInfo[index].position.x - offset,
                            currentShapeNodesInfo[index].position.y),
                        color = predictColors[colorIndex]
                    };
                }
            }

            // 显示高亮
            if (displayPredictShape)
            {
                for (var index = 0; index < predictShape.Count; index++)
                {
                    NodesManager.GetNodeColor(predictShape[index].position.x, predictShape[index].position.y).sprite =
                        predictColors[colorIndex];
                }
            }
        }

        /// <summary>
        /// 清空高亮结点
        /// </summary>
        public static void ClearPredictShape(Sprite backColor)
        {
            if (!object.ReferenceEquals(predictShape, null))
            {
                for (var index = 0; index < predictShape.Count; index++)
                {
                    NodesManager.GetNodeColor(predictShape[index].position.x,
                        predictShape[index].position.y).sprite = backColor;
                }
            }
        }
    }
}