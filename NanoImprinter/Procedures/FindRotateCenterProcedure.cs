using NanoImprinter.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WestLakeShape.Common;

namespace NanoImprinter.Procedures
{
    public class FindRotateCenterProcedure : WorkProcedure
    {
        private Point2D _wafeCenter;
        public FindRotateCenterProcedure()
        {
            _wafeCenter = new Point2D(0, 0);
        }
        protected override bool OnExcute()
        {
            var model = new NanoImprinterModel();
            //移动到圆心左监测点 
             model.MacroPlatform.MoveTo(model.MacroPlatform.Config.LeftCenterPoint);

            //等待相机检测到圆心


            //移动到圆心右监测点
            model.MacroPlatform.MoveTo(model.MacroPlatform.Config.RightCenterPoint);

            //等待相机检测到圆心


            //移动到圆心上监测点
            model.MacroPlatform.MoveTo(model.MacroPlatform.Config.UpCenterPoint);

            //等待相机检测到圆心


            //移动到圆心下监测点
            model.MacroPlatform.MoveTo(model.MacroPlatform.Config.DownCenterPoint);

            //等待相机检测到圆心


            ///图像算法获取圆心位置
            model.Config.WafeInfo.Center = new Point2D(1.0, 1.0);


            return true;
        }

        protected override bool Prepare()
        {
            Debug.WriteLine($"Prepare {typeof(FindRotateCenterProcedure)}");
            Thread.Sleep(1);
            return true;
        }

    }
}
