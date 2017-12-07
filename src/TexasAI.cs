using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TexasHoldEm.PokerGame;

namespace TexasHoldEm

    //TODO: fix raise system
{
    class TexasAI
    {
        public CircularInt PlayerGoesFirst { get; private set; }
        public CircularInt PlayersTurn { get; private set; }
        public bool IsNewGame;
        private string _ai_stage;
        public string AI_Stage { get { return _ai_stage; } set { _ai_stage = value; Console.WriteLine("AI Stage now set to: " + _ai_stage); } }

        public TexasAI(PokerGame myGame)
        {
            PlayerGoesFirst = new CircularInt(myGame._players.Length - 1);
            PlayersTurn = new CircularInt(myGame._players.Length - 1);
            ResetAI();
        }

        public void ResetAI()
        {
        AI_Stage = null;
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
                        PlayersTurn.Equals(PlayerGoesFirst);
                        PlayerGoesFirst.Add(1);
                        BlindRounds(myGame);
                    }
                    RaiseOrHoldOrFold(myGame);
                    break;
            }
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
            
            if(shouldRaise(myGame, PlayersTurn._val))
            {
                
                AI_Raise(myGame, PlayersTurn._val, determineRaiseAmount(myGame, PlayersTurn._val));

            }
            else if (shouldFold(myGame, PlayersTurn._val))
            {
                AI_Fold(myGame, PlayersTurn._val);

            }
            else
            {
                AI_Hold(myGame, PlayersTurn._val);

            }
            //repeat full round
            PlayersTurn.Add(1);
            RaiseOrHoldOrFold(myGame);
            return;
        }

        private void BlindRounds(PokerGame myGame)
        {
            Console.WriteLine("@@ BLIND ROUND");
            myGame.TakeTurn("Blind", PlayersTurn._val);
            PlayersTurn.Add(2);
        }

        private void AI_Fold(PokerGame myGame, int playerIndex)
        {
            Console.WriteLine("Decided to Fold!");
            myGame.TakeTurn("Fold", playerIndex);
        }

        private void AI_Hold(PokerGame myGame, int playerIndex)
        {
            Console.WriteLine("Decided to Hold!");
            myGame.TakeTurn("Hold", playerIndex);
        }

        private bool AI_Match(PokerGame myGame, int playerIndex)
        {
            Console.WriteLine("Decided to Match!");
            return myGame.TakeTurn("Match", playerIndex);
        }

        private void AI_Raise(PokerGame myGame, int playerIndex, int amount)
        {
            Console.WriteLine("Decided to Raise!");
            myGame.TakeTurn("Raise", playerIndex, amount);
        }

        private void ContinueMatchCheck(PokerGame myGame)
        {
            //TODO: fix

            Console.Write("Match Check -> New ");
            bool buffer = true;
            while (buffer)
            {
                Console.WriteLine("TEXASAI -> 129 -> " + myGame._players[PlayersTurn._val].Name + " has matched!");

                if (PlayersTurn._val == myGame._players.Length-1)
                {
                    PlayersTurn.Add(1); AI_Stage = "Match or Fold"; return;
                }

                //TODO: assume match always for now
                buffer = !AI_Match(myGame, PlayersTurn._val);
                PlayersTurn.Add(1);
            }

            Console.WriteLine("Match Check -> End ");
        }

        private bool shouldRaise(PokerGame myGame, int playerIndex)
        {
            System.Random random = new System.Random();
            if (myGame._players[playerIndex]._random_decisions)
            {
                int i = random.Next(1, 10);
                if (i % 3 == 0) return true;
            }
            else
            {
                //TODO:
                throw new NotImplementedException("personality choices not yet defined, only framework exists");
            }
            return false;
        }

        private bool shouldFold(PokerGame myGame, int playerIndex)
        {
            System.Random random = new System.Random();
            if (myGame._players[playerIndex]._random_decisions)
            {
                int i = random.Next(1, 10);
                if (i % 5 == 0) return true;
            }
            else
            {
                //TODO:
                throw new NotImplementedException("personality choices not yet defined, only framework exists");
            }
            return false;
        }

        private int determineRaiseAmount(PokerGame myGame, int playerIndex)
        {
            //TODO: implement less random choice

            int min = Convert.ToInt32(myGame._minBet*myGame._players[playerIndex]._minBetFactor);
            int max = Convert.ToInt32(myGame._maxBet * myGame._players[playerIndex]._maxBetFactor);
            System.Random random = new System.Random();
            return random.Next(min, max);
        }
    }
}
