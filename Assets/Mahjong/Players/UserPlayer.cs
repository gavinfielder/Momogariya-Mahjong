using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    public class UserPlayer : Player, IPlayer
    {
        private TurnArgs turnArgs;
        private HandAnalyzer handAnalyzer;
        private List<Naki> sendingToUI;

        //Takes a turn. 
        public override void StartTurn(TurnArgs args)
        {
            SetState("Starting Turn");
            turnArgs = args;
        }

        //Offers a discard to be called as naki
        public override void Offer(Tile tile)
        {
            TileID id = tile.Query();
            List<PotentialMeld> melds = handAnalyzer.GetWaitingMelds(id);

            /*
            string message = "Hand Analyzer " + PlayerNumber + " returned these melds matching the offer of " +
                id.ToString() + ":\r\n";
            for (int i = 0; i < melds.Count; i++)
                message += melds[i].ToString() + "\r\n";
            Debug.Log(message);
            */

            //Formulate a list of potential naki to send to the Naki UI Manager
            List<Naki> naki = new List<Naki>();
            bool add;
            for (int i = 0; i < melds.Count; i++)
            {
                add = true;
                if (Naki.GetNakiType(melds[i].Waits[0].type) == NakiType.Chii)
                {
                    //If I am not kamicha (player to right), I cannot request chii
                    if ((PlayerNumber - tile.StolenFrom + 4) % 4 != 1)
                    {
                        add = false;
                    }
                }
                if (add)
                {
                    naki.Add(new Naki()
                    {
                        meld = melds[i],
                        requestor = this,
                        type = Naki.GetNakiType(melds[i].Waits[0].type)
                    });
                }
            }
            //If I cannot request any naki for this tile, respond that I don't want the discard
            if (naki.Count == 0)
            {
                controller.RespondToOffer(new Naki()
                {
                    type = NakiType.Nashi,
                    requestor = this,
                    meld = null
                });
            }
            //Otherwise pass the list of possible naki to the Naki UI Manager for user input
            else
            {
                sendingToUI = naki;
                SetState("Awaiting User Naki Choice");
            }
        }
        

        //Gives a hand to the player to set the references and access key
        public override void Setup(int playerNumber, Hand h, Kawa k, GameController control, GameBoard gameBoard)
        {
            //Debug.Log("Setting up User Player " + playerNumber);
            //accessKey = Security.AccessKeyHash(Security.GetRandomAccessKey());
            hand = h;
            kawa = k;
            board = gameBoard;
            controller = control;
            PlayerNumber = playerNumber;
            hand.SetOwner(accessKey);
            hand.PlayerNumber = playerNumber;

            InitializeStateMachine();
            handAnalyzer = new HandAnalyzer(hand, accessKey);
        }
        
        

        //***********************************************************************
        //*********************** State Machine Functions ***********************
        //***********************************************************************

        //State machine
        public string State { get; protected set; }
        protected delegate void StateProcess();
        protected StateProcess ProcessState;
        protected Dictionary<string, StateProcess> stateProcesses;
        protected bool initialized;

        //Initializes the state dictionary
        protected void InitializeStateMachine()
        {
            stateProcesses = new Dictionary<string, StateProcess>();
            stateProcesses.Add("Null", Process_Null);
            stateProcesses.Add("Awaiting User Input", Process_Null);
            stateProcesses.Add("Awaiting Turn", Process_Null);

            stateProcesses.Add("Starting Turn", Process_StartingTurn);
            stateProcesses.Add("Deciding Discard", Process_DecidingDiscard);
            stateProcesses.Add("Awaiting User Naki Choice", Process_AwaitingUserNakiChoice);
        }

        //Initialization
        protected void Start()
        {
            Initialize();
        }

        protected void Initialize()
        {
            if (initialized) return;
            InitializeStateMachine();
            SetState("Null");
        }

        //Called once per update cycle, when enabled.
        protected void Update()
        {
            ProcessState();
        }

        //Sets the internal state
        protected void SetState(string state)
        {
            StateProcess prev = ProcessState;
            string prevStr = State;
            bool found = stateProcesses.TryGetValue(state, out ProcessState);
            if (found)
            {
                State = state;
                //Debug.Log("Player " + PlayerNumber + " new state: " + state);
            }
            else
            {
                Debug.LogError("Invalid player state requested: '" + state + "'. Returning to previous state: '" + prevStr + "'.");
                ProcessState = prev;
            }
        }



        //***********************************************************************
        //*************************** State Processors **************************
        //***********************************************************************

        private void Process_Null() { }

        protected void Process_StartingTurn()
        {
            switch (turnArgs.type)
            {
                case TurnArgsType.Default:
                    //Draw a tile from the wall
                    hand.AddDraw(board.Wall.Draw());
                    SetState("Deciding Discard");
                    break;
                case TurnArgsType.Chii:
                    //Grab the most recent discard
                    hand.AddDraw(Kawa.MostRecentKawa.Steal());
                    hand.FormOpenMeld(turnArgs.naki.meld, accessKey);
                    SetState("Deciding Discard");
                    break;
                case TurnArgsType.Pon:
                    //Grab the most recent discard
                    hand.AddDraw(Kawa.MostRecentKawa.Steal());
                    hand.FormOpenMeld(turnArgs.naki.meld, accessKey);
                    SetState("Deciding Discard");
                    break;
                case TurnArgsType.Daiminkan:
                    SetState("Null");
                    break;
                case TurnArgsType.KanContinue:
                    SetState("Null");
                    break;
            }
        }
        protected void Process_DecidingDiscard()
        {
            //Wait for a tile to be clicked
            EventManager.Subscribe("Tile Clicked", DecidingDiscard_OnTileClick);
            SetState("Awaiting User Input");
        }
        protected void DecidingDiscard_OnTileClick()
        {
            if (hand.Tiles.Contains(Tile.LastClickedTile, accessKey))
            {
                EventManager.Unsubscribe("Tile Clicked", DecidingDiscard_OnTileClick);
                hand.RemoveDiscard(Tile.LastClickedTile, kawa, accessKey);
                SetState("Awaiting Turn");
            }
        }
        protected void Process_AwaitingUserNakiChoice()
        {
            bool requestAccepted = controller.NakiManager.RequestService(sendingToUI,  controller.RespondToOffer, PlayerNumber, accessKey);
            if (requestAccepted)
            {
                SetState("Awaiting Turn");
            }
        }



        //***********************************************************************
        //**************************** Event Handlers ***************************
        //***********************************************************************



    }
}
