using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoImprinter.Procedures
{
    /// <summary>
    /// 脱模流程
    /// </summary>
    public class ReleaseProcedure : WorkProcedure
    {
        protected override async Task<bool> OnExcute()
        {
            await Task.Delay(1);
            return true;
        }
        protected override async Task<bool> Prepare()
        {
            await Task.Delay(1);
            return true;
        }
    }
}
