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
        private GluePlatform _plate;
        private GluePlatformConfig _config;

        private bool _isReady;       //Z轴是否Ready
        private bool _isAlaram;      //Z轴是否报警
        private bool _isConnected;   //点胶机是否连接
        
        private double _waitPosition;//Z轴等待位
        private double _gluePosition;//Z轴点胶位
        private double _workVel;     //Z轴移动速度

        private int _openTime;        //开阀时间
        private int _closeTime;       //关阀时间
        private int _openIntensity;   //开阀力度
        private int _temperature;     //当前温度

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

        private DelegateCommand _goHomeCommand;
        private DelegateCommand _clearAlarmCommand;
        private DelegateCommand _moveToWaitPositionCommand;
        private DelegateCommand _moveToGluePositionCommand;
        private DelegateCommand _saveParamCommand;
        private DelegateCommand _glueCommand;

        public DelegateCommand GoHomeCommand => _goHomeCommand ?? new DelegateCommand(GoHome).ObservesCanExecute(() => IsReady);
        public DelegateCommand ClearAlarmCommand => _clearAlarmCommand ?? new DelegateCommand(ResetAlarm).ObservesCanExecute(() => IsAlarm);
        public DelegateCommand MoveToWaitPositionCommand => _moveToWaitPositionCommand ?? new DelegateCommand(MoveToWaitPosition).ObservesCanExecute(() => IsReady);
        public DelegateCommand MoveToGluePositionCommand => _moveToGluePositionCommand ?? new DelegateCommand(MoveToTakePicturePosition).ObservesCanExecute(() => IsReady);
        public DelegateCommand SaveParamCommand => _saveParamCommand ?? new DelegateCommand(SaveParam);
        public DelegateCommand GlueCommmand => _glueCommand ?? new DelegateCommand(Glue).ObservesCanExecute(()=>IsConnected);

        #endregion

        public GlueViewModel(IMachineModel machine)
        {
            _machine = machine;
            _plate = _machine.GetPlatform(typeof(GluePlatform).Name) as GluePlatform;
            Axes = new ObservableCollection<IAxis>();
            Axes.Add(_plate.ZAxis);
        }


        public void GoHome()
        {
            _plate.GoHome();
        }
        public void ResetAlarm()
        {
            _plate.ResetAxesAlarm();
        }

        public void MoveToWaitPosition()
        {
            _plate.MoveToWaitPosition();
        }

        public void MoveToTakePicturePosition()
        {
            _plate.MoveToGluePosition();
        }

        public void SaveParam()
        {
            _plate.Config.WaitPosition = WaitPosition;
            _plate.Config.GluePosition = GluePosition;
            _plate.Config.GlueConfig.OpenValveTime = OpenTime;
            _plate.Config.GlueConfig.OpenValveIntensity = OpenIntensity;
            _plate.Config.GlueConfig.ClosedValveTime = CloseTime;
            _plate.Config.GlueConfig.TargetTemperatore = Temperature;
        }

        public void Glue()
        {
            _plate.Glue();
        }

    }
}
