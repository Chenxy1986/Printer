using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoImprinter.Model
{
    public interface IMachineModel
    {
        Dictionary<string, IPlatform> Platforms { get; }

        NanoImprinterModelConfig Config { get; set; }
        void LoadParam();
        void SaveParam();
        IPlatform GetPlatform(string name);
    }
}
