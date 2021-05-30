using System;
using UnityEngine;

namespace Utility
{
    public static class InputUtility
    {
        public static Action rotate;

        public static Action moveDown;
        public static Action cancelMoveDown;

        public static Action moveLeft;
        public static Action cancelMoveLeft;

        public static Action moveRight;
        public static Action cancelMoveRight;
        
        public static void OnUpdate()
        {
            // 旋转
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rotate?.Invoke();
            }

            // 左移
            if (Input.GetKeyDown(KeyCode.A))
            {
                moveLeft?.Invoke();
            }

            // 取消左移
            if (Input.GetKeyUp(KeyCode.A))
            {
                cancelMoveLeft?.Invoke();
            }

            // 右移
            if (Input.GetKeyDown(KeyCode.D))
            {
                moveRight?.Invoke();
            }

            // 取消右移
            if (Input.GetKeyUp(KeyCode.D))
            {
                cancelMoveRight?.Invoke();
            }

            // 加速
            if (Input.GetKeyDown(KeyCode.S))
            {
                moveDown?.Invoke();
            }

            // 取消加速
            if (Input.GetKeyUp(KeyCode.S))
            {
                cancelMoveDown?.Invoke();
            }
        }
    }
}