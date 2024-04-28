using System;
using System.Collections.Generic;
using Tetris.Manager;
using Tetris.Shape;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tetris.Utility
{
    public static class RandomUtility
    {
        /// <summary>
        /// 随机一个形状并返回
        /// </summary>
        public static void RandomShape(in List<Sprite> colors, ref ShapeInfo shapeInfo)
        {
            var color = RandomColor(colors);
            var type = RandomShapeTypeEasyVersion();

            while (type == TipsManager.tipOne.type && type == TipsManager.tipTwo.type)
            {
                type = RandomShapeType<EM_SHAPE_TYPE>();
            }

            var tetrisShape = CreateShape(type, color);
            shapeInfo.shape = tetrisShape;
            shapeInfo.color = color;
            shapeInfo.type = type;
        }

        /// <summary>
        /// 创建执行类型和颜色的形状
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="color">颜色</param>
        /// <returns>形状</returns>
        public static TetrisShape CreateShape(EM_SHAPE_TYPE type, Sprite color)
        {
            TetrisShape tetrisShape = type switch
            {
                EM_SHAPE_TYPE.ShapeI => new TetrisShapeI(NodesManager.BirthPosition, color),
                EM_SHAPE_TYPE.ShapeJ => new TetrisShapeJ(NodesManager.BirthPosition, color),
                EM_SHAPE_TYPE.ShapeL => new TetrisShapeL(NodesManager.BirthPosition, color),
                EM_SHAPE_TYPE.ShapeO => new TetrisShapeO(NodesManager.BirthPosition, color),
                EM_SHAPE_TYPE.ShapeS => new TetrisShapeS(NodesManager.BirthPosition, color),
                EM_SHAPE_TYPE.ShapeT => new TetrisShapeT(NodesManager.BirthPosition, color),
                EM_SHAPE_TYPE.ShapeZ => new TetrisShapeZ(NodesManager.BirthPosition, color),
                _ => new TetrisShapeI(NodesManager.BirthPosition, color)
            };

            return tetrisShape;
        }

        /// <summary>
        /// 随机一个颜色
        /// </summary>
        /// <returns></returns>
        private static Sprite RandomColor(in List<Sprite> colors)
        {
            var colorIndex = Random.Range(0, colors.Count);
            return colors[colorIndex];
        }

        /// <summary>
        /// 随机一个形状类型
        /// 等概率生成
        /// </summary>
        /// <returns></returns>
        private static T RandomShapeType<T>() where T : Enum
        {
            var enums = Enum.GetValues(typeof(EM_SHAPE_TYPE));
            var typeIndex = Random.Range(0, enums.Length);
            return (T) enums.GetValue(typeIndex);
        }

        /// <summary>
        /// 随机一个形状类型
        /// 简易版本
        /// </summary>
        /// <returns></returns>
        private static EM_SHAPE_TYPE RandomShapeTypeEasyVersion()
        {
            EM_SHAPE_TYPE type;
            var number = Random.Range(0, 100);

            // I : 20
            if (number >= 0 && number < 20)
            {
                type = EM_SHAPE_TYPE.ShapeI;
            }

            // J : 10
            else if (number >= 20 && number < 30)
            {
                type = EM_SHAPE_TYPE.ShapeJ;
            }

            // L : 10
            else if (number >= 30 && number < 40)
            {
                type = EM_SHAPE_TYPE.ShapeL;
            }

            // O : 16
            else if (number >= 40 && number < 56)
            {
                type = EM_SHAPE_TYPE.ShapeO;
            }

            // S : 14
            else if (number >= 56 && number < 70)
            {
                type = EM_SHAPE_TYPE.ShapeS;
            }

            // T : 16
            else if (number >= 70 && number < 86)
            {
                type = EM_SHAPE_TYPE.ShapeT;
            }

            // Z : 14
            else
            {
                type = EM_SHAPE_TYPE.ShapeZ;
            }

            return type;
        }
    }
}