using NanoImprinter.Model;
using NanoImprinter.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace NanoImprinter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AfmCameraView>();
            containerRegistry.RegisterForNavigation<CameraView>();
            containerRegistry.RegisterForNavigation<GlueView>();
            containerRegistry.RegisterForNavigation<ImprintView>();
            containerRegistry.RegisterForNavigation<MacroView>();
            containerRegistry.RegisterForNavigation<MainView>();
            containerRegistry.RegisterForNavigation<MicroView>();
            containerRegistry.RegisterSingleton<IMachineModel, MachineModel>();
            containerRegistry.RegisterSingleton<ProcedureManager>();
        }
    }
}
