using System;
using System.Collections.Generic;
using Managers;
using Tetris.Data;
using Tetris.Shape;
using UnityEngine;
using Utility;
using JsonUtility = Utility.JsonUtility;

namespace Tetris.Manager
{
    /// <summary>
    /// 行为类型
    /// </summary>
    [Serializable]
    public enum EM_ACTION_TYPE
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    /// <summary>
    /// 形状类型
    /// </summary>
    [Serializable]
    public enum EM_SHAPE_TYPE
    {
        ShapeI,
        ShapeJ,
        ShapeL,
        ShapeO,
        ShapeS,
        ShapeT,
        ShapeZ
    }

    /// <summary>
    /// 形状信息
    /// </summary>
    public struct ShapeInfo
    {
        /// <summary>
        /// 形状内所有结点的信息
        /// </summary>
        public TetrisShape shape;

        /// <summary>
        /// 结点引用的精灵图
        /// </summary>
        public Sprite color;

        /// <summary>
        /// 形状类型
        /// </summary>
        public EM_SHAPE_TYPE type;
    }

    /// <summary>
    /// 二维边界
    /// </summary>
    [Serializable]
    public struct Border2
    {
        public int min;
        public int max;

        public Border2(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }

    /// <summary>
    /// 数据管理器
    /// </summary>
    public static class DataManager
    {
        /// <summary>
        /// 死亡线索引
        /// </summary>
        public static int DeathLineIndex { get; private set; }
        
        /// <summary>
        /// 玩家数据
        /// </summary>
        private static readonly PlayerData playerData = new PlayerData();

        /// <summary>
        /// UI 玩家分数
        /// </summary>
        private static int PlayerScore
        {
            get => int.Parse(playerData.score ?? "0");
            set
            {
                playerData.score = value.ToString();
                UIManager.Instance.score.text = playerData.score;
            }
        }

        /// <summary>
        /// UI 玩家等级
        /// </summary>
        private static int PlayerLevel
        {
            get => int.Parse(playerData.level ?? "0");
            set
            {
                playerData.level = value.ToString();
                UIManager.Instance.level.text = playerData.level;
            }
        }

