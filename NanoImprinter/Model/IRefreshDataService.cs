using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NanoImprinter.Model
{
    /// <summary>
    /// 通过定义一个单例模式，并将所有需要刷新的方法注册到单例，单例定时触发已注册方法。
    /// </summary>
    public interface IRefreshDataService
    {
        void Register(Action action);
        void Unregister(Action action);
    }  

    public class RefreshDataService : IRefreshDataService
    {
        private Timer _timer;
        private List<Action> _actions = new List<Action>();
        private static readonly RefreshDataService _service = new RefreshDataService();

        public static RefreshDataService Instance => _service;


        private RefreshDataService()
        {
            _timer = new Timer(new TimerCallback(RefreshData), null, 100, 2);
        }


        public void Register(Action action)
        {
            if (!_actions.Contains(action))
            {
                _actions.Add(action);
            }
        }


        public void Unregister(Action action)
        {
            if (_actions.Contains(action))
            {
                _actions.Remove(action);
            }
        }


        private void RefreshData(object state)
        {
            foreach (var action in _actions)
            {
                action?.Invoke();
            }
        }
    }

    //public class RefreshValue
    //{
    //    private DispatcherTimer _timer;
    //    private List<Action> _actions = new List<Action>();

    //    public RefreshDataService()
    //    {
    //        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };

    //        _timer.Tick += (sender, e) =>
    //        {
    //            foreach (var action in _actions)
    //            {
    //                action?.Invoke();
    //            }
    //        };

    //        _timer.Start();
    //    }

    //    public void Register(Action action)
    //    {
    //        if (!_actions.Contains(action))
    //        {
    //            _actions.Add(action);
    //        }
    //    }

    //    public void Unregister(Action action)
    //    {
    //        if (_actions.Contains(action))
    //        {
    //            _actions.Remove(action);
    //        }
    //    }
    //}
}
