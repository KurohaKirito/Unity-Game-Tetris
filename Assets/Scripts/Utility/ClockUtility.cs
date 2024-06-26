﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// 定时器
    /// </summary>
    [Serializable]
    public class Clock
    {
        /// <summary>
        /// 定时器标签
        /// </summary>
        public int tag;

        /// <summary>
        /// 已计时时间
        /// </summary>
        public float timed;

        /// <summary>
        /// 定时器总时长
        /// </summary>
        public float life;

        /// <summary>
        /// 剩余计时次数
        /// </summary>
        public int count;

        /// <summary>
        /// 触发委托
        /// </summary>
        [NonSerialized]
        public Action action;
    }

    /// <summary>
    /// 定时器管理类
    /// </summary>
    public class ClockUtility : MonoBehaviour
    {
        /// <summary>
        /// 所有的定时器
        /// </summary>
        private static readonly List<Clock> schedules = new List<Clock>();

        /// <summary>
        /// 定时器标签, 用于唯一标识定时器
        /// </summary>
        private static int clockTag;

        /// <summary>
        /// 定时主函数
        /// 更新定时器
        /// </summary>
        private void Update()
        {
            if (schedules.Count == 0)
            {
                return;
            }

            for (var index = 0; index < schedules.Count; ++index)
            {
                var currentScheduler = schedules[index];

                // 移除空定时器
                if (currentScheduler == null)
                {
                    UnRegisterClockByIndex(index--);
                    continue;
                }

                // 定时器次数剩余为空, 不触发任何操作, 需手动移除
                if (currentScheduler.count == 0)
                {
                    continue;
                }

                // 累计定时
                currentScheduler.timed += Time.deltaTime;

                // 定时未结束, 返回继续定时
                if (currentScheduler.timed < currentScheduler.life)
                {
                    continue;
                }

                // 定时结束, 触发委托
                currentScheduler.action();

                // 重置定时
                currentScheduler.timed -= currentScheduler.life;

                // 负数为持续定时
                if (currentScheduler.count < 0)
                {
                    continue;
                }

                // 减少一次定时次数
                --currentScheduler.count;
            }
        }

        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="time">定时时长 (单位秒)</param>
        /// <param name="action">调度方法</param>
        /// <param name="count">调度次数, 负数为永久</param>
        /// <returns>定时器的标签值</returns>
        public static int RegisterClock(float time, int count, Action action)
        {
            // 创建新的定时器
            var scheduler = new Clock
            {
                timed = 0,
                life = time,
                action = action,
                count = count,
                tag = ++clockTag
            };

            // 注册
            schedules.Add(scheduler);

            // 返回标签
            return clockTag;
        }

        /// <summary>
        /// 通过标签获取定时器
        /// </summary>
        /// <param name="tag">标签值</param>
        /// <returns></returns>
        private static Clock GetClock(int tag)
        {
            foreach (var schedule in schedules)
            {
                if (schedule.tag == tag)
                {
                    return schedule;
                }
            }

            return null;
        }

        /// <summary>
        /// 更新特定定时器的定时长度
        /// </summary>
        /// <param name="tag">定时器标签</param>
        /// <param name="life">新的定时长度</param>
        /// <param name="timed">已定时时间</param>
        /// <param name="count">剩余定时次数</param>
        public static void UpdateClock(int tag, float life, float timed, int count)
        {
            var clock = GetClock(tag);
            if (clock == null)
            {
                return;
            }

            clock.life = life;
            clock.timed = timed;
            clock.count = count;
        }

        /// <summary>
        /// 更新特定定时器的定时长度
        /// </summary>
        /// <param name="tag">定时器标签</param>
        /// <param name="life">新的定时长度</param>
        /// <param name="count">剩余定时次数</param>
        public static void UpdateClock(int tag, float life, int count)
        {
            var clock = GetClock(tag);
            if (clock == null)
            {
                return;
            }

            clock.life = life;
            clock.count = count;
        }

        /// <summary>
        /// 更新特定定时器的定时长度
        /// </summary>
        /// <param name="tag">定时器标签</param>
        /// <param name="life">新的定时长度</param>
        public static void UpdateClock(int tag, float life)
        {
            var clock = GetClock(tag);
            if (clock == null)
            {
                return;
            }

            clock.life = life;
        }

        /// <summary>
        /// 更新特定定时器的定时长度
        /// </summary>
        /// <param name="tag">定时器标签</param>
        /// <param name="count">剩余定时次数</param>
        public static void UpdateClock(int tag, int count)
        {
            var clock = GetClock(tag);
            if (clock == null)
            {
                return;
            }

            clock.count = count;
        }

        /// <summary>
        /// 通过索引移除定时器
        /// </summary>
        /// <param name="index">定时器索引</param>
        private static void UnRegisterClockByIndex(int index)
        {
            if (index < 0 || index >= schedules.Count)
            {
                return;
            }

            schedules[index] = schedules[^1];
            schedules.RemoveAt(schedules.Count - 1);
        }

        /// <summary>
        /// 通过标签移除定时器
        /// </summary>
        /// <param name="tag">定时器标签</param>
        public static void UnRegisterClockByTag(int tag)
        {
            for (var index = 0; index < schedules.Count; index++)
            {
                if (schedules[index].tag == tag)
                {
                    UnRegisterClockByIndex(index);
                }
            }
        }
    }
}