        /// <summary>
        /// UI 玩家最高分
        /// </summary>
        private static SaveHighestScore PlayerHighestScore
        {
            set
            {
                var current = int.Parse(value.score);
                var highest = int.Parse(playerData.highestScore.score ?? "0");
                if (current != 0 && current <= highest)
                {
                    return;
                }
                playerData.highestScore = value;
                UIManager.Instance.highestScore.text = playerData.highestScore.score;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="deathLine">死亡判定线</param>
        public static void Init(int deathLine)
        {
            DeathLineIndex = deathLine - 1;

            // 初始化玩家数据
            playerData.playAreaColors = new List<List<int>>();
            for (var rowIndex = 0; rowIndex < NodesManager.RowCount; rowIndex++)
            {
                // 新建一行
                playerData.playAreaColors.Add(new List<int>());
                for (var columnIndex = 0; columnIndex < NodesManager.ColumnCount; columnIndex++)
                {
                    // 新建一个结点
                    playerData.playAreaColors[rowIndex].Add(-1);
                }
            }
        }
        
        /// <summary>
        /// 读取存档
        /// </summary>
        public static PlayerData LoadData()
        {
            var data = JsonUtility.LoadData<PlayerData>();
            if (data == null)
            {
                Debug.Log($"当前没有存档, 初始化游戏!");
                UpdateScoreLevel(init: true);
                return null;
            }

            UpdateScoreLevel(int.Parse(data.score));
            PlayerHighestScore = data.highestScore;

            if (data.playAreaColors != null)
            {
                for (var rowIndex = 0; rowIndex < NodesManager.RowCount; rowIndex++)
                {
                    for (var columnIndex = 0; columnIndex < NodesManager.ColumnCount; columnIndex++)
                    {
                        var colorIndex = data.playAreaColors[rowIndex][columnIndex];
                        var color = RandomManager.GetColorByIndex(colorIndex);
                        var nodeImage = NodesManager.GetNodeColor(rowIndex, columnIndex);
                        if (color != null)
                        {
                            nodeImage.sprite = color;
                        }
                    }
                }
            }

            LoadShape(data.shape, out RandomManager.currentTetrisShape);
            LoadShape(data.tipOne, out TipsManager.tipOne);
            LoadShape(data.tipTwo, out TipsManager.tipTwo);

            return data;
        }
        
        /// <summary>
        /// 读取存档中的结点信息并返回游戏中可用的结点信息
        /// </summary>
        private static void LoadShape(SaveShapeInfo shape, out ShapeInfo shapeInfo)
        {
            var color = RandomManager.GetColorByIndex(shape.colorIndex);
            var type = shape.type;

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
        /// 保存存档
        /// </summary>
        public static void SaveData()
        {
            UpdateSaveNodesData();
            JsonUtility.SaveData(playerData);
        }
        
        /// <summary>
        /// 更新存档数据
        /// </summary>
        private static void UpdateSaveNodesData()
        {
            // 玩家区域结点的颜色写入存档
            for (var rowIndex = 0; rowIndex < NodesManager.RowCount; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < NodesManager.ColumnCount; columnIndex++)
                {
                    playerData.playAreaColors[rowIndex][columnIndex] = RandomManager.GetColorIndexByName(
                        NodesManager.GetNodeColor(rowIndex, columnIndex).sprite.name);
                }
            }

            // 当前结点形状与颜色写入存档
            playerData.shape = new SaveShapeInfo(
                RandomManager.GetColorIndexByName(RandomManager.currentTetrisShape.color.name),
                RandomManager.currentTetrisShape.type);

            // 提示 One 结点形状与颜色写入存档
            playerData.tipOne = new SaveShapeInfo(
                RandomManager.GetColorIndexByName(TipsManager.tipOne.color.name),
                TipsManager.tipOne.type);

            // 提示 Two 结点形状与颜色写入存档
            playerData.tipTwo = new SaveShapeInfo(
                RandomManager.GetColorIndexByName(TipsManager.tipTwo.color.name),
                TipsManager.tipTwo.type);
        }

        /// <summary>
        /// 更新分数和等级
        /// </summary>
        /// <param name="number">增加的分数</param>
        /// <param name="reset">是否重置游戏</param>
        public static void UpdateScoreLevel(int number = 0, bool reset = false, bool init = false)
        {
            // TODO: 制定详细的等级规则, 目前就是 1000 分 1 个等级
            // 重置游戏, 分数清空
            if (reset)
            {
                number = -PlayerScore;
            }

            // 更新分数
            PlayerScore += number;
            
            // 更新最高分
            if (init || PlayerScore != 0)
            {
                PlayerHighestScore = new SaveHighestScore(PlayerScore.ToString(), DateTime.Now.ToLongTimeString());
            }

            // 更新等级
            PlayerLevel = PlayerScore / 1000 + 1;

            // 更新下落速度
            var curDownSpeed = 1f - PlayerLevel * 0.1f;
            GameManager.Instance.speedDown = Mathf.Max(curDownSpeed, 0) + 0.1f;

            // 更新定时器
            if (ClockUtility.GetClock(GameManager.Instance.clockDown) == null)
            {
                return;
            }
            
            // 用于解决此问题: 玩家一直按着下的时候发生了消除, 新的形状生成时, 加速效果会消失
            if (Input.GetKey(KeyCode.S))
            {
                ClockUtility.GetClock(GameManager.Instance.clockDown).life = GameManager.Instance.speedDown / 4;
            }
            else
            {
                ClockUtility.GetClock(GameManager.Instance.clockDown).life = GameManager.Instance.speedDown;
            }
        }
    }
}