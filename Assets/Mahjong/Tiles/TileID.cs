using System;
using System.Collections.Generic;

namespace Mahjong
{

    public class TileID
    {
        //Identifiers
        public enum Suits : byte
        {
            Man,
            Pin,
            Sou,
            Kaze,
            Sangen
        }
        public const byte TON = 2;
        public const byte NAN = 3;
        public const byte SHAA = 4;
        public const byte PEI = 5;
        public const byte CHUN = 2;
        public const byte HAKU = 3;
        public const byte HATSU = 4;

        public static readonly TileID Invalid = new TileID(Suits.Man, 0);
        public static readonly TileID Hidden = new TileID(Suits.Pin, 0);
        public static readonly TileID HiddenHand = new TileID(Suits.Sou, 0);


        //Fields
        public Suits Suit { get; private set; }
        public byte Number { get; private set; }
        public bool Routou { get; private set; } //terminal?
        public bool Jihai { get; private set; } //honor?
        public bool Aka { get; private set; } //red?



        //Constructor
        public TileID(Suits suit, byte number, bool aka = false)
        {
            Suit = suit;
            Number = number;
            if (Number > 9)
            {
                Number = Invalid.Number;
                Suit = Invalid.Suit;
            }
            else
            {
                if (Number == 1 || Number == 9) Routou = true;
                else Routou = false;
                if (Suit == Suits.Kaze || Suit == Suits.Sangen) Jihai = true;
                else Jihai = false;
                Aka = aka;
            }
        }

        //Comparison operators
        public static bool operator ==(TileID t1, TileID t2)
        {
            if (t1.Suit != t2.Suit) return false;
            if (t1.Number != t2.Number) return false;
            return true;
        }
        public static bool operator !=(TileID t1, TileID t2)
        {
            return !(t1 == t2);
        }
        public override bool Equals(object other)
        {
            if (other == null) return false;
            return (this == (TileID)other);
        }
        public override int GetHashCode()
        {
            return (((byte)Suit << 4) | Number);
        }
        //Inequality operators are for the standard hand ordering used by player and AI
        public static bool operator <(TileID t1, TileID t2)
        {
            HandSortingMethod m = HandSortingMethod.Default;
            if (m.SuitOrder[t1.Suit] < m.SuitOrder[t2.Suit]) return true;
            else if (m.SuitOrder[t1.Suit] > m.SuitOrder[t2.Suit]) return false;
            else
            {
                if (t1.Number < t2.Number) return true;
                else if (t1.Number > t2.Number) return false;
                else
                {
                    if (t1.Aka == false && t2.Aka == true) return true;
                    else return false;
                }
            }
        }
        public static bool operator >(TileID t1, TileID t2)
        {
            return (t2 < t1);
        }
        public static bool operator <=(TileID t1, TileID t2)
        {
            return (t1 < t2 || t1 == t2);
        }
        public static bool operator >=(TileID t1, TileID t2)
        {
            return (t2 < t1 || t1 == t2);
        }

        //Returns the dora if this tile is the indicator
        public TileID GetDoraFromIndicator()
        {
            byte number = (byte)(Number + 1);
            if (number == 10) number = 1;
            if (Suit == Suits.Kaze && Number == PEI) number = TON;
            if (Suit == Suits.Sangen && Number == HATSU) number = CHUN;
            return new TileID(Suit, number);
        }

        //Creates a copy of the tile. Used for access safety
        public TileID Copy()
        {
            TileID tile = new TileID(this.Suit, this.Number, this.Aka);
            return tile;
        }

        //Returns true if two tiles are in the same suji
        public static bool InSameSuji(TileID t1, TileID t2)
        {
            if (t1.Number == 0 || t2.Number == 0) return false;
            if (t1.Suit != t2.Suit) return false;
            if (Math.Abs(t1.Number - t2.Number) > 2) return false;
            return true;
        }

        //For ToString()
        private static string[] kazeStr = new string[4]
        {
            "Ton (east wind)",
            "Nan (south wind)",
            "Shaa (west wind)",
            "Pei (north wind)"
        };
        private static string[] kazeStr_short = new string[4]
        {
            /*
            "Ton",
            "Nan",
            "Shaa",
            "Pei"
            */ //TODO: this is for test showcasing only
            "East wind",
            "South wind",
            "West wind",
            "North wind"
        };
        private static string[] sangenStr = new string[3]
        {
            "Chun (red dragon)",
            "Haku (white dragon)",
            "Hatsu (green dragon)"
        };
        private static string[] sangenStr_short = new string[3]
        {
            /*
            "Chun",
            "Haku",
            "Hatsu"
            */
            //TODO: this is for test showcasing only
            "Red dragon",
            "White dragon",
            "Green dragon"
        };
        private static string[] numberStr = new string[9]
        {
            "Ii",
            "Ni",
            "San",
            "Suu",
            "Uu",
            "Ryuu",
            "Chii",
            "Paa",
            "Chuu"
        };

        //Returns the representation of the tile as a string
        public override string ToString()
        {
            if (Number == 0) return "Invalid";
            string r = "";
            if (Suit == Suits.Kaze)
            {
                r = kazeStr[Number - 2];
            }
            else if (Suit == Suits.Sangen)
            {
                r = sangenStr[Number - 2];
            }
            else
            {
                r = numberStr[Number - 1];
                if (Suit == Suits.Man)
                    r += "man (" + Number + "-man)";
                else if (Suit == Suits.Pin)
                    r += "pin (" + Number + "-pin)";
                else if (Suit == Suits.Sou)
                    r += "sou (" + Number + "-sou)";
            }
            return r;
        }

        //Returns a shorter string version
        public string ToShortString()
        {
            if (Number == 0) return "Invalid";
            string r = "";
            if (Suit == Suits.Kaze)
            {
                r = kazeStr_short[Number - 2];
            }
            else if (Suit == Suits.Sangen)
            {
                r = sangenStr_short[Number - 2];
            }
            else
            {
                r += Number;
                if (Suit == Suits.Man)
                    r += "-man";
                else if (Suit == Suits.Pin)
                    r += "-pin";
                else if (Suit == Suits.Sou)
                    r += "-sou";
            }
            return r;
        }
    }
}
