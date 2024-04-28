using System.Collections.Generic;
using Tetris.Manager;

namespace Tetris.Data
{
    /// <summary>
    /// 历史最高分
    /// </summary>
    public struct HighestScore
    {
        /// <summary>
        /// 分数
        /// </summary>
        public string score;

        /// <summary>
        /// 日期
        /// </summary>
        public string date;

        /// <summary>
        /// 时间
        /// </summary>
        public string time;
    }

    /// <summary>
    /// 形状信息
    /// </summary>
    public struct ShapeInfo
    {
        /// <summary>
        /// 颜色, 精灵图索引
        /// </summary>
        public int colorIndex;

        /// <summary>
        /// 形状
        /// </summary>
        public EM_SHAPE_TYPE type;
    }

    /// <summary>
    /// 玩家数据类
    /// </summary>
    public class PlayerData
    {
        /// <summary>
        /// 玩家等级
        /// </summary>
        public string level;

        /// <summary>
        /// 玩家分数
        /// </summary>
        public string score;

        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public bool isGameOver;

        /// <summary>
        /// 历史最高分
        /// </summary>
        public HighestScore highestScore;

        /// <summary>
        /// 所有玩家区域的结点颜色索引
        /// </summary>
        public List<List<int>> playAreaColors;

        /// <summary>
        /// 当前形状
        /// </summary>
        public ShapeInfo shape;

        /// <summary>
        /// 提示 One
        /// </summary>
        public ShapeInfo tipOne;

        /// <summary>
        /// 提示 Two
        /// </summary>
        public ShapeInfo tipTwo;
    }
}