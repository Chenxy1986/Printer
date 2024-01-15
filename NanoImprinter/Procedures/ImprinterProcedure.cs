using NanoImprinter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WestLakeShape.Common;

namespace NanoImprinter.Procedures
{
    /// <summary>
    /// 压印流程
    /// </summary>
    public class ImprinterProcedure : WorkProcedure
    {
        private NanoImprinterModel _model;

        public ImprinterProcedure()
        {
            //_model = NanoImprinterModel.Instance;
        }

        protected override bool OnExcute()
        {
            ///移动到预压印位置
            _model.ImprintPlatform.MoveToPreprintHeight();

            //移动到相机拍照位置
            _model.ImprintPlatform.MoveToTakePictureHeight();

            //等待相机拍照完成并计算偏移值
            var offsetValue = new PointXYR(5, 5, 0);

            //宏动平台移动补偿值
            _model.MacroPlatform.MoveTo(offsetValue);

            //微动平台向上移动，等待压力传感器达到设定值停止。

            return true;
        }
        protected override bool Prepare()
        {
            Thread.Sleep(1);
            return true;
        }
    }
}
