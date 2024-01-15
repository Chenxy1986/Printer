using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestLakeShape.Common;
using WestLakeShape.Motion.Device;

namespace NanoImprinter.Model
{
    public class NanoImprinterModel:IMachineModel
    {
        private const string Config_File_Name = "MachineConfig.config";
        private readonly string _rootFolder = "C:\\NanoImprinterConfig";

        

        /// <summary>
        /// Afm测量平台
        /// </summary>
        public AfmPlatform AfmPlatform { get; private set; }

        /// <summary>
        /// 点胶平台
        /// </summary>
        public GluePlatform GluePlatform { get; private set; }

        /// <summary>
        /// 宏动平台
        /// </summary>
        public MacroPlatform MacroPlatform { get; private set; }

        /// <summary>
        /// 微动平台
        /// </summary>
        public MicroPlatform MicroPlatform { get; private set; }

        /// <summary>
        /// 打印平台
        /// </summary>
        public ImprintPlatform ImprintPlatform { get; private set; }

        /// <summary>
        /// 所有IO卡
        /// </summary>
        public ImprinterIO IO { get; private set; }

        /// <summary>
        /// 所有轴
        /// </summary>
        public ImprinterAxis Axes { get; private set; }

        public NanoImprinterModelConfig Config { get; set; }

        public MachineStatus Status { get; set; }

        public Dictionary<string, IPlatform> Platforms { get; private set; }

        //public static NanoImprinterModel Instance { get; private set; }

        public NanoImprinterModel()
        {
            LoadParam();
        }


        public void SaveParam()
        {
            var path = _rootFolder + Config_File_Name;
            var folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var config = new NanoImprinterModelConfig
            {
                MacroPlatform = MacroPlatform.Config,
                MicroPlatform = MicroPlatform.Config,
                ImprinterIO = IO.Config
            };
            File.WriteAllText(path, JsonConvert.SerializeObject(config), Constants.Encoding);
        }

        public void LoadParam()
        {
            //var model = new NanoImprinterModel();
            var configFile = _rootFolder + Config_File_Name;
            NanoImprinterModelConfig config;
            if (Directory.Exists(configFile))
            {
                var content = File.ReadAllText(configFile, Constants.Encoding);
                config = JsonConvert.DeserializeObject<NanoImprinterModelConfig>(content);
            }
            else
            {
                Directory.CreateDirectory(configFile);
                config = new NanoImprinterModelConfig();
            }

            IO = new ImprinterIO(config.ImprinterIO);
            Axes = new ImprinterAxis(config.ImprinterAxis);
            //MacroPlatform = new MacroPlatform(config.MacroPlatform,
            //                               Axes.MacroPlatformAxes());
            //MicroPlatform = new MicroPlatform(config.MicroPlatform);
            //AfmPlatform = new AfmPlatform(config.AfmPlatform);
            //GluePlatform = new GluePlatform(config.GluePlatform,
            //                             Axes.GluePlatformAxes());
            //ImprintPlatform = new ImprintPlatform(config.ImprintPlatform,
            //                               Axes.PrintPlatformAxes());

            Platforms.Add(typeof(ImprintPlatform).Name, new ImprintPlatform(config.ImprintPlatform,
                                                                            Axes.PrintPlatformAxes()));
            Platforms.Add(typeof(GluePlatform).Name, new GluePlatform(config.GluePlatform,
                                                                      Axes.GluePlatformAxes()));
            Platforms.Add(typeof(AfmPlatform).Name, new AfmPlatform(config.AfmPlatform));
            Platforms.Add(typeof(MicroPlatform).Name, new MicroPlatform(config.MicroPlatform));
            Platforms.Add(typeof(MacroPlatform).Name, new MacroPlatform(config.MacroPlatform,
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

    public class NanoImprinterModelConfig
    {

        public AfmPlatformConfig AfmPlatform { get; set; }

        public GluePlatformConfig GluePlatform { get; set; }


        public ImprinterAxisConfig ImprinterAxis { get; set; }


        public ImprinterIOConfig ImprinterIO { get; set; }


        public MacroPlatformConfig MacroPlatform { get; set; }


        public MicroPlatformConfig MicroPlatform { get; set; }


        public ImprintPlatformConfig ImprintPlatform { get; set; }

        public WafeInfo WafeInfo { get; set; }

        public MaskInfo MaskInfo { get; set; }
    }

    public enum MachineStatus
    {
        Manual,
        Auto,
        Running,
        Paused,
        Emergency,
    }
}
