﻿using Microsoft.Win32;
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
using WestLakeShape.Common;
using static WestLakeShape.Motion.IOStateSource;

namespace NanoImprinter.ViewModels
{
    public class OtherViewModel : BindableBase
    {
        private readonly IMachineModel _machine;
        private MaskInfo _maskInfo;
        private string _configFileName;

        public string ConfigFileName
        {
            get => _configFileName;
            set => SetProperty(ref _configFileName, value);
        }

        public DelegateCommand<string> SetOutputValueCommand => new DelegateCommand<string>(SetOutputValue);
        public DelegateCommand OpenFileCommand => new DelegateCommand(OpenFile);
        public DelegateCommand ReloadParamCommand => new DelegateCommand(LoadParam);
        public DelegateCommand SaveParamCommand => new DelegateCommand(SaveParam);
        public DelegateCommand SaveConfigFileNameCommand => new DelegateCommand(SaveConfigFileName);
        public ImprinterIO IOStates { get;private set; }
        
        public OtherViewModel(IMachineModel machine)
        {
            _machine = machine;
            IOStates = machine.IOStates;
            _maskInfo = _machine.Config.MaskInfo;
            ConfigFileName = _machine.ConfigFileName;
        }

        private void SetOutputValue(string stateName)
        {
            IOStates.SetValue(stateName);
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Config files(*.config)|*.config";
            openFileDialog.InitialDirectory = Constants.ConfigRootFolder;

            if (openFileDialog.ShowDialog() == true)
            {
                ConfigFileName = openFileDialog.FileName;
               
            }
        }

        private void LoadParam()
        {
            _machine.LoadParam();
        }

        private void SaveParam()
        {
            _machine.SaveParam();
        }
        private void SaveConfigFileName()
        {
            if (!_configFileName.Contains(".config"))
            {
                if (!_configFileName.Contains("."))
                {
                    ConfigFileName = string.Concat(_configFileName, ".config");
                }
                else
                {
                    var dotIndex = _configFileName.IndexOf(".");
                    ConfigFileName = _configFileName.Substring(0, dotIndex) + ".config";
                }
            }
            _machine.ConfigFileName = ConfigFileName;
        }
    }
}
