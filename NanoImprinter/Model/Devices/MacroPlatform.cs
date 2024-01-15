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
    public interface IMacroPlatform
    {
        MacroPlatformConfig Config { get; set; }
        bool MoveTo(PointXYR offsetValue);
        bool MoveTo(Point2D offsetValue);
        //bool GoHome();
        bool MoveToLoadPosition();
        bool MoveToPosition(PointXYR pos);
        bool MoveToPressurePosition();
        bool MoveToGluePosition();
        void ResetAxesAlarm();
    }

    public class MacroPlatform:IMacroPlatform,IPlatform
    {
        public IAxis XAxis { get; }
        public IAxis YAxis { get; }
        public IAxis RAxis { get; }

        public MacroPlatformConfig Config
        {
            get; set;
        }

        public MacroPlatform(MacroPlatformConfig config,IAxis[] axes)
        {
            Config = config;
            XAxis = axes[0];
            YAxis = axes[1];
            RAxis = axes[2];
            //XAxis = new TrioAxis(Config.XAxisConfig);
            //YAxis = new TrioAxis(Config.YAxisConfig);
            //RAxis = new TrioAxis(Config.RAxisConfig);
        }

        public List<IAxis> Axes()
        {
            return new List<IAxis>
            {
                XAxis,
                YAxis,
                RAxis
            };
        }

        /// <summary>
        /// XY方向和R方向的移动
        /// </summary>
        /// <param name="offsetValue"></param>
        /// <returns></returns>
        public bool MoveTo(PointXYR offsetValue)
        {
            return MoveBy(offsetValue.X,
                          offsetValue.Y,
                          offsetValue.R);
        }

        /// <summary>
        /// XY方向的移动
        /// </summary>
        /// <param name="offsetValue"></param>
        /// <returns></returns>
        public bool MoveTo(Point2D offsetValue)
        {
            return MoveBy(offsetValue.X,
                          offsetValue.Y,
                          0);
        }

        /// <summary>
        /// 宏动平台回零
        /// </summary>
        /// <returns></returns>
        public bool GoHome()
        {
            var xMovement = Task.Run(()=> XAxis.GoHome());
            var yMovement = Task.Run(() => YAxis.GoHome());
            var rMovement = Task.Run(() => RAxis.GoHome());
           
            Task.WaitAll(xMovement, yMovement, rMovement);
            
            return true;
        }

        public bool MoveToLoadPosition()
        {
            return MoveBy(Config.LoadPosition.X,
                          Config.LoadPosition.Y,
                          0);
        }

        public bool MoveToPosition(PointXYR pos)
        {
            return MoveBy(pos.X,
                          pos.Y,
                          pos.R);
        }

        public bool MoveToPressurePosition()
        {
            return MoveBy(Config.ImprintPosition.X,
                          Config.ImprintPosition.Y,
                          0);
        }

        public bool MoveToGluePosition()
        {
            return MoveBy(Config.GluePosition.X,
                          Config.GluePosition.Y,
                          0);
        }

        public void ResetAxesAlarm()
        {
            ((TrioAxis)XAxis).ResetAlarm();
            ((TrioAxis)YAxis).ResetAlarm();
            ((TrioAxis)RAxis).ResetAlarm();
        }

        private bool MoveBy(double x, double y, double r)
        {
            var xMovement = Task.Run(() => XAxis.MoveTo(x));
            var yMovement = Task.Run(() => YAxis.MoveTo(y));
            var rMovement = Task.Run(() => RAxis.MoveTo(r));
            Task.WaitAll(xMovement, yMovement, rMovement);
            return true;
        }


    }


    public class MacroPlatformConfig
    {
        [Category("MacroPlatform"), Description("放晶圆位置")]
        [DisplayName("放料位置")]
        public Point2D LoadPosition { get; set; }

        [Category("MacroPlatform"), Description("点胶位置")]
        [DisplayName("点胶位置")]
        public Point2D GluePosition { get; set; }


        [Category("MacroPlatform"), Description("压印位置")]
        [DisplayName("压印位置")]
        public Point2D ImprintPosition { get; set; }

        [Category("MacroPlatform"), Description("圆心左监测点")]
        [DisplayName("圆心左监测点")]
        public Point2D LeftCenterPoint { get; set; }

        [Category("MacroPlatform"), Description("圆心右监测点")]
        [DisplayName("圆心右监测点")]
        public Point2D RightCenterPoint { get; set; }


        [Category("MacroPlatform"), Description("圆心上监测点")]
        [DisplayName("圆心上监测点")]
        public Point2D UpCenterPoint { get; set; }

        [Category("MacroPlatform"), Description("圆心下监测点")]
        [DisplayName("圆心下监测点")]
        public Point2D DownCenterPoint { get; set; }


        //public TrioAxisConfig XAxisConfig { get; set; }
        //public TrioAxisConfig YAxisConfig { get; set; }
        //public TrioAxisConfig RAxisConfig { get; set; }
    }
}
