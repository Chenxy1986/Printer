using NanoImprinter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected override async Task<bool> OnExcute()
        {
            var model = new NanoImprinterModel();

            //移动到放料位
            model.MacroPlatform.MoveToLoadPosition();

            //等待取走晶圆
            await model.IO.GetInputIO(ImprinterIOName.HasWafe).AsyncWait(false);
            //间隔一定时间
            await Task.Delay(100);

            //等待放入新的晶圆
            await model.IO.GetInputIO(ImprinterIOName.HasWafe).AsyncWait(false);

            return true;
        }


        protected override Task<bool> Prepare()
        {
            throw new NotImplementedException();
        }
    }
}
