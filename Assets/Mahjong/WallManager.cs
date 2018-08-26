using System;
using System.Collections.Generic;
using UnityEngine.Events;
//using UnityEngine;

namespace Mahjong
{
    public class WallManager
    {
        private List<Tile> AllTiles = new List<Tile>();

        private int kanReplacementDraws = 0;
        private int numberOfRegularDoras = 0;

        public int NumberOfDeadTiles {
            get
            {
                return 14 - kanReplacementDraws; 
            }
        }
        public int NumberOfHiddenDeadTiles {
            get
            {
                return 14 - kanReplacementDraws - numberOfRegularDoras;
            }
        }
        public int NumberDrawsRemaining
        {
            get
            {
                return AllTiles.Count - 14;
            }
        }

        //Provides public access to the number of wall which was broken.
        //Needed for GameBoardRenderer to properly render the wall. 
        public int BrokenAt { get; private set; }

        private List<Tile> doras = new List<Tile>();
        public List<Tile> Doras
        {
            get
            {
                return doras;
            }
        }

        //Builds the wall for a new game
        public void Build(RuleSet rules)
        {
            doras.Clear();
            AddAllTiles(rules);
            ShuffleRange(0, AllTiles.Count - 1);
            EventManager.FlagEvent("Wall Built");
        }

        //Builds a wall while forcing the first N draws and dora/kandora. For testing only.
        public void Build_ForceOrder(RuleSet rules, List<Tile> firstDraws, List<Tile> firstDoras)
        {
            AddAllTiles(rules);
            for (int i = 0; i < firstDraws.Count; i++)
            {
                ForceTileAt(firstDraws[i], i, i + 1, AllTiles.Count - 1);
            }
            for (int i = 0; i < firstDoras.Count; i++)
            {
                ForceTileAt(firstDoras[i], AllTiles.Count - 1 - i, firstDraws.Count, AllTiles.Count - 1 - i - 1);
            }
            ShuffleRange(firstDraws.Count, AllTiles.Count - 1 - firstDoras.Count);
            EventManager.FlagEvent("Wall Built");
        }

        //Breaks the wall and reveals the first dora
        public void Break(int wallNumber)
        {
            BrokenAt = wallNumber;
            //Since this class is just all data, we don't actually need logic for 'breaking' the wall
            EventManager.FlagEvent("Wall Broken");
            //Reveal the first dora
            NewDora();
        }

        //Draws a single tile from the wall
        public Tile Draw()
        {
            if (NumberDrawsRemaining == 0) return Tile.Invalid;
            Tile tile = AllTiles[0];
            AllTiles.RemoveAt(0);
            EventManager.FlagEvent("Normal Draw");
            return tile;
        }

        //Draws a kan replacement tile from the dead wall
        public Tile DrawKanReplacement()
        {
            //kanReplacementDraws++;
            return Tile.Invalid;
        }

        //Flips a new dora. 
        public Tile NewDora()
        {
            AllTiles[AllTiles.Count - numberOfRegularDoras - 1].Revealed = true;
            doras.Add(AllTiles[AllTiles.Count - numberOfRegularDoras - 1].GetDoraFromIndicator());
            numberOfRegularDoras++;
            EventManager.FlagEvent("New Dora");
            return doras[doras.Count - 1];
        }

        //Returns all the revealed dora indicators. Used by GameBoardRenderer
        public List<Tile> GetDoraIndicators()
        {
            List<Tile> list = new List<Tile>();
            for (int i = 0; i < doras.Count; i++)
            {
                list.Add(AllTiles[AllTiles.Count - 1 - i].Copy());
            }
            return list;
        }

        //Returns the number of ura dora
        public int GetNumberOfUradora(RuleSet rules)
        {
            int r = 0;
            if (rules.Uradora == true) r = 1;
            if (rules.Kanuradora == true) r = doras.Count;
            return r;
        }

