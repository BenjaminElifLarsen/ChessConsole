using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Chess
{
    /// <summary>
    /// 
    /// </summary>
    class NetworkUpdateReceiver //rename
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pub"></param>
        public NetworkUpdateReceiver(NetPublisher pub)
        {
            pub.RaiseNetEvent += NetEventHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NetEventHandler(object sender, ControlEvents.NetworkEventArgs e)
        {
            Debug.WriteLine($"Ended is {e.GameEnded.ToString()}");
            GameStates.IsTurn = e.IsTurn != null ? (bool)e.IsTurn : GameStates.IsTurn;
            GameStates.LostConnection = e.LostConnection != null ? (bool)e.LostConnection : GameStates.LostConnection;
            GameStates.Pause = e.Pause != null ? (bool)e.Pause : GameStates.Pause;
            if (e.GameEnded == true)
            {
                GameStates.WhiteWin = e.WhiteWon != null ? (bool)e.WhiteWon : GameStates.WhiteWin;
                GameStates.Won = e.Won;
                GameStates.OtherPlayerSurrendered = e.OtherPlayerSurrendered != null ? (bool)e.OtherPlayerSurrendered : GameStates.OtherPlayerSurrendered;
                GameStates.GameEnded = e.GameEnded != null ? (bool)e.GameEnded : GameStates.GameEnded;
            }
        }
    }
}
