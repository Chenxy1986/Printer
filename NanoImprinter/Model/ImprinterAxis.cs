﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WestLakeShape.Motion;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.Model
{
    public class ImprinterAxis
    {
        private TrioAxis _imprintPlatformMaskZAxis;// 压印Z轴
        private TrioAxis _imprintPlatformCameraAxis;//压印相机轴
        //private ZmcAxis _gluePlatformXAxis;//点胶X轴
        //private ZmcAxis _gluePlatformYAxis;//点胶Y轴
        private TrioAxis _gluePlatformZAxis;// 点胶平台

        private TrioAxis _afmPlatformXAxis;// AFM平台
        private TrioAxis _afmPlatformYAxis;
        private TrioAxis _afmPlatformZAxis;//

        private TrioAxis _uvPlatformXAxis;//UV平台轴
        
        private TrioAxis _macroPlatformRAxis;// 宏动平台
        private TrioAxis _macroPlatformYAxis;
        private TrioAxis _macroPlatformXAxis;
        
        //private TrioAxis _microPlatformRXAxis;// 微动平台
        //private TrioAxis _microPlatformRYAxis;
        //private TrioAxis _microPlatformZAxis;

        private TrioControl _trioControl;
        private ImprinterAxisConfig _config;
        private bool _isConnected;

        public bool IsConnected => _isConnected;

        public ImprinterAxis(ImprinterAxisConfig config)
        {
            _config = config;

            _macroPlatformXAxis = new TrioAxis(config.MacroPlatformXAxis);
            _macroPlatformYAxis = new TrioAxis(config.MacroPlatformYAxis);
            _macroPlatformRAxis = new TrioAxis(config.MacroPlatformRAxis);
            _imprintPlatformCameraAxis = new DCAxis(config.ImprintPlatformCameraZAxis);
            _imprintPlatformMaskZAxis = new DCAxis(config.ImprintPlatformMaskZAxis);
            _gluePlatformZAxis = new DCAxis(config.GluePlatformZAxis);
            _uvPlatformXAxis = new DCAxis(config.UVPlatformXAxis);
            _afmPlatformXAxis = new DCAxis(config.AfmPlatformXAxis);
            _afmPlatformYAxis = new DCAxis(config.AfmPlatformYAxis);
            _afmPlatformZAxis = new DCAxis(config.AfmPlatformZAxis);

            _trioControl = TrioControl.Instance;
        }

        public void Connected()
        {
           //打开控制器
           var isOpened = _trioControl.Connected();
            if (isOpened)
            {
                var axes = All();
                axes.ForEach(o =>
                {
                    o.InitialParameter();
                    o.ServoOn();
                });
            }
        }

        public void Disconnected()
        {
            _isConnected = _trioControl.Disconnected();
        }

        private void Initial()
        {
            var axes = All();
            _macroPlatformYAxis.InitialParameter();     
        }

        public List<IAxis> All()
        {
            return new List<IAxis>() {
             _macroPlatformXAxis,
             _macroPlatformYAxis,
             _macroPlatformRAxis,
             _imprintPlatformCameraAxis,
             _imprintPlatformMaskZAxis,
             _gluePlatformZAxis,
             _uvPlatformXAxis,
             _afmPlatformXAxis,
             _afmPlatformYAxis,
             _afmPlatformZAxis,//原来UV2轴更改为AFM3轴
             };
        }

        public IAxis[] MacroPlatformAxes()
        {
            return new IAxis[]
            {
               _macroPlatformXAxis,
               _macroPlatformYAxis,
               _macroPlatformRAxis,
            };
        }

        public IAxis[] GluePlatformAxes()
        {
            return new IAxis[]
            {
               _gluePlatformZAxis,
            };
        }

        public IAxis[] AFMPlatformAxes()
        {
            return new IAxis[]
                {
                   _afmPlatformXAxis,
                   _afmPlatformYAxis,
                   _afmPlatformZAxis,
                };
        }

        public IAxis[] PrintPlatformAxes()
        {
            return new IAxis[]
                {
                    _imprintPlatformMaskZAxis,
                    _imprintPlatformCameraAxis,
                     _uvPlatformXAxis,
                };
        }

        public List<IAxis> DCAxes()
        {
            return new List<IAxis>() {
             _imprintPlatformMaskZAxis,
             _imprintPlatformCameraAxis,
             _uvPlatformXAxis,
             _gluePlatformZAxis,
             _afmPlatformXAxis,
             _afmPlatformYAxis,
             _afmPlatformZAxis,
            };
        }

        public List<IAxis> ACAxes()
        {
            return new List<IAxis>() {
             _macroPlatformXAxis,
             _macroPlatformYAxis,
             _macroPlatformRAxis,
            };
        }
    
        //public void RefreshAxisValue(object state)
        //{
        //    var axes = All();
        //    for(int i=0; i<axes.Count; i++)
        //    {
        //        if (_positions[i] != axes[i].Position)
        //        {
        //            OnUpdateStatus(axes[i].Name, axes[i].Position);
        //        }
        //    }
        //}

        //public event EventHandler<AxisStatusEventArgs> UpdataStatusEvent;
        //protected virtual void OnUpdateStatus(string name,double position)
        //{
        //    UpdataStatusEvent?.Invoke(this, new AxisStatusEventArgs(name,position));
        //}
    
    }


    public class ImprinterAxisConfig
    {
        /// <summary>
        /// 宏动平台
        /// </summary>
        public TrioAxisConfig MacroPlatformXAxis { get; set; } = new TrioAxisConfig() { Name = "宏动平台X轴", Index = 0 };
        //X2轴不需要设置
        //public TrioAxisConfig MacroPlatformX2Axis { get; set; } = new TrioAxisConfig() { Name = "宏动平台X2轴", Index = 1 };

        public TrioAxisConfig MacroPlatformYAxis { get; set; } = new TrioAxisConfig() { Name = "宏动平台Y轴", Index = 2 };
        public TrioAxisConfig MacroPlatformRAxis { get; set; } = new TrioAxisConfig() { Name = "宏动平台R轴", Index = 3 };
        public DCAxisConfig ImprintPlatformCameraZAxis { get; set; } = new DCAxisConfig() { Name = "相机Z轴", Index = 4 };
        /// <summary>
        /// 压印平台
        /// </summary>
        public DCAxisConfig ImprintPlatformMaskZAxis { get; set; } = new DCAxisConfig() { Name = "掩膜Z轴", Index = 5};
        /// <summary>
        /// 点胶平台
        /// </summary>
        //public ZmcAxisConfig GluePlatformXAxis { get; set; }
        //public ZmcAxisConfig GluePlatformYAxis { get; set; }
        public DCAxisConfig GluePlatformZAxis { get; set; } = new DCAxisConfig() { Name = "点胶平台Z轴", Index = 6 };
        /// <summary>
        /// UV平台
        /// </summary>
        public DCAxisConfig UVPlatformXAxis { get; set; } = new DCAxisConfig() { Name = "UV平台X轴", Index = 7 };

        /// <summary>
        /// AFM平台
        /// </summary>
        public DCAxisConfig AfmPlatformXAxis { get; set; } = new DCAxisConfig() { Name = "AFM平台X轴",Index= 8 };
        public DCAxisConfig AfmPlatformYAxis { get; set; } = new DCAxisConfig() { Name = "AFM平台Y轴",Index=9 };
        public DCAxisConfig AfmPlatformZAxis { get; set; } = new DCAxisConfig() { Name = "AFM平台Z轴", Index = 10 };
    }





    public class AxisStatusEventArgs : EventArgs
    {
        public double CurrentPosition { get; private set; }
        public string Name { get; private set; }

        public AxisStatusEventArgs(string name,double currentPosition)
        {
            Name = name;
            CurrentPosition = currentPosition;
        }
    }
}
