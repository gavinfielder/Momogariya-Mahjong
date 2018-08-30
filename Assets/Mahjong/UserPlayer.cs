using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    public class UserPlayer : Player, IPlayer
    {
        private TurnArgs turnArgs;

        //Takes a turn. 
        public override void StartTurn(TurnArgs args)
        {
            SetState("Starting Turn");
            turnArgs = args;
        }

        //Offers a discard to be called as naki
        public override void Offer(TileID tile)
        { 

        }
        

        //Gives a hand to the player to set the references and access key
        new public void Setup(int playerNumber, ref Hand h, ref Kawa k, GameController control, ref GameBoard gameBoard)
        {
            //accessKey = Security.AccessKeyHash(Security.GetRandomAccessKey());
            hand = h;
            kawa = k;
            board = gameBoard;
            controller = control;
            InitializeStateMachine();
            PlayerNumber = playerNumber;
            hand.SetOwner(PlayerNumber, accessKey);
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
            stateProcesses.Add("Deciding Naki", Process_DecidingNaki);
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
                Debug.Log("Player " + PlayerNumber + " new state: " + state);
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
            Debug.Log("Process_StartingTurn()");
            switch (turnArgs)
            {
                case TurnArgs.Default:
                    //Draw a tile from the wall
                    hand.AddDraw(board.Wall.Draw());
                    SetState("Deciding Discard");
                    break;
                case TurnArgs.Chii:
                    SetState("Null");
                    break;
                case TurnArgs.Pon:
                    SetState("Null");
                    break;
                case TurnArgs.Daiminkan:
                    SetState("Null");
                    break;
                case TurnArgs.KanContinue:
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
            if (hand.Contains(Tile.LastClickedTile, accessKey))
            {
                EventManager.Unsubscribe("Tile Clicked", DecidingDiscard_OnTileClick);
                TileID tileID = Tile.LastClickedTile.GetComponent<Tile>().Query(accessKey);
                hand.RemoveDiscard(tileID, kawa);
                SetState("Awaiting Turn");
            }
        }
        protected void Process_DecidingNaki()
        {
            SetState("Null");
        }



        //***********************************************************************
        //**************************** Event Handlers ***************************
        //***********************************************************************



    }
}
