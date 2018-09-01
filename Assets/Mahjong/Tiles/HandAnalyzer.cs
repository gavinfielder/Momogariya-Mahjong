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
        private int _accessKey = 0;
        private int PlayerNumber;



        //Constructor
        public HandAnalyzer(int accessKey, ref Hand h)
        {
            hand = h;
            _accessKey = accessKey;
            PlayerNumber = hand.PlayerNumber;

            EventManager.Subscribe("Hand " + PlayerNumber + " Draw", OnTileDraw);
            EventManager.Subscribe("Hand " + PlayerNumber + " Discard", OnDiscard);
            EventManager.Subscribe("Hand " + PlayerNumber + " Open Meld", OnOpenMeld);
            EventManager.Subscribe("Hands Dealt", OnDeal);
        }

        //Occurs when a tile is drawn
        private void OnTileDraw()
        {

        }

        //Occurs when a tile is discarded
        private void OnDiscard()
        {

        }

        //Occurs when an open meld is formed
        private void OnOpenMeld()
        {

        }

        //Occurs when the hands are initially dealt
        private void OnDeal()
        {

        }
        


    }
}
