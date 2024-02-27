using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows;
using WestLakeShape.Motion;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IMachineModel _machine;
        private readonly ProcedureManager _manager;

        private double _currentPosition;
        
        public double CurrentPostion { get; private set; }
        
        public DelegateCommand<string> NavigateCommand { get; private set; }

        public MainWindowViewModel(IRegionManager regionManager,
            IMachineModel machine,
            ProcedureManager manager)
        {
            _regionManager = regionManager;
            _machine = machine;
            _manager = manager;
            NavigateCommand = new DelegateCommand<string>(Navigate);
            _machine.Axes.UpdataStatusEvent += RefreshAxesStatus;   
        }
        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate("ContentRegion", navigatePath);
        }

        private void RefreshAxesStatus(Object sender, AxisStatusEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (e.Name)
                {
                    case "轴1":
                        
                        break;
                    case "轴2":

                        break;
                    case "轴3":

                        break;
                    case "轴4":

                        break;
                    case "轴5":

                        break;
                    case "轴6":

                        break;
                    case "轴7":

                        break;
                    case "轴8":

                        break;
                    case "轴9":

                        break;
                    case "轴10":

                        break;
                    case "轴11":

                        break;
                    case "轴12":

                        break;
                }
            });
        }
    
    }
}
