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

        private static void BoardSetup()
        {
            Console.CursorVisible = false;
            ushort distance = (ushort)(9 + 8 * squareSize);
            string[] letters = { "a", "b", "c", "d", "e", "f", "g", "h" };
            string[] numbers = { "1", "2", "3", "4", "5", "6", "7", "8" };
            byte alignment = (byte)Math.Ceiling(squareSize / 2f);

            //top left corner
            Console.SetCursorPosition(offset[0] + edgeSpacing + spacing - 1, offset[1] + edgeSpacing + spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + lineBaseColour[0] + ";" + lineBaseColour[1] + ";" + lineBaseColour[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + topLeftCorner + Settings.CVTS.DEC.DEC_Deactive}");

            //top right corner 
            Console.SetCursorPosition(distance - 1 + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + lineBaseColour[0] + ";" + lineBaseColour[1] + ";" + lineBaseColour[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + topRightCorner + Settings.CVTS.DEC.DEC_Deactive}");

            //bottom left corner
            Console.SetCursorPosition(Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, distance - 1 + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + lineBaseColour[0] + ";" + lineBaseColour[1] + ";" + lineBaseColour[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + bottomLeftCorner + Settings.CVTS.DEC.DEC_Deactive}");

            //bottom right corner 
            Console.SetCursorPosition(distance - 1 + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, distance - 1 + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + lineBaseColour[0] + ";" + lineBaseColour[1] + ";" + lineBaseColour[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + bottomRightCorner + Settings.CVTS.DEC.DEC_Deactive}");


            for (int k = 1; k < distance - 1; k++) //vertical lines
                for (int i = 0; i < distance; i += 1 + squareSize)
                {
                    float intersectX = (float)Math.Floor(i / (double)(squareSize + spacing));
                    float intersectY = k % (float)(squareSize + spacing);
                    Console.SetCursorPosition(i + offset[0] + edgeSpacing + spacing - 1, k + offset[1] + edgeSpacing + spacing - 1);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + lineBaseColour[0] + ";" + lineBaseColour[1] + ";" + lineBaseColour[2] + "m "); //background colour
                    Console.SetCursorPosition(i + offset[0] + edgeSpacing + spacing - 1, k + offset[1] + edgeSpacing +spacing - 1);
                    if (intersectY != 0) //no intersection at all
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + verticalLine + Settings.CVTS.DEC.DEC_Deactive}");
                    else if (intersectY == 0 && intersectX == 0) //intersection on the left side
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + intersectionLeft + Settings.CVTS.DEC.DEC_Deactive}");
                    else //intersection at the right side. 
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + intersectionRight + Settings.CVTS.DEC.DEC_Deactive}");
                }

            for (int k = 0; k < distance; k += 1 + squareSize) //horizontal lines
                for (int i = 1; i < distance - 1; i++)
                {
                    float intersectX = i % (float)(squareSize + spacing);
                    float intersectY = (float)Math.Floor(k / (double)(squareSize + spacing));
                    Console.SetCursorPosition(i + offset[0] + edgeSpacing + spacing - 1, k + offset[1] + edgeSpacing + spacing - 1);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + lineBaseColour[0] + ";" + lineBaseColour[1] + ";" + lineBaseColour[2] + "m "); //background colour
                    Console.SetCursorPosition(i + offset[0] + edgeSpacing + spacing - 1, k + offset[1] + edgeSpacing + spacing - 1);
                    if (intersectX != 0) //no intersection at all
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + horizontalLine + Settings.CVTS.DEC.DEC_Deactive}");
                    else if (intersectX == 0 && intersectY == 0) //intersection at the top
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + intersectionTop + Settings.CVTS.DEC.DEC_Deactive}");
                    else if (intersectX == 0 && intersectY == 8) //intersection at the bottom
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + intersectionBottom+ Settings.CVTS.DEC.DEC_Deactive}");
                    else //intersection all other places.
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + intersectionFull+ Settings.CVTS.DEC.DEC_Deactive}");
                }

            for (int k = 0; k < numbers.Length; k++) //numbers
            { //the 1s in this and below for loop should be a setting, it is the amount of empty squares between the numbers/letters and the board edge
                Console.SetCursorPosition(Settings.Offset[0] - Settings.EdgeSpacing - 1 + Settings.Spacing, k + Settings.EdgeSpacing + Settings.Spacing + Settings.Offset[1] + alignment + (Settings.SquareSize * k) - 1);
                Console.Write(numbers[7 - k]); //- Settings.EdgeSpacing should not be used in the calculation above.
                Console.SetCursorPosition(Settings.Offset[0] + distance + Settings.EdgeSpacing + Settings.Spacing, k + Settings.Spacing + Settings.EdgeSpacing + Settings.Offset[1] + alignment + (Settings.SquareSize * k) - 1);
                Console.Write(numbers[7 - k]);
            }

            for (int k = 0; k < letters.Length; k++) //letters
            {
                Console.SetCursorPosition(k + Settings.Spacing + Settings.Offset[0] + alignment + (Settings.SquareSize * k), Settings.Offset[1] + Settings.Spacing - Settings.EdgeSpacing - 1);
                Console.Write(letters[k]);
                Console.SetCursorPosition(k + Settings.Spacing + Settings.Offset[0] + alignment + (Settings.SquareSize * k), Settings.Offset[1] + distance + Settings.Spacing + Settings.EdgeSpacing);
                Console.Write(letters[k]);
            }

            BoardColouring();
        }

        private static void BoardColouring()
        {
            ushort distance = (ushort)(8 + 8 * Settings.SquareSize);
            byte location = 1;
            for (int n = 0; n < distance; n += 1 + Settings.SquareSize)
            {
                for (int m = 0; m < distance; m += 1 + Settings.SquareSize)
                {
                    for (int i = 0; i < Settings.SquareSize; i++)
                    {
                        for (int k = 0; k < Settings.SquareSize; k++)
                        {
                            Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing + n, k + Settings.Offset[1] + Settings.Spacing + Settings.EdgeSpacing + m);
                            if (location % 2 == 1)
                                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.SquareColour1[0] + ";" + Settings.SquareColour1[1] + ";" + Settings.SquareColour1[2] + "m " + Settings.CVTS.Reset);
                            else if (location % 2 == 0)
                                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.SquareColour2[0] + ";" + Settings.SquareColour2[1] + ";" + Settings.SquareColour2[2] + "m " + Settings.CVTS.Reset);
                        }
                    }
                    location++;
                }
                location++;
            }
        }

        private static class Designs //neeeds to contain information about the most important part of each CMD design. 
        {
            //bishop
            /// <summary> //make all these readonly
            /// 
            /// </summary>
            private static string[] bishopCmdDesign =
            {
                $"_{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Plus_Minus}{Settings.CVTS.DEC.DEC_Deactive}_",
                $"{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Vertical_Line}{Settings.CVTS.DEC.DEC_Deactive}O{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Vertical_Line}{Settings.CVTS.DEC.DEC_Deactive}",
                $"-B-"
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
                " - ",
                " | ",
                "-P-"
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
                "_w_",
                "~|~",
                "-Q-"
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
                "^V^",
                "*|*",
                "-K-"
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
                "^^^",
                "|=|",
                "-R-"
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
                " ^_",
                " |>",
                "-k-"
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
