using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{

    //Player turn arguments based on type of draw
    public enum TurnArgsType : byte
    {
        Default = 1,
        Chii = 2,
        Pon = 3,
        Daiminkan = 4,
        KanContinue = 5
    }

    public class TurnArgs
    {
        public TurnArgsType type;
        public Naki naki;

        public static TurnArgs Default = new TurnArgs();

        public TurnArgs()
        {
            type = TurnArgsType.Default;
            naki = new Naki() { type = NakiType.Nashi };
        }

        public TurnArgs(TurnArgsType t, Naki n)
        {
            type = t;
            naki = n;
        }

        //For ToString
        public static string[] typeStrings = new string[6]
        {
            "Error: TurnArgsType = 0",
            "Default",
            "Chii",
            "Pon",
            "Daiminkan",
            "KanContinue"
        };

        public override string ToString()
        {
            return typeStrings[(byte)type] + "with Naki: " + naki;
        }
    }
}
