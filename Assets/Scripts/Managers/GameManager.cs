using System.Collections.Generic;
using Tetris.Manager;
using Tetris.Utility;
using UnityEngine;
using Utility;
using JsonUtility = Utility.JsonUtility;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private bool isGaming;
        
        [Header("游戏阶段")]
        public Transform startArea;
        public Transform gameArea;
        public Transform overArea;
        
        [Header("颜色")]
        public Sprite backColor;
        public List<Sprite> colors;
        
        [Header("所有行的父结点")]
        [SerializeField]
        private Transform randomRowsParent;
        [SerializeField]
        private Transform playerRowsParent;
        [SerializeField]
        private Transform tipOneRowsParent;
        [SerializeField]
        private Transform tipTwoRowsParent;

        [Header("速度设置")]
        public float speedDown = 0.2f;
        private const float SPEED_LEFT = 0.2f;
        private const float SPEED_RIGHT = 1f;

        [Header("时钟设置")]
        public int clockDown;
        private int clockLeft;
        private int clockRight;
        
        [Header("当前的形状")]
        public ShapeInfo tetrisShape;
        
        /// <summary>
        /// 单例模式
        /// </summary>
        private static GameManager instance;
        /// <summary>
        /// 单例模式
        /// </summary>
        public static GameManager Instance
        {
            get => instance;
            private set
            {
                if (instance == null)
                {
                    instance = value;
                }
            }
        }
        /// <summary>
        /// 单例模式
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            // 开始游戏
            isGaming = true;
            gameArea.gameObject.SetActive(true);
            startArea.gameObject.SetActive(false);

            // 初始化存档路径以及文件信息
            JsonUtility.Init();
            
            // 初始化随机器所需要的全部颜色
            RandomManager.Init(colors, backColor);
            
            // 初始化结点管理器, 将所有玩家区域的 Transform 读入内存, 并初始化所有结点为背景色
            NodesManager.Init(backColor, playerRowsParent, randomRowsParent);
            
            // 初始化提示管理器, 将所有提示区域的 Transform 读入内存, 并初始化所有结点为背景色
            TipsManager.Init(backColor, tipOneRowsParent, tipTwoRowsParent);
            
            // 初始化数据管理器, 用于存档以及读档
            DataManager.Init(playerRowsParent.childCount);
            
            // 读取存档, 若存档为空, 则创建初始形状和两个提示的形状
            if (DataManager.LoadData() == null)
            {
                RandomManager.InstantiateOriginShapes();
            }
            tetrisShape = RandomManager.currentTetrisShape;

            // 立即显示形状
            NodesUtility.RefreshNodeDisplay(tetrisShape.shape, backColor);
            TipsManager.RefreshTipOneDisplay(backColor);
            TipsManager.RefreshTipTwoDisplay(backColor);
            
            // 创建定时器
            clockDown = ClockUtility.RegisterClock(speedDown, () => MoveUtility.MoveDown(tetrisShape.shape, backColor), -1);
            clockLeft = ClockUtility.RegisterClock(speedDown, () => MoveUtility.MoveLeft(tetrisShape.shape, backColor), 0);
            clockRight = ClockUtility.RegisterClock(speedDown, () => MoveUtility.MoveRight(tetrisShape.shape, backColor), 0);
            
            // 初始化时钟
            gameObject.AddComponent<ClockUtility>();
            
            // 初始化玩家输入响应事件
            RegisterInput();
            
            // 日志
            Debug.Log($"游戏初始化完毕! 游玩区域为 {NodesManager.RowCount} 行 {NodesManager.ColumnCount} 列!");
        }

        private void Update()
        {
            if (isGaming)
            {
                InputUtility.OnUpdate();
            }
        }
        
        /// <summary>
        /// 注册输入事件
        /// </summary>
        private void RegisterInput()
        {
            InputUtility.rotate = () =>
            {
                RotateUtility.Rotate(tetrisShape.shape, backColor);
            };
            InputUtility.moveLeft = () =>
            {
                MoveUtility.MoveLeft(tetrisShape.shape, backColor);
                ClockUtility.GetClock(clockLeft).life = SPEED_LEFT;
                ClockUtility.GetClock(clockLeft).timed = 0f;
                ClockUtility.GetClock(clockLeft).count = -1;
            };
            InputUtility.cancelMoveLeft = () =>
            {
                ClockUtility.GetClock(clockLeft).life = speedDown;
                ClockUtility.GetClock(clockLeft).count = 0;
            };
            InputUtility.moveRight = () =>
            {
                MoveUtility.MoveRight(tetrisShape.shape, backColor);
                ClockUtility.GetClock(clockRight).life = SPEED_RIGHT;
                ClockUtility.GetClock(clockRight).timed = 0f;
                ClockUtility.GetClock(clockRight).count = -1;
            };
            InputUtility.cancelMoveRight = () =>
            {
                ClockUtility.GetClock(clockRight).life = speedDown;
                ClockUtility.GetClock(clockRight).count = 0;
            };
            InputUtility.moveDown = () =>
            {
                MoveUtility.MoveDown(tetrisShape.shape, backColor);
                ClockUtility.GetClock(clockDown).life = speedDown / 4;
                ClockUtility.GetClock(clockDown).timed = 0;
            };
            InputUtility.cancelMoveDown = () =>
            {
                ClockUtility.GetClock(clockDown).life = speedDown;
            };
        }

        /// <summary>
        /// 设置游戏结束
        /// </summary>
        public void GameOver()
        {
            // 游戏结束
            isGaming = false;
            Time.timeScale = 0;
            
            // 展示 Game Over 页面
            overArea.gameObject.SetActive(true);
            
            // 重置全部结点颜色
            NodesUtility.ResetAreaNodes(backColor);

            // 存档
            DataManager.SaveData();
        }
        
        /// <summary>
        /// 重新开始游戏
        /// </summary>
        public void ReStartGame()
        {
            // 游戏重开
            isGaming = true;
            Time.timeScale = 1;
            
            // 隐藏 Game Over 界面
            overArea.gameObject.SetActive(false);
            
            // 重置分数, 等级, 速度
            DataManager.UpdateScoreLevel(reset: true);
            
            // 重置三个形状
            
            
            // 立即刷新三个形状的显示
            TipsManager.RefreshTipOneDisplay(backColor);
            TipsManager.RefreshTipTwoDisplay(backColor);
            NodesUtility.RefreshNodeDisplay(tetrisShape.shape, backColor);
        }
    }
}