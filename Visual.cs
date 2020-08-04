using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Net;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Class that contains the methods for the visual part of the program.
    /// </summary>
    public static class Visual
    {
        //how to do this...
        //should this class hold the designs?
        //should "paint" to the console and to a graphical user interfaces
        //should the GUI use signs for the visual representation or should it use polygons(PointF[])/bitmaps?
        //for the GUI keep the map board, while also having an Image board. There should be a readonly bitmap board that can be copied from which
            //adding/changing the visual is done. 
        //store static variables for all colours and designs used for the board

        /// <summary>
        /// 
        /// </summary>
        static public void ChessBoardPaint()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        static public void SquareBackground()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        static public void SquareForeground()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        static public void SquareFull()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        static public void SquareHighLight()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        static public void RemoveDraw()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawLocationX"></param>
        /// <param name="drawLocationY"></param>
        /// <param name="mapLoc"></param>
        /// <param name="design"></param>
        /// <param name="mapLocation"></param>
        /// <returns></returns>
        static private byte[] PaintCalculations(out int drawLocationX, out int drawLocationY, int[] mapLoc, string[] design, byte[] mapLocation)
        {
            int[] location = new int[2] { mapLocation[0] * Settings.SquareSize + (mapLocation[0] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[0], mapLocation[1] * Settings.SquareSize + (mapLocation[1] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[1] };
            int designSize = design.Length;
            drawLocationX = location[0] + (int)(Settings.SquareSize - designSize) / 2;
            drawLocationY = location[1] + (int)(Settings.SquareSize - designSize) / 2;
            int locationForColour = (mapLoc[0] + mapLoc[1]) % 2; //if zero, background colour is "white", else background colour is "black".
            byte[] colours = locationForColour == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
            return colours;
        }

        static private byte[] PaintCalculations(out int drawLocationX, out int drawLocationY, int[] mapLoc, PointF[] design, byte[] mapLocation)
        {
            drawLocationX = 0;
            drawLocationY = 0;
            return null;
        }

        private static class Designs
        {
            //bishop
            /// <summary>
            /// 
            /// </summary>
            private static string[] bishopCmdDesign =
            {

            };
            /// <summary>
            /// 
            /// </summary>
            private static PointF[] bishopGUIDesign =
            { //might be best to contain all designs in this class rather than the different chess piece classes to prevent having to change them when changing their design.

            };
            /// <summary>
            /// 
            /// </summary>
            public static string[] CMD_Bishop { get => bishopCmdDesign; }
            /// <summary>
            /// 
            /// </summary>
            public static PointF[] GUI_Bishop { get => bishopGUIDesign; }

            //--------------------------------

            /// <summary>
            /// 
            /// </summary>
            private static string[] pawnCMDDesign =
            {

            };
            /// <summary>
            /// 
            /// </summary>
            private static PointF[] pawnGUIDesign =
            {

            };
            /// <summary>
            /// 
            /// </summary>
            public static string[] CMD_Pawn { get => pawnCMDDesign; }
            /// <summary>
            /// 
            /// </summary>
            public static PointF[] GUI_Pawn { get => pawnGUIDesign; }

        }
    }
}
