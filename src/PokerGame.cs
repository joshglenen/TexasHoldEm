using System;
using System.Collections.Generic;
using System.Linq;

//TODO: finish scoring system, add leaderboard

namespace TexasHoldEm
{
    class PokerGame : DeckOfCards
    {

    #region session vars (do not change in a session)
        public Player[] _players { get; private set; }
        public Player _dealer { get; private set; }
        public int _gameNumber { get; private set; }
        public int _minBet { get; private set; }
        public int _maxBet { get; private set; }
        public bool _noLimits { get; private set; }
        public int _smallBlind { get; private set; }
        public int _bigBlind { get; private set; }
    #endregion

    #region game vars (change every game in a session)
    public int _pot { get; private set; }
    public int _betAmount { get; private set; }
    public int _raisePlayerIndex;
    public int _betAmountPlayerBuffer;
    private string _stage;
    public string _winner { get; private set; }
    public string Stage { get { return _stage; } set {_stage = value; Console.WriteLine("Player Stage now set to: " + _stage); } }

    #endregion

    #region Beginning of game

    public PokerGame(string[] names = null, int numCards = 5, int funds = 100, bool noLimits = false, int smallBlind = 5, int bigBlind = 10, int minBet = 10, int maxBet = 100)
    {
        _gameNumber = -1;
        _minBet = minBet;
        _maxBet = maxBet;
        _smallBlind = smallBlind;
        _bigBlind = bigBlind;
        _noLimits = noLimits;
        _players = new Player[names.Length];
        _dealer = new Player("Dealer", 5, 0);
        for (int i = 0; i < names.Length; i++)
        {
            _players[i] = new Player(names[i], numCards, funds);
        }
        NewGame();
    }
    /// <summary>
    /// Resets game variables within a session of games. Specific to texas holdem. 
    /// </summary>
    public void NewGame()
    {
        _winner = "No Winner";
        _gameNumber++;
        NewDeck();
        _pot = 0;
        _betAmount = 0;
        _betAmountPlayerBuffer = 0;
        _raisePlayerIndex = 0;
        _stage = null;

        foreach (Player player in _players)
        {
            player.ResetHand();
            player.DrawCard(DrawTop());
            player.DrawCard(DrawTop());
        }
        _dealer.ResetHand();
        _dealer.DrawCard(DrawTop());
        _dealer.DrawCard(DrawTop());

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
                _dealer.DrawCard(DrawTop());
            }
            else
            {
                _players[playerIndex].DrawCard(DrawTop());
            }
        }
        public void DrawCard(string playerName)
        {
            if (playerName == "Dealer") _dealer.DrawCard(DrawTop());
            foreach (Player player in _players)
            {
                if (player.Name == playerName) player.DrawCard(DrawTop());
            }

        }
        public void DrawCardPlayers()
        {
            foreach (Player player in _players)
            {
                player.DrawCard(DrawTop());
            }
        }
        #endregion

    #region Middle of game

        /// <summary>
        /// All UI friendly actions are now merged into this single method.
        /// </summary>
        /// <param name="stage">Input of user: Pass,Raise,Match,Fold,New Hand,Blind</param>
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
                    if (_players[playerIndex].AllIn) throw new Exception("all in's cant raise.");
                    _raisePlayerIndex = playerIndex;
                    RaiseBet(playerIndex, bet);
                    break;

                case "Match":
                    MatchBet(playerIndex);
                    int i = playerIndex;
                    if (i == _players.Length - 1) i = 0;
                    else i++;
                    return i == _raisePlayerIndex; //check if all players have reacted to the raise bet

                case "Fold":

                    Fold(playerIndex);
                    break;

                case "New Hand":
                    NewGame();
                    break;

