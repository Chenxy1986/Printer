using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WestLakeShape.Common;
using WestLakeShape.Common.WpfCommon;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.Model
{
    public interface IMicroPlatform
    {
        MicroPlatformConfig Config { get; set; }

        void OnConnecting();
        void OnDisconnecting();
        bool MoveToContactPosition();
        bool Demold();
        bool Creep(ChannelNo index,double distance);
        //bool GoHome();
        bool JogForward(ChannelNo index, double position);
        bool JogBackward(ChannelNo index, double position);
    }

    public class MicroPlatform:IMicroPlatform,IPlatform
    {
        private MicroPlatformConfig _config;
        private PiezoActuator _piezo;
        private bool _isconnected;
        private PointZRXY _currentPosition;

        public bool IsConnected => _piezo.IsConnected;
        public ChannelNo ZAxis => ChannelNo.Third;
        public ChannelNo RXAxis => ChannelNo.One;
        public ChannelNo RYAxis => ChannelNo.Two;

        public MicroPlatformConfig Config
        {
            get => _config;
            set => _config = value;
        }

        public PointZRXY CurrentPosition => _currentPosition;


        public MicroPlatform(MicroPlatformConfig config)
        {
            _config = config;
            _piezo = new PiezoActuator(_config.PortName);
            _currentPosition = new PointZRXY(0, 0, 0);
        }


        public void OnConnecting()
        {
            if (_isconnected)
            {
                _piezo.Connect();
                Thread.Sleep(100);
                _currentPosition = RefreshStates();
            }
        }

        public void OnDisconnecting()
        {
            if (!_isconnected)
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
            _piezo.WriteDisplace(index, distance);
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
            MoveTo(index, targetPosition);

            return true;
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
            MoveTo(index, targetPosition);

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
            Thread.Sleep(100);

            RefreshStates();
        }

        /// <summary>
        /// 多通道移动
        /// </summary>
        /// <param name="position"></param>
        public void MoveTo(PointZRXY position)
        {
            _piezo.WriteMultiDisplace(new double[] { position.RX, position.RY, position.Z });
            Thread.Sleep(100);

            RefreshStates();
        }


        private PointZRXY RefreshStates()
        {
            var position = _piezo.ReadMultiDisplace();

            return new PointZRXY(position[0], position[1], position[2]);
        }

        private double CalculateDisplace(ChannelNo index, double postion)
        {
            double position = 0;
            switch (index)
            {
                case ChannelNo.One:
                    position = _currentPosition.RX + postion;
                    break;
                case ChannelNo.Two:

                    position = _currentPosition.RY + postion;
                    break;
                case ChannelNo.Third:
                    position = _currentPosition.Z + postion;
                    break;
            }

            return position;
        }

    }
    public class MicroPlatformConfig:NotifyPropertyChanged
    {
        private string _portName = "Com1";
        private double _contactPosition;
        private double _zCreepDistance;
        private PointZRXY _levelPosition;
        private PointZRXY _demoldPosition;
        private double _maxPressure;
        private double _minPressure;

        [Category("MicroPlatform"), Description("串口名称")]
        [DisplayName("串口名称")]
        public string PortName 
        {
            get => _portName;
            set => SetProperty(ref _portName, value);
        } 

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
    }
}
