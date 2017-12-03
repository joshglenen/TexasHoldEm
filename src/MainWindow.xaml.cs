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

//TODO: finish UI structuring

namespace TexasHoldEm
{
    public partial class MainWindow : Window
    {
        PokerGame myGame;
        TexasAI myAI;

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
            int funds = 1000;

            myAI = new TexasAI();
            myGame = new PokerGame(args, 5, numPlayers, funds);
        }

        #region state machine (changes _stage property of Game class)

        /// <summary>
        /// sets stage to hold, updates myGame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Fold(object sender, RoutedEventArgs e)
        {
            if ((myGame.Stage == "Match or Fold") || (myGame.Stage == "Raise or Hold or Fold"))
            {
                int k = 0;
                foreach (Player player in myGame.Players)
                {
                    if (player.Playing) { k++; }
                }
                if (k == 2)
                {
                    myGame.TakeTurn("Fold");
                    UpdateEndGameUI();

                }
                else
                {
                    myGame.TakeTurn("Fold");
                    myGame.Stage = "Fold";
                    UpdateTopLeftPanel();
                    WaitForAI();
                }
            }
        }

        /// <summary>
        /// sets stage to hold, updates myGame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Hold(object sender, RoutedEventArgs e)
        {
            if (myGame.Stage == "Raise or Hold or Fold")
            {
                myGame.TakeTurn();
                myGame.Stage = "Hold";
                WaitForAI();
            }
        }

        /// <summary>
        /// sets stage to match or raiseorholdorfold if player is the last to match, updates myGame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Match(object sender, RoutedEventArgs e)
        {
            if (myGame.Stage == "Match or Fold")
            {
                myGame.Stage = "Match";
                //Triggers when all players have responded to a raise.
                if (myGame.TakeTurn("Match")) myGame.Stage = "Raise or Hold or Fold";
                UpdateTopLeftPanel();
                WaitForAI();
            }
        }

        /// <summary>
        /// Sets stage to raise, updates myGame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Raise(object sender, RoutedEventArgs e)
        {
            if (myGame.Stage == "Raise or Hold or Fold")
            {
                //Checks if raise is appropriate.
                if ((myGame._betAmountPlayerBuffer <= 0) || (myGame._betAmountPlayerBuffer > myGame.Players[0].Funds))
                {
                    UpdateLeftPanel("Dealer won't accept your bet!");
                    return;
                }
                myGame.TakeTurn("Raise",0, myGame._betAmountPlayerBuffer);
                myGame.Stage = "Raise";
                UpdateTopLeftPanel();
                WaitForAI();
            }
        }

        /// <summary>
        /// Reset's the game UI and sets the stage to its default.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_NewHand(object sender, RoutedEventArgs e)
        {
            //generates a new hand and populates image frames.
            if ((myGame.Stage == "End") || (myGame.Stage == null))
            {
                //TODO: needs restructuring as some methods need to occur at end of game and some at beginning.
                myGame.TakeTurn("New Hand");
                myGame.Stage = "Raise or Hold or Fold";
                ResetMainPanel();
                UpdateTopLeftPanel();
                UpdateRightPanelImages();

                //Start game
                WaitForAI();
            }
        }

        /// <summary>
        /// Allows AI to take it's turn and determine's the next state for the player
        /// </summary>
        private void WaitForAI()
        {
        myGame.Stage  = myAI.TexasStateMachineForAI(myGame.Stage, myGame);
        if (myGame.Stage == null) { myGame.Stage = "Raise or Hold or Fold"; }
        else if (myGame.Stage == "Raise or Hold or Fold") { DealerDrawNext();}

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

        /// <summary>
        /// Add's a card to the dealer's hand and updates the images. One of two ways to end the game.
        /// </summary>
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
                    UpdateEndGameUI();
                    break;
            }
        }

        /// <summary>
        /// Sets stage to end, updates the UI when the game ends
        /// </summary>
        private void UpdateEndGameUI()
        {
            myGame.Stage = "End";
            PrintScores();
            UpdateTopLeftPanel();
            UpdateRightPanelStats();

            //unique endgame updates
            TextBlock_GameWinner.Text = myGame._winner;
        }

        #endregion

        #region Menu Methods (changes XAML GUI or reads from it)

        /// <summary>
        /// Output's each players scores along with the game number at end of game
        /// </summary>
        private void PrintScores()
        {
            string buffer = "Game: " + myGame.gameNumber.ToString() + "\n";
            for (int i = 0; i < myGame.numPlayers ; i++)
            {
                buffer += myGame.Players[i].Name.ToString() + "'s Score: " + myGame.Players[i].Score.ToString() + "\n";
            }
            UpdateLeftPanel(buffer);
        }

        /// <summary>
        /// Updates left panel text output
        /// </summary>
        /// <param name="args">Text for the user to see</param>
        private void UpdateLeftPanel(string args)
        {
            string topBuffer = "\n" + args;
            string bottomBuffer = TextBox_ScrollViewer_LeftSide.Text;
            topBuffer += "\n";
            topBuffer += bottomBuffer;
            TextBox_ScrollViewer_LeftSide.Text = topBuffer;
        }

        /// <summary>
        /// updates right panel textblocks
        /// </summary>
        private void UpdateRightPanelStats()
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

        /// <summary>
        /// updates right panel images
        /// </summary>
        private void UpdateRightPanelImages()
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

        /// <summary>
        /// updates main panel textblocks and images
        /// </summary>
        private void ResetMainPanel()
        {
            //clears extra image frames when starting a new game if a game had previously been played in a session.
            if (imgDealer5.Source != null)
            {
                imgDealer5.Source = new BitmapImage();
                imgDealer4.Source = new BitmapImage();
                imgDealer3.Source = new BitmapImage();
            }

            TextBlock_GameWinner.Text = null;
            imgDealer1.Source = new BitmapImage(new Uri(@myGame.Dealer._myHand[0].Asset));
            imgDealer2.Source = new BitmapImage(new Uri(@myGame.Dealer._myHand[1].Asset));
            imgMain1.Source = new BitmapImage(new Uri(@myGame.Players[0]._myHand[0].Asset));
            imgMain2.Source = new BitmapImage(new Uri(@myGame.Players[0]._myHand[1].Asset));
        }

        /// <summary>
        /// updates top left panel textblocks
        /// </summary>
        private void UpdateTopLeftPanel()
        {
            TextBlock_GameNumberCounter.Text = "Game " + myGame.gameNumber.ToString();
            TextBlock_CurrentPlayerName.Text =  myGame.Players[0].Name.ToString();
            TextBlock_Game_Pot.Text = "Current Pot: " +  myGame._pot.ToString();
            TextBlock_Player1_CurrentBet.Text = "Current Bet: " + myGame.Players[0].Bet.ToString();
            TextBlock_Player1_Funds.Text = "Funds: " + myGame.Players[0].Funds.ToString();
            TextBlock_Player1_NetProfit.Text = "Net: " + (myGame.Players[0].Funds-myGame.Players[0].OriginalFunds).ToString();
        }

        /// <summary>
        /// Raise Textbox Input Updater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyRaise_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if(myGame != null) Int32.TryParse(myRaise.Text, out myGame._betAmountPlayerBuffer);

        } 

        #endregion


    }
}

