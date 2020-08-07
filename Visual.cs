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
        //    adding/changing the visual is done. 
        //store static variables for all colours and designs used for the board

        private static readonly byte squareSize = Settings.SquareSize; //if the square size is changed this will need to be updated. 
        private static readonly byte spacing = Settings.Spacing;
        private static readonly byte edgeSpacing = Settings.EdgeSpacing;
        private static readonly int[] consoleSize = Settings.WindowSize;
        private static readonly byte[] offset = Settings.Offset;

        private static readonly byte[] whiteBackgroundColour = Settings.SquareColour1;
        private static readonly byte[] blackBackgroundColour = Settings.SquareColour2;
        private static readonly byte[] whiteColour = Settings.WhiteColour;
        private static readonly byte[] blackColour = Settings.BlackColour;
        private static readonly byte[] lineColour = Settings.LineColour;
        private static readonly byte[] lineBaseColour = Settings.LineColourBase;
        private static readonly byte[] selectSquareColour = Settings.SelectSquareColour;
        private static readonly byte[] selectPieceColour = Settings.SelectPieceColour;
        private static readonly byte[] selectMoveSquareColour = Settings.SelectMoveSquareColour;

        private static readonly string topLeftCorner = Settings.CVTS.DEC.DEC_Corner_TopLeft;
        private static readonly string topRightCorner = Settings.CVTS.DEC.DEC_Corner_TopRight;
        private static readonly string bottomLeftCorner = Settings.CVTS.DEC.DEC_Corner_BottomLeft;
        private static readonly string bottomRightCorner = Settings.CVTS.DEC.DEC_Corner_BottomRight;
        private static readonly string horizontalLine = Settings.CVTS.DEC.DEC_Horizontal_Line;
        private static readonly string verticalLine = Settings.CVTS.DEC.DEC_Vertical_Line;
        private static readonly string intersectionFull = Settings.CVTS.DEC.DEC_Intersection_Full;
        private static readonly string intersectionRight = Settings.CVTS.DEC.DEC_Intersection_Right;
        private static readonly string intersectionLeft = Settings.CVTS.DEC.DEC_Intersection_Left;
        private static readonly string intersectionTop = Settings.CVTS.DEC.DEC_Intersection_Top;
        private static readonly string intersectionBottom = Settings.CVTS.DEC.DEC_Intersection_Bottom;

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
            int[] location = new int[2] { mapLocation[0] * squareSize + (mapLocation[0] + edgeSpacing + spacing) * 1 + offset[0], mapLocation[1] *squareSize + (mapLocation[1] + edgeSpacing + spacing) * 1 + offset[1] };
            int designSize = design.Length;
            drawLocationX = location[0] + (int)(squareSize - designSize) / 2;
            drawLocationY = location[1] + (int)(squareSize - designSize) / 2;
            int locationForColour = (mapLoc[0] + mapLoc[1]) % 2; //if zero, background colour is "white", else background colour is "black".
            byte[] colours = locationForColour == 0 ? whiteBackgroundColour : blackBackgroundColour;
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
            /// <summary> //make all these readonly
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

            //pawn
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

            //queen
            //--------------------------------

            /// <summary>
            /// 
            /// </summary>
            private static string[] queenCMDDesign = 
            { 
            
            };
            /// <summary>
            /// 
            /// </summary>
            private static PointF[] queenGUIDesign =
            {

            };
            /// <summary>
            /// 
            /// </summary>
            public static string[] CMD_Queen { get => queenCMDDesign; }
            /// <summary>
            /// 
            /// </summary>
            public static PointF[] GUI_Queen { get => queenGUIDesign; }

            //king
            //--------------------------------

            /// <summary>
            /// 
            /// </summary>
            private static string[] kingCMDDesign =
            {

            };
            /// <summary>
            /// 
            /// </summary>
            private static PointF[] kingGUIDesign =
            {

            };
            /// <summary>
            /// 
            /// </summary>
            public static string[] CMD_King { get => kingCMDDesign; }
            /// <summary>
            /// 
            /// </summary>
            public static PointF[] GUI_King { get => kingGUIDesign; }

            //rook
            //--------------------------------

            /// <summary>
            /// 
            /// </summary>
            private static string[] rookCMDDesign =
            {

            };
            /// <summary>
            /// 
            /// </summary>
            private static PointF[] rookGUIDesign =
            {

            };
            /// <summary>
            /// 
            /// </summary>
            public static string[] CMD_Rook { get => rookCMDDesign; }
            /// <summary>
            /// 
            /// </summary>
            public static PointF[] GUI_Rook { get => rookGUIDesign; }

            //knight
            //--------------------------------

            /// <summary>
            /// 
            /// </summary>
            private static string[] knightCMDDesign =
            {

            };

            /// <summary>
            /// 
            /// </summary>
            private static PointF[] knightGUIDesign =
            {

            };
            /// <summary>
            /// 
            /// </summary>
            public static string[] CMD_Knight { get => knightCMDDesign; }
            /// <summary>
            /// 
            /// </summary>
            public static PointF[] GUI_Knight { get => knightGUIDesign; }

        }
    }
}
