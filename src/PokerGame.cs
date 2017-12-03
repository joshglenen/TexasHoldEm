using System;
using System.Collections.Generic;
using System.Linq;

//TODO: finish scoring system, add leaderboard

namespace TexasHoldEm
{
    class PokerGame : DeckOfCards
    {

#region session vars
        public Player[] Players;
        public Player Dealer;
        public int numPlayers;
        public int gameNumber;
        #endregion

#region game vars
        public int _pot { get; private set; }
        public int _betAmount { get; private set; }
        public int _raisePlayerIndex;
        public int _betAmountPlayerBuffer;
        private string _stage;
        public string _winner { get; private set; }
        public string Stage { get { return _stage; } set {_stage = value; Console.WriteLine("@Player@ Stage now set to: " + _stage); } }

        #endregion

#region Beginning of game

        public PokerGame(string[] names = null, int numCards = 5, int NumPlayers = 2, int funds = 100)
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
            _winner = "No Winner";
            gameNumber++;
            NewDeck();
            _pot = 0;
            _betAmount = 0;
            _betAmountPlayerBuffer = 0;
            _raisePlayerIndex = 0;
            _stage = null;

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
            _pot += amount;
            _betAmount = amount;
        }
        public void MatchBet(int playerIndex)
        {
            Players[playerIndex].RaiseBet(_betAmount);
            _pot += _betAmount;
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

        /// <summary>
        /// Ends the game by calling the getscores class and checking for a tie. Stops the players from playing further.
        /// </summary>
        public void EndGame()
        {
            int numberOfWinners = 0;
            int[] winningPlayers = new int[numPlayers];
            int[] scores = new int[Players.Length + 1];

            //determine winner and points
            scores = GetScores();

            //check for tie
            for (int i = 1; i < scores.Length; i++)
            {
                if (scores[i] == scores[scores[0]+1]) { numberOfWinners++; winningPlayers[numberOfWinners-1] = i - 1; }
            }
            if(numberOfWinners>1)
            {
                
                scores[0] = DetermineWinnerOfTie(winningPlayers)[0];
                if(scores[1]!=0)
                {
                    //case for a true tie: split the pot
                    //TODO: perform proper split based on number of tied players
                    int splitPot = _pot/numPlayers;
                    foreach(Player player in Players)
                    {
                        player.SetFunds(splitPot);
                    }

                    _winner = "Tie";
                    
                    foreach (Player player in Players)
                    {
                        player.Playing = false;
                    }
                    return;
                }
            }
            _winner = Players[scores[0]].Name + " is the winner!";
            //Give money to winner
            Players[scores[0]].SetFunds(_pot);

            //stop players from continuing to play
            foreach (Player player in Players)
            {
                player.Playing = false;
            }
        }

        /// <summary>
        /// Determines the scores of each playing hand and returns the winner index of Players followed by the scores of each.
        /// Assigns the scores to each player.
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
                    List<int> valueBuffer = null;
                    valueBuffer = Dealer.GetValues().ToList();
                    List<string> suitBuffer = null;
                    suitBuffer = Dealer.GetSuits().ToList();

                    //combine dealer and player hand 
                    valueBuffer.AddRange(Players[i].GetValues().ToList());
                    suitBuffer.AddRange(Players[i].GetSuits().ToList());

                    //determine winning score
                    scoreBuffer = HandValueCalculator.BOF_Calculate(valueBuffer.ToArray(), suitBuffer.ToArray());
                    Players[i].Score = scoreBuffer;
                    if (winningScore < scoreBuffer)
                    {
                        winningScore = scoreBuffer;
                        winner = i;
                    }
                    //update buffer
                    scores[i + 1] = scoreBuffer;
                }
                else scores[i + 1] = 0;
            }
            scores[0] = winner;
            return scores;

        }

        /// <summary>
        /// Determines if there is a tie or a a winner between players with similar scores.
        /// </summary>
        /// <param name="tieList">list of players based on index of class variable Players in no particular order</param>
        /// <returns>array with index zero as winning player and index 1 as zero if no tie otherwise 1</returns>
        private int[] DetermineWinnerOfTie(int[] tieList)
        {

            //TODO: when only a subset of players with the same score have the same max value, need to get thier id's and overwrite tieList IMPORTANT
            int[] PlayerHand = new int[2];
            int[] maxValues = new int[tieList.Length];
            List<int> newTieList = new List<int>();
            int maxValue = 0;
            int maxCounter = 0;
            int winner = -1;

            //get player hands
            for (int i = 0; i < tieList.Length; i++)
            {
                PlayerHand = Players[tieList[i]].GetValues();
                for (int u = 0; u < 2; u++)
                {
                    if (PlayerHand[u] == 1) PlayerHand[u] = 14;
                }
                PlayerHand = PlayerHand.OrderByDescending(c => c).ToArray();
                maxValues[i] = PlayerHand[0];
            }

            //determine if only one maximum
            maxValue = maxValues.Max();
            Console.WriteLine("PokerGame -> 312 -> max value: " + maxValue.ToString());
            for (int i = 0; i < tieList.Length; i++)
            {
                if (maxValues[i] == maxValue)
                {
                    Console.WriteLine("PokerGame -> 317 -> determine max loop: " + maxValues[i].ToString());
                    winner = tieList[i];
                    newTieList.Add(tieList[i]);
                    maxCounter++;
                }
            }
            
            //not a true tie
            if(maxCounter==1)
            {
                tieList = new int[2];
                tieList[0] = winner;
                tieList[1] = 0;
                return tieList;
            }

            //TODO: getting bad value for console writeline at 349
            //now check each second highest (of two) player card
            for (int i = 0; i < newTieList.Count; i++)
            {
                PlayerHand = Players[newTieList[i]].GetValues();
                for (int u = 0; u < 2; u++)
                {
                    if (PlayerHand[u] == 1) PlayerHand[u] = 14;
                }
                PlayerHand = PlayerHand.OrderByDescending(c => c).ToArray();
                maxValues[i] = PlayerHand[1];
            }

            maxCounter = 0;
            winner = -1;
            maxValue = maxValues.Max();
            Console.WriteLine("PokerGame -> 347 -> max value: " + maxValue.ToString());
            for (int i = 0; i < newTieList.Count; i++)
            {
                if (maxValues[i] == maxValue)
                {
                    Console.WriteLine("PokerGame -> 353 -> determine max loop: " + maxValues[i].ToString());
                    winner = newTieList[i];
                    maxCounter++;
                }
            }

            //not a true tie
            if (maxCounter == 1)
            {
                tieList = new int[2];
                tieList[0] = winner;
                tieList[1] = 0;
                return tieList;
            }

            //a true tie
            tieList = new int[2];
            tieList[0] = winner;
            tieList[1] = 1;
            return tieList;
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
