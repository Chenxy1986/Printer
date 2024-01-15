using NanoImprinter.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoImprinter.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IMachineModel _machine;
        
        public MainViewModel(IMachineModel machine)
        {
            _machine = machine;
            //_platform = _machine.GetPlatform(typeof(MacroPlatform).Name) as MacroPlatform;
        }
        
    }
}
