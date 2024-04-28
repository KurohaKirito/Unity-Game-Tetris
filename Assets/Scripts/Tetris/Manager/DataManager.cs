using System;
using System.Collections.Generic;
using Managers;
using Tetris.Data;
using Tetris.Shape;
using Tetris.Utility;
using UnityEngine;
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
        private static PlayerData playerData = new PlayerData();

        /// <summary>
        /// 游戏状态
        /// </summary>
        public static bool FlagGameOver
        {
            get => playerData.isGameOver;
            set => playerData.isGameOver = value;
        }

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
        private static HighestScore PlayerHighestScore
        {
            get => string.IsNullOrEmpty(playerData.highestScore.score)
                ? new HighestScore
                {
                    score = "0",
                    date = DateTime.Now.ToString("yyyy/MM/dd"),
                    time = DateTime.Now.ToLongTimeString()
                }
                : playerData.highestScore;
            set
            {
                playerData.highestScore = value;
                UIManager.Instance.highestScore.text = playerData.highestScore.score;
                UIManager.Instance.highestScoreDate.text = playerData.highestScore.date;
                UIManager.Instance.highestScoreTime.text = playerData.highestScore.time;
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
                if (GameManager.Instance.isLogEnable)
                {
                    Debug.Log("当前没有存档, 初始化游戏!");
                }

                UpdateScoreLevel(init: true);
            }
            else if (data.isGameOver)
            {
                if (GameManager.Instance.isLogEnable)
                {
                    Debug.Log("检测到存档, 但是游戏已经结束, 重置游戏!");
                }

                playerData = data;
                GameManager.Instance.ReStartGame();
            }
            else
            {
                if (GameManager.Instance.isLogEnable)
                {
                    Debug.Log("检测到存档, 恢复存档数据!");
                }

                playerData = data;
                UpdateScoreLevel(loadSave: true);
                ResumeNodes(data);
            }

            return data;
        }

        /// <summary>
        /// 从存档中恢复全部的结点
        /// </summary>
        /// <param name="data"></param>
        private static void ResumeNodes(PlayerData data)
        {
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
        }

        /// <summary>
        /// 读取存档中的结点信息并返回游戏中可用的结点信息
        /// </summary>
        private static void LoadShape(Data.ShapeInfo shape, out ShapeInfo shapeInfo)
        {
            var color = RandomManager.GetColorByIndex(shape.colorIndex);
            var type = shape.type;

            shapeInfo = new ShapeInfo
            {
                shape = RandomUtility.CreateShape(type, color),
                color = color,
                type = type
            };
        }

        /// <summary>
        /// 保存存档
        /// 存档是必须保证 "当前结点" 和 "高亮结点" 没有显示
        /// </summary>
        public static void SaveData()
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
            playerData.shape = new Data.ShapeInfo
            {
                colorIndex = RandomManager.GetColorIndexByName(RandomManager.currentTetrisShape.color.name),
                type = RandomManager.currentTetrisShape.type
            };

            // 提示 One 结点形状与颜色写入存档
            playerData.tipOne = new Data.ShapeInfo
            {
                colorIndex = RandomManager.GetColorIndexByName(TipsManager.tipOne.color.name),
                type = TipsManager.tipOne.type
            };

            // 提示 Two 结点形状与颜色写入存档
            playerData.tipTwo = new Data.ShapeInfo
            {
                colorIndex = RandomManager.GetColorIndexByName(TipsManager.tipTwo.color.name),
                type = TipsManager.tipTwo.type
            };

            // 写入存档数据
            JsonUtility.SaveData(playerData);
        }

        /// <summary>
        /// 更新分数和等级以及 UI 显示
        /// </summary>
        /// <param name="score">增加的分数</param>
        /// <param name="reset">是否重置游戏</param>
        /// <param name="init">是否初始化游戏</param>
        /// <param name="loadSave">是否读档</param>
        public static void UpdateScoreLevel(int score = 0, bool init = false, bool reset = false, bool loadSave = false)
        {
            // 更新当前分数
            PlayerScore += reset ? -PlayerScore : score;

            // 更新最高分
            if (init)
            {
                UpdateHighestScore(useNow: true);
            }
            else if (loadSave)
            {
                UpdateHighestScore(useSave: true);
            }
            else if (reset)
            {
                UpdateHighestScore(useSave: true);
            }
            else if (PlayerScore > int.Parse(PlayerHighestScore.score))
            {
                UpdateHighestScore(useNow: true);
            }

            // 更新等级
            PlayerLevel = GetPlayerLevel(PlayerScore);

            // 更新下落速度
            GameManager.Instance.UpdateDownSpeed(GetDownSpeed());
        }

        /// <summary>
        /// 更新最高分
        /// </summary>
        /// <param name="useSave">读取存档分数</param>
        /// <param name="useNow">使用当前分数</param>
        private static void UpdateHighestScore(bool useSave = false, bool useNow = false)
        {
            if (useSave)
            {
                PlayerHighestScore = PlayerHighestScore;
            }

            if (useNow)
            {
                PlayerHighestScore = new HighestScore
                {
                    score = PlayerScore.ToString(),
                    date = DateTime.Now.ToString("yyyy/MM/dd"),
                    time = DateTime.Now.ToLongTimeString()
                };
            }
        }

        /// <summary>
        /// 通过玩家分数计算得出玩家等级
        /// </summary>
        /// <param name="score">玩家分数</param>
        /// <returns>玩家等级</returns>
        private static int GetPlayerLevel(int score)
        {
            int level;

            if (score <= 1000)
            {
                level = score / 100;
            }
            else if (score <= 2500)
            {
                level = (score - 1000) / 150 + 10;
            }
            else if (score <= 4500)
            {
                level = (score - 2500) / 200 + 20;
            }
            else if (score <= 7000)
            {
                level = (score - 4500) / 250 + 30;
            }
            else if (score <= 10000)
            {
                level = (score - 7000) / 300 + 40;
            }
            else
            {
                level = (score - 10000) / 350 + 50;
            }

            return level;
        }

        /// <summary>
        /// 获取当前的下落速度
        /// </summary>
        /// <returns>下落速度</returns>
        public static float GetDownSpeed()
        {
            var level = PlayerLevel;
            float speed;

            if (level <= 10)
            {
                speed = 0.69f;
            }
            else if (level <= 20)
            {
                speed = 0.5f;
            }
            else if (level <= 30)
            {
                speed = 0.36f;
            }
            else if (level <= 40)
            {
                speed = 0.26f;
            }
            else if (level <= 50)
            {
                speed = 0.19f;
            }
            else if (level <= 60)
            {
                speed = 0.14f;
            }
            else
            {
                speed = 0.1f;
            }

            return speed;
        }

        /// <summary>
        /// 计算得出当前消除操作得到的分数
        /// </summary>
        /// <param name="rowNumber">消除的行数</param>
        /// <returns>获得的分数</returns>
        public static int GetScoreOnClear(int rowNumber)
        {
            var baseScore = rowNumber switch
            {
                1 => 10,
                2 => 30,
                3 => 60,
                4 => 100,
                _ => 10
            };

            return baseScore + PlayerLevel * rowNumber;
        }
    }
}