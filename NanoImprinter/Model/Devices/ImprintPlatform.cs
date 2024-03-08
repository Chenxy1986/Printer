using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WestLakeShape.Common;
using WestLakeShape.Common.WpfCommon;
using WestLakeShape.Motion;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.Model
{
    public interface IImprintPlatform
    {
        ImprintPlatformConfig Config { get; set; }
        //bool GoHome();
        bool MoveToMaskPreprintHeight();
        bool MoveToTakePictureHeight();
        void ResetAxesAlarm();
    }

    public class ImprintPlatform:IImprintPlatform,IPlatform,INotifyPropertyChanged
    {
        private ImprintPlatformConfig _config;
        private IAxis _maskZAxis;
        private IAxis _cameraZAxis;
        private IAxis _uvXAxis;
        private IAxis _uvZAxis;
        private ForceSensorControl _forceSensorControl;
        private UVControl _uvControl;
        
        private double _currentPositionMaskZ;
        private double _currentPositionCameraZ;
        private double _currentPositionUVX;
        private double _currentPositionUVZ;
        private double _forceValue0;
        private double _forceValue1;
        private double _forceValue2;
        private double _forceValue3;

        public ImprintPlatformConfig Config
        {
            get => _config;
            set => _config = value;
        }

        public IAxis MaskZAxis => _maskZAxis;
        public IAxis CameraZAxis => _cameraZAxis;
        public IAxis UVXAxis => _uvXAxis;
        public IAxis UVZAxis => _uvZAxis;

        #region 实时数据
        public double CurrentPositionMaskZ
        {
            get => _currentPositionMaskZ;
            set
            {
                if (_currentPositionMaskZ != value)
                {
                    _currentPositionMaskZ = value;
                    OnPropertyChanged(nameof(CurrentPositionMaskZ));
                }
            }
        }

        public double CurrentPositionCameraZ
        {
            get => _currentPositionCameraZ;
            set
            {
                if (_currentPositionCameraZ != value)
                {
                    _currentPositionCameraZ = value;
                    OnPropertyChanged(nameof(CurrentPositionCameraZ));
                }
            }
        }

        public double CurrentPositionUVX
        {
            get => _currentPositionUVX;
            set
            {
                if (_currentPositionUVX != value)
                {
                    _currentPositionUVX = value;
                    OnPropertyChanged(nameof(CurrentPositionUVX));
                }
            }
        }
        public double CurrentPositionUVZ
        {
            get => _currentPositionUVZ;
            set
            {
                if (_currentPositionUVZ != value)
                {
                    _currentPositionUVZ = value;
                    OnPropertyChanged(nameof(CurrentPositionUVZ));
                }
            }
        }

        public double ForceValue0
        {
            get => _forceValue0;
            set
            {
                if (_forceValue0 != value)
                {
                    _forceValue0 = value;
                    OnPropertyChanged(nameof(ForceValue0));
                }
            }
        }
        public double ForceValue1
        {
            get => _forceValue1;
            set
            {
                if (_forceValue1 != value)
                {
                    _forceValue1 = value;
                    OnPropertyChanged(nameof(ForceValue1));
                }
            }
        }
        public double ForceValue2
        {
            get => _forceValue2;
            set
            {
                if (_forceValue2 != value)
                {
                    _forceValue2 = value;
                    OnPropertyChanged(nameof(ForceValue2));
                }
            }
        }
        public double ForceValue3
        {
            get => _forceValue3;
            set
            {
                if (_forceValue3 != value)
                {
                    _forceValue3 = value;
                    OnPropertyChanged(nameof(ForceValue3));
                }
            }
        }

        #endregion

        public ImprintPlatform(ImprintPlatformConfig config,IAxis[] axes)
        {
            _config = config;
            _maskZAxis = axes[0];
            _cameraZAxis = axes[1];
            _uvXAxis = axes[2];
            _uvZAxis = axes[3];
            _forceSensorControl = new ForceSensorControl(_config.ForceSensorControlConfig);
            _uvControl = new UVControl(_config.UVConfig);
            //_maskZAxis = new TrioAxis(_config.MaskZAxisConfig);
            //_cameraZAxis = new TrioAxis(_config.CameraZAxisConfig);
            //_uvXAxis = new TrioAxis(_config.UVXAxisConfig);
            //_uvYAxis = new TrioAxis(_config.UVYAxisConfig);
            RefreshDataService.Instance.Register(RefreshRealtimeData);
        }

        public void Connected()
        {
            _forceSensorControl.Connected();
            _uvControl.OnConnecting();
        }
        public void ConnectedForceControl()
        {
            _forceSensorControl.Connected();
        }
        public void ConnectedUVControl()
        {
            _uvControl.OnConnecting();
        }
        public void Disconnected()
        {
            _forceSensorControl.Disconnected();
            _uvControl.OnDisconnecting();
        }


        public bool GoHome()
        {
            var czTask = Task.Run(()=> _cameraZAxis.GoHome());
            var uxTask = Task.Run(()=>_uvXAxis.GoHome());
            var uyTask = Task.Run(()=> _uvZAxis.GoHome());
            Task.WaitAll(czTask, uxTask, uyTask);
            return _maskZAxis.GoHome();
        }

        public void ReloadConfig()
        {
            _uvControl.ReloadConfig();
            _forceSensorControl.ReloadConfig();
        }

        public bool MoveToMaskPreprintHeight()
        {
            return MoveBy(_maskZAxis,_config.MaskPreprintHeight);
        }
        public bool MoveToMaskWaitHeight()
        {
            return MoveBy(_maskZAxis, _config.MaskWaitHeight);
        }

        public bool MoveToTakePictureHeight()
        {
            if (_uvXAxis.Position > _config.CameraXDirSafePosition)
                throw new Exception("UV未离开冲突区域，移动相机会发生碰撞");
            if (_maskZAxis.Position < _config.CameraTakePictureHeight)
                throw new Exception("掩膜高度太低，移动相机会发生碰撞");

            return MoveBy(_cameraZAxis,_config.CameraTakePictureHeight);
        }

        public bool MoveToCameraWaitHeight()
        {
            return MoveBy(_cameraZAxis,_config.CameraWaitHeight);
        }

        public bool MoveToUVWaitPositon()
        {
            var xTask = Task.Run(() => MoveBy(_uvXAxis, _config.UVWaitPosition.X));
            var zTask = Task.Run(() => MoveBy(_uvZAxis, _config.UVWaitPosition.Z));
            Task.WaitAll(xTask, zTask);
            return true;
        }

        public bool MoveToUVIrradiationPosition()
        {
            if (_maskZAxis.Position < _config.UVIrradiationPosition.Z)
                throw new Exception("掩膜高度太高，移动UV会发生碰撞");
            if (_cameraZAxis.Position < _config.UVZDirSafePosition)
                throw new Exception("相机高度太低，移动UV会发生碰撞");

            var xTask = Task.Run(() => MoveBy(_uvXAxis, _config.UVIrradiationPosition.X));
            var zTask = Task.Run(() => MoveBy(_uvZAxis, _config.UVIrradiationPosition.Z));
            Task.WaitAll(xTask, zTask);
            return true;
        }

        private bool MoveBy(IAxis axis,double position)
        {
            return axis.MoveTo(position);
        }

        public void ResetAxesAlarm()
        {
            ((TrioAxis)_maskZAxis).ResetAlarm();
            ((TrioAxis)_cameraZAxis).ResetAlarm();
        }

        public bool UVIrradiate()
        {
            _uvControl.TurnOn();
            Thread.Sleep(_config.UVConfig.PreheatTime);
            _uvControl.TurnOff();
            return true;
        }

        private void RefreshRealtimeData()
        {
            if (_currentPositionCameraZ != _cameraZAxis.Position)
                CurrentPositionCameraZ = _cameraZAxis.Position;
            if (_currentPositionMaskZ != _maskZAxis.Position)
                CurrentPositionMaskZ = _maskZAxis.Position;
            if (_currentPositionUVX != _uvXAxis.Position)
                CurrentPositionUVX = _uvXAxis.Position;
            if (_forceValue0 != _forceSensorControl.ForceValue0)
                ForceValue0 = _forceSensorControl.ForceValue0;
            if (_forceValue1 != _forceSensorControl.ForceValue1)
                ForceValue1 = _forceSensorControl.ForceValue1;
            if (_forceValue2 != _forceSensorControl.ForceValue2)
                ForceValue2 = _forceSensorControl.ForceValue2;
            if (_forceValue3 != _forceSensorControl.ForceValue3)
                ForceValue3 = _forceSensorControl.ForceValue3;
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

    public class ImprintPlatformConfig:NotifyPropertyChanged
    {
        private double _maskWaitHeight;
        private double _maskPrepintHeight;
        private double _maskZWorkVel;
        private double _cameraWaitHeight;
        private double _cameraTakePictureHeight;
        private double _cameraZWorkVel;
        private double _cameraXDirSafePosition;
        private PointXZ _uvWaitPosition = new PointXZ(0, 0);
        private PointXZ _uvIrradiationPosition = new PointXZ(0, 0);
        private double _uvXWorkVel;
        private double _uvYWorkVel;
        private UVControlConfig _uvConfig = new UVControlConfig();
        private ForceSensorControlConfig _forceSensorConfig = new ForceSensorControlConfig();
        private double _uvZDirSafePosition;

        //掩膜组件
        [Category("ImprintPlatform"), Description("掩膜等待高度")]
        [DisplayName("掩膜等待高度")]
        public double MaskWaitHeight 
        {
            get => _maskWaitHeight;
            set => SetProperty(ref _maskWaitHeight, value);
        }

        //掩膜组件
        [Category("ImprintPlatform"), Description("掩膜预压印高度")]
        [DisplayName("掩膜预压印高度")]
        public double MaskPreprintHeight 
        {
            get => _maskPrepintHeight;
            set => SetProperty(ref _maskPrepintHeight, value);
        }

        [Category("ImprintPlatform"), Description("压印速度")]
        [DisplayName("压印速度")]
        public double MaskZWorkVel
        {
            get => _maskZWorkVel;
            set => SetProperty(ref _maskZWorkVel, value);
        }


        //拍照组件参数
        [Category("ImprintPlatform"), Description("等待拍照高度")]
        [DisplayName("等待拍照高度")]
        public double CameraWaitHeight
        {
            get => _cameraWaitHeight;
            set => SetProperty(ref _cameraWaitHeight, value);
        }

        [Category("ImprintPlatform"), Description("拍照高度")]
        [DisplayName("拍照高度")]
        public double CameraTakePictureHeight
        {
            get => _cameraTakePictureHeight;
            set => SetProperty(ref _cameraTakePictureHeight, value);
        }

        [Category("ImprintPlatform"), Description("移动拍照位速度")]
        [DisplayName("移动拍照位速度")]
        public double CameraZWorkVel
        {
            get => _cameraZWorkVel;
            set => SetProperty(ref _cameraZWorkVel, value);
        }

        [Category("ImprintPlatform"), Description("相机移动过程中，X方向UV发生碰撞的位置")]
        [DisplayName("X方向安全位置")]
        public double CameraXDirSafePosition
        {
            get => _cameraXDirSafePosition;
            set => SetProperty(ref _cameraXDirSafePosition, value);
        }


        //UV组件参数
        [Category("ImprintPlatform"), Description("UV等待位")]
        [DisplayName("UV等待位")]
        public PointXZ UVWaitPosition 
        {
            get => _uvWaitPosition;
            set => SetProperty(ref _uvWaitPosition, value);
        }

        [Category("ImprintPlatform"), Description("UV照射位")]
        [DisplayName("UV照射位")]
        public PointXZ UVIrradiationPosition
        {
            get => _uvIrradiationPosition ;
            set => SetProperty(ref _uvIrradiationPosition, value);
        }

        [Category("ImprintPlatform"), Description("UVX轴工作速度")]
        [DisplayName("UVX轴工作速度")]
        public double UVXWorkVel 
        {
            get => _uvXWorkVel;
            set => SetProperty(ref _uvXWorkVel, value);
        }

        [Category("ImprintPlatform"), Description("UVY轴工作速度")]
        [DisplayName("UVY轴工作速度")]
        public double UVYWorkVel
        {
            get => _uvYWorkVel;
            set => SetProperty(ref _uvYWorkVel, value);
        }

        [Category("ImprintPlatform"), Description("UV配置参数")]
        [DisplayName("UV配置参数")]
        public UVControlConfig UVConfig
        {
            get => _uvConfig?? new UVControlConfig();
            set => SetProperty(ref _uvConfig, value);
        }
        [Category("ImprintPlatform"), Description("力传感器配置参数")]
        [DisplayName("力传感器参数")]
        public ForceSensorControlConfig ForceSensorControlConfig
        {
            get => _forceSensorConfig;
            set => SetProperty(ref _forceSensorConfig, value);
        }


        [Category("PrintPlatform"), Description("UV移动过程中，Z方向相机发生碰撞的位置")]
        [DisplayName("Z方向安全位置")]
        public double UVZDirSafePosition
        {
            get => _uvZDirSafePosition;
            set => SetProperty(ref _uvZDirSafePosition, value);
        }



        //[Category("PrintPlatform"), Description("掩膜Z轴配置参数")]
        //[DisplayName("掩膜Z轴配置参数")]
        //public TrioAxisConfig MaskZAxisConfig { get; set; }

        //[Category("PrintPlatform"), Description("相机Z轴配置参数")]
        //[DisplayName("相机Z轴配置参数")]
        //public TrioAxisConfig CameraZAxisConfig { get; set; }


        //[Category("PrintPlatform"), Description("UVX轴配置参数")]
        //[DisplayName("UVX轴配置参数")]
        //public TrioAxisConfig UVXAxisConfig { get; set; }

        //[Category("PrintPlatform"), Description("UVY轴配置参数")]
        //[DisplayName("UVY轴配置参数")]
        //public TrioAxisConfig UVYAxisConfig { get; set; }
    }
}
