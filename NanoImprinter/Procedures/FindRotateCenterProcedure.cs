using NanoImprinter.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        protected override async Task<bool> OnExcute()
        {
            var model = new NanoImprinterModel();
            //移动到圆心左监测点 
            await model.MacroPlatform.MoveTo(model.MacroPlatform.Config.LeftCenterPoint).ConfigureAwait(false);

            //等待相机检测到圆心


            //移动到圆心右监测点
            await model.MacroPlatform.MoveTo(model.MacroPlatform.Config.RightCenterPoint).ConfigureAwait(false);

            //等待相机检测到圆心


            //移动到圆心上监测点
            await model.MacroPlatform.MoveTo(model.MacroPlatform.Config.UpCenterPoint).ConfigureAwait(false);

            //等待相机检测到圆心


            //移动到圆心下监测点
            await model.MacroPlatform.MoveTo(model.MacroPlatform.Config.DownCenterPoint).ConfigureAwait(false);

            //等待相机检测到圆心


            ///图像算法获取圆心位置
            model.Config.WafeInfo.Center = new Point2D(1.0, 1.0);


            return true;
        }

        protected override async Task<bool> Prepare()
        {
            Debug.WriteLine($"Prepare {typeof(FindRotateCenterProcedure)}");
            await Task.Delay(5);
            return true;
        }

    }
}
