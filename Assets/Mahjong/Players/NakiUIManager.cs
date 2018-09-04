using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mahjong
{
    public class NakiUIManager : MonoBehaviour
    {
        //Object references
        #pragma warning disable CS0649
        public Canvas CallCanvas;
        public Text Header;
        public Button PonButton;
        public Button ChiiButton;
        public Button KanButton;
        public Button RonButton;
        public Button TsumoButton;
        public Button ContinueButton;
        #pragma warning restore CS0649
        
        //For callbacks
        public delegate void NakiHandler(Naki naki);
        private bool busy = false;
        private NakiHandler callback;
        //private Player player;
        private int playerNum;
        private List<Naki> calls;
        private int accessKey; //Needed if the user needs to select a tile to clarify
        

        //Requests naki handling. Returns false if busy, true if request accepted
        public bool RequestService(List<Naki> naki, NakiHandler cb, int playerNumber, int handAccessKey)
        {
            if (busy) return false;
            if (naki == null || naki.Count == 0)
            {
                //We can handle an empty service request immediately
                cb(new Naki() { type = NakiType.Nashi });
                return true;
            }
            //Set up the call canvas to handle the request
            callback = cb;
            calls = naki;
            //player = naki[0].requestor;
            playerNum = playerNumber;
            handAccessKey = accessKey;
            CallCanvas.gameObject.SetActive(true);
            SetOptions();
            busy = true;
            return true; //says that the request has been accepted and will be handled
        }

        //Sets the options
        private void SetOptions()
        {
            Header.text = "Player " + playerNum;
            if (ContainsType(NakiType.Chii)) ChiiButton.gameObject.SetActive(true);
            if (ContainsType(NakiType.Pon)) PonButton.gameObject.SetActive(true);
            if (ContainsType(NakiType.Ron)) RonButton.gameObject.SetActive(true);
            if (ContainsType(NakiType.Kan)) KanButton.gameObject.SetActive(true);
            if (ContainsType(NakiType.Tsumo)) ChiiButton.gameObject.SetActive(true);
            ContinueButton.gameObject.SetActive(true);
        }

        //Occurs when the user clicks the pon button
        public void OnPonClick()
        {
            Naki naki = GetNaki(NakiType.Pon);
            callback(naki);
            FinalizeService();
        }

        //Occurs when the user clicks the chii button
        public void OnChiiClick()
        {
            //TODO: check if ambiguous
            Naki naki = GetNaki(NakiType.Chii);
            callback(naki);
            FinalizeService();

        }

        //Occurs when the user clicks the kan button
        public void OnKanClick()
        {
            Naki naki = GetNaki(NakiType.Kan);
            callback(naki);
            FinalizeService();

        }

        //Occurs when the user clicks the ron button
        public void OnRonClick()
        {
            Naki naki = GetNaki(NakiType.Ron);
            callback(naki);
            FinalizeService();

        }

        //Occurs when the user clicks the tsumo button
        public void OnTsumoClick()
        {
            Naki naki = GetNaki(NakiType.Tsumo);
            callback(naki);
            FinalizeService();

        }

        //Occurs when the user clicks the continue button
        public void OnContinueClick()
        {
            Naki naki = new Naki()
            {
                type = NakiType.Nashi,
                //requestor = player,
                meld = null
            };
            callback(naki);
            FinalizeService();
        }

        //Checks the list of calls to see if more than one potential chii exists
        private bool IsAmbiguousChii()
        {
            return false;
        }

        //Gets the first naki of the type in the meld list 
        //...[that involves the given TileID, if that arg is passed]
        private Naki GetNaki(NakiType type, TileID involving = null)
        {
            int i = 0;
            while (i < calls.Count && calls[i].type != type)
                i++;
            if (i == calls.Count) return new Naki() { type = NakiType.Nashi/*, requestor = player*/ };
            else return calls[i];
        }

        //Checks the naki list to see if it contains a type
        private bool ContainsType(NakiType type)
        {
            int i = 0;
            while (i < calls.Count && calls[i].type != type)
                i++;
            if (i == calls.Count) return false;
            else return true;
        }


        //Shuts down the service
        private void FinalizeService()
        {
            PonButton.gameObject.SetActive(false);
            ChiiButton.gameObject.SetActive(false);
            KanButton.gameObject.SetActive(false);
            RonButton.gameObject.SetActive(false);
            TsumoButton.gameObject.SetActive(false);
            ContinueButton.gameObject.SetActive(false);
            CallCanvas.gameObject.SetActive(false);
            callback = null;
            //player = null;
            calls = null;
            busy = false;
        }


    }
}
