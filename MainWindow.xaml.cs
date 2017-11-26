﻿using System.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Drawing;
using System.Windows.Media;  
using System.Diagnostics;
using System.Windows.Media.Imaging;


#region References
//https://en.wikipedia.org/wiki/Texas_hold_%27em#Strategy
#endregion


namespace TexasHoldEm
{
    public partial class MainWindow : Window
    {
        Game myGame;
        TexasAI myAI;

        private string _stage = null;
        public string Stage { get { return _stage; } set { _stage = value; Console.WriteLine("Stage now set to: " + _stage); } }

        int raiseAmount = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializePlayers();
            TextBlock_CurrentPlayerName.Text = myGame.Players[0].Name;
            TextBlock_GameNumberCounter.Text = "Game 1";
        }
        private void InitializePlayers()
        {
            //TODO: collect data from xml instead
            string[] args = { "Jason", "Annie","Oswald" , "Cooper"};
            int numPlayers = 4;
            int funds = 100;

            myAI = new TexasAI();

            myGame = new Game(args, 5, numPlayers, funds);
            


        }




#region state machine

        private void Button_Click_Fold(object sender, RoutedEventArgs e)
        {
            if ((Stage == "Match or Fold") || (Stage == "Raise or Hold or Fold"))
            {
                int k = 0;
                foreach (Player player in myGame.Players)
                {
                    if (player.Playing) { k++; }
                }
                if (k == 2)
                {
                    Stage = null;
                }

                myGame.TakeTurn("Fold");
                Stage = "Fold";
                WaitForAI();
            }
        }

        private void Button_Click_Hold(object sender, RoutedEventArgs e)
        {
            if (Stage == "Raise or Hold or Fold")
            {
                myGame.TakeTurn();
                Stage = "Hold";
                WaitForAI();
            }
        }

        private void Button_Click_Match(object sender, RoutedEventArgs e)
        {
            if (Stage == "Match or Fold")
            {
                Stage = "Match";
                //Triggers when all players have responded to a raise.
                if (myGame.TakeTurn("Match"))
                {
                    Stage = "Raise or Hold or Fold";
                    myAI.PlayerHasRaised = false;
                }
                WaitForAI();
            }
        }

        private void Button_Click_Raise(object sender, RoutedEventArgs e)
        {
            if (Stage == "Raise or Hold or Fold")
            {
                //Checks if raise is appropriate.
                if ((raiseAmount <= 0) || (raiseAmount > myGame.Players[0].Funds))
                {
                    Console.WriteLine( "Dealer won't accept your bet!");
                    return;
                }
                myGame.TakeTurn("Raise",0, raiseAmount);
                Stage = "Raise";
                WaitForAI();
            }
        }

        private void Button_Click_NewHand(object sender, RoutedEventArgs e)
        {
            //generates a new hand and populates image frames.
            if ((Stage == "End") || (Stage == null))
            {

                //clears extra image frames when starting a new game if a game had previously been played in a session.
                if (imgDealer5.Source != null)
                {
                    imgDealer5.Source = new BitmapImage();
                    imgDealer4.Source = new BitmapImage();
                    imgDealer3.Source = new BitmapImage();
                }

                //update Game class
                myGame.TakeTurn("New Hand");

                //load cards
                imgDealer1.Source = new BitmapImage(new Uri(@myGame.Dealer._myHand[0].Asset));
                imgDealer2.Source = new BitmapImage(new Uri(@myGame.Dealer._myHand[1].Asset));
                imgMain1.Source = new BitmapImage(new Uri(@myGame.Players[0]._myHand[0].Asset));
                imgMain2.Source = new BitmapImage(new Uri(@myGame.Players[0]._myHand[1].Asset));
                imgONE1.Source = new BitmapImage(new Uri(@myGame.Players[1]._myHand[0].Asset));
                imgONE2.Source = new BitmapImage(new Uri(@myGame.Players[1]._myHand[1].Asset));
                if(myGame.numPlayers>2)
                {
                    imgTWO1.Source = new BitmapImage(new Uri(@myGame.Players[2]._myHand[0].Asset));
                    imgTWO2.Source = new BitmapImage(new Uri(@myGame.Players[2]._myHand[1].Asset));
                }
                if (myGame.numPlayers > 3)
                {
                    imgTHREE1.Source = new BitmapImage(new Uri(@myGame.Players[3]._myHand[0].Asset));
                    imgTHREE2.Source = new BitmapImage(new Uri(@myGame.Players[3]._myHand[1].Asset));
                }
                if (myGame.numPlayers > 4)
                {
                    throw new Exception("Only supports 4 players");
                }

                //update window
                TextBlock_GameNumberCounter.Text = "Game " + myGame.gameNumber.ToString();

                //Start game
                Stage = "Raise or Hold or Fold";
                WaitForAI();
            }


        }

        private void WaitForAI()
        {
        Stage  = myAI.TexasStateMachineForAI(Stage, myGame);
        if (Stage == null) { Stage = "Raise or Hold or Fold"; }
        else if (Stage == "Raise or Hold or Fold") { DealerDrawNext();}

        //AI must continue if player stops playing.
        if(!myGame.Players[0].Playing)
        {
            //waits until all dealer cards are drawn or all but one players have folded.
            int k = 0;
            foreach(Player player in myGame.Players)
            {
                 if (player.Playing) { k++; }
            }
                if ((k!=0)&&(myGame.Dealer.HandIndex<=4))
            {
                WaitForAI();
            }
        }
        }

        private void DealerDrawNext()
        {
            myGame.DrawCard(-1);
            switch (myGame.Dealer.HandIndex)
            {

                case 3:
                    imgDealer3.Source = new BitmapImage(new Uri(@myGame.Dealer._myHand[2].Asset));
                    break;

                case 4:
                    imgDealer4.Source = new BitmapImage(new Uri(@myGame.Dealer._myHand[3].Asset));
                    break;

                case 5:
                    imgDealer5.Source = new BitmapImage(new Uri(@myGame.Dealer._myHand[4].Asset));
                    myGame.EndGame();
                    Stage = "End";
                    break;
            }
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

        private void MyRaise_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Int32.TryParse(myRaise.Text, out raiseAmount);

        } //Raise Textbox Ultrafast Input Updater

        #endregion


    }
}

