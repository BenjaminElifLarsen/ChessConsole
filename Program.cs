﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Security.Cryptography;

namespace Chess
{   //https://www.chessvariants.com/d.chess/chess.html
    //team == false, player = black, else player = white. White top, black bottom
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
            chessListBlack = null;
            chessListWhite = null;
        }
    }

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

    /// <summary>
    /// Class that contains the settings used by the program.
    /// </summary>
    public class Settings
    { 
        private Settings() { }
        private static bool canHighlight = true; 
        private static byte squareSize = 5; //default is 5.
        private static byte chesspieceDesignSize = 3;
        private static byte[] lineColour = new byte[] { 122, 122, 122 };
        private static byte[] lineColourBase = new byte[] { 87, 65, 47 };
        private static byte[] squareColour1 = new byte[] { 182, 123, 91 };
        private static byte[] squareColour2 = new byte[] { 135, 68, 31 };
        private static byte[] hoverOverSquareColour = new byte[] { 193, 76, 29 };
        private static byte[] chessPieceHoverOverSquareColour = new byte[] { 34, 124, 66 };
        private static byte[] chessPieceHoverOver = new byte[] { 31, 135, 113 };
        private static byte[] menuColour = new byte[] { 0, 255, 0 };
        private static byte[] menuColourHovered = new byte[] { 255, 0, 0 };
        private static byte[] menuColourTitle = new byte[] {255,0,255 };
        private static byte[] offset = new byte[] { 4, 2 }; //works as it should
        private static byte[] menuOffset = new byte[] { 2, 3 };
        private static byte[] menuTitleOffset = new byte[] { (byte)(menuOffset[0]-1),(byte)(menuOffset[1]-1) };
        private static byte[] textOffset = new byte[] { 2, 2 };
        private static byte[] headerTextOffset = new byte[] { 1, 1 };
        private static char lineX = '-'; //works as it should
        private static char lineY = '|'; //works as it should
        private static char exitWordLeft = '[';
        private static char exitWordRight = ']';
        private static char keywordLeft = '{'; //maybe use an array instead of, new char[2];
        private static char keywordRight = '}';
        private static char offsetWord = '@';
        private static char[] exitWordBracket = new char[2] { exitWordLeft, exitWordRight };
        private static char[] keywordBracket = new char[2] {keywordLeft, keywordRight };
        private static ConsoleKey controlKey = ConsoleKey.Enter;
        private static ConsoleKey downKey = ConsoleKey.DownArrow;
        private static ConsoleKey upKey = ConsoleKey.UpArrow;
        private static ConsoleKey leftKey = ConsoleKey.LeftArrow;
        private static ConsoleKey rightKey = ConsoleKey.RightArrow;
        private static ConsoleKey escapeKey = ConsoleKey.Escape;
        private static string title = "Chess";
        private static byte extraSpacing = 1; //works as it should
        private static byte edgeSize = (byte)(1); //does not affect the letters and numbers in the correct way. 
        private static byte[] windowSizeModifer = new byte[] { 20, 4 }; //not a setting that should be access too.
        private static byte[] colourWhite = { 255, 255, 255 };
        private static byte[] colourBlack = { 0, 0, 0 };
        private static int[] windowSize = new int[] { squareSize * 8 + 9 + 2 * edgeSize + offset[0] * 2 + windowSizeModifer[0], squareSize * 8 + 9 + 2 * edgeSize + offset[1] * 2 + windowSizeModifer[1] };
        private static int[,] writeLocationCheckHeader = new int[,] { { extraSpacing + windowSize[0] - windowSizeModifer[0], 10 + extraSpacing }, { extraSpacing + windowSize[0] - windowSizeModifer[0] + 8, 10 + extraSpacing } };
        private static int[,] writeLocationCheck = new int[,] { { writeLocationCheckHeader[0, 0], writeLocationCheckHeader[0, 1] + 2 }, { writeLocationCheckHeader[1, 0], writeLocationCheckHeader[1, 1] + 2 } }; //x,y //each line should contain two symbols, e.g. D5, A2 etc..
        //need to deal with nulls in the places that calls the different settings. 
        private static int[] writeLocationPromotion = new int[] { offset[0] + edgeSize + 2, windowSize[1] - windowSizeModifer[1] };
        /// <summary>
        /// Gets the size of the squares. 
        /// </summary>
        public static byte SquareSize { get => squareSize; }
        /// <summary>
        /// Gets the size of the chess pieces. 
        /// </summary>
        public static byte ChesspieceDesignSize { get => chesspieceDesignSize; }
        /// <summary>
        /// Gets the foreground colour of the lines between the squares.
        /// </summary>
        public static byte[] LineColour { get => lineColour; }
        /// <summary>
        /// Gets the background colour of the lines between the squares.
        /// </summary>
        public static byte[] LineColourBase { get => lineColourBase; }
        /// <summary>
        /// Gets the first square board colour.
        /// </summary>
        public static byte[] SquareColour1 { get => squareColour1; }
        /// <summary>
        /// Gets the second square board colour.
        /// </summary>
        public static byte[] SquareColour2 { get => squareColour2; }
        /// <summary>
        /// Gets the colour used to display the possible end locations a chess piece can move too.
        /// </summary>
        public static byte[] SelectSquareColour { get => hoverOverSquareColour; }
        /// <summary>
        /// Gets the colour used to display the hovered over square for a chess piece move. 
        /// </summary>
        public static byte[] SelectMoveSquareColour { get => chessPieceHoverOverSquareColour; }
        /// <summary>
        /// Gets the colour used to display a chess piece if it is hovered over.
        /// </summary>
        public static byte[] SelectPieceColour { get => chessPieceHoverOver; }
        /// <summary>
        /// Gets the offset from the top left corner to the top left part of the board.
        /// </summary>
        public static byte[] Offset { get => offset; }
        /// <summary>
        /// Gets the char used for the x line on the board.
        /// </summary>
        public static char GetLineX { get => lineX; }
        /// <summary>
        /// Gets the char used for the y line on the board.
        /// </summary>
        public static char GetLineY { get => lineY; }
        /// <summary>
        /// Gets the title of the program.
        /// </summary>
        public static string GetTitle { get => title; }
        /// <summary>
        /// Get the spacing that is added to OffSet
        /// </summary>
        public static byte Spacing { get => extraSpacing; } //not all paint functions are used this one properly. 
        /// <summary>
        /// Get the edge size of the board
        /// </summary>
        public static byte EdgeSpacing { get => edgeSize; }
        /// <summary>
        /// Gets the size of the window.
        /// </summary>
        public static int[] WindowSize { get => windowSize; } //consider having two settings for player write locations
        /// <summary>
        /// Gets the locations to write the check header too.
        /// </summary>
        public static int[,] CheckHeaderLocation { get => writeLocationCheckHeader; }
        /// <summary>
        /// Gets the locations to write check out too. 
        /// </summary>
        public static int[,] CheckWriteLocation { get => writeLocationCheck; }
        /// <summary>
        /// Gets the location to write out the promotions. 
        /// </summary>
        public static int[] PromotionWriteLocation { get => writeLocationPromotion; }
        /// <summary>
        /// The colour of the menu options.
        /// </summary>
        public static byte[] MenuColour { get => menuColour; }
        /// <summary>
        /// The colour of the hovered over menu option.
        /// </summary>
        public static byte[] MenuColourHovered { get => menuColourHovered; }
        /// <summary>
        /// The colour of the title of the menu.
        /// </summary>
        public static byte[] MenuColourTitle { get => menuColourTitle; }
        /// <summary>
        /// The offset of the menu. 
        /// </summary>
        public static byte[] MenuOffset { get => menuOffset; }
        /// <summary>
        /// The offset of the title of the menu.
        /// </summary>
        public static byte[] MenuTitleOffset { get => menuTitleOffset; }
        /// <summary>
        /// The colour for the white chess pieces.
        /// </summary>
        public static byte[] WhiteColour { get => colourWhite; }
        /// <summary>
        /// The colour for the black chess pieces. 
        /// </summary>
        public static byte[] BlackColour { get => colourBlack; }
        /// <summary>
        /// Key used to select with.
        /// </summary>
        public static ConsoleKey SelectKey { get => controlKey; }
        /// <summary>
        /// Key used to go down with.
        /// </summary>
        public static ConsoleKey DownKey { get => downKey; }
        /// <summary>
        /// Key used to go up with.
        /// </summary>
        public static ConsoleKey UpKey { get => upKey; }
        /// <summary>
        /// Key used to go left with.
        /// </summary>
        public static ConsoleKey LeftKey { get => leftKey; }
        /// <summary>
        /// Key used to go right with.
        /// </summary>
        public static ConsoleKey RightKey { get => rightKey; }
        /// <summary>
        /// Key used to escape with.
        /// </summary>
        public static ConsoleKey EscapeKey { get => escapeKey; }
        /// <summary>
        /// Text file offset.
        /// </summary>
        public static byte[] TextOffset { get => textOffset; }
        /// <summary>
        /// Header text file offset
        /// </summary>
        public static byte[] HeaderTextOffset { get => headerTextOffset; }
        /// <summary>
        /// Gets the left and right brackets that indicats a keyword. 
        /// </summary>
        public static char[] KeywordBrackets { get => keywordBracket; }
        /// <summary>
        /// Gets the left and right brackets that indicats an exit word. 
        /// </summary>
        public static char[] ExitWordBrackets { get => exitWordBracket; }
        /// <summary>
        /// Char used to identify any modifier to text execpt for keywords and exit words, see <c>KeywordBrackets</c> and <c>ExitWordBrackets</c> for those. 
        /// </summary>
        public static char TextOffsetChar { get => offsetWord; }
        /// <summary>
        /// If true square/piece can be highlighted else not. It will still highlight empty squares if false. 
        /// </summary>
        public static bool CanHighLight { get => canHighlight; set => canHighlight = value; }
        /// <summary>
        /// Sets the size of the squares. Maximum value is 10, minimum value is 1.
        /// </summary>
        public static byte SetSquareSize { set
            {
                if (value > 9)
                    squareSize = 9;
                else if (value < 1)
                    squareSize = 1;
                else
                    squareSize = value;
            } 
        }

        /// <summary>
        /// Updates the screen size and chess writing locations (pawns' promotions and chech location).
        /// </summary>
        public static void UpdateScreen()
        {
            windowSize = new int[] { squareSize * 8 + 9 + 2 * edgeSize + offset[0] * 2 + windowSizeModifer[0], squareSize * 8 + 9 + 2 * edgeSize + offset[1] * 2 + windowSizeModifer[1] };
            writeLocationCheckHeader = new int[,] { { extraSpacing + windowSize[0] - windowSizeModifer[0], 10 + extraSpacing }, { extraSpacing + windowSize[0] - windowSizeModifer[0] + 8, 10 + extraSpacing } };
            writeLocationCheck = new int[,] { { writeLocationCheckHeader[0, 0], writeLocationCheckHeader[0, 1] + 2 }, { writeLocationCheckHeader[1, 0], writeLocationCheckHeader[1, 1] + 2 } };
            writeLocationPromotion = new int[] { offset[0] + edgeSize + 2, windowSize[1] - windowSizeModifer[1] };
        }

        /// <summary>
        /// Console Virtual Terminal Sequences
        /// </summary>
        public class CVTS //Console Virtual Terminal Sequences
        { 

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool GetConsoleMode(IntPtr handle, out int mode);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GetStdHandle(int handle);

            private static string whiteBrightForColour = "\x1b[97m";
            private static string cyanBrightForColour = "\x1b[96m";
            private static string underscore = "\x1b[4m";
            private static string underscore_off = "\x1b[24m";
            private static string reset = "\x1b[0m";
            private static string redBrightForColour = "\x1b[91m";
            private static string foregroundColouring = "\x1b[38;2;";
            private static string backgroundColouring = "\x1b[48;2;";

            /// <summary>
            /// Sets the foreground colour. Does not need ExtendedForegroundColour_RGB to work
            /// </summary>
            public static string BrightWhiteForeground { get => whiteBrightForColour; }
            /// <summary>
            /// Sets the foreground colour. Does not need ExtendedForegroundColour_RGB to work
            /// </summary>
            public static string BrightCyanForeground { get => cyanBrightForColour; }
            /// <summary>
            /// Sets the foreground colour. Does not need ExtendedForegroundColour_RGB to work
            /// </summary>
            public static string BrightRedForeground { get => redBrightForColour; }
            /// <summary>
            /// Allows the use of rgb values for the foregrund. Values given right after the call. The values need to be given as "r;g;b m"
            /// </summary>
            public static string ExtendedForegroundColour_RGB { get =>foregroundColouring; }
            /// <summary>
            /// Allows the use of rgb values for the background. Values given right after the call. The values need to be given as "r;g;b m". 
            /// </summary>
            public static string ExtendedBackgroundColour_RGB { get => backgroundColouring; }
            /// <summary>
            /// Puts underscore on all text after this call. <c>Underscore_Off</c> to turn underscore off. 
            /// </summary>
            public static string Underscore { get => underscore; }
            /// <summary>
            /// Ensures that there no underscore on the text after this call.
            /// </summary>
            public static string Underscore_Off { get => underscore_off; }
            /// <summary>
            /// All text called after this will be displayed with default values.
            /// </summary>
            public static string Reset { get => reset; }

            /// <summary>
            /// Allows the use of CVTS and DEC.
            /// </summary>
            public static void ActivateCVTS()
            {
                var handle = GetStdHandle(-11);
                int mode;
                GetConsoleMode(handle, out mode);
                SetConsoleMode(handle, mode | 0x4);
            }
            
            /// <summary>
            ///  Digital Equipment Corporation Special Graphics Character Set class. Requires <c>Settings.CVTS.ActivateCVTS()</c> to be called first to work
            /// </summary>
            public class DEC
            {
                private static string DECActive = "\x1b(0";
                private static string DECDeactive = "\x1b(B";
                private static string DECVerticalLine = "x";
                private static string DECHorizontalLine = "q";
                private static string DECFullIntersection = "n";
                private static string DECUpIntersection = "v";
                private static string DECDownIntersection = "w";
                private static string DECRightIntersection = "t";
                private static string DECLeftIntersection = "u";
                private static string DECBottomRightCorner = "j";
                private static string DECTopRightCorner = "k";
                private static string DECTopLeftCorner = "l";
                private static string DECBottomLeftCorner = "m";
                private static string DECPlusMinus = "g";

                /// <summary>
                /// Activates DEC and all texts after this call will be displayed as DEC rather than ASCII. Use <c>DEC_Deactive</c> to deactive DEC character set mapping
                /// </summary>
                public static string DEC_Active { get => DECActive; } //all of these DEC have nothing to do with CVTS and should be moved to their own class.
                /// <summary>
                /// Deactivates DEC and all texts after this call will be displayed as ASCII. 
                /// </summary>
                public static string DEC_Deactive { get => DECDeactive; }
                /// <summary>
                /// Get the symbol used to display a vertical line in DEC.
                /// </summary>
                public static string DEC_Vertical_Line { get => DECVerticalLine; }
                /// <summary>
                /// Gets the symbol used to display a horizontal line in DEC. 
                /// </summary>
                public static string DEC_Horizontal_Line { get => DECHorizontalLine; }
                /// <summary>
                /// Gets the symbol used to display a full intersection in DEC.
                /// </summary>
                public static string DEC_Intersection_Full { get => DECFullIntersection; }
                /// <summary>
                /// Gets the symbol used to display an intersection of the left in DEC.
                /// </summary>
                public static string DEC_Intersection_Right { get => DECLeftIntersection; }
                /// <summary>
                /// Gets the symbol used to display an intersection of the right in DEC.
                /// </summary>
                public static string DEC_Intersection_Left { get => DECRightIntersection; }
                /// <summary>
                /// Gets the symbol used to display an intersection of the top in DEC.
                /// </summary>
                public static string DEC_Intersection_Top { get => DECDownIntersection; }
                /// <summary>
                /// Gets the symbol used to display an intersection of the bottom in DEC.
                /// </summary>
                public static string DEC_Intersection_Bottom { get => DECUpIntersection; }
                /// <summary>
                /// Gets the symbol used to display a top right corner in DEC.
                /// </summary>
                public static string DEC_Corner_TopRight { get => DECTopRightCorner; }
                /// <summary>
                /// Gets the symbol used to display a top left corner in DEC.
                /// </summary>
                public static string DEC_Corner_TopLeft { get => DECTopLeftCorner; }
                /// <summary>
                /// Gets the symbol used to display a bottom right corner in DEC.
                /// </summary>
                public static string DEC_Corner_BottomRight { get => DECBottomRightCorner; }
                /// <summary>
                /// Gets the symbol used to display a bottom left corner in DEC.
                /// </summary>
                public static string DEC_Corner_BottomLeft { get => DECBottomLeftCorner; }
                /// <summary>
                /// Gets the symbol used to display a plus/minus in DEC.
                /// </summary>
                public static string DEC_Plus_Minus { get => DECPlusMinus; }
            }
        }

    }

    /// <summary>
    /// Contains the publisher class that all objects, that needs to react when a key is pressed, should subscribe too.
    /// </summary>
    public class Publishers
    {
        private static KeyPublisher pubKey;
        private static NetPublisher pubNet;
        private static CapturePublisher pubCapture;

        private Publishers() { }
        /// <summary>
        /// Get the key publisher class instant.
        /// </summary>
        public static KeyPublisher PubKey { get => pubKey; }
        /// <summary>
        /// Gets the network publisher class instant.
        /// </summary>
        public static NetPublisher PubNet { get => pubNet; }
        /// <summary>
        /// Gets the capture publisher class instant.
        /// </summary>
        public static CapturePublisher PubCapture { get => pubCapture; }
        /// <summary>
        /// Ensures that the publisher instant is not null if it is null.
        /// </summary>
        public static void SetKeyClass()
        {
            if(pubKey == null)
                pubKey = new KeyPublisher();
        }
        /// <summary>
        /// 
        /// </summary>
        public static void SetNetClass()
        {
            if (pubNet == null)
                pubNet = new NetPublisher();
        }
        /// <summary>
        /// 
        /// </summary>
        public static void SetCaptureClass()
        {
            if (pubCapture == null)
                pubCapture = new CapturePublisher();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GameStates
    {
        private GameStates() { }
        private static bool canMove;
        private static bool? won; //null for draw, true for win, false for lose.
        private static bool gameEnded;
        private static bool isOnline;
        private static bool whiteWon; //true for white, false for black.
        private static bool pause;
        private static byte[,] chessPieceAmounts = new byte[2,2];
        private static short turns = -1;
        private static short turnDrawCounter;
        private static bool nonTurnWin;
        private static bool? playerTeam = null;
        private static bool lostConnection;

        /// <summary>
        /// True if it is player turn, else false.
        /// </summary>
        public static bool IsTurn { get => canMove; set => canMove = value; }
        /// <summary>
        /// Null for draw, true for victory, false for defeat.
        /// </summary>
        public static bool? Won { get => won; set => won = value; } //rename to VictoryType
        /// <summary>
        /// True if the game has ended, false otherwise. 
        /// </summary>
        public static bool GameEnded { get => gameEnded; set => gameEnded = value; }
        /// <summary>
        /// If true the game is played online, else false.
        /// </summary>
        public static bool IsOnline { get => isOnline; set => isOnline = value; }
        /// <summary>
        /// True if the white player won, false otherwise.
        /// </summary>
        public static bool WhiteWin { get => whiteWon; set => whiteWon = value; }
        /// <summary>
        /// Used to "pause" the game while waiting on data from the other player.
        /// </summary>
        public static bool Pause { get => pause; set => pause = value; }
        /// <summary>
        /// The other player surrendered while it was not their turn. 
        /// </summary>
        public static bool OtherPlayerSurrendered { get => nonTurnWin; set => nonTurnWin = value; }
        /// <summary>
        /// True for white player. False for black player. Only used online. If playing offline it is set to null.
        /// </summary>
        public static bool? PlayerTeam { get => playerTeam; set => playerTeam = value; }
        /// <summary>
        /// True if the connect is lost and the game ends.
        /// </summary>
        public static bool LostConnection { get => lostConnection; set => lostConnection = value; }
        /// <summary>
        /// Sets and gets the number of chespieces. [0,~] is white. [1,~] is black. [~,0] is new value. [~,1] is old value.
        /// </summary>
        public static byte[,] PieceAmount { get => chessPieceAmounts; set => chessPieceAmounts = value; }
        /// <summary>
        /// The amount of turns the game has lasted.
        /// </summary>
        public static short TurnCounter { get => turns; set => turns = value; }
        /// <summary>
        /// Amount of turns since the last capture or moved pawn.
        /// </summary>
        public static short TurnDrawCounter { get => turnDrawCounter; set => turnDrawCounter = value; }
        /// <summary>
        /// Resets all game states. 
        /// </summary>
        public static void Reset()
        {
            pause = false;
            whiteWon = false;
            isOnline = false;
            gameEnded = false;
            won = null;
            canMove = false;
            nonTurnWin = false;
            NetSearch.Abort = false;
            NetSearch.Searching = false;
            turns = -1;
            turnDrawCounter = 0;
            playerTeam = null;
        }

        public class NetSearch
        {

            private static bool searchingForPlayer;
            private static bool abort;

            /// <summary>
            /// True if searching for a player, otherwise false.
            /// </summary>
            public static bool Searching { get => searchingForPlayer; set => searchingForPlayer = value; }
            /// <summary>
            /// True if the search for other players should be aborted, else false. 
            /// </summary>
            public static bool Abort { get => abort; set => abort = value; }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            menu.Run();
        }
    } 

    /// <summary>
    /// Event class with events related to the control of the program.
    /// </summary>
    public class ControlEvents : EventArgs
    {
        /// <summary>
        /// Class that holds event data of the input control system.
        /// </summary>
        public class KeyEventArgs
        {
            /// <summary>
            /// Base constructor for the consoleKey event data.
            /// </summary>
            /// <param name="key">The ConsoleKeyInfo to be transmitted.</param>
            public KeyEventArgs(ConsoleKeyInfo key)
            {
                Key = key;
            }
            /// <summary>
            /// 
            /// </summary>
            public ConsoleKeyInfo Key { get; set; }
        }

        /// <summary>
        /// Class that holds event data for capture of pieces.
        /// </summary>
        public class CaptureEventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ID"></param>
            public CaptureEventArgs(string ID)
            {
                this.ID = ID;
            }
            /// <summary>
            /// 
            /// </summary>
            public string ID { get; set; }
        }

        /// <summary>
        /// Class that holds event data of the network portion. 
        /// </summary>
        public class NetworkEventArgs
        {
            /// <summary>
            /// Base constructor for the network event data. 
            /// </summary>
            /// <param name="lostConnection">True if the connect is lost and the game ends.</param>
            /// <param name="whiteWon">True if the white player won, false otherwise.</param>
            /// <param name="otherPlayerSurrendered">True if the game has ended, false otherwise. </param>
            /// <param name="gameEnded">True if the game has ended, false otherwise. </param>
            /// <param name="pause">Used to "pause" the game while waiting on data from the other player.</param>
            /// <param name="won">Null for draw, true for victory, false for defeat.</param>
            /// <param name="isTurn">True if it is player turn, else false.</param>
            public NetworkEventArgs(bool? lostConnection = null, bool? whiteWon = null, bool? otherPlayerSurrendered = null, 
                                    bool? gameEnded = null, bool? pause = null, bool? won = null, bool? isTurn = null)
            {
                LostConnection = lostConnection;
                WhiteWon = whiteWon;
                OtherPlayerSurrendered = otherPlayerSurrendered;
                GameEnded = gameEnded;
                Pause = pause;
                Won = won;
                IsTurn = isTurn;
            }
            /// <summary>
            /// True if the connect is lost and the game ends.
            /// </summary>
            public bool? LostConnection { get; set; }
            /// <summary>
            /// True if the white player won, false otherwise.
            /// </summary>
            public bool? WhiteWon { get; set; }
            /// <summary>
            /// If ture, the other player surrendered while it was not their turn. 
            /// </summary>
            public bool? OtherPlayerSurrendered { get; set; }
            /// <summary>
            /// True if the game has ended, false otherwise. 
            /// </summary>
            public bool? GameEnded { get; set; }
            /// <summary>
            /// Used to "pause" the game while waiting on data from the other player.
            /// </summary>
            public bool? Pause { get; set; }
            /// <summary>
            /// Null for draw, true for victory, false for defeat.
            /// </summary>
            public bool? Won { get; set; }
            /// <summary>
            /// True if it is player turn, else false.
            /// </summary>
            public bool? IsTurn { get; set; }
        }

    }

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

    /// <summary>
    /// Class contains events, delegates and functions related to the movement and selection using a keyboard.  
    /// </summary>
    public class KeyPublisher
    {
        public delegate void keyEventHandler(object sender, ControlEvents.KeyEventArgs args);

        public event keyEventHandler RaiseKeyEvent;

        /// <summary>
        /// Runs as long time the program is running. If a a key is pressed it will trigger an event that transmit the <c>ConsoleKeyInfo key</c> to all subscribers.  
        /// </summary>
        public void KeyPresser()
        {
            while (true) //!GameStates.GameEnded
            {
                while (!Console.KeyAvailable) ;
                ConsoleKeyInfo key = Console.ReadKey(true);
                OnKeyPress(new ControlEvents.KeyEventArgs(key));
                while (Console.KeyAvailable)
                    Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Invokes the event RaiseKeyEvent. 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnKeyPress(ControlEvents.KeyEventArgs e)
        {
            keyEventHandler eventHandler = RaiseKeyEvent;
            if (eventHandler != null)
                eventHandler.Invoke(this, e);
        }
    }

    public class CapturePublisher
    {
        public delegate void captureEventHandler(object sender, ControlEvents.CaptureEventArgs args);

        public event captureEventHandler RaiseCaptureEvent;

        public void Capture(string ID)
        {
            OnCapture(new ControlEvents.CaptureEventArgs(ID));
        }

        protected virtual void OnCapture(ControlEvents.CaptureEventArgs e)
        {
            captureEventHandler captureEventHandler = RaiseCaptureEvent;
            var test = captureEventHandler.GetInvocationList();
            if (captureEventHandler != null)
                captureEventHandler.Invoke(this, e);
        }
    }

    /// <summary>
    /// Contains classes for transmitting and receivering data and a support class.
    /// </summary>
    class Network
    {
        //for the transmission and reception of the map array. You could write it into to a string, encode it and trasmit it. Then decode it and, knowing the size of the array, write it back into the map.
        //given you always know when the program should trasmit and recieve data, transmit after ones play and recieve until it receives data, is it not needed to multitread this part of the program?
        //in the menu class, the network menu will call another play function that is designed for network.
        /* https://docs.microsoft.com/en-us/dotnet/framework/network-programming/?redirectedfrom=MSDN
         * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/network-programming-samples
         * https://www.codeproject.com/Articles/1415/Introduction-to-TCP-client-server-in-C
         * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/how-to-create-a-socket
         * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/using-client-sockets
         * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/listening-with-sockets
         * https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.bind?view=netcore-3.1
         * https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.listen?view=netcore-3.1
         * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/introducing-pluggable-protocols
         * https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?redirectedfrom=MSDN&view=netcore-3.1
         * https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient.connect?view=netcore-3.1#System_Net_Sockets_TcpClient_Connect_System_String_System_Int32_
         * https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=netcore-3.1
         * http://csharp.net-informations.com/communications/csharp-chat-client.htm
         * https://stackoverflow.com/questions/7508942/loopback-localhost-question
         * https://stackoverflow.com/questions/12952679/socket-programming-in-c-sharp-the-client-never-connects-to-the-server
         * https://stackoverflow.com/questions/15162312/tcpclient-not-connecting-to-local-computer
         * https://stackoverflow.com/questions/37308169/c-sharp-tcp-socket-client-refuses-connection
         * http://csharp.net-informations.com/communications/csharp-socket-programming.htm
         * https://www.geeksforgeeks.org/socket-programming-in-c-sharp/
         * https://www.codeproject.com/Articles/10649/An-Introduction-to-Socket-Programming-in-NET-using
         * https://www.c-sharpcorner.com/article/socket-programming-in-C-Sharp/
         */
        static TcpClient transmitter = null;
        static TcpListener receiver = null;

        //public Network()
        //{
        //    //Receive reTest = new Receive();
        //    //Transmit trTest = new Transmit();
        //    Receive.ReceiveSetup();

        //    //Transmit.TransmitSetup();
        //}

        /// <summary>
        /// Class for transmitting data.
        /// </summary>
        public class Transmit
        {
            private static string ipAddress_other;
            /// <summary>
            /// Gets the IP address of the other player.
            /// </summary>
            public static string OtherPlayerIpAddress { get => ipAddress_other; set => ipAddress_other = value; }

            /// <summary>
            /// Sets up the transmitter with <paramref name="IPaddress"/>.
            /// If it succeeded it return true, else false.
            /// </summary>
            /// <param name="IPaddress">The IP address to connect too.</param>
            /// <returns>Returns true if it can connect to <paramref name="IPaddress"/>, else false.</returns>
            public static bool TransmitSetup(string IPaddress)
            {
                try
                {
                    int port = 23000;
                    IPAddress transmitterAddress = IPAddress.Parse(IPaddress);
                    transmitter = new TcpClient(IPaddress, port);
                    return true;
                }
                catch (Exception e)
                { //write anything out?
                    Debug.WriteLine(e);
                    if (!GameStates.LostConnection)
                    {
                        Console.Clear();
                        Console.WriteLine("Connection could not be established/was lost");
                        Console.WriteLine("Enter to return too the menu");
                        while (Console.ReadKey().Key != ConsoleKey.Enter) ;
                        GameStates.Won = null;
                        GameStates.LostConnection = true;
                        GameStates.GameEnded = true;
                    }
                    return false;
                }

            }

            /// <summary>
            /// Called to ensure that the receiver of the other player is still running. 
            /// </summary>
            /// <param name="ipAddress">The IP address to contact.</param>
            /// <returns></returns>
            public static void StillConnected(object ipAddress)
            {
                //return GeneralValueTransmission(10, ipAddress,true); //maybe modify GeneralValueTransmission with a bool waitOnAnswer = false;
                //and then it it should wait on an answer and gets none, return false. 

                bool isConnected = true;
                double counter = 0;
                ushort maxValue = 3000;
                Thread connectThread = new Thread(new ThreadStart(Run));
                connectThread.Start();
                //int[] time = new int[] { DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second };
                DateTime oldTime = DateTime.Now;
                do
                {
                    DateTime newTime = DateTime.Now;
                    counter = (newTime - oldTime).TotalMilliseconds;

                    Thread.Sleep(1);
                } while (!GameStates.GameEnded && isConnected && counter < maxValue);
                GameStates.LostConnection = isConnected;
                GameStates.GameEnded = true;
                if (GameStates.PlayerTeam == false)
                    GameStates.TurnCounter++;
                void Run()
                {
                    do {
                        oldTime = DateTime.Now;
                        isConnected = GeneralValueTransmission(10, (string)ipAddress, true);
                        Thread.Sleep(maxValue/2);
                    } while (!GameStates.GameEnded && isConnected);
                }
            }
           
            //public static void TransmitSetup()
            //{ //might need a try catch
            //    Int32 port = 23000;
            //    IPAddress transmitterAddress = IPAddress.Parse("127.0.0.1");
            //    transmitter = new TcpClient("localhost", port);
            //    //https://docs.microsoft.com/en-us/dotnet/api/system.net.networkinformation.tcpconnectioninformation?redirectedfrom=MSDN&view=netcore-3.1


            /// <summary>
            /// Transmit the map data to the IP address stored in <c>OtherPlayerIpAddress</c>.
            /// </summary>
            public static bool TransmitMapData(string ipAddress)
            { 
                //should it only allow IPv4 or also IPv6?
                try //is it better to use Socket or TcpListiner/TcpClient?
                { // https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.networkstream?view=netcore-3.1
                    //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket?view=netcore-3.1
                    //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.sockettype?view=netcore-3.1#System_Net_Sockets_SocketType_Stream
                    //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/using-an-asynchronous-client-socket?view=netcore-3.1
                    //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/sockets?view=netcore-3.1
                    byte[] receptionAnswerByte = new byte[4];
                    short receptionAnswer = -1;

                    //contacts the receiver of the other player. 
                    TransmitSetup(ipAddress);

                    //converts the map to a string.
                    string mapData = NetworkSupport.MapToStringConvertion();

                    //open transmission
                    NetworkStream networkStream = transmitter.GetStream();

                    //transmit data so the receiver knows it is about to receive map data
                    byte[] data;
                    Converter.Conversion.ValueToBitArrayQuick(1, out data);
                    networkStream.Write(data, 0, data.Length);

                    //convert and transmit map data size.
                    byte[] mapdataByte = Converter.Conversion.ASCIIToByteArray(mapData);
                    byte[] mapdataByteSize = null;
                    Converter.Conversion.ValueToBitArrayQuick(mapdataByte.Length, out mapdataByteSize);
                    networkStream.Write(mapdataByteSize, 0, mapdataByteSize.Length);

                    Debug.WriteLine("Transmitting map. {0} bytes", mapdataByte.Length);

                    //Testing for possible errors
                    //Environment.Exit(1);

                    //wait on answer from the receiver
                    networkStream.Read(receptionAnswerByte, 0, 4);  //the reads are used to ensure that the TransmitData is not ahead of ReceiveData. This ensures the next turn is not started before the map and chess pieces are ready.
                    Converter.Conversion.ByteConverterToInterger(receptionAnswerByte, ref receptionAnswer);
                    //should be 0.

                    //transmit the mapdataByte
                    networkStream.Write(mapdataByte, 0, mapdataByte.Length);

                    //wait on answer from the receiver. 
                    networkStream.Read(receptionAnswerByte, 0, 4); //penty of exceptions to deal with regarding .Write() and .Read()
                    //it does not seem like there is a function that allows to control whether a networkstream can write or not, so the .CanWrite() will be needed most likely. Try and find out what determine if it possible to write to a stream or not. 
                    //same for .CanRead()
                    //After the CanRead() page: "Provide the appropriate FileAccess enumerated value in the constructor to set the readability and writability of the NetworkStream.", so most likely will not have to worry about it. Can be useful for other projects
                    Converter.Conversion.ByteConverterToInterger(receptionAnswerByte, ref receptionAnswer);
                    //should be 1. 

                    //shut down transmission
                    networkStream.Close();
                    transmitter.Close();
                    return true;

                }
                catch (ObjectDisposedException e) //this should only be entered if the receiver has been shut down... I think
                {
                    Debug.WriteLine(e);
                    transmitter.Close(); 

                    //GameStates.Won = null;
                    //GameStates.LostConnection = true;
                    //GameStates.GameEnded = true;
                    Publishers.PubNet.TransmitAnyData(won: null, lostConnection: true, gameEnded: true);
                    return false;
                }
                catch (Exception e)
                { //what to do if this is entered?
                    Debug.WriteLine(e);
                    //GameStates.Won = null;
                    //GameStates.LostConnection = true;
                    //GameStates.GameEnded = true;
                    Publishers.PubNet.TransmitAnyData(won: null, lostConnection: true, gameEnded: true);
                    return false;
                }

            }

            /// <summary>
            /// Used to transmit integers while the chess gameplay is going on.
            /// <c>Network.Receive.ReceiveGameLoop</c> will receive the data.
            /// Will return true if it success transmitting <paramref name="data"/> to <paramref name="ipAddress"/>, else false.
            /// </summary>
            /// <param name="data"> The int value to transmit.</param>
            /// <param name="ipAddress">The IP address to transmit it too.</param>
            /// <param name="waitOnAnswer">If true, it will wait on receiving data back.</param>
            /// <returns>Returns true if it can trasmit data, else false. 
            /// If <paramref name="waitOnAnswer"/> is true, it will return true if it receives an answer back, else it will return false.</returns>
            public static bool GeneralValueTransmission(int data, string ipAddress, bool waitOnAnswer = false)
            {
                Debug.WriteLine("Data transmission: {0}",data);
                bool returnValue;
                try
                {
                    TransmitSetup(ipAddress);
                    Converter.Conversion.ValueToBitArrayQuick(data, out byte[] dataArray);
                    NetworkStream networkStream = transmitter.GetStream();
                    networkStream.Write(dataArray, 0, dataArray.Length);
                    if (waitOnAnswer)
                    {
                        byte[] returnData = new byte[4];
                        int answer = 0;
                        networkStream.Read(returnData, 0, 4);
                        Converter.Conversion.ByteConverterToInterger(returnData, ref answer);
                        if (answer == 1)
                            returnValue = true;
                        else
                            returnValue = false;
                    }
                    else
                        returnValue = true;
                    networkStream.Close();
                    transmitter.Close();
                    return returnValue;
                }
                catch
                {
                    returnValue = false;
                    return returnValue;
                }

            }

            /// <summary>
            /// Transmit any <paramref name="data"/> string to the IP-address stored in <c>OtherPlayerIpAddress</c>.
            /// </summary>
            /// <param name="data">String to transmit.</param>
            /// <returns>Returns true when <paramref name="data"/> has been trasmitted. </returns>
            public static bool GeneralDataTransmission(string data, string ipAddress)
            {
                Debug.WriteLine("Transmission: {0}", data);
                try
                {
                    byte[] reply = new byte[1];

                    //connect to server
                    TransmitSetup(ipAddress);
                    NetworkStream networkStream = transmitter.GetStream();

                    //read data
                    networkStream.Read(reply, 0, reply.Length);

                    //transmit data string length
                    byte[] stringByte = Converter.Conversion.ASCIIToByteArray(data);
                    byte[] stringByteLengthByte;
                    Converter.Conversion.ValueToBitArrayQuick(stringByte.Length, out stringByteLengthByte);
                    networkStream.Write(stringByteLengthByte, 0, stringByteLengthByte.Length);

                    //read data
                    networkStream.Read(reply, 0, reply.Length);

                    //transmit string byte array
                    networkStream.Write(stringByte, 0, stringByte.Length);

                    //read data
                    networkStream.Read(reply, 0, reply.Length);

                    //shut down
                    networkStream.Close();
                    transmitter.Close();

                    //return
                    return true; 
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return false;
                }

            }
        }

        /// <summary>
        /// Class for receiving data.
        /// </summary>
        public class Receive
        {
            //public delegate void turnChangedDel(string nogetTekst);
            //public static event turnChangedDel TurnChanged;
            /// <summary>
            /// Starts the receiver. 
            /// </summary>
            public static void Start()
            {
                Debug.WriteLine("Receiver starting up");
                receiver.Start();
                Debug.WriteLine("Receiver has started up");

                //TurnChanged?.Invoke("hello");

            }

            /// <summary>
            /// Stops the receiver.
            /// </summary>
            public static void Stop()
            {
                Debug.WriteLine("Receiver stopping");
                receiver.Stop();
                Debug.WriteLine("Receiver has stopped");
            }

            /// <summary>
            /// Waits on a client connection to the listiner and returns the IP address of the client. 
            /// If the search for a player is aborted, it will return null.
            /// </summary>
            /// <returns>Returns the IP address of the client. If the search for a player is aborted, returns null.</returns>
            public static string GetConnectionIpAddress() 
            {
                while (!receiver.Pending() && !GameStates.NetSearch.Abort)
                {

                }
                if (!GameStates.NetSearch.Abort)
                {
                    TcpClient client = receiver.AcceptTcpClient();
                    string endPoint = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    client.Close();
                    GameStates.NetSearch.Searching = false;
                    Debug.WriteLine(endPoint);
                    return endPoint;
                }
                return null;
            }

            /// <summary>
            /// Waits on a client to connect and transmit data. The data is then converted into a ASCII string. //rewrite
            /// </summary>
            /// <returns>Returns an ASCII string received from a client.</returns>
            public static string GeneralDataReception()
            {
                /* Client connect to server. 
                 * Waits on answer from the server
                 * Client writes the string and waits on an answer
                 * Receiver reads the bytes and convert them to a string.
                 * Receiver transmit an answer and the function returns the string. 
                 * Client reads the answer and the function returns "true"
                 */

                //wats on a requist for connection
                while (!receiver.Pending()) ;

                //accepts the client and gets its stream. 
                TcpClient client = receiver.AcceptTcpClient();
                NetworkStream networkStream = client.GetStream();
                byte[] receivedData;

                //writes to the client so it knows a connection has been established.
                networkStream.Write(new byte[] { 0 }, 0, 1);

                //reads data that states the length of the string that is going to be transmitted
                receivedData = new byte[4];
                networkStream.Read(receivedData, 0, receivedData.Length);
                uint dataLength = 0;
                Converter.Conversion.ByteConverterToInterger(receivedData, ref dataLength);
                
                //writes an answer so the transmitter knows it can transmit the string byte array.
                networkStream.Write(new byte[] { 1 }, 0, 1);

                //reads string data sent by the client 
                receivedData = new byte[dataLength];
                networkStream.Read(receivedData, 0, receivedData.Length);

                //converts it to a string
                string data = Converter.Conversion.ByteArrayToASCII(receivedData);

                Debug.WriteLine("Received: {0}",data);

                //writes an answer back, so the transmitter knows it can stop.
                networkStream.Write(new byte[] { 2 }, 0, 1);

                //close connections
                networkStream.Close();
                client.Close();

                //returns the string 
                return data;
            }

            /// <summary>
            /// Sets up the receiver with the <paramref name="IPaddress"/>.
            /// </summary>
            /// <param name="IPaddress">IP addresss to initialise the receiver with.</param>
            /// <returns>Returns true if the receiver is initialised, else false.</returns>
            public static bool ReceiveSetup(string IPaddress, int portNumber = 23000)
            {
                try
                { //tires to start up the receiver. 
                    int port = portNumber;
                    IPAddress receiverAddress = IPAddress.Parse(IPaddress);
                    receiver = new TcpListener(receiverAddress, port);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            //public static void ReceiveSetup()
            //{ //try catch
            //    Int32 port = 23000;
            //    IPAddress receiverAddress = IPAddress.Parse("127.0.0.1"); //change later
            //    receiver = new TcpListener(receiverAddress, port);
            //    //OSSupportsIPv4 aand OSSupportsIPv6 might be useful at some points, used to check whether the underlying operating system and network adaptors support those specific IPvs
            //}

            /// <summary>
            /// Is run while the chess gameplay is going on. 
            /// What more to write?
            /// </summary>
            /// <param name="team">bool: True for white player, false for black player.</param>
            public static void ReceiveGameLoop(object team/*, TcpClient otherPlayer*/)
            {

                //Receive.TurnChanged += Receive_TurnChanged;
                List<Thread> clientThread = new List<Thread>();
                try
                {

                    while (!GameStates.GameEnded)
                    {
                        //waits on someone to connect.
                        while (!receiver.Pending())
                        {
                            if (GameStates.GameEnded) //breaks if the game ends while waiting, e.g. this player ends the game. 
                                break;
                        }
                        if (GameStates.GameEnded) //if the game has ended, no reason to do anything else of the loop. //read to see if there is a better way to do this.
                            break;
                        clientThread.Add(new Thread(new ThreadStart(NonLoopPart)));
                        clientThread[clientThread.Count - 1].Start();

                    }
                    //receiver.Stop();
                }
                catch (IOException e) //failed read, write or connection closed (forced or not forced).
                { //this should catch in case of a lost connection, but what to do? Go back to the main menu? Try some times to get a connection and it failes n times, do something else?
                    Debug.WriteLine(e); //might not be needed with the InnerException, need to read some more up on InnerException
                    Debug.WriteLine(e.InnerException); //helps with identified which specific error. 
                    //receiver.Stop();
                }
                catch (Exception e) //other.
                { //what to do. Some of the possible expections are known from testing, e.g. nulls in the new map array (caused by a bug in the code, might want to catch it anyway).
                    Debug.WriteLine(e);
                    Debug.WriteLine(e.InnerException);
                }
                //finally
                //{
                //    //receiver.Stop();
                //}

                void NonLoopPart()
                {
                    TcpClient otherPlayer = null;
                    NetworkStream networkStream = null;
                    try
                    {
                        byte[] tranmissionAnswerByte = new byte[4];
                        byte[] dataSizeByte = new byte[4];
                        ushort dataSize = 0;

                        //accept client and connects
                        otherPlayer = receiver.AcceptTcpClient();
                        networkStream = otherPlayer.GetStream();

                        //receive data to know what will happen. 
                        while (!networkStream.DataAvailable) ;
                        byte[] typeOfTransmission = new byte[4]; //rename
                        networkStream.Read(typeOfTransmission, 0, 4);
                        int type = 0;
                        Converter.Conversion.ByteConverterToInterger(typeOfTransmission, ref type);

                        Debug.WriteLine("Received data: {0}", type);

                        if (type == 1)
                        {//connected client is about to transmit map data

                            //receive the size of the mapdata string
                            while (!networkStream.DataAvailable) ;

                            networkStream.Read(dataSizeByte, 0, dataSizeByte.Length);
                            Converter.Conversion.ByteConverterToInterger(dataSizeByte, ref dataSize);
                            byte[] data = new byte[dataSize];

                            //transmit an answer, so the client knows that it can transmit the mapdata.
                            Converter.Conversion.ValueToBitArrayQuick(0, out tranmissionAnswerByte);
                            networkStream.Write(tranmissionAnswerByte, 0, tranmissionAnswerByte.Length);

                            //waits on data
                            while (!networkStream.DataAvailable) ;

                            //receive the map data
                            networkStream.Read(data, 0, dataSize); //how to ensure the data is correct? Since it is using TCP, it does not matter. Would matter if it was UDP. 

                            //decode data 
                            string mapdataString = Converter.Conversion.ByteArrayToASCII(data);
                            string[,] newMap = NetworkSupport.StringToMapConvertion(mapdataString);

                            //update the chess pieces and the map
                            //MapMatrix.UpdateOldMap(); 
                            NetworkSupport.UpdatedChessPieces(newMap, (bool)team);
                            NetworkSupport.UpdateMap(newMap);

                            //transmit an answer depending on the received data. This is needed to prevent the game from continue before the map and pieces are updated.
                            Converter.Conversion.ValueToBitArrayQuick(1, out tranmissionAnswerByte);
                            networkStream.Write(tranmissionAnswerByte, 0, tranmissionAnswerByte.Length);

                            //GameStates.IsTurn = true;
                            Publishers.PubNet.IsTurn(true);
                        }
                        else if (type == 2) //draw //not really needed
                        { //game has ended.
                          //GameStates.GameEnded = true;
                          //GameStates.Won = null;
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: null);
                        }
                        else if (type == 3) //being asked for a draw
                        {
                            Console.Clear();
                            //GameStates.Pause = true;
                            Publishers.PubNet.Pause(true);
                            string[] drawOptions = { "Accept Draw", "Decline Draw" };
                            string title = "Other play ask for draw";
                            string answer = Menu.MenuAccess(drawOptions, title);
                            switch (answer)
                            { //the transmitter answer will need to be transmitted by the GeneralValueTransmission since the ReceiveGameLoop will receive the data
                                case "Accept Draw":

                                    //update the turn counter if black player
                                    if (GameStates.PlayerTeam == false)
                                        GameStates.TurnCounter++;
                                    //GameStates.GameEnded = true;
                                    //GameStates.Won = null;
                                    //GameStates.OtherPlayerSurrendered = true; //not the best use since they did not surrender
                                    Publishers.PubNet.TransmitAnyData(gameEnded: true, won: null, otherPlayerSurrendered: true);

                                    //transmit answer
                                    Transmit.GeneralValueTransmission(30, Transmit.OtherPlayerIpAddress);
                                    break;

                                case "Decline Draw":
                                    //transmit answer
                                    Transmit.GeneralValueTransmission(31, Transmit.OtherPlayerIpAddress);

                                    //redraw map and pieces.
                                    Console.Clear();
                                    ChessTable.RepaintBoardAndPieces();
                                    NetworkSupport.RepaintEndLocation((bool)team);
                                    break;
                            }
                            //GameStates.Pause = false;
                            Publishers.PubNet.Pause(false);
                        }
                        else if (type == 4) //this player is victory
                        {
                            if ((bool)team == false)
                                GameStates.TurnCounter++;
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: true);
                            //GameStates.GameEnded = true;
                            //GameStates.Won = true;
                        }
                        else if (type == 5) //this player is defeated
                        {
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: false);
                            //GameStates.GameEnded = true;
                            //GameStates.Won = false;
                        }
                        else if (type == 6) //draw by gamerules.
                        {
                            if ((bool)team == false)
                                GameStates.TurnCounter++;
                            //GameStates.Won = null;
                            //GameStates.GameEnded = true;
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: null);
                        }
                        else if (type == 10) //contacted to ensure there is a connection
                        {
                            Converter.Conversion.ValueToBitArrayQuick(1, out byte[] array);
                            networkStream.Write(array, 0, array.Length);
                        }
                        else if (type == 30) //draw was accepted
                        {
                            if ((bool)team == false)
                                GameStates.TurnCounter++;
                            //GameStates.Pause = false;
                            //GameStates.GameEnded = true;
                            //GameStates.Won = null;
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: null, pause: false);
                        }
                        else if (type == 31) //draw was denied
                        {
                            //GameStates.Pause = false;
                            Publishers.PubNet.Pause(false);
                            Console.Clear();
                            ChessTable.RepaintBoardAndPieces();
                        }
                        else if (type == 41) //won by the other player surrendered
                        {
                            if (GameStates.PlayerTeam == false)
                                GameStates.TurnCounter++;
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: true, otherPlayerSurrendered: true);
                            //GameStates.GameEnded = true;
                            //GameStates.Won = true;
                            //GameStates.OtherPlayerSurrendered = true;
                        }
                        else
                        {//connected client is not about to transmit any registrated data
                         //do something more? 
                         //networkStream.Flush(); //"... however, because NetworkStream is not buffered, it has no effect on network streams." - https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.networkstream.flush?view=netcore-3.1
                        }
                        //shutdown connection to client. 
                        //networkStream.Close();
                        //otherPlayer.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    finally
                    {
                        if (otherPlayer != null) //ensures the connection is closed.
                        {
                            if (networkStream != null)
                                networkStream.Close();
                            otherPlayer.Close();
                        }
                    }
                }
            }


            //private static void Receive_TurnChanged(string nogetTekst)
            //{
            //    throw new NotImplementedException();
            //}
        }

        /// <summary>
        /// Class to support the network, only used by Network. //not fully true anymore.
        /// </summary>
        public static class NetworkSupport
        {
            /// <summary>
            /// Repaints the end location(s) 
            /// </summary>
            /// <param name="team"></param>
            public static void RepaintEndLocation(bool team)
            {
                foreach (ChessPiece chePie in ChessList.GetList(team))
                {
                    if (chePie.IsSelected)
                    {
                        chePie.NetworkUpdate(repaintLocation: true);
                    }
                }
            }

            /// <summary>
            /// Gets the, internetwork, address used by the computer when contacting the router to get out on the internet. 
            /// </summary>
            /// <returns></returns>
            public static string LocalAddress
            {
                get
                {
                    string localIP;
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) //https://en.wikipedia.org/wiki/Berkeley_sockets
                    { //https://stackoverflow.com/questions/6803073/get-local-ip-address
                        socket.Connect("8.8.8.8", 65530);
                        IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                        localIP = endPoint.Address.ToString();
                    }
                    return localIP;
                }
            }

            /// <summary>
            /// Convert the MapMatrix.Map into a string. Each entry is seperated using a !. 
            /// </summary>
            /// <returns>Returns a string consisting of all MapMatrix.Map entires.</returns>
            public static string MapToStringConvertion()
            {
                string mapString = "";
                for (int x = 0; x < MapMatrix.Map.GetLength(0); x++)
                {
                    for (int y = 0; y < MapMatrix.Map.GetLength(1); y++)
                    {
                        if (y == 7 && x == 7)
                            mapString += MapMatrix.Map[x, y]; //last part of the string does not need a !, if it got one, the string will be split into 65 parts.
                        else
                            mapString += MapMatrix.Map[x, y] + "!"; //the ! is to help seperate the string into a 1d array.
                    }
                }
                return mapString;
            }

            /// <summary>
            /// Converts a string into a 2d string array, size of 8,8. Each entry is taken from <paramref name="data"/>.
            /// </summary>
            /// <param name="data">The string to convert.</param>
            /// <returns>Returns a 8,8 array of strings.</returns>
            public static string[,] StringToMapConvertion(string data)
            { 
                string[] mapStringSeperated = data.Split('!'); 
                string[,] newMap = new string[8, 8];
                byte y = 0;
                for (int n = 0; n < mapStringSeperated.Length; n++)
                {
                    byte x = (byte)Math.Floor(n / 8d);
                    newMap[x, y] = mapStringSeperated[n]; 
                    y = y == 7 ? (byte)0 : (byte)(y + 1); 
                }
                return newMap;
            }

            /// <summary>
            /// Find any changes between <paramref name="newMap"/> and MapMatrix.Map and updates the chess pieces accordingly. 
            /// </summary>
            /// <param name="newMap">The map, 2d array of 8,8, used to compare to MapMatrix.Map.</param>
            /// <param name="team">The team of the player that has just moved. E.g. if white moves a piece it is true. If black, false.</param>
            public static void UpdatedChessPieces(string[,] newMap, bool team)
            {
                string[,] oldMap = new string[8, 8];
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                        oldMap[x, y] = MapMatrix.Map[x, y];
                
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        if (newMap[x, y] != oldMap[x, y]) 
                        {
                            string feltIDNew = newMap[x, y];
                            string feltIDOld = oldMap[x, y];
                            PawnPromotionCheck(feltIDNew, x, y);
                            if (feltIDNew != "" && feltIDOld == "")
                            {//a piece has moved to the, empty, location
                                if (feltIDNew.Split(':')[1] == "6") //pawn
                                {
                                    foreach (ChessPiece chePie in ChessList.GetList(!team)) //find the other player's piece that has moved.
                                    {
                                        if (chePie.GetID == feltIDNew)
                                        {
                                            if (chePie.GetMapLocation[0] != x) //en passant 
                                            {
                                                //find out the move direction and check the y in the other direction. 
                                                sbyte direction = (sbyte)(chePie.GetMapLocation[1] - y) > 0 ? (sbyte)1 : (sbyte)-1; //if positive, the pieces has moved up. If negative, it has moved down.
                                                string capturedPawn = oldMap[x, y + direction];
                                                foreach (ChessPiece chePieCaptured in ChessList.GetList(team)) //finds the player's piece that has been taken.
                                                {
                                                    if (chePieCaptured.GetID == capturedPawn)
                                                    {
                                                        chePieCaptured.NetworkUpdate(captured: true);
                                                        break;
                                                    }
                                                }
                                            }
                                            chePie.NetworkUpdate(new int[] { x, y });
                                            break;
                                        }
                                    }
                                } 
                                else if (feltIDNew.Split(':')[1] == "1") //king
                                {
                                    foreach (ChessPiece chePie in ChessList.GetList(!team))
                                    {
                                        if (chePie.GetID == feltIDNew)
                                        {
                                            if (Math.Abs(chePie.GetMapLocation[0] - x) > 1) //true only if the king castled.
                                            {
                                                //rewrite the comments to be better, spent a little to much time trying to remember why it is written as it is and what is happening...
                                                //Finds direction of the castling to determine the rock. 
                                                //Checking both ends of x on the y of the king. If one of those two ends do not have a rock on the new map and there was a rock on the old map, it must be the castling rock.
                                                string leftRock = newMap[0, chePie.GetMapLocation[1]];
                                                string rightRock = newMap[7, chePie.GetMapLocation[1]];
                                                string leftRockOld = oldMap[0, chePie.GetMapLocation[1]];
                                                string chosenRock = leftRock != leftRockOld ? leftRock : rightRock; //if true, chosenRock is left rock. Else, chosenRock is right rock.
                                                //either new locations, of the rooks, will be the same as their old locations if they have not castled.
                                                //this code can only be reached if the king has moved, and moved using castling, so neither of the rocks have been moved since the last change to the map.

                                                bool chosenRockDirection = leftRock != leftRockOld ? true : false; //if true, chosenRock go left. Else, chosenRock go right
                                                foreach (ChessPiece chePieRock in ChessList.GetList(!team))
                                                {
                                                    if (chePieRock.GetID == chosenRock)
                                                    {
                                                        sbyte location = chosenRockDirection ? (sbyte)1 : (sbyte)-1; //Left rock goes to the right side of the king. Right rock goes to left side of the king. 
                                                        chePieRock.NetworkUpdate(new int[] { x + location, y });
                                                        break;
                                                    }
                                                }
                                            }
                                            chePie.NetworkUpdate(new int[] { x, y });
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (ChessPiece chePie in ChessList.GetList(!team)) //find the other player's piece that has moved.
                                    {
                                        if (chePie.GetID == feltIDNew)
                                        {
                                            chePie.NetworkUpdate(new int[] { x, y });
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (feltIDOld != "" && feltIDNew != "" && feltIDOld != feltIDNew) 
                            {//a piece has been taken and a new stand on it or pawn promotion.

                                foreach (ChessPiece chePie in ChessList.GetList(team)) //find the player's piece that has been taken.
                                {
                                    if (chePie.GetID == feltIDOld)
                                    {
                                        chePie.NetworkUpdate(captured: true);
                                        break;
                                    }
                                }
                                foreach (ChessPiece chePie in ChessList.GetList(!team)) //find the other player's piece that has moved.
                                {
                                    if (chePie.GetID == feltIDNew)
                                    {
                                        chePie.NetworkUpdate(new int[] { x, y });
                                        break;
                                    }
                                }

                            }

                        }
                    }

                void PawnPromotionCheck(string feltIDNew, int x, int y)
                {
                    if (feltIDNew != "")
                        if (feltIDNew.Split(':')[2].ToCharArray().Length == 2)
                        {
                            foreach (ChessPiece chePie in ChessList.GetList(!team))
                            {
                                if (chePie.GetID.Split(':')[2] + "P" == feltIDNew.Split(':')[2] && chePie.CanBePromoted)
                                {//checks the last two parts to see if there is a pawn promotion

                                    byte[] colour = team ? Settings.BlackColour : Settings.WhiteColour; 
                                    string chessNumber = feltIDNew.Split(':')[1];
                                    chePie.NetworkUpdate(captured: true);
                                    if (chessNumber == "2")
                                    {
                                        ChessList.GetList(!team).Add(new Queen(colour, !team, new int[] { x, y }, feltIDNew, Publishers.PubKey, Publishers.PubCapture));
                                    }
                                    else if (chessNumber == "3")
                                    {
                                        ChessList.GetList(!team).Add(new Bishop(colour, !team, new int[] { x, y }, feltIDNew, Publishers.PubKey, Publishers.PubCapture));
                                    }
                                    else if (chessNumber == "4")
                                    {
                                        ChessList.GetList(!team).Add(new Knight(colour, !team, new int[] { x, y }, feltIDNew, Publishers.PubKey, Publishers.PubCapture));
                                    }
                                    else if (chessNumber == "5")
                                    {
                                        ChessList.GetList(!team).Add(new Rook(colour, !team, new int[] { x, y }, feltIDNew, Publishers.PubKey, Publishers.PubCapture));
                                    }
                                    break;
                                }
                            }
                        }
                }
            }

            public static void UpdateMap(string[,] map)
            {
                MapMatrix.Map = map;
            }
        }
    }

    /// <summary>
    /// The menu class. 
    /// </summary>
    class Menu
    {
        private static ConsoleKeyInfo key;
        private static bool isActive = false; 

        /// <summary>
        /// Sets up the Menu for use
        /// </summary>
        public Menu()
        {
            Settings.CVTS.ActivateCVTS();
            Console.CursorVisible = false;
            Publishers.SetKeyClass();
            Publishers.PubKey.RaiseKeyEvent += KeyEventHandler;
            Publishers.SetNetClass();
            Publishers.SetCaptureClass();
            NetworkUpdateReceiver networkUpdateReceiver = new NetworkUpdateReceiver(Publishers.PubNet);
        }

        /// <summary>
        /// Runs the menu.
        /// </summary>
        public void Run()
        {
            Thread controlSystem = new Thread(Publishers.PubKey.KeyPresser);
            controlSystem.Name = "Control Thread";
            controlSystem.Start();
            MainMenu();
        }

        /// <summary>
        /// Sets the window size to the values in the Settings.WindowSize or the max size if Settings.WindowSize is big.
        /// </summary>
        private void WindowSize()
        {
            int[] windowsSize = new int[2];
            windowsSize[0] = Settings.WindowSize[0] > Console.LargestWindowWidth ? Console.LargestWindowWidth : Settings.WindowSize[0];
            windowsSize[1] = 20 > Console.LargestWindowHeight ? Console.LargestWindowHeight : 20;
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);
        }

        /// <summary>
        /// Sets the window size to the values in the width/height or the max size if Settings.WindowSize is big.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window</param>
        private void WindowSize(byte width, byte height)
        {
            int[] windowsSize = new int[2];
            windowsSize[0] = width > Console.LargestWindowWidth ? Console.LargestWindowWidth : width;
            windowsSize[1] = height > Console.LargestWindowHeight ? Console.LargestWindowHeight : height;
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);
        }

        /// <summary>
        /// The main menu. 
        /// </summary>
        private void MainMenu()
        {
            string option;
            string title = "Main Menu";
            string[] options =
            {
                "Local Play",
                "Net Play",
                "Rules",
                "Interaction",
                "Settings",
                "Exit"
            };

            do
            {
                WindowSize();
                Console.Title = Settings.GetTitle + ": Main Menu";
                Console.Clear();
                option = Interact(options, title);
                switch (option)
                {
                    case "Local Play":
                        LocalPlayMenu();
                        break;

                    case "Rules":
                        RulesMenu();
                        break;

                    case "Interaction":
                        InteractionMenu();
                        break;

                    case "Exit":
                        Environment.Exit(0);
                        break;

                    case "Net Play": 
                        NetMenu();
                        break;

                    case "Settings":
                        SettingMenu();
                        break;
                }

            } while (true);
        }

        /// <summary>
        /// Settings menu. 
        /// </summary>
        private void SettingMenu()
        {
            bool run = true;
            string title = "Settings Menu";
            string[] options =
            {
                "Square Size",
                "Back"
            }; 
            Console.Title = Settings.GetTitle + ": " + title;
            do {
                string option = Interact(options, title);
                Console.Clear();
                switch (option)
                {
                    case "Square Size":
                            Settings.SetSquareSize = GetNewSize();
                            Settings.UpdateScreen();
                        break;

                    case "Back":
                        run = false;
                        break;
                }
            }while(run) ;

            byte GetNewSize()
            {
                byte value;
                string writtenValue = "";
                string[] text = String.Format("Enter a square size. Value is limited to minimum of 1 and maximum of 9. {1} Default is 5. Current value is {0}. {1} Enter to confirm value.", Settings.SquareSize, Environment.NewLine).Split(' ');
                string textOut = "";
                ushort length = 0;
                foreach(string word in text) //ensures the text does not go off the screen.
                {
                    length += (ushort)(word.Length + 1);
                    if (length < Settings.WindowSize[0] && word[0] != Environment.NewLine[0])
                        textOut += word + " ";
                    else if( word[0] != Environment.NewLine[0])
                    {
                        length = (ushort)(word.Length + 1);
                        textOut += Environment.NewLine + word + " ";
                    }
                    else if(word[0] == Environment.NewLine[0])
                    {
                        length = 0;
                        textOut += Environment.NewLine;
                    }
                }
                Console.WriteLine(textOut);
                Console.CursorVisible = true;
                int yLocation = Console.CursorTop;
                do
                {
                    writtenValue = Console.ReadLine();
                    Console.CursorTop = yLocation;
                    Console.Write("".PadLeft(Settings.WindowSize[0]));
                    Console.CursorLeft = 0;
                } while (!Byte.TryParse(writtenValue, out value));
                Console.CursorVisible = false;
                return value;
            }
        }

        /// <summary>
        /// Used for testing and practicing stuff.
        /// </summary>
        private void TestMenu()
        {
            //Console.Clear();
            //DelegateTest DT = new DelegateTest();

            //DT.WriteOut("Test");
            //DT.MathTest(2, 5, 1.1f, -5.2f, 5, 1, -10, 3);

            //double result = DT.MathCalculator(5, 2, 5, 1, 2, 5);
            //DT.WriteOut(DT.MathCalculator(5, -2, 1, 5, 2, -0.1).ToString());
            //DT.WriteOutEvent();
            //while (Console.ReadKey().Key != ConsoleKey.Enter) ;

        }

        /// <summary>
        /// The net menu. Allows for people to host, join and return the main menu.
        /// </summary>
        private void NetMenu()
        {
            Console.Title = Settings.GetTitle + ": Net Menu";
            string title = "Net Menu";
            string[] options = { "Host", "Join", "Back"};
            string option;

            do
            {
                Console.Clear();
                option = Interact(options,title);

                switch (option)
                {
                    case "Host":
                        Host();
                        break;

                    case "Join":
                        Join();
                        break;

                    case "Back":
                        break;

                    case "Test":
                        Console.Clear();
                        Abort(); //testing
                        break;
                }
            } while (option != options[2]);


            void Host() //ensure good comments in all of this net code and code related to the net
            {
                //gets and displays the IP address the joiner needs
                string ipAddress = Network.NetworkSupport.LocalAddress;
                Console.Clear();
                Console.CursorTop = Settings.MenuOffset[1];
                Console.WriteLine($"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}IP Address: {Settings.CVTS.BrightCyanForeground}{ipAddress}{Settings.CVTS.Reset}"); 
                Console.WriteLine($"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}Waiting on player{Settings.CVTS.Reset}");

                //starts up the reciver. 
                Network.Receive.ReceiveSetup(ipAddress); //need to catch if it fails
                Network.Receive.Start();
                GameStates.NetSearch.Searching = true;

                //gives the player the options to abort searching for a player. 
                Thread abortThread = new Thread(Abort);
                abortThread.Start();

                //waits on the joiner to connect to the receiver so their IP-address is arquired to ensure data can be transmitted to their receiver.  
                //Network.Receive.GetConnectionIpAddress() loops until Network.Transmit.TransmitSetup(ipAddress) in Join() has ran or GameStates.NetSeach.Abort is true.
                string ipAddressJoiner = Network.Receive.GetConnectionIpAddress();

                //if not aborted, run the rest of the set-up.
                if (!GameStates.NetSearch.Abort)
                {
                    Console.Clear();

                    Network.Transmit.OtherPlayerIpAddress = ipAddressJoiner; //transmitter now got the ip address needed to contact the receiver of the joiner

                    //select colour
                    string[] colourOptions = { "White", "Black" };
                    string colourTitle = "Select your colour";
                    string colour = null;
                    colour = Interact(colourOptions,colourTitle);
                    Console.CursorTop += 1;
                    Console.WriteLine($"{Settings.CVTS.BrightWhiteForeground}Conneting{Settings.CVTS.Reset}");

                    //transmit the colour
                    Network.Transmit.GeneralDataTransmission(colour, ipAddressJoiner);

                    //wait on response from the player joined to ensure they are ready.
                    string response = Network.Receive.GeneralDataReception();

                    //starts game, string colour parameters that decide whoes get the first turn.
                    if (response == "ready")
                    { //what if the response is not "ready"? It would mean there is some unalignment in the transmission and reception of data. So, firstly bugfixing. Else, return to the main menu
                        bool firstMove = colour == "White" ? true : false;
                        GameStates.PlayerTeam = firstMove;
                        Console.WriteLine($"{Settings.CVTS.BrightWhiteForeground}Game Starting{Settings.CVTS.Reset}");
                        Start(firstMove);
                    }
                    else
                    {
                        Console.WriteLine(response);
                        Console.ReadKey();
                    }
                }//if aborted, skip the rest
                else
                { 
                    Network.Receive.Stop();
                    GameStates.Reset();
                }
            }

            void Join()
            {
                string ipAddress;
                //ensure it is a proper address.
                //set up and start the receiver
                string ownIpAddress = Network.NetworkSupport.LocalAddress;
                Network.Receive.ReceiveSetup(ownIpAddress); 
                Network.Receive.Start();
                GameStates.NetSearch.Searching = true;
                do
                {
                    Console.Clear();
                    Console.CursorTop = Settings.MenuOffset[1];
                    Console.Write($"" +
                        $"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}Enter {Settings.CVTS.BrightCyanForeground}host IP address{Settings.CVTS.BrightWhiteForeground}.{Environment.NewLine}" +
                        $"{"".PadLeft(Settings.MenuOffset[0])}Press {Settings.CVTS.BrightRedForeground}Enter{Settings.CVTS.BrightWhiteForeground} to comfirm.{Environment.NewLine}" +
                        $"{"".PadLeft(Settings.MenuOffset[0])}If {Settings.CVTS.BrightCyanForeground}empty{Settings.CVTS.BrightWhiteForeground}, return to menu.{Environment.NewLine}" +
                        $"{"".PadLeft(Settings.MenuOffset[0])}Address:{Settings.CVTS.Reset} ");
                    Console.CursorVisible = true;
                    ipAddress = Console.ReadLine();
                    if (ipAddress == "")
                        GameStates.NetSearch.Abort = true;
                    Network.Transmit.OtherPlayerIpAddress = ipAddress;
                    Console.CursorVisible = false;
                } while (!GameStates.NetSearch.Abort && !Network.Transmit.TransmitSetup(ipAddress)); //starts up the transmitter to ensure the host' receiver can get the joiner' IP address and give it to host' transmitter. 

                if (!GameStates.NetSearch.Abort)
                {
                    //function is run to allows the host' transmitter to get the ip address of the client' receiver. Port is known in the code, just the IP-address that is missing... can be better explained...
                    Console.WriteLine($"{Settings.CVTS.BrightWhiteForeground}Connecting{Settings.CVTS.Reset}");

                    //gets the colour the host is using and selects the other
                    string colourHost = Network.Receive.GeneralDataReception();
                    string colour = colourHost == "White" ? "Black" : "White";

                    //send ready data.
                    Network.Transmit.GeneralDataTransmission("ready", ipAddress);

                    //starts game, string colour parameters that decide whoes get the first turn.
                    bool firstMove = colour == "White" ? true : false;
                    GameStates.PlayerTeam = firstMove;
                    Console.WriteLine($"{Settings.CVTS.BrightWhiteForeground}Game Starting{Settings.CVTS.Reset}");
                    Start(firstMove);
                }
                else
                {
                    Network.Receive.Stop();
                    GameStates.Reset();
                }

            }

            void Start(bool playerStarter)
            {
                Console.Clear();
                ChessTable chess = new ChessTable();
                chess.NetPlay(playerStarter);
            }

            void Abort()
            { //threated. Maybe not needed to be threaten, Network.Receive.GetConnectionIpAddress() should be threaten then
                GameStates.NetSearch.Searching = true; //just here for testning
                int[] cursorPosistion = new int[] { Console.CursorLeft, Console.CursorTop };
                Console.WriteLine($"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}Abort?{Settings.CVTS.Reset}");
                Console.WriteLine($"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightRedForeground}{Settings.CVTS.Underscore}Y{Settings.CVTS.BrightWhiteForeground}{Settings.CVTS.Underscore_Off}es{Settings.CVTS.Reset}");
                isActive = true;
                while (GameStates.NetSearch.Searching) //bool, true if searching for a player. False if a player is found. Store it in GameStates?
                { 
                    if((int)key.Key != 0 )
                    {
                        //ConsoleKeyInfo key = Console.ReadKey(true);
                        if(key.Key == ConsoleKey.Y)
                        {
                            GameStates.NetSearch.Searching = false;
                            GameStates.NetSearch.Abort = true;
                        }
                    }
                }
                isActive = false;
            }

        }

        /// <summary>
        /// Writes out all text stored in the Interaction.txt file.
        /// </summary>
        private void InteractionMenu()
        {
            Console.Title = Settings.GetTitle + ": Interaction";
            try
            {
                string[] interaction = File.ReadAllLines("Interaction.txt");
                PrintOut(interaction);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e); //this block is also entered if something else goes wrong, e.g. System.NullReferenceException and index size
                Console.WriteLine("Interaction.txt could not be found.");
                Console.WriteLine("{0}{1} to return.", Environment.NewLine, Settings.SelectKey);
            }
            PrintOutMenuExit();
        }

        private void PrintOutMenuExit()
        {
            isActive = true;
            while (key.Key != Settings.SelectKey) ;
            isActive = false;
            key = new ConsoleKeyInfo();
        }

        /// <summary>
        /// Writes out all text stored in the Rules.txt file.
        /// </summary>
        private void RulesMenu()
        {
            Console.Title = Settings.GetTitle + ": Rules";
            try
            {
                string[] about = File.ReadAllLines("Rules.txt");
                PrintOut(about);
                
            }
            catch
            {
                Console.WriteLine("Rules.txt could not be found.");
                Console.WriteLine("{0}{1} to return.", Environment.NewLine, Settings.SelectKey);
            }
            PrintOutMenuExit();
        }

        /// <summary>
        /// Writes out every string in <paramref name="writeOut"/> to the console.
        /// </summary>
        /// <param name="writeOut">String array to write out to the console.</param>
        /// <param name="keywords">String array of words to be coloured differently.</param>
        /// <param name="exitwords">String array of words to be coloured differently.</param>
        /// <param name="keyColour">Can be any foreground colour, except of the extented type, from Settings.CVTS</param>
        /// <param name="exitColour">Can be any foreground colour, except of the extented type, from Settings.CVTS</param>
        private void PrintOut(string[] writeOut, string keyColour = null, string exitColour = null)
        {
            Console.Clear();

            keyColour = keyColour == null ? Settings.CVTS.BrightCyanForeground : keyColour;
            exitColour = exitColour == null ? Settings.CVTS.BrightRedForeground : exitColour;
            sbyte lineNumber = -1;
            foreach (string str in writeOut)
            {
                lineNumber++;
                byte extraLines = 0;
                string[] words = str.Split(' ');
                string newStr = "";
                int stringLength = 0;
                int wordNumber = 0;

                foreach (string word in words)
                {
                    wordNumber++;
                    bool isKeyword = false;
                    bool isExitWord = false;
                    bool isOffset = false;

                    char[] wordChar = word.ToCharArray();
                    string addToString = null;
                    bool comma = false;
                    bool dot = false;
                    bool semicolon = false;
                    bool colon = false;
                    SpecialEndSign(word);

                    if (wordChar.Length > 2)
                    {
                        bool isCapitalised = false;
                        byte endPoint = comma || dot || semicolon || colon ? (byte)2 : (byte)1;
                        if (wordChar[0] == Settings.TextOffsetChar)
                        {
                            endPoint = comma || dot || semicolon || colon ? (byte)1 : (byte)0;
                            char[] function = new char[wordChar.Length - (endPoint+1)];
                            for (int i = 1; i < wordChar.Length - (endPoint); i++)
                                function[i - 1] = wordChar[i];
                            isCapitalised = (int)function[0] < 97 ? true : false;
                            if (!isCapitalised)
                                function[0] = (char)((int)function[0] - 32);
                            string findProperty = new string(function);
                            Type setting = typeof(Settings);
                            byte[] offset = (byte[])setting.GetProperty(findProperty).GetValue(setting);
                            addToString = "".PadLeft(offset[0]);
                            isOffset = true;
                        } 
                        else if (wordChar[0] == Settings.KeywordBrackets[0] && wordChar[wordChar.Length - endPoint] == Settings.KeywordBrackets[1]) 
                        {
                            char[] property = new char[wordChar.Length - (1 + endPoint)];
                            for (int i = 1; i < wordChar.Length - endPoint; i++)
                            {
                                property[i - 1] = wordChar[i];
                            }
                            isCapitalised = (int)property[0] < 97 ? true : false;
                            if (!isCapitalised)
                                property[0] = (char)((int)property[0] - 32);
                            string findProperty = new string(property);
                            Type setting = typeof(Settings);
                            //Settings setting = new Settings(true);
                            //addToString = setting.GetType().GetProperty(find).GetValue(setting).ToString();
                            try
                            {

                                if (setting.GetProperty(findProperty).PropertyType.Name == "ConsoleKey")
                                    addToString = !isCapitalised ? setting.GetProperty(findProperty).GetValue(setting).ToString().ToLower() : setting.GetProperty(findProperty).GetValue(setting).ToString();
                                //else if(setting.GetProperty(findProperty).PropertyType.Name == "Byte[]")
                                //{
                                //    byte[] offset = (byte[])setting.GetProperty(findProperty).GetValue(setting);
                                //    addToString = "".PadLeft(offset[0] - 1);
                                //}
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e);
                                addToString = "erorr";
                            }
                            isExitWord = true;
                        }
                        else if (wordChar[0] == Settings.ExitWordBrackets[0] && wordChar[wordChar.Length - endPoint] == Settings.ExitWordBrackets[1])
                        {
                            char[] keyWord = new char[wordChar.Length - (1 + endPoint)];
                            for (int i = 1; i < wordChar.Length - endPoint; i++)
                            {
                                keyWord[i - 1] = wordChar[i];
                            }
                            addToString = new string(keyWord);
                            isKeyword = true;
                        }

                    }
                    if (addToString == null)
                        addToString = word;

                    if (stringLength + addToString.Length + 1 >= Console.WindowWidth && wordNumber != words.Length)
                    {
                        stringLength = 0 + "".PadLeft(Settings.TextOffset[0]).Length;
                        newStr += Environment.NewLine + "".PadLeft(Settings.TextOffset[0]);
                        lineNumber++;
                        extraLines++;
                    }

                    if (!isKeyword && !isExitWord)
                        newStr += Settings.CVTS.BrightWhiteForeground + addToString + Settings.CVTS.Reset;
                    else if (isKeyword)
                        newStr += keyColour + addToString + Settings.CVTS.Reset;
                    else if (isExitWord)
                        newStr += exitColour + addToString + Settings.CVTS.Reset;
                    
                    if (dot && (isKeyword || isExitWord))
                    {
                        newStr += ".";
                        stringLength++;
                    }
                    else if (comma && (isKeyword || isExitWord))
                    {
                        newStr += ",";
                        stringLength++;
                    }
                    else if (semicolon && (isKeyword || isExitWord))
                    {
                        newStr += ";";
                        stringLength++;
                    }
                    else if (colon && (isKeyword || isExitWord))
                    {
                        newStr += ":";
                        stringLength++;
                    }

                    if (!isOffset)
                    {
                        newStr += " ";
                        stringLength += addToString.Length + 1;
                    }
                    //Debug.WriteLine("String Width (word added): " + stringLength);
                    void SpecialEndSign(string wordToCheck)
                    {

                            comma = wordToCheck.EndsWith(',');
                            dot = wordToCheck.EndsWith('.');
                            semicolon = wordToCheck.EndsWith(';');
                            colon = wordToCheck.EndsWith(':');

                    }
                }
                Console.CursorTop = lineNumber + Settings.MenuTitleOffset[1] - extraLines;
                Console.CursorLeft = 0;
                //https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/reflection
                //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection
                Console.Write($"{newStr}");


            }
            Console.SetCursorPosition(0, 0);
            if(lineNumber+1 > Console.WindowHeight)
                WindowSize((byte)Console.WindowWidth, (byte)(lineNumber+1));
        }

        /// <summary>
        /// The local play menu.
        /// </summary>
        private void LocalPlayMenu()
        {
            Console.Clear();
            ChessTable chess = new ChessTable();
            chess.LocalPlay();
        }

        /// <summary>
        /// Allows other classes to use to menu interaction and displaying. 
        /// </summary>
        /// <param name="options">String array of options.</param>
        /// <returns>Returns the selected option.</returns>
        public static string MenuAccess(string[] options, string title = null)
        {
            return Interact(options,title);
        }

        /// <summary>
        /// Allows for the select of an index in <paramref name="options"/>. This function will also call the <c>Display</c> function.
        /// </summary>
        /// <param name="options">String array of options.</param>
        /// <returns>Returns the selected option.</returns>
        private static string Interact(string[] options, string title = null)
        { //used to move around in the displayed options. All it should do is being a function that checks if up/down key arrows are pressed and then 
            //increase or decrease a variable used for the hoveredOverOption in Display().
            bool selected = false;
            byte currentLocation = 0;
            string answer = null;
            isActive = true;
            Console.Clear();
            do
            {
                Display(options, currentLocation, Settings.MenuColour, Settings.MenuColourHovered, Settings.MenuOffset, title, Settings.MenuColourTitle, Settings.MenuTitleOffset);
                Console.CursorTop = 0;
                Console.CursorLeft = 0;
                if (Move())
                {
                    answer = options[currentLocation];
                    selected = true;
                }
                key = new ConsoleKeyInfo();
            } while (!selected);
            isActive = false;
            return answer;

            bool Move()
            {
                //ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                while ((int)key.Key == 0) ;
                if (key.Key == Settings.UpKey && currentLocation > 0)
                {
                    currentLocation--;
                }
                else if (key.Key == Settings.DownKey && currentLocation < options.Length - 1)
                {
                    currentLocation++;
                }
                else if (key.Key == Settings.SelectKey)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Displays the <paramref name="options"/> in the console in the colours given by <paramref name="optionColours"/> and <paramref name="hoveredColour"/>. 
        /// </summary>
        /// <param name="options">String list. Each entry is considered an option.</param>
        /// <param name="hoveredOverOption">Indicate which option is the currently hovered over.</param>
        /// <param name="optionColours">The default colour of the options.</param>
        /// <param name="hoveredColour">The colour of the hovered over option.</param>
        /// <param name="offset">Offset to the top left corner.</param>
        private static void Display(string[] options, byte hoveredOverOption, byte[] optionColours, byte[] hoveredColour, byte[] offset, string title = null, byte[] titleColour = null, byte[] titleOffset = null)
        {
            byte yOffSetSupport = 0;
            if(title != null)
            {
                //Debug.WriteLine($"Title should be {title}");
                if (titleOffset == null)
                {
                    yOffSetSupport++;
                    Console.SetCursorPosition(offset[0], offset[1]);
                }
                else
                    Console.SetCursorPosition(titleOffset[0], titleOffset[1]);
                if (titleColour == null)
                    Paint(title, optionColours);
                else
                    Paint(title, titleColour);
            }

            for (int i = 0; i < options.Length; i++)
            {
                Console.SetCursorPosition(offset[0], offset[1] + i + yOffSetSupport);
                if (i == hoveredOverOption)
                {
                    Paint(options[i], hoveredColour);
                }
                else
                {
                    Paint(options[i], optionColours);
                }
            }

            void Paint(string option, byte[] colour)
            {
                //Debug.WriteLine($"Printed out: {option}");
                Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m{0}" + Settings.CVTS.Reset, option);
                //Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// If the menu is the active one it will sets its <c>key</c> to <c>e.Key.key</c>.
        /// </summary>
        /// <param name="sender">The object that invoked the event.</param>
        /// <param name="e">The parameter containing the variables and their values of ControlEvents.</param>
        protected void KeyEventHandler(object sender, ControlEvents.KeyEventArgs e)
        {
            if (isActive)
                key = e.Key;
        }

    }

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
            if(e.GameEnded == true)
            {
                GameStates.WhiteWin = e.WhiteWon != null ? (bool)e.WhiteWon : GameStates.WhiteWin;
                GameStates.Won = e.Won;
                GameStates.OtherPlayerSurrendered = e.OtherPlayerSurrendered != null ? (bool)e.OtherPlayerSurrendered : GameStates.OtherPlayerSurrendered;
                GameStates.GameEnded = e.GameEnded != null ? (bool)e.GameEnded : GameStates.GameEnded;
            }
        }
    }

    /// <summary>
    /// Class for creating and displaying the chess table and creating the chess pieces.
    /// </summary>
    class ChessTable
    {
        private Player white; 
        private Player black; 
        //private int[,] whiteSpawnLocation;
        //private int[,] blackSpawnLocation;
        private int[] windowsSize = new int[2];

        public ChessTable()
        {
            MapMatrix.PrepareMap();
            Settings.CanHighLight = Settings.SquareSize > Settings.ChesspieceDesignSize + 1 ? true : false; 
            windowsSize[0] = Settings.WindowSize[0] > Console.LargestWindowWidth ? Console.LargestWindowWidth : Settings.WindowSize[0];
            windowsSize[1] = Settings.WindowSize[1] > Console.LargestWindowHeight ? Console.LargestWindowHeight : Settings.WindowSize[1];
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);

            int[,] blackSpawnLocation = new int[,] {
                { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 },
                { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }
            };
            int[,] whiteSpawnLocation = new int[,] {
                { 0, 6 }, { 1, 6 }, { 2, 6 }, { 3, 6 }, { 4, 6 }, { 5, 6 }, { 6, 6 }, { 7, 6 },
                { 0, 7 }, { 1, 7 }, { 2, 7 }, { 3, 7 }, { 4, 7 }, { 5, 7 }, { 6, 7 }, { 7, 7 }
            };
            BoardSetup();
            CreatePieces(true, whiteSpawnLocation, Settings.WhiteColour);
            CreatePieces(false, blackSpawnLocation, Settings.BlackColour);
            PlayerSetup();
        }

        /// <summary>
        /// Repaints the board and all of the chess pieces.
        /// </summary>
        public static void RepaintBoardAndPieces()
        {
            bool kingcheck;
            BoardSetup();
            foreach (ChessPiece chePie in ChessList.GetList(true)) 
            {
                chePie.IsHoveredOn(false); //this function is not meant for this, but it works regarding replainting the pieces. 
                if (chePie is King king)
                    kingcheck = king.SpecialBool; //called to get the king, if threaten, to write out the location it is treaten from
            }
            foreach (ChessPiece chePie in ChessList.GetList(!true))
            {
                chePie.IsHoveredOn(false);
                if (chePie is King king)
                    kingcheck = king.SpecialBool; 
            }
        }

        /// <summary>
        /// Sets up the board and paint the outlines. 
        /// </summary>
        private static void BoardSetup()
        {//8 squares in each direction. Each piece is 3*3 currently, each square is 5*5 currently. 
            Console.CursorVisible = false;
            ushort distance = (ushort)(9 + 8 * Settings.SquareSize);
            string[] letters = { "a", "b", "c", "d", "e", "f", "g", "h" };
            string[] numbers = { "1", "2", "3", "4", "5", "6", "7", "8" };
            byte alignment = (byte)Math.Ceiling(Settings.SquareSize / 2f);

            //top left corner
            Console.SetCursorPosition(Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m" 
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Corner_TopLeft + Settings.CVTS.DEC.DEC_Deactive}");
            
            //top right corner 
            Console.SetCursorPosition(distance - 1 + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Corner_TopRight + Settings.CVTS.DEC.DEC_Deactive}");
            
            //bottom left corner
            Console.SetCursorPosition(Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, distance - 1 + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Corner_BottomLeft + Settings.CVTS.DEC.DEC_Deactive}");
            
            //bottom right corner 
            Console.SetCursorPosition(distance - 1 + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, distance - 1 + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Corner_BottomRight + Settings.CVTS.DEC.DEC_Deactive}");


            for (int k = 1; k < distance-1; k++) //vertical lines
                for (int i = 0; i < distance; i += 1 + Settings.SquareSize)
                {

                    float intersectX = (float)Math.Floor(i / (double)(Settings.SquareSize+Settings.Spacing));
                    float intersectY = k % (float)(Settings.SquareSize + Settings.Spacing);
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, k + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m "); //background colour
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, k + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
                    if(intersectY != 0) //no intersection at all
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Vertical_Line + Settings.CVTS.DEC.DEC_Deactive}");
                    else if (intersectY == 0 && intersectX == 0) //intersection on the left side
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Intersection_Left + Settings.CVTS.DEC.DEC_Deactive}");
                    else //intersection at the right side. 
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Intersection_Right + Settings.CVTS.DEC.DEC_Deactive}");

                }
            for (int k = 0; k < distance; k += 1 + Settings.SquareSize) //horizontal lines
                for (int i = 1; i < distance - 1; i++)
                {
                    float intersectX = i % (float)(Settings.SquareSize + Settings.Spacing);
                    float intersectY = (float)Math.Floor(k / (double)(Settings.SquareSize + Settings.Spacing));
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, k + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m "); //background colour
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, k + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
                    if(intersectX != 0) //no intersection at all
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Horizontal_Line + Settings.CVTS.DEC.DEC_Deactive}");
                    else if (intersectX == 0 && intersectY == 0) //intersection at the top
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Intersection_Top + Settings.CVTS.DEC.DEC_Deactive}");
                    else if (intersectX == 0 && intersectY == 8) //intersection at the bottom
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Intersection_Bottom + Settings.CVTS.DEC.DEC_Deactive}");
                    else //intersection all other places.
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Intersection_Full + Settings.CVTS.DEC.DEC_Deactive}");


                }

            for (int k = 0; k < numbers.Length; k++) //numbers
            { //the 1s in this and below for loop should be a setting, it is the amount of empty squares between the numbers/letters and the board edge
                Console.SetCursorPosition(Settings.Offset[0] - Settings.EdgeSpacing - 1 + Settings.Spacing, k + Settings.EdgeSpacing + Settings.Spacing + Settings.Offset[1] + alignment + (Settings.SquareSize * k)-1);
                Console.Write(numbers[7-k]); //- Settings.EdgeSpacing should not be used in the calculation above.
                Console.SetCursorPosition(Settings.Offset[0] + distance + Settings.EdgeSpacing + Settings.Spacing, k + Settings.Spacing + Settings.EdgeSpacing + Settings.Offset[1] + alignment + (Settings.SquareSize * k)-1);
                Console.Write(numbers[7-k]);
            }

            for (int k = 0; k < letters.Length; k++) //letters
            {
                Console.SetCursorPosition(k + Settings.Spacing + Settings.Offset[0] + alignment + (Settings.SquareSize * k), Settings.Offset[1] + Settings.Spacing - Settings.EdgeSpacing - 1);
                Console.Write(letters[k]);
                Console.SetCursorPosition(k + Settings.Spacing + Settings.Offset[0] + alignment + (Settings.SquareSize * k), Settings.Offset[1] + distance + Settings.Spacing + Settings.EdgeSpacing);
                Console.Write(letters[k]);
            }

            BoardColouring();
            KingWriteLocationSetup();
        }

        /// <summary>
        /// Colour the board. 
        /// </summary>
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

        /// <summary>
        /// Setup the location the two kings are going to write to, if they are under threat. 
        /// </summary>
        private static void KingWriteLocationSetup()
        {
            int[] white = new int[] { Settings.CheckHeaderLocation[0, 0], Settings.CheckHeaderLocation[0, 1] };
            int[] black = new int[] { Settings.CheckHeaderLocation[1, 0], Settings.CheckHeaderLocation[1, 1] };
            Write("White", white);
            Write("Black", black);

            void Write(string colour, int[] location)
            {
                Console.SetCursorPosition(location[0], location[1]);
                Console.Write(colour);
                Console.SetCursorPosition(location[0], location[1] + 1);
                Console.Write("King");
            }
        }


        /// <summary>
        /// Function that checks if the game has reached a draw. It wil lalso update the draw turn counter.
        /// </summary>
        /// <returns>Returns true if the game is in a draw, else false.</returns>
        private bool Draw(bool newMove = false, bool updatePieces = false)
        {

            bool noCapture;
            bool pawnChange = false;
            if (ChessList.GetList(true).Count == 1 && ChessList.GetList(false).Count == 1)
                return true;

            if (newMove) 
                if(MapMatrix.LastMoveMap[0, 0] != null)
                    for (int n = 0; n < 8; n++)
                    {
                        for (int m = 0; m < 8; m++)
                        { //check if any pawns has been changed. 
                            string newFelt = MapMatrix.Map[m, n];
                            string oldFelt = MapMatrix.LastMoveMap[m, n];
                            if(newFelt != oldFelt)
                                if(oldFelt != "")
                                    if (oldFelt.Split(':')[1] == "6")
                                        pawnChange = true;
                        }
                    }

            if (updatePieces)
            {
                if (GameStates.PieceAmount[0, 0] == GameStates.PieceAmount[0, 1] && GameStates.PieceAmount[1, 0] == GameStates.PieceAmount[1, 1])
                    noCapture = true;
                else
                    noCapture = false;

                if (noCapture && !pawnChange)
                    GameStates.TurnDrawCounter++;
                else
                    GameStates.TurnDrawCounter = 0;

                if (GameStates.TurnDrawCounter == 70)
                    return true;
            }
            return false;
        }

        private void EndScreen()
        {
            Console.Title = Settings.GetTitle + ": Game Ended";
            string endMessage = null;
            if (!GameStates.LostConnection)
            {
                if (GameStates.Won == null)
                    endMessage = "The Game Ended in a Draw";
                else
                {
                    string firstPart = GameStates.WhiteWin ? "White" : "Black";
                    endMessage = $"{firstPart} Player Won";
                }
            }
            else
            {
                endMessage = "The Connection was lost";
            }
            Console.CursorTop = Settings.MenuOffset[1];
            Console.WriteLine($"{"".PadLeft(Settings.MenuTitleOffset[0])}{Settings.CVTS.BrightWhiteForeground}{Settings.CVTS.Underscore}Game Finished{Settings.CVTS.Underscore_Off}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}{endMessage} {Settings.CVTS.Reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}Amount of Moves: {Settings.CVTS.BrightCyanForeground}{Settings.CVTS.Underscore}{GameStates.TurnCounter}{Settings.CVTS.Reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightRedForeground}Enter{Settings.CVTS.BrightWhiteForeground} to continue. {Settings.CVTS.Reset}");
            while (Console.ReadKey(true).Key != Settings.SelectKey) ;
            Console.Clear();
        }

        /// <summary>
        /// Sets the title of the program while a game is going on.
        /// </summary>
        static public void GameRunTitle()
        {
            Console.Title = Settings.GetTitle + ": Game";
        }

        /// <summary>
        /// Runs the online game loop. <paramref name="starter"/> decides if the player makes the first move or not.
        /// </summary>
        /// <param name="starter">True for first move, false for second move.</param>
        private void GameLoopNet(bool starter)
        {
            GameRunTitle();
            bool whiteTeam = starter;
            bool firstTurn = starter;

            Thread receiveThread = new Thread(Network.Receive.ReceiveGameLoop);
            receiveThread.Name = "Receiver Thread";
            receiveThread.Start(starter);
            Thread connectionThread = new Thread(Network.Transmit.StillConnected);
            connectionThread.Name = "Connection Checker Thread";
            connectionThread.Start(Network.Transmit.OtherPlayerIpAddress);

            GameStates.IsTurn = starter;
            GameStates.IsOnline = true;
            GameStates.PieceAmount[0, 0] = (byte)ChessList.GetList(true).Count;
            GameStates.PieceAmount[0, 1] = (byte)ChessList.GetList(true).Count;
            GameStates.PieceAmount[1, 0] = (byte)ChessList.GetList(false).Count;
            GameStates.PieceAmount[1, 1] = (byte)ChessList.GetList(false).Count;

            if (MapMatrix.LastMoveMap[0, 0] == null)
                MapMatrix.UpdateOldMap();
            //game loop
            do
            {
                //byte counter = 0;
                //bool connectionExist;
                if (GameStates.IsTurn) //only true when Network.Receive.ReceiveMapData has received map data from the other player' transmitter
                {
                    GameStates.GameEnded = PlayerControlNet(starter);
                    GameStates.IsTurn = false;
                    if (GameStates.GameEnded && GameStates.LostConnection == false) 
                    { //transmit a signal to the other player' receiver to let them know the game has ended. 
                        if (GameStates.Won == null && !GameStates.OtherPlayerSurrendered)
                            Network.Transmit.GeneralValueTransmission(6, Network.Transmit.OtherPlayerIpAddress); //Draw  //5 is loss for the other player, 4 is victory for the other player. 
                        else if (GameStates.Won == true && !GameStates.OtherPlayerSurrendered)
                            Network.Transmit.GeneralValueTransmission(5, Network.Transmit.OtherPlayerIpAddress); //other player lost
                        else if (GameStates.Won == false && !GameStates.OtherPlayerSurrendered)
                            Network.Transmit.GeneralValueTransmission(4, Network.Transmit.OtherPlayerIpAddress); //other player won
                        //Network.Receive.Stop();
                    }
                }
                else //not this computer's turn to move. 
                {
                    Player waitingPlayer = starter ? white : black;
                    waitingPlayer.ControlOnlyMenu();
                }

            } while (!GameStates.GameEnded);
            ChessList.RemoveAllPieces();
            Network.Receive.Stop();
            Console.Clear();
            EndScreen();
            GameStates.Reset();
            MapMatrix.AllowForMapPreparation();

            bool PlayerControlNet(bool team)
            {
                bool? checkmate = false; bool draw = false; bool? otherPlayerCheckMate = false;
                
                GameStates.TurnCounter++;

                for (int i = ChessList.GetList(team).Count - 1; i >= 0; i--) //removes it own, captured, pieces. Needs to be called before player.Control else the default hover overed piece might be one of the captured. 
                {
                    if (ChessList.GetList(team)[i].BeenCaptured)
                        ChessList.GetList(team).RemoveAt(i);
                }

                Player player;
                if (team)
                    player = white;
                else
                    player = black;

                ProtectKing.ProtectEndLocations.Clear();
                ProtectKing.ProtectingTheKing.Clear();
                ProtectKing.CannotMove = ProtectingTheKing(team); 
                
                if(team)
                    PieceAmountUpdate(); //draw counters need to be updated before white player turn.

                if(!firstTurn)
                Draw(true, updatePieces: team); //a "move", after the rules, consist of both players turn. This call ensures the draw counter is updated before white player's turn
                if (team) 
                    MapMatrix.UpdateOldMap();
                firstTurn = false;

                otherPlayerCheckMate = IsKingChecked(!team); //updates whether the other player' king is under threat.
                checkmate = CheckmateChecker(team, out List<string> saveKingList); 

                ProtectKing.Protect = saveKingList;

                if(checkmate != null)
                    if(!(bool)checkmate) //if the king is not checkmate, play
                        player.Control();

                otherPlayerCheckMate = CheckmateChecker(!team); //these two updates the write locations
                checkmate = IsKingChecked(team); //these two updates the write locations

                for (int i = ChessList.GetList(team).Count - 1; i >= 0; i--) //ensures any promoted pawns are removed from the list.
                {
                    if (ChessList.GetList(team)[i].BeenCaptured)
                    {
                        ChessList.GetList(team).RemoveAt(i);
                        break;
                    }
                }
                for (int i = ChessList.GetList(!team).Count - 1; i >= 0; i--) //removes captured pieces.
                {
                    if (ChessList.GetList(!team)[i].BeenCaptured)
                    {
                        ChessList.GetList(!team).RemoveAt(i);
                        break;
                    }
                }

                for (int i = ChessList.GetList(!team).Count - 1; i >= 0; i--)
                {
                    if (ChessList.GetList(!team)[i] is Pawn && ChessList.GetList(!team)[i].SpecialBool == true)
                    { //Prevents en passant from happening over multiple moves. 
                        ChessList.GetList(!team)[i].SpecialBool = false;
                        break;
                    }
                }

                if(!team)
                    PieceAmountUpdate(); //needs to be updated after black player turn.

                if (!GameStates.GameEnded)
                    draw = Draw(false, updatePieces: !team); //ensures that draw 50 move counter is updated after black player turn. Auto-draw from the fifty-move rule can only happen at the end of black player turn
                if (!team)
                    MapMatrix.UpdateOldMap();
                if (checkmate == true || draw || otherPlayerCheckMate == true)
                {
                    if (draw)
                        GameStates.Won = null;
                    else if (checkmate == true) //this one would never be true as otherPlayerCheckMate will be true before this one can be true. 
                        GameStates.Won = false;
                    else if (otherPlayerCheckMate == true)
                        GameStates.Won = true;
                    return true;
                }
                if (!GameStates.GameEnded)
                {

                    bool successTransmission = Network.Transmit.TransmitMapData(Network.Transmit.OtherPlayerIpAddress);
                    if (!successTransmission) //if it fails to transmit data end the game
                        return true;
                }
                else
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Updates the GameStates.PieceAmount.
        /// </summary>
        private void PieceAmountUpdate()
        {
            GameStates.PieceAmount[1, 1] = GameStates.PieceAmount[1, 0];
            GameStates.PieceAmount[1, 0] = (byte)ChessList.GetList(false).Count;
            GameStates.PieceAmount[0, 1] = GameStates.PieceAmount[0, 0];
            GameStates.PieceAmount[0, 0] = (byte)ChessList.GetList(true).Count;
        }

        /// <summary>
        /// Contains the code that allows the game to be played and loops through it until either a draw or one side wins.
        /// </summary>
        private void GameLoop()
        {
            GameRunTitle();
            bool gameEnded = false;
            GameStates.IsOnline = false;
            GameStates.PieceAmount[0, 0] = (byte)ChessList.GetList(true).Count;
            GameStates.PieceAmount[0, 1] = (byte)ChessList.GetList(true).Count;
            GameStates.PieceAmount[1, 0] = (byte)ChessList.GetList(false).Count;
            GameStates.PieceAmount[1, 1] = (byte)ChessList.GetList(false).Count;

            do
            {
                gameEnded = PlayerControl(true);
                if (gameEnded || GameStates.GameEnded)
                {
                    GameStates.GameEnded = true;
                    break;
                }
                gameEnded = PlayerControl(false);
                if (gameEnded || GameStates.GameEnded)
                    GameStates.GameEnded = true;
            } while (!GameStates.GameEnded);
            ChessList.RemoveAllPieces();
            Console.Clear();
            EndScreen();
            GameStates.Reset();
            MapMatrix.AllowForMapPreparation();

            unsafe bool PlayerControl(bool team)
            {
                if (team)
                {
                    MapMatrix.UpdateOldMap();
                    GameStates.TurnCounter++;
                }
                bool checkmate = false; bool draw = false;
                Player player;
                if (team)
                    player = white;
                else
                    player = black;

                ProtectKing.CannotMove = ProtectingTheKing(team); 

                player.Control();
                IsKingChecked(team);
                ProtectKing.ProtectEndLocations.Clear();
                ProtectKing.ProtectingTheKing.Clear();


                for (int i = ChessList.GetList(team).Count - 1; i >= 0; i--) //ensures any promoted pawns are removed from the list.
                {
                    if (ChessList.GetList(team)[i].BeenCaptured)
                        ChessList.GetList(team).RemoveAt(i);
                }
                for (int i = ChessList.GetList(!team).Count - 1; i >= 0; i--) //removes any captured hostile pieces. 
                {
                    if (ChessList.GetList(!team)[i].BeenCaptured)
                        ChessList.GetList(!team).RemoveAt(i);
                }
                for (int i = ChessList.GetList(!team).Count - 1; i >= 0; i--) //ensures that en passant is set to false if it is true.
                {
                    if (ChessList.GetList(!team)[i] is Pawn && ChessList.GetList(!team)[i].SpecialBool == true)
                        ChessList.GetList(!team)[i].SpecialBool = false;
                }
                PieceAmountUpdate();

                checkmate = CheckmateChecker(!team, out List<string> saveKingList);
                ProtectKing.Protect = saveKingList;

                if (checkmate)
                {
                    GameStates.WhiteWin = team;
                    GameStates.Won = true;
                    return true;
                }

                draw = Draw(team, team);
                if (draw)
                {
                    GameStates.Won = null;
                    return true;
                }
                return false;

            }
        }

        /// <summary>
        /// Calls the function that runs the loop and code that plays the game online.
        /// </summary>
        public void NetPlay(bool playerStarter)
        {
            GameLoopNet(playerStarter);
        }

        /// <summary>
        /// Calls the function that runs the loop and code that plays the game offline.
        /// </summary>
        public void LocalPlay()
        {
            GameLoop();
        }

        /// <summary>
        /// Check if a king, colour depending on <paramref name="team"/>, is checked or not. If the king is checked, it will return true. Else false. 
        /// If null is returned, it means there is no king.
        /// </summary>
        /// <param name="team">True for white, false for black.</param>
        /// <returns>True if the king is checked, else false. If null is returned it means there is no king.</returns>
        private bool? IsKingChecked(bool team)
        {
            foreach (ChessPiece chePie in ChessList.GetList(team))
            {
                if (chePie is King)
                {
                    return chePie.SpecialBool;
                }
            }
            return null; //if this is reached, it would mean there is no king and something has gone very wrong. 
        }

        /// <summary>
        /// Checks if there are any pieces that are protecting the king.
        /// If there is none, the returned list will be empty.
        /// </summary>
        /// <param name="team">The team that should be checked. True for white, false for black.</param>
        /// <returns></returns>
        private List<string> ProtectingTheKing(bool team)
        {
            int[] kingLocation = new int[2];
            int[][] directions = new int[][]
            { 
                new int[] {0,1 }, 
                new int[] {0,-1 }, 
                new int[] {1,0 }, 
                new int[] {-1,0 }, 
                new int[] {1,1 }, 
                new int[] {1,-1 }, 
                new int[] {-1,1 }, 
                new int[] {-1,-1 } 
            };
            List<string> protectingPieces = new List<string>();
            foreach (ChessPiece chePie in ChessList.GetList(team))
            {
                if (chePie is King king)
                {
                    kingLocation = king.GetMapLocation;
                    break;
                }
            }
            foreach (ChessPiece chePie in ChessList.GetList(team))
            {
                if(chePie is Pawn pawn)
                {
                    PawnProtecting(pawn);
                }
                else if(chePie.GetID.Split(':')[1] != "1")
                {
                    Protecting(chePie);
                }
            }
            return protectingPieces;

            void PawnProtecting(Pawn pawn)
            {
                int[] hostileLocation = new int[2];
                int[] scaledDifferences = new int[2];
                int[] hostileDiffernece = new int[2];
                int[] differences = new int[] { pawn.GetMapLocation[0] - kingLocation[0], pawn.GetMapLocation[1] - kingLocation[1] };
                if (differences[0] != 0 && (differences[0] == differences[1] || differences[0] == -differences[1]))
                    scaledDifferences[0] = differences[0] > 0 ? 1 : -1; //scales it to be 1 or -1
                else if (differences[0] == 0)
                    scaledDifferences[1] = differences[1] > 0 ? 1 : -1;
                if (differences[1] != 0 && (differences[0] == differences[1] || differences[0] == -differences[1]))
                    scaledDifferences[1] = differences[1] > 0 ? 1 : -1;
                else if (differences[1] == 0)
                    scaledDifferences[0] = differences[0] > 0 ? 1 : -1;
                bool canReachKing = false;
                int[] mov = new int[2];
                foreach (int[] move in directions)
                {
                    if (move[0] == scaledDifferences[0] && move[1] == scaledDifferences[1])
                    {
                        mov = move;
                        canReachKing = true;
                        break;
                    }
                }
                if (canReachKing)
                {

                    List<string> toCheckFor = new List<string>();
                    if (mov[0] != 0 && mov[1] != 0)
                    {
                        toCheckFor.Add("2");
                        toCheckFor.Add("3");
                    }
                    else
                    {
                        toCheckFor.Add("2");
                        toCheckFor.Add("5");
                    }
                    bool keepKingSafe = false;
                    //check all squres in both directions of mov (-mov) 
                    if (PawnFeltCheck(toCheckFor)) 
                    {
                        keepKingSafe = PawnFeltCheck(new List<string>() {"1"}, true); //need to first check the other direction
                        hostileDiffernece = new int[] { hostileLocation[0] - pawn.GetMapLocation[0], hostileLocation[1] - pawn.GetMapLocation[1]};
                        if(keepKingSafe)
                            protectingPieces.Add(pawn.GetID);
                        //if it is true, remove the (..., true) and just add it to the list in this if-statement.
                    }

                    if (mov[0] == 0 && keepKingSafe) //this means that the king is either above or below the pawn and thus can move. 
                    {
                        List<int[,]> canStandOn = new List<int[,]>();
                        int dir = pawn.GetDirection[0][1];
                        bool cantDouble = pawn.HasMoved; 
                        byte maxRange = !cantDouble ? (byte)2 : (byte)1;
                        byte pos = 0;
                        do
                        {
                            pos++;
                            if (MapMatrix.Map[pawn.GetMapLocation[0], pawn.GetMapLocation[1] + (dir * pos)] == "")
                            {
                                canStandOn.Add(new int[,] { { pawn.GetMapLocation[0], pawn.GetMapLocation[1] + (dir * pos) } });
                            }
                            else
                                break;
                        } while (pos < maxRange);
                        //if (canStandOn.Count != 0)
                            ProtectKing.ProtectingTheKing.Add(pawn.GetID, canStandOn);
                    }
                    else if (keepKingSafe && mov[0] == hostileDiffernece[0] && mov[1] == hostileDiffernece[1]) //can capture the hostile
                    {
                        ProtectKing.ProtectingTheKing.Add(pawn.GetID, new List<int[,]>() { new int[,] { { hostileLocation[0], hostileLocation[1] } } });
                    }
                    else if (keepKingSafe) //cannot move
                        ProtectKing.ProtectingTheKing.Add(pawn.GetID, new List<int[,]>());

                }
                bool PawnFeltCheck(List<string> toCheckFor, bool direction = false)
                {
                    int[] loc_ = new int[] { pawn.GetMapLocation[0], pawn.GetMapLocation[1] };
                    bool foundPiece = false;
                    bool shouldCheck = true;
                    string teamSignToCheck;
                    sbyte dir = direction ? (sbyte)-1 : (sbyte)1; 
                    do
                    {
                        if (direction)
                            teamSignToCheck = team ? "+" : "-";
                        else
                            teamSignToCheck = team ? "-" : "+";
                        loc_[0] += mov[0] * dir;
                        loc_[1] += mov[1] * dir;
                        if ((loc_[0] <= 7 && loc_[0] >= 0) && (loc_[1] <= 7 && loc_[1] >= 0))
                        {
                            string id = MapMatrix.Map[loc_[0], loc_[1]];
                            if (id != "")
                            {
                                string[] IDparts = id.Split(':');
                                if (IDparts[0] == teamSignToCheck)
                                {
                                    foreach (string checkFor in toCheckFor)
                                    {
                                        if (checkFor == IDparts[1])
                                        {
                                            foundPiece = true;
                                            shouldCheck = false;
                                            if (!direction)
                                            {
                                                hostileLocation[0] = loc_[0];
                                                hostileLocation[1] = loc_[1];
                                            }
                                            break; 
                                        }
                                    }
                                    shouldCheck = false;
                                }
                                else
                                {
                                    if (direction && (kingLocation[0] != loc_[0] && kingLocation[1] != loc_[1])) //need to check if the piece is the king. If it is it means the pawn is protecting the king.
                                        foundPiece = true;
                                    shouldCheck = false;
                                    break;
                                }

                                //shouldCheck = true;
                            }
                        }
                        else
                            shouldCheck = false;
                    } while (shouldCheck);
                    return foundPiece;
                }

            }

            void Protecting(ChessPiece pieces)
            {
                int[] scaledDifferences = new int[2];
                int[] differences = new int[] {pieces.GetMapLocation[0] - kingLocation[0], pieces.GetMapLocation[1] - kingLocation[1] };
                if (differences[0] != 0 && (differences[0] == differences[1] || differences[0] == -differences[1]))
                    scaledDifferences[0] = differences[0] > 0 ? 1 : -1; //scales it to be 1 or -1
                else if (differences[0] == 0)
                    scaledDifferences[1] = differences[1] > 0 ? 1 : -1;
                if (differences[1] != 0 && (differences[0] == differences[1] || differences[0] == -differences[1]))
                    scaledDifferences[1] = differences[1] > 0 ? 1 : -1;
                else if (differences[1] == 0)
                    scaledDifferences[0] = differences[0] > 0 ? 1 : -1;
                bool canReachKing = false;

                int[] mov = new int[2];
                foreach (int[] move in directions)
                {
                    if(move[0] == scaledDifferences[0] && move[1] == scaledDifferences[1])
                    {
                        mov = move;
                        canReachKing = true;
                        break;
                    }
                }
                if (canReachKing) //check in the direction of mov for a hostile piece and if the hostile piece got the oppesite of mov
                {
                    List<string> toCheckFor = new List<string>();
                    if (mov[0] != 0 && mov[1] != 0)
                    {
                        toCheckFor.Add("2");
                        toCheckFor.Add("3");
                    }
                    else
                    {
                        toCheckFor.Add("2");
                        toCheckFor.Add("5");
                    }
                    //int[] loc_ = new int[] { chepie.GetMapLocation[0], chepie.GetMapLocation[1] };

                    List<int[,]> endLocations = new List<int[,]>();
                    bool needToCheckOtherDirection = CheckFelts(mov, toCheckFor); //true if it find a hostile pice
                    bool clearPathToKing = false;
                    if (needToCheckOtherDirection) //true if there is nothing between it and the king
                        clearPathToKing = CheckFelts(new int[] { -mov[0], -mov[1] }, new List<string>() {"1" },  true);
                    if (clearPathToKing) //need to ensure a piece can only be moved in its directions. Pawn can be slightly difficult.
                    {
                        foreach (int[] dir in pieces.GetDirection)
                        {
                            if(dir[0] == mov[0] && dir[1] == mov[1])
                            {

                                CheckFelts(mov, toCheckFor, addLocationToDic: true);
                                CheckFelts(new int[] { -mov[0], -mov[1] }, toCheckFor, addLocationToDic: true);
                                //ProtectKing.ProtectingTheKing.Add(pieces.GetID, endLocations);
                                break;
                            }
                        }
                        ProtectKing.ProtectingTheKing.Add(pieces.GetID, endLocations);
                    }
                    //bool foundHostile = false;
                    //bool shouldCheck = true;
                    //do
                    //{
                    //    loc_[0] += mov[0];
                    //    loc_[1] += mov[1];
                    //    if ((loc_[0] <= 7 && loc_[0] >= 0) && (loc_[1] <= 7 && loc_[1] >= 0))
                    //    {
                    //        string id = MapMatrix.Map[loc_[0], loc_[1]];
                    //        if (id != "")
                    //        {
                    //            string[] IDparts = id.Split(':');
                    //            if (IDparts[0] != chepie.GetID.Split(':')[0])
                    //            {
                    //                foreach (string checkFor in toCheckFor)
                    //                {
                    //                    if (checkFor == IDparts[1])
                    //                    {
                    //                        protectingPieces.Add(chepie.GetID);
                    //                        foundHostile = true;
                    //                        shouldCheck = false; //maybe add a bool foundHostile and later if try, go through a similar loop, but add each location  
                    //                        break; //if the piece can move in that direction //if foundHostile is true, check the other direction for any piece
                    //                    }
                    //                }
                    //            }
                    //            else
                    //            {
                    //                shouldCheck = false;
                    //                break;
                    //            }

                    //            //shouldCheck = true;
                    //        }
                    //    }
                    //    else
                    //        shouldCheck = false;
                    //} while (shouldCheck);
                    bool CheckFelts(int[] mov_, List<string> lookFor, bool addToList = false, bool addLocationToDic = false)
                    { //does not work with pawn movement
                        int[] loc_ = new int[] { pieces.GetMapLocation[0], pieces.GetMapLocation[1] };
                        bool foundPiece = false;
                        bool shouldCheck = true;
                        do
                        {
                            loc_[0] += mov_[0];
                            loc_[1] += mov_[1];
                            if ((loc_[0] <= 7 && loc_[0] >= 0) && (loc_[1] <= 7 && loc_[1] >= 0))
                            {
                                string id = MapMatrix.Map[loc_[0], loc_[1]];
                                if (id != "")
                                {
                                    string[] IDparts = id.Split(':');
                                    string teamSignToCheck = "";
                                    if(addToList)
                                        teamSignToCheck = team ? "+" : "-";
                                    else
                                        teamSignToCheck = team ? "-" : "+";
                                    if (teamSignToCheck  == IDparts[0])
                                    {

                                        foreach (string checkFor in lookFor)
                                        {

                                            if (checkFor == IDparts[1])
                                            {
                                                if (addToList)
                                                    protectingPieces.Add(pieces.GetID);
                                                if(addLocationToDic)
                                                    endLocations.Add(new int[,] { { loc_[0], loc_[1] } });
                                                foundPiece = true;
                                                shouldCheck = false; //maybe add a bool foundHostile and later if try, go through a similar loop, but add each location  
                                                break; //if the piece can move in that direction //if foundHostile is true, check the other direction for any piece
                                            }
                                        }
                                        shouldCheck = false;
                                    }
                                    else
                                    {
                                        shouldCheck = false;
                                        break;
                                    }

                                    //shouldCheck = true;
                                }
                                else if (addLocationToDic && id == "")
                                {
                                    endLocations.Add(new int[,] { { loc_[0], loc_[1] } });
                                }
                            }
                            else
                                shouldCheck = false;
                        } while (shouldCheck);
                        return foundPiece;
                    }
                }


                
            }
        }

        /// <summary>
        /// Check if a king, depending on <paramref name="team"/> is checkmated or not. If the king is checkmated, it will return true. Else false. 
        /// </summary>
        /// <param name="team">True for white, false for black.</param>
        /// <returns>True if the king is checkmated, else false</returns>
        private bool CheckmateChecker(bool team)
        {
            return CheckmateChecker(team, out List<string> canProtectKing);
        }

        /// <summary>
        /// Check if a king, depending on <paramref name="team"/> is checkmated or not. If the king is checkmated, it will return true. Else false. 
        /// Also <paramref name="canProtectKing"/> contains the ID of all pieces that can protect the king, if the king is checked. The king will also be in the list if the king can move. 
        /// </summary>
        /// <param name="team">True for white, false for black.</param>
        /// <returns>True if the king is checkmated, else false</returns>
        private bool CheckmateChecker(bool team, out List<string> canProtectKing)
        {
            List<int[]> locations = new List<int[]>();
            int[] kingLocation = new int[2];
            bool isCheked = false;
            bool isKnight = false;
            bool kingCanMove = false;
            canProtectKing = new List<string>();
            foreach (ChessPiece chePie in ChessList.GetList(team))
            {
                if (chePie is King king)
                {
                    isCheked = chePie.SpecialBool;
                    if (isCheked)
                    {
                        locations = king.GetCheckList;
                        kingLocation = king.GetMapLocation;
                        kingCanMove = king.CanMove;
                        break;
                    }
                }
            }
            if (isCheked)
            {
                foreach (ChessPiece chePie in ChessList.GetList(team))
                {
                    string[] idParts = chePie.GetID.Split(':');
                    int[] chePieLocation = chePie.GetMapLocation;
                    string[] feltIDParts = MapMatrix.Map[locations[0][0], locations[0][1]].Split(':');
                    isKnight = feltIDParts[1] == "4" ? true : false;
                    foreach (string canHelp in ProtectKing.CannotMove)
                    {
                        if(canHelp == chePie.GetID)
                        { //if the ID exist in ProtectKing.CannotMove it is already protecting the king. This ensures it is skipped and minimise this function runtime.
                            break;
                        }
                    }
                    if (chePie is King)
                    {
                        if (kingCanMove)
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, null);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Queen)
                    {
                        int[][] movement = chePie.GetDirection;
                        if (QRBCheck(movement, chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Rook)
                    {
                        int[][] movement = chePie.GetDirection;
                        if (QRBCheck(movement, chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Bishop)
                    {
                        int[][] movement = chePie.GetDirection;
                        if (QRBCheck(movement, chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Knight)
                    {
                        if (KnightCheck(chePie.GetDirection,chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Pawn pawn)
                    {
                        if (PawnCheck(chePie.GetMapLocation, pawn.HasMoved, out List<int[,]> endLocation))
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                }
                if (canProtectKing.Count != 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;

            bool KnightCheck(int[][] directions, int[] ownLocation, out List<int[,]> endLocations)
            {
                int[] kingHostileDifference = new int[] { kingLocation[0] - locations[0][0], kingLocation[1] - locations[0][1] };
                endLocations = new List<int[,]>();
                KnightCanReach(locations[0], ref endLocations);

                if (!isKnight)
                {
                    int biggestDifference = Math.Abs(kingHostileDifference[0]) < Math.Abs(kingHostileDifference[1]) ? Math.Abs(kingHostileDifference[1]) : Math.Abs(kingHostileDifference[0]);
                    int distance = 2;
                    int[] newLocation = { kingLocation[0], kingLocation[1] };
                    int[] movement = new int[2];
                    if (kingHostileDifference[0] > 0)//left //calculates the location that is needed to go to get from the king to the hostile piece.
                        movement[0] = -1;
                    else if (kingHostileDifference[0] < 0)//right
                        movement[0] = 1;
                    else
                        movement[0] = 0;

                    if (kingHostileDifference[1] > 0)//up
                        movement[1] = -1;
                    else if (kingHostileDifference[1] < 0)//down
                        movement[1] = 1;
                    else
                        movement[1] = 0;

                    while (distance <= biggestDifference) //finds all locations between the king and hostile piece.
                    {
                        newLocation[0] += movement[0];
                        newLocation[1] += movement[1];
                        string feltID = MapMatrix.Map[newLocation[0], newLocation[1]];
                        if (feltID != "")
                            break;
                        KnightCanReach(newLocation, ref endLocations);
                            //return true;
                        distance++;
                    }
                }
                if (endLocations.Count != 0)
                    return true;
                else
                    return false;

                void KnightCanReach(int[] standLocation, ref List<int[,]> endLoc)
                {
                    int[] locationDifference = new int[] { standLocation[0] - locations[0][0], standLocation[1] - locations[0][1] };
                    int[][] movement = directions;
                    foreach (int[] mov in movement)
                    {
                        int[] movLocation = new int[] { ownLocation[0] + mov[0], ownLocation[1] + mov[1] };
                        if (movLocation[0] == standLocation[0] && movLocation[1] == standLocation[1])
                        {
                            if (MapMatrix.Map[movLocation[0], movLocation[1]] == "" || (movLocation[0] == locations[0][0] && movLocation[1] == locations[0][1]))
                                endLoc.Add(new int[,] { { movLocation[0], movLocation[1] } });
                        }
                    }

                }
            }

            bool PawnCheck(int[] ownLocation, bool hasMoved, out List<int[,]> endLocations)
            { //changes this one so it returns depends on endLocations to allow for multiple locations rather than one. 
                int direction = team ? -1 : 1;
                int[] locationDifference = new int[] { ownLocation[0] - locations[0][0], ownLocation[1] - locations[0][1] };
                endLocations = new List<int[,]>();
                if(direction == -1)
                    if (locationDifference[1] == 1) //only a white pawn can move up and positive locationDifference[0] means the pawn is below the threating piece.
                    {
                        if (locationDifference[0] == -1)
                        {
                            endLocations.Add(new int[,] { { ownLocation[0] + 1, ownLocation[1] - 1 } }); //right
                            //return true;
                        }
                        else if (locationDifference[0] == 1)
                        {
                            endLocations.Add(new int[,] { { ownLocation[0] - 1, ownLocation[1] - 1 } }); //left
                            //return true;
                        }
                    }
                if(direction == 1)
                    if (locationDifference[1] == -1) //only a black pawn can move down and negative lcoationDifference[0] means the pawn is above the threatining piece. 
                    {
                        if (locationDifference[0] == -1)
                        {
                            endLocations.Add(new int[,] { { ownLocation[0] + 1, ownLocation[1] + 1 } }); //right
                            //return true;
                        }
                        else if (locationDifference[0] == 1)
                        {
                            endLocations.Add(new int[,] { { ownLocation[0] - 1, ownLocation[1] + 1 } }); //left
                            //return true;
                        }
                    }

                if (!isKnight)
                {

                    int xBig = kingLocation[0] > locations[0][0] ? kingLocation[0] : locations[0][0];
                    int xSmall = kingLocation[0] > locations[0][0] ? locations[0][0] : kingLocation[0];
                    if (ownLocation[0] > xSmall && ownLocation[0] < xBig)
                    {
                        int[] directions = new int[] { kingLocation[0] - locations[0][0], kingLocation[1] - locations[0][1] };
                        int[] movement = new int[2];
                        if (directions[0] > 0)//left //calculates the location that is needed to go to get from the king to the hostile piece.
                            movement[0] = -1;
                        else if (directions[0] < 0)//right
                            movement[0] = 1;

                        if (directions[1] > 0)//up
                            movement[1] = -1;
                        else if (directions[1] < 0)//down
                            movement[1] = 1;
                        else
                            movement[1] = 0;

                        int[] standLocation = new int[2] { kingLocation[0], kingLocation[1] };
                        do
                        { //finds the y of the square that is between king and hostile piece that got the same x as the pawn.
                            standLocation[0] += movement[0];
                            standLocation[1] += movement[1]; 
                        } while (standLocation[0] != ownLocation[0]);

                        int yDistance = standLocation[1] - ownLocation[1];
                        int maxRange = hasMoved ? 1 : 2;
                        int pos = 0;
                        if (maxRange >= Math.Abs(yDistance)) 
                        {
                            do
                            {
                                pos++;
                                string feltID = MapMatrix.Map[ownLocation[0], ownLocation[1] + direction * pos];
                                if (feltID != "")
                                    break;
                            } while (pos < Math.Abs(yDistance));
                            if(ownLocation[1] + (pos*direction) == standLocation[1])
                                endLocations.Add(new int[,] { { ownLocation[0], ownLocation[1] + (pos) * direction } }); 
                        }
                    }
                }
                if (endLocations.Count != 0)
                    return true;
                else
                    return false;
            }

            bool QRBCheck(int[][] directions, int[] ownLocation, out List<int[,]> endLocations)
            { //queen could save king, should have been able to take the hostile piece and stand between it and the king. However, it could only capture the hostile piece.
                endLocations = new List<int[,]>();
                foreach (int[] dir in directions)
                {
                    /* To take:
                     * If locationDifference == e.g. [2,3] no piece can reach it expect maybe for special moves like the knight.
                     * if locationDIfference == e.g. [2,2] it can only be reached by a piece that moves diagonal, i.e. bishop and queen. 
                     * If and only if locationDifference == [1,1] can a pawn reach it.
                     * if locationDifference == e.g. [0,2] or e.g. [2,0] it can only be reached by rock and queen.
                     * In all cases it also need to check the inbetween squares to see if there is a free path.  
                     * If locationDifference == any combination of either 0s and 1s, the king can reach it.
                     * Knights need to check if they can land on the locationDifference square or not. 
                     * 
                     * To intercept: 
                     * Find every square between the king and the hostile piece. Check if any square can be "taken" as above. 
                     */

                    if (CanReach(dir, ownLocation, locations[0], ref endLocations))
                    {
                        endLocations.Add(new int[,] { { locations[0][0], locations[0][1] } });
                    }
                    else if (!isKnight)
                    {
                        int[] kingHostileDifference = new int[] { kingLocation[0] - locations[0][0], kingLocation[1] - locations[0][1] }; //negative right/down, positve left/up
                        int biggestDifference = Math.Abs(kingHostileDifference[0]) < Math.Abs(kingHostileDifference[1]) ? Math.Abs(kingHostileDifference[1]) : Math.Abs(kingHostileDifference[0]);
                        int distance = 2;  //king and hostile piece cannot stand on the same location, so no reason to start at 0. 
                        //if the king and hostile piece is right next to each other, biggestDifference will be 1. Thus the reason for starting with 2  
                        int[] newLocation = { kingLocation[0], kingLocation[1] };
                        int[] movement = new int[2];
                        if (kingHostileDifference[0] > 0)//left //calculates the location that is needed to go to get from the king to the hostile piece.
                            movement[0] = -1;
                        else if (kingHostileDifference[0] < 0)//right
                            movement[0] = 1;
                        else
                            movement[0] = 0; //this if-else statement and the one below, does not seem to work that well when the hostile piece is between the piece running this code and the king. 
                                             //then again, this code should not be reached in that case and most likely only the bug that is causing it to be reached
                        if (kingHostileDifference[1] > 0)//up
                            movement[1] = -1;
                        else if (kingHostileDifference[1] < 0)//down
                            movement[1] = 1;
                        else
                            movement[1] = 0;

                        while (distance <= biggestDifference)
                        {
                            newLocation[0] += movement[0];
                            newLocation[1] += movement[1];
                            string feltID = MapMatrix.Map[newLocation[0], newLocation[1]];
                            if (feltID != "")
                                break;
                            CanReach(dir, ownLocation, newLocation, ref endLocations);
                            //return true;
                            distance++;
                        }
                    }
                }
                if (endLocations.Count != 0)
                    return true;
                else
                    return false;

            }

            bool CanReach(int[] dir, int[] ownLocation, int[] toEndOnLocation, ref List<int[,]> endLocations)
            {
                bool index1Sign; bool index2Sign; bool diagonal; bool straight;

                int[] locationDifference = new int[] { ownLocation[0] - toEndOnLocation[0], ownLocation[1] - toEndOnLocation[1] };  //negative right/down, positve left/up
                if (locationDifference[0] != 0 && dir[0] != 0) //find a way to make this look better
                {
                    int sign = locationDifference[0] / dir[0];
                    index1Sign = sign > 0 ? false : true; //if above zero, the signs are the same. Different signs will give a negative result. The piece can only reach the toEndOnLocation if the signs are different. 
                }
                else if (locationDifference[0] == 0 && dir[0] == 0)
                    index1Sign = true;
                else //if only one of the variables specific index is zero and the other one is not, can never reach the destination
                    index1Sign = false;

                if (locationDifference[1] != 0 && dir[1] != 0)
                {
                    int sign = locationDifference[1] / dir[1];
                    index2Sign = sign > 0 ? false : true;
                }
                else if (locationDifference[1] == 0 && dir[1] == 0)
                    index2Sign = true;
                else
                    index2Sign = false;

                if (locationDifference[0] != 0 && locationDifference[1] != 0) //cannot be reach by a straight movement. 
                {
                    if (Math.Abs(locationDifference[0]) == Math.Abs(locationDifference[1])) //can be reached by a diagonal movement
                    {
                        diagonal = true;
                    }
                    else //cannot be reached by a diagonal movement;
                        diagonal = false;
                }
                else
                    diagonal = true;

                if (locationDifference[0] == 0 || locationDifference[1] == 0) //can be reached by a straight movement.
                {
                    straight = true;
                }
                else
                    straight = false;

                if (index1Sign && index2Sign && (diagonal || straight))
                {
                    /*
                     * if both parts of locationDifference is the same, it need to not have the same signs in the dir
                     * E.g. locationDifference == [2,2], dir needs to be [-1,-1]
                     * If LocationDifference == [-2,2], dir needs to be [1,-1]. 
                     * If locationDifference == [0,2], dir needs to be [0,-1] 
                     * etc.
                     * So if any index in locationDifference is zero, the same index in dir needs to be zero,
                     * else any index in dir needs to be of oppesit sign of the same index in locationDifference. 
                     */

                    int[] currentLocation = new int[] { ownLocation[0], ownLocation[1] };
                    int locationsRemaining = Math.Abs(locationDifference[0]) > Math.Abs(locationDifference[1]) ? Math.Abs(locationDifference[0]) : Math.Abs(locationDifference[1]); 
                    string feltID = ""; //maybe have a setting for the default value on the map
                    while (locationsRemaining != 0) //rewrite all of this, also write better comments for the future
                    {
                        currentLocation[0] += dir[0];
                        currentLocation[1] += dir[1];
                        feltID = MapMatrix.Map[currentLocation[0], currentLocation[1]];
                        if (feltID != "" && feltID != MapMatrix.Map[locations[0][0], locations[0][1]])
                            return false;

                        if (locationsRemaining == 1)
                        {
                            endLocations.Add(new int[,] { { currentLocation[0], currentLocation[1] } });
                            return true;
                        }
                        locationsRemaining--;
                    }
                }
                return false;
            }

        }

        /// <summary>
        /// Sets up the two players.
        /// </summary>
        private void PlayerSetup()
        {
            white = new Player(true, Publishers.PubKey);
            black = new Player(false, Publishers.PubKey);
        }

        /// <summary>
        /// Creates the chess pieces. 
        /// </summary>
        private void CreatePieces(bool white, int[,] spawnLocations, byte[] colour)
        {
            string team = white == true ? "+" : "-";
            string ID;
            int[] spawn;
            List<ChessPiece> chessPieces = new List<ChessPiece>();
            for (int i = 0; i < 8; i++)
            {
                ID = String.Format("{0}:6:{1}", team, i + 1);
                spawn = new int[] { spawnLocations[i, 0], spawnLocations[i, 1] };
                chessPieces.Add(new Pawn(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            }
            ID = String.Format("{0}:5:{1}", team, 1);
            spawn = new int[] { spawnLocations[8, 0], spawnLocations[8, 1] };
            chessPieces.Add(new Rook(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:5:{1}", team, 2);
            spawn = new int[] { spawnLocations[15, 0], spawnLocations[15, 1] };
            chessPieces.Add(new Rook(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:4:{1}", team, 1);
            spawn = new int[] { spawnLocations[9, 0], spawnLocations[9, 1] };
            chessPieces.Add(new Knight(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:4:{1}", team, 2);
            spawn = new int[] { spawnLocations[14, 0], spawnLocations[14, 1] };
            chessPieces.Add(new Knight(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:3:{1}", team, 1);
            spawn = new int[] { spawnLocations[10, 0], spawnLocations[10, 1] };
            chessPieces.Add(new Bishop(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:3:{1}", team, 2);
            spawn = new int[] { spawnLocations[13, 0], spawnLocations[13, 1] };
            chessPieces.Add(new Bishop(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:2:{1}", team, 1);
            spawn = new int[] { spawnLocations[11, 0], spawnLocations[11, 1] };
            chessPieces.Add(new Queen(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:1:{1}", team, 1);
            spawn = new int[] { spawnLocations[12, 0], spawnLocations[12, 1] };
            chessPieces.Add(new King(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));

            ChessList.SetChessList(chessPieces, white);
        }
        //protected void KeyEventHandler(object sender, ControlEvents e)
        //{

        //}
    }

    class Player //got to ensure that no spawnlocation is overlapping and deal with it in case there is an overlap. 
    { //this class is set to be an abstract in the UML, but is that really needed? 
        private bool white;
        private string team;
        private int selectedChessPiece;
        private int[] location; //x,y
        private bool didMove = false;
        private ConsoleKeyInfo key;
        private bool isActive = false;

        public Player(bool startTurn, KeyPublisher keyPub)
        {
            this.white = startTurn;
            team = white == true ? "+" : "-";
            keyPub.RaiseKeyEvent += KeyEventHandler;
            isActive = startTurn;
        }

        /// <summary>
        /// Function that calls functions needed for playing the game.
        /// </summary>
        public void Control()
        {
            isActive = true;
            do
            {
                bool didNotSurrender = HoverOver();
                if (didNotSurrender)
                {
                    MovePiece();
                    didMove = ChessList.GetList(white)[selectedChessPiece].CouldMove;
                }
                else
                {
                    break;
                }

            } while (!didMove);
            isActive = false;
        }

        /// <summary>
        /// Allows access to the PlayerMenu() function if pressing esc
        /// </summary>
        public void ControlOnlyMenu()
        {
            isActive = true;
            while (!GameStates.IsTurn && !GameStates.GameEnded)
                if((int)key.Key != 0)
                    if (key.Key == ConsoleKey.Escape)
                        PlayerMenu();
            isActive = false;
        }

        /// <summary>
        /// Lets the player hover over a felt on the boardgame and gets the ID of that felt. If the ID is a chesspiece, it will be highlighted. 
        /// </summary>
        private bool HoverOver()
        {
            string lastMapLocationID;
            int? lastPiece = 0;
            ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(true);
            bool hasSelected = false;
            bool? friendlyPiece = true;
            location = ChessList.GetList(white)[0].GetMapLocation;
            SquareHighLight(true);
            do
            {
                bool? selected = FeltMove(location);
                if (lastPiece != null && !GameStates.GameEnded) //if a previous chess piece has been hovered over, remove the highlight. 
                {
                    if(friendlyPiece == true)
                    ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(false);
                    else if (friendlyPiece == false)
                        ChessList.GetList(!white)[(int)lastPiece].IsHoveredOn(false);
                    lastPiece = null;
                    lastMapLocationID = null;
                    friendlyPiece = null;
                }
                string squareID = MapMatrix.Map[location[0], location[1]];
                if(selected == null)
                {
                    if (GameStates.GameEnded)
                    {
                        hasSelected = true;
                        return false;
                    }
                }
                else if (squareID != "") //is there a chess piece on the location
                    if (squareID.Split(':')[0] == team) //is it on the team
                    {
                        int posistion = 0; //used for later finding the posistion in the List<ChessPiece> the chosen chesspiece got.
                        foreach (ChessPiece piece in ChessList.GetList(white))
                        {
                            if (piece.GetID == squareID)
                            { //correct chess piece and highlight it
                                piece.IsHoveredOn(true);
                                lastMapLocationID = piece.GetID;
                                lastPiece = posistion;
                                friendlyPiece = true;
                                if (selected == true)
                                {
                                    if (ProtectKing.Protect.Count != 0) //ensures only pieces that can protect the king can be moved
                                    {
                                        foreach (string id in ProtectKing.Protect)
                                        {
                                            if (piece.GetID == id)
                                            {
                                                hasSelected = true;
                                                selectedChessPiece = posistion;
                                                ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(false);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        hasSelected = true;
                                        selectedChessPiece = posistion;
                                        ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(false);
                                        break;
                                    }

                                }
                                break;
                            }
                            posistion++;
                        }
                    } 
                    else
                    {
                        int posistion = 0;
                        if (!Settings.CanHighLight)
                            foreach (ChessPiece chePie in ChessList.GetList(!white))
                            {
                                if (chePie.GetID == squareID)
                                {
                                    friendlyPiece = false;
                                    chePie.IsHoveredOn(true,true);
                                    lastPiece = posistion;
                                    break;
                                }
                                posistion++;
                            }
                    }

            } while (!hasSelected);
            SquareHighLight(false);
            return true;
        }

        /// <summary>
        /// Menu that allows a player to stay on playing, ask for a draw or surrender 
        /// </summary>
        private void PlayerMenu()
        { //should allow the player to surrender, ask for a draw or return to play.
            //remember, you can call the board paint and chess paint functions to recreate the visual board and visual pieces.
            //Should people be allowed to use the menu while it is not their turn? If they are, have to deal with a menu appearing while they e.g. moving a piece and such.
            Console.Title = Settings.GetTitle + ": Player Menu";
            string[] playerOptions =
            {
                "Stay Playing",
                "Call for Draw",
                "Game Stats",
                "Surrender"
            };
            Console.Clear();
            string title = "Options";
            isActive = false;
            string answer = Menu.MenuAccess(playerOptions, title);
            isActive = true;
            switch (answer)
            {
                case "Stay Playing":
                    Console.Clear();
                    ChessTable.GameRunTitle();
                    ChessTable.RepaintBoardAndPieces();
                    break;

                case "Call for Draw":
                    if (GameStates.IsOnline)
                    {
                        Network.Transmit.GeneralValueTransmission(3, Network.Transmit.OtherPlayerIpAddress);
                        GameStates.Pause = true;
                        Console.Clear();
                        Console.SetCursorPosition(Settings.MenuTitleOffset[0], Settings.MenuTitleOffset[1]);
                        Console.WriteLine("Waiting on answer");
                        while (GameStates.Pause) ;
                        ChessTable.GameRunTitle();
                    }
                    else
                    {
                        Console.Clear();
                        string drawTitle = "Other player wants to draw";
                        string[] drawOptions = {"Accept Draw", "Decline Draw" };
                        string drawAnswer = Menu.MenuAccess(drawOptions, drawTitle);
                        switch (drawAnswer)
                        {
                            case "Accept Draw":
                                GameStates.GameEnded = true;
                                GameStates.Won = null;
                                break;

                            case "Decline Draw":
                                Console.Clear();
                                ChessTable.RepaintBoardAndPieces();
                                ChessTable.GameRunTitle();
                                break;
                        }
                    }
                    break;

                case "Surrender": 
                    GameStates.GameEnded = true;
                    GameStates.Won = false; 
                    GameStates.WhiteWin = !white;
                    if (GameStates.IsOnline /*&& !GameStates.IsTurn*/)
                    {
                        if (GameStates.PlayerTeam == false)
                            GameStates.TurnCounter++;
                        Network.Transmit.GeneralValueTransmission(41, Network.Transmit.OtherPlayerIpAddress);
                    }
                    break;

                case "Game Stats":
                    GameStatsDisplay();
                    ChessTable.GameRunTitle();
                    ChessTable.RepaintBoardAndPieces();
                    break;
            }
        }

        /// <summary>
        /// Displays the stats of the game.
        /// </summary>
        private void GameStatsDisplay()
        {
            string mainColour = Settings.CVTS.BrightWhiteForeground;
            string highLightColour = Settings.CVTS.BrightCyanForeground;
            string underscore = Settings.CVTS.Underscore;
            string underscore_off = Settings.CVTS.Underscore_Off;
            string reset = Settings.CVTS.Reset;
            string enterColour = Settings.CVTS.BrightRedForeground;

            Console.Clear();
            Console.CursorTop = Settings.MenuOffset[1];
            Console.WriteLine($"\x1b]2;Chess: Game Stats\x07" + //sets the title
                $"{"".PadLeft(Settings.MenuTitleOffset[0])}{mainColour}{underscore}Stats:{reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{mainColour}Amount of white pieces left: {highLightColour}{underscore}{ChessList.GetList(true).Count}{underscore_off}{mainColour}.{reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{mainColour}Amount of black pieces left: {highLightColour}{underscore}{ChessList.GetList(false).Count}{underscore_off}{mainColour}.{reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{mainColour}Amount of turns: {highLightColour}{underscore}{GameStates.TurnCounter}{underscore_off}{mainColour}.{reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{mainColour}Turns since last capture or pawn move: {highLightColour}{underscore}{GameStates.TurnDrawCounter}{underscore_off}{mainColour}.{reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{enterColour}Enter {mainColour}to return.{reset}"); 
            Console.ReadLine();
            Console.Clear();
        }

        /// <summary>
        /// Allows the player to either move to a connecting square to <paramref name="currentLocation"/> or select the <paramref name="currentLocation"/>.
        /// </summary>
        /// <param name="currentLocation">The current location on the board.</param>
        /// <returns>Returns true if enter is pressed, null if escape is pressed, else false.</returns>
        private bool? FeltMove(int[] currentLocation)
        {
            //ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
            //while (Console.KeyAvailable) //flushes any keys pressed to early (does not seem to work all the time).
            //{
            //    Console.ReadKey(true);
            //}

            while (/*!Console.KeyAvailable*/ (int)key.Key == 0 && !GameStates.GameEnded) ;
            //if(!GameStates.GameEnded)
            //    keyInfo = Console.ReadKey(true);

            while (GameStates.Pause) ;

            if (!GameStates.GameEnded) { 
                if (key.Key == Settings.UpKey && currentLocation[1] > 0)
                {
                    SquareHighLight(false);
                    currentLocation[1]--;
                    SquareHighLight(true);
                }
                else if (key.Key == Settings.DownKey && currentLocation[1] < 7)
                {
                    SquareHighLight(false);
                    currentLocation[1]++;
                    SquareHighLight(true);
                }
                else if (key.Key == Settings.LeftKey && currentLocation[0] > 0)
                {
                    SquareHighLight(false);
                    currentLocation[0]--;
                    SquareHighLight(true);
                }
                else if (key.Key == Settings.RightKey && currentLocation[0] < 7)
                {
                    SquareHighLight(false);
                    currentLocation[0]++;
                    SquareHighLight(true);
                }
                else if (key.Key == Settings.SelectKey)
                {
                    key = new ConsoleKeyInfo();
                    return true;
                }
                else if (key.Key == Settings.EscapeKey)
                {
                    PlayerMenu();
                    SquareHighLight(true);
                    key = new ConsoleKeyInfo();
                    return null; 
                }
                key = new ConsoleKeyInfo();
                return false;
            }
            key = new ConsoleKeyInfo();
            return null;
        }

        /// <summary>
        /// Highlights or removes the hightlight of a sqaure. 
        /// </summary>
        /// <param name="isHighlighted">If true highlights the square. If false, it will remove the highligh.</param>
        private void SquareHighLight(bool isHighlighted)
        {
            byte squareSize = Settings.SquareSize;
            int startLocationX = location[0] * squareSize + (location[0] + Settings.Spacing + Settings.EdgeSpacing) * 1 + Settings.Offset[0];
            int startLocationY = location[1] * squareSize + (location[1] + Settings.Spacing + Settings.EdgeSpacing) * 1 + Settings.Offset[1];
            bool emptySquare = MapMatrix.Map[location[0], location[1]] == "" ? true : false;
            bool shallHighlight = true;
            if (!Settings.CanHighLight)
                if (!emptySquare)
                    shallHighlight = false;

            if (isHighlighted)
            {
                byte[] colour = Settings.SelectSquareColour;
                if(shallHighlight)
                    Paint(colour);
            }
            else
            {
                byte colorLocator = (byte)((location[0] + location[1]) % 2);
                byte[] colour = colorLocator == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
                if(shallHighlight)
                    Paint(colour);
            }

            void Paint(byte[] colour)
            {
                for (int n = startLocationX; n < startLocationX + squareSize; n++)
                {
                    Console.SetCursorPosition((int)n, (int)startLocationY);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + Settings.CVTS.Reset);
                    Console.SetCursorPosition((int)n, (int)startLocationY + squareSize - 1);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + Settings.CVTS.Reset);
                }
                for (int n = startLocationY; n < startLocationY + squareSize; n++)
                {
                    Console.SetCursorPosition((int)startLocationX, (int)n);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + Settings.CVTS.Reset);
                    Console.SetCursorPosition((int)startLocationX + squareSize - 1, (int)n);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + Settings.CVTS.Reset);
                }
            }
        }

        /// <summary>
        /// Function that calls the selected chess piece control function. 
        /// </summary>
        private void MovePiece()
        {
            List<int[,]> locations = null;
            locations = ProtectKing.GetListFromDic(ChessList.GetList(white)[selectedChessPiece].GetID);
            if(locations == null)
            {
                locations = ProtectKing.GetListFromProtectingKingDic(ChessList.GetList(white)[selectedChessPiece].GetID);
            }
            if (locations != null)
                ChessList.GetList(white)[selectedChessPiece].SetEndLocations = locations;
            isActive = false;
            ChessList.GetList(white)[selectedChessPiece].Control();
            isActive = true;
        }

        /// <summary>
        /// If the player is the active one it will sets its <c>key</c> to <c>e.Key.key</c>.
        /// </summary>
        /// <param name="sender">The object that invoked the event.</param>
        /// <param name="e">The parameter containing the variables and their values of ControlEvents.</param>
        protected void KeyEventHandler(object sender, ControlEvents.KeyEventArgs e)
        {
            if(isActive)
                key = e.Key;
        }

    }

    /// <summary>
    /// The class for kings
    /// </summary>
    sealed class King : ChessPiece
    { 

        private List<int[]> checkLocations = new List<int[]>(); //contain the locations of the chesspieces that treatens the king.
        private List<string> castLingCandidates;
        private bool isChecked;
        private bool hasMoved = false;
        private byte lastAmountOfThreats = 0;

        /// <summary>
        /// The constructor for the king chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public King(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                "^V^",
                "*|*",
                "-K-"
            };
            mostImportantDesignPart = new byte[] { 1, 2 };
            DesignResizer();
            Draw();
            directions = new int[][]
            {
                            new int[]{-1,0},
                            new int[]{1,0},
                            new int[]{0,1},
                            new int[]{0,-1},
                            new int[]{-1,-1},
                            new int[]{-1,1},
                            new int[]{1,-1},
                            new int[]{1,1}
            };
        }

        /// <summary>
        /// King only function. Returns the list of squares it is treaten from. 
        /// </summary>
        public List<int[]> GetCheckList { get => checkLocations; }

        /// <summary>
        /// Returns true if the king can move. False otherwise. 
        /// </summary>
        public bool CanMove
        {
            get
            {
                EndLocations();
                bool canMove = possibleEndLocations.Count != 0;
                possibleEndLocations = null;
                return canMove;
            }
        }

        /// <summary>
        /// Returns true if the king is checked, false otherwise. 
        /// </summary>
        public override bool SpecialBool
        {
            get
            {
                checkLocations.Clear();
                isChecked = IsInChecked(mapLocation, checkLocations);
                CheckWriteOut();
                return isChecked;
            }
            set => specialBool = value;
        }

        /// <summary>
        /// Contains the code needed to move the king. 
        /// </summary>
        public override void Control()
        {
            isSelected = true;
            Move();
            RemoveDraw(oldMapLocation);
            LocationUpdate();
            Draw();
            UpdateMapMatrix(oldMapLocation);
            castLingCandidates.Clear();
            isSelected = false;
        }

        /// <summary>
        /// Writes out at a specific location, depending on team and given by the Settings class, from where it is treaten.
        /// </summary>
        private void CheckWriteOut()
        {
            if (isChecked || lastAmountOfThreats > 0)
            {
                int[,] writeLocation = Settings.CheckWriteLocation; //should be modified so it return a new array rather than the existing array. 
                byte teamLoc;
                teamLoc = team ? (byte)0 : (byte)1;

                int[] writeAt = new int[] { writeLocation[teamLoc, 0], writeLocation[teamLoc, 1] };

                byte pos = 0;
                for (byte i = 0; i < lastAmountOfThreats; i++)
                {
                    Console.SetCursorPosition(writeAt[0], writeAt[1] + i);
                    Console.Write("".PadLeft(2));
                }
                foreach (int[] loc in checkLocations)
                {
                    char letter = (char)(97 + loc[0]);
                    string writeLout = String.Format("{0}{1}", letter, 8-loc[1]);
                    Console.SetCursorPosition(writeAt[0], writeAt[1] + pos);
                    Console.WriteLine(writeLout);
                    pos++;
                }
                lastAmountOfThreats = pos;
            }
        }

        /// <summary>
        /// Calculates the end locations and, if legal and is not under threat, adds them to a list. 
        /// </summary>
        protected override void EndLocations()
        {
            possibleEndLocations = new List<int[,]>();
            FindCastlingOptions(possibleEndLocations);

            foreach(int[] pos in directions)
            {
                CheckPosistions(pos);
            }

            void CheckPosistions(int[] currentPosition)
            {
                int[] loc = new int[2] { currentPosition[0], currentPosition[1] };
                int[] loc_ = new int[] { loc[0] + mapLocation[0], loc[1] + mapLocation[1] };
                if (!((loc_[0] > 7 || loc_[0] < 0) || (loc_[1] > 7 || loc_[1] < 0)))
                {
                    List<int[]> locationUnderThreat = new List<int[]>();
                    string feltID = MapMatrix.Map[loc[0] + mapLocation[0], mapLocation[1] + loc[1]];
                    if (feltID == "")
                    {
                        if (!IsInChecked(loc_, locationUnderThreat))
                            Add(loc_);
                    }
                    else
                    {
                        if (teamIcon != feltID.Split(':')[0])
                        {
                            if (!IsInChecked(loc_, locationUnderThreat))
                                Add(loc_);
                        }
                    }
                }
            }

            void Add(int[] posistions)
            {
                possibleEndLocations.Add(new int[,] { { (posistions[0]), (posistions[1]) } });
            }
        }

        /// <summary>
        /// Function that checks if <paramref name="location_"/> is under threat by a hostile chess piece. Returns true if it is.
        /// </summary>
        /// <param name="location_">Location to check for being threaten.</param>
        /// <returns>Returns true if <paramref name="location_"/>is under threat, false otherwise. </returns>
        public bool IsInChecked(int[] location_)
        {
            List<int[]> list_ = new List<int[]>();
            return IsInChecked(location_, list_);
        }

        /// <summary>
        /// Functions that check if <paramref name="location_"/> is under threat by a hostile chess piece. Returns true if it is.
        /// </summary>
        /// <param name="location_">Location to check for being threaten.</param>
        /// <param name="toAddToList">List that contains the locations of hostile pieces that are threating the <paramref name="location_"/>.</param>
        /// <returns>Returns true if <paramref name="location_"/>is under threat, false otherwise. </returns>
        private bool IsInChecked(int[] location_, List<int[]> toAddToList)
        { 
            sbyte[,] moveDirection;
            string[][] toLookFor;
            moveDirection = new sbyte[,] { { -1, 0 }, { 0, -1 }, { -1, -1 }, { -1, 1 }, { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 }, };
            //                              left        up          left/up   left/down   down     right    right/down right/up   
            toLookFor = new string[][]
            {//"2", "3", "5" 
                new string[]{"2","5"},
                new string[]{"2","5"},
                new string[]{"2","3"},
                new string[]{"2","3"},
                new string[]{"2","5"},
                new string[]{"2","5"},
                new string[]{"2","3"},
                new string[]{"2","3"}
            }; 
            QRBCheck(moveDirection, toLookFor);

            PawnCheck();

            KnightCheck();

            KingNear();

            if (toAddToList.Count != 0)
                return true;
            else
                return false;

            void KingNear()
            {
                if (!(location_[0] == mapLocation[0] && location_[1] == mapLocation[1]))
                {
                    int[] placement_;
                    placement_ = new int[] { -1, -1 }; //left, up
                    Placement(placement_);
                    placement_ = new int[] { -1, 1 }; //left, down
                    Placement(placement_);
                    placement_ = new int[] { 1, -1 }; //right, up
                    Placement(placement_);
                    placement_ = new int[] { 1, 1 }; //right, down
                    Placement(placement_);
                    placement_ = new int[] { 0, -1 }; //up
                    Placement(placement_);
                    placement_ = new int[] { 0, 1 }; //down
                    Placement(placement_);
                    placement_ = new int[] { 1, 0 }; //right
                    Placement(placement_);
                    placement_ = new int[] { -1, 0 }; //left
                    Placement(placement_);

                    void Placement(int[] direction_)
                    {
                        int[] feltLocation = new int[] { (int)(direction_[0] + location_[0]), (int)(direction_[1] + location_[1]) };
                        if (feltLocation[0] >= 0 && feltLocation[0] <= 7 && feltLocation[1] >= 0 && feltLocation[1] <= 7)
                        {
                            string feltID = MapMatrix.Map[feltLocation[0], feltLocation[1]];
                            if (feltID != "")
                            {
                                string[] feltIDParts = feltID.Split(':');
                                if (feltIDParts[0] != teamIcon)
                                {
                                    if (feltIDParts[1] == "1")
                                    {
                                        toAddToList.Add(new int[2] { feltLocation[0], feltLocation[1] });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            void KnightCheck()
            { 
                int[] placement_;
                placement_ = new int[] { -2, -1 }; //two left, up
                Placement(placement_);
                placement_ = new int[] { -2, 1 }; //two left, down
                Placement(placement_);
                placement_ = new int[] { 2, -1 }; //two right, up
                Placement(placement_);
                placement_ = new int[] { 2, 1 }; //two right, down
                Placement(placement_);
                placement_ = new int[] { -1, -2 }; //left, 2 up
                Placement(placement_);
                placement_ = new int[] { -1, 2 }; //left, 2 down
                Placement(placement_);
                placement_ = new int[] { 1, -2 }; //right, 2 up
                Placement(placement_);
                placement_ = new int[] { 1, 2 }; //right, 2 down
                Placement(placement_);

                void Placement(int[] direction_)
                { //could rewrite this function to take a jaggered array and operate on it instead of calling the function multiple times. 
                    int[] feltLocation = new int[] { (int)(direction_[0] + location_[0]), (int)(direction_[1] + location_[1]) };
                    if (feltLocation[0] >= 0 && feltLocation[0] <= 7 && feltLocation[1] >= 0 && feltLocation[1] <= 7)
                    {
                        string feltID = MapMatrix.Map[feltLocation[0], feltLocation[1]];
                        if (feltID != "")
                        {
                            string[] feltIDParts = feltID.Split(':');
                            if (feltIDParts[0] != teamIcon)
                            {
                                if (feltIDParts[1] == "4")
                                {
                                    toAddToList.Add(new int[2] { feltLocation[0], feltLocation[1] });
                                }
                            }
                        }
                    }
                }
            }


            void PawnCheck()
            {
                sbyte hostileDirection = team ? (sbyte)-1 : (sbyte)1; //if white, pawns to look out for comes for the bottom. If black, they come from the top.
                byte edge = team ? (byte)0 : (byte)7;
                if (location_[0] != 0 && location_[1] != edge) //check left side
                {
                    string feltID = MapMatrix.Map[location_[0] - 1, location_[1] + hostileDirection];
                    FeltChecker(feltID, -1);
                }
                if (location_[0] != 7 && location_[1] != edge) //check right side
                {
                    string feltID = MapMatrix.Map[location_[0] + 1, location_[1] + hostileDirection];
                    FeltChecker(feltID, 1);
                }

                void FeltChecker(string toCheck, sbyte direction)
                {
                    if (toCheck != "")
                    {
                        string[] idParts = toCheck.Split(':');
                        if (idParts[0] != teamIcon)
                        {
                            if (idParts[1] == "6")
                            {
                                toAddToList.Add(new int[2] { (int)(location_[0] + direction), (int)(location_[1] + hostileDirection) });
                            }
                        }
                    }
                }
            }

            void QRBCheck(sbyte[,] directions, string[][] checkpiecesToCheckFor)
            { //can be used to check for queens, rooks and bishops.
                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    int[] checkLocation = new int[2] { location_[0], location_[1] };
                    sbyte[] directions_ = new sbyte[2] { directions[i, 0], directions[i, 1] };
                    string[] piecesToCheckFor = checkpiecesToCheckFor[i];
                    if ((checkLocation[0] + directions_[0] >= 0 && checkLocation[0] + directions_[0] <= 7 && checkLocation[1] + directions_[1] >= 0 && checkLocation[1] + directions_[1] <= 7))
                    {
                        bool end = false;
                        do
                        {
                            string feltID = MapMatrix.Map[checkLocation[0] + directions_[0], checkLocation[1] + directions_[1]];
                            if (feltID != "") //checks if there is a piece on the current square or not
                            {
                                string[] IDstrings = feltID.Split(':');
                                if (IDstrings[0] != teamIcon) //checks if it is hostile or not 
                                {
                                    foreach (string pieceNumber in piecesToCheckFor) //loops to it find the hostile one
                                    {
                                        if (IDstrings[1] == pieceNumber) //checks if the hostile piece is one of the chess pieces that can threaten the current location. 
                                        {
                                            toAddToList.Add(new int[2] { (int)(checkLocation[0] + directions_[0]), (int)(checkLocation[1] + directions_[1]) });
                                            break;
                                        }
                                    }
                                    break;
                                }
                                else
                                {
                                    if (feltID != ID)
                                        break;
                                }
                            }
                            directions_[0] += directions[i, 0];
                            directions_[1] += directions[i, 1];
                            if (!((checkLocation[0] + directions_[0] >= 0 && checkLocation[0] + directions_[0] <= 7) && (checkLocation[1] + directions_[1] >= 0 && checkLocation[1] + directions_[1] <= 7)))
                            {
                                end = true;
                            }
                        } while (!end);
                    }
                }
            }
        }

        public override void NetworkUpdate(int[] newLocation = null, bool captured = false, bool repaintLocation = false)
        {
            if (captured)
            {
                Captured();
                Debug.WriteLine("{0} Captured", ID);
            }
            else if(newLocation != null)
            {
                hasMoved = true;
                Debug.WriteLine("Old Location: {0} {1}", mapLocation[0], mapLocation[1]);
                RemoveDraw(mapLocation);
                Debug.WriteLine("Visuals Removed");
                mapLocation = newLocation;
                Debug.WriteLine("New Location: {0} {1}", mapLocation[0], mapLocation[1]);
                LocationUpdate();
                Debug.WriteLine("Location Updated");
                Draw();
                Debug.WriteLine("Visuals Repainted");
            }
            else if (repaintLocation)
            {
                DisplayPossibleMove();
                SquareHighLight(true, cursorLocation);
            }
        }

        /// <summary>
        /// Returns true if the king is checked, false otherwise. 
        /// </summary>
        /// <returns></returns>
        public override bool SpecialChessPieceFunction()
        {
            List<int[]> checkList = new List<int[]>();
            return IsInChecked(mapLocation, checkList);
        }

        /// <summary>
        /// Returns true if the chess piece has moved. False otherwise. 
        /// </summary>
        private bool HasMoved
        {
            get
            {
                return hasMoved;
            }
            set
            {
                hasMoved = value;
            }
        }

        /// <summary>
        /// Find any legal candidate for the castling move. Requirements are that the king is not treaten. Neither king nor rock has moved. The king does not move to a treaten location or pass trough a treaten location.
        /// Any found candidate is addded to a list. 
        /// </summary>
        private void FindCastlingOptions(List<int[,]> toAddToo)
        {
            if (!HasMoved)
            { //by the officel rules, castling is considered a king move. 
                castLingCandidates = new List<string>(); 
                foreach (ChessPiece chepie in ChessList.GetList(team))
                {
                    if (chepie is Rook)
                    {
                        if(chepie.GetMapLocation[1] == mapLocation[1]) //ensures that no pawn promoted rook can be castled with.
                            if (!chepie.SpecialBool) //has not moved
                            {
                                List<int[]> location_ = new List<int[]>();
                                bool isEmptyRow = false;
                                int direction = (int)(chepie.GetMapLocation[0] - mapLocation[0]); //if positive, go left. If negative, go right
                                int[] currentFeltLocation = new int[] { mapLocation[0], mapLocation[1] };
                                sbyte moveDirection = direction > 0 ? (sbyte)1 : (sbyte)-1;
                                byte squareGoneThrough = 0;
                                do
                                {
                                    squareGoneThrough++;
                                    currentFeltLocation[0] += moveDirection;
                                    string feltID = MapMatrix.Map[currentFeltLocation[0], currentFeltLocation[1]];
                                    if (chepie.GetMapLocation[0] == currentFeltLocation[0])
                                    {
                                        isEmptyRow = true;
                                        break;
                                    }
                                    else if (feltID != "")
                                    {
                                        isEmptyRow = false;
                                        break;
                                    }
                                    else
                                    {
                                        if (squareGoneThrough <= 2)
                                        {
                                            IsInChecked(currentFeltLocation, location_);
                                        }
                                        //the second square is the end location of the king
                                        //the first square is the end location of the rock
                                    }
                                } while (chepie.GetMapLocation[0] != currentFeltLocation[0]);
                                if (isEmptyRow)
                                {
                                    if (location_.Count == 0)
                                    {
                                        castLingCandidates.Add(chepie.GetID);
                                        int[,] arr = new int[,] { { chepie.GetMapLocation[0], chepie.GetMapLocation[1] } };
                                        toAddToo.Add(arr);
                                    }

                                }
                            }
                    }

                }
            }
        }

        /// <summary>
        /// Allows the chesspiece to move. Any square under treat cannot be selected 
        /// </summary>
        protected override void Move()
        {
            oldMapLocation = null;
            bool hasSelected = false;
            if(possibleEndLocations == null)
                EndLocations();
            if (possibleEndLocations.Count != 0)
            {
                possibleEndLocations.Add(new int[,] { { mapLocation[0], mapLocation[1] } });
                DisplayPossibleMove();
                int[] cursorLocation = GetMapLocation;
                do
                {
                    bool? selected = FeltMove(cursorLocation);
                    if (selected == true)
                    {
                        foreach (int[,] loc in possibleEndLocations)
                        {
                            int[] endloc_ = new int[2] { loc[0, 0], loc[0, 1] };
                            if (endloc_[0] == cursorLocation[0] && endloc_[1] == cursorLocation[1])
                            {
                                if(endloc_[0] == mapLocation[0] && endloc_[1] == mapLocation[1])
                                {
                                    couldMove = false;
                                    hasSelected = true;
                                }
                                else
                                {
                                    hasMoved = true;
                                    couldMove = true;
                                    bool castling = true;
                                    oldMapLocation = new int[2] { mapLocation[0], mapLocation[1] };
                                    castling = FeltIDCheck(cursorLocation);
                                    if (castling) //this if-statement and the code above could be written better. Took some time to figure out how castling was done after not looked at the code for a while. 
                                    {
                                        Castling(cursorLocation);
                                        hasSelected = true;
                                        break;
                                    }
                                    else if (!castling)
                                        CaptureEnemyPiece(cursorLocation);
                                    mapLocation = new int[2] { cursorLocation[0], cursorLocation[1] };
                                    hasSelected = true;
                                }

                                break;
                            }
                        }
                    }
                    else if (selected == null)
                    {
                        hasSelected = true;
                        couldMove = false;
                    }
                } while (!hasSelected);
                RemoveDisplayPossibleMove();
                possibleEndLocations = null;
            }
            else
            {
                possibleEndLocations = null;
                couldMove = false;
            }

            bool FeltIDCheck(int[] loc_)
            {
                string[] feltIDParts = MapMatrix.Map[loc_[0], loc_[1]].Split(':');
                if (feltIDParts[0] == teamIcon)
                    if (feltIDParts[1] == "5")
                    {
                        return true;
                    }
                return false;
            }
        }

        /// <summary>
        /// Selects a rook using the map location.
        /// </summary>
        /// <param name="locationOfRock"></param>
        private void Castling(int[] locationOfRock)
        {
            string rookID = MapMatrix.Map[locationOfRock[0], locationOfRock[1]];
            foreach (ChessPiece chePie in ChessList.GetList(team))
            {
                if (chePie.GetID == rookID)
                {
                    int x = chePie.GetMapLocation[0];
                    if (x < mapLocation[0])
                    {
                        mapLocation[0] -= 2;
                    }
                    else
                    {
                        mapLocation[0] += 2;
                    }
                    chePie.SpecialChessPieceFunction();
                    break;
                }
            }
        }

    }

    /// <summary>
    /// The class for queens.
    /// </summary>
    sealed class Queen : ChessPiece
    {
        /// <summary>
        /// The constructor for the queen chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public Queen(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                "_w_",
                "~|~",
                "-Q-"
            };
            mostImportantDesignPart = new byte[] { 1, 2 };
            DesignResizer();
            Draw();
            directions = new int[][]
                        {
                            new int[]{-1,0},
                            new int[]{1,0},
                            new int[]{0,1},
                            new int[]{0,-1},
                            new int[]{-1,-1},
                            new int[]{-1,1},
                            new int[]{1,-1},
                            new int[]{1,1}
                        };
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        {
            possibleEndLocations = new List<int[,]>();
            sbyte[] position = new sbyte[2] { -1, 0 };
            CheckPosistions(position); //left

            position = new sbyte[2] { 1, 0 };
            CheckPosistions(position); //right

            position = new sbyte[2] { 0, -1 };
            CheckPosistions(position); //up

            position = new sbyte[2] { 0, 1 };
            CheckPosistions(position); //down

            position = new sbyte[2] { -1, -1 };
            CheckPosistions(position); //left, up

            position = new sbyte[2] { 1, -1 };
            CheckPosistions(position); //right, up

            position = new sbyte[2] { -1, 1 };
            CheckPosistions(position); //,left down

            position = new sbyte[2] { 1, 1 };
            CheckPosistions(position); //right, down

            void CheckPosistions(sbyte[] currentPosition)
            {
                bool currentCanMove = true;
                sbyte[] loc = new sbyte[2] { currentPosition[0], currentPosition[1] };
                do
                {
                    if ((loc[0] + mapLocation[0] > 7 || loc[0] + mapLocation[0] < 0) || (loc[1] + mapLocation[1] > 7 || loc[1] + mapLocation[1] < 0))
                    {
                        break;
                    }
                    string feltID = MapMatrix.Map[loc[0] + mapLocation[0], mapLocation[1] + loc[1]];
                    if (feltID == "")
                    {
                        Add(loc);
                        loc[0] += currentPosition[0];
                        loc[1] += currentPosition[1];
                    }
                    else
                    {
                        if (teamIcon != feltID.Split(':')[0])
                        {
                            Add(loc);
                        }
                        currentCanMove = false;
                    }

                } while (currentCanMove);
            }

            void Add(sbyte[] posistions)
            {
                possibleEndLocations.Add(new int[,] { { (int)(mapLocation[0] + posistions[0]), (int)(mapLocation[1] + posistions[1]) } });
            }
        }

    }

    /// <summary>
    /// The class for pawns.
    /// </summary>
    sealed class Pawn : ChessPiece
    { 
        private bool firstTurn = true;
        private bool hasMoved = false;
        private sbyte moveDirection;
        private Dictionary<string, byte> promotions = new Dictionary<string, byte>(4);
        // https://docs.microsoft.com/en-us/dotnet/standard/generics/covariance-and-contravariance
        // https://stackoverflow.com/questions/210601/accessing-a-property-of-derived-class-from-the-base-class-in-c-sharp

        /// <summary>
        /// The constructor for the pawn chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public Pawn(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                " - ",
                " | ",
                "-P-"
            };
            moveDirection = team ? (sbyte)-1 : (sbyte)1;
            //teamString = team ? "+" : "-";
            mostImportantDesignPart = new byte[] { 1, 2 };
            DesignResizer();
            Draw();
            promotions.Add("knight", 4); 
            promotions.Add("rook", 5);
            promotions.Add("bishop", 3);
            promotions.Add("queen", 2);
            directions = new int[][] { new int[] {0, moveDirection } };
            canPromoted = true;
        }

        /// <summary>
        /// Returns true if the pawn has moved at some point. False otherwise. 
        /// </summary>
        public bool HasMoved { get => hasMoved; }

        /// <summary>
        /// A modified version of the base Move function. Designed to check if the player uses a double move. 
        /// </summary>
        protected override void Move()
        {
            oldMapLocation = null;
            bool hasSelected = false;

            if(possibleEndLocations == null)
                EndLocations();

            if (possibleEndLocations.Count != 0)
            {
                possibleEndLocations.Add(new int[,] { { mapLocation[0], mapLocation[1] } });
                DisplayPossibleMove();
                int[] cursorLocation = GetMapLocation;
                do
                {
                    bool? selected = FeltMove(cursorLocation);
                    if (selected == true)
                    {
                        foreach (int[,] loc in possibleEndLocations)
                        {
                            int[] endloc_ = new int[2] { loc[0, 0], loc[0, 1] };
                            if (endloc_[0] == cursorLocation[0] && endloc_[1] == cursorLocation[1])
                            {
                                if(endloc_[0] == mapLocation[0] && endloc_[1] == mapLocation[1])
                                {
                                    couldMove = false;
                                    hasSelected = true;
                                }
                                else
                                {
                                    firstTurn = false; //firstTurn ? false : false;
                                    couldMove = true;
                                    oldMapLocation = new int[2] { mapLocation[0], mapLocation[1] };
                                    mapLocation = new int[2] { cursorLocation[0], cursorLocation[1] };
                                    hasSelected = true;
                                    if (Math.Abs((sbyte)(oldMapLocation[1]) - (sbyte)(cursorLocation[1])) == 2)
                                    {
                                        SpecialBool = true;
                                    }
                                    else
                                    {
                                        SpecialBool = false;
                                        if (oldMapLocation[0] != cursorLocation[0])
                                        {
                                            if (MapMatrix.Map[cursorLocation[0], cursorLocation[1]] == "")
                                                CaptureEnemyPiece(new int[] { cursorLocation[0], cursorLocation[1] - moveDirection }); //minus since the direction the pawn is moving is the oppesite direction of the hostile pawn is at. 
                                            else
                                                CaptureEnemyPiece(cursorLocation);
                                        }
                                    }
                                }
                                
                                break;
                            }
                        }
                    }
                    else if (selected == null)
                    {
                        hasSelected = true;
                        couldMove = false;
                    }
                } while (!hasSelected);
                RemoveDisplayPossibleMove();
                possibleEndLocations = null;
                hasMoved = true;
            }
            else
            {
                possibleEndLocations = null;
                couldMove = false;
            }
            //RemoveDisplayPossibleMove();
            //possibleEndLocations = null;

        }

        /// <summary>
        /// Used by the online play and used to update a piece that has been changed by the other player that is not on this computer. 
        /// </summary>
        /// <param name="newLocation"></param>
        /// <param name="captured"></param>
        public override void NetworkUpdate(int[] newLocation = null, bool captured = false, bool repaintLocation = false)
        {
            if (captured)
            {
                Captured();
                Debug.WriteLine("{0} Captured", ID);
            }
            else if (newLocation != null)
            {
                Debug.WriteLine("Old Location: {0} {1}", mapLocation[0], mapLocation[1]);
                if (Math.Abs(newLocation[1] - mapLocation[1]) == 2)
                    specialBool = true;
                RemoveDraw(mapLocation);
                Debug.WriteLine("Visuals Removed");
                mapLocation = newLocation;
                Debug.WriteLine("New Location: {0} {1}", mapLocation[0], mapLocation[1]);
                LocationUpdate();
                Debug.WriteLine("Location Updated");
                Draw();
                Debug.WriteLine("Visuals Repainted");
            }
            else if (repaintLocation)
            {
                DisplayPossibleMove();
                SquareHighLight(true, cursorLocation);
            }
        }



        /// <summary>
        /// Overriden control function of the base class. Checks if the chess piece is ready for a promotion. 
        /// </summary>
        public override void Control()
        {
            isSelected = true;
            Move();
            RemoveDraw(oldMapLocation);
            LocationUpdate();
            Draw();
            UpdateMapMatrix(oldMapLocation);
            Promotion();
            isSelected = false;
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        {
            possibleEndLocations = new List<int[,]>();
            if ((!team && mapLocation[1] != 0) || (team && mapLocation[1] != 7))
                if (MapMatrix.Map[mapLocation[0], mapLocation[1] + moveDirection] == "")
                {
                    possibleEndLocations.Add(new int[,] { { mapLocation[0], (int)(mapLocation[1] + moveDirection) } });
                }
            if (firstTurn)
            {
                if (MapMatrix.Map[mapLocation[0], (mapLocation[1] + moveDirection * 2)] == "" && MapMatrix.Map[mapLocation[0], (mapLocation[1] + moveDirection)] == "")
                {
                    possibleEndLocations.Add(new int[,] { { mapLocation[0], (int)(mapLocation[1] + moveDirection * 2) } });
                }

            }
            CheckAttackPossbilities();
            
        }

        /// <summary>
        /// Checks if there possible hostile piece that can be taken. If there is, they locations are added to the possibleEndLocations.
        /// </summary>
        private void CheckAttackPossbilities()
        {
            if ((!team && mapLocation[1] != 0) || (team && mapLocation[1] != 7))
            {
                if (mapLocation[0] != 0) //check square to the left side
                    LocationCheck(-1);
                if (mapLocation[0] != 7) //check square to the right side
                    LocationCheck(1);
                //if (firstTurn)
                //{
                EnPassant();
                //}
            }

            void LocationCheck(sbyte direction) //find a better name
            {
                string locationID = MapMatrix.Map[mapLocation[0] + direction, mapLocation[1] + moveDirection];
                if (locationID != "")
                {
                    string teamID = locationID.Split(':')[0];
                    if (teamID != teamIcon)
                        possibleEndLocations.Add(new int[,] { { (int)(mapLocation[0] + direction), (int)(mapLocation[1] + moveDirection) } });
                }
            }

            void EnPassant()
            {
                byte chessAmount = (byte)ChessList.GetList(!team).Count;
                for (byte i = 0; i < chessAmount; i++) //goes through all chess pieces
                {
                    string idCheck = ChessList.GetList(!team)[i].GetID;
                    if (idCheck.Split(':')[1] == "6") //checks if the current piece is a pawn
                    {
                        if (ChessList.GetList(!team)[i].SpecialBool) //checks if the pawn as double moved and en passant is allowed.
                        {
                            int[] hostileLocation = ChessList.GetList(!team)[i].GetMapLocation;
                            if (hostileLocation[1] == mapLocation[1]) //does the pawn and en-passant pawn share the same y. 
                                if (hostileLocation[0] == mapLocation[0] + 1 || hostileLocation[0] == mapLocation[0] - 1) //Checks if the pawn x is a location that allows it to be en passant.  
                                {
                                    possibleEndLocations.Add(new int[,] { { hostileLocation[0], hostileLocation[1] + moveDirection } });
                                }
                        }
                    }
                }

            }

        }



        /// <summary>
        /// Promotes the pawn. 
        /// </summary>
        private void Promotion()
        {
            if ((!team && mapLocation[1] == 7) || (team && mapLocation[1] == 0))
            {
                bool chosen = false;
                string command = "Write to Select: ";
                string answer = "";
                //How to select? Arrowkeys? Numberkeys? Written?
                DisplayPromotions();
                Console.SetCursorPosition(Settings.PromotionWriteLocation[0], Settings.PromotionWriteLocation[1] + 2);
                Console.Write(command);
                do //not really happy with this, it does not fit the rest of the game. Consider other ways to do it.
                {
                    Console.SetCursorPosition(Settings.PromotionWriteLocation[0] + command.Length, Settings.PromotionWriteLocation[1] + 2);
                    Console.Write("".PadLeft(answer.Length));
                    Console.SetCursorPosition(Settings.PromotionWriteLocation[0] + command.Length, Settings.PromotionWriteLocation[1] + 2);
                    answer = Console.ReadLine();
                    foreach (string promotionKey in promotions.Keys)
                    {
                        if (promotionKey.ToLower() == answer.ToLower())
                        {
                            chosen = true;
                            break;
                        }
                    }
                } while (!chosen);

                answer = answer.ToLower();
                Console.SetCursorPosition(0, Settings.PromotionWriteLocation[1]); //removes the written promotion text
                Console.WriteLine("".PadLeft(Settings.WindowSize[0]));
                Console.WriteLine("".PadLeft(Settings.WindowSize[0]));
                Console.WriteLine("".PadLeft(Settings.WindowSize[0]));
                Captured();
                string[] IDParts = ID.Split(':');
                string newID;
                IDParts[1] = promotions[answer].ToString();
                switch (answer)
                {
                    case "knight":
                        //IDParts[1] = "4";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]); //The P is to indicate that the piece used to be a pawn.
                        ChessList.GetList(team).Add(new Knight(colour, team, mapLocation, newID, Publishers.PubKey, Publishers.PubCapture));
                        break;

                    case "bishop":
                        //IDParts[1] = "3";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]);
                        ChessList.GetList(team).Add(new Bishop(colour, team, mapLocation, newID, Publishers.PubKey, Publishers.PubCapture));
                        break;

                    case "rook":
                        //IDParts[1] = "5";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]);
                        ChessList.GetList(team).Add(new Rook(colour, team, mapLocation, newID, Publishers.PubKey, Publishers.PubCapture));
                        break;

                    case "queen":
                        //IDParts[1] = "2";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]);
                        ChessList.GetList(team).Add(new Queen(colour, team, mapLocation, newID, Publishers.PubKey, Publishers.PubCapture));
                        break;

                }
                ChessList.GetList(team)[ChessList.GetList(team).Count - 1].SpecialBool = true;
            }

        }

        /// <summary>
        /// Displays the possible promotions
        /// </summary>
        private void DisplayPromotions()
        { //writes to a location what chesspieces it can be promoted too.
            string promotionsString = $"Possible Promotions:{Environment.NewLine}{"".PadLeft(Settings.PromotionWriteLocation[0])}";
            foreach (string key in promotions.Keys)
            {
                char[] charArray = key.ToCharArray();
                charArray[0] = (char)((int)charArray[0] - 32);
                string keyUpper = new string(charArray);
                promotionsString += keyUpper + " ";
            }
            Console.SetCursorPosition(Settings.PromotionWriteLocation[0], Settings.PromotionWriteLocation[1]);
            Console.Write(promotionsString);
        }

    }

    /// <summary>
    /// The class for rocks.
    /// </summary>
    sealed class Rook : ChessPiece
    {
        private bool hasMoved = false;

        /// <summary>
        /// The constructor for the rook chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public Rook(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                "^^^",
                "|=|",
                "-R-"
            };
            mostImportantDesignPart = new byte[] { 1, 2 };
            DesignResizer();
            Draw();
            directions = new int[][]
                        {
                            new int[]{-1,0},
                            new int[]{1,0},
                            new int[]{0,1},
                            new int[]{0,-1}
                        };
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        {
            possibleEndLocations = new List<int[,]>();
            sbyte[] position = new sbyte[2] { -1, 0 };
            CheckPosistions(position); //left

            position = new sbyte[2] { 1, 0 };
            CheckPosistions(position); //right

            position = new sbyte[2] { 0, -1 };
            CheckPosistions(position); //up

            position = new sbyte[2] { 0, 1 };
            CheckPosistions(position); //down

            if (possibleEndLocations.Count != 0)
            { //need to make sure that if a player selects the rook and it cannot move, it does not prevent castling from happening. 
                SpecialBool = true;
            }

            void CheckPosistions(sbyte[] currentPosition)
            {
                bool currentCanMove = true;
                sbyte[] loc = new sbyte[2] { currentPosition[0], currentPosition[1] };
                do
                {
                    if ((loc[0] + mapLocation[0] > 7 || loc[0] + mapLocation[0] < 0) || (loc[1] + mapLocation[1] > 7 || loc[1] + mapLocation[1] < 0))
                    {
                        break;
                    }
                    string feltID = MapMatrix.Map[loc[0] + mapLocation[0], mapLocation[1] + loc[1]];
                    if (feltID == "")
                    {
                        Add(loc);
                        loc[0] += currentPosition[0];
                        loc[1] += currentPosition[1];
                    }
                    else
                    {
                        if (teamIcon != feltID.Split(':')[0])
                        {
                            Add(loc);
                        }
                        currentCanMove = false;
                    }

                } while (currentCanMove);
            }

            void Add(sbyte[] posistions)
            {
                possibleEndLocations.Add(new int[,] { { (mapLocation[0] + posistions[0]), (mapLocation[1] + posistions[1]) } });
            }
        }

        /// <summary>
        /// Returns true if the chess piece has moved at any point, false otherwise. 
        /// </summary>
        public override bool SpecialBool
        {
            get
            {
                return HasMoved;
            }
            set
            {
                HasMoved = value;
            }
        }

        /// <summary>
        /// Returns true if the chess piece has moved at any point, false otherwise. 
        /// </summary>
        private bool HasMoved
        {
            get
            {
                return hasMoved;
            }
            set
            {
                hasMoved = value;
            }
        }

        /// <summary>
        /// Calls the castling function. 
        /// </summary>
        /// <returns></returns>
        public override bool SpecialChessPieceFunction()
        { //used to set its position after castling. So RemoveDraw, update locations, Draw, set variable regarding if it has moved to true. 
          //called by the active piece, so its own and the one it is castling with. 
          //if it was moved, it return false, true if it can castle. 

            //if (shouldCastle && !HasMoved)
            //Castling();
            Castling();
            return false;
        }

        public override void NetworkUpdate(int[] newLocation = null, bool captured = false, bool repaintLocation = false)
        {
            if (captured)
            {
                Captured();
                Debug.WriteLine("{0} Captured", ID);
            }
            //Taken();
            else if(newLocation != null)
            {
                    hasMoved = true;
                    Debug.WriteLine("Old Location: {0} {1}", mapLocation[0], mapLocation[1]);
                    RemoveDraw(mapLocation);
                    Debug.WriteLine("Visuals Removed");
                    mapLocation = newLocation;
                    Debug.WriteLine("New Location: {0} {1}", mapLocation[0], mapLocation[1]);
                    LocationUpdate();
                    Debug.WriteLine("Location Updated");
                    Draw();
                    Debug.WriteLine("Visuals Repainted");
            }
            else if (repaintLocation)
            {
                DisplayPossibleMove();
                SquareHighLight(true, cursorLocation);
            }

        }

        /// <summary>
        /// Moves the chess piece after the rules of castling. 
        /// </summary>
        private void Castling()
        {
            RemoveDraw(mapLocation);
            int[] newLocation = new int[2];
            int[] oldLocation = mapLocation;
            newLocation[1] = mapLocation[1];
            newLocation[0] = mapLocation[0] == 0 ? 3 : 5;
            mapLocation = newLocation;
            UpdateMapMatrix(oldLocation);
            LocationUpdate();
            Draw();
            hasMoved = true;
        }

    }

    /// <summary>
    /// The class for bishops. 
    /// </summary>
    sealed class Bishop : ChessPiece
    {
        /// <summary>
        /// The constructor for the bishop chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>

        public Bishop(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                $"_{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Plus_Minus}{Settings.CVTS.DEC.DEC_Deactive}_",
                $"{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Vertical_Line}{Settings.CVTS.DEC.DEC_Deactive}O{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Vertical_Line}{Settings.CVTS.DEC.DEC_Deactive}",
                $"-B-"
            };
            mostImportantDesignPart = new byte[] { 1, 2 };
            DesignResizer();
            //Design = new string[]
            //{ //changes the length of the first string so code needs to be changed to either use the amount of strings in the array or something else. Also the code that control the paint location
            //    $"_{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Plus_Minus}{Settings.CVTS.DEC.DEC_Deactive}_",
            //    "|O|",
            //    "-B-"
            //};
            Draw();
            directions = new int[][]
                        {
                            new int[]{-1,-1},
                            new int[]{-1,1},
                            new int[]{1,-1},
                            new int[]{1,1}
                        };
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        {
            possibleEndLocations = new List<int[,]>();
            sbyte[] position = new sbyte[2] { -1, -1 };
            CheckPosistions(position); //left, up

            position = new sbyte[2] { 1, -1 };
            CheckPosistions(position); //right, up

            position = new sbyte[2] { -1, 1 };
            CheckPosistions(position); //,left down

            position = new sbyte[2] { 1, 1 };
            CheckPosistions(position); //right, down


            void CheckPosistions(sbyte[] currentPosition)
            {
                bool currentCanMove = true;
                sbyte[] loc = new sbyte[2] { currentPosition[0], currentPosition[1] };
                do
                {
                    if ((loc[0] + mapLocation[0] > 7 || loc[0] + mapLocation[0] < 0) || (loc[1] + mapLocation[1] > 7 || loc[1] + mapLocation[1] < 0))
                    {
                        break;
                        //Solucation might not work as intended as it the current values cannot go negative and if posistion is 0 - 1 it will reach the max value. This will be caugt, but consider a different approach. 
                    }
                    string feltID = MapMatrix.Map[loc[0] + mapLocation[0], mapLocation[1] + loc[1]];
                    if (feltID == "")
                    {
                        Add(loc);
                        loc[0] += currentPosition[0];
                        loc[1] += currentPosition[1];
                    }
                    else
                    {
                        if (teamIcon != feltID.Split(':')[0])
                        {
                            Add(loc);
                            break;
                        }
                        currentCanMove = false;
                    }

                } while (currentCanMove);
            }

            void Add(sbyte[] posistions)
            {
                possibleEndLocations.Add(new int[,] { { (int)(mapLocation[0] + posistions[0]), (mapLocation[1] + posistions[1]) } });
            }
        }
    }

    /// <summary>
    /// The class for knights.
    /// </summary>
    sealed class Knight : ChessPiece
    {
        /// <summary>
        /// The constructor for the knight chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public Knight(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                " ^_",
                " |>",
                "-k-"
            };
            mostImportantDesignPart = new byte[] { 1, 2 };
            DesignResizer();
            Draw();
            directions = new int[][]
                {
                        new int[] { 1, 2 }, 
                        new int[] { 2, 1 }, 
                        new int[] { -1, -2 }, 
                        new int[] { -2, -1 }, 
                        new int[] { 1, -2 }, 
                        new int[] { 2, -1 }, 
                        new int[] { -1, 2 },
                        new int[] { -2, 1 } 
                };
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        { //there must be a better way to do this...
            possibleEndLocations = new List<int[,]>();
            sbyte[] potenieltLocation = { -2, -1 }; //2 down left
            if (mapLocation[0] + potenieltLocation[0] >= 0 && mapLocation[1] + potenieltLocation[1] >= 0)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { 2, -1 }; //2 up left
            if (mapLocation[0] + potenieltLocation[0] <= 7 && mapLocation[1] + potenieltLocation[1] >= 0)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { 2, 1 }; //2 up right
            if (mapLocation[0] + potenieltLocation[0] <= 7 && mapLocation[1] + potenieltLocation[1] <= 7)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { -2, 1 }; //2 down right
            if (mapLocation[0] + potenieltLocation[0] >= 0 && mapLocation[1] + potenieltLocation[1] <= 7)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { -1, -2 }; //down 2 left
            if (mapLocation[0] + potenieltLocation[0] >= 0 && mapLocation[1] + potenieltLocation[1] >= 0)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { 1, -2 }; //up 2 left
            if (mapLocation[0] + potenieltLocation[0] <= 7 && mapLocation[1] + potenieltLocation[1] >= 0)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { 1, 2 }; //up 2 right
            if (mapLocation[0] + potenieltLocation[0] <= 7 && mapLocation[1] + potenieltLocation[1] <= 7)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { -1, 2 }; //down 2 right
            if (mapLocation[0] + potenieltLocation[0] >= 0 && mapLocation[1] + potenieltLocation[1] <= 7)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }

            bool CheckPosistions(sbyte[] posistions)
            { //returns true if the piece can legal move there, false otherwise. 
                string feltID = MapMatrix.Map[mapLocation[0] + posistions[0], mapLocation[1] + posistions[1]];
                if (feltID != "")
                {
                    if (feltID.Split(':')[0] != teamIcon)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            void Add(sbyte[] posistions)
            {
                possibleEndLocations.Add(new int[,] { { (mapLocation[0] + posistions[0]), (mapLocation[1] + posistions[1]) }, });
            }
        }

    }


    /// <summary>
    /// The base class for chess pieces. Any chess piece should derive from this class.
    /// </summary>
    abstract public class ChessPiece //still got a lot to read and learn about what is the best choice for a base class, class is abstract, everything is abstract, nothing is abstract and so on. 
    {
        protected int[] location = new int[2]; //x,y
        protected byte[] colour; // https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/inheritance 
        protected string[] design;
        protected bool team;
        protected int[] mapLocation;
        protected int[] oldMapLocation;
        protected string id;
        protected bool hasBeenTaken = false;
        protected bool isSelected = false;
        //protected byte squareSize = Settings.SquareSize;
        protected List<int[,]> possibleEndLocations = null;
        protected string teamIcon; //come up with a better name
        protected bool couldMove;
        protected bool specialBool;
        protected bool canPromoted; 
        protected int[][] directions;
        protected byte[] mostImportantDesignPart;
        protected int[] cursorLocation;
        protected ConsoleKeyInfo key;
        protected bool IsActive = false;
        //https://en.wikipedia.org/wiki/Chess_piece_relative_value if you ever want to implement an AI this could help 


        /// <summary>
        /// The default chess piece constructor. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece, true for white, false for black.</param>
        /// <param name="mapLocation_">The start location on the map.</param>
        /// <param name="ID">The ID of the chess piece. The constructor does nothing to ensure the ID is unique.</param>
        public ChessPiece(byte[] colour_, bool team_, int[] mapLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub)
        {
            Colour = colour_;
            SetTeam(team_);
            MapLocation = mapLocation_;
            this.ID = ID; //String.Format("{0}n:{1}", team, i); team = team_ == true ? "-" : "+"; n being the chesspiece type
            LocationUpdate();
            MapMatrix.Map[mapLocation[0], mapLocation[1]] = ID;
            teamIcon = ID.Split(':')[0];
            keyPub.RaiseKeyEvent += KeyEventHandler;
            capPub.RaiseCaptureEvent -= CaptureEventHandler; //might help
            capPub.RaiseCaptureEvent += CaptureEventHandler;
        }

        /// <summary>
        /// Returns a bool that indicate if this piece has been captured by another player's piece. 
        /// </summary>
        public bool BeenCaptured { get => hasBeenTaken; } //use by other code to see if the piece have been "taken" and should be removed from game. 

        /// <summary>
        /// Gets and sets the location of the chesspiece on the console.  
        /// </summary>
        protected int[] Location
        {
            set
            {
                Debug.WriteLine(ID + " set before: " + location[0] + " " +  location[1]);
                location = value;
                Debug.WriteLine(ID + " set after: " + location[0] + " " + location[1]);
            }
            get
            {
                Debug.WriteLine(ID + " get before: " + location[0] + " " + location[1]);
                int[] loc = new int[2] { location[0], location[1] };
                Debug.WriteLine(ID + " get after: " + location[0] + " " + location[1]);
                return loc;
            }

        }

        /// <summary>
        /// sets the colour of the chesspiece.
        /// </summary>
        protected byte[] Colour { set => colour = value; }

        /// <summary>
        /// Sets and gets the design of the chesspieice. 
        /// </summary>
        protected string[] Design { get => design; set => design = value; }

        /// <summary>
        /// Sets the location on the map.
        /// </summary>
        protected int[] MapLocation { set => mapLocation = value; }

        /// <summary>
        /// Gets and set the ID of the chesspiece. //maybe have some code that ensures the ID is unique 
        /// </summary>
        protected string ID { get => id; set => id = value; } //maybe split into two. Set being protected and the Get being public 

        /// <summary>
        /// Returns the ID of the chess piece.
        /// </summary>
        public string GetID { get => ID; }

        /// <summary>
        /// Returns true if the chess piece could move, false if it could not move.
        /// </summary>
        public bool CouldMove { get => couldMove; }

        /// <summary>
        /// What the bool is related to depends on the chess piece type. For a rock, it returns true if it has moved, else false. For...
        /// </summary>
        public virtual bool SpecialBool { get => specialBool; set => specialBool = value; }

        /// <summary>
        /// Access the directions the chess pieces can move in. 
        /// </summary>
        public int[][] GetDirection { get => directions; }
        
        /// <summary>
        /// If true the piece can promoted, else it is not allowed to be promoted
        /// </summary>
        public bool CanBePromoted { get => canPromoted; }

        /// <summary>
        /// Returns the location on the map. 
        /// </summary>
        public int[] GetMapLocation
        {
            get
            {
                int[] mapLoc = new int[2] { mapLocation[0], mapLocation[1] };
                return mapLoc;
            }
        }

        /// <summary>
        /// If true the piece is selected by a player, else it is false.
        /// </summary>
        public bool IsSelected { get => isSelected; set => isSelected = value; }
        
        /// <summary>
        /// Function that "controls" a piece. What to explain and how to..
        /// </summary>
        public virtual void Control()
        {
            isSelected = true;
            Move();
            RemoveDraw(oldMapLocation);
            LocationUpdate();
            Draw();
            UpdateMapMatrix(oldMapLocation);
            isSelected = false;
        }

        /// <summary>
        /// if the design is bigger than the square, part of the design will be removed so it fits the square. 
        /// </summary>
        public void DesignResizer()
        {
            if (Settings.ChesspieceDesignSize > Settings.SquareSize)
            {
                string[] resizedDesign = new string[Settings.SquareSize];

                if (resizedDesign.Length > 1) 
                {
                    //figure out if there are strings above and/or below the most significant string
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
                design = resizedDesign;

                string DesignCreate(char[] charArray, byte charLength)
                {
                    bool?[] DECLocation = new bool?[charLength];
                    bool?[] DECSymbolUsed = new bool?[charLength];
                    int charDECLength = 0;
                    if(charLength < charArray.Length) //DEC is present 
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

                    char[] newDesign = new char[Settings.SquareSize];
                    byte mostImportLocation = mostImportantDesignPart[0] < Settings.SquareSize ? mostImportantDesignPart[0] : (byte)(newDesign.Length - 1);
                    newDesign[mostImportLocation] = charArray[mostImportantDesignPart[0]];
                    byte rightAmountOfChars = (byte)(charLength - (mostImportantDesignPart[0]+1)); //the +1 is to make up with the difference in array length and indexes.
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
                    if(charDECLength > charLength) //checks to see if DEC escape signs might should be reimplemented
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
                        if(reconsturectionChar.Count > newDesign.Length)
                            newDesign = reconsturectionChar.ToArray();
                    }
                    string newDesignString = new string(newDesign);
                    return newDesignString;
                }
            }
        }

        /// <summary>
        /// The function of this function depends on the chesspiece. Rock, pawn, and king got different implementations. The rest will always return false.
        /// The rook and the king will be true if they have moved and the pawn will be true if last move made a double move.
        /// </summary>
        /// <returns></returns>
        public virtual bool SpecialChessPieceFunction()
        {
            return false;
        }

        /// <summary>
        /// Calculates the, legal, end locations that a chess piece can move too. Should also be overriden the inherience class
        /// </summary>
        protected virtual void EndLocations()
        {

        }

        /// <summary>
        /// Updates the piece, if it has been modified by the other player. If any combinations of the parameters are not null/false the piece will be set as captured only.
        /// </summary>
        /// <param name="newLocation">If not null, will move the piece to this location.</param>
        /// <param name="captured">If not null, will set the piece as been captured.</param>
        public virtual void NetworkUpdate(int[] newLocation = null, bool captured = false, bool repaintLocation = false)
        {
            if (captured)
            {
                Captured();
                Debug.WriteLine($"{ID} Captured");
            }
                //Taken();
            else if (newLocation != null)
            {
                    Debug.WriteLine("Old Location: {0} {1}", mapLocation[0],mapLocation[1]);
                    RemoveDraw(mapLocation);
                    Debug.WriteLine("Visuals Removed");
                    mapLocation = newLocation;
                    Debug.WriteLine("New Location: {0} {1}", mapLocation[0], mapLocation[1]);
                    LocationUpdate();
                    Debug.WriteLine("Location Updated");
                    Draw();
                    Debug.WriteLine("Visuals Repainted");
            }
            else if (repaintLocation)
            {
                DisplayPossibleMove();
                SquareHighLight(true, cursorLocation);
            }
        }

        /// <summary>
        /// Allows the chesspiece to move. 
        /// </summary>
        protected virtual void Move()
        {
            oldMapLocation = null;
            bool hasSelected = false;
            if(possibleEndLocations == null)
                EndLocations();
            
            if (possibleEndLocations.Count != 0)
            {
                possibleEndLocations.Add(new int[,] { { mapLocation[0], mapLocation[1] } });
                DisplayPossibleMove();
                int[] cursorLocation = GetMapLocation;
                do
                {
                    bool? selected = FeltMove(cursorLocation);
                    if (selected == true)
                    {
                        foreach (int[,] loc in possibleEndLocations)
                        {
                            int[] endloc_ = new int[2] { loc[0, 0], loc[0, 1] };
                            if (endloc_[0] == cursorLocation[0] && endloc_[1] == cursorLocation[1])
                            {
                                if(endloc_[0] == mapLocation[0] && endloc_[1] == mapLocation[1])
                                {
                                    couldMove = false;
                                    hasSelected = true;
                                }
                                else
                                {
                                    couldMove = true;
                                    oldMapLocation = new int[2] { mapLocation[0], mapLocation[1] };
                                    CaptureEnemyPiece(cursorLocation);
                                    mapLocation = new int[2] { cursorLocation[0], cursorLocation[1] };
                                    hasSelected = true;
                                }
                                break;
                            }
                        }
                    }
                    else if (selected == null)
                    {
                        hasSelected = true;
                        couldMove = false;
                    }
                } while (!hasSelected);
                RemoveDisplayPossibleMove();
                possibleEndLocations = null;
            }
            else
            {
                possibleEndLocations = null;
                couldMove = false;
            }
        }

        /// <summary>
        /// Allows the chesspiece to move to locations in <paramref name="movements"/>. 
        /// </summary>
        /// <param name="movements">List of locations the piece can move to.</param>
        public virtual List<int[,]> SetEndLocations { set => possibleEndLocations = value; }

        /// <summary>
        /// Checks if <paramref name="locationToCheck"/> contains an ID and if the ID is hostile, the function will call that ID's chesspiece's Captured function.
        /// </summary>
        /// <param name="locationToCheck">The location to check for a chess piece</param>
        protected void CaptureEnemyPiece(int[] locationToCheck)
        {
            string feltID = MapMatrix.Map[locationToCheck[0], locationToCheck[1]];
            if (feltID != "")
                if (teamIcon != feltID.Split(':')[0])
                {
                    Publishers.PubCapture.Capture(feltID);
                    //foreach (ChessPiece chessHostile in ChessList.GetList(!team))
                    //{
                    //    if (chessHostile.GetID == feltID)
                    //    {
                    //        chessHostile.Captured();
                    //    }
                    //}
                }
        }

        /// <summary>
        /// Allows the chesspiece to select different sqaures on the board. If enter is pressed on a square the chesspiece can move too, it will move to that square. 
        /// </summary>
        /// <param name="currentLocation">The current location of hovered over square.</param>
        /// <returns>Returns true if enter is pressed, else false.</returns>
        protected bool? FeltMove(int[] currentLocation)
        {
            cursorLocation = currentLocation;
            SquareHighLight(true, currentLocation);
            //ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
            //while (Console.KeyAvailable) //this should flush the keys
            //{
            //    Console.ReadKey(true);
            //}

            while (/*!Console.KeyAvailable*/ (int)key.Key == 0 && !GameStates.GameEnded) ;
            //if (!GameStates.GameEnded) 
            //    keyInfo = Console.ReadKey(true); //consider moving this and its variable into the if-statment below

            while (GameStates.Pause) ;

            if (!GameStates.GameEnded)
            {
                SquareHighLight(false, currentLocation);
                foreach (int[,] loc in possibleEndLocations) //paints a single legal move square instead of repainting them all
                {
                    int[] endloc_ = new int[2] { loc[0, 0], loc[0, 1] };
                    if (endloc_[0] == currentLocation[0] && endloc_[1] == currentLocation[1])
                    {
                        PaintBackground(Settings.SelectMoveSquareColour, loc);
                        break;
                    }
                }
                if (key.Key == Settings.UpKey && currentLocation[1] > 0)
                {
                    currentLocation[1]--;
                }
                else if (key.Key == Settings.DownKey && currentLocation[1] < 7)
                {
                    currentLocation[1]++;
                }
                else if (key.Key == Settings.LeftKey && currentLocation[0] > 0)
                {
                    currentLocation[0]--;
                }
                else if (key.Key == Settings.RightKey && currentLocation[0] < 7)
                {
                    currentLocation[0]++;
                }
                else if (key.Key == Settings.SelectKey)
                {
                    key = new ConsoleKeyInfo();  
                    return true;
                }
                cursorLocation = currentLocation;
                //SquareHighLight(true, currentLocation);
                key = new ConsoleKeyInfo();
                return false;
            }
            key = new ConsoleKeyInfo();
            return null;
        }

        /// <summary>
        /// Highlights or dehighlights a sqare.
        /// </summary>
        /// <param name="isHighlighted">If true, the square at <paramref name="currentLocation"/> is highlighted, else it is not highlighted.</param>
        /// <param name="currentLocation">The location to (de)highlight.</param>
        protected void SquareHighLight(bool isHighlighted, int[] currentLocation)
        {
            byte squareSize = Settings.SquareSize;
            byte[] colour;
            int startLocationX = currentLocation[0] * squareSize + (currentLocation[0] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[0];
            int startLocationY = currentLocation[1] * squareSize + (currentLocation[1] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[1];
            bool notEmpty = MapMatrix.Map[currentLocation[0], currentLocation[1]] != "" ? true : false;

            if (isHighlighted)
            {
                colour = Settings.SelectSquareColour;
            }
            else
            {
                byte colorLocator = (byte)((currentLocation[0] + currentLocation[1]) % 2);
                colour = colorLocator == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
            }
            if (!Settings.CanHighLight)
            {
                if (notEmpty) //if the piece can move to a location with a hostile it will not display the "cursor"
                {
                    bool wasFound = false;
                    foreach (int[,] loc in possibleEndLocations)
                    {
                        if(currentLocation[0] == loc[0,0] && currentLocation[1]== loc[0, 1])
                        {
                            Paint(colour);
                            wasFound = true;
                            break;
                        }
                    }
                    if (!wasFound)
                    {
                        string squareID = MapMatrix.Map[currentLocation[0], currentLocation[1]];
                        if (squareID != "")
                        {
                            if(squareID.Split(':')[0] != teamIcon)
                            {
                                foreach (ChessPiece chePie in ChessList.GetList(!team))
                                {
                                    if(chePie.GetID == squareID)
                                    {
                                        if (isHighlighted) 
                                            chePie.IsHoveredOn(true, true);
                                        else
                                            chePie.Draw();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                foreach (ChessPiece chePie in ChessList.GetList(team))
                                {
                                    if (chePie.GetID == squareID)
                                    {
                                        if (isHighlighted)
                                            chePie.IsHoveredOn(true, true);
                                        else
                                            chePie.Draw();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Paint(colour);
                }
                
            }
            else
                Paint(colour); 
            
                
            void Paint(byte[] paintColour)
            {
                for (int n = startLocationX; n < startLocationX + squareSize; n++)
                {
                    Console.SetCursorPosition((int)n, (int)startLocationY);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + paintColour[0] + ";" + paintColour[1] + ";" + paintColour[2] + "m " + Settings.CVTS.Reset);
                    Console.SetCursorPosition((int)n, (int)startLocationY + squareSize - 1);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + paintColour[0] + ";" + paintColour[1] + ";" + paintColour[2] + "m " + Settings.CVTS.Reset);
                }
                for (int n = startLocationY; n < startLocationY + squareSize; n++)
                {
                    Console.SetCursorPosition((int)startLocationX, (int)n);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + paintColour[0] + ";" + paintColour[1] + ";" + paintColour[2] + "m " + Settings.CVTS.Reset);
                    Console.SetCursorPosition((int)startLocationX + squareSize - 1, (int)n);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + paintColour[0] + ";" + paintColour[1] + ";" + paintColour[2] + "m " + Settings.CVTS.Reset);
                }
            }
        }

        /// <summary>
        /// updates the location that is used for displaying the chesspiece on the chessboard
        /// </summary>
        protected void LocationUpdate()
        {
            Location = new int[2] { mapLocation[0] * Settings.SquareSize + (mapLocation[0] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[0], mapLocation[1] * Settings.SquareSize + (mapLocation[1] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[1] };
        }

        /// <summary>
        /// Draws the chesspiece at its specific location
        /// </summary>
        protected void Draw()
        {
            PaintForeground();
        }

        /// <summary>
        /// Displays a piece in another colour if it is hovered over. 
        /// </summary>
        /// <param name="hover">If true, the piece will be coloured in a different colour. If false, the piece will have its normal colour.</param>
        public void IsHoveredOn(bool hover, bool otherTeam = false)
        {
            if (hover)
                PaintBoth(otherTeam);
            else
                Draw();
        }

        /// <summary>
        /// Calculates the paint location and the paint colour. Paint colour is calculated out from <paramref name="mapLoc"/>.
        /// </summary>
        /// <param name="drawLocationX">The x start posistion.</param>
        /// <param name="drawLocationY">The y start posistion.</param>
        /// <param name="mapLoc">The location on the map matrix that is used to determine background colour.</param>
        /// <returns>Returns the colours of the calculated location.</returns>
        protected byte[] PaintCalculations(out int drawLocationX, out int drawLocationY, int[] mapLoc)
        {
            int designSize = Design.Length;
            drawLocationX = Location[0] + (int)(Settings.SquareSize - designSize) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            drawLocationY = Location[1] + (int)(Settings.SquareSize - designSize) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            int locationForColour = (mapLoc[0] + mapLoc[1]) % 2; //if zero, background colour is "white", else background colour is "black".
            byte[] colours = locationForColour == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
            return colours;
        }

        /// <summary>
        /// Paints the foreground of the current location.
        /// </summary>
        protected void PaintForeground()
        {
            byte[] colours = PaintCalculations(out int drawLocationX, out int drawLocationY, mapLocation);
            for (int i = 0; i < design.Length; i++) //why does all the inputs, length, count and so on use signed variable types... 
            { 
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + colours[0] + ";" + colours[1] + ";" + colours[2] + "m" + Settings.CVTS.ExtendedForegroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m{0}" + Settings.CVTS.Reset, design[i], colours);
            }
        }

        /// <summary>
        /// Paints both the background and foreground of the current location. 
        /// </summary>
        protected void PaintBoth(bool otherPlayer = false)
        {
            byte[] backColours = PaintCalculations(out int drawLocationX, out int drawLocationY, mapLocation);
            byte[] colours;// = Settings.SelectPieceColour;
            if(!otherPlayer)
                colours = Settings.SelectPieceColour;
            else
                colours = new byte[] {(byte)(255 - Settings.SelectPieceColour[0]), (byte)(255 - Settings.SelectPieceColour[1]), (byte)(255 - Settings.SelectPieceColour[2]) };

            for (int i = 0; i < design.Length; i++)
            {
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + backColours[0] + ";" + backColours[1] + ";" + backColours[2] + "m" + Settings.CVTS.ExtendedForegroundColour_RGB + colours[0] + ";" + colours[1] + ";" + colours[2] + "m{0}" + Settings.CVTS.Reset, design[i]);
            }
        }


        /// <summary>
        /// Removes the visual identication of a chesspiece at its current location.
        /// </summary>
        protected void RemoveDraw(int[] locationToRemove)
        {
            if (locationToRemove != null)
            {
                byte[] colours = PaintCalculations(out int drawLocationX, out int drawLocationY, locationToRemove);
                for (int i = 0; i < design.Length; i++)
                {
                    Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + colours[0] + ";" + colours[1] + ";" + colours[2] + "m".PadRight(design.Length + 1, ' ') + Settings.CVTS.Reset);
                }
            }
        }

        /// <summary>
        /// Updates the map matrix with the new location of the chess piece and sets the old location to "". 
        /// </summary>
        protected void UpdateMapMatrix(int[] oldMapLocation) //need to call this before the LocationUpdate
        {
            if (oldMapLocation != null)
            {
                MapMatrix.Map[mapLocation[0], mapLocation[1]] = ID;
                MapMatrix.Map[oldMapLocation[0], oldMapLocation[1]] = "";
            }
        }

        /// <summary>
        /// Set a chesspeice set to be captured so it can be removed from the game and removes its visual representation. 
        /// </summary>
        public void Captured() //consider making a delegate/event with this. If a piece capture another, send out an event with the captured piece's ID, all pieces are subscribed to the event. 
        {//call by another piece, the one that captures this piece. 
            Debug.WriteLine("Captured Function 1 " + this.ID + ": " + location[0] + " " + location[1]);
            hasBeenTaken = true;
            MapMatrix.Map[mapLocation[0], mapLocation[1]] = "";
            Debug.WriteLine("Captured Function 2 " + this.ID + ": " + location[0] + " " + location[1]);
            RemoveDraw(mapLocation);
            Publishers.PubCapture.RaiseCaptureEvent -= CaptureEventHandler;
        }

        /// <summary>
        /// Sets the team of the chesspiece.
        /// </summary>
        /// <param name="team_">True for white. False for black.</param>
        protected void SetTeam(bool team_)
        {
            team = team_;
        }

        /// <summary>
        /// Removes all displayed possible moves. 
        /// </summary>
        protected void RemoveDisplayPossibleMove()
        {
            foreach (int[,] end in possibleEndLocations)
            {
                byte colourLoc = (byte)((end[0, 0] + end[0, 1]) % 2);
                byte[] backColour = colourLoc == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
                if (!Settings.CanHighLight)
                {
                    string squareID = MapMatrix.Map[end[0, 0], end[0, 1]];
                    if (squareID != "")
                    {
                        if (squareID == ID)
                            PaintBackground(backColour, end);
                        else if(squareID.Split(':')[0] != teamIcon)
                        {
                            foreach (ChessPiece chePie in ChessList.GetList(!team))
                            {
                                if(chePie.GetID == squareID)
                                {
                                    chePie.Draw();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (ChessPiece chePie in ChessList.GetList(team))
                            {
                                if (chePie.GetID == squareID)
                                {
                                    chePie.Draw();
                                    break;
                                }
                            }
                        }
                    }
                    else
                        PaintBackground(backColour, end);
                }
                else
                    PaintBackground(backColour, end);
            }
        }

        /// <summary>
        /// Display the possible, legal, moves a chesspiece can take. 
        /// </summary>
        protected void DisplayPossibleMove()
        {
            foreach (int[,] end in possibleEndLocations)
            {
                PaintBackground(Settings.SelectMoveSquareColour, end);
            }
        }

        /// <summary>
        /// Display the possible, legal, moves a chesspiece can take. 
        /// </summary>
        protected void DisplayPossibleMove(List<int[,]> locations)
        {
            foreach (int[,] end in locations)
            {
                PaintBackground(Settings.SelectMoveSquareColour, end);
            }
        }

        /// <summary>
        /// Paints the background. 
        /// </summary>
        /// <param name="colour">Colour of the background.</param>
        /// <param name="locationEnd">Array of locations. </param>
        protected void PaintBackground(byte[] colour, int[,] locationEnd)
        {
            byte squareSize = Settings.SquareSize;
            int startLocationX = locationEnd[0, 0] * squareSize + (locationEnd[0, 0] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[0];
            int startLocationY = locationEnd[0, 1] * squareSize + (locationEnd[0, 1] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[1];
            for (int n = startLocationX; n < startLocationX + squareSize; n++)
            {
                Console.SetCursorPosition((int)n, (int)startLocationY);
                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + Settings.CVTS.Reset);
                Console.SetCursorPosition((int)n, (int)startLocationY + squareSize - 1);
                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + Settings.CVTS.Reset);
            }
            for (int n = startLocationY; n < startLocationY + squareSize; n++)
            {
                Console.SetCursorPosition((int)startLocationX, (int)n);
                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + Settings.CVTS.Reset);
                Console.SetCursorPosition((int)startLocationX + squareSize - 1, (int)n);
                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + Settings.CVTS.Reset);
            }
        }

        /// <summary>
        /// If the piece is the active one it will sets its <c>key</c> to <c>e.Key.key</c>.
        /// </summary>
        /// <param name="sender">The object that invoked the event.</param>
        /// <param name="e">The parameter containing the variables and their values of ControlEvents.</param>
        protected void KeyEventHandler(object sender, ControlEvents.KeyEventArgs e)
        {
            if (isSelected)
                key = e.Key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CaptureEventHandler(object sender, ControlEvents.CaptureEventArgs e)
        {
            Debug.WriteLine("Captured Event " + this.ID + ": " + location[0] + " " + location[1]);
            if (e.ID == ID)
            {
                Debug.WriteLine("Captured Event Before " + this.ID + ": " + location[0] + " " + location[1]);
                Captured();
                Debug.WriteLine("Captured Event After " + this.ID +": " + location[0] + " " + location[1]);
            }
        }

    }



    //public class DelegateTest
    //{
    //    private delegate void StringOut(string s);
    //    private delegate double ReturnOut(params double[] values);
    //    private delegate void ArrayOut<T>(params T[][] arrays);
    //    StringOut stringOut;
    //    ReturnOut returnOut;
    //    ArrayOut<string> typeOut;
    //    ArrayOut<int> typeIntOut;
    //    //ArrayOut test;
    //    //private static event StringOut stringOutTest;
    //    Publisher pub;
    //    List<Subscriber> subs = new List<Subscriber>();
    //    public DelegateTest()
    //    {
    //        stringOut = StringWritter;
    //        stringOut += StringMehWritter;
    //        stringOut += StringFancyWritter;
    //        returnOut = MathCal;
    //        typeOut = OutDotPrintLine;
    //        typeIntOut = OutDotPrintLine<int>;

    //        pub = new Publisher();
    //        subs.Add(new Subscriber("sub1", pub));
    //        subs.Add(new Subscriber("sub2", pub));

    //    }

    //    public void Print<T>(params T[][] arrays)
    //    {

    //        //if (typeof(T) == typeof(int))
    //            //typeIntOut(Array.ConvertAll(arrays, item => (int)item); //https://stackoverflow.com/questions/2068120/c-sharp-cast-entire-array
    //        //else if
    //    }

    //    private void OutDotPrintLine<T>(params T[][] arrays)
    //    {
    //        foreach (T[] array in arrays)
    //        {
    //            foreach (T item in array)
    //            {
    //                Console.Write(item.ToString() + "");
    //            }
    //            Console.WriteLine();
    //        }
    //    }

    //    public double MathCalculator(params double[] values)
    //    {
    //        double result = returnOut(values);
    //        stringOut(result.ToString());
    //        return result;
    //    }

    //    private double MathCal(params double[] values)
    //    {
    //        double result = 0;
    //        foreach (double value in values)
    //        {
    //            result += value;
    //        }
    //        return result;
    //    } 

    //    public void WriteOutEvent()
    //    {
    //        pub.Test();
    //        pub.ArrowTest();
    //    }

    //    public void WriteOut(string message)
    //    {
    //        stringOut(message);
    //    }
        
    //    public void MathTest(params float[] values)
    //    {
    //        Math(stringOut, values);
    //    }

    //    private void Math(StringOut callBack, params float[] values)
    //    {
    //        float total = 0;
    //        foreach (float value in values)
    //        {
    //            total += value;
    //            callBack(value.ToString());
    //        }
    //        callBack($"Total is {total}. Average is {total/values.Length}");
    //    }

    //    private void StringWritter(string s)
    //    {
    //        Console.WriteLine(s);
    //    }

    //    private void StringMehWritter(string s)
    //    {
    //        Console.WriteLine("Meh... " + s);
    //    }

    //    private void StringFancyWritter(string s)
    //    {
    //        Console.WriteLine(Settings.CVTS.BrightRedForeground + s + Settings.CVTS.Reset);
    //    }


    //    public class Event : EventArgs
    //    {
    //        public Event(string s)
    //        {
    //            Message = s;
    //        }
    //        public Event(params double[] values)
    //        {
    //            Values = values;
    //        }
    //        public Event(ConsoleKeyInfo key)
    //        {
    //            Key = key;
    //        }
    //        public double[] Values { get; set; }
    //        public string Message { get; set; }
    //        public ConsoleKeyInfo Key { get; set; }
    //    }


    //    public class Publisher
    //    {
    //        public event EventTest StringEvent;

    //        public delegate void EventTest(object sender, Event args);

    //        public event EventHandler<Event> StringEvent2;

    //        public event EventTest ValueEvent;

    //        public event EventHandler<Event> KeyEvent;


    //        public void Test()
    //        {
    //            OnEventTest(new Event("Event Ran"));
    //            OnEventTest(new Event(5, 2, 1));
    //            //OnEventTest(new Event(5, -2, 1));
    //        }

    //        public void ArrowTest()
    //        {
    //            bool test = false;
    //            do
    //            {
    //                while (!Console.KeyAvailable) ;
    //                ConsoleKeyInfo key = Console.ReadKey(); //have a list of ConsoleKeys for the reworked movement system
    //                if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Enter)
    //                    onArrowKeyPress(new Event(key));

    //            } while (!test);
    //        }

    //        protected virtual void onArrowKeyPress(Event e)
    //        {
    //            EventHandler<Event> eventHandler = KeyEvent;
    //            if(eventHandler != null)
    //            {
    //                eventHandler.Invoke(this, e);
    //            }
    //        }

    //        protected virtual void OnEventTest(Event e)
    //        {
    //            EventHandler<Event> testEvent = StringEvent2;
    //            var eValues = e.GetType().GetProperties();
    //            if (testEvent != null)
    //            {
    //                for (int n = 0; n < eValues.Length; n++)
    //                {
    //                    string type = eValues[n].PropertyType.Name;

    //                    if (type == "Double[]")
    //                    {
    //                        if (eValues[n].GetValue(e) != null)
    //                        {
    //                            double result = 0;
    //                            double[] values = (double[])e.GetType().GetProperty(eValues[n].Name).GetValue(e);
    //                            foreach (double value in values)
    //                            {
    //                                result += value;
    //                            }
    //                            e.Values = new double[] { result };
    //                            //e.Message += $"Result is {result}"; //
    //                            testEvent.Invoke(this, e);
    //                        }
    //                    }
    //                    else if (type == "String")
    //                    {
    //                        if (eValues[n].GetValue(e) != null) //even if Message is null, since Values is before it will add the results to Message and thus after Message is no longer null
    //                        {
    //                            e.Message += $" at {DateTime.Now}";
    //                            testEvent.Invoke(this, e);
    //                        }
    //                    }

    //                }
    //            }
    //        }
    //    }


    //    public class Subscriber
    //    {
    //        private readonly string ID;
    //        private int[] direction;
    //        public Subscriber(string ID, Publisher Pub)
    //        {
    //            Random rnd = new Random();
    //            this.ID = ID;
    //            Pub.StringEvent2 += HandleString2Event;
    //            Pub.KeyEvent += KeyEvent;
    //            direction = new int[2] {rnd.Next(-5,5), rnd.Next(-10,10) };
    //        }

    //        protected void KeyEvent(object sender, Event e)
    //        {
    //            Console.CursorLeft = 0;
    //            ConsoleKey key = e.Key.Key;
    //            Console.WriteLine($"{ID} recieved {key.ToString()}");
    //            if (key == ConsoleKey.DownArrow)
    //                direction[1]++;
    //            else if (key == ConsoleKey.UpArrow)
    //                direction[1]--;
    //            else if (key == ConsoleKey.RightArrow)
    //                direction[0]++;
    //            else if (key == ConsoleKey.LeftArrow)
    //                direction[0]--;
    //            //Console.CursorLeft = 0; //without this one, sub 1 will write at index 1 instead of index 0 for some reason. The formatted string does not contain an empty space
    //            Console.WriteLine($"{ID}'s Coordinates: {direction[0]} {direction[1]}"); //and both subs places the cursor the same location on the next line
    //            //found out of why, pressing a key will still "write" to the console and thus move the cursor
    //        }

    //        protected void HandleString2Event(object sender, Event e)
    //        {
    //            var eProperties = e.GetType().GetProperties();
    //            for (int n = 0; n < eProperties.Length; n++)
    //            {
    //                if(eProperties[n].GetValue(e) != null)
    //                {
    //                    string type = eProperties[n].PropertyType.Name;
    //                    if (type == "String")
    //                        Console.WriteLine($"{ID} received this message: {e.Message}");
    //                    else if (type == "Double[]")
    //                        Console.WriteLine($"{ID} received this value: {e.Values[0]}");
    //                }
    //            }
    //        }

    //    }



    //}

}
