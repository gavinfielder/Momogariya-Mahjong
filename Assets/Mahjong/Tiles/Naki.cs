using System.Collections.Generic;

namespace Mahjong
{

    //Types of Naki. Enumerated by priority low to high
    public enum NakiType : byte
    {
        Nashi = 0, //No call aka continue
        Chii = 2,
        Pon = 3,
        Kan = 4,
        Ron = 6,
        Tsumo = 7
    }

    //Describes a tile discard call request
    public class Naki
    {
        public NakiType type;
        public PotentialMeld meld;
        public Player requestor;

        //For ToString()
        private static string[] typeStrings = new string[7]
        {
            "Nashi",
            "Error: NakiType = 1",
            "Chii",
            "Pon",
            "Kan",
            "Ron",
            "Tsumo"
        };

        //ToString
        public override string ToString()
        {
            int pn = 0; if (requestor != null) pn = requestor.PlayerNumber;
            string str = "Player " + pn + " calls " + typeStrings[(byte)type];
            return str;
        }

        //Static helper functions for lookup
        private static bool initialized = false;
        private static Dictionary<WaitType, NakiType> waitNaki;
        private static Dictionary<NakiType, TurnArgsType> nakiTurnArgs;

        public static NakiType GetNakiType(WaitType wait)
        {
            if (!(initialized)) Initialize();
            return waitNaki[wait];
        }

        public static TurnArgsType GetTurnArgs(NakiType naki)
        {
            if (!(initialized)) Initialize();
            return nakiTurnArgs[naki];
        }

        public static void Initialize()
        {
            waitNaki = new Dictionary<WaitType, NakiType>();

            waitNaki.Add(WaitType.Ryanmen, NakiType.Chii);
            waitNaki.Add(WaitType.Penchan, NakiType.Chii);
            waitNaki.Add(WaitType.Kanchan, NakiType.Chii);
            waitNaki.Add(WaitType.Koutsu, NakiType.Pon);
            waitNaki.Add(WaitType.Kantsu, NakiType.Kan);

            waitNaki.Add(WaitType.Nobetan, NakiType.Ron);
            waitNaki.Add(WaitType.Sanmenchan, NakiType.Ron);
            waitNaki.Add(WaitType.Sanmentan, NakiType.Ron);
            waitNaki.Add(WaitType.Entotsu, NakiType.Ron);
            waitNaki.Add(WaitType.Aryanmen, NakiType.Ron);
            waitNaki.Add(WaitType.Ryantan, NakiType.Ron);
            waitNaki.Add(WaitType.Pentan, NakiType.Ron);
            waitNaki.Add(WaitType.Kantan, NakiType.Ron);
            waitNaki.Add(WaitType.Kantankan, NakiType.Ron);
            waitNaki.Add(WaitType.Tatsumaki, NakiType.Ron);
            waitNaki.Add(WaitType.Happoubijin, NakiType.Ron);
            waitNaki.Add(WaitType.Shanpon, NakiType.Ron);
            waitNaki.Add(WaitType.Tanki, NakiType.Ron);
            waitNaki.Add(WaitType.RyanmenTenpai, NakiType.Ron);
            waitNaki.Add(WaitType.PenchanTenpai, NakiType.Ron);
            waitNaki.Add(WaitType.KanchanTenpai, NakiType.Ron);
            waitNaki.Add(WaitType.ChuurenPoutouKyuumenMachi, NakiType.Ron);
            waitNaki.Add(WaitType.KokushiMusou13Machi, NakiType.Ron);

            nakiTurnArgs = new Dictionary<NakiType, TurnArgsType>();

            nakiTurnArgs.Add(NakiType.Chii, TurnArgsType.Chii);
            nakiTurnArgs.Add(NakiType.Kan, TurnArgsType.Daiminkan);
            nakiTurnArgs.Add(NakiType.Nashi, TurnArgsType.Default);
            nakiTurnArgs.Add(NakiType.Pon, TurnArgsType.Pon);

            initialized = true;
        }
    }
}
