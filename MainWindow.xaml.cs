using System.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Drawing;
using System.Windows.Media;

#region References
//https://en.wikipedia.org/wiki/Texas_hold_%27em#Strategy
#endregion

namespace TexasHoldEm
{
    public partial class MainWindow : Window
    {
        DeckOfCards myDeck;
        Player[] myPlayers;

        public MainWindow()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            myDeck = new DeckOfCards();
            myPlayers = new Player[0];
            myPlayers[0] = new Player("Me");
        }


        #region Window Methods
        #endregion

        #region Game Methods



        #endregion

        #region Probability Methods



        #endregion

        #region Menu Methods



        #endregion

    }
}

