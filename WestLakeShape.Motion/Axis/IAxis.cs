using System.Threading.Tasks;

namespace WestLakeShape.Motion
{
    public interface IAxis
    {
        string Name { get; }
        /// <summary>
        /// 当前位置
        /// </summary>
        double Position { get; }

        double Speed { get; }

        void ServoOn();

        void ServoOff();

        bool GoHome();

        bool MoveTo(double position);

        void Stop();
        void EmergencyStop();
        void ResetAlarm();
    }

    public interface IAxis<TAxisConfig> : IAxis
    where TAxisConfig : AxisConfig
    {
    }

}
