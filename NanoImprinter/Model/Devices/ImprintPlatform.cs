using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

    public class ImprintPlatform:IImprintPlatform,IPlatform
    {
        private ImprintPlatformConfig _config;
        private IAxis _maskZAxis;
        private IAxis _cameraZAxis;
        private IAxis _uvXAxis;
        private IAxis _uvZAxis;
        private ForceSensorControl _forceControl;
        private UVControl _uvControl;

        public ImprintPlatformConfig Config
        {
            get => _config;
            set => _config = value;
        }

        public IAxis MaskZAxis => _maskZAxis;
        public IAxis CameraZAxis => _cameraZAxis;
        public IAxis UVXAxis => _uvXAxis;
        public IAxis UVZAxis => _uvZAxis;


        public ImprintPlatform(ImprintPlatformConfig config,IAxis[] axes)
        {
            _config = config;
            _maskZAxis = axes[0];
            _cameraZAxis = axes[1];
            _uvXAxis = axes[2];
            _uvZAxis = axes[3];
            _forceControl = new ForceSensorControl("com1");
            _uvControl = new UVControl(_config.UVConfig);
            //_maskZAxis = new TrioAxis(_config.MaskZAxisConfig);
            //_cameraZAxis = new TrioAxis(_config.CameraZAxisConfig);
            //_uvXAxis = new TrioAxis(_config.UVXAxisConfig);
            //_uvYAxis = new TrioAxis(_config.UVYAxisConfig);
        }

        public bool GoHome()
        {
            var czTask = Task.Run(()=> _cameraZAxis.GoHome());
            var uxTask = Task.Run(()=>_uvXAxis.GoHome());
            var uyTask = Task.Run(()=> _uvZAxis.GoHome());
            Task.WaitAll(czTask, uxTask, uyTask);
            return _maskZAxis.GoHome();
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
            if (_uvXAxis.Position > _config.XDirSafePosition)
                throw new Exception("UV未离开冲突区域，移动相机会发生碰撞");
            if (_maskZAxis.Position < _config.TakePictureHeight)
                throw new Exception("掩膜高度太低，移动相机会发生碰撞");

            return MoveBy(_cameraZAxis,_config.TakePictureHeight);
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

        public double[] GetForceValue()
        {
           return _forceControl.RefreshValue();
        }

        public bool UVIrradiate()
        {
            _uvControl.TurnOn();
            Thread.Sleep(_config.UVConfig.PreheatTime);
            _uvControl.TurnOff();
            return true;
        }
    }

    public class ImprintPlatformConfig:NotifyPropertyChanged
    {
        private double _maskWaitHeight;
        private double _maskPrepintHeight;
        private double _maskZWorkVel;
        private double _cameraWaitHeight;
        private double _takePictureHeight;
        private double _cameraZWorkVel;
        private double _xDirSafePosition;
        private PointXZ _uvWaitPosition;
        private PointXZ _uvIrradiationPosition;
        private double _uvXWorkVel;
        private double _uvYWorkVel;
        private UVControlConfig _uvConfig = new UVControlConfig();
        private double _uvZDirSafePosition;

        //掩膜组件
        [Category("PrintPlatform"), Description("掩膜等待高度")]
        [DisplayName("掩膜等待高度")]
        public double MaskWaitHeight 
        {
            get => _maskWaitHeight;
            set => SetProperty(ref _maskWaitHeight, value);
        }

        //掩膜组件
        [Category("PrintPlatform"), Description("掩膜预压印高度")]
        [DisplayName("掩膜预压印高度")]
        public double MaskPreprintHeight 
        {
            get => _maskPrepintHeight;
            set => SetProperty(ref _maskPrepintHeight, value);
        }

        [Category("PrintPlatform"), Description("压印速度")]
        [DisplayName("压印速度")]
        public double MaskZWorkVel
        {
            get => _maskZWorkVel;
            set => SetProperty(ref _maskZWorkVel, value);
        }


        //拍照组件参数
        [Category("PrintPlatform"), Description("等待拍照高度")]
        [DisplayName("等待拍照高度")]
        public double CameraWaitHeight
        {
            get => _cameraWaitHeight;
            set => SetProperty(ref _cameraWaitHeight, value);
        }

        [Category("PrintPlatform"), Description("拍照高度")]
        [DisplayName("拍照高度")]
        public double TakePictureHeight
        {
            get => _takePictureHeight;
            set => SetProperty(ref _takePictureHeight, value);
        }

        [Category("PrintPlatform"), Description("移动拍照位速度")]
        [DisplayName("移动拍照位速度")]
        public double CameraZWorkVel
        {
            get => _cameraZWorkVel;
            set => SetProperty(ref _cameraZWorkVel, value);
        }

        [Category("PrintPlatform"), Description("相机移动过程中，X方向UV发生碰撞的位置")]
        [DisplayName("X方向安全位置")]
        public double XDirSafePosition
        {
            get => _xDirSafePosition;
            set => SetProperty(ref _xDirSafePosition, value);
        }


        //UV组件参数
        [Category("PrintPlatform"), Description("UV等待位")]
        [DisplayName("UV等待位")]
        public PointXZ UVWaitPosition 
        {
            get => _uvWaitPosition;
            set => SetProperty(ref _uvWaitPosition, value);
        }

        [Category("PrintPlatform"), Description("UV照射位")]
        [DisplayName("UV照射位")]
        public PointXZ UVIrradiationPosition
        {
            get => _uvIrradiationPosition;
            set => SetProperty(ref _uvIrradiationPosition, value);
        }

        [Category("PrintPlatform"), Description("UVX轴工作速度")]
        [DisplayName("UVX轴工作速度")]
        public double UVXWorkVel 
        {
            get => _uvXWorkVel;
            set => SetProperty(ref _uvXWorkVel, value);
        }

        [Category("PrintPlatform"), Description("UVY轴工作速度")]
        [DisplayName("UVY轴工作速度")]
        public double UVYWorkVel
        {
            get => _uvYWorkVel;
            set => SetProperty(ref _uvYWorkVel, value);
        }

        [Category("PrintPlatform"), Description("UV配置参数")]
        [DisplayName("UV配置参数")]
        public UVControlConfig UVConfig
        {
            get => _uvConfig;
            set => SetProperty(ref _uvConfig, value);
        } 

        [Category("PrintPlatform"), Description("UV移动过程中，Z方向相机发生碰撞的位置")]
        [DisplayName("X方向安全位置")]
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
