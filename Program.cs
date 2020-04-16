using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Chess
{   //https://www.chessvariants.com/d.chess/chess.html
    //team == false, player = black, else player = white. White top, black bottom
    public class MapMatrix
    {
        private static string[,] map = new string[8, 8];
        private static bool mapPrepared = false;
        private MapMatrix()
        {

        }

        public static void PrepareMap()
        {
            if (mapPrepared == false)
                for (int n = 0; n < 8; n++)
                    for (int m = 0; m < 8; m++)
                        map[n, m] = "";
            mapPrepared = true;
        }
        public static string[,] Map { get => map; set => map = value; }
    }

    public class ChessList
    {
        private ChessList() { }
        private static List<ChessPiece> chessListBlack = new List<ChessPiece>();
        private static List<ChessPiece> chessListWhite = new List<ChessPiece>();
        //public static void SetChessListBlack(List<ChessPiece> list)
        //{
        //    chessListBlack = list;
        //}
        public static void SetChessList(List<ChessPiece> list, bool team)
        {
            if (team)
                chessListWhite = list;
            else
                chessListBlack = list;
        }
        public static List<ChessPiece> GetList(bool team)
        {
            return team == false ? chessListBlack : chessListWhite;
        }
    }

    public class Settings
    {
        private Settings() { }
        private static byte squareSize = 5;
        private static byte[] lineColour = new byte[] { 122, 122, 122 };
        private static byte[] lineColourBase = new byte[] { 87, 65, 47 };
        private static byte[] squareColour1 = new byte[] { 182, 123, 91 };
        private static byte[] squareColour2 = new byte[] { 135, 68, 31 };
        private static byte[] hoverOverSquareColour = new byte[] { 193, 76, 29 };
        private static byte[] chessPieceHoverOverSquareColour = new byte[] { 34, 124, 66 };
        private static byte[] chessPieceHoverOver = new byte[] { 31, 135, 113 };
        private static byte[] offset = new byte[] { 4, 2 };
        private static char lineX = '-';
        private static char lineY = '|';
        private static byte extraSpacing = 1;
        public static byte SquareSize { get => squareSize; }
        public static byte[] LineColour { get => lineColour; }
        public static byte[] LineColourBase { get => lineColourBase; }
        public static byte[] SquareColour1 { get => squareColour1; }
        public static byte[] SquareColour2 { get => squareColour2; }
        public static byte[] SelectSquareColour { get => hoverOverSquareColour; }
        public static byte[] SelectMoveSquareColour { get => chessPieceHoverOverSquareColour; }
        public static byte[] SelectPieceColour { get => chessPieceHoverOver; }
        public static byte[] Offset { get => offset; } //remember the '|' and '-'
        public static char GetLineX { get => lineX; }
        public static char GetLineY { get => lineY; }
        public static byte Spacing { get => extraSpacing; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ChessTable chess = new ChessTable();
            chess.Play();
            Console.ReadLine();
        }
    }

    class ChessTable
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        private Player white; //white top
        private Player black; //black bottom
        private uint[,] whiteSpawnLocation; //since the map matrix is being used, no real reason for having these outside their player class
        private uint[,] blackSpawnLocation;
        private byte squareSize;
        private byte[] lineColour;
        private byte[] lineColourBase;
        private byte[] squareColour1;
        private byte[] squareColour2;
        private byte[] offset;
        private byte[] windowsSize = new byte[2];


        public ChessTable()
        {
            MapMatrix.PrepareMap();
            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);

            squareSize = 5;
            lineColour = new byte[] { 122, 122, 122 };
            lineColourBase = new byte[] { 87, 65, 47 };
            squareColour1 = new byte[] { 182, 123, 91 };
            squareColour2 = new byte[] { 135, 68, 31 };
            offset = new byte[] { 2, 2 };

            windowsSize[0] = (byte)(9 + 8 * squareSize + 10);
            windowsSize[1] = (byte)(9 + 8 * squareSize + 10);
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);
            whiteSpawnLocation = new uint[,] {
                { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 },
                { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }
            };
            blackSpawnLocation = new uint[,] {
                { 0, 6 }, { 1, 6 }, { 2, 6 }, { 3, 6 }, { 4, 6 }, { 5, 6 }, { 6, 6 }, { 7, 6 },
                { 0, 7 }, { 1, 7 }, { 2, 7 }, { 3, 7 }, { 4, 7 }, { 5, 7 }, { 6, 7 }, { 7, 7 }
            }; 
            BoardSetup();
            PlayerSetup();
        }


        private void BoardSetup()
        {//8 squares in each direction. Each piece is 3*3 currently, each square is 5*5 currently. 
            Console.CursorVisible = false;
            ushort distance = (ushort)(9 + 8 * squareSize);
            for (int k = 0; k < distance; k++)
                for (int i = 0; i < distance; i += 1 + squareSize)
                {
                    Console.SetCursorPosition(i + Settings.Offset[0], k + Settings.Offset[1]);
                    Console.Write("\x1b[48;2;" + lineColourBase[0] + ";" + lineColourBase[1] + ";" + lineColourBase[2] + "m ");
                    Console.SetCursorPosition(i + Settings.Offset[0], k + Settings.Offset[1]);
                    Console.Write("\x1b[38;2;" + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m|" + "\x1b[0m");
                }
            for (int k = 0; k < distance; k += 1 + squareSize)
                for (int i = 1; i < distance - 1; i++)
                {
                    Console.SetCursorPosition(i + Settings.Offset[0], k + Settings.Offset[1]);
                    Console.Write("\x1b[48;2;" + lineColourBase[0] + ";" + lineColourBase[1] + ";" + lineColourBase[2] + "m ");
                    Console.SetCursorPosition(i + Settings.Offset[0], k + Settings.Offset[1]);
                    Console.Write("\x1b[38;2;" + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m-" + "\x1b[0m");
                }
            BoardColouring();

        }

        private void BoardColouring()
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
                            Console.SetCursorPosition(i + Settings.Offset[0] + 1 + n, k + Settings.Offset[1] + 1 + m);
                            if (location % 2 == 1)
                                Console.Write("\x1b[48;2;" + squareColour1[0] + ";" + squareColour1[1] + ";" + squareColour1[2] + "m " + "\x1b[0m");
                            else if (location % 2 == 0)
                                Console.Write("\x1b[48;2;" + squareColour2[0] + ";" + squareColour2[1] + ";" + squareColour2[2] + "m " + "\x1b[0m");
                        }
                    }
                    location++;
                }
                location++;
            }


        }

        public void Play()
        {
            do
            {
                white.Control();
                for (int i = ChessList.GetList(false).Count - 1; i >= 0; i--)
                {
                    if (ChessList.GetList(false)[i].SpecialBool)
                        ChessList.GetList(false)[i].SpecialBool = false;
                    if (ChessList.GetList(false)[i].BeenTaken)
                        ChessList.GetList(false).RemoveAt(i);
                }
                black.Control();
                for (int i = ChessList.GetList(true).Count - 1; i >= 0; i--)
                {
                    if (ChessList.GetList(true)[i].SpecialBool)
                        ChessList.GetList(true)[i].SpecialBool = false;
                    if (ChessList.GetList(true)[i].BeenTaken)
                        ChessList.GetList(true).RemoveAt(i);
                }
            } while (true);
        }

        private void PlayerSetup()
        {
            byte[] colourWhite =
            {
                255,255,255
            };
            white = new Player(colourWhite, true, whiteSpawnLocation);
            byte[] colourBlack =
            {
                0,0,0
            };
            black = new Player(colourBlack, false, blackSpawnLocation);
        }

    }

    class Player //got to ensure that no spawnlocation is overlapping and deal with it in case there is an overlap. 
    { //this class is set to be an abstract in the UML, but is that really needed? 
        private byte[] colour;
        private bool white;
        private List<ChessPiece> chessPieces = new List<ChessPiece>();
        private uint[,] spawnLocations; //start with the pawns, left to right and then the rest, left to right
        private string team;
        private string selectedID;
        private int selectedChessPiece;
        private uint[] location; //x,y
        private bool didMove = false;

        public Player(byte[] colour, bool startTurn, uint[,] spawnLocations)
        {
            this.colour = colour;
            this.white = startTurn;
            this.spawnLocations = spawnLocations;
            team = white == true ? "+" : "-";
            CreatePieces();
        }

        public void Control()
        {
            do
            {
                HoverOver();
                MovePiece();
                didMove = ChessList.GetList(white)[selectedChessPiece].CouldMove;
            } while (!didMove);
        }

        /// <summary>
        /// Lets the player hover over a felt on the boardgame and gets the ID of that felt. If the ID is a chesspiece, it will be highlighted. 
        /// </summary>
        private void HoverOver()
        {
            string lastMapLocationID;
            int? lastPiece = null;
            bool hasSelected = false;
            location = ChessList.GetList(white)[0].GetMapLocation;
            SquareHighLight(true);
            do
            {
                bool selected = FeltMove(location);
                if (lastPiece != null)
                {
                    ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(false);
                    lastPiece = null;
                    lastMapLocationID = null;
                }
                string squareID = MapMatrix.Map[location[0], location[1]];
                if (squareID != "")
                    if (squareID.Split(':')[0] == team)
                    {
                        int posistion = 0;
                        foreach (ChessPiece piece in ChessList.GetList(white))
                        {
                            if (piece.GetID == squareID)
                            {
                                piece.IsHoveredOn(true);
                                lastMapLocationID = piece.GetID;
                                lastPiece = posistion;
                                if (selected == true)
                                {
                                    hasSelected = true;
                                    selectedChessPiece = posistion;
                                    ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(false);
                                }
                            }
                            posistion++;
                        }
                    }

            } while (!hasSelected);
            SquareHighLight(false);
            SelectPiece();

        }

        /// <summary>
        /// Allows the player to either move to a connecting square to <paramref name="currentLocation"/> or select the <paramref name="currentLocation"/>.
        /// </summary>
        /// <param name="currentLocation">The current location on the board.</param>
        /// <returns></returns>
        private bool FeltMove(uint[] currentLocation)
        {
            //uint[] currentLocation = location; //remember that both arrays point to the same memory.

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            SquareHighLight(false);
            if (keyInfo.Key == ConsoleKey.UpArrow && currentLocation[1] > 0)
            {
                currentLocation[1]--;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow && currentLocation[1] < 7)
            {
                currentLocation[1]++;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow && currentLocation[0] > 0)
            {
                currentLocation[0]--;
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow && currentLocation[0] < 7)
            {
                currentLocation[0]++;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                return true;
            }

            SquareHighLight(true);
            return false;

        }


        /// <summary>
        /// Highlights or removes the hightlight of a sqaure. 
        /// </summary>
        /// <param name="isHighlighted">If true highlights the square. If false, it will remove the highligh.</param>
        private void SquareHighLight(bool isHighlighted)
        {
            byte squareSize = Settings.SquareSize;
            uint startLocationX = location[0] * squareSize + (location[0] + 1) * 1 + Settings.Offset[0];
            uint startLocationY = location[1] * squareSize + (location[1] + 1) * 1 + Settings.Offset[1];
            if (isHighlighted)
            {
                byte[] colour = Settings.SelectSquareColour;
                Paint(colour);
            }
            else
            {
                byte colorLocator = (byte)((location[0] + location[1]) % 2);
                byte[] colour = colorLocator == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
                Paint(colour);
            }

            void Paint(byte[] colour)
            {
                for (uint n = startLocationX; n < startLocationX + squareSize; n++)
                {
                    Console.SetCursorPosition((int)n, (int)startLocationY);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                    Console.SetCursorPosition((int)n, (int)startLocationY + squareSize - 1);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                }
                for (uint n = startLocationY; n < startLocationY + squareSize; n++)
                {
                    Console.SetCursorPosition((int)startLocationX, (int)n);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                    Console.SetCursorPosition((int)startLocationX + squareSize - 1, (int)n);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                }
            }
        }

        private void SelectPiece()
        { //how this function will work will depent on the chosen method for hovering over, i.e. using the list or not.
            selectedID = null; //will be needed to find the chosen chess piece in the list if the list is not the selected hover over method

        }

        /// <summary>
        /// Function that calls the selected chess piece control function. 
        /// </summary>
        private void MovePiece()
        {
            chessPieces[selectedChessPiece].Control();
        }

        /// <summary>
        /// Creates the chess pieces. 
        /// </summary>
        private void CreatePieces()
        {

            for (int i = 0; i < 8; i++)
            {
                string ID = String.Format("{0}:6:{1}", team, i);
                uint[] spawn = new uint[] { spawnLocations[i, 0], spawnLocations[i, 1] };
                chessPieces.Add(new Pawn(colour, white, spawn, ID));
            }

            //for (int i = 0; i < 2; i++)
            //{ //loop that creates each piece 
            //    string rockID = String.Format("{0}:5:{1}", team, i);
            //    string bishopID = String.Format("{0}:3:{1}", team, i);
            //    string knightID = String.Format("{0}:4:{1}", team, i);
            //    //set other values for each piece and create them.
            //    //chessPieces.Add
            //}

            //string queenID = String.Format("{0}:2:{1}", team, 0);
            //string kingID = String.Format("{0}:1:{1}", team, 0);
            //string id_ = String.Format("{0}:6:{1}", team, 0);
            //uint[] spawn = new uint[] { spawnLocations[0, 0], spawnLocations[0, 1] };
            //ChessPiece pawn = new Pawn(colour, white, spawn, id_);
            //string id_2 = String.Format("{0}:6:{1}", team, 1);
            //uint[] spawn2 = new uint[] { spawnLocations[1, 0], spawnLocations[1, 1] };
            //ChessPiece pawn2 = new Pawn(colour, white, spawn2, id_2);
            //string id_3 = String.Format("{0}:5:{1}", team, 0);
            //uint[] spawn3 = new uint[] { spawnLocations[2, 0], spawnLocations[2, 1] };
            //ChessPiece rock1 = new Rock(colour, white, spawn3, id_3);
            //chessPieces.Add(pawn);
            //chessPieces.Add(pawn2);
            //chessPieces.Add(rock1);
            ChessList.SetChessList(chessPieces, white);
        }

        public bool Turn(bool turn)
        {


            return white;
        }

    }



    sealed class King : ChessPiece
    { //this one really need to keep an eye on all other pieces and their location
        public King(byte[] colour_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "^V^",
                "*|*",
                "-K-"
            };
            Draw();
        } //king cannot move next to another king

        protected override void EndLocations()
        { //implement a check for Castling and/or call the Castling function
            //is there a better way to do this than the current way. Currently it can go out of bounds. 
            //could most likely make a nested function of the do while loop
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

            if(possibleEndLocations.Count != 0)
            { //need to make sure that if a player selects the king and it cannot move, it does not prevent castling from happening. 
                SpecialBool = true;
            }

            void CheckPosistions(sbyte[] currentPosition)
            { //need to have code implemented that check each square and wether a hostile piece can take it on that square. E.g. need to check if there is a knight in a location that can jump there
                //if there is a straight line with a rock or queen on it. Diagnoal lines with a queen or a bishop. If a pawn is up or below (depending on team) and to the side of that square.
                //Lastly, it should check if the hostile king is touching that square. 
                sbyte[] loc = new sbyte[2] { currentPosition[0], currentPosition[1] };

                if ((loc[0] + mapLocation[0] > 7 || loc[0] + mapLocation[0] < 0) || (loc[1] + mapLocation[1] > 7 || loc[1] + mapLocation[1] < 0))
                {

                }
                else
                {
                    string feltID = MapMatrix.Map[loc[0] + mapLocation[0], mapLocation[1] + loc[1]];
                    if (feltID == "")
                    {
                        Add(loc);
                        loc[0] += currentPosition[0];
                        loc[1] += currentPosition[1];
                    }
                    else
                    {
                        if (teamString != feltID.Split(':')[0])
                        {
                            Add(loc);
                        }
                    }
                }


            }

            void Add(sbyte[] posistions)
            {
                possibleEndLocations.Add(new uint[,] { { (uint)(mapLocation[0] + posistions[0]) }, { (uint)(mapLocation[1] + posistions[1]) } });
            }
        }

        public bool IsInChecked() //maybe make this the specialChessPieceFunction
        { //if true, it should force the player to move it. Also, it needs to check each time the other player has made a move 
            //should also check if it even can move, if it cannot the game should end. //find the other player's chesspieces on the map matrix, look at the IDs and see if there is a clear legal move that touces the king.
            //hmm... could also look check specific squares for specific chesspieces, e.g. check all left, right, up and down squares for rocks and queen, check specific squares that 3 squares away for knights and so on. 
            //the king can, however, take a chesspiece as long time that piece is not protected by another nor is the other king. Cannot move next to the hostile king 
            return false;
        }

        protected override bool SpecialChessPieceFunction()
        {
            return Castling();
        }

        private bool Castling() //should this return a bool?
        { //king moves two squares (some say three) in the direction of the chosen rock, the rock moves to the other side of the king. Neither should have moved in the game and the space between them needs to be empty. Also, none of the squares should be threanten by
            //hostile piece??? 


            return false;
        }

    }

    sealed class Queen : ChessPiece
    {
        public Queen(byte[] colour_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "_w_",
                "~|~",
                "-Q-"
            };
            Draw();
        }

        protected override void EndLocations()
        { //implement a check for Castling and/or call the Castling function
            //is there a better way to do this than the current way. Currently it can go out of bounds. 
            //could most likely make a nested function of the do while loop
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
                        if (teamString != feltID.Split(':')[0])
                        {
                            Add(loc);
                        }
                        currentCanMove = false;
                    }

                } while (currentCanMove);
            }

            void Add(sbyte[] posistions)
            {
                possibleEndLocations.Add(new uint[,] { { (uint)(mapLocation[0] + posistions[0]) }, { (uint)(mapLocation[1] + posistions[1]) } });
            }
        }

    }

    sealed class Pawn : ChessPiece
    { //does not check if a square two away from it is empty. Can jump over another piece with the double first turn move.
        //bug: If the chess piece cannot move and have not moved and it is selected, its RemoveDraw code will be run and give an error with the oldMapLocation is null.
        //oldMapLocation is only set in the Move function and is called by RemoveDraw right after. RemoveDraw needs to be able to handle and solve null arrays. 
        private bool firstTurn = true;
        private sbyte moveDirection;
        // https://docs.microsoft.com/en-us/dotnet/standard/generics/covariance-and-contravariance
        // https://stackoverflow.com/questions/210601/accessing-a-property-of-derived-class-from-the-base-class-in-c-sharp
        //consider added a function in the base call that returns whatever is needed, e.g. can castle, has double moved and not moved after etc. Most likely being a bool. 

        public Pawn(byte[] colour_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                " - ",
                " | ",
                "-P-"
            };
            moveDirection = team ? (sbyte)1 : (sbyte)-1;
            //teamString = team ? "+" : "-";
            Draw();
        }

        /// <summary>
        /// Function that check if the pawn has double moved as its last move. If it has not, or moved after, it return negative.
        /// Needed for the en-passant rule. 
        /// </summary>
        /// <returns></returns>
        protected override bool SpecialChessPieceFunction()
        {
            /* Rules: 
             * The capturing pawn must be on its fifth rank;
             * The captured pawn must be on an adjacent file and must have just moved two squares in a single move (i.e. a double-step move);
             * The capture can only be made on the move immediately after the enemy pawn makes the double-step move; otherwise, the right to capture it en passant is lost.
             */
             //how to implement this... 
             //Got a SpecialBool get/set to use together with this one
             //If the player choose to double move with their pawn, SpecialBool is set to true.
             //This means the pawn needs to override the move function.
             //Most likely the play code need to check after a player has moved if the other player has a pawn that has doubled moved on their turn, call SpecialBool and set it to false. 
            return false;
        }

        /// <summary>
        /// A modified version of the base Move function. Designed to check if the player uses a double move. 
        /// </summary>
        protected override void Move()
        {
            oldMapLocation = null;
            bool hasSelected = false;
            EndLocations();
            if (possibleEndLocations.Count != 0)
            {
                DisplayPossibleMove();
                uint[] cursorLocation = GetMapLocation;
                do
                {
                    bool selected = FeltMove(cursorLocation);
                    if (selected)
                    {
                        foreach (uint[,] loc in possibleEndLocations)
                        {
                            uint[] endloc_ = new uint[2] { loc[0, 0], loc[1, 0] };
                            if (endloc_[0] == cursorLocation[0] && endloc_[1] == cursorLocation[1])
                            {

                                couldMove = true;
                                oldMapLocation = new uint[2] { mapLocation[0], mapLocation[1] };
                                TakeEnemyPiece(cursorLocation);
                                mapLocation = new uint[2] { cursorLocation[0], cursorLocation[1] };
                                hasSelected = true;
                                if (Math.Abs((sbyte)(oldMapLocation[1]) - (sbyte)(cursorLocation[1])) == 2)
                                {
                                    SpecialBool = true;
                                }
                                else
                                {
                                    SpecialBool = false;
                                }
                                break;
                            }
                        }
                    }
                } while (!hasSelected);
                NoneDisplayPossibleMove();
                possibleEndLocations.Clear();
            }
            else
            {
                couldMove = false;
            }
        }

        public override void Control()
        {
            Move();
            RemoveDraw(oldMapLocation);
            LocationUpdate();
            Draw();
            UpdateMapMatrix(oldMapLocation);
            Promotion();
        }

        protected override void EndLocations()
        {
            //if (possibleEndLocations.Count != 0)
            //    possibleEndLocations.Clear();
            if ((!team && mapLocation[1] != 0) || (team && mapLocation[1] != 7))
                if (MapMatrix.Map[mapLocation[0], mapLocation[1] + moveDirection] == "")
                {
                    possibleEndLocations.Add(new uint[,] { { mapLocation[0] }, { (uint)(mapLocation[1] + moveDirection) } });
                }
            if (firstTurn)
            {
                if (MapMatrix.Map[mapLocation[0], (mapLocation[1] + moveDirection * 2)] == "" && MapMatrix.Map[mapLocation[0], (mapLocation[1] + moveDirection)] == "")
                {
                    possibleEndLocations.Add(new uint[,] { { mapLocation[0] }, { (uint)(mapLocation[1] + moveDirection * 2) } });
                }
                CheckAttackPossbilities();
                firstTurn = false;
            }
        }

        /// <summary>
        /// Checks if there possible hostile piece that can be taken. If there is, they locations are added to the possibleEndLocations.
        /// </summary>
        private void CheckAttackPossbilities() //consider making it take an array, so it can be called in the firstTurn code regarding the en-passant rule. 
        { //again, since the list of pieces only show the base functions and not the pawn functions, it will become a problem
            if ((!team && mapLocation[1] != 0) || (team && mapLocation[1] != 7))
            {
                if (mapLocation[0] != 0) //check square to the left side
                    LocationCheck(-1);
                if (mapLocation[0] != 7) //check square to the right side
                    LocationCheck(1);
                if (firstTurn)
                {
                    EnPassant();
                }
            }

            void LocationCheck(sbyte direction) //find a better name
            {
                string locationID = MapMatrix.Map[mapLocation[0] + direction, mapLocation[1] + moveDirection];
                if (locationID != "")
                {
                    string teamID = locationID.Split(':')[0];
                    if (teamID != teamString)
                        possibleEndLocations.Add(new uint[,] { { (uint)(mapLocation[0] + direction) }, { (uint)(mapLocation[1] + moveDirection) } });
                }
            }
            //read up on the "en-passant" rule regarding taking another pawn that has double moved. With the current functions you cannot check if a double move have been made this turn by a pawn or not, only if it has moved or not

            void EnPassant()
            { //needs not have to moved
                byte chessAmount = (byte)ChessList.GetList(!team).Count;
                for (byte i = 0; i < chessAmount; i++) //goes through all chess pieces
                {
                    string idCheck = ChessList.GetList(!team)[i].GetID;
                    if (idCheck.Split(':')[1] == "6") //checks if the current piece is a pawn
                    {
                        if (ChessList.GetList(!team)[i].SpecialBool) //checks if the pawn as double moved and en passant is allowed.
                        {
                            uint[] hostileLocation = ChessList.GetList(!team)[i].GetMapLocation;
                            if (hostileLocation[0] == mapLocation[0] + 1 || hostileLocation[0] == mapLocation[0] - 1) //Checks if the pawn is a location that allows it to be en passant.  
                            {
                                possibleEndLocations.Add(new uint[,] { { hostileLocation[0] }, { hostileLocation[1] } });
                            }
                        }
                    }
                }

            }

        }

        private bool HasDoubleSquareMoved { get; set; }

        private void Promotion()
        {
            if ((!team && mapLocation[1] == 0) || (team && mapLocation[1] == 7))
            {
                Taken();
                //how should the selection be designed? Text written below the board? Next to the board? How to select? Arrowkeys? Numberkeys? Written?
                DisplayPromotions();

                //test code
                ChessList.GetList(team).Add(new Queen(colour, team, mapLocation, GetID)); //the final version should update the ID to match the new chesspiece. This leaves a question for building up the ID, e.g. if the pawn is pawn number 1, team white,
                //and it becomes a queen, it cannot be set as +:2:1 as the start queen is already that. 
                //Since the last part of the ID is never used by anything else than in combination with the entire ID, maybe add a symbol after. The symbol could be the chesspiece type part of the ID, e.g. 2 for queen
                //so it will becokme +:2:12
            }

        }

        private void DisplayPromotions()
        { //writes to a location what chesspieces it can be promoted too.
            string[] promotions =
            {
                "Knight",
                "Bishop",
                "Rock",
                "Queen"
            };
        }

    }

    sealed class Rock : ChessPiece
    {
        private bool hasMoved = false;
        public Rock(byte[] colour_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "^^^",
                "|=|",
                "-R-"
            };
            Draw();
        }

        protected override void EndLocations()
        { //implement a check for Castling and/or call the Castling function
            //is there a better way to do this than the current way. Currently it can go out of bounds. 
            //could most likely make a nested function of the do while loop
            sbyte[] position = new sbyte[2] { -1, 0 };
            CheckPosistions(position); //left

            position = new sbyte[2] { 1, 0 };
            CheckPosistions(position); //right

            position = new sbyte[2] { 0, -1 };
            CheckPosistions(position); //up

            position = new sbyte[2] { 0, 1 };
            CheckPosistions(position); //down

            hasMoved = true;

            if (possibleEndLocations.Count != 0)
            { //need to make sure that if a player selects the rock and it cannot move, it does not prevent castling from happening. 
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
                        if (teamString != feltID.Split(':')[0])
                        {
                            Add(loc);
                        }
                        currentCanMove = false;
                    }

                } while (currentCanMove);
            }

            void Add(sbyte[] posistions)
            {
                possibleEndLocations.Add(new uint[,] { { (uint)(mapLocation[0] + posistions[0]) }, { (uint)(mapLocation[1] + posistions[1]) } });
            }
        }

        private bool HasMoved { get => hasMoved; } //if moved, castling cannot be done. How is the king going to call this code. Currently, the king would only be able to call other's functions that are given in the base class.

        private bool Castling()
        {


            return false;
        }

    }

    sealed class Bishop : ChessPiece
    {
        public Bishop(byte[] colour_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "_+_",
                "|O|",
                "-B-"
            };
            Draw();
        }

        protected override void EndLocations()
        {
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
                        if (teamString != feltID.Split(':')[0])
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
                possibleEndLocations.Add(new uint[,] { { (uint)(mapLocation[0] + posistions[0]) }, { (uint)(mapLocation[1] + posistions[1]) } });
            }
        }
    }

    sealed class Knight : ChessPiece
    {
        public Knight(byte[] colour_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        { 
            Design = new string[]
            {
                " ^_",
                " |>",
                "-k-"
            };
            Draw();

        }

        protected override void EndLocations()
        { //there must be a better way to do this...
            sbyte[] potenieltLocation = { -2, -1 }; //2 down left
            if (mapLocation[0] + potenieltLocation[0] >= 0 && mapLocation[1] + potenieltLocation[1] >= 0)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2]{ 2, - 1}; //2 up left
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
            potenieltLocation = new sbyte[2] { -2 , 1 }; //2 down right
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
                if(CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }

            bool CheckPosistions(sbyte[] posistions)
            { //returns true if the piece can legal move there, false otherwise. 
                string feltID = MapMatrix.Map[mapLocation[0] + posistions[0], mapLocation[1] + posistions[1]];
                if (feltID != "")
                {
                    if (feltID.Split(':')[0] != teamString)
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
                possibleEndLocations.Add(new uint[,] { { (uint)(mapLocation[0] + posistions[0]) }, { (uint)(mapLocation[1] + posistions[1]) } });
            }
        }

    }



    abstract public class ChessPiece //still got a lot to read and learn about what is the best choice for a base class, class is abstract, everything is abstract, nothing is abstract and so on. 
    {//when put on a location, check if there is an allie, if there is invalid move, if enemy, call that pieces removeDraw and call their Taken using TakeEnemyPiece
        protected uint[] location = new uint[2]; //x,y
        protected byte[] colour; // https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/inheritance 
        protected string[] design;
        protected bool team;
        protected uint[] mapLocation;
        protected uint[] oldMapLocation;
        protected string id;
        protected bool hasBeenTaken = false;
        protected byte squareSize = Settings.SquareSize;
        protected List<uint[,]> possibleEndLocations = new List<uint[,]>();
        protected string teamString; //come up with a better name
        protected bool couldMove;
        protected bool specialBool; 
        //https://en.wikipedia.org/wiki/Chess_piece_relative_value if you ever want to implement an AI this could help 

        public ChessPiece(byte[] colour_, bool team_, uint[] mapLocation_, string ID)
        { 
            Colour = colour_;
            SetTeam(team_);
            MapLocation = mapLocation_; 
            this.ID = ID; //String.Format("{0}n:{1}", team, i); team = team_ == true ? "-" : "+"; n being the chesspiece type
            LocationUpdate();
            MapMatrix.Map[mapLocation[0], mapLocation[1]] = ID;
            teamString = ID.Split(':')[0];
        }

        /// <summary>
        /// Returns a bool that indicate if this piece has been "taken" by another player's piece. 
        /// </summary>
        public bool BeenTaken { get => hasBeenTaken; } //use by other code to see if the piece have been "taken" and should be removed from game. 

        /// <summary>
        /// Sets the hasBeenTaken value...
        /// </summary>
        protected bool SetBeenTaken { set => hasBeenTaken = value; }

        /// <summary>
        /// Gets and sets the location of the chesspiece on the console.  
        /// </summary>
        protected uint[] Location
        {
            set
            {
                location = value;
            }
            get
            {
                uint[] loc = new uint[2] { location[0], location[1] };
                return loc;
                //return location;
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

        protected uint[] MapLocation { set => mapLocation = value; }

        public uint[] GetMapLocation
        {
            get
            {
                uint[] mapLo = new uint[2] { mapLocation[0], mapLocation[1] };
                return mapLo;
            }
        }

        /// <summary>
        /// Gets and set the ID of the chesspiece. //maybe have some code that ensures the ID is unique 
        /// </summary>
        protected string ID { get => id; set => id = value; } //maybe split into two. Set being protected and the Get being public 

        public string GetID { get => ID; }

        public bool CouldMove { get => couldMove; }

        public bool SpecialBool { get => specialBool; set => specialBool = value; }

        /// <summary>
        /// Function that "controls" a piece. What to explain and how to..
        /// </summary>
        public virtual void Control()
        {
            Move();
            RemoveDraw(oldMapLocation);
            LocationUpdate();
            Draw();
            UpdateMapMatrix(oldMapLocation);
        }

        /// <summary>
        /// The function of this function depends on the chesspiece. Rock, pawn, and king got different implementations.
        /// </summary>
        /// <returns></returns>
        protected virtual bool SpecialChessPieceFunction()
        {

            return false;
        }

        /// <summary>
        /// Calculates the, legal, end locations that a chess piece can move too.
        /// </summary>
        protected virtual void EndLocations()
        {

        }

        /// <summary>
        /// Allows the chesspiece to move. 
        /// </summary>
        protected virtual void Move() 
        {
            oldMapLocation = null;
            bool hasSelected = false;
            EndLocations();
            if (possibleEndLocations.Count != 0)
            {
                DisplayPossibleMove();
                uint[] cursorLocation = GetMapLocation;
                do
                {
                    bool selected = FeltMove(cursorLocation);
                    if (selected)
                    {
                        foreach (uint[,] loc in possibleEndLocations)
                        {
                            uint[] endloc_ = new uint[2] { loc[0, 0], loc[1, 0] };
                            if (endloc_[0] == cursorLocation[0] && endloc_[1] == cursorLocation[1])
                            {
                                couldMove = true;
                                oldMapLocation = new uint[2] { mapLocation[0], mapLocation[1] };
                                TakeEnemyPiece(cursorLocation);
                                mapLocation = new uint[2] { cursorLocation[0], cursorLocation[1] };
                                hasSelected = true;
                                break;
                            }
                        }
                    }
                } while (!hasSelected);
                NoneDisplayPossibleMove();
                possibleEndLocations.Clear();
            }
            else
            {
                couldMove = false;
            }
        }

        /// <summary>
        /// Checks if <paramref name="locationToCheck"/> contain an ID and if the ID is hostile, the function will call that ID's chesspiece's Taken function.
        /// </summary>
        /// <param name="locationToCheck">The location to check for a chess piece</param>
        protected void TakeEnemyPiece(uint[] locationToCheck)
        {
            string feltID = MapMatrix.Map[locationToCheck[0], locationToCheck[1]];
            if (feltID != "")
                if (teamString != feltID.Split(':')[0])
                {
                    foreach (ChessPiece chessHostile in ChessList.GetList(!team))
                    {
                        if (chessHostile.GetID == feltID)
                        {
                            chessHostile.Taken();
                        }
                    }
                }
        }

        /// <summary>
        /// Allows the chesspiece to select different sqaures on the board. If enter is pressed on a square the chesspiece can move too, it will move to that square. 
        /// </summary>
        /// <param name="currentLocation">The current location of hovered over square. </param>
        /// <returns>Returns true if enter is pressed, else false.</returns>
        protected bool FeltMove(uint[] currentLocation)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            SquareHighLight(false, currentLocation);
            foreach (uint[,] loc in possibleEndLocations)
            {
                uint[] endloc_ = new uint[2] { loc[0, 0], loc[1, 0] };
                if (endloc_[0] == currentLocation[0] && endloc_[1] == currentLocation[1])
                {
                    PaintBackground(Settings.SelectMoveSquareColour, loc);
                    break;
                }
            }
            if (keyInfo.Key == ConsoleKey.UpArrow && currentLocation[1] > 0)
            {
                currentLocation[1]--;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow && currentLocation[1] < 7)
            {
                currentLocation[1]++;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow && currentLocation[0] > 0)
            {
                currentLocation[0]--;
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow && currentLocation[0] < 7)
            {
                currentLocation[0]++;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                return true;
            }
            SquareHighLight(true, currentLocation);
            return false;

        }

        /// <summary>
        /// Highlights or dehighlights a sqare.
        /// </summary>
        /// <param name="isHighlighted">If true, the square at <paramref name="currentLocation"/> is highlighted, else it is not highlighted.</param>
        /// <param name="currentLocation">The location to (de)highlight.</param>
        protected void SquareHighLight(bool isHighlighted, uint[] currentLocation)
        {
            byte squareSize = Settings.SquareSize;
            uint startLocationX = currentLocation[0] * squareSize + (currentLocation[0] + 1) * 1 + Settings.Offset[0];
            uint startLocationY = currentLocation[1] * squareSize + (currentLocation[1] + 1) * 1 + Settings.Offset[1];
            if (isHighlighted)
            {
                byte[] colour = Settings.SelectSquareColour;
                Paint(colour);
            }
            else
            {
                byte colorLocator = (byte)((currentLocation[0] + currentLocation[1]) % 2);
                byte[] colour = colorLocator == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
                Paint(colour);
            }

            void Paint(byte[] colour)
            {
                for (uint n = startLocationX; n < startLocationX + squareSize; n++)
                {
                    Console.SetCursorPosition((int)n, (int)startLocationY);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                    Console.SetCursorPosition((int)n, (int)startLocationY + squareSize - 1);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                }
                for (uint n = startLocationY; n < startLocationY + squareSize; n++)
                {
                    Console.SetCursorPosition((int)startLocationX, (int)n);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                    Console.SetCursorPosition((int)startLocationX + squareSize - 1, (int)n);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                }
            }
        }

        /// <summary>
        /// updates the location that is used for displaying the chesspiece on the chessboard
        /// </summary>
        protected void LocationUpdate()
        {
            Location = new uint[2] { mapLocation[0] * squareSize + (mapLocation[0] + Settings.Spacing) * 1 + Settings.Offset[0], mapLocation[1] * squareSize + (mapLocation[1] + Settings.Spacing) * 1 + Settings.Offset[1] };
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
        public void IsHoveredOn(bool hover)
        {
            if (hover)
            { //a lot of this code is the same as in other functions, e.g. RemoveDraw and Draw, just with other colours. Consider making a new function with this code and parameters for colour.
                PaintBoth();
            }
            else
                Draw();
        }

        /// <summary>
        /// Calculates the paint location and the paint colour. Paint colour is calculated out from <paramref name="mapLoc"/>.
        /// </summary>
        /// <param name="drawLocationX">The x start posistion.</param>
        /// <param name="drawLocationY">The y start posistion.</param>
        /// <param name="mapLoc">The location on the map matrix that is used to determine background colour.</param>
        /// <returns></returns>
        protected byte[] PaintCalculations(out int drawLocationX, out int drawLocationY, uint[] mapLoc)
        { //replace a lot of code with this function
            byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
            drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            uint locationForColour = (mapLoc[0] + mapLoc[1]) % 2; //if zero, background colour is "white", else background colour is "black".
            byte[] colours = locationForColour == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
            return colours;
        }

        /// <summary>
        /// Paints the foreground of the current location.
        /// </summary>
        protected void PaintForeground() //these did not really end up saving on code. Consider moving the first 3 or 5 lines of code into a function...
        {

            byte[] colours = PaintCalculations(out int drawLocationX, out int drawLocationY, mapLocation);
            for (int i = 0; i < design[0].Length; i++) //why does all the inputs, length, count and so on use signed variable types... 
            { //To fix, the background colour is overwritten with the default colour, black, rather than keeping the current background colour.
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("\x1b[48;2;" + colours[0] + ";" + colours[1] + ";" + colours[2] + "m\x1b[38;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m{0}\x1b[0m", design[i], colours);
            }
        }

        /// <summary>
        /// Paints both the background and foreground of the current location. 
        /// </summary>
        protected void PaintBoth()
        {
            byte[] backColours = PaintCalculations(out int drawLocationX, out int drawLocationY, mapLocation);
            byte[] colours = Settings.SelectPieceColour;
            for (int i = 0; i < design[0].Length; i++)
            {
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("\x1b[48;2;" + backColours[0] + ";" + backColours[1] + ";" + backColours[2] + "m\x1b[38;2;" + colours[0] + ";" + colours[1] + ";" + colours[2] + "m{0}\x1b[0m", design[i]);
            }
        }


        /// <summary>
        /// Removes the visual identication of a chesspiece at its current location.
        /// </summary>
        protected void RemoveDraw(uint[] locationToRemove)
        {
            if (locationToRemove != null)
            {
                byte[] colours = PaintCalculations(out int drawLocationX, out int drawLocationY, locationToRemove);
                for (int i = 0; i < design[0].Length; i++)
                {
                    Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                    Console.Write("\x1b[48;2;" + colours[0] + ";" + colours[1] + ";" + colours[2] + "m".PadRight(design[0].Length + 1, ' ') + "\x1b[0m");
                }
            }
        }

        /// <summary>
        /// updates the map matrix with the new location of the chess piece and sets the old location to zero. 
        /// </summary>
        protected void UpdateMapMatrix(uint[] oldMapLocation) //need to call this before the LocationUpdate
        {
            if (oldMapLocation != null)
            {
                MapMatrix.Map[mapLocation[0], mapLocation[1]] = ID;
                MapMatrix.Map[oldMapLocation[0], oldMapLocation[1]] = "";
            }
        }


        /// <summary>
        /// Set a chesspeice set to be taken so it can be removed and removes its visual representation. 
        /// </summary>
        public void Taken()
        {//call by another piece, the one that takes this piece. 
            hasBeenTaken = true;
            MapMatrix.Map[mapLocation[0], mapLocation[1]] = "";
            RemoveDraw(mapLocation);
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
        protected void NoneDisplayPossibleMove()
        {
            foreach (uint[,] end in possibleEndLocations)
            {
                byte colourLoc = (byte)((end[0, 0] + end[1, 0]) % 2);
                byte[] backColour = colourLoc == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
                PaintBackground(backColour, end);
            }
        }

        /// <summary>
        /// Display the possible, legal, moves a chesspiece can take. 
        /// </summary>
        protected void DisplayPossibleMove()
        {
            foreach (uint[,] end in possibleEndLocations)
            {
                PaintBackground(Settings.SelectMoveSquareColour, end);
            }

        }

        /// <summary>
        /// Paints the background. 
        /// </summary>
        /// <param name="colour">Colour of the background.</param>
        /// <param name="locationEnd">Array of locations. </param>
        protected void PaintBackground(byte[] colour, uint[,] locationEnd)
        {
            byte squareSize = Settings.SquareSize;
            uint startLocationX = locationEnd[0, 0] * squareSize + (locationEnd[0, 0] + 1) * 1 + Settings.Offset[0];
            uint startLocationY = locationEnd[1, 0] * squareSize + (locationEnd[1, 0] + 1) * 1 + Settings.Offset[1];
            for (uint n = startLocationX; n < startLocationX + squareSize; n++)
            {
                Console.SetCursorPosition((int)n, (int)startLocationY);
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                Console.SetCursorPosition((int)n, (int)startLocationY + squareSize - 1);
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
            }
            for (uint n = startLocationY; n < startLocationY + squareSize; n++)
            {
                Console.SetCursorPosition((int)startLocationX, (int)n);
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                Console.SetCursorPosition((int)startLocationX + squareSize - 1, (int)n);
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
            }
        }

    }


}