                case "Blind":
                    SmallBlind(playerIndex);
                    if (playerIndex + 1 < _players.Length) BigBlind(playerIndex + 1);
                    else BigBlind(0);
                    break;
            }

            return false; //no need for check outside of match bet case.
        }

        private void RaiseBet(int playerIndex, int amount)
        {
            _players[playerIndex].RaiseBet(amount);
            _pot += amount;
            _betAmount = amount;
        }
        private void MatchBet(int playerIndex)
        {
            try
            {
                _players[playerIndex].RaiseBet(_betAmount);
                _pot += _betAmount;
            }
            catch
            {
                throw new Exception("Index out of range error in PokerGame -> TakeTurn: player index is " + playerIndex.ToString() + "," + ToString());
            }

        }
        private void Fold(int playerIndex)
        {
            if (!_players[playerIndex].Playing)
            {
                throw new Exception("Cant fold if player has already folded");
            }
            else
            {
                _players[playerIndex].Fold();
                //check if one left

                int k = 0;
                for (int i = 0; i < _players.Length; i++)
                {
                    if (_players[i].Playing == true) { k++;}
                }
                if (k == 1) EndGame();
            }
        }
        private void SmallBlind(int playerIndex)
        {
            _players[playerIndex].BlindBet(_smallBlind);
            _pot += _smallBlind;
        }
        private void BigBlind(int playerIndex)
        {
            _players[playerIndex].BlindBet(_bigBlind);
            _pot += _bigBlind;
        }
        #endregion

    #region End of game

    /// <summary>
    /// Ends the game by calling the getscores class and checking for a tie. Stops the players from playing further.
    /// </summary>
    public void EndGame()
    {
        List<int> winningPlayers = new List<int>();
        int[] scores = new int[_players.Length + 1];

        //determine winner and points
        scores = GetScores();

        //check for tie
        for (int i = 1; i < scores.Length; i++)
        {
            if (scores[i] == scores[scores[0] + 1])
            {
                winningPlayers.Add(i-1);
            }
        }

        if(winningPlayers.Count > 1)
        {
            List<int> tieChecker;
            tieChecker = DetermineWinnerOfTie(winningPlayers.ToArray()).ToList();
            scores[0] = tieChecker[0];
            if(tieChecker[1]!=0)
            {
                //case for a true tie: split the pot

                int splitPot = _pot/ tieChecker[2];
                _winner = "Tie between ";
                for (int j = 0; j < tieChecker[2] - 1; j++)
                {
                    _players[tieChecker[3+j]].SetFunds(splitPot); _winner += _players[tieChecker[3 + j]].Name + " and ";
                }
                _players[tieChecker[tieChecker[2]]].SetFunds(splitPot); _winner += _players[tieChecker[tieChecker[2]]].Name;


                foreach (Player player in _players)
                {
                    player.Playing = false;
                }
                return;
            }
        }
        _winner = _players[scores[0]].Name + " is the winner!";
        //Give money to winner
        _players[scores[0]].SetFunds(_pot);

        //stop players from continuing to play
        foreach (Player player in _players)
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
        int[] scores = new int[_players.Length + 1]; //zero is winner index of Players

        if (_dealer.HandIndex < 5) throw new Exception("Poker Hand Value only works with all five dealer cards! Need to revise this section or prevent this exception.");
            
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].Playing)
            {
                List<int> valueBuffer = null;
                valueBuffer = _dealer.GetValues().ToList();
                List<string> suitBuffer = null;
                suitBuffer = _dealer.GetSuits().ToList();

                //combine dealer and player hand 
                valueBuffer.AddRange(_players[i].GetValues().ToList());
                suitBuffer.AddRange(_players[i].GetSuits().ToList());

                //determine winning score
                scoreBuffer = HandValueCalculator.BOF_Calculate(valueBuffer.ToArray(), suitBuffer.ToArray());
                _players[i].Score = scoreBuffer;
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
    /// <returns>dynamic array with index zero as winning player and index 1 as zero if no tie otherwise 1, followed by number of winners and thier indexes in cases with a true tie</returns>
    private int[] DetermineWinnerOfTie(int[] tieList)
    {

        int[] PlayerHand = new int[2];
        int[] maxValues = new int[tieList.Length];
        List<int> newTieList = new List<int>();
        List<int> newTieList2 = new List<int>();
        int[] returnBuffer = new int[tieList.Length + 3];
        int maxValue = 0;
        int maxCounter = 0;
        int winner = -1;

        //get player hands
        
        for (int i = 0; i < tieList.Length; i++)
        {
            PlayerHand = _players[tieList[i]].GetValues();
            for (int u = 0; u < 2; u++)
            {
                //ace high
                if (PlayerHand[u] == 1) PlayerHand[u] = 14;
            }
            PlayerHand = PlayerHand.OrderByDescending(c => c).ToArray();
            maxValues[i] = PlayerHand[0];
            Console.WriteLine("TIE: " + _players[tieList[i]].Name + " has a hand of "  + PlayerHand[0].ToString() + PlayerHand[1].ToString());
        }

        //determine if only one maximum
        
        maxValue = maxValues.Max();
        for (int i = 0; i < tieList.Length; i++)
        {
            if (maxValues[i] == maxValue)
            {
                winner = tieList[i];
                newTieList.Add(tieList[i]);
                maxCounter++;
            }
        }
            
        //not a true tie
        if(maxCounter==1)
        {
            returnBuffer[0] = winner;
            returnBuffer[1] = 0;
            return returnBuffer;
        }
        
        //now check each second highest (of two) player card
        for (int i = 0; i < newTieList.Count; i++)
        {
            PlayerHand = _players[newTieList[i]].GetValues();
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
        for (int i = 0; i < newTieList.Count; i++)
        {
            if (maxValues[i] == maxValue)
            {
                winner = newTieList[i];
                newTieList2.Add(newTieList[i]);
                maxCounter++;
                }
            }

        //not a true tie
        if (maxCounter == 1)
        {
            returnBuffer[0] = winner;
            returnBuffer[1] = 0;
            return returnBuffer;
        }

        //a true tie
        returnBuffer[0] = winner;
        returnBuffer[1] = 1;
        returnBuffer[2] = newTieList2.Count;
        Console.WriteLine("maxcounter " + maxCounter.ToString()+ "/ tielength " + newTieList2.Count.ToString());
        //TODO: fix
        if (newTieList2.Count < 2) throw new IndexOutOfRangeException("Tie list cannot contain less than two people. Are the card values accurate? I'm still getting this exception but it only happens rarely. Need to fix!");
        for (int d = 0; d < newTieList2.Count; d++)
        {
            returnBuffer[3+d] = newTieList2[d];
        }
        return returnBuffer;
    }

    #endregion

    #region debug

        /// <summary>
        /// Outputs the state of the game into a string.
        /// </summary>
        /// <returns>state of game</returns>
        public override string ToString()
        {
            string temp = "";
            foreach (Player player in _players)
            {
                temp += player.Name + " playing is " + player.Playing.ToString() + " and funds are " + player.Funds.ToString() + " and bet is " + player.Bet.ToString() + ", ";
            }
            temp += base.ToString();
            return temp;
        }
#endregion

    }
}
