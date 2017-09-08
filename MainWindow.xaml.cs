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
        int numberOfPlayers = 2;

        public MainWindow()
        {
            InitializeComponent();
            InitializePlayers();
        }

        private void InitializePlayers()
        {
            myDeck = new DeckOfCards();
            myPlayers = new Player[numberOfPlayers];
            myPlayers[0] = new Player("House");
            myPlayers[1] = new Player("Me");
        }


#region Window Methods
#endregion

#region state machine



#endregion

#region Game Methods

        public void NewHand()
        {
            foreach(Player player in myPlayers)
            {
                player.ResetHand();
            }
        }

        public void DrawCard(int player)
        {
            CardBase myCard = myDeck.DrawTop();
            myPlayers[player].DrawCard(myCard);
        }

        public void Fold(int player, int multiplier = 2)
        {
            myPlayers[player].CashIn(0);

            //check if 
            int k = 0;
            int winner = -1;
            for(int i=0; i<numberOfPlayers-1; i++)
            {
                if (myPlayers[i].Bet != 0)
                {
                    k++; winner = i;
                }
            }
            if ((k == 1)&&(winner >= 0)) EndHand(winner);
        }

        public void CheckWinner()
        {
            int winner = -1;
            int maxPoints = 0;
            for(int i=0; i<numberOfPlayers; i++)
            {
                if(maxPoints < myPlayers[i].CheckPoints())
                 maxPoints = myPlayers[i].CheckPoints();
            }
            EndHand(winner);
        }

        public void EndHand(int winner)
        {
            string NameOfWinner = myPlayers[winner].Name;
            int Pool = 0;
            foreach(Player player in myPlayers)
            {
                Pool += player.Bet;
            }
            myPlayers[winner].Bet = Pool;
            myPlayers[winner].CashIn();                 //TODO: ADD MULTIPLIER FUNCTION OR REMOVE IT

            myDeck = null;
            myPlayers = null;
        }

#endregion

#region Probability Methods



#endregion

#region Menu Methods



#endregion
    }
}

