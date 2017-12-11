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
    
    //TODO: add changable cardbacks with svg file support

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
            //TODO: collect data from xml instead of here
            string[] names = { "Jason", "Annie","Oswald" , "Cooper"};
            int funds = 1000;
            int minBet = 10;
            int maxBet = 100;
            bool noLimits = false;
            ///TODO: error when numcards is not set to 5. Must fix
            myGame = new PokerGame(names, 5, funds, noLimits, minBet, maxBet);
            myAI = new TexasAI(myGame);
            UpdateTopLeftPanel();
            UpdateRightPanelStats();
            UpdateTopPanel();
        }

        #region state machine (changes _stage property of Game class)

        /// <summary>
        /// sets stage to hold, updates myGame
        /// </summary>
        private void Button_Click_Fold(object sender, RoutedEventArgs e)
        {
            if ((myGame.Stage == "Raise or Match or Fold") || (myGame.Stage == "Raise or Hold or Fold"))
            {
                int k = 0;
                foreach (Player player in myGame._players)
                {
                    if (player.Playing) { k++; }
                }
                myGame.TakeTurn("Fold");
                if (k == 1)
                {
                    myGame.Stage = "End";
                    myGame.EndGame();
                    UpdateEndGameUI();
                }
                else
                {
                    myGame.Stage = "Fold";
                    UpdateTopLeftPanel();
                    WaitForTexasAI();
                }
            }
        }

        /// <summary>
        /// sets stage to hold, updates myGame
        /// </summary>
        private void Button_Click_Hold(object sender, RoutedEventArgs e)
        {
            if (myGame.Stage == "Raise or Hold or Fold")
            {
                myGame.TakeTurn();
                myGame.Stage = "Hold";
                WaitForTexasAI();
            }
        }

        /// <summary>
        /// sets stage to match or raiseorholdorfold if player is the last to match, updates myGame
        /// </summary>
        private void Button_Click_Match(object sender, RoutedEventArgs e)
        {
            if (myGame.Stage == "Raise or Match or Fold")
            {
                //Triggers when all players have responded to a raise.
                myGame.TakeTurn("Match");
                myGame.Stage = "Match";
                UpdateTopLeftPanel();
                WaitForTexasAI();
            }
        }

        /// <summary>
        /// Sets stage to raise, updates myGame
        /// </summary>
        private void Button_Click_Raise(object sender, RoutedEventArgs e)
        {
            if (((myGame.Stage == "Raise or Hold or Fold")||(myGame.Stage == "Raise or Match or Fold"))&&(!myGame._players[0].RoseBetThisTurn))
            {
                myGame._players[0].RoseBetThisTurn = true;

                //Checks if raise is appropriate.
                if ((myGame._betAmountPlayerBuffer < myGame._minBet) || (myGame._betAmountPlayerBuffer > myGame._maxBet) || (myGame._betAmountPlayerBuffer > myGame._players[0].Funds))
                {
                    UpdateLeftPanel("Dealer won't accept your bet of " + myGame._betAmountPlayerBuffer);
                    return;
                }
                myGame.TakeTurn("Raise", 0, myGame._betAmountPlayerBuffer);
                myGame.Stage = "Raise";
                UpdateTopLeftPanel();
                WaitForTexasAI();
            }
        }

        /// <summary>
        /// Reset's the game UI and sets the stage to its default.
        /// </summary>
        private void Button_Click_NewHand(object sender, RoutedEventArgs e)
        {
            //generates a new hand and populates image frames.
            if ((myGame.Stage == "End") || (myGame.Stage == null))
            {
                myGame.TakeTurn("New Hand");
                myGame.Stage = "Raise or Hold or Fold";
                myAI.ResetAI();
                UpdateNewGameUI();

                //Start game
                WaitForTexasAI();
                UpdateTopLeftPanel();
                UpdateRightPanelStats();
            }
        }

        /// <summary>
        /// Allows AI to take it's turn and determine's the next state for the player
        /// </summary>
        private void WaitForTexasAI()
        {
            if (myGame.Stage != "End") myGame.Stage  = myAI.ExternalStateMachine(myGame.Stage, myGame);

            if (myGame.Stage == "End")
            {
                myGame.EndGame();
                UpdateEndGameUI();
            }

            else if (myGame.Stage == "Draw")
            {
                UpdateTopLeftPanel();
                myGame._players[0].RoseBetThisTurn = false;
                myGame.Stage = "Raise or Hold or Fold";
                DealerDrawNext();
                if (myGame.Stage == "End") return;
                WaitForTexasAI();
            }

            else if (myGame.Stage == "Draw, player goes first")
            {
                UpdateTopLeftPanel();
                myGame._players[0].RoseBetThisTurn = false;
                myGame.Stage = "Raise or Hold or Fold";
                DealerDrawNext();
            }
            else if (myGame.Stage == "Raise or Match or Fold")
            {
                UpdateTopLeftPanel();
                UpdateLeftPanel("AI RAISED");
            }

            else if (myGame.Stage != "Raise or Hold or Fold") throw new Exception("Oops, how did you get here!");

            //Update GUI
            UpdateTopPanel();

            //AI must continue if player stops playing.
            if (!myGame._players[0].Playing)
            {
                //waits until all dealer cards are drawn or all but one players have folded.
                int k = 0;
                foreach(Player player in myGame._players)
                {
                        if (player.Playing) { k++; }
                }
                if ((k!=0)&&(myGame._dealer.HandIndex<=4))
                {
                    WaitForTexasAI();
                }
            }
        }

        /// <summary>
        /// New: Now only updates the UI and triggers the end of a game without a fold victory. Dealer's card is already drawn.
        /// </summary>
        private void DealerDrawNext()
        {
            Console.WriteLine("A new card was drawn.");
            switch (myGame._dealer.HandIndex)
            {
                //the turn
                case 4:
                    imgDealer4.Source = new BitmapImage(new Uri(@myGame._dealer._myHand[3].Asset));
                    break;

                //the river
                case 5:
                    imgDealer5.Source = new BitmapImage(new Uri(@myGame._dealer._myHand[4].Asset));
                    myGame.Stage = "End";
                    myGame.EndGame();
                    UpdateEndGameUI();
                    break;
                
                //the flop
                default:
                    imgDealer1.Source = new BitmapImage(new Uri(@myGame._dealer._myHand[0].Asset));
                    imgDealer2.Source = new BitmapImage(new Uri(@myGame._dealer._myHand[1].Asset));
                    imgDealer3.Source = new BitmapImage(new Uri(@myGame._dealer._myHand[2].Asset));
                    break;
            }
        }

        #endregion

        #region Menu Methods (changes XAML GUI or reads from it)

        private void UpdateTopPanel()
        {
            TextBlock_MinBetMaxBet.Text = "Minimum Bet: " + myGame._minBet.ToString() + " Maximum Bet: " + myGame._maxBet.ToString();
            TextBlock_Stage.Text = myGame.Stage;
        }

        /// <summary>
        /// Output's each players scores along with the game number at end of game
        /// </summary>
        private void PrintScores()
        {
            string buffer = "Game: " + myGame._gameNumber.ToString() + "\n";
            for (int i = 0; i < myGame._players.Length; i++)
            {
                buffer += myGame._players[i].Name.ToString() + "'s Score: " + myGame._players[i].Score.ToString() + "\n";
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
            topBuffer += bottomBuffer;
            TextBox_ScrollViewer_LeftSide.Text = topBuffer;
        }

        /// <summary>
        /// updates right panel textblocks
        /// </summary>
        private void UpdateRightPanelStats()
        {
            TextBlock_Player2_Name.Text = myGame._players[1].Name.ToString();
            TextBlock_Player2_Funds.Text = "Funds: " + myGame._players[1].Funds.ToString();
            TextBlock_Player3_Name.Text = null;
            TextBlock_Player3_Funds.Text = null;
            TextBlock_Player4_Name.Text = null;
            TextBlock_Player4_Funds.Text = null;
            if (myGame._players.Length > 2)
            {
                TextBlock_Player3_Funds.Text = "Funds: " + myGame._players[2].Funds.ToString();
                TextBlock_Player3_Name.Text = myGame._players[2].Name.ToString();
            }
            if (myGame._players.Length > 3)
            {
                TextBlock_Player4_Funds.Text = "Funds: " + myGame._players[3].Funds.ToString();
                TextBlock_Player4_Name.Text = myGame._players[3].Name.ToString();
            }
            if (myGame._players.Length > 4)
            {
                throw new Exception("Only supports 4 players");
            }
        }

        //TODO: make dynamic scrollable grid based on number of players in a session.

        /// <summary>
        /// updates right panel images
        /// </summary>
        private void UpdateRightPanelImagesHidden()
        {
            //only change AI hands, not the players
            
            if (myGame._players.Length > 1)
            {
                imgONE1.Source = new BitmapImage(new Uri(@myGame._cardBackMain));
                imgONE2.Source = new BitmapImage(new Uri(@myGame._cardBackMain));
            }
            else
            {
                //TODO: merge with above method, or separte them as done previously.
                //TODO: Win screen update
                TextBlock_GameWinner.Text = "You beat all the pther players!";
                TextBlock_Player2_status.Text = null;
            }
            if (myGame._players.Length > 2)
            {
                imgTWO1.Source = new BitmapImage(new Uri(@myGame._cardBackMain));
                imgTWO2.Source = new BitmapImage(new Uri(@myGame._cardBackMain));
            }
            else
            {
                imgTWO1.Source = new BitmapImage(new Uri(@myGame._cardBackGameOver));
                imgTWO2.Source = new BitmapImage(new Uri(@myGame._cardBackGameOver));
                TextBlock_Player3_status.Text = null;
            }
            if (myGame._players.Length > 3)
            {
                imgTHREE1.Source = new BitmapImage(new Uri(@myGame._cardBackMain));
                imgTHREE2.Source = new BitmapImage(new Uri(@myGame._cardBackMain));
            }
            else
            {
                imgTHREE1.Source = new BitmapImage(new Uri(@myGame._cardBackGameOver));
                imgTHREE2.Source = new BitmapImage(new Uri(@myGame._cardBackGameOver));
                TextBlock_Player4_status.Text = null;
            }
            if (myGame._players.Length > 4)
            {
                throw new Exception("Only supports 4 players");
            }
        }
        private void UpdateRightPanelImagesShown()
        {
            //only change AI hands, not the players

            if ((myGame._players.Length > 1) && (myGame._players[1]._myHand!=null))
            {
                imgONE1.Source = new BitmapImage(new Uri(@myGame._players[1]._myHand[0].Asset));
                imgONE2.Source = new BitmapImage(new Uri(@myGame._players[1]._myHand[1].Asset));
            }
            else if (myGame._players.Length > 1)
            {
                imgONE1.Source = new BitmapImage(new Uri(@myGame._cardBackFold));
                imgONE2.Source = new BitmapImage(new Uri(@myGame._cardBackFold));
            }
            if ((myGame._players.Length > 2) && (myGame._players[2]._myHand != null))
            {
                imgTWO1.Source = new BitmapImage(new Uri(@myGame._players[2]._myHand[0].Asset));
                imgTWO2.Source = new BitmapImage(new Uri(@myGame._players[2]._myHand[1].Asset));
            }
            else if (myGame._players.Length > 2)
            {
                imgTWO1.Source = new BitmapImage(new Uri(@myGame._cardBackFold));
                imgTWO2.Source = new BitmapImage(new Uri(@myGame._cardBackFold));
            }
            if ((myGame._players.Length > 3) && (myGame._players[3]._myHand != null))
            {
                 imgTHREE1.Source = new BitmapImage(new Uri(@myGame._players[3]._myHand[0].Asset));
                 imgTHREE2.Source = new BitmapImage(new Uri(@myGame._players[3]._myHand[1].Asset));
            }
            else if (myGame._players.Length > 3)
            {
                imgTHREE1.Source = new BitmapImage(new Uri(@myGame._cardBackFold));
                imgTHREE2.Source = new BitmapImage(new Uri(@myGame._cardBackFold));
            }
            if (myGame._players.Length > 4)
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
            if (imgDealer1.Source != null)
            {
                imgDealer5.Source = new BitmapImage();
                imgDealer4.Source = new BitmapImage();
                imgDealer3.Source = new BitmapImage();
                imgDealer2.Source = new BitmapImage();
                imgDealer1.Source = new BitmapImage();
            }

            TextBlock_GameWinner.Text = "Texas Hold'em";
            imgMain1.Source = new BitmapImage(new Uri(@myGame._players[0]._myHand[0].Asset));
            imgMain2.Source = new BitmapImage(new Uri(@myGame._players[0]._myHand[1].Asset));
        }

        /// <summary>
        /// updates top left panel textblocks
        /// </summary>
        private void UpdateTopLeftPanel()
        {
            if (myGame._gameNumber < 1)
            {
                TextBlock_GameNumberCounter.Text = "Welcome";
            }
            else
            {
                TextBlock_GameNumberCounter.Text = "Game " + myGame._gameNumber.ToString();
            }
            TextBlock_totalBetAmount.Text = (myGame._totalBetAmount - myGame._players[0].Bet).ToString();
            TextBlock_CurrentPlayerName.Text =  myGame._players[0].Name.ToString();
            TextBlock_Game_Pot.Text = "Current Pot: " +  myGame._pot.ToString();
            TextBlock_Player1_CurrentBet.Text = "Current Bet: " + myGame._players[0].Bet.ToString();
            TextBlock_Player1_Funds.Text = "Funds: " + myGame._players[0].Funds.ToString();
            TextBlock_Player1_NetProfit.Text = "Net: " + (myGame._players[0].Funds-myGame._players[0].OriginalFunds).ToString();
        }

        /// <summary>
        /// Updates the UI when the game ends
        /// </summary>
        private void UpdateEndGameUI()
        {
            PrintScores();
            UpdateTopLeftPanel();
            UpdateRightPanelStats();
            UpdatePlayerStatuses();
            UpdateTopPanel();
            UpdateRightPanelImagesShown();

            //unique endgame updates
            TextBlock_GameWinner.Text = myGame._winner;
        }

        private void UpdatePlayerStatuses()
        {
            TextBlock_Player4_status.Text = "Not Implemented";
            TextBlock_Player3_status.Text = "Not Implemented";
            TextBlock_Player2_status.Text = "Not Implemented";
        }
        /// <summary>
        /// Updates the UI when the game starts
        /// </summary>
        private void UpdateNewGameUI()
        {
            MyRaise_TextChanged(this, null);
            ResetMainPanel();
            UpdateTopLeftPanel();
            UpdatePlayerStatuses();
            UpdateRightPanelImagesHidden();
            UpdateTopPanel();
        }

        /// <summary>
        /// Player's bet counter which is only processed when raise is clicked but is changed whenever the texbox is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyRaise_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if(myGame != null) Int32.TryParse(myRaise.Text, out myGame._betAmountPlayerBuffer);

        }


        #endregion

        //Extra, not yet categorized

        private void Button_Click(object sender, RoutedEventArgs e)
        {
             Console.WriteLine(myAI.ToString() + myGame.ToString());
        }
    }
}

