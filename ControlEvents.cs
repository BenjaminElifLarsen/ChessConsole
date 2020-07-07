using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Event class with events related to the control of the program.
    /// </summary>
    public class ControlEvents : EventArgs
    {
        /// <summary>
        /// Class that holds event data of the input control system.
        /// </summary>
        public class KeyEventArgs
        {
            /// <summary>
            /// Base constructor for the consoleKey event data.
            /// </summary>
            /// <param name="key">The ConsoleKeyInfo to be transmitted.</param>
            public KeyEventArgs(ConsoleKeyInfo key)
            {
                Key = key;
            }
            /// <summary>
            /// Gets and sets the consoleKeyInfo key. 
            /// </summary>
            public ConsoleKeyInfo Key { get; set; }
        }

        /// <summary>
        /// Class that holds event data for capture of pieces.
        /// </summary>
        public class CaptureEventArgs
        {
            /// <summary>
            /// Base constructor for the string ID event data.
            /// </summary>
            /// <param name="ID">The ID to be transmitted.</param>
            public CaptureEventArgs(string ID)
            {
                this.ID = ID;
            }
            /// <summary>
            /// Get and sets the string ID
            /// </summary>
            public string ID { get; set; }
        }

        /// <summary>
        /// Class that holds event data of the network portion. 
        /// </summary>
        public class NetworkEventArgs
        {
            /// <summary>
            /// Base constructor for the network event data. 
            /// </summary>
            /// <param name="lostConnection">True if the connect is lost and the game ends.</param>
            /// <param name="whiteWon">True if the white player won, false otherwise.</param>
            /// <param name="otherPlayerSurrendered">True if the game has ended, false otherwise. </param>
            /// <param name="gameEnded">True if the game has ended, false otherwise. </param>
            /// <param name="pause">Used to "pause" the game while waiting on data from the other player.</param>
            /// <param name="won">Null for draw, true for victory, false for defeat.</param>
            /// <param name="isTurn">True if it is player turn, else false.</param>
            public NetworkEventArgs(bool? lostConnection = null, bool? whiteWon = null, bool? otherPlayerSurrendered = null,
                                    bool? gameEnded = null, bool? pause = null, bool? won = null, bool? isTurn = null)
            {
                LostConnection = lostConnection;
                WhiteWon = whiteWon;
                OtherPlayerSurrendered = otherPlayerSurrendered;
                GameEnded = gameEnded;
                Pause = pause;
                Won = won;
                IsTurn = isTurn;
            }
            /// <summary>
            /// True if the connect is lost and the game ends.
            /// </summary>
            public bool? LostConnection { get; set; }
            /// <summary>
            /// True if the white player won, false otherwise.
            /// </summary>
            public bool? WhiteWon { get; set; }
            /// <summary>
            /// If ture, the other player surrendered while it was not their turn. 
            /// </summary>
            public bool? OtherPlayerSurrendered { get; set; }
            /// <summary>
            /// True if the game has ended, false otherwise. 
            /// </summary>
            public bool? GameEnded { get; set; }
            /// <summary>
            /// Used to "pause" the game while waiting on data from the other player.
            /// </summary>
            public bool? Pause { get; set; }
            /// <summary>
            /// Null for draw, true for victory, false for defeat.
            /// </summary>
            public bool? Won { get; set; }
            /// <summary>
            /// True if it is player turn, else false.
            /// </summary>
            public bool? IsTurn { get; set; }
        }

    }
}
