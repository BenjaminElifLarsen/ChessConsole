using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Chess
{
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
        private static byte[] menuColourTitle = new byte[] { 255, 0, 255 };
        private static byte[] offset = new byte[] { 4, 2 }; //works as it should
        private static byte[] menuOffset = new byte[] { 2, 3 };
        private static byte[] menuTitleOffset = new byte[] { (byte)(menuOffset[0] - 1), (byte)(menuOffset[1] - 1) };
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
        private static char[] keywordBracket = new char[2] { keywordLeft, keywordRight };
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
        public static byte SetSquareSize
        {
            set
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
            public static string ExtendedForegroundColour_RGB { get => foregroundColouring; }
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
}
