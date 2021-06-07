using System.Collections.Generic;
using Tetris.Manager;
using Tetris.Utility;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using JsonUtility = Utility.JsonUtility;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private const float SPEED_DOWN = 0.04f;
        private const float SPEED_LEFT = 0.15f;
        private const float SPEED_RIGHT = 0.15f;

        private int clockDown;
        private int clockLeft;
        private int clockRight;

        [Header("日志开关")]
        public bool isLogEnable;

        [Header("游戏状态")]
        public bool isPlay;

        [Header("游戏状态")]
        public Image statusSwitch;
        public Sprite statusPlay;
        public Sprite statusPause;

        [Header("游戏阶段")]
        public Transform startArea;
        public Transform gameArea;
        public Transform overArea;

        [Header("背景色")]
        public Sprite backColor;

        [Header("结点颜色")]
        public List<Sprite> predictColors;

        [Header("高亮颜色")]
        public List<Sprite> colors;

        [Header("所有行的父结点")]
        [SerializeField] private Transform randomRowsParent;
        [SerializeField] private Transform playerRowsParent;
        [SerializeField] private Transform tipOneRowsParent;
        [SerializeField] private Transform tipTwoRowsParent;

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
        /// 初始化
        /// </summary>
        private void Start()
        {
            // 初始化存档路径以及文件信息
            JsonUtility.Init();

            // 初始化预判高亮器所需要的全部颜色
            PredictManager.Init(predictColors);

            // 初始化随机器所需要的全部颜色
            RandomManager.Init(colors, backColor);

            // 初始化结点管理器, 将所有玩家区域的 Transform 读入内存, 并初始化所有结点为背景色
            NodesManager.Init(backColor, playerRowsParent, randomRowsParent);

            // 初始化提示管理器, 将所有提示区域的 Transform 读入内存, 并初始化所有结点为背景色
            TipsManager.Init(backColor, tipOneRowsParent, tipTwoRowsParent);

            // 初始化数据管理器, 用于存档以及读档
            DataManager.Init(playerRowsParent.childCount);

            // 初始化时钟
            gameObject.AddComponent<ClockUtility>();

            // 读取存档, 若存档为空, 则创建初始形状和两个提示的形状
            var save = DataManager.LoadData();
            if (save == null || save.isGameOver)
            {
                RandomManager.InstantiateOriginShapes();
            }
        }

        /// <summary>
        /// 帧更新
        /// </summary>
        private void Update()
        {
            if (isPlay && !DataManager.FlagGameOver)
            {
                InputUtility.OnUpdate();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitGame();
            }
        }

        /// <summary>
        /// 注册输入事件
        /// </summary>
        private void RegisterInput()
        {
            InputUtility.rotate = () =>
            {
                if (isPlay)
                {
                    RotateUtility.Rotate(RandomManager.currentTetrisShape.shape, backColor);
                }
            };

            InputUtility.moveLeft = () =>
            {
                if (isPlay)
                {
                    MoveUtility.MoveLeft(RandomManager.currentTetrisShape.shape, backColor);
                    ClockUtility.UpdateClock(clockLeft, SPEED_LEFT, 0, -1);
                }
            };
            InputUtility.cancelMoveLeft = () =>
            {
                if (isPlay)
                {
                    ClockUtility.UpdateClock(clockLeft, 0);
                }
            };

            InputUtility.moveRight = () =>
            {
                if (isPlay)
                {
                    MoveUtility.MoveRight(RandomManager.currentTetrisShape.shape, backColor);
                    ClockUtility.UpdateClock(clockRight, SPEED_RIGHT, 0, -1);
                }
            };
            InputUtility.cancelMoveRight = () =>
            {
                if (isPlay)
                {
                    ClockUtility.UpdateClock(clockRight, 0);
                }
            };

            InputUtility.moveDown = () =>
            {
                if (isPlay)
                {
                    MoveUtility.MoveDown(RandomManager.currentTetrisShape.shape, backColor);
                    ClockUtility.UpdateClock(clockDown, SPEED_DOWN, 0, -1);
                }
            };
            InputUtility.cancelMoveDown = () =>
            {
                if (isPlay)
                {
                    ClockUtility.UpdateClock(clockDown, DataManager.GetDownSpeed());
                }
            };
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            // 开始游戏
            SetPlay();
            DataManager.FlagGameOver = false;
            gameArea.gameObject.SetActive(true);
            startArea.gameObject.SetActive(false);
            
            // 立即显示高亮提示
            PredictManager.UpdatePredictShape(backColor, RandomManager.currentTetrisShape.shape.GetNodesInfo());

            // 立即显示形状
            TipsManager.RefreshTipOneDisplay(backColor);
            TipsManager.RefreshTipTwoDisplay(backColor);
            NodesUtility.RefreshCurrentShapeDisplay(backColor);

            // 创建定时器
            clockDown = ClockUtility.RegisterClock(DataManager.GetDownSpeed(), -1,
                () => MoveUtility.MoveDown(RandomManager.currentTetrisShape.shape, backColor));
            clockLeft = ClockUtility.RegisterClock(SPEED_LEFT, 0,
                () => MoveUtility.MoveLeft(RandomManager.currentTetrisShape.shape, backColor));
            clockRight = ClockUtility.RegisterClock(SPEED_RIGHT, 0,
                () => MoveUtility.MoveRight(RandomManager.currentTetrisShape.shape, backColor));

            // 初始化玩家输入响应事件
            RegisterInput();
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        /// <summary>
        /// 切换游戏状态
        /// Play Pause
        /// </summary>
        public void ChangeGameStatus()
        {
            if (!DataManager.FlagGameOver)
            {
                if (Time.timeScale == 0)
                {
                    SetPlay();
                }
                else
                {
                    SetPause();
                }
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        private void SetPause()
        {
            isPlay = false;
            Time.timeScale = 0;
            statusSwitch.sprite = statusPlay;
        }

        /// <summary>
        /// 继续
        /// </summary>
        private void SetPlay()
        {
            isPlay = true;
            Time.timeScale = 1;
            statusSwitch.sprite = statusPause;
        }

        /// <summary>
        /// 设置游戏结束
        /// </summary>
        public void GameOver()
        {
            // 游戏结束
            SetPause();
            DataManager.FlagGameOver = true;

            // 展示 Game Over 页面
            overArea.gameObject.SetActive(true);

            // 存档
            DataManager.SaveData();
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        public void ReStartGame()
        {
            // 游戏重开
            SetPlay();
            DataManager.FlagGameOver = false;

            // 隐藏 Game Over 界面
            overArea.gameObject.SetActive(false);

            // 重置全部结点颜色
            NodesUtility.ResetAllAreaNodes(backColor);

            // 重置分数, 等级, 速度
            DataManager.UpdateScoreLevel(reset: true);

            // 重新生成 3 个形状
            RandomManager.InstantiateOriginShapes();

            // 立即显示高亮
            PredictManager.UpdatePredictShape(backColor, RandomManager.currentTetrisShape.shape.GetNodesInfo());

            // 立即刷新三个形状的显示
            TipsManager.RefreshTipOneDisplay(backColor);
            TipsManager.RefreshTipTwoDisplay(backColor);
            NodesUtility.RefreshCurrentShapeDisplay(backColor);
        }

        /// <summary>
        /// 更新下落速度
        /// </summary>
        public void UpdateDownSpeed(float speed)
        {
            ClockUtility.UpdateClock(clockDown, Input.GetKey(KeyCode.S) ? SPEED_DOWN : speed);
        }
    }
}