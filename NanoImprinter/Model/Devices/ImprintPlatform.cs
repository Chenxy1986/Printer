using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestLakeShape.Common;
using WestLakeShape.Motion;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.Model
{
    public interface IImprintPlatform
    {
        ImprintPlatformConfig Config { get; set; }
        //bool GoHome();
        bool MoveToPreprintHeight();
        bool MoveToTakePictureHeight();
        void ResetAxesAlarm();
    }

    public class ImprintPlatform:IImprintPlatform,IPlatform
    {
        private ImprintPlatformConfig _config;
        private IAxis _maskZAxis;
        private IAxis _cameraZAxis;
        private IAxis _uvXAxis;
        private IAxis _uvYAxis;

        public ImprintPlatformConfig Config
        {
            get => _config;
            set => _config = value;
        }


        public ImprintPlatform(ImprintPlatformConfig config,IAxis[] axes)
        {
            _config = config;
            _maskZAxis = axes[0];
            _cameraZAxis = axes[1];
            _uvXAxis = axes[2];
            _uvYAxis = axes[3];
            //_maskZAxis = new TrioAxis(_config.MaskZAxisConfig);
            //_cameraZAxis = new TrioAxis(_config.CameraZAxisConfig);
            //_uvXAxis = new TrioAxis(_config.UVXAxisConfig);
            //_uvYAxis = new TrioAxis(_config.UVYAxisConfig);
        }

        public bool GoHome()
        {
            return _maskZAxis.GoHome();
        }


        public bool MoveToPreprintHeight()
        {
            return ZAxisMoveBy(_config.PreprintHeight);
        }

        public bool MoveToTakePictureHeight()
        {
            return  CameraAxisMoveBy(_config.TakePictureHeight);
        }


        private bool ZAxisMoveBy(double position)
        {
            return  _maskZAxis.MoveTo(position);
        }
        private bool CameraAxisMoveBy(double position)
        {
            return _cameraZAxis.MoveTo(position);
        }

        public void ResetAxesAlarm()
        {
            ((TrioAxis)_maskZAxis).ResetAlarm();
            ((TrioAxis)_cameraZAxis).ResetAlarm();
        }
    }

    public class ImprintPlatformConfig
    {
        [Category("PrintPlatform"), Description("掩膜预压印高度")]
        [DisplayName("掩膜预压印高度")]
        public double PreprintHeight { get; set; }

        [Category("PrintPlatform"), Description("压印速度")]
        [DisplayName("压印速度")]
        public double ImprintSpeed { get; set; }



        [Category("PrintPlatform"), Description("拍照高度")]
        [DisplayName("拍照高度")]
        public double TakePictureHeight { get; set; }

        [Category("PrintPlatform"), Description("移动拍照位速度")]
        [DisplayName("移动拍照位速度")]
        public double CameraMoveSpeed { get; set; }


        [Category("PrintPlatform"), Description("UV等待位")]
        [DisplayName("UV等待位")]
        public PointXZ WaitPosition { get; set; }

        [Category("PrintPlatform"), Description("UV照射位")]
        [DisplayName("UV照射位")]
        public PointXZ IrradiationPosition { get; set; }

        [Category("PrintPlatform"), Description("UVX轴工作速度")]
        [DisplayName("UVX轴工作速度")]
        public double UVXWorkSpeed { get; set; }

        [Category("PrintPlatform"), Description("UVY轴工作速度")]
        [DisplayName("UVY轴工作速度")]
        public double UVYWorkSpeed { get; set; }

        [Category("PrintPlatform"), Description("UV配置参数")]
        [DisplayName("UV配置参数")]
        public UVControlConfig UVConfig { get; set; }


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
