using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Class that contains the methods for the visual part of the program.
    /// </summary>
    public static class Visual //needs the command promp chess piece design resizer. 
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
        /// if the design is bigger than the square, part of the design will be removed so it fits the square. 
        /// </summary>
        static private string[] DesignResizer(string[] design, byte[] mostImportantDesignPart)
        {
            if (Settings.ChesspieceDesignSize > squareSize)
            {
                string[] resizedDesign = new string[squareSize];

                if (resizedDesign.Length > 1)
                {
                    byte belowStringAmount = (byte)(design.Length - (mostImportantDesignPart[1] + 1)); //the amount of strings below
                    byte aboveStringAmount = (byte)(design.Length - belowStringAmount - 1); //the -1 is the most important string
                    sbyte direction = belowStringAmount >= aboveStringAmount ? (sbyte)1 : (sbyte)-1;
                    byte stringAmount = belowStringAmount >= aboveStringAmount ? belowStringAmount : aboveStringAmount;

                    byte startLocation = (byte)(mostImportantDesignPart[1]);
                    byte endLocation = direction < 0 ? (byte)(startLocation - resizedDesign.Length) : (byte)(startLocation + resizedDesign.Length);
                    byte lengthDifference = (byte)(design.Length - resizedDesign.Length);
                    byte indexSubtraction = startLocation < stringAmount ? (byte)0 : lengthDifference;
                    for (int i = startLocation; i != endLocation; i += direction)
                    {
                        char[] designImportant = design[i].ToCharArray();
                        resizedDesign[i - indexSubtraction] = DesignCreate(designImportant, (byte)design.Length);
                    }
                }
                else
                {
                    string importantString = design[mostImportantDesignPart[1]];
                    char[] importantChars = importantString.ToCharArray();
                    char[] importantChar = new char[] { importantChars[mostImportantDesignPart[0]] };
                    resizedDesign[0] = new string(importantChar);
                }
                return resizedDesign;

                string DesignCreate(char[] charArray, byte charLength)
                {
                    bool?[] DECLocation = new bool?[charLength];
                    bool?[] DECSymbolUsed = new bool?[charLength];
                    int charDECLength = 0;
                    if (charLength < charArray.Length) //DEC is present 
                    {
                        charDECLength = charArray.Length;
                        byte posistion = 0;
                        char[] nonDECArray = new char[charLength];
                        for (int m = 0; m < charArray.Length; m++)
                        {
                            if (charArray[m] == '\u001b') //DEC is either activated or deactivated
                            {
                                if (charArray[m + 2] == '0') //DEC is activated 
                                {
                                    DECLocation[posistion] = true;
                                }
                                m += 2; //jumps to the end of the DEC char sequence. 
                            }
                            else
                            {
                                nonDECArray[posistion] = charArray[m];
                                posistion++;
                            }
                        }
                        charArray = nonDECArray;
                    }

                    char[] newDesign = new char[squareSize];
                    byte mostImportLocation = mostImportantDesignPart[0] < squareSize ? mostImportantDesignPart[0] : (byte)(newDesign.Length - 1);
                    newDesign[mostImportLocation] = charArray[mostImportantDesignPart[0]];
                    byte rightAmountOfChars = (byte)(charLength - (mostImportantDesignPart[0] + 1)); //the +1 is to make up with the difference in array length and indexes.
                    byte leftAmountOfChars = (byte)(charLength - 1 - rightAmountOfChars); //the -1 is to subtract the important char. 
                    sbyte charDirection = rightAmountOfChars >= leftAmountOfChars ? (sbyte)1 : (sbyte)-1; //right : left
                    byte charAmount = rightAmountOfChars >= leftAmountOfChars ? rightAmountOfChars : leftAmountOfChars;
                    byte startCharLocation = (byte)(mostImportantDesignPart[0]);
                    byte endCharLoction = charDirection < 0 ? (byte)(startCharLocation - newDesign.Length) : (byte)(startCharLocation + newDesign.Length);
                    byte lengthCharDifference = (byte)(charLength - newDesign.Length);
                    byte indexCharSubtraction = startCharLocation < charAmount ? (byte)0 : lengthCharDifference;
                    for (int n = startCharLocation; n != endCharLoction; n += charDirection)
                    {
                        newDesign[n - indexCharSubtraction] = charArray[n];
                        if (DECLocation[n] == true)
                            DECSymbolUsed[n] = true;
                        else
                            DECSymbolUsed[n] = false;
                    }
                    if (charDECLength > charLength) //checks to see if DEC escape signs might should be reimplemented
                    {
                        List<char> reconsturectionChar = new List<char>();
                        for (int k = 0; k < DECSymbolUsed.Length; k++)
                        {
                            if (DECSymbolUsed[k] == true)
                            {
                                reconsturectionChar.Add('\u001b');
                                reconsturectionChar.Add('(');
                                reconsturectionChar.Add('0'); //active DEC
                                reconsturectionChar.Add(charArray[k]);
                                reconsturectionChar.Add('\u001b');
                                reconsturectionChar.Add('(');
                                reconsturectionChar.Add('B'); //deactive DEC
                            }
                            else if (DECSymbolUsed[k] == false)
                                reconsturectionChar.Add(charArray[k]);
                        }
                        if (reconsturectionChar.Count > newDesign.Length)
                            newDesign = reconsturectionChar.ToArray();
                    }
                    string newDesignString = new string(newDesign);
                    return newDesignString;
                }
            }
            return design;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawLocationX"></param>
        /// <param name="drawLocationY"></param>
        /// <param name="mapLoc"></param>
        /// <param name="design"></param>
        /// <param name="mapLocation"></param>
        /// <returns></returns>
        static private byte[] PaintCalculations(out int drawLocationX, out int drawLocationY, int[] mapLoc, PointF[] design, byte[] mapLocation)
        {
            drawLocationX = 0;
            drawLocationY = 0;
            return null;
        }

        /// <summary>
        /// Calculates the size of <paramref name="design"/> and returns the locations of the lowest and highest X and Y.
        /// </summary>
        /// <param name="design"></param>
        /// <returns></returns>
        static private float[,] DesignSizeGUICalculatior(PointF[] design) //expalin that the first part of the return index is lowest and the last part is the highest
        {
            float lowestX = float.MaxValue;
            float lowestY = float.MaxValue;
            float highestX = float.MinValue;
            float highestY = float.MinValue;
            foreach (PointF pointF in design)
            {
                lowestX = pointF.X < lowestX ? pointF.X : lowestX;
                lowestY = pointF.Y < lowestY ? pointF.Y : lowestY;
                highestX = pointF.X > highestX ? pointF.X : highestX;
                highestY = pointF.Y > highestY ? pointF.Y : highestY;
            }
            return new float[,] { { lowestX, lowestY }, { highestX, highestY } };
        }

        /// <summary>
        /// Aligns <paramref name="design"/> to have its point closes to (0,0) moved to (0,0) and ensures all other point are corrosponding moved. 
        /// </summary>
        /// <param name="design"
        /// <returns></returns>
        static private PointF[] ZeroAlign(PointF[] design) //improve that summary. 
        {
            float[,] originalLocation = DesignSizeGUICalculatior(design);
            PointF[] convertedDesign = new PointF[design.Length];
            for (int i = 0; i < convertedDesign.Length; i++)
                convertedDesign[i] = design[i];

            if(originalLocation[0,0] != 0)
            {
                for (int i = 0; i < design.Length; i++)
                {
                    convertedDesign[i].X -= originalLocation[0, 0];
                }
            }
            if(originalLocation[0,1] != 0)
            {
                for (int i = 0; i < design.Length; i++)
                {
                    convertedDesign[i].Y -= originalLocation[0, 1];
                }
            }
            return convertedDesign;
        }

        /// <summary>
        /// Takes <paramref name="design"/> and scale it by the value of <paramref name="scale"/> and returns it.
        /// </summary>
        /// <param name="design"></param>
        /// <param name="scale"></param>
        /// <returns>Returns a PointF array of <paramref name="design"/> scaled with <paramref name="scale"/>.</returns>
        static private PointF[] Resize(PointF[] design, float scale, bool aligne = false)
        {
            PointF[] alignedDesign;
            if (aligne)
                alignedDesign = ZeroAlign(design);
            else
            {
                alignedDesign = new PointF[design.Length];
                for (int i = 0; i < alignedDesign.Length; i++)
                    alignedDesign[i] = design[i];
            }

            for (int i = 0; i < alignedDesign.Length; i++)
            {
                alignedDesign[i].X *= scale;
                alignedDesign[i].Y *= scale;
            }
            return alignedDesign;
        }

        /// <summary>
        /// Creates the command prompt outlines of the chess board. 
        /// </summary>
        public static void BoardSetupCMD()
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
            Console.SetCursorPosition(distance - 1 + offset[0] + edgeSpacing + spacing - 1, offset[1] + edgeSpacing + spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + lineBaseColour[0] + ";" + lineBaseColour[1] + ";" + lineBaseColour[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + topRightCorner + Settings.CVTS.DEC.DEC_Deactive}");

            //bottom left corner
            Console.SetCursorPosition(offset[0] + edgeSpacing + spacing - 1, distance - 1 + offset[1] + edgeSpacing + spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + lineBaseColour[0] + ";" + lineBaseColour[1] + ";" + lineBaseColour[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + bottomLeftCorner + Settings.CVTS.DEC.DEC_Deactive}");

            //bottom right corner 
            Console.SetCursorPosition(distance - 1 + offset[0] + edgeSpacing + spacing - 1, distance - 1 + offset[1] + edgeSpacing + spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + lineBaseColour[0] + ";" + lineBaseColour[1] + ";" + lineBaseColour[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + Settings.CVTS.Reset,
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
                Console.SetCursorPosition(offset[0] - edgeSpacing - 1 + spacing, k + edgeSpacing + spacing + offset[1] + alignment + (squareSize * k) - 1);
                Console.Write(numbers[7 - k]); //- Settings.EdgeSpacing should not be used in the calculation above.
                Console.SetCursorPosition(offset[0] + distance + edgeSpacing + spacing, k + spacing + edgeSpacing + offset[1] + alignment + (squareSize * k) - 1);
                Console.Write(numbers[7 - k]);
            }

            for (int k = 0; k < letters.Length; k++) //letters
            {
                Console.SetCursorPosition(k + spacing + offset[0] + alignment + (squareSize * k), offset[1] + spacing - edgeSpacing - 1);
                Console.Write(letters[k]);
                Console.SetCursorPosition(k + spacing + offset[0] + alignment + (squareSize * k),offset[1] + distance + spacing + edgeSpacing);
                Console.Write(letters[k]);
            }

            BoardColouringCMD();
        }

        /// <summary>
        /// Colours the command promp chess table.
        /// </summary>
        private static void BoardColouringCMD()
        {
            ushort distance = (ushort)(8 + 8 * squareSize);
            byte location = 1;
            for (int n = 0; n < distance; n += 1 + squareSize)
            {
                for (int m = 0; m < distance; m += 1 + squareSize)
                {
                    for (int i = 0; i < squareSize; i++)
                    {
                        for (int k = 0; k < squareSize; k++)
                        {
                            Console.SetCursorPosition(i + offset[0] + edgeSpacing + spacing + n, k + offset[1] + spacing + edgeSpacing + m);
                            if (location % 2 == 1)
                                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + whiteBackgroundColour[0] + ";" + whiteBackgroundColour[1] + ";" + whiteBackgroundColour[2] + "m " + Settings.CVTS.Reset);
                            else if (location % 2 == 0)
                                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + blackBackgroundColour[0] + ";" + blackBackgroundColour[1] + ";" + blackBackgroundColour[2] + "m " + Settings.CVTS.Reset);
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
            /// <summary> //make all these readonly or allow (for the GUI version) them to be overwritten with the resize and aligned versions. 
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
