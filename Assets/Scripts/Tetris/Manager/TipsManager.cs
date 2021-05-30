using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Tetris.Utility;

namespace Tetris.Manager
{
    using TetrisRow = List<Image>;
    
    public static class TipsManager
    {
        /// <summary>
        /// 提示形状 One
        /// </summary>
        private static List<TetrisRow> tipOneNodes;

        /// <summary>
        /// 提示形状 Two
        /// </summary>
        private static List<TetrisRow> tipTwoNodes;
        
        /// <summary>
        /// 提示区域的行数
        /// </summary>
        public static int RowCount => tipOneNodes.Count;
        
        /// <summary>
        /// 提示区域的列数
        /// </summary>
        public static int ColumnCount => tipOneNodes[0].Count;
        
        /// <summary>
        /// 提示 One
        /// </summary>
        public static ShapeInfo tipOne;
        
        /// <summary>
        /// 提示 Two
        /// </summary>
        public static ShapeInfo tipTwo;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init(Sprite backColor, Transform tipOneRows, Transform tipTwoRows)
        {
            tipOneNodes = NodesUtility.InitAreaNodes(backColor, tipOneRows);
            tipTwoNodes = NodesUtility.InitAreaNodes(backColor, tipTwoRows);
        }

        /// <summary>
        /// 刷新显示提示 One
        /// </summary>
        public static void RefreshTipOneDisplay(Sprite backColor)
        {
            RefreshTip(backColor, tipOneNodes, tipOne);
        }
        
        /// <summary>
        /// 刷新显示提示 Two
        /// </summary>
        public static void RefreshTipTwoDisplay(Sprite backColor)
        {
            RefreshTip(backColor, tipTwoNodes, tipTwo);
        }

        /// <summary>
        /// 刷新提示
        /// </summary>
        private static void RefreshTip(Sprite backColor, in List<TetrisRow> tipNodes, ShapeInfo tip)
        {
            // 清空所有结点
            for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
                {
                    tipNodes[rowIndex][columnIndex].sprite = backColor;
                }
            }

            // 获取颜色
            var color = tip.color;

            // 获取形状
            switch (tip.type)
            {
                case EM_SHAPE_TYPE.ShapeI:
                    tipNodes[0][0].sprite = color;
                    tipNodes[0][1].sprite = color;
                    tipNodes[0][2].sprite = color;
                    tipNodes[0][3].sprite = color;
                    break;
                case EM_SHAPE_TYPE.ShapeJ:
                    tipNodes[1][0].sprite = color;
                    tipNodes[0][0].sprite = color;
                    tipNodes[0][1].sprite = color;
                    tipNodes[0][2].sprite = color;
                    break;
                case EM_SHAPE_TYPE.ShapeL:
                    tipNodes[0][0].sprite = color;
                    tipNodes[0][1].sprite = color;
                    tipNodes[0][2].sprite = color;
                    tipNodes[1][2].sprite = color;
                    break;
                case EM_SHAPE_TYPE.ShapeO:
                    tipNodes[1][0].sprite = color;
                    tipNodes[1][1].sprite = color;
                    tipNodes[0][0].sprite = color;
                    tipNodes[0][1].sprite = color;
                    break;
                case EM_SHAPE_TYPE.ShapeS:
                    tipNodes[0][0].sprite = color;
                    tipNodes[0][1].sprite = color;
                    tipNodes[1][1].sprite = color;
                    tipNodes[1][2].sprite = color;
                    break;
                case EM_SHAPE_TYPE.ShapeT:
                    tipNodes[0][0].sprite = color;
                    tipNodes[0][1].sprite = color;
                    tipNodes[1][1].sprite = color;
                    tipNodes[0][2].sprite = color;
                    break;
                case EM_SHAPE_TYPE.ShapeZ:
                    tipNodes[1][0].sprite = color;
                    tipNodes[1][1].sprite = color;
                    tipNodes[0][1].sprite = color;
                    tipNodes[0][2].sprite = color;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// 获取提示 One 结点的颜色
        /// </summary>
        /// <param name="rowIndex">行坐标</param>
        /// <param name="columnIndex">列坐标</param>
        /// <returns></returns>
        public static Image GetTipOneNodeColor(int rowIndex, int columnIndex)
        {
            return tipOneNodes[rowIndex][columnIndex];
        }
        
        /// <summary>
        /// 获取提示 Two 结点的颜色
        /// </summary>
        /// <param name="rowIndex">行坐标</param>
        /// <param name="columnIndex">列坐标</param>
        /// <returns></returns>
        public static Image GetTipTwoNodeColor(int rowIndex, int columnIndex)
        {
            return tipTwoNodes[rowIndex][columnIndex];
        }
    }
}