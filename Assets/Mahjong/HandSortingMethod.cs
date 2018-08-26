using System;
using System.Collections.Generic;

namespace Mahjong
{
    public struct HandSortingMethod
    {
        public Dictionary<Tile.Suits, int> SuitOrder;
        public HandSortingMethod(int man, int pin, int sou, int sangen, int kaze)
        {
            SuitOrder = new Dictionary<Tile.Suits, int>();
            SuitOrder.Add(Tile.Suits.Man, man);
            SuitOrder.Add(Tile.Suits.Pin, pin);
            SuitOrder.Add(Tile.Suits.Sou, sou);
            SuitOrder.Add(Tile.Suits.Sangen, sangen);
            SuitOrder.Add(Tile.Suits.Kaze, kaze);
        }

        public static HandSortingMethod Default = new HandSortingMethod(1, 2, 3, 4, 5);

        //Returns whether t1 < t2
        public bool LessThan(Tile t1, Tile t2)
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
        public bool GreaterThan(Tile t1, Tile t2)
        {
            return LessThan(t2, t1);
        }
        public bool LessOrEqual(Tile t1, Tile t2)
        {
            return (LessThan(t1, t2) || (t1 == t2));
        }
        public bool GreaterOrEqual(Tile t1, Tile t2)
        {
            return (LessThan(t2, t1) || (t1 == t2));
        }
    }
}
