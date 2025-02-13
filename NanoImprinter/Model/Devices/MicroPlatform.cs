﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WestLakeShape.Common;
using WestLakeShape.Common.WpfCommon;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.Model
{
    public interface IMicroPlatform : INotifyPropertyChanged, IPlatform
    {
        MicroPlatformConfig Config { get; set; }
        bool MoveToContactPosition();
        bool Demold();
        bool Creep(ChannelNo index,double distance);
        //bool GoHome();
        bool JogForward(ChannelNo index, double position);
        bool JogBackward(ChannelNo index, double position);
    }

    public class MicroPlatform : IMicroPlatform
    {
        private MicroPlatformConfig _config;
        private PiezoActuator _piezo;
        private bool _isConnected;
        private bool _isClosedLoop;

        private double _currentPositionZ;
        private double _currentPositionRX;
        private double _currentPositionRY;

        public delegate void MessageHandler(string message);
        public event MessageHandler OnMessage;

        #region
        public ChannelNo ZAxis => ChannelNo.Third;
        public ChannelNo RXAxis => ChannelNo.One;
        public ChannelNo RYAxis => ChannelNo.Two;
        public bool IsConnected => _piezo.IsConnected;
        public bool IsClosedLoop => _piezo.IsClosedLoop;

        public MicroPlatformConfig Config
        {
            get => _config;
            set => _config = value;
        }

        public double CurrentPositionZ
        {
            get => _currentPositionZ;
            set
            {
                if (_currentPositionZ != value)
                {
                    _currentPositionZ = value;
                    OnPropertyChanged(nameof(CurrentPositionZ));
                }
            }
        }
        public double CurrentPositionRX
        {
            get => _currentPositionRX;
            set
            {
                if (_currentPositionRX != value)
                {
                    _currentPositionRX = value;
                    OnPropertyChanged(nameof(CurrentPositionRX));
                }
            }
        }
        public double CurrentPositionRY
        {
            get => _currentPositionRY;
            set
            {
                if (_currentPositionRY != value)
                {
                    _currentPositionRY = value;
                    OnPropertyChanged(nameof(CurrentPositionRY));
                }
            }
        }
        
        #endregion
        
        public MicroPlatform(MicroPlatformConfig config)
        {
            _config = config;
            _piezo = new PiezoActuator(_config.PiezoActuatorConfig);
            CurrentPositionZ = 0;
            CurrentPositionRX = 0;
            CurrentPositionRY = 0;
            RefreshDataService.Instance.Register(RefreshRealtimeData);
        }


        public void SetClosedLoop(bool isClosedLoop)
        {
            _piezo.SetClosedLoop(1, isClosedLoop);
           // _piezo.SetAllChannelClosedLoop(isClosedLoop);
        }

        public void Connected()
        {            
            _piezo.Connect();
            //获取当前闭环开环状态
            _piezo.ReadClosedLoopFlag();
            //获取当前数值信息
            ReadPositions();
        }

        public void Disconnected()
        {
            _piezo.Disconnected();
        }

        /// <summary>
        /// 微平台移动到接触位置
        /// </summary>
        /// <returns></returns>
        public bool MoveToContactPosition()
        {
            MoveTo(ZAxis, _config.ContactPosition);
            return true;
        }

        /// <summary>
        /// 微平台脱模流程
        /// </summary>
        /// <returns></returns>
        public bool Demold()
        {
            MoveTo(_config.DemoldPosition);

            return true;
        }

        /// <summary>
        /// 轴缓慢移动
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Creep(ChannelNo index,double distance)
        {
            MoveTo(index, distance);
            return true;
        }

        /// <summary>
        /// 回零
        /// </summary>
        /// <returns></returns>
        public bool GoHome()
        {
            var zero = new PointZRXY(0, 0, 0);
            MoveTo(zero);

            return true;
        }

        /// <summary>
        /// Jog前移
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool JogForward(ChannelNo index, double position)
        {
            var targetPosition = CalculateDisplace(index, position);
            if (targetPosition >= 0)
            {
                MoveTo(index, targetPosition);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Jog后移
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool JogBackward(ChannelNo index, double position)
        {
            var targetPosition = CalculateDisplace(index, -position);
            if (targetPosition >= 0)
            {
                MoveTo(index, targetPosition);
            }
            return true;
        }

        /// <summary>
        /// 单通道移动
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        private void MoveTo(ChannelNo index, double position)
        {
            _piezo.WriteDisplace(index, position);

            while ((index == ChannelNo.One&&position!=CurrentPositionZ)||
                   (index == ChannelNo.Two && position != CurrentPositionRX) ||
                   (index == ChannelNo.Third && position != CurrentPositionRY))
            {
                Thread.Sleep(20);
                ReadPositions();
            }
        }

        /// <summary>
        /// 多通道移动
        /// </summary>
        /// <param name="position"></param>
        public void MoveTo(PointZRXY position)
        {
            _piezo.WriteMultiDisplace(new double[] { position.Z,position.RX, position.RY});
            
            while (position.Z!=CurrentPositionZ||
                   position.RX!=CurrentPositionRX||
                   position.RY!=CurrentPositionRY)
            {
                Thread.Sleep(20);
                ReadPositions();
            }
        }

        public void ReloadConfig()
        {
            _piezo.ReloadConfig();
        }

        private bool ReadPositions()
        {
            var position = _piezo.ReadMultiDisplace();
            _currentPositionZ = position[0];
            _currentPositionRX = position[1];
            _currentPositionRY = position[2];

            return true;
        }

        private double CalculateDisplace(ChannelNo index, double postion)
        {
            double position = 0;
            switch (index)
            {
                case ChannelNo.One:
                    position = _currentPositionZ + postion;
                    if (position > _config.PiezoActuatorConfig.ZMaxValue ||
                        position < _config.PiezoActuatorConfig.ZMinValue)
                    {
                        OnMessage?.Invoke($"当前目标位置{position}不在行程内，请保证{_config.PiezoActuatorConfig.ZMinValue}<Z<{_config.PiezoActuatorConfig.ZMaxValue}");
                        position = -1;
                    } 
                    break;
                case ChannelNo.Two:
                    position = _currentPositionRX + postion;
                    if (position > _config.PiezoActuatorConfig.RXMaxValue ||
                       position < _config.PiezoActuatorConfig.RXMinValue)
                    {
                        OnMessage?.Invoke($"当前目标位置{position}不在行程内，请保证{_config.PiezoActuatorConfig.RXMinValue}<RX<{_config.PiezoActuatorConfig.RXMaxValue}");
                        position = -1;
                    }
                    break;
                case ChannelNo.Third:
                    position = _currentPositionRY + postion;
                    if (position > _config.PiezoActuatorConfig.RYMaxValue ||
                       position < _config.PiezoActuatorConfig.RYMinValue)
                    {
                        OnMessage?.Invoke($"当前目标位置{position}不在行程内，请保证{_config.PiezoActuatorConfig.RYMinValue}<RY<{_config.PiezoActuatorConfig.RYMaxValue}");
                        position = -1;
                    }
                    break;
            }
            return position;
        }

        private void RefreshRealtimeData()
        {
            CurrentPositionRX = _currentPositionRX;
            CurrentPositionRY = _currentPositionRX;
            CurrentPositionZ = _currentPositionZ;
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

    public class MicroPlatformConfig:NotifyPropertyChanged
    {

        private double _contactPosition;
        private double _zCreepDistance;
        private PointZRXY _levelPosition = new PointZRXY(0, 0, 0);
        private PointZRXY _demoldPosition = new PointZRXY(0, 0, 0);
        private double _maxPressure;
        private double _minPressure;
        private PiezoActuatorConfig _piezoActuatorConfig = new PiezoActuatorConfig();

        [Category("MicroPlatform"), Description("接触位置")]
        [DisplayName("接触位置")]
        public double ContactPosition
        {
            get => _contactPosition;
            set => SetProperty(ref _contactPosition, value);
        }

        [Category("MicroPlatform"), Description("蠕动距离")]
        [DisplayName("蠕动距离")]
        public double ZCreepDistance 
        {
            get => _zCreepDistance;
            set => SetProperty(ref _zCreepDistance, value);
        }

        [Category("MicroPlatform"), Description("脱模位置")]
        [DisplayName("jiec")]
        public PointZRXY DemoldPosition 
        {
            get => _demoldPosition;
            set => SetProperty(ref _demoldPosition, value);
        }

        [Category("MicroPlatform"), Description("调平位置")]
        [DisplayName("调平位置")]
        public PointZRXY LevelPosition
        {
            get => _levelPosition;
            set => SetProperty(ref _levelPosition, value);
        }

        [Category("MicroPlatform"), Description("压力最大值")]
        [DisplayName("压力最大值")]
        public double MaxPressure 
        {
            get => _maxPressure;
            set => SetProperty(ref _maxPressure, value);
        }


        [Category("MicroPlatform"), Description("压力最小值")]
        [DisplayName("压力最小值")]
        public double MinPressure 
        {
            get => _minPressure;
            set => SetProperty(ref _minPressure, value);
        }

        public PiezoActuatorConfig PiezoActuatorConfig
        {
            get => _piezoActuatorConfig;
            set => SetProperty(ref _piezoActuatorConfig, value);
        }

    }

}
