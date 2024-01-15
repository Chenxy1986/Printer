using System.ComponentModel;
using System.Threading.Tasks;

namespace WestLakeShape.Motion
{
    public abstract class Axis<TAxisConfig> : IAxis<TAxisConfig>
        where TAxisConfig : AxisConfig
    {
        public abstract double Position { get; }

        public abstract double Speed { get; }

        public TAxisConfig Config { get; set; }

        public string Name { get; }

        public Axis(TAxisConfig config)
        {
            Config = config;
        }

        public abstract bool GoHome();

       
        public abstract bool MoveTo(double position);

        public abstract void ServoOff();

        public abstract void ServoOn();

        public abstract void Stop();
    }

    public class AxisConfig
    {
        [Category("Axis"), Description("当前速度"), DefaultValue(10)]
        [RefreshProperties(RefreshProperties.All)]
        [DisplayName("当前速度")]
        public double Speed { get; set; } = 10;

        [Category("Axis"), Description("轴号"), DefaultValue(1)]
        [DisplayName("轴号")]
        public int Index { get; set; } = 0;

        [Category("Axis"), Description("轴名字")]
        [DisplayName("轴名字")]
        public string Name { get; set; }

        [Category("Axis"), Description("板卡名字")]
        [DisplayName("板卡名字")]
        public short CardIndex { get; set; } = 1;
    }
}
