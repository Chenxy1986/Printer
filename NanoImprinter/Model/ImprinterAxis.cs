using System;
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
        private Timer _timerCallBack;
        
        private ImprinterAxisConfig _config;

        private TrioAxis _afmPlatformXAxis;// AFM平台
        private TrioAxis _afmPlatformYAxis;

        //private ZmcAxis _gluePlatformXAxis;//点胶X轴
        //private ZmcAxis _gluePlatformYAxis;//点胶Y轴
        private TrioAxis _gluePlatformZAxis;// 点胶平台

        private TrioAxis _macroPlatformXAxis;// 宏动平台
        private TrioAxis _macroPlatformYAxis;
        private TrioAxis _macroPlatformRAxis;

        //private TrioAxis _microPlatformRXAxis;// 微动平台
        //private TrioAxis _microPlatformRYAxis;
        //private TrioAxis _microPlatformZAxis;

        private TrioAxis _imprintPlatformMaskZAxis;// 压印Z轴
        private TrioAxis _imprintPlatformCameraAxis;//压印相机轴

        private TrioAxis _uvPlatformXAxis;//UV平台轴
        private TrioAxis _uvPlatformZAxis; 

        public ImprinterAxis(ImprinterAxisConfig config)
        {
            _config = config;
           
            _afmPlatformXAxis = new TrioAxis(config.AfmPlatformXAxis);
            _afmPlatformYAxis = new TrioAxis(config.AfmPlatformYAxis);

            //_gluePlatformXAxis = new ZmcAxis(config.GluePlatformXAxis);
            //_gluePlatformYAxis = new ZmcAxis(config.GluePlatformYAxis);
            _gluePlatformZAxis = new TrioAxis(config.GluePlatformZAxis);

            _macroPlatformXAxis = new TrioAxis(config.MacroPlatformXAxis);
            _macroPlatformYAxis = new TrioAxis(config.MacroPlatformYAxis);
            _macroPlatformRAxis = new TrioAxis(config.MacroPlatformRAxis);

            //_microPlatformRXAxis = new TrioAxis(config.MicroPlatformRXAxis);
            //_microPlatformRYAxis = new TrioAxis(config.MicroPlatformRYAxis);
            //_microPlatformZAxis = new TrioAxis(config.MicroPlatformZAxis);

            _imprintPlatformMaskZAxis = new TrioAxis(config.ImprintPlatformMaskZAxis);
            _imprintPlatformCameraAxis = new TrioAxis(config.ImprintPlatformCameraZAxis);

            _uvPlatformXAxis = new TrioAxis(config.UVPlatformXAxis);
            _uvPlatformZAxis = new TrioAxis(config.UVPlatformZAxis);

        }

        public List<IAxis> All()
        {
            return new List<IAxis>() {
            _afmPlatformXAxis,
            _afmPlatformYAxis,

            _gluePlatformZAxis,

            _macroPlatformXAxis,
            _macroPlatformYAxis,
            _macroPlatformRAxis,

             _imprintPlatformMaskZAxis,
             _imprintPlatformCameraAxis,

             _uvPlatformXAxis,
             _uvPlatformZAxis
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
               //_gluePlatformXAxis,
               //_gluePlatformYAxis,
               _gluePlatformZAxis,
            };
        }

        public IAxis[] AFMPlatformAxes()
        {
            return new IAxis[]
                {
                   _afmPlatformXAxis,
                   _afmPlatformYAxis,
                };
        }

        public IAxis[] PrintPlatformAxes()
        {
            return new IAxis[]
                {
                    _imprintPlatformMaskZAxis,
                    _imprintPlatformCameraAxis,
                      _uvPlatformXAxis,
                    _uvPlatformZAxis,
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
        /// AFM平台
        /// </summary>
        public TrioAxisConfig AfmPlatformXAxis { get; set; } = new TrioAxisConfig() { Name = "X轴" };
        public TrioAxisConfig AfmPlatformYAxis { get; set; } = new TrioAxisConfig() { Name = "Y轴" };

        /// <summary>
        /// 点胶平台
        /// </summary>
        //public ZmcAxisConfig GluePlatformXAxis { get; set; }
        //public ZmcAxisConfig GluePlatformYAxis { get; set; }
        public TrioAxisConfig GluePlatformZAxis { get; set; } = new TrioAxisConfig() { Name = "Z轴" };


        /// <summary>
        /// 宏动平台
        /// </summary>
        public TrioAxisConfig MacroPlatformXAxis { get; set; } = new TrioAxisConfig() { Name = "X轴" };
        public TrioAxisConfig MacroPlatformYAxis { get; set; } = new TrioAxisConfig() { Name = "Y轴" };
        public TrioAxisConfig MacroPlatformRAxis { get; set; } = new TrioAxisConfig() { Name = "R轴" };

        /// <summary>
        /// 压印平台
        /// </summary>
        public TrioAxisConfig ImprintPlatformMaskZAxis { get; set; } = new TrioAxisConfig() { Name = "掩膜Z轴" };
        public TrioAxisConfig ImprintPlatformCameraZAxis { get; set; } = new TrioAxisConfig() { Name = "相机Z轴" };

        /// <summary>
        /// UV平台
        /// </summary>
        public TrioAxisConfig UVPlatformXAxis { get; set; } = new TrioAxisConfig() { Name = "UV X轴" };
        public TrioAxisConfig UVPlatformZAxis { get; set; } = new TrioAxisConfig() { Name = "UV Z轴" };
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
