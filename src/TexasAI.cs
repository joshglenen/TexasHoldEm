using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TexasHoldEm.PokerGame;

namespace TexasHoldEm

    //TODO: fix raise system, start choice backend, add manditory small bet system
{
    class Personality
    {
        public bool _ignores_bluffs;
        public bool _high_roller;
        public bool _cautious_better;
        public bool _passive_player;
        public bool _random_decisions;

        public int _maxBet;
        public int _minBet;
        public int _blindSmall;
        public int _blindBig;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="difficulty"></param>
        /// <param name="myGame"></param>
        /// <param name="personality">Granny, Timmy, Johnny, Jake</param>
        public Personality(PokerGame myGame, string personality = null)
        {
            switch(personality)
            {
                case null:
                    {
                        _ignores_bluffs = false;
                        _high_roller = false;
                        _cautious_better = false;
                        _passive_player = false ;
                        _random_decisions = true;
                        _maxBet = myGame._maxBet;
                        _minBet = myGame._minBet;
                        _blindSmall = myGame._smallBlind;
                        _blindBig = myGame._bigBlind;
                    }
                    break;
                case "Granny": //plays for fun but calls bluffs
                    {
                        _ignores_bluffs = false;
                        _high_roller = true;
                        _cautious_better = false;
                        _passive_player = false;
                        _random_decisions = true;
                        _maxBet = myGame._maxBet;
                        _minBet = myGame._minBet ;
                        _blindSmall = myGame._smallBlind;
                        _blindBig = myGame._bigBlind;
                    }
                    break;
                case "Timmy": //plays for big swing turns and cautiously with bad hands
                    {
                        _ignores_bluffs = false;
                        _high_roller = false;
                        _cautious_better = false;
                        _passive_player = false;
                        _random_decisions = true;
                        _maxBet = myGame._maxBet;
                        _minBet = myGame._minBet;
                        _blindSmall = myGame._smallBlind;
                        _blindBig = myGame._bigBlind;
                    }
                    break;
                case "Johnny": //plays cautiously always but calls bluffs
                    {
                        _ignores_bluffs = false;
                        _high_roller = false;
                        _cautious_better = false;
                        _passive_player = false;
                        _random_decisions = true;
                        _maxBet = myGame._maxBet;
                        _minBet = myGame._minBet;
                        _blindSmall = myGame._smallBlind;
                        _blindBig = myGame._bigBlind;
                    }
                    break;
                case "Jake": //plays to win but doesn't call bluffs
                    {
                        _ignores_bluffs = false;
                        _high_roller = false;
                        _cautious_better = false;
                        _passive_player = false;
                        _random_decisions = true;
                        _maxBet = myGame._maxBet;
                        _minBet = myGame._minBet;
                        _blindSmall = myGame._smallBlind;
                        _blindBig = myGame._bigBlind;
                    }
                    break;
            }
        }
    }


    class TexasAI
    {
        public int PlayerGoesFirst { get; private set; }
        protected bool IsNewGame;
        private string _ai_stage;
        public string AI_Stage { get { return _ai_stage; } set { _ai_stage = value; Console.WriteLine("!AI! Stage now set to: " + _ai_stage); } }

        protected int PlayersTurn;

        public TexasAI(int index = 0)
        {
            ResetAI();
            IsNewGame = true;
            PlayerGoesFirst = index-1;
            PlayersTurn = index;
        }

        public void ResetAI()
        {
        AI_Stage = null;
        PlayersTurn = 0;
        IsNewGame = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">"Fold","Hold","Match","Raise"</param>
        /// <returns>"Raise or Hold or Fold","Match or Fold"</returns>
        public string TexasStateMachineForAI(string state, PokerGame myGame)
        {
            AI_Stage = "Raise or Hold or Fold";

            switch (state)
            {
                case "Fold":
                    RaiseOrHoldOrFold(myGame);
                    break;

                case "Hold":
                    RaiseOrHoldOrFold(myGame);
                    break;

                case "Match":
                    ContinueMatchCheck(myGame);
                    break;

                case "Raise":
                    ContinueMatchCheck(myGame);
                    break;

                case "Raise or Hold or Fold":
                    if(IsNewGame)
                    {
                        IsNewGame = false;
                        if (PlayerGoesFirst == myGame._numPlayers - 1)
                        {
                            Console.WriteLine("Here i am");
                            PlayerGoesFirst = 0;
                            return null;
                        }
                        PlayersTurn = PlayerGoesFirst;
                        PlayerGoesFirst++;
                    }
                    else
                    {
                        RaiseOrHoldOrFold(myGame);
                    }
                    break;
            }
            return AI_Stage;

        }

        private void RaiseOrHoldOrFold(PokerGame myGame)
        {
            if(PlayersTurn != myGame._numPlayers)
            {
                //TODO: always hold for now
                AI_Hold(myGame, PlayersTurn);

                PlayersTurn++;
                RaiseOrHoldOrFold(myGame);
            }
            PlayersTurn = 1;
        }

        private void AI_Fold(PokerGame myGame, int playerIndex)
        {
            myGame.TakeTurn("Fold", playerIndex);
        }
        private void AI_Hold(PokerGame myGame, int playerIndex)
        {
            myGame.TakeTurn("Hold", playerIndex);
        }
        private bool AI_Match(PokerGame myGame, int playerIndex)
        {
            return myGame.TakeTurn("Match", playerIndex);
        }
        private void AI_Raise(PokerGame myGame, int playerIndex, int amount)
        {
            Console.WriteLine("$$ AI has raised " + playerIndex.ToString());
            myGame.TakeTurn("Raise", playerIndex, amount);
        }

        private void ContinueMatchCheck(PokerGame myGame)
        {
                bool buffer = true;
                while (buffer)
                {
                    if (PlayersTurn == myGame._numPlayers) { PlayersTurn = 1; AI_Stage = "Match or Fold"; return; }

                    //TODO: assume match always for now
                    buffer = !AI_Match(myGame, PlayersTurn);
                    PlayersTurn++;
                }
            }
        
    }
}
