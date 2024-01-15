using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanoImprinter.Procedures
{
    /// <summary>
    /// 脱模流程
    /// </summary>
    public class ReleaseProcedure : WorkProcedure
    {
        protected override bool OnExcute()
        {
            Thread.Sleep(1);
            return true;
        }
        protected override bool Prepare()
        {
            Thread.Sleep(1);
            return true;
        }
    }
}
