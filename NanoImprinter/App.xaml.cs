using NanoImprinter.Model;
using NanoImprinter.Views;
using Prism.Ioc;
using Prism.Modularity;
using Serilog;
using Serilog.Events;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using WestLakeShape.Common.LogService;

namespace NanoImprinter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IObservable<LogEvent> _logEventStream;
        private ISubject<LogEvent> _logEventSubject = new Subject<LogEvent>();

        public IObservable<LogEvent> LogEventStream => _logEventStream;

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
            containerRegistry.RegisterForNavigation<OtherView>();
            containerRegistry.RegisterSingleton<IMachineModel, MachineModel>();
            containerRegistry.RegisterSingleton<ProcedureManager>();
           
            //containerRegistry.RegisterSingleton<ILogService, LogService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _logEventStream = _logEventSubject.AsObservable();
           
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(@"D:\Logs\log-.txt", rollingInterval: RollingInterval.Day,
                              fileSizeLimitBytes:10000000, //10MB
                              retainedFileCountLimit:7,    //保留7天
                              outputTemplate:"[{Timestamp:HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Observers(o=>o.Subscribe(_logEventSubject))
                .CreateLogger();
            base.OnStartup(e);
        }
        //protected override void OnInitialized()
        //{
        //    base.OnInitialized();
        //    var loggerConfigguany
        //}
        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("退出程序");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
