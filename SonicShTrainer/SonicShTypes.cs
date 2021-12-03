using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicShTrainer
{
    static class Goodies
    {
        private enum Fields
        {
            Sonic0 = 0x1,
            Sonic1 = 0x2,
            Sonic2 = 0x4,
            SonicPasses = 0x7,
            BusPass1 = 0x8,
            BusPass2 = 0x10,
            Key = 0x20,
        }

        public static uint AddSonicPass(uint raw) => raw | (uint)Fields.SonicPasses;
        public static uint AddBusPass(uint raw) => raw | (uint)Fields.BusPass1 | (uint)Fields.BusPass2;
        public static uint AddKey(uint raw) => raw | (uint)Fields.Key;
    }
    class SonicShTypes
    {
    }
}
