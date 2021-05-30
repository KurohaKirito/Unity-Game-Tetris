using System;
using System.Collections.Generic;
using Tetris.Manager;

namespace Tetris.Data
{
    /// <summary>
    /// 历史最高分
    /// </summary>
    public struct SaveHighestScore
    {
        /// <summary>
        /// 分数
        /// </summary>
        public string score;
        
        /// <summary>
        /// 日期和时间
        /// </summary>
        public string date;

        public SaveHighestScore(string scoreStr, string dateStr)
        {
            score = scoreStr;
            date = dateStr;
        }
    }
    
    /// <summary>
    /// 形状信息
    /// </summary>
    public readonly struct SaveShapeInfo
    {
        /// <summary>
        /// 颜色, 精灵图索引
        /// </summary>
        public readonly int colorIndex;

        /// <summary>
        /// 形状
        /// </summary>
        public readonly EM_SHAPE_TYPE type;

        public SaveShapeInfo(int colorIndex, EM_SHAPE_TYPE type)
        {
            this.colorIndex = colorIndex;
            this.type = type;
        }
    }
    
    /// <summary>
    /// 玩家数据类
    /// 保存玩家此时的数据, 包括
    /// 1. 整个方块系统中所有方块的颜色
    /// 2. 玩家目前的等级, 等级通过计算公式来决定下落速度
    /// 3. 玩家目前的分数
    /// 4. 玩家的历史分数以及历史分数的时间
    /// 5. 随机系统的下两个生成结点 tip
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
        /// 历史最高分
        /// </summary>
        public SaveHighestScore highestScore;
        
        /// <summary>
        /// 所有玩家区域的结点颜色索引
        /// </summary>
        public List<List<int>> playAreaColors;
        
        /// <summary>
        /// 当前形状
        /// </summary>
        public SaveShapeInfo shape;
        
        /// <summary>
        /// 提示 One
        /// </summary>
        public SaveShapeInfo tipOne;
        
        /// <summary>
        /// 提示 Two
        /// </summary>
        public SaveShapeInfo tipTwo;
    }
}