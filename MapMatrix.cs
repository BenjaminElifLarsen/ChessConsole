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
        //all test variables and functions should be removed when the NetworkSupport and Network are fully working and well tested. 
        private static string[,] map = new string[8, 8];
        private static bool mapPrepared = false;
        private static string[,] testMap = new string[8, 8]; //for testing purposes only.
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
        public static void UpdateOldMap() //for testing only. 
        {
            for (int n = 0; n < 8; n++)
            {
                for (int m = 0; m < 8; m++)
                {
                    testMap[n, m] = map[n, m];
                }
            }
            //return testMap;
        }
        public static string[,] LastMoveMap { get => testMap; } //for testing only. 
    }
}
