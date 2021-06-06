using System;
using System.Diagnostics;
using Tetris.Manager;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Utility
{
    public enum EM_MOUSE_TYPE
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    public static class InputUtility
    {
        #region 常量

        /// <summary>
        /// 触发滑动的最小距离 X
        /// 单位: 像素
        /// </summary>
        private const int MIN_X = 36;

        /// <summary>
        /// 触发滑动的最小距离 Y
        /// 单位: 像素
        /// </summary>
        private const int MIN_Y = 36;

        /// <summary>
        /// 触发滑动的最短时长
        /// </summary>
        private const float MIN_TIME = 0.005f;

        #endregion

        #region 字段

        /// <summary>
        /// 鼠标检测类型
        /// </summary>
        private static EM_MOUSE_TYPE mouseType;

        /// <summary>
        /// 玩家初次点击的位置
        /// 仅按下时更新, 滑动期间不更新
        /// </summary>
        private static Vector3 firstPos;

        /// <summary>
        /// 玩家当前点击的位置
        /// 滑动期间实时更新, 松开后不再更新
        /// </summary>
        private static Vector3 currentPos;

        /// <summary>
        /// 记录玩家滑动屏幕的时间长短
        /// </summary>
        private static float timer;

        #endregion

        #region 委托

        public static Action rotate;

        public static Action moveDown;
        public static Action cancelMoveDown;

        public static Action moveLeft;
        public static Action cancelMoveLeft;

        public static Action moveRight;
        public static Action cancelMoveRight;

        #endregion

        private static bool isPressed;
        private static EM_ACTION_TYPE actionType;
        private static bool rotateFlag;
        private static bool leftClock;
        private static bool rightClock;
        private static bool downClock;

        public static void OnUpdate()
        {
            MouseCheck();
            TouchCheck();
            KeyBoardCheck();

            if (isPressed)
            {
                switch (actionType)
                {
                    case EM_ACTION_TYPE.Up:
                        if (!rotateFlag)
                        {
                            rotate?.Invoke();
                            cancelMoveDown?.Invoke();
                            cancelMoveLeft?.Invoke();
                            cancelMoveRight?.Invoke();
                            rotateFlag = true;
                        }

                        break;

                    case EM_ACTION_TYPE.Down:
                        if (!downClock)
                        {
                            moveDown?.Invoke();
                            cancelMoveLeft?.Invoke();
                            cancelMoveRight?.Invoke();
                            downClock = true;
                        }

                        break;
                    case EM_ACTION_TYPE.Left:
                        if (!leftClock)
                        {
                            moveLeft?.Invoke();
                            cancelMoveDown?.Invoke();
                            cancelMoveRight?.Invoke();
                            leftClock = true;
                        }

                        break;
                    case EM_ACTION_TYPE.Right:
                        if (!rightClock)
                        {
                            moveRight?.Invoke();
                            cancelMoveLeft?.Invoke();
                            cancelMoveDown?.Invoke();
                            rightClock = true;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                if (rotateFlag)
                {
                    rotateFlag = false;
                }

                if (downClock)
                {
                    cancelMoveDown?.Invoke();
                    downClock = false;
                }

                if (leftClock)
                {
                    cancelMoveLeft?.Invoke();
                    leftClock = false;
                }

                if (rightClock)
                {
                    cancelMoveRight?.Invoke();
                    rightClock = false;
                }
            }
        }

        /// <summary>
        /// Window: 鼠标滑动检测
        /// </summary>
        [Conditional("UNITY_STANDALONE_WIN")]
        private static void MouseCheck()
        {
            // 记录玩家鼠标点击时的位置和时间信息
            if (Input.GetMouseButtonDown((int) mouseType))
            {
                firstPos = Input.mousePosition;
                currentPos = Input.mousePosition;
                timer = 0;
            }

            // 记录玩家鼠标滑动时的位置和时间信息
            if (Input.GetMouseButton((int) mouseType))
            {
                currentPos = Input.mousePosition;
                timer += Time.deltaTime;

                if (timer > MIN_TIME)
                {
                    if (currentPos.x < firstPos.x - MIN_X)
                    {
                        isPressed = true;
                        actionType = EM_ACTION_TYPE.Left;
                    }
                    else if (currentPos.x > firstPos.x + MIN_X)
                    {
                        isPressed = true;
                        actionType = EM_ACTION_TYPE.Right;
                    }

                    else if (currentPos.y > firstPos.y + MIN_Y)
                    {
                        isPressed = true;
                        actionType = EM_ACTION_TYPE.Up;
                    }

                    else if (currentPos.y < firstPos.y - MIN_Y)
                    {
                        isPressed = true;
                        actionType = EM_ACTION_TYPE.Down;
                    }
                    else
                    {
                        isPressed = false;
                    }
                }

                firstPos = Input.mousePosition;
            }

            // 记录玩家鼠标抬起时的位置和时间信息
            if (Input.GetMouseButtonUp((int) mouseType))
            {
                isPressed = false;
            }
        }

        /// <summary>
        /// Android: 手指触控检测
        /// </summary>
        [Conditional("UNITY_ANDROID")]
        private static void TouchCheck()
        {
            // 判断用户 1 根手指触摸屏幕
            if (Input.touchCount == 1)
            {
                // 按下
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    firstPos = Input.GetTouch(0).position;
                    currentPos = Input.GetTouch(0).position;
                    timer = 0;
                }

                // 滑动
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    currentPos = Input.GetTouch(0).position;
                    timer += Time.deltaTime;
                    
                    if (timer > MIN_TIME)
                    {
                        if (currentPos.x < firstPos.x - MIN_X)
                        {
                            isPressed = true;
                            actionType = EM_ACTION_TYPE.Left;
                        }
                        else if (currentPos.x > firstPos.x + MIN_X)
                        {
                            isPressed = true;
                            actionType = EM_ACTION_TYPE.Right;
                        }

                        else if (currentPos.y > firstPos.y + MIN_Y)
                        {
                            isPressed = true;
                            actionType = EM_ACTION_TYPE.Up;
                        }

                        else if (currentPos.y < firstPos.y - MIN_Y)
                        {
                            isPressed = true;
                            actionType = EM_ACTION_TYPE.Down;
                        }
                        else
                        {
                            isPressed = false;
                        }
                    }

                    firstPos = Input.mousePosition;
                }

                // 抬起
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    isPressed = false;
                }
            }
        }

        /// <summary>
        /// Windows: 键盘检测
        /// </summary>
        [Conditional("UNITY_STANDALONE_WIN")]
        private static void KeyBoardCheck()
        {
            // 旋转
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isPressed = true;
                actionType = EM_ACTION_TYPE.Up;
            }

            // 左移
            if (Input.GetKeyDown(KeyCode.A))
            {
                isPressed = true;
                actionType = EM_ACTION_TYPE.Left;
            }

            // 取消左移
            if (Input.GetKeyUp(KeyCode.A))
            {
                isPressed = false;
            }

            // 右移
            if (Input.GetKeyDown(KeyCode.D))
            {
                isPressed = true;
                actionType = EM_ACTION_TYPE.Right;
            }

            // 取消右移
            if (Input.GetKeyUp(KeyCode.D))
            {
                isPressed = false;
            }

            // 加速
            if (Input.GetKeyDown(KeyCode.S))
            {
                isPressed = true;
                actionType = EM_ACTION_TYPE.Down;
            }

            // 取消加速
            if (Input.GetKeyUp(KeyCode.S))
            {
                isPressed = false;
            }
        }
        
        /// <summary>
        /// Window: 鼠标手势检测
        /// </summary>
        [Conditional("UNITY_STANDALONE_WIN")]
        private static void MouseSignCheck()
        {
            // 记录玩家鼠标点击时的位置和时间信息
            if (Input.GetMouseButtonDown((int) mouseType))
            {
                firstPos = Input.mousePosition;
                currentPos = Input.mousePosition;
                timer = 0;
            }

            // 记录玩家鼠标滑动时的位置和时间信息
            if (Input.GetMouseButton((int) mouseType))
            {
                currentPos = Input.mousePosition;
                timer += Time.deltaTime;
            }

            // 记录玩家鼠标抬起时的位置和时间信息
            if (Input.GetMouseButtonUp((int) mouseType))
            {
                if (timer > MIN_TIME)
                {
                    if (currentPos.x < firstPos.x - MIN_X)
                    {
                        Debug.Log($"[手势]: 向左滑动: 当前的为 {currentPos.x}, 上一帧为 {firstPos.x}");
                    }

                    if (currentPos.x > firstPos.x + MIN_X)
                    {
                        Debug.Log($"[手势]: 向右滑动: 当前的为 {currentPos.x}, 上一帧为 {firstPos.x}");
                    }

                    if (currentPos.y > firstPos.y + MIN_Y)
                    {
                        Debug.Log($"[手势]: 向上滑动: 当前的为 {currentPos.y}, 上一帧为 {firstPos.y}");
                    }

                    if (currentPos.y < firstPos.y - MIN_Y)
                    {
                        Debug.Log($"[手势]: 向下滑动: 当前的为 {currentPos.y}, 上一帧为 {firstPos.y}");
                    }
                }
            }
        }

        /// <summary>
        /// Android: 手指触控检测
        /// </summary>
        [Conditional("UNITY_ANDROID")]
        private static void TouchSignCheck()
        {
            // 判断用户 1 根手指触摸屏幕
            if (Input.touchCount == 1)
            {
                // 按下
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    firstPos = Input.GetTouch(0).position;
                    currentPos = Input.GetTouch(0).position;
                    timer = 0;
                }

                // 滑动
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    currentPos = Input.GetTouch(0).position;
                    timer += Time.deltaTime;
                }

                // 抬起
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if (timer > MIN_TIME)
                    {
                        if (currentPos.x < firstPos.x - MIN_X)
                        {
                            Debug.Log($"向左滑动: 当前的为 {currentPos.x}, 上一帧为 {firstPos.x}");
                        }

                        if (currentPos.x > firstPos.x + MIN_X)
                        {
                            Debug.Log($"向右滑动: 当前的为 {currentPos.x}, 上一帧为 {firstPos.x}");
                        }

                        if (currentPos.y > firstPos.y + MIN_Y)
                        {
                            Debug.Log($"向上滑动: 当前的为 {currentPos.y}, 上一帧为 {firstPos.y}");
                        }

                        if (currentPos.y < firstPos.y - MIN_Y)
                        {
                            Debug.Log($"向下滑动: 当前的为 {currentPos.y}, 上一帧为 {firstPos.y}");
                        }
                    }
                }
            }
        }
    }
}