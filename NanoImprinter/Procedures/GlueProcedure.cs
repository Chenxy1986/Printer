using NanoImprinter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanoImprinter.Procedures
{
    /// <summary>
    /// 点胶流程
    /// </summary>
    public class GlueProcedure : WorkProcedure
    {
        protected override bool OnExcute()
        {
            var model = new NanoImprinterModel();

            //宏动平台移动点胶位置
            model.MacroPlatform.MoveToGluePosition();

            //点胶平台Z轴移动到点胶高度
            model.GluePlatform.MoveToGluePosition();

            //执行点胶
            model.GluePlatform.Glue();

            //点胶平台移动到等待位置
            model.GluePlatform.MoveToWaitPosition();

            return true;
        }
        protected override bool Prepare()
        {
            Thread.Sleep(1);
            return true;
        }
    }
}
