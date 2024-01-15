using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace NanoImprinter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IMachineModel _devices;

        public DelegateCommand<string> NavigateCommand { get; private set; }

        public MainWindowViewModel(IRegionManager regionManager,IMachineModel device)
        {
            _regionManager = regionManager;
            _devices = device;

            NavigateCommand = new DelegateCommand<string>(Navigate);
        }
        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate("ContentRegion", navigatePath);
        }
    }
}
