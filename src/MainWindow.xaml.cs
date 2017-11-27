using System.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Drawing;
using System.Windows.Media;  
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Controls;


#region Credits
//https://en.wikipedia.org/wiki/Texas_hold_%27em#Strategy
// App desktop icon credit -> Icon made by freepik from www.flaticon.com
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
            UpdateTopLeftPanel();
            UpdateRightPanelStats();
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
                //TODO: needs restructuring as some methods need to occur at end of game and some at beginning.
                myGame.TakeTurn("New Hand");
                ResetMainPanel();
                UpdateTopLeftPanel();
                UpdateRightPanelImages();

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

                    //TODO: check method order either start of game or end of game
                    UpdateLeftPanel();
                    UpdateTopLeftPanel();
                    UpdateRightPanelStats();

                    Stage = "End";
                    break;
            }
        }

        #endregion

        #region Menu Methods

        public void UpdateLeftPanel()
        {
            string topBuffer = "";
            string bottomBuffer = TextBox_ScrollViewer_LeftSide.Text;
            topBuffer += "Game: " + myGame.gameNumber.ToString() + "\n";
            for (int i = 0; i < myGame.numPlayers ; i++)
            {
                topBuffer += myGame.Players[i].Name.ToString() + "'s Score: " + myGame.Players[i].Score.ToString() + "\n";

                Console.WriteLine("198:" + topBuffer);
            }
            topBuffer += "\n";
            topBuffer += bottomBuffer;
            TextBox_ScrollViewer_LeftSide.Text = topBuffer;
            topBuffer = null;
            bottomBuffer = null;
        }

        public void UpdateRightPanelStats()
        {
            TextBlock_Player2_Name.Text = myGame.Players[1].Name.ToString();
            TextBlock_Player2_Funds.Text = "Funds: " + myGame.Players[1].Funds.ToString();
            TextBlock_Player3_Name.Text = null;
            TextBlock_Player3_Funds.Text = null;
            TextBlock_Player4_Name.Text = null;
            TextBlock_Player4_Funds.Text = null;
            if (myGame.numPlayers > 2)
            {
                TextBlock_Player3_Funds.Text = "Funds: " + myGame.Players[2].Funds.ToString();
                TextBlock_Player3_Name.Text = myGame.Players[2].Name.ToString();
            }
            if (myGame.numPlayers > 3)
            {
                TextBlock_Player4_Funds.Text = "Funds: " + myGame.Players[3].Funds.ToString();
                TextBlock_Player4_Name.Text = myGame.Players[3].Name.ToString();
            }
            if (myGame.numPlayers > 4)
            {
                throw new Exception("Only supports 4 players");
            }
        }

        public void UpdateRightPanelImages()
        {
            imgONE1.Source = new BitmapImage(new Uri(@myGame.Players[1]._myHand[0].Asset));
            imgONE2.Source = new BitmapImage(new Uri(@myGame.Players[1]._myHand[1].Asset));
            if (myGame.numPlayers > 2)
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
        }

        public void ResetMainPanel()
        {
            //clears extra image frames when starting a new game if a game had previously been played in a session.
            if (imgDealer5.Source != null)
            {
                imgDealer5.Source = new BitmapImage();
                imgDealer4.Source = new BitmapImage();
                imgDealer3.Source = new BitmapImage();
            }

            imgDealer1.Source = new BitmapImage(new Uri(@myGame.Dealer._myHand[0].Asset));
            imgDealer2.Source = new BitmapImage(new Uri(@myGame.Dealer._myHand[1].Asset));
            imgMain1.Source = new BitmapImage(new Uri(@myGame.Players[0]._myHand[0].Asset));
            imgMain2.Source = new BitmapImage(new Uri(@myGame.Players[0]._myHand[1].Asset));
        }

        public void UpdateTopLeftPanel()
        {
            TextBlock_GameNumberCounter.Text = "Game " + myGame.gameNumber.ToString();
            TextBlock_CurrentPlayerName.Text =  myGame.Players[0].Name.ToString();
            TextBlock_Game_Pool.Text = myGame._pool.ToString();
            TextBlock_Player1_CurrentBet.Text = "Current Bet: " + myGame.Players[0].Bet.ToString();
            TextBlock_Player1_Funds.Text = "Funds: " + myGame.Players[0].Funds.ToString();
            TextBlock_Player1_NetProfit.Text = "Net: " + myGame.Players[0].Profit.ToString();
        }

        private void MyRaise_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Int32.TryParse(myRaise.Text, out raiseAmount);

        } //Raise Textbox Ultrafast Input Updater

        #endregion


    }
}

