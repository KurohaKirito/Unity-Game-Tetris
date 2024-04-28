using System;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        /// <summary>
        /// UI 时间
        /// </summary>
        public Text time;

        /// <summary>
        /// UI 等级
        /// </summary>
        public Text level;

        /// <summary>
        /// UI 当前分数
        /// </summary>
        public Text score;

        /// <summary>
        /// UI 最高分
        /// </summary>
        public Text highestScore;

        /// <summary>
        /// UI 最高分取得日期
        /// </summary>
        public Text highestScoreDate;

        /// <summary>
        /// UI 最高分取得时间
        /// </summary>
        public Text highestScoreTime;

        /// <summary>
        /// 单例模式
        /// </summary>
        private static UIManager instance;

        /// <summary>
        /// 单例模式
        /// </summary>
        public static UIManager Instance
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

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 每帧更新钟表
        /// </summary>
        private void Update()
        {
            time.text = DateTime.Now.ToLongTimeString();
        }
    }
}