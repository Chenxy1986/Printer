using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WestLakeShape.Common;
using WestLakeShape.Common.WpfCommon;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.Model
{
    public interface IMachineModel
    {
        Dictionary<string, IPlatform> Platforms { get; }
        
        /// <summary>
        /// 上次流程执行到的步骤
        /// </summary>
        int ProcedureIndex { get; set; }

        ImprinterIO IOStates { get;}
        ImprinterAxis Axes { get; }

        MachineModelConfig Config { get; }
        void LoadParam();
        void SaveParam();
        IPlatform GetPlatform(string name);
    }

    public class MachineModel : IMachineModel
    {
        private readonly string Config_File_Name = "MachineConfig.config";
        private readonly string _rootFolder = @"D:\NanoImprinterConfig\";
        
        /// <summary>
        /// 所有IO卡
        /// </summary>
        public ImprinterIO IOStates { get; private set; }
        /// <summary>
        /// 所有轴
        /// </summary>
        public ImprinterAxis Axes { get; private set; }

        /// <summary>
        /// 上次流程执行到的步骤
        /// </summary>
        public int ProcedureIndex { get; set; }

        public MachineModelConfig Config { get; private set; }

        public Dictionary<string, IPlatform> Platforms { get; private set; }

        public MachineModel()
        {
            Platforms = new Dictionary<string, IPlatform>();
            LoadParam();
        }


        public void SaveParam()
        {
            var path = _rootFolder + Config_File_Name;
            var folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            File.WriteAllText(path, JsonConvert.SerializeObject(Config,Formatting.Indented), Constants.Encoding);
        }

        public void LoadParam()
        {
            //var model = new NanoImprinterModel();
            var configFile = Path.Combine(_rootFolder, Config_File_Name);
          
            if (File.Exists(configFile))
            {
                var content = File.ReadAllText(configFile, Constants.Encoding);
                Config = JsonConvert.DeserializeObject<MachineModelConfig>(content);
            }
            else
            {
                Config = new MachineModelConfig();
                SaveParam();
            }

            IOStates = new ImprinterIO(Config.ImprinterIO);
            Axes = new ImprinterAxis(Config.ImprinterAxis);
            //MacroPlatform = new MacroPlatform(config.MacroPlatform,
            //                               Axes.MacroPlatformAxes());
            //MicroPlatform = new MicroPlatform(config.MicroPlatform);
            //AfmPlatform = new AfmPlatform(config.AfmPlatform);
            //GluePlatform = new GluePlatform(config.GluePlatform,
            //                             Axes.GluePlatformAxes());
            //ImprintPlatform = new ImprintPlatform(config.ImprintPlatform,
            //                               Axes.PrintPlatformAxes());

            Platforms.Add(typeof(ImprintPlatform).Name, new ImprintPlatform(Config.ImprintPlatform,
                                                                            Axes.PrintPlatformAxes()));
            Platforms.Add(typeof(GluePlatform).Name, new GluePlatform(Config.GluePlatform,
                                                                      Axes.GluePlatformAxes()));
            Platforms.Add(typeof(AfmPlatform).Name, new AfmPlatform(Config.AfmPlatform));
            Platforms.Add(typeof(MicroPlatform).Name, new MicroPlatform(Config.MicroPlatform));
            Platforms.Add(typeof(MacroPlatform).Name, new MacroPlatform(Config.MacroPlatform,
                                                                        Axes.MacroPlatformAxes()));
            //Instance = model;lei
        }

        public IPlatform GetPlatform(string type)
        {
            if (!Platforms.TryGetValue(type, out var platform))
            {
                return null;
            }
            return platform;
        }
    }

    public class MachineModelConfig
    {
        public WafeInfo WafeInfo { get; set; } = new WafeInfo();
        public MaskInfo MaskInfo { get; set; } = new MaskInfo();

        public ImprinterAxisConfig ImprinterAxis { get; set; } = new ImprinterAxisConfig();
        public ImprinterIOConfig ImprinterIO { get; set; } = new ImprinterIOConfig();

        public AfmPlatformConfig AfmPlatform { get; set; } = new AfmPlatformConfig();
        public GluePlatformConfig GluePlatform { get; set; } = new GluePlatformConfig();
        public MacroPlatformConfig MacroPlatform { get; set; } = new MacroPlatformConfig();
        public MicroPlatformConfig MicroPlatform { get; set; } = new MicroPlatformConfig();
        public ImprintPlatformConfig ImprintPlatform { get; set; } = new ImprintPlatformConfig();
      
    }

}
