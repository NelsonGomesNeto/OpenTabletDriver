using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Configurations.Parsers.TenMoon
{
    public class TenMoonReportParser : IReportParser<IDeviceReport>
    {
        public IDeviceReport Parse(byte[] report)
        {
            // // This is buggy since blocks using the tablet when any of the
            // // aux buttons are used hahaha
            // if (report[11] != 0xFF)
            //     return new TenMoonAuxReport(report);
            // else
            return new TenMoonTabletReport(report);
        }
    }
}
