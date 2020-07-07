using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Class contains events, delegates and functions related to the network portion of the program.
    /// </summary>
    public class NetPublisher
    {
        public delegate void netEventHandler(object sender, ControlEvents.NetworkEventArgs args);

        public event netEventHandler RaiseNetEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lostConnection"></param>
        /// <param name="whiteWon"></param>
        /// <param name="otherPlayerSurrendered"></param>
        /// <param name="gameEnded"></param>
        /// <param name="pause"></param>
        /// <param name="won"></param>
        /// <param name="isTurn"></param>
        public void TransmitAnyData(bool? lostConnection = null, bool? whiteWon = null, bool? otherPlayerSurrendered = null,
                                    bool? gameEnded = null, bool? pause = null, bool? won = null, bool? isTurn = null)
        {
            OnNetWorkChange(new ControlEvents.NetworkEventArgs(lostConnection, whiteWon, otherPlayerSurrendered, gameEnded, pause, won, isTurn));
        }
        /// <summary>
        /// Used to inform all subscribers of whether the game has ended or not. True if the gamed has ended else false.
        /// Invokes an event with a specific data set to <paramref name="gameHasEnded"/>.
        /// </summary>
        /// <param name="gameHasEnded">Null if it should be ignored in the invokement, true if the game has ended else false.</param>
        public void GameEnded(bool? gameHasEnded)
        {
            OnNetWorkChange(new ControlEvents.NetworkEventArgs(gameEnded: gameHasEnded));
        }
        /// <summary>
        /// Used to inform all subscribes of the connection status of the program. True if the connection is lost. 
        /// Invokes an event with a specific data set to <paramref name="connectionLost"/>.
        /// </summary>
        /// <param name="connectionLost">Null if it should be ignored in the invokement, true if the connection is lost, false if the connection is still present.</param>
        public void LostConnection(bool? connectionLost)
        {
            OnNetWorkChange(new ControlEvents.NetworkEventArgs(lostConnection: connectionLost));
        }
        /// <summary>
        /// Used to inform all subscribers of the game end state. Null for draw, false for defeat and true for victory.
        /// Invokes an event with a specific data set to <paramref name="hasWon"/>.
        /// </summary>
        /// <param name="hasWon">Null if the game ended in a draw, true if the player has won, false if the player has lost.</param>
        public void Won(bool? hasWon)
        {
            OnNetWorkChange(new ControlEvents.NetworkEventArgs(won: hasWon));
        }
        /// <summary>
        /// Used to inform all subscribers of if the game is paused or not. True if the game is paused, false if it is not. 
        /// Invokes an event with a specific dta set to <paramref name="isPaused"/>.
        /// </summary>
        /// <param name="isPaused">Null if it should be ignored in the invokement, true if the game is paused, false if it is not.</param>
        public void Pause(bool? isPaused)
        {
            OnNetWorkChange(new ControlEvents.NetworkEventArgs(pause: isPaused));
        }
        /// <summary>
        /// Used to inform all subscribers of whether the other player surrendered or not. True if they did else false.
        /// Invokes an event with a specific data set to <paramref name="didOtherPlayerSurrender"/>.
        /// </summary>
        /// <param name="didOtherPlayerSurrender">Null if it should be ignored in the invokement, true if the other player ended the game else, false if they did not.</param>
        public void OtherPlayerSurrendered(bool? didOtherPlayerSurrender)
        {
            OnNetWorkChange(new ControlEvents.NetworkEventArgs(otherPlayerSurrendered: didOtherPlayerSurrender));
        }
        /// <summary>
        /// Used to inform all subscribers on whether it is the player's turn or not. True if it is else false.
        /// Invoke an event with a specific data set to <paramref name="isTurn"/>.
        /// </summary>
        /// <param name="isTurn">Null if it should be ignored in the invokement, true if its the player's turn else false.</param>
        public void IsTurn(bool? isTurn)
        {
            OnNetWorkChange(new ControlEvents.NetworkEventArgs(isTurn: isTurn));
        }
        /// <summary>
        /// Used to inform all subscribers on whether white player won or not. True if they did else false.
        /// Invoke an event with a specific data set <paramref name="didWhiteWin"/>.
        /// </summary>
        /// <param name="didWhiteWin">Null if it should be ignored in the invokement, true if white won else false.</param>
        public void WhiteWon(bool? didWhiteWin)
        {
            OnNetWorkChange(new ControlEvents.NetworkEventArgs(whiteWon: didWhiteWin));
        }

        /// <summary>
        /// Invokes an event with the data of <paramref name="e"/>, if there are any subscribers.
        /// </summary>
        /// <param name="e">ControlEvents.NetworkEventArgs event.</param>
        protected virtual void OnNetWorkChange(ControlEvents.NetworkEventArgs e)
        {
            netEventHandler eventHandler = RaiseNetEvent;
            if (eventHandler != null)
                eventHandler.Invoke(this, e);
        }
    }
}
