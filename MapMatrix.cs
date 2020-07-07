using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Class that contains a 2D array that is used to keep the location of all chess pieces on the board.
    /// </summary>
    public class MapMatrix
    {
        private static string[,] map = new string[8, 8];
        private static bool mapPrepared = false;
        private static string[,] oldMap = new string[8, 8]; //for testing purposes only.
        private MapMatrix() { }

        /// <summary>
        /// Prepares the map for usages.
        /// </summary>
        public static void PrepareMap()
        {
            if (mapPrepared == false)
                for (int n = 0; n < 8; n++)
                    for (int m = 0; m < 8; m++)
                        map[n, m] = "";
            mapPrepared = true;
        }

        /// <summary>
        /// Allows the <c>PrepareMap()</c> to be called and run. 
        /// </summary>
        public static void AllowForMapPreparation()
        {
            mapPrepared = false;
        }

        /// <summary>
        /// Used to get and set values on the board.
        /// </summary>
        public static string[,] Map { get => map; set => map = value; }

        /// <summary>
        /// Updates the old version of the map with the data of the current map.
        /// </summary>
        public static void UpdateOldMap() 
        {
            for (int n = 0; n < 8; n++)
            {
                for (int m = 0; m < 8; m++)
                {
                    oldMap[n, m] = map[n, m];
                }
            }
        }

        /// <summary>
        /// Returns the map of the last move.
        /// </summary>
        public static string[,] LastMoveMap { get => oldMap; }
    }
}
