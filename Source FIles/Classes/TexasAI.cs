using System;

namespace TexasHoldEm

//todo: inheriting from pokergame might be better looking.
{
    class TexasAI
    {
        private System.Random randomInteger;
        private int _lastToRaise;
        private int _dealerOfRound;
        private bool _endOfRound;
        private string _ai_stage;
        private bool _newGame;

        public CircularInt PlayerGoesFirst { get; private set; }
        public CircularInt PlayersTurn { get; private set; }
        public string AI_Stage { get { return _ai_stage; } set { _ai_stage = value; Console.WriteLine("AI Stage now set to: " + _ai_stage); } }

        /// <summary>
        /// Controls the actions of the AI and switches the state of the game only. Note: Does not control human player's actions, only responds to them.
        /// </summary>
        public TexasAI(PokerGame myGame)
        {
            PlayerGoesFirst = new CircularInt(myGame._players.Length - 1);
            PlayersTurn = new CircularInt(myGame._players.Length - 1);
            randomInteger = new System.Random();
            ResetAI();
        }

        /// <summary>
        /// Call at start of session and at the start of each game in the session to reset the variables of this class.
        /// </summary>
        public void ResetAI()
        {
            _endOfRound = false;
            _dealerOfRound = -1;
            AI_Stage = null;
            _lastToRaise = -1;
            _newGame = true;
        }

        /// <summary>
        /// Determines the next state of play based on the stat of the game. Will rework in future to inherit from PokerGame once bugs worked out.
        /// </summary>
        /// <param name="state">"Fold","Hold","Match","Raise"</param>
        /// <returns>"Raise or Hold or Fold","Raise or Match or Fold", "Draw, Player goes first", "Draw, AI goes first"</returns>
        public string ExternalStateMachine(string state, PokerGame myGame)
        {
            switch (state)
            {
                case "Fold":
                    myGame._players[0].TookTurn = true;
                    InternalStateMachine(myGame);
                    break;

                case "Hold":
                    myGame._players[0].TookTurn = true;
                    InternalStateMachine(myGame);
                    break;

                case "Match":
                    RaiseOrMatchOrFold(myGame);
                    break;

                case "Raise":
                    _lastToRaise = 0;
                    PlayersTurn.Equals(1);
                    myGame._players[0].TookTurn = true;
                    RaiseOrMatchOrFold(myGame);
                    break;

                case "Raise or Hold or Fold":
                    if (_newGame)
                    {
                        _newGame = false;
                        PlayersTurn.Equals(PlayerGoesFirst);
                        _dealerOfRound = PlayersTurn._val;
                        PlayerGoesFirst.Add(1);
                        PlayersTurn.Add(1);
                        BlindRounds(myGame);
                    }
                    InternalStateMachine(myGame);
                    break;
            }
            return AI_Stage;
        }

        #region All Players

        /// <summary>
        /// Determine's which player is next and let's them take thier turn.
        /// </summary>
        private void InternalStateMachine(PokerGame myGame)
        {
            //need to check if round is over after player turn
            if (CheckIfEndOfRound(myGame)) { EndRound(myGame); return; }

            //need to exit if player's turn
            if (0 == PlayersTurn._val)
            {
                Console.WriteLine("@ My turn!");
                AI_Stage = "Raise or Hold or Fold";
                NextPlayerTurn(myGame);
                return;
            }

            //need to determine AI's actions
            Console.Write("@" + myGame._players[PlayersTurn._val].Name + "'s turn! -> ");
            RaiseOrHoldOrFold(myGame);
            myGame._players[PlayersTurn._val].TookTurn = true;
            NextPlayerTurn(myGame);

            //check for fold victory
            if ((myGame.Stage == "End") || CheckFoldVictory(myGame)) {EndRound(myGame); return; }

            //need to check if round is over after AI turn
            if (CheckIfEndOfRound(myGame)) { EndRound(myGame); return; }

            //Round is not over yet
            InternalStateMachine(myGame);
        }

        private bool CheckIfEndOfRound(PokerGame myGame)
        {
            //occurs when human player is last to take turn
            if ((_lastToRaise == -1) && (_endOfRound)) return true;

            int k = 0;
            foreach (Player player in myGame._players)
            {
                if (player.TookTurn == false) k++;

            }
            if (k == 0)
            {
                return true;
            }
            else if ((k == 1) && (myGame._players[0].TookTurn == false)) _endOfRound = true;
            return false;
        }

        /// <summary>
        /// Starts a new round of play, calls internal state machine, resets turn variables.
        /// </summary>
        private void NewRound(PokerGame myGame)
        {
            _endOfRound = false;
            foreach (Player player in myGame._players)
            {
                if (player.Playing)
                {
                    player.RoseBetThisTurn = false;
                    player.TookTurn = false;
                }
            }
        }

        /// <summary>
        /// Checks if game is over; otherwise, draws a card for the dealer and starts a new round.
        /// </summary>
        private void EndRound(PokerGame myGame)
        {
            if ((myGame.Stage == "End") || (AI_Stage == "End")) {return; }
            Console.WriteLine("Round has ended");
            myGame.DealerDrawCard();
            NewRound(myGame);

            AI_Stage = "Draw";
            if(PlayersTurn._val==0)
            {
                Console.WriteLine("@ My turn!");
                NextPlayerTurn(myGame);
                AI_Stage = "Draw, player goes first";
            }
        }

        private void RaiseOrHoldOrFold(PokerGame myGame)
        {
            if (!myGame._players[PlayersTurn._val].Playing) { Console.WriteLine(" player not playing tried to play!"); return; }
            if (ShouldRaise(myGame, PlayersTurn._val))
            {
                AI_Raise(myGame, PlayersTurn._val, DetermineRaiseAmount(myGame, PlayersTurn._val));
                NextPlayerTurn(myGame);
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
        }

        private void RaiseOrMatchOrFold(PokerGame myGame)
        {
            //check if all have raised that are playing
            CircularInt playersTurnBuffer = new CircularInt(myGame._players.Length - 1, 0, PlayersTurn._val);
            while(!myGame._players[playersTurnBuffer._val].Playing)
            {
                playersTurnBuffer.Add(1);
            }
            if (playersTurnBuffer._val == _lastToRaise)
            {
                AI_Stage = "Raise or Hold or Fold";
                _lastToRaise = -1;
                playersTurnBuffer.Add(1);
                PlayersTurn.Equals(playersTurnBuffer._val);
                Console.WriteLine("Raise round has ended");
                InternalStateMachine(myGame);
                return;
            }

            //player's turn, need to exit and wait.
            if (PlayersTurn._val == 0)
            {
                Console.WriteLine("@ #RAISE# my turn!");
                AI_Stage = "Raise or Match or Fold";
                NextPlayerTurn(myGame);
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
            NextPlayerTurn(myGame);
            RaiseOrMatchOrFold(myGame);
        }

        private void BlindRounds(PokerGame myGame)
        {
            Console.WriteLine("@@ BLIND ROUND");
            myGame.TakeTurn("Blind", PlayersTurn._val);
            Console.WriteLine("@" + myGame._players[PlayersTurn._val].Name + "'s turn! -> Small Blind.");
            PlayersTurn.Add(1);
            Console.WriteLine("@" + myGame._players[PlayersTurn._val].Name + "'s turn! -> Big Blind.");
            PlayersTurn.Add(1);
        }

        private void NextPlayerTurn(PokerGame myGame)
        {
            PlayersTurn.Add(1);
            while (!myGame._players[PlayersTurn._val].Playing)
            {
                PlayersTurn.Add(1);
                Console.Write("&");
            }
        }

        #endregion

        #region Determine Actions

        /// <summary>
        /// Determines if a player will to raise based on the personality of the player and properties of the game.
        /// </summary>
        private bool ShouldRaise(PokerGame myGame, int playerIndex, int amount = 0)
        {
            if (!myGame._players[playerIndex].RoseBetThisTurn)
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

        /// <summary>
        /// Determines if a player will to fold based on the personality of the player and properties of the game.
        /// </summary>
        private bool ShouldFold(PokerGame myGame, int playerIndex, int amount = 0)
        {
            
            if (myGame._players[playerIndex]._random_decisions)
            {
                int i = randomInteger.Next(1, 10);
                if (i % 10 == 0) return true;
            }
            else
            {
                //TODO:
                throw new NotImplementedException("personality choices not yet defined, only framework exists");
            }
            return false;
        }

        /// <summary>
        /// Chooses an amount to raise based on the personality of the player and properties of the game.
        /// </summary>
        private int DetermineRaiseAmount(PokerGame myGame, int playerIndex)
        {
            //TODO: implement less random choice

            int min = Convert.ToInt32(myGame._minBet * myGame._players[playerIndex]._minBetFactor);
            int max = Convert.ToInt32(myGame._maxBet * myGame._players[playerIndex]._maxBetFactor);
            return randomInteger.Next(min, max);
        }

        /// <summary>
        /// Sets the stage to end if there  is a fold victory and returns true. Fold victory occurs if only one player is left playing while the game has not ended.
        /// </summary>
        private bool CheckFoldVictory(PokerGame myGame)
        {
            int k = 0;
            foreach (Player player in myGame._players)
            {
                if (player.Playing) { k++; }
            }
            if (k == 1)
            {
                AI_Stage = "End";
                return true;
            }
            return false;
        }

        #endregion
        
        #region AI Actions

        /// <summary>
        /// Forces an AI player to take a turn, changes the stage, updates variables.
        /// </summary>
        private void AI_Fold(PokerGame myGame, int playerIndex)
        {
            Console.WriteLine("Decided to Fold!");
            myGame.TakeTurn("Fold", playerIndex);
            if (CheckFoldVictory(myGame)) return;
            AI_Stage = "Raise or Hold or Fold";
        }

        /// <summary>
        /// Forces an AI player to take a turn, changes the stage, updates variables.
        /// </summary>
        private void AI_Hold(PokerGame myGame, int playerIndex)
        {
            Console.WriteLine("Decided to Hold!");
            myGame.TakeTurn("Hold", playerIndex);
            AI_Stage = "Raise or Hold or Fold";
        }

        /// <summary>
        /// Forces an AI player to take a turn, changes the stage, updates variables.
        /// </summary>
        private void AI_Match(PokerGame myGame, int playerIndex)
        {
            Console.WriteLine("Decided to Match!");
            myGame.TakeTurn("Match", playerIndex);
        }

        /// <summary>
        /// Forces an AI player to take a turn, changes the stage, updates variables.
        /// </summary>
        private void AI_Raise(PokerGame myGame, int playerIndex, int amount)
        {
            Console.WriteLine("Decided to Raise!");
            myGame.TakeTurn("Raise", playerIndex, amount);
            _lastToRaise = playerIndex;
            myGame._players[playerIndex].RoseBetThisTurn = true;
            AI_Stage = "Raise or Match or Fold";

        }

        #endregion

        #region Debug

    /// <summary>
    /// Returns the value of the public and private class variables in thier current state of the TexasAI class only.
    /// </summary>
    public override string ToString()
    {
        string buffer =  "TexasAI Debug -> " ;
        buffer += "The current randomInteger is " + randomInteger.ToString();
        buffer += ", ";
        buffer += "The current _lastToRaise is " + _lastToRaise.ToString();
        buffer += ", ";
        buffer += "The current _dealerOfRound is " + _dealerOfRound.ToString();
        buffer += ", ";
        buffer += "The current _endOfTurn is " + _endOfRound.ToString();
        buffer += ", ";
        buffer += "The current _ai_stage is " + _ai_stage.ToString();
        buffer += ", ";
        buffer += "The current PlayerGoesFirst is " + PlayerGoesFirst._val.ToString();
        buffer += ", ";
        buffer += "The current PlayersTurn is " + PlayersTurn._val.ToString();
        buffer += ", ";
        buffer += "The current _newGame is " + _newGame.ToString();
        buffer += ".";
        return buffer;
    }

    #endregion

    }
}
