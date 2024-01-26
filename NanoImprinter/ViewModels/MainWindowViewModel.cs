using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace NanoImprinter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IMachineModel _machine;
        private readonly ProcedureManager _manager;
        public DelegateCommand<string> NavigateCommand { get; private set; }

        public MainWindowViewModel(IRegionManager regionManager,IMachineModel machine,ProcedureManager manager)
        {
            _regionManager = regionManager;
            _machine = machine;
            _manager = manager;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }
        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate("ContentRegion", navigatePath);
        }


     
    }
}
