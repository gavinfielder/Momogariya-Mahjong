using System;
using System.Collections.Generic;

namespace Mahjong
{
    public struct HandSortingMethod
    {
        public Dictionary<TileID.Suits, int> SuitOrder;
        public HandSortingMethod(int man, int pin, int sou, int sangen, int kaze)
        {
            SuitOrder = new Dictionary<TileID.Suits, int>();
            SuitOrder.Add(TileID.Suits.Man, man);
            SuitOrder.Add(TileID.Suits.Pin, pin);
            SuitOrder.Add(TileID.Suits.Sou, sou);
            SuitOrder.Add(TileID.Suits.Sangen, sangen);
            SuitOrder.Add(TileID.Suits.Kaze, kaze);
        }

        public static HandSortingMethod Default = new HandSortingMethod(1, 2, 3, 4, 5);

        //Returns whether t1 < t2
        public bool LessThan(TileID t1, TileID t2)
        {
            if (SuitOrder[t1.Suit] < SuitOrder[t2.Suit]) return true;
            else if (SuitOrder[t1.Suit] > SuitOrder[t2.Suit]) return false;
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
        public bool GreaterThan(TileID t1, TileID t2)
        {
            return LessThan(t2, t1);
        }
        public bool LessOrEqual(TileID t1, TileID t2)
        {
            return (LessThan(t1, t2) || (t1 == t2));
        }
        public bool GreaterOrEqual(TileID t1, TileID t2)
        {
            return (LessThan(t2, t1) || (t1 == t2));
        }
    }
}
