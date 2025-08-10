using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Configurations.Parsers.TenMoon
{
    public struct TenMoonTabletReport : ITabletReport, IAuxReport
    {
        public TenMoonTabletReport(byte[] report)
        {
            Raw = report;

            // New way
            var x = report[1] << 8 | report[2];
            var y = report[3] << 8 | report[4];
            // don't ask me why it's like this
            if ((y & 0x8000) != 0)
                y = (ushort)(0x8F - (ushort)(y & 0x7FFF));
            else
                y += 0x8F;
            Position = new Vector2(x, y);
            // Old way
            // Position = new Vector2
            // {
            //     X = report[1] << 8 | report[2],
            //     Y = Math.Max((short)(report[3] << 8 | report[4]), (short)0)
            // };

            // New way
            ushort prePressure = (ushort)(report[5] << 8 | report[6]);
            // ushort calibratedMax = (ushort)(report[7] << 8 | report[8]);
            // I found this manually looking at the minimum values at
            // "prePressure".
            ushort calibratedMax = (ushort)(0x05 << 8 | 0xCE);
            ushort pressure = (ushort)(calibratedMax - prePressure);
            if ((pressure & 0x8000) != 0 || report[10] == 0x0F)
                pressure = 0;
            Pressure = pressure;

            // Old way
            // var buttonPressed = (report[9] & 6) != 0;
            // var prePressure = report[5] << 8 | report[6];
            // // Pressure = (uint)(0x0672 - (prePressure - (buttonPressed ? 50 : 0)));
            // Pressure = (uint)(1500 - (prePressure - (buttonPressed ? 50 : 0)));

            PenButtons = new bool[]
            {
                // new?
                (report[9] & 0b110) == 0b100,
                (report[9] & 0b110) == 0b110
                // working
                // report[9] == 4,
                // report[9] == 6
                // old
                // report[9].IsBitSet(2),
                // (report[9] & 6) == 6
            };

            AuxButtons = new bool[]
            {
                !report[12].IsBitSet(0), // Eighth
                !report[12].IsBitSet(5), // Nineth

                !report[12].IsBitSet(1), // First
                !report[12].IsBitSet(4), // Seventh

                !report[11].IsBitSet(6), // Third
                !report[11].IsBitSet(7), // Second

                !report[11].IsBitSet(5), // Fourth
                !report[11].IsBitSet(4), // Fifth

                // These buttons don't exist on my tablet hahaha
                !report[11].IsBitSet(3), // Sixth
                !report[11].IsBitSet(0), // Tenth
                !report[11].IsBitSet(1), // Eleventh
                !report[11].IsBitSet(2) // Twelveth
            };
        }

        public byte[] Raw { set; get; }
        public bool[] AuxButtons { set; get; }
        public Vector2 Position { set; get; }
        public uint Pressure { set; get; }
        public bool[] PenButtons { set; get; }
    }
}
