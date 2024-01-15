using NanoImprinter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WestLakeShape.Motion;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.Procedures
{
    /// <summary>
    /// 取料流程
    /// </summary>
    public class PickProcedure : WorkProcedure
    {
        public PickProcedure()
        {
            Name = ProcedureName.PickProcedure.ToString();
        }

        protected override bool OnExcute()
        {
            var model = new NanoImprinterModel();

            //移动到放料位
            model.MacroPlatform.MoveToLoadPosition();

            //等待取走晶圆
            model.IO.GetInputIO(ImprinterIOName.HasWafe);
            //间隔一定时间
            Thread.Sleep(10);

            //等待放入新的晶圆
            model.IO.GetInputIO(ImprinterIOName.HasWafe);

            return true;
        }


        protected override bool Prepare()
        {
            throw new NotImplementedException();
        }
    }
}
