using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static WestLakeShape.Motion.IOStateSource;

namespace NanoImprinter.ViewModels
{
    public class OtherViewModel
    {
        private readonly IMachineModel _machine;
        public DelegateCommand<string> SetOutputValueCommand => new DelegateCommand<string>(SetOutputValue);
        public ImprinterIO IOStates { get;private set; }
        public OtherViewModel(IMachineModel machine)
        {
            _machine = machine;
            IOStates = machine.IOStates;
        }
        private void SetOutputValue(string stateName)
        {
            IOStates.SetValue(stateName);
        }
    }
}
