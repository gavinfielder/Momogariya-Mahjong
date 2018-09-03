using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    //Provides useful analysis tools for hands
    public class HandAnalyzer
    {
        //***********************************************************************
        //*************************** Hand Interfacing **************************
        //***********************************************************************

        private Hand hand;
        private int accessKey = 0;
        private int PlayerNumber;
        private List<PotentialMeld> Melds = new List<PotentialMeld>();
        private TileCollection LooseTiles = new TileCollection();

        //Constructor
        public HandAnalyzer(Hand h, int handAccessKey)
        {
            hand = h;
            accessKey = handAccessKey;
            PlayerNumber = hand.PlayerNumber;
            LooseTiles.SetOwner(accessKey);

            EventManager.Subscribe("Hand " + PlayerNumber + " Draw", OnTileDraw);
            EventManager.Subscribe("Hand " + PlayerNumber + " Discard", OnDiscard);
            EventManager.Subscribe("Hand " + PlayerNumber + " Open Meld", OnOpenMeld);
            EventManager.Subscribe("Hands Dealt", OnDeal);

            //Debug.Log("Hand Analyzer created for Player " + PlayerNumber);
        }

        //Occurs when a tile is drawn
        private void OnTileDraw()
        {
            CheckDraw();
            //EventManager.FlagEvent("Debug: New HandAnalyzer Analysis Available");
            //Debug.Log("Hand " + PlayerNumber + "'s HandAnalyzer processed a draw. Result:\r\n" + Debug_Print());
        }

        //Occurs when a tile is discarded
        private void OnDiscard()
        {
            CheckDiscard();
            //EventManager.FlagEvent("Debug: New HandAnalyzer Analysis Available");
            Debug.Log("Hand " + PlayerNumber + "'s HandAnalyzer processed a discard. Result:\r\n" + Debug_Print());
        }

        //Occurs when an open meld is formed
        private void OnOpenMeld()
        {
            CheckOpenMeld();
        }

        //Occurs when the hands are initially dealt
        private void OnDeal()
        {
            RebuildMeldList();
            //EventManager.FlagEvent("Debug: New HandAnalyzer Analysis Available");
            Debug.Log("Hand " + PlayerNumber + "'s HandAnalyzer processed a deal. Result:\r\n" + Debug_Print());
        }

        //***********************************************************************
        //*************************** Public Methods ****************************
        //***********************************************************************

        //Rebuilds the potential meld list from zero
        public void RebuildMeldList()
        {
            Melds.Clear();
            //All closed tiles are loose until proven otherwise
            LooseTiles = new TileCollection(hand.Tiles.Closed.GetTileList(), accessKey);
            //Add all the open melds
            int i = 0;
            for (i = 0; i < hand.OpenMelds.Count; i++)
                AddMeld(hand.OpenMelds[i].ConvertToPotentialMeld());
            //Handle all closed tiles
            List<TileID> closed = hand.Tiles.Closed.GetTileIDList(accessKey);
            CheckForwardResponse info;
            i = 0;
            while (i < closed.Count)
            {
                info = CheckForward(i);
                if (info.count > 0)
                    LooseTiles.Remove(hand.Tiles.Closed[i], accessKey);
                i++;
            }
        }

        //Return type for CheckForward function
        private struct CheckForwardResponse
        {
            public int count; //number of melds added
        }

        //Checks up to the next 3 tiles in the list and adds any potential melds to the collection
        //Returns the number of potential melds added.
        private CheckForwardResponse CheckForward(int index)
        {
            CheckForwardResponse info = new CheckForwardResponse
            {
                count = 0,
            };
            List<TileID> list = hand.Tiles.Closed.GetTileIDList(accessKey);
            PotentialMeld meld;
            TileID t1 = TileID.Invalid, t2 = TileID.Invalid, t3 = TileID.Invalid, t4 = TileID.Invalid;
            t1 = list[index];
            //First rule out any obvious disqualifiers
            if (index + 1 < list.Count) t2 = list[index + 1];
            else return info; //index is last tile in list
            if (t2.Suit != t1.Suit) return info; //next suit different
            if (t1.Jihai && t1.Number != t2.Number) return info; //different honor
            //Set the next tileIDs
            if (index + 2 < list.Count) t3 = list[index + 2];
            if (index + 3 < list.Count) t4 = list[index + 3];

            //Check for koutsu/kantsu
            if (t2 == t1)
            {
                if (t3 == t1)
                {
                    if (t4 == t1)
                    {
                        //4 of the same tile
                        //Completed kantsu
                        meld = new PotentialMeld()
                        {
                            Type = Meld.MeldType.Kantsu,
                            Completed = true
                        };
                        meld.Add(t1); meld.Add(t1); meld.Add(t1); meld.Add(t1);
                        AddMeld(meld);
                        info.count++;
                        //Completed koutsu
                        meld = new PotentialMeld()
                        {
                            Type = Meld.MeldType.Koutsu,
                            Completed = true
                        };
                        meld.Add(t1); meld.Add(t1); meld.Add(t1);
                        AddMeld(meld);
                        info.count++;
                        //Completed jantou
                        meld = new PotentialMeld()
                        {
                            Type = Meld.MeldType.Jantou,
                            Completed = true,
                        };
                        meld.Add(t1); meld.Add(t1); 
                        AddMeld(meld);
                        info.count++;
                        LooseTiles.Remove(hand.Tiles.Closed[index], accessKey);
                        LooseTiles.Remove(hand.Tiles.Closed[index + 1], accessKey);
                        LooseTiles.Remove(hand.Tiles.Closed[index + 2], accessKey);
                        LooseTiles.Remove(hand.Tiles.Closed[index + 3], accessKey);
                    }
                    else
                    {
                        //3 of the same tile. 
                        //Potential kantsu
                        meld = new PotentialMeld()
                        {
                            Type = Meld.MeldType.Kantsu,
                        };
                        meld.Add(t1); meld.Add(t1); meld.Add(t1);
                        meld.Waits.Add(new Wait(WaitType.Kantsu, t1));
                        AddMeld(meld);
                        info.count++;
                        //Completed koutsu
                        meld = new PotentialMeld()
                        {
                            Type = Meld.MeldType.Koutsu,
                            Completed = true
                        };
                        meld.Add(t1); meld.Add(t1); meld.Add(t1);
                        AddMeld(meld);
                        info.count++;
                        //Add potential koutsu
                        info.count += AddPotentialVersions(meld);
                        //Completed jantou
                        meld = new PotentialMeld()
                        {
                            Type = Meld.MeldType.Jantou,
                            Completed = true,
                        };
                        meld.Add(t1); meld.Add(t1);
                        AddMeld(meld);
                        info.count++;
                        LooseTiles.Remove(hand.Tiles.Closed[index], accessKey);
                        LooseTiles.Remove(hand.Tiles.Closed[index + 1], accessKey);
                        LooseTiles.Remove(hand.Tiles.Closed[index + 2], accessKey);
                    }
                }
                else
                {
                    //2 of the same tile
                    //Potential koutsu
                    meld = new PotentialMeld()
                    {
                        Type = Meld.MeldType.Koutsu,
                    };
                    meld.Add(t1); meld.Add(t1);
                    meld.Waits.Add(new Wait(WaitType.Koutsu, t1));
                    AddMeld(meld);
                    info.count++;

                    //Completed jantou
                    meld = new PotentialMeld()
                    {
                        Type = Meld.MeldType.Jantou,
                        Completed = true,
                    };
                    meld.Add(t1); meld.Add(t1);
                    AddMeld(meld);
                    info.count++;
                    LooseTiles.Remove(hand.Tiles.Closed[index], accessKey);
                    LooseTiles.Remove(hand.Tiles.Closed[index + 1], accessKey);
                }
            }
            //Check for shuntsu
            else
            {
                if (t2.Number - t1.Number == 1)
                {
                    if (t3.Suit == t1.Suit && t3.Number - t2.Number == 1)
                    {
                        //Completed shuntsu
                        meld = new PotentialMeld()
                        {
                            Type = Meld.MeldType.Shuntsu,
                            Completed = true
                        };
                        meld.Add(t1); meld.Add(t2); meld.Add(t3);
                        AddMeld(meld);
                        info.count++;
                        //Add all the potential shuntsu
                        info.count += AddPotentialVersions(meld);


                        LooseTiles.Remove(hand.Tiles.Closed[index], accessKey);
                        LooseTiles.Remove(hand.Tiles.Closed[index + 1], accessKey);
                        LooseTiles.Remove(hand.Tiles.Closed[index + 2], accessKey);
                    }
                    else
                    {
                        //Potential shuntsu with an outer wait
                        meld = new PotentialMeld()
                        {
                            Type = Meld.MeldType.Shuntsu,
                        };
                        meld.Add(t1); meld.Add(t2);
                        if (t1.Number == 1)
                            meld.Waits.Add(new Wait(WaitType.Penchan, new TileID(t1.Suit, 3)));
                        else if (t2.Number == 9)
                            meld.Waits.Add(new Wait(WaitType.Penchan, new TileID(t1.Suit, 7)));
                        else
                        {
                            meld.Waits.Add(new Wait(WaitType.Ryanmen, new TileID(t1.Suit, (byte)(t1.Number - 1))));
                            meld.Waits.Add(new Wait(WaitType.Ryanmen, new TileID(t1.Suit, (byte)(t2.Number + 1))));
                        }
                        AddMeld(meld);
                        info.count++;
                        LooseTiles.Remove(hand.Tiles.Closed[index], accessKey);
                        LooseTiles.Remove(hand.Tiles.Closed[index + 1], accessKey);
                    }
                }
                else if (t2.Number - t1.Number == 2)
                {
                    //Potential shuntsu with a kanchan (inner) wait
                    meld = new PotentialMeld()
                    {
                        Type = Meld.MeldType.Shuntsu,
                    };
                    meld.Add(t1); meld.Add(t2);
                    meld.Waits.Add(new Wait(WaitType.Kanchan, new TileID(t1.Suit, (byte)(t1.Number + 1))));
                    AddMeld(meld);
                    info.count++;
                    LooseTiles.Remove(hand.Tiles.Closed[index], accessKey);
                    LooseTiles.Remove(hand.Tiles.Closed[index + 1], accessKey);
                }
            }

            return info;
        }

        //Adds all the waiting versions of a completed meld. Returns the number added.
        private int AddPotentialVersions(PotentialMeld completed)
        {
            int count = 0;
            PotentialMeld meld;
            if (completed.Type == Meld.MeldType.Koutsu)
            {
                //Potential koutsu
                meld = new PotentialMeld()
                {
                    Type = Meld.MeldType.Koutsu,
                };
                meld.Add(completed.IDs[0]); meld.Add(completed.IDs[0]);
                meld.Waits.Add(new Wait(WaitType.Kantsu, completed.IDs[0]));
                AddMeld(meld);
                count++;
            }
            else if (completed.Type == Meld.MeldType.Shuntsu)
            {
                //Potential shuntsu lower
                meld = new PotentialMeld()
                {
                    Type = Meld.MeldType.Shuntsu,
                };
                TileID t1 = completed.IDs[0];
                TileID t2 = completed.IDs[1];
                TileID t3 = completed.IDs[2];
                meld.Add(t1); meld.Add(t2);
                if (t1.Number == 1)
                    meld.Waits.Add(new Wait(WaitType.Penchan, new TileID(t1.Suit, 3)));
                else
                {
                    meld.Waits.Add(new Wait(WaitType.Ryanmen, new TileID(t1.Suit, (byte)(t1.Number - 1))));
                    meld.Waits.Add(new Wait(WaitType.Ryanmen, t3));
                }
                AddMeld(meld);
                count++;

                //Potential shuntsu upper
                meld = new PotentialMeld()
                {
                    Type = Meld.MeldType.Shuntsu,
                };
                meld.Add(t2); meld.Add(t3);
                if (t3.Number == 9)
                    meld.Waits.Add(new Wait(WaitType.Penchan, new TileID(t1.Suit, 7)));
                else
                {
                    meld.Waits.Add(new Wait(WaitType.Ryanmen, t1));
                    meld.Waits.Add(new Wait(WaitType.Ryanmen, new TileID(t1.Suit, (byte)(t3.Number + 1))));
                }
                AddMeld(meld);
                count++;

                //Potential shuntsu center (kanchan)
                meld = new PotentialMeld()
                {
                    Type = Meld.MeldType.Shuntsu,
                };
                meld.Add(t1); meld.Add(t3);
                meld.Waits.Add(new Wait(WaitType.Kanchan, t2));
                AddMeld(meld);
                count++;

            }
            return count;
        }
        
        //Adds the potential meld to the list, as long as it or a completed version of it isn't already present
        private void AddMeld(PotentialMeld meld)
        {
            if (Melds.Contains(meld)) return;
            /*
            //Check for completed versions of this meld
            List<PotentialMeld> completed = meld.GetCompleted();
            for (int i = 0; i < completed.Count; i++)
                if (Melds.Contains(completed[i])) return;
            */
            Melds.Add(meld);
        }

        //Returns all potential melds that are waiting on the given tile ID
        public List<PotentialMeld> GetWaitingMelds(TileID tile)
        {
            List<PotentialMeld> list = new List<PotentialMeld>();
            for (int i = 0; i < Melds.Count; i++)
            {
                if (!(Melds[i].Completed) && (tile.Suit == Melds[i].Suit))
                {
                    for (int j = 0; j < Melds[i].Waits.Count; j++)
                    {
                        if (Melds[i].Waits[j].tile == tile)
                        {
                            list.Add(Melds[i]);
                            break;
                        }
                    }
                }
            }
            return list;
        }

        //Updates the meld list for the drawn tile
        private void CheckDraw()
        {
            Tile draw = hand.Draw;
            TileID drawID = draw.Query(accessKey);
            //All tiles are loose until proven otherwise, so put it in the loose tile list
            LooseTiles.Add(draw, accessKey);
            /* The following may not be necessary if the incompleted versions of the completed 
             * meld stays in the list anyway, as we're just going to  be checking around the
             * first instance of the draw anyway. 
             * TODO: if it seems to be working, remove this dead code
            //First, check all the waiting melds and update any who were waiting on this draw
            List<PotentialMeld> waitingMelds = GetWaitingMelds(drawID);
            int i = 0;
            for (i = 0; i < waitingMelds.Count; i++)
            {
                waitingMelds[i].Add(drawID);
                waitingMelds[i].Waits.Clear();
                waitingMelds[i].Completed = true;
                //If a koutsu was just completed, then let's also register the potential kantsu
                if (waitingMelds[i].Type == Meld.MeldType.Koutsu)
                {
                    PotentialMeld meld = new PotentialMeld(waitingMelds[i].IDs);
                    meld.Waits.Add(new Wait(WaitType.Kantsu, drawID));
                    AddMeld(meld);
                }
            }
            */
            //Next, check the closed tiles around the first identical tile as the draw
            int index = hand.Tiles.Closed.IndexOf(drawID, accessKey);
            CheckAround(index);
        }

        //Checks the tiles before and after the given index in the closed tile set for potential melds
        private void CheckAround(int index)
        {
            int i = index - 2;
            while (i <= index && i < hand.Tiles.Closed.Count)
            {
                if (i >= 0) CheckForward(i);
                i++;
            }
        }

        //Updates the meld list after a discard
        private void CheckDiscard()
        {
            Tile discard = hand.Discard;
            TileID discardID = discard.Query(accessKey);
            //First, remove it from loose tiles if it's in there
            LooseTiles.Remove(discard, accessKey);
            //Find all the melds that used it and remove them.
            RemoveUsedInMelds(discardID);
            //Find the index among the closed tiles where the first identical tile to the discard would go
            int i = FindWouldBeLocationInClosed(discardID);
            //Check around this spot to add back any potential melds that are still valid
            CheckAround(i);

        }

        //Updates the meld list after an open meld
        private void CheckOpenMeld()
        {
            OpenMeld latest = hand.OpenMelds[hand.OpenMelds.Count - 1];
            //Find all the melds that used any of its tiles and remove them.
            int i = 0;
            for (i = 0; i < latest.Count; i++)
            {
                RemoveUsedInMelds(latest.IDs[i]);
            }
            //Find the index where this meld went
            i = FindWouldBeLocationInClosed(latest.IDs[0]);
            //Check around this spot to add back any potential melds that are still valid
            CheckAround(i);
        }

        //Returns the index in the closed tile set where the given tileID would go
        private int FindWouldBeLocationInClosed(TileID id)
        {
            int i = 0;
            while (i < hand.Tiles.Closed.Count && hand.Tiles.Closed[i].Query(accessKey) < id)
                i++;
            return i;
        }

        //Finds all the melds a tile ID is used in and removes them. Also checks for any newly loose tiles
        private void RemoveUsedInMelds(TileID id)
        {
            List<PotentialMeld> usedIn = GetUsedInMelds(id);
            int i = 0;
            for (i = 0; i < usedIn.Count; i++)
            {
                Melds.Remove(usedIn[i]);
                //Also see if there are new loose tiles leftover in these melds
                for (int j = 0; j < usedIn[i].Count; j++)
                    CheckIfNewLoose(usedIn[i][j]);
            }
        }

        //Gets all the potential closed melds that currently include a particular tile ID
        public List<PotentialMeld> GetUsedInMelds(TileID tile)
        {
            List<PotentialMeld> list = new List<PotentialMeld>();
            int i = 0;
            while (i < Melds.Count)
            {
                if ((Melds[i].Fungibility > 0) && (Melds[i].IDs.Contains(tile)))
                {
                    list.Add(Melds[i]);
                }
                i++;
            }
            return list;
        }

        //Checks the given tile ID to see if the hand may contain a loose tile of it after a discard
        //If loose, adds the tile to the loose tile list.
        private void CheckIfNewLoose(TileID tile)
        {
            int index = hand.Tiles.Closed.IndexOf(tile, accessKey);
            if (index == -1) return;
            if (hand.Tiles.Closed.CountOf(tile, accessKey) >= 2)
                return;
            if (IsLoose(hand.Tiles.Closed[index]))
                LooseTiles.Add(hand.Tiles.Closed[index], accessKey);
        }

        //Returns true if the tile qualifies as loose. Does not add it to the loose tiles collection.
        private bool IsLoose(Tile tile)
        {
            int index = hand.Tiles.Closed.IndexOf(tile, accessKey);
            if (index == -1) return false;
            if (index > 0 && TileID.InSameSuji(hand.Tiles.Closed[index - 1].Query(accessKey), tile.Query(accessKey)))
                return false;
            if (index + 1 < hand.Tiles.Closed.Count && TileID.InSameSuji(hand.Tiles.Closed[index + 1].Query(accessKey), tile.Query(accessKey)))
                return false;
            return true;
        }


        //***********************************************************************
        //**************************** Debug Methods ****************************
        //***********************************************************************

        public string Debug_Print()
        {
            string output = "Hand Analyzer has detected " + Melds.Count + " potential or completed melds:\r\n";
            for (int i = 0; i < Melds.Count; i++)
                output += Melds[i].ToString() + "\r\n";
            output += "The hand also has " + LooseTiles.Count + " loose tiles: { ";
            List<TileID> loose = LooseTiles.GetTileIDList(accessKey);
            for (int i = 0; i < loose.Count; i++)
                output += loose[i].ToShortString() + ", ";
            output += "}\r\n";
            return output;
        }
    }
}
