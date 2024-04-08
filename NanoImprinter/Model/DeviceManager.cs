using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NanoImprinter.Model
{
    public class DeviceManager
    {
        private DeviceModel _model;
        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(_isConnected));
                }
            }
        }

        public DeviceManager(DeviceModel model)
        {
            _model = model;
        }

        public void GoHome()
        {
            var afmPlatform = _model.GetPlatform(typeof(AfmPlatform).Name);
            var gluePlatform = _model.GetPlatform(typeof(GluePlatform).Name);
            var imprintPlatform = _model.GetPlatform(typeof(ImprintPlatform).Name);
            var macroPlatform = _model.GetPlatform(typeof(MacroPlatform).Name);
            var microPlatform = _model.GetPlatform(typeof(MicroPlatform).Name);

            //微动平台回零=压印回零=点胶回零>>>>>>>宏动平台回零=afm回零
            var microTask = Task.Run(() => microPlatform.GoHome());
            var imprintTask = Task.Run(()=>imprintPlatform.GoHome());
            var glueTask = Task.Run(() => gluePlatform.GoHome());
            Task.WaitAll(imprintTask,glueTask,microTask);
            
            var macroTask = Task.Run(() => macroPlatform.GoHome());
            var afmTask = Task.Run(() => afmPlatform.GoHome());
            Task.WaitAll(macroTask, afmTask);
        }

        public void ConnectedPlatform()
        {
            _model.Axes.Connected();
            //foreach (var pairs in Platforms)
            //{
            //    pairs.Value.Connected();
            //}

            _isConnected = true;
        }

        public void DisconnectedPlatform()
        {
            _model.Axes.Disconnected();

            foreach (var pairs in _model.Platforms)
            {
                pairs.Value.Disconnected();
            }

            _isConnected = false;
        }
        private void RefreshRealtimeData()
        {
            var isAll = _model.Platforms.Values.All(o => o.IsConnected);
            IsConnected = isAll;
        }

        public bool CheckBeforeAfmPlatformMove()
        {
            //afm移动前检查压印，UV,点胶轴位置
            return true;
        }
        public bool CheckBeforeGluePlatformMove()
        {

            return true;
        }
        public bool CheckBeforeImprintPlatformMove()
        {

            return true;
        }
        public bool CheckBeforeMacroPlatformMove()
        {

            return true;
        }
        public bool CheckBeforeMicroPlatformMove()
        {

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }
    }
}
