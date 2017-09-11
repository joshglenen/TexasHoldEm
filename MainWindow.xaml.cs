using System.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Drawing;
using System.Windows.Media;
using System.Diagnostics;


#region References
//https://en.wikipedia.org/wiki/Texas_hold_%27em#Strategy
#endregion


namespace TexasHoldEm
{
    public partial class MainWindow : Window
    {
        DeckOfCards myDeck;
        Player[] myPlayers;
        Player myDealer;
       
        int numberOfPlayers = 2;
        int startingPlayer = 0;
        int stageOfPlay = 0;
        int mostRecentRaise = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializePlayers();
        }

        private void InitializePlayers()
        {
            myDeck = new DeckOfCards();
            myPlayers = new Player[numberOfPlayers];
            myDealer = new Player("Dealer", 5);
            myPlayers[0] = new Player("Mark");
            myPlayers[1] = new Player("Jenny");
        }


#region Window Methods



        #endregion


#region state machine

        private void Button_Click_Fold(object sender, RoutedEventArgs e)
        {
            if ((stageOfPlay != 2) || (stageOfPlay != 3)) return;
            stageOfPlay = 0;
            Fold(0);
            FinishAuto();
        }

        private void Button_Click_Hold(object sender, RoutedEventArgs e)
        {
            if (stageOfPlay != 2) return;
            stageOfPlay = 1;
            mostRecentRaise = 0;
        }

        private void Button_Click_Match(object sender, RoutedEventArgs e)
        {
            if (stageOfPlay != 3) return;
            stageOfPlay = 1;
            myPlayers[0].RaiseBet(mostRecentRaise);
        }

        private void Button_Click_Raise(object sender, RoutedEventArgs e)
        {
            if (stageOfPlay != 2) return;
            stageOfPlay = 3;
            Int32.TryParse(myRaise.Text, out mostRecentRaise);
        }

        private void Button_Click_NewHand(object sender, RoutedEventArgs e)
        {
            if (stageOfPlay != 0) return;
            stageOfPlay = 1;
            NewHand();
        }


#endregion


#region Game Methods

        public void NewHand()
        {
            foreach(Player player in myPlayers)
            {
                player.ResetHand();
                player.DrawCard(myDeck.DrawTop());
                player.DrawCard(myDeck.DrawTop());
            }
        }

        public void DrawCard(int player)
        {
            CardBase myCard = myDeck.DrawTop();
            myPlayers[player].DrawCard(myCard);
        }

        public void Fold(int player, int multiplier = 2)
        {
            myPlayers[player].Fold();

            //check if one left
            int k = 0;
            int winner = -1;
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (myPlayers[i].Playing == true) { k++; winner = i; }
            }
            if (k == 1) EndHand(winner);
        }

        public int[] GetScores()
        {
            int winner = 0;
            int winningScore = 0;
            int scoreBuffer = 0;
            int[] scores = new int[myPlayers.Length+1];
            List<int> valueBuffer = myDealer.GetValues().ToList();
            List<string> suitBuffer = myDealer.GetSuits().ToList();

            for (int i = 0; i < myPlayers.Length; i++)
            {
                //combine dealer and players hand
                valueBuffer.AddRange(myPlayers[i].GetValues().ToList());
                suitBuffer.AddRange(myPlayers[i].GetSuits().ToList());

                //determine winning score
                scoreBuffer = PokerHandValue.Calculate(valueBuffer.ToArray(),suitBuffer.ToArray());
                if (winningScore < scoreBuffer)
                {
                    winningScore = scoreBuffer;
                    winner = i;
                }
                scores[i+1] = scoreBuffer;
            }
            scores[0] = winner;
            return scores;

        } //returns an array with 0 as winner id, and 1-x as player 0-x's scores

        public void EndHand(int foldWinner = -1)
        {

            Debug.WriteLine(myPlayers.Length.ToString());
            int[] scores = new int[myPlayers.Length + 1];
            if (foldWinner == -1)
            {
                scores = GetScores();
            }
            else
            {
                scores[foldWinner] = 100;
                for (int i = 0; i < myPlayers.Length; i++)
                {
                    if (i != foldWinner) scores[i] = 0;
                }
            }
            //string NameOfWinner = myPlayers[scores[0]].Name;
            //TODO: ADD win screen

            int Pool = 0;
            foreach(Player player in myPlayers)
            {
                Pool += player.Bet;
            }
            myPlayers[scores[0]].NewBet(Pool);
            myPlayers[scores[0]].CashIn();                 //TODO: ADD MULTIPLIER FUNCTION OR REMOVE IT
            
        }

        public void FinishAuto()
        {
            EndHand();
        }

#endregion


#region Probability Methods

        public double GetHandProbability(int[] values, string[] suits) //determines the odds of the hand given to win
        {
            return 0;

        } 


#endregion
         

#region Menu Methods

        public void LoadGameSettings() //generates a form with {numplayers, starting funds}
        {

        }
        
        public void LoadAppHistory() //checks for first time, history, leaderboards
        {

        }

        public void NewGame() //load players, ui, and start first hand
        {

        }

        #endregion
        
       
    }
}

