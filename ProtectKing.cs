using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Class that contain information about pieces that can protect ore are the king. 
    /// </summary>
    public class ProtectKing
    {
        private ProtectKing() { }
        private static List<string> chessListProtectKing = new List<string>();
        private static List<string> cannotMoveProtectingKing = new List<string>();
        private static Dictionary<string, List<int[,]>> chessPiecesAndEndLocations = new Dictionary<string, List<int[,]>>();
        private static Dictionary<string, List<int[,]>> cannotMovePiecesAndEndLocation = new Dictionary<string, List<int[,]>>();
        /// <summary>
        /// List of all pieces that can protect the king, if the king is checked. If the king can move, it is also in the list.
        /// </summary>
        public static List<string> Protect { get => chessListProtectKing; set => chessListProtectKing = value; }
        /// <summary>
        /// List of all pieces that cannot move as they are protecting the king from being chekced. 
        /// </summary>
        public static List<string> CannotMove { get => cannotMoveProtectingKing; set => cannotMoveProtectingKing = value; }
        /// <summary>
        /// Dictionary containing all the pieces that can prevent the king from being checked and the locations they can move too to prevent the check. If the king can move, it will also be in this list, but its value will be null.
        /// The IDs are the keys.
        /// </summary>
        public static Dictionary<string, List<int[,]>> ProtectEndLocations { get => chessPiecesAndEndLocations; set => chessPiecesAndEndLocations = value; }
        /// <summary>
        /// Will return a list of endlocations for a specific ID. If the ID is not a key, it will return null.
        /// </summary>
        /// <param name="chesspiece">The ID of the chesspiece.</param>
        /// <returns></returns>
        public static List<int[,]> GetListFromDic(string chesspiece)
        {
            try
            {
                return chessPiecesAndEndLocations[chesspiece];
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Testning
        /// </summary>
        public static Dictionary<string, List<int[,]>> ProtectingTheKing { get => cannotMovePiecesAndEndLocation; set => cannotMovePiecesAndEndLocation = value; }
        /// <summary>
        /// Will return a list of endlocations for a specific ID. If the ID is not a key, it will return null.
        /// </summary>
        /// <param name="chesspiece">The ID of the chesspiece.</param>
        /// <returns></returns>
        public static List<int[,]> GetListFromProtectingKingDic(string chesspiece)
        {
            try
            {
                return cannotMovePiecesAndEndLocation[chesspiece];
            }
            catch
            {
                return null;
            }
        }
    }
}
