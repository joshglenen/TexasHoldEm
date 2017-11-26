using System;
using System.Collections.Generic;
using System.Linq;

namespace TexasHoldEm
{
    class Player
    {
        //Session information
        public string Name;
        public static int NumCards = 0;
        public int Bet { get; private set; }
        public int Funds { get; private set; }

        //internal variables
        public CardBase[] _myHand { get; private set; }
        public int HandIndex {get; private set;}
        public bool Playing;
        public bool AllIn;

        public Player(string name, int numberOfCardsInHand = 2, int funds = 100)
        {
            Name = name;
            Funds = funds;
            NumCards = numberOfCardsInHand;
            ResetHand();
        }
        
        public int[] GetValues()
        {
            int[] buffer = new int[5];
            for (int i = 0; i < _myHand.Length; i++)
            {
                buffer[i] = _myHand[i].Value;
            }
            return buffer;
        }

        public string[] GetSuits()
        {
            string[] buffer = new string[5];
            for (int i = 0; i < _myHand.Length; i++)
            {
                buffer[i] = _myHand[i].Suit;
            }
            return buffer;
        }

        public void DrawCard(CardBase Card)
        {
            _myHand[HandIndex] = Card;
            HandIndex++;
        }

        public void ResetHand()
        {
            Playing = true;
            AllIn = false;
            _myHand = new CardBase[NumCards];
            HandIndex = 0;
            Bet = 0;
        } //clear array and reset attributes

        public bool NewBet(int bet)
        {
            if (AllIn) return false;
            if (bet < Funds)
            {
                Funds += Bet;
                Bet = bet;
                Funds -= Bet;
                return true;
            }

            else
            {
                AllIn = true;
                Bet = Funds;
                Funds = 0;
                return true;
            }

        } //replaces a bet or creates one

        public bool RaiseBet(int bet)
        {
            if (AllIn) return false;
            if (bet < Funds)
            {
                Bet += bet;
                Funds -= bet;
                return true;
            }

            else
            {
                AllIn = true;
                Bet += Funds;
                Funds = 0;
                return true;
            }

        } //adds to a bet or creates one

        public void Fold()
        {
            _myHand = null;
            Playing = false;
            AllIn = false;
        }

        public void SetFunds(int income)
        {
            Funds += income;
            Playing = false;
            AllIn = false;
        } //add winnings to funds

    }

    class Game : DeckOfCards
    {

#region session vars
        public Player[] Players;
        public Player Dealer;
        public int numPlayers;
        public int gameNumber;
        #endregion

#region game vars
        public int _pool { get; private set; }
        public int _lastBet { get; private set; }
        public int _raisePlayerIndex;
#endregion

#region Beginning of game

