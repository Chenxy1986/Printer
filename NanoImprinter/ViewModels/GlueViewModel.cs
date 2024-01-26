using NanoImprinter.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestLakeShape.Motion;

namespace NanoImprinter.ViewModels
{
    public class GlueViewModel : BindableBase
    {
        private readonly IMachineModel _machine;
        private GluePlatform _gluePlatform;
        private GluePlatformConfig _config;

        private bool _isReady;       //Z轴是否Ready
        private bool _isAlaram;      //Z轴是否报警
        private bool _isConnected;   //点胶机是否连接

        private int _openTime;        //开阀时间
        private int _closeTime;       //关阀时间
        private int _openIntensity;   //开阀力度
        private int _temperature;     //当前温度
        private double _waitPosition;
        private double _gluePosition;
        private double _workVel;


        #region property
        public bool IsReady 
        {
            get => _isReady; 
            set => SetProperty(ref _isReady, value); 
        }

        public bool IsAlarm
        {
            get => _isAlaram;
            set => SetProperty(ref _isAlaram, value);
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }
        
        public double WaitPosition
        {
            get => _waitPosition;
            set => SetProperty(ref _waitPosition, value);
        }
        public double GluePosition
        {
            get => _gluePosition;
            set => SetProperty(ref _gluePosition, value);
        }
        public double WorkVel 
        {
            get => _workVel;
            set => SetProperty(ref _workVel, value);
        }
        public int OpenTime
        {
            get => _openTime;
            set => SetProperty(ref _openTime, value);
        }
        public int CloseTime
        {
            get => _closeTime;
            set => SetProperty(ref _closeTime, value);
        }

        public int OpenIntensity
        {
            get => _openIntensity;
            set => SetProperty(ref _openIntensity, value);
        }

        public int Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        public ObservableCollection<IAxis> Axes { get; set; }
        #endregion

        #region command

        public DelegateCommand GoHomeCommand => new DelegateCommand(GoHome).ObservesCanExecute(() => IsReady);
        public DelegateCommand ClearAlarmCommand => new DelegateCommand(ResetAlarm).ObservesCanExecute(() => IsAlarm);
        public DelegateCommand MoveToWaitPositionCommand => new DelegateCommand(MoveToWaitPosition).ObservesCanExecute(() => IsReady);
        public DelegateCommand MoveToGluePositionCommand => new DelegateCommand(MoveToTakePicturePosition).ObservesCanExecute(() => IsReady);
        public DelegateCommand SaveParamCommand => new DelegateCommand(SaveParam);
        public DelegateCommand ReloadParamCommand => new DelegateCommand(ReloadParam);
        public DelegateCommand GlueCommmand => new DelegateCommand(Glue).ObservesCanExecute(()=>IsConnected);

        #endregion

        public GlueViewModel(IMachineModel machine)
        {
            _machine = machine;
            _gluePlatform = _machine.GetPlatform(typeof(GluePlatform).Name) as GluePlatform;
            Axes = new ObservableCollection<IAxis>();
            Axes.Add(_gluePlatform.ZAxis);
        }


        public void GoHome()
        {
            _gluePlatform.GoHome();
        }
        public void ResetAlarm()
        {
            _gluePlatform.ResetAxesAlarm();
        }

        public void MoveToWaitPosition()
        {
            _gluePlatform.MoveToWaitPosition();
        }

        public void MoveToTakePicturePosition()
        {
            _gluePlatform.MoveToGluePosition();
        }

        public void SaveParam()
        {
            _machine.Config.GluePlatform.WaitPosition = WaitPosition;
            _machine.Config.GluePlatform.GluePosition = GluePosition;
            _machine.Config.GluePlatform.WorkVel = WorkVel;

            _machine.Config.GluePlatform.GlueConfig.OpenValveTime = OpenTime;
            _machine.Config.GluePlatform.GlueConfig.OpenValveIntensity = OpenIntensity;
            _machine.Config.GluePlatform.GlueConfig.ClosedValveTime = CloseTime;
            _machine.Config.GluePlatform.GlueConfig.TargetTemperatore = Temperature;
            _machine.SaveParam();
        }
        public void ReloadParam()
        {
            WaitPosition = _machine.Config.GluePlatform.WaitPosition;
            GluePosition = _machine.Config.GluePlatform.GluePosition;
            WorkVel = _machine.Config.GluePlatform.WorkVel;
            OpenTime = _machine.Config.GluePlatform.GlueConfig.OpenValveTime;
            OpenIntensity = _machine.Config.GluePlatform.GlueConfig.OpenValveIntensity;
            CloseTime = _machine.Config.GluePlatform.GlueConfig.ClosedValveTime;
            Temperature = _machine.Config.GluePlatform.GlueConfig.TargetTemperatore;
        }

        public void Glue()
        {
            _gluePlatform.Glue();
        }

    }
}
