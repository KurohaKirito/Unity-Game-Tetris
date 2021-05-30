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
        public static void RandomShape(in List<Sprite> colors, out ShapeInfo shapeInfo)
        {
            var color = RandomColor(colors);
            var type = RandomShapeType<EM_SHAPE_TYPE>();
            
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
            
            shapeInfo = new ShapeInfo
            {
                shape = tetrisShape,
                color = color,
                type = type
            };
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
        /// 随机一个枚举值
        /// </summary>
        /// <returns></returns>
        private static T RandomShapeType<T>() where T : Enum
        {
            var enums = Enum.GetValues(typeof(EM_SHAPE_TYPE));
            var typeIndex = Random.Range(0, enums.Length);
            return (T) enums.GetValue(typeIndex);
        }
    }
}