using System.Collections.Generic;
using System.Linq;
using Tetris.Utility;
using UnityEngine;

namespace Tetris.Manager
{
    /// <summary>
    /// 随机生成形状管理类
    /// </summary>
    public static class RandomManager
    {
        /// <summary>
        /// 当前形状
        /// </summary>
        public static ShapeInfo currentTetrisShape;

        /// <summary>
        /// 所有的结点颜色
        /// </summary>
        private static List<Sprite> forwardColors;

        /// <summary>
        /// 背景色
        /// </summary>
        private static Sprite backColor;

        /// <summary>
        /// 颜色字典
        /// </summary>
        private static readonly Dictionary<string, int> colorDictionary = new Dictionary<string, int>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="nodeColorList">所有的结点颜色</param>
        /// <param name="backNodeColor">背景色</param>
        public static void Init(List<Sprite> nodeColorList, Sprite backNodeColor)
        {
            if (nodeColorList == null || nodeColorList.Count <= 0)
            {
                return;
            }

            forwardColors = nodeColorList;
            backColor = backNodeColor;

            for (var i = 0; i < forwardColors.Count; i++)
            {
                colorDictionary.Add(forwardColors[i].name, i);
            }
        }

        /// <summary>
        /// 生成初始形状
        /// </summary>
        public static void InstantiateOriginShapes()
        {
            RandomUtility.RandomShape(forwardColors, ref currentTetrisShape);
            RandomUtility.RandomShape(forwardColors, ref TipsManager.tipOne);
            RandomUtility.RandomShape(forwardColors, ref TipsManager.tipTwo);
        }

        /// <summary>
        /// 生成下一个形状
        /// </summary>
        public static void NextShape()
        {
            currentTetrisShape = TipsManager.tipOne;
            TipsManager.tipOne = TipsManager.tipTwo;
            RandomUtility.RandomShape(forwardColors, ref TipsManager.tipTwo);
        }

        /// <summary>
        /// 获取颜色索引
        /// </summary>
        /// <param name="colorName">颜色名称</param>
        /// <returns></returns>
        public static int GetColorIndexByName(string colorName)
        {
            if (colorDictionary.ContainsKey(colorName))
            {
                return colorDictionary[colorName];
            }

            // 背景返回 -1
            return -1;
        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="colorIndex">颜色索引</param>
        /// <returns></returns>
        public static Sprite GetColorByIndex(int colorIndex)
        {
            if (colorIndex >= 0 && colorIndex < forwardColors.Count)
            {
                return forwardColors[colorIndex];
            }

            return backColor;
        }

        /// <summary>
        /// 判断颜色是否是结点颜色
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public static bool IsNodeColor(Sprite color)
        {
            return Enumerable.Contains(forwardColors, color);
        }

        /// <summary>
        /// 判断颜色是否是结点颜色
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public static bool IsBackColor(Sprite color)
        {
            return backColor.Equals(color);
        }
    }
}