        public Game(string[] names = null, int numCards = 5, int NumPlayers = 2, int funds = 100)
        {
            gameNumber = -1;
            Players = new Player[NumPlayers];
            Dealer = new Player("Dealer", 5, 0);
            numPlayers = NumPlayers;
            for (int i = 0; i < NumPlayers; i++)
            {
                Players[i] = new Player(names[i], numCards, funds);
            }
            NewGame();
        }
        /// <summary>
        /// Resets game variables within a session of games. Specific to texas holdem. 
        /// </summary>
        public void NewGame()
        {
            gameNumber++;
            NewDeck();
            _pool = 0;
            _lastBet = 0;
            _raisePlayerIndex = 0;

            foreach (Player player in Players)
            {
                player.ResetHand();
                player.DrawCard(DrawTop());
                player.DrawCard(DrawTop());
            }
            Dealer.ResetHand();
            Dealer.DrawCard(DrawTop());
            Dealer.DrawCard(DrawTop());

        }
        private void NewDeck()
        {
            DeckOfCards Buffer = new DeckOfCards();
            Deck = Buffer.Deck;
            Buffer = null;
        }
#endregion

#region Draw Cards
        /// <summary>
        /// Draw a card and give it to the player
        /// </summary>
        /// <param name="playerIndex">Dealer is -1. Players start at 0.</param>
        ///
        public void DrawCard(int playerIndex)
        {
            if (playerIndex == -1)
            {
                Dealer.DrawCard(DrawTop());
            }
            else
            {
                Players[playerIndex].DrawCard(DrawTop());
            }
        }
        public void DrawCard(string playerName)
        {
            if (playerName == "Dealer") Dealer.DrawCard(DrawTop());
            foreach (Player player in Players)
            {
                if (player.Name == playerName) player.DrawCard(DrawTop());
            }

        }
        public void DrawCardPlayers()
        {
            foreach (Player player in Players)
            {
                player.DrawCard(DrawTop());
            }
        }
        #endregion

#region Middle of game
        /// <summary>
        /// All UI friendly actions are now merged into this single method.
        /// </summary>
        /// <param name="stage">Input of user: Pass,Raise,Match,Fold,New Hand</param>
        /// <param name="playerIndex">Player in Players</param>
        /// <param name="bet">Amount if raising bet, not required</param>
        /// <returns>checks when all players have responded to a raise</returns>
        public bool TakeTurn(string stage = "Pass", int playerIndex = 0, int bet = 0)
        {
            switch (stage)
            {
                case "Pass":
                    break;

                case "Raise":
                    if (Players[playerIndex].AllIn) break; //all in's cant raise.
                    _raisePlayerIndex = playerIndex;
                    RaiseBet(playerIndex, bet);
                    break;

                case "Match":
                    MatchBet(playerIndex);
                    int i = playerIndex;
                    if (i == numPlayers) i = 0;
                    else i++;
                    return i == _raisePlayerIndex; //check if all players have reacted to the raise bet

                case "Fold":

                    Fold(playerIndex);
                    break;

                case "New Hand":
                    NewGame();
                    break;
            }

            return false; //no need for check outside of match bet case.
        }
        private void RaiseBet(int playerIndex, int amount)
        {
            Players[playerIndex].RaiseBet(amount);
            _pool += amount;
            _lastBet = amount;
        }
        public void MatchBet(int playerIndex)
        {
            Players[playerIndex].RaiseBet(_lastBet);
            _pool += _lastBet;
        }
        public void Fold(int playerIndex)
        {
            if (!Players[playerIndex].Playing)
            {
                throw new Exception("Cant fold if player has already folded");
            }
            else
            {
                Players[playerIndex].Fold();
                //check if one left

                int k = 0;
                for (int i = 0; i < numPlayers; i++)
                {
                    if (Players[i].Playing == true) { k++;}
                }
                if (k == 1) EndGame();
            }
        }
        #endregion

#region End of game
        public void EndGame()
        {
            int[] scores = new int[Players.Length + 1];
            scores = GetScores();
            string NameOfWinner = Players[scores[0]].Name;

            Players[scores[0]].SetFunds(_pool);
            foreach (Player player in Players)
            {
                player.Playing = false;
            }
        }
        /// <summary>
        /// Determines the scores of each playing hand and returns the winner index of Players followed by the scores of each 
        /// </summary>
        /// <returns>array with 0 as winner id, and 1-x as player 0-x's scores</returns>
        private int[] GetScores()
        {
            int winner = 0;
            int winningScore = 0;
            int scoreBuffer = 0;
            int[] scores = new int[Players.Length + 1]; //zero is winner index of Players

            if (Dealer.HandIndex < 5) throw new Exception("Poker Hand Value only works with all five dealer cards! Need to revise this section or prevent this exception.");
            
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].Playing)
                {

                    List<int> valueBuffer = Dealer.GetValues().ToList();
                    List<string> suitBuffer = Dealer.GetSuits().ToList();

                    //combine dealer and player hand 
                    valueBuffer.AddRange(Players[i].GetValues().ToList());
                    suitBuffer.AddRange(Players[i].GetSuits().ToList());

                    //determine winning score
                    scoreBuffer = PokerHandValue.TexasHoldemCalculate(valueBuffer.ToArray(), suitBuffer.ToArray());
                    Console.WriteLine("Player : " + i.ToString() + " has a score of " + scoreBuffer.ToString());
                    if (winningScore < scoreBuffer)
                    {
                        winningScore = scoreBuffer;
                        winner = i;
                    }
                    else if ((winningScore == scoreBuffer) && (i < 0))
                    {
                        Console.WriteLine("Player.cs -> getscores -> Possible tie");
                    }

                    //update buffer
                    scores[i + 1] = scoreBuffer;
                }
                else scores[i + 1] = 0;
            }
            scores[0] = winner;
            return scores;

        }
        #endregion

#region debug
        public override string ToString()
        {
            string temp = "";
            foreach (Player player in Players)
            {
                temp += player.Name + " playing is " + player.Playing.ToString() + " and funds are " + player.Funds.ToString() + " and bet is " + player.Bet.ToString() + ", ";
            }
            temp += base.ToString();
            return temp;
        }
#endregion

    }
}
