using NanoImprinter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoImprinter.Procedures
{
    /// <summary>
    /// 点胶流程
    /// </summary>
    public class GlueProcedure : WorkProcedure
    {
        protected override async Task<bool> OnExcute()
        {
            var model = new NanoImprinterModel();

            //宏动平台移动点胶位置
            await model.MacroPlatform.MoveToGluePosition().ConfigureAwait(false);

            //点胶平台Z轴移动到点胶高度
            await model.GluePlatform.MoveToGluePosition().ConfigureAwait(false);

            //执行点胶
            await model.GluePlatform.Glue().ConfigureAwait(false);

            //点胶平台移动到等待位置
            await model.GluePlatform.MoveToWaitPosition().ConfigureAwait(false);

            return true;
        }
        protected override async Task<bool> Prepare()
        {
            await Task.Delay(1);
            return true;
        }
    }
}
