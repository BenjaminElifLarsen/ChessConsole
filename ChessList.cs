using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Class that contains two lists, one of white chess pieces and one of black chess pieces.
    /// </summary>
    public class ChessList
    {
        private ChessList() { }
        private static List<ChessPiece> chessListBlack = new List<ChessPiece>();
        private static List<ChessPiece> chessListWhite = new List<ChessPiece>();
        //public static void SetChessListBlack(List<ChessPiece> list)
        //{
        //    chessListBlack = list;
        //}
        /// <summary>
        /// Sets the chess pieces.
        /// </summary>
        /// <param name="list">The list containing the chess pieces.</param>
        /// <param name="team">The team that <paramref name="list"/> belongs too.</param>
        public static void SetChessList(List<ChessPiece> list, bool team)
        {
            if (team)
                chessListWhite = list;
            else
                chessListBlack = list;
        }
        /// <summary>
        /// Returns a list depending on <paramref name="team"/>.
        /// </summary>
        /// <param name="team">True for white, false for black.</param>
        /// <returns>Returns a list of either black or white chess pieces depending on <paramref name="team"/>.</returns>
        public static List<ChessPiece> GetList(bool team)
        {
            return team == false ? chessListBlack : chessListWhite;
        }
        /// <summary>
        /// Empty both player chess piece lists. 
        /// </summary>
        public static void RemoveAllPieces()
        {
            foreach (ChessPiece chePie in chessListBlack)
                chePie.RemoveSubscriptions();
            foreach (ChessPiece chePie in chessListWhite)
                chePie.RemoveSubscriptions();
            chessListBlack = null;
            chessListWhite = null;
        }
    }
}