        //Adds all the tiles to the main collection
        private void AddAllTiles(RuleSet rules)
        {
            AllTiles.Clear();

            for (int i = 0; i < 4; i++)
            {
                AllTiles.Add(new Tile(Tile.Suits.Man, 1));
                AllTiles.Add(new Tile(Tile.Suits.Man, 2));
                AllTiles.Add(new Tile(Tile.Suits.Man, 3));
                AllTiles.Add(new Tile(Tile.Suits.Man, 4));
                AllTiles.Add(new Tile(Tile.Suits.Man, 6));
                AllTiles.Add(new Tile(Tile.Suits.Man, 7));
                AllTiles.Add(new Tile(Tile.Suits.Man, 8));
                AllTiles.Add(new Tile(Tile.Suits.Man, 9));

                AllTiles.Add(new Tile(Tile.Suits.Pin, 1));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 2));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 3));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 4));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 6));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 7));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 8));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 9));

                AllTiles.Add(new Tile(Tile.Suits.Sou, 1));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 2));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 3));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 4));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 6));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 7));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 8));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 9));

                AllTiles.Add(new Tile(Tile.Suits.Kaze, Tile.TON));
                AllTiles.Add(new Tile(Tile.Suits.Kaze, Tile.NAN));
                AllTiles.Add(new Tile(Tile.Suits.Kaze, Tile.SHAA));
                AllTiles.Add(new Tile(Tile.Suits.Kaze, Tile.PEI));

                AllTiles.Add(new Tile(Tile.Suits.Sangen, Tile.CHUN));
                AllTiles.Add(new Tile(Tile.Suits.Sangen, Tile.HAKU));
                AllTiles.Add(new Tile(Tile.Suits.Sangen, Tile.HATSU));
            }
            //Add in 5's depending on whether akadora is specified
            if (rules.Akadora == true)
            {
                AllTiles.Add(new Tile(Tile.Suits.Man, 5));
                AllTiles.Add(new Tile(Tile.Suits.Man, 5));
                AllTiles.Add(new Tile(Tile.Suits.Man, 5));
                AllTiles.Add(new Tile(Tile.Suits.Man, 5, true, true));

                AllTiles.Add(new Tile(Tile.Suits.Pin, 5));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 5));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 5));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 5, true, true));

                AllTiles.Add(new Tile(Tile.Suits.Sou, 5));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 5));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 5));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 5, true, true));
            }
            else
            {
                AllTiles.Add(new Tile(Tile.Suits.Man, 5));
                AllTiles.Add(new Tile(Tile.Suits.Man, 5));
                AllTiles.Add(new Tile(Tile.Suits.Man, 5));
                AllTiles.Add(new Tile(Tile.Suits.Man, 5));

                AllTiles.Add(new Tile(Tile.Suits.Pin, 5));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 5));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 5));
                AllTiles.Add(new Tile(Tile.Suits.Pin, 5));

                AllTiles.Add(new Tile(Tile.Suits.Sou, 5));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 5));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 5));
                AllTiles.Add(new Tile(Tile.Suits.Sou, 5));

            }
        }

        //Shuffles the tile at the specified index into a random location over a range
        private void ShuffleTile(int index, int low, int high)
        {
            Tile shuffling = AllTiles[index];
            //Use invalid placeholder to not mess up the range
            AllTiles[index] = Tile.Invalid;
            Random rnd = new Random();
            int to = rnd.Next(low, high);
            AllTiles.Insert(to, shuffling);
            AllTiles.Remove(Tile.Invalid);
        }

        //Shuffles all the tiles over the specified range
        private void ShuffleRange(int low, int high)
        {
            int shuffles = (high - low + 1) * 37;
            Random rnd = new Random();
            int i, j;
            for (int n = 0; n < shuffles; n++)
            {
                i = rnd.Next(low, high + 1);
                j = rnd.Next(low, high + 1);
                Swap2(i, j);
            }
        }

        //Performs an in-place swap of two tiles
        private void Swap2(int i, int j)
        {
            if (i == j) return;
            Tile temp = AllTiles[i];
            AllTiles[i] = AllTiles[j];
            AllTiles[j] = temp;
        }

        //Forces a tile at the specified index, shuffling into the given range until achieved
        private void ForceTileAt(Tile tile, int index, int low, int high)
        {
            while (AllTiles[index] != tile)
            {
                ShuffleTile(index, low, high);
            }
        }

        
        
    }
}
