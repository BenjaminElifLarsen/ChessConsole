using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Contains all the game states. 
    /// </summary>
    public class GameStates
    {
        private GameStates() { }
        private static bool canMove;
        private static bool? won; //null for draw, true for win, false for lose.
        private static bool gameEnded;
        private static bool isOnline;
        private static bool whiteWon; //true for white, false for black.
        private static bool pause;
        private static byte[,] chessPieceAmounts = new byte[2, 2];
        private static short turns = -1;
        private static short turnDrawCounter;
        private static bool nonTurnWin;
        private static bool? playerTeam = null;
        private static bool lostConnection;
        private static bool isInMenu;

        /// <summary>
        /// True if it is player turn, else false.
        /// </summary>
        public static bool IsTurn { get => canMove; set => canMove = value; }
        /// <summary>
        /// Null for draw, true for victory, false for defeat.
        /// </summary>
        public static bool? VictoryType { get => won; set => won = value; } 
        /// <summary>
        /// True if the game has ended, false otherwise. 
        /// </summary>
        public static bool GameEnded { get => gameEnded; set => gameEnded = value; }
        /// <summary>
        /// If true the game is played online, else false.
        /// </summary>
        public static bool IsOnline { get => isOnline; set => isOnline = value; }
        /// <summary>
        /// True if the white player won, false otherwise.
        /// </summary>
        public static bool WhiteWin { get => whiteWon; set => whiteWon = value; }
        /// <summary>
        /// Used to "pause" the game while waiting on data from the other player.
        /// </summary>
        public static bool Pause { get => pause; set => pause = value; }
        /// <summary>
        /// The other player surrendered while it was not their turn. 
        /// </summary>
        public static bool OtherPlayerSurrendered { get => nonTurnWin; set => nonTurnWin = value; }
        /// <summary>
        /// True for white player. False for black player. Only used online. If playing offline it is set to null.
        /// </summary>
        public static bool? PlayerTeam { get => playerTeam; set => playerTeam = value; }
        /// <summary>
        /// True if the connect is lost and the game ends.
        /// </summary>
        public static bool LostConnection { get => lostConnection; set => lostConnection = value; }
        /// <summary>
        /// True if a menu is running, else false.
        /// </summary>
        public static bool IsInMenu { get => isInMenu; set => isInMenu = value; }
        /// <summary>
        /// Sets and gets the number of chespieces. [0,~] is white. [1,~] is black. [~,0] is new value. [~,1] is old value.
        /// </summary>
        public static byte[,] PieceAmount { get => chessPieceAmounts; set => chessPieceAmounts = value; }
        /// <summary>
        /// The amount of turns the game has lasted.
        /// </summary>
        public static short TurnCounter { get => turns; set => turns = value; }
        /// <summary>
        /// Amount of turns since the last capture or moved pawn.
        /// </summary>
        public static short TurnDrawCounter { get => turnDrawCounter; set => turnDrawCounter = value; }
        /// <summary>
        /// Resets all game states. 
        /// </summary>
        public static void Reset()
        {
            pause = false;
            whiteWon = false;
            isOnline = false;
            gameEnded = false;
            won = null;
            canMove = false;
            nonTurnWin = false;
            NetSearch.Abort = false;
            NetSearch.Searching = false;
            turns = -1;
            turnDrawCounter = 0;
            playerTeam = null;
        }

        public class NetSearch
        {

            private static bool searchingForPlayer;
            private static bool abort;

            /// <summary>
            /// True if searching for a player, otherwise false.
            /// </summary>
            public static bool Searching { get => searchingForPlayer; set => searchingForPlayer = value; }
            /// <summary>
            /// True if the search for other players should be aborted, else false. 
            /// </summary>
            public static bool Abort { get => abort; set => abort = value; }
        }
    }
}
