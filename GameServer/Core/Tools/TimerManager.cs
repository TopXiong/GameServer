using GameServer.Core.Base;
using System;
using System.Collections.Generic;

namespace GameServer.Core.Tools
{

    /// <summary>
    /// 计时器类型
    /// </summary>
    public enum TimerType
    {
        /// <summary>
        /// 只使用一次
        /// </summary>
        OnceTimer,

        /// <summary>
        /// 循环调用
        /// </summary>
        RepeatedTimer,
    }

    /// <summary>
    /// 计时器类
    /// </summary>
    public class TimerAction
    {
        /// <summary>
        /// 调用类型
        /// </summary>
        public TimerType TimerType { get; set; }

        /// <summary>
        /// 时间到了之后的回调函数
        /// </summary>
        public Action Callback { get; set; }

        /// <summary>
        /// 计时时间，ms
        /// </summary>
        public long RemainingTime { get; set; }

        public bool Active { get; set; }

        public TimerAction(TimerType timerType, Action callback, long milliseconds, bool active = true)
        {
            this.TimerType = timerType;
            this.Callback = callback;
            this.RemainingTime = milliseconds;
            Active = active;
        }

        public void Start()
        {
            TimerManager.Instance.AddTimer(this);
        }

    }


    /// <summary>
    /// 计时器管理
    /// </summary>
    public class TimerManager:BaseGameObject
    {

        private static TimerManager s_instance;

        public static TimerManager Instance
        {
            get
            {
                return s_instance;
            }
        }

        public override void Awake()
        {
            s_instance = this;
        }

        /// <summary>
        /// 所有的计时器，键为触发时间，值为计时器列表
        /// </summary>
        private readonly MultiMap<long, TimerAction> m_timeId = new MultiMap<long, TimerAction>();

        /// <summary>
        /// 正在执行的计时器列表，记录并在执行完后移除
        /// </summary>
        private readonly MultiMap<long, TimerAction> m_timeOutTime = new MultiMap<long, TimerAction>();

        /// <summary>
        /// 当前要执行的计时器
        /// </summary>
        private readonly Queue<TimerAction> m_timers = new Queue<TimerAction>();

        /// <summary>
        /// 下一个要执行的时间
        /// </summary>
        private long m_minTime;

        public override void Update()
        {
            if (m_timeId.Count == 0)
            {
                return;
            }

            long timeNow = CurrentTimeGet();
            // 如果小于一个最近的计时器就直接返回
            if (timeNow < m_minTime)
            {
                return;
            }


            foreach (var kv in m_timeId)
            {
                long k = kv.Key;
                //时间大于当前时间，不触发跳出
                if (k > timeNow)
                {
                    m_minTime = k;
                    break;
                }

                //添加要触发的计时器列表
                m_timeOutTime.Add(kv.Key, kv.Value);
            }


            foreach (var kv in m_timeOutTime)
            {
                //将一个触发的计时器列表添加到待处理列表中
                var timers = kv.Value;
                foreach (var timer in timers)
                {
                    m_timers.Enqueue(timer);
                }

                //移除已经 处理的列表
                m_timeId.Remove(kv.Key);
            }

            m_timeOutTime.Clear();
            //处理每一个激活的计时器
            while (this.m_timers.Count > 0)
            {
                TimerAction timerAction = this.m_timers.Dequeue();
                //未激活
                if (timerAction == null || !timerAction.Active)
                {
                    continue;
                }

                HandleTimer(timerAction);
            }
        }

        private void HandleTimer(TimerAction timerAction)
        {
            switch (timerAction.TimerType)
            {
                case TimerType.OnceTimer:
                    timerAction.Callback?.Invoke();
                    Remove(timerAction);
                    break;
                case TimerType.RepeatedTimer:
                    timerAction.Callback?.Invoke();
                    AddTimer(timerAction);
                    break;
            }
        }

        /// <summary>
        /// 获取系统当前时间,ms
        /// </summary>
        /// <returns></returns>
        public long CurrentTimeGet()
        {
            return DateTime.Now.Ticks / 10000;
        }

        /// <summary>
        /// 开启计时器
        /// </summary>
        /// <param name="timerType">计时器类型，单次还是循环</param>
        /// <param name="callback">回调函数</param>
        /// <param name="time">计时时长</param>
        public void AddTimer(TimerType timerType, Action callback, long time)
        {
            AddTimer(new TimerAction(timerType, callback, time));
        }

        /// <summary>
        /// 开启计时器
        /// </summary>
        /// <param name="timerAction"></param>
        public void AddTimer(TimerAction timerAction)
        {
            long tillTIme = CurrentTimeGet() + timerAction.RemainingTime;
            m_timeId.Add(tillTIme, timerAction);
            //更新下一个要执行的时间
            if (tillTIme < m_minTime)
            {
                m_minTime = tillTIme;
            }
        }

        public bool Remove(TimerAction timerAction)
        {
            if (timerAction == null)
            {
                return false;
            }

            timerAction.Active = false;
            return true;
        }

    }
}