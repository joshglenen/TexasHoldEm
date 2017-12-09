using System;

namespace TexasHoldEm

//todo: inheriting from pokergame might be better looking.

//TODO: fix raise system
{
    class TexasAI
    {
        public CircularInt PlayerGoesFirst { get; private set; }
        public CircularInt PlayersTurn { get; private set; }
        private System.Random randomInteger;
        private int _lastToRaise;

        public bool _newGame;
        private bool _endOfTurn;
        private string _ai_stage;
        public string AI_Stage { get { return _ai_stage; } set { _ai_stage = value; Console.WriteLine("AI Stage now set to: " + _ai_stage); } }

        public TexasAI(PokerGame myGame)
        {
            PlayerGoesFirst = new CircularInt(myGame._players.Length - 1);
            PlayersTurn = new CircularInt(myGame._players.Length - 1);
            randomInteger = new System.Random();
            ResetAI();
        }

        public void ResetAI()
        {
            _endOfTurn = false;
            AI_Stage = "New";
            _lastToRaise = -1;
            _newGame = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">"Fold","Hold","Match","Raise"</param>
        /// <returns>"Raise or Hold or Fold","Raise or Match or Fold, Draw"</returns>
        public string TexasStateMachineForAI(string state, PokerGame myGame)
        {
            if (_endOfTurn)
            {
                // EndTurnActions(myGame);
                // return AI_Stage = "Draw";
            }

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
                    RaiseOrMatchOrFold(myGame);
                    break;

                case "Raise":
                    _lastToRaise = 0;
                    PlayersTurn.Equals(1);
                    RaiseOrMatchOrFold(myGame);
                    break;

                case "Raise or Hold or Fold":
                    if (_newGame)
                    {
                        _newGame = false;
                        PlayersTurn.Equals(PlayerGoesFirst);
                        PlayerGoesFirst.Add(1);
                        BlindRounds(myGame);
                    }
                    RaiseOrHoldOrFold(myGame);
                    break;
            }
            
            //Check if the turn is finished and all players have made an action OTHER THAN blind bet.
            PlayerGoesFirst.Add(2);
            if ((PlayersTurn._val == PlayerGoesFirst._val) && (_lastToRaise == -1))
            {
                PlayerGoesFirst.Subtract(2);
                EndTurnActions(myGame);
                return AI_Stage = "Draw";
            }
            else if ((PlayersTurn._val == 1)&&(PlayerGoesFirst._val==0) && (_lastToRaise == -1)) _endOfTurn = true;
            PlayerGoesFirst.Subtract(2);

            return AI_Stage;

        }

        private void RaiseOrHoldOrFold(PokerGame myGame)
        {
            if (PlayersTurn._val == 0)
            {
                Console.WriteLine("@ my turn!");
                PlayersTurn.Add(1);
                return;
            }

            Console.Write("@" + myGame._players[PlayersTurn._val].Name + "'s turn! -> ");

            if (ShouldRaise(myGame, PlayersTurn._val))
            {
                AI_Raise(myGame, PlayersTurn._val, DetermineRaiseAmount(myGame, PlayersTurn._val));
                PlayersTurn.Add(1);
                while (!myGame._players[PlayersTurn._val].Playing)
                {
                    PlayersTurn.Add(1);
                    Console.WriteLine("Stuck in this loop with yooouu!");
                }
                RaiseOrMatchOrFold(myGame);
                return;
            }

            else if (ShouldFold(myGame, PlayersTurn._val))
            {
                AI_Fold(myGame, PlayersTurn._val);
            }

            else
            {
                AI_Hold(myGame, PlayersTurn._val);
            }

            //repeat full round
            PlayersTurn.Add(1);
            while (!myGame._players[PlayersTurn._val].Playing)
            {
                PlayersTurn.Add(1);
                Console.WriteLine("Stuck in this loop with yooouu!");
            }
            if((AI_Stage != "End")||(myGame.Stage != "End")) CheckFoldVictory(myGame);
            if (AI_Stage != "End") RaiseOrHoldOrFold(myGame);
            return;
        }

        private void RaiseOrMatchOrFold(PokerGame myGame)
        {
            //check if all have raised that are playing
            CircularInt playersTurnBuffer = new CircularInt(myGame._players.Length - 1, 0, PlayersTurn._val);
            while(!myGame._players[playersTurnBuffer._val].Playing)
            {
                playersTurnBuffer.Add(1);
                Console.WriteLine("Stuck in this loop with yooouu!");
            }
            if (playersTurnBuffer._val == _lastToRaise)
            {
                AI_Stage = "Raise or Hold or Fold";
                _lastToRaise = -1;
                playersTurnBuffer.Add(1);
                PlayersTurn.Equals(playersTurnBuffer._val);
                RaiseOrHoldOrFold(myGame);
                return;
            }

            //player's turn, need to exit and wait.
            if (PlayersTurn._val == 0)
            {
                Console.WriteLine("@ #RAISE# my turn!");
                AI_Stage = "Raise or Match or Fold";
                PlayersTurn.Add(1);
                while (!myGame._players[PlayersTurn._val].Playing)
                {
                    PlayersTurn.Add(1);
                    Console.WriteLine("Stuck in this loop with yooouu!");
                }
                return;
            }

            Console.Write("@ #RAISE# " + myGame._players[PlayersTurn._val].Name + "'s turn! -> ");

            int raise_amount = myGame._totalBetAmount - myGame._players[PlayersTurn._val].Bet;

            if (ShouldRaise(myGame, PlayersTurn._val, raise_amount))
            {
                AI_Raise(myGame, PlayersTurn._val, DetermineRaiseAmount(myGame, PlayersTurn._val));
            }

            else if (ShouldFold(myGame, PlayersTurn._val, raise_amount))
            {
                AI_Fold(myGame, PlayersTurn._val);
            }

            else
            {
                AI_Match(myGame, PlayersTurn._val);
            }

            //repeat full round of raise checks

            PlayersTurn.Add(1);
            while (!myGame._players[PlayersTurn._val].Playing)
            {
                PlayersTurn.Add(1);
                Console.WriteLine("Stuck in this loop with yooouu!");
            }
            RaiseOrMatchOrFold(myGame);
            return;
        }

        private void BlindRounds(PokerGame myGame)
        {
            Console.WriteLine("@@ BLIND ROUND");
            myGame.TakeTurn("Blind", PlayersTurn._val);
            PlayersTurn.Add(2);
        }

        private bool ShouldRaise(PokerGame myGame, int playerIndex, int amount = 0)
        {
            if (myGame._players[playerIndex]._roseBetThisTurn)
            {
                if (myGame._players[playerIndex]._random_decisions)
                {
                    int i = randomInteger.Next(1, 10);
                    if (i % 3 == 0) return true;
                }
                else
                {
                    //TODO:
                    throw new NotImplementedException("personality choices not yet defined, only framework exists");
                }
            }
            return false;
        }

        private bool ShouldFold(PokerGame myGame, int playerIndex, int amount = 0)
        {
            
            if (myGame._players[playerIndex]._random_decisions)
            {
                int i = randomInteger.Next(1, 10);
                if (i % 5 == 0) return true;
            }
            else
            {
                //TODO:
                throw new NotImplementedException("personality choices not yet defined, only framework exists");
            }
            return false;
        }

        private int DetermineRaiseAmount(PokerGame myGame, int playerIndex)
        {
            //TODO: implement less random choice

            int min = Convert.ToInt32(myGame._minBet * myGame._players[playerIndex]._minBetFactor);
            int max = Convert.ToInt32(myGame._maxBet * myGame._players[playerIndex]._maxBetFactor);
            return randomInteger.Next(min, max);
        }

        private void CheckFoldVictory(PokerGame myGame)
        {
            int k = 0;
            foreach (Player player in myGame._players)
            {
                if (player.Playing) { k++; }
            }
            if (k == 1)
            {
                AI_Stage = "End";
            }
        }

        private void AI_Fold(PokerGame myGame, int playerIndex)
        {
            Console.WriteLine("Decided to Fold!");
            myGame.TakeTurn("Fold", playerIndex);
            CheckFoldVictory(myGame);
        }

        private void AI_Hold(PokerGame myGame, int playerIndex)
        {
            Console.WriteLine("Decided to Hold!");
            myGame.TakeTurn("Hold", playerIndex);
        }

        private void AI_Match(PokerGame myGame, int playerIndex)
        {
            Console.WriteLine("Decided to Match!");
            myGame.TakeTurn("Match", playerIndex);
        }

        private void AI_Raise(PokerGame myGame, int playerIndex, int amount)
        {
            Console.WriteLine("Decided to Raise!");
            myGame.TakeTurn("Raise", playerIndex, amount);
            _lastToRaise = playerIndex;
            myGame._players[playerIndex]._roseBetThisTurn = true;
            AI_Stage = "Raise or Match or Fold";

        }

        private void EndTurnActions(PokerGame myGame)
        {
            _endOfTurn = false;
            foreach (Player player in myGame._players)
            {
                player._roseBetThisTurn = false;
            }
        }
    }
}
