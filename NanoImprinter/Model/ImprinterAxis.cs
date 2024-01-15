using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestLakeShape.Motion;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.Model
{
    public class ImprinterAxis
    {
        private ImprinterAxisConfig _config;

        private TrioAxis _afmPlatformXAxis;// AFM平台
        private TrioAxis _afmPlatformYAxis;

        //private ZmcAxis _gluePlatformXAxis;//点胶X轴
        //private ZmcAxis _gluePlatformYAxis;//点胶Y轴
        private TrioAxis _gluePlatformZAxis;// 点胶平台

        private TrioAxis _macroPlatformXAxis;// 宏动平台
        private TrioAxis _macroPlatformYAxis;
        private TrioAxis _macroPlatformRAxis;

        private TrioAxis _microPlatformRXAxis;// 微动平台
        private TrioAxis _microPlatformRYAxis;
        private TrioAxis _microPlatformZAxis;

        private TrioAxis _printPlatformZAxis;// 压印Z轴
        private TrioAxis _printPlatformCameraAxis;//压印相机轴

        private TrioAxis _uvPlatformXAxis;//UV平台轴
        private TrioAxis _uvPlatformZAxis;


        public ImprinterAxis(ImprinterAxisConfig config)
        {
            _config = config;

            _afmPlatformXAxis = new TrioAxis(config.AFMPlatformXAxis);
            _afmPlatformYAxis = new TrioAxis(config.AFMPlatformYAxis);

            //_gluePlatformXAxis = new ZmcAxis(config.GluePlatformXAxis);
            //_gluePlatformYAxis = new ZmcAxis(config.GluePlatformYAxis);
            _gluePlatformZAxis = new TrioAxis(config.GluePlatformZAxis);

            _macroPlatformXAxis = new TrioAxis(config.MacroPlatformXAxis);
            _macroPlatformYAxis = new TrioAxis(config.MacroPlatformYAxis);
            _macroPlatformRAxis = new TrioAxis(config.MacroPlatformRAxis);

            _microPlatformRXAxis = new TrioAxis(config.MicroPlatformRXAxis);
            _microPlatformRYAxis = new TrioAxis(config.MicroPlatformRYAxis);
            _microPlatformZAxis = new TrioAxis(config.MicroPlatformZAxis);

            _printPlatformZAxis = new TrioAxis(config.PrintPlatformZAxis);
            _printPlatformCameraAxis = new TrioAxis(config.PrintPlatformCameraAxis);

            _uvPlatformXAxis = new TrioAxis(config.UVPlatformXAxis);
            _uvPlatformZAxis = new TrioAxis(config.UVPlatformZAxis);
        }

        public IAxis[] MicroPlatformAxes()
        {
            return new IAxis[]
            {
                _microPlatformRXAxis,
                _microPlatformRYAxis,
                _microPlatformZAxis,
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
                    _printPlatformZAxis,
                    _printPlatformCameraAxis,
                };
        }

        public IAxis[] UVPlatformAxes()
        {
            return new IAxis[]
                {
                    _uvPlatformXAxis,
                    _uvPlatformZAxis,
                };
        }

    }

    public class ImprinterAxisConfig
    {
        /// <summary>
        /// AFM平台
        /// </summary>
        public TrioAxisConfig AFMPlatformXAxis { get; set; }
        public TrioAxisConfig AFMPlatformYAxis { get; set; }


        /// <summary>
        /// 点胶平台
        /// </summary>
        //public ZmcAxisConfig GluePlatformXAxis { get; set; }
        //public ZmcAxisConfig GluePlatformYAxis { get; set; }
        public TrioAxisConfig GluePlatformZAxis { get; set; }


        /// <summary>
        /// 宏动平台
        /// </summary>
        public TrioAxisConfig MacroPlatformXAxis { get; set; }
        public TrioAxisConfig MacroPlatformYAxis { get; set; }
        public TrioAxisConfig MacroPlatformRAxis { get; set; }


        /// <summary>
        /// 微动平台
        /// </summary>
        public TrioAxisConfig MicroPlatformRXAxis { get; set; }
        public TrioAxisConfig MicroPlatformRYAxis { get; set; }
        public TrioAxisConfig MicroPlatformZAxis { get; set; }


        /// <summary>
        /// 压印平台
        /// </summary>
        public TrioAxisConfig PrintPlatformZAxis { get; set; }
        public TrioAxisConfig PrintPlatformCameraAxis { get; set; }

        /// <summary>
        /// UV平台
        /// </summary>
        public TrioAxisConfig UVPlatformXAxis { get; set; }
        public TrioAxisConfig UVPlatformZAxis { get; set; }
    }
}
