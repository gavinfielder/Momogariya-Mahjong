using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    //For better descriptions see http://arcturus.su/wiki/Machi
    public enum WaitType : byte
    {
        //Non-tenpai waits
        Ryanmen = 1, //2-sided shuntsu wait
        Penchan = 2, //terminal shuntsu wait for a 3 or 7
        Kanchan = 4, //inner shuntsu wait
        Kantsu = 5, //completed triplet waiting on potential fourth
        Koutsu = 6, //Completed pair waiting on potential third

        //Tenpai waits are all greater than 10
        Nobetan = 11, //4-tile sequence either end pair wait
        Sanmenchan = 12, //5-tile sequence 3-tile wait
        Sanmentan = 13, //7-tile sequence 3-tile wait
        Entotsu = 14, //shuntsu/koutsu and pair 3-tile wait
        Aryanmen = 15, //shuntsu+endpair 2-tile wait
        Ryantan = 16, //shuntsu/koutsu 3-tile wait eg 4555 waiting on 346
        Pentan = 17, //same as above but on terminal eg 1222 waiting on 13
        Kantan = 18, //same as above but inner wait eg 3555 waiting on 34
        Kantankan = 19, //eg 3335777 waiting on 456
        Tatsumaki = 20, //eg 4445666 waiting on 34567
        Happoubijin = 21, //eg 2223456777 waiting on 12345678
        Shanpon = 22, //double-pair wait
        Tanki = 23, //single-tile pair wait
        RyanmenTenpai = 24,
        PenchanTenpai = 25,
        KanchanTenpai = 26,

        //Special Yakuman patterns starting greater than 30
        ChuurenPoutouKyuumenMachi = 31,
        KokushiMusou13Machi = 32,
    }

    public struct Wait
    {
        public WaitType type;
        public TileID tile;

        public Wait(WaitType t, TileID id)
        {
            tile = id;
            type = t;
        }
    }

    //Allows a wait or wait type to be printed
    public static class WaitPrinter
    {
        private static bool initialized = false;
        private static Dictionary<WaitType, string> waitTypeStrings;

        private static void Initialize()
        {
            if (initialized) return;
            waitTypeStrings = new Dictionary<WaitType, string>();
            waitTypeStrings.Add(WaitType.Ryanmen, "Ryanmen");
            waitTypeStrings.Add(WaitType.Penchan, "Penchan");
            waitTypeStrings.Add(WaitType.Shanpon, "Shanpon");
            waitTypeStrings.Add(WaitType.Kanchan, "Kanchan");
            waitTypeStrings.Add(WaitType.Tanki, "Tanki");
            waitTypeStrings.Add(WaitType.Nobetan, "Nobetan");
            waitTypeStrings.Add(WaitType.Sanmenchan, "Sanmenchan");
            waitTypeStrings.Add(WaitType.Sanmentan, "Sanmentan");
            waitTypeStrings.Add(WaitType.Entotsu, "Entotsu");
            waitTypeStrings.Add(WaitType.Aryanmen, "Aryanmen");
            waitTypeStrings.Add(WaitType.Ryantan, "Ryantan");
            waitTypeStrings.Add(WaitType.Pentan, "Pentan");
            waitTypeStrings.Add(WaitType.Kantan, "Kantan");
            waitTypeStrings.Add(WaitType.Kantankan, "Kantankan");
            waitTypeStrings.Add(WaitType.Tatsumaki, "Tatsumaki");
            waitTypeStrings.Add(WaitType.Happoubijin, "Happoubijin");
            waitTypeStrings.Add(WaitType.ChuurenPoutouKyuumenMachi, "ChuurenPoutouKyuumenMachi");
            waitTypeStrings.Add(WaitType.KokushiMusou13Machi, "KokushiMusou13Machi");
            waitTypeStrings.Add(WaitType.Kantsu, "Kantsu");
            waitTypeStrings.Add(WaitType.Koutsu, "Koutsu");
            initialized = true;
        }

        public static string WaitTypeString(WaitType type)
        {
            if (!(initialized)) Initialize();
            //return waitTypeStrings[type];
            return "Waiting"; //TODO: this is for test showcasing only
        }

    }
}
