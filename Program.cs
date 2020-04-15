﻿using System;
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
            whiteSpawnLocation = new uint[,] { { 1, 1 }, { 2, 1 }, { 0, 0 } };
            blackSpawnLocation = new uint[,] { { 1, 6 }, { 2, 6 }, { 0, 7 } };
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
                for (int i = ChessList.GetList(false).Count-1; i >= 0; i--)
                {
                    if (ChessList.GetList(false)[i].BeenTaken)
                        ChessList.GetList(false).RemoveAt(i);
                }
                black.Control();
                for (int i = ChessList.GetList(true).Count - 1; i >= 0; i--)
                {
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

            //for (int i = 0; i < 8; i++)
            //{//loop that creates each piece 
            //    string pawnID = String.Format("{0}:6:{1}", team, i);
            //    //set other values for each piece and create them.
            //    //chessPieces.Add
            //}

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
            string id_ = String.Format("{0}:6:{1}", team, 0);
            uint[] spawn = new uint[] { spawnLocations[0, 0], spawnLocations[0, 1] };
            ChessPiece pawn = new Pawn(colour, white, spawn, id_);
            string id_2 = String.Format("{0}:6:{1}", team, 1);
            uint[] spawn2 = new uint[] { spawnLocations[1, 0], spawnLocations[1, 1] };
            ChessPiece pawn2 = new Bishop(colour, white, spawn2, id_2);
            string id_3 = String.Format("{0}:5:{1}", team, 0);
            uint[] spawn3 = new uint[] { spawnLocations[2, 0], spawnLocations[2, 1] };
            ChessPiece rock1 = new Rock(colour, white, spawn3, id_3);
            chessPieces.Add(pawn);
            chessPieces.Add(pawn2);
            chessPieces.Add(rock1);
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

        public bool IsInChecked()
        { //if true, it should force the player to move it. Also, it needs to check each time the other player has made a move 
            //should also check if it even can move, if it cannot the game should end. //find the other player's chesspieces on the map matrix, look at the IDs and see if there is a clear legal move that touces the king.
            //hmm... could also look check specific squares for specific chesspieces, e.g. check all left, right, up and down squares for rocks and queen, check specific squares that 3 squares away for knights and so on. 
            //the king can, however, take a chesspiece as long time that piece is not protected by another nor is the other king. Cannot move next to the hostile king 
            return false;
        }

        private bool HasMoved { get; } //if moved, castling cannot be done

        private bool Castling()
        { //king moves two squares in the direction of the chosen rock, the rock moves to the other side of the king. Neither should have moved in the game and the space between them needs to be empty. Also, none of the squares should be threanten by
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

    }

    sealed class Pawn : ChessPiece
    { //does not check if a square two away from it is empty. 
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
                if (MapMatrix.Map[mapLocation[0], (mapLocation[1] + moveDirection * 2)] == "")
                {
                    possibleEndLocations.Add(new uint[,] { { mapLocation[0] }, { (uint)(mapLocation[1] + moveDirection * 2) } });
                }
                firstTurn = false;
            }
            CheckAttackPossbilities();
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
        }

        private bool HasDoubleSquareMoved { get; set; }

        private void Promotion()
        { //pawn can become any other type of chesspiece. It should remove itself and create a new instance of the chosen piece on the same location.
            if ((!team && mapLocation[1] == 0) || (team && mapLocation[1] == 7))
            {
                //how should the selection be designed? Text written below the board? Next to the board? How to select? Arrowkeys? Numberkeys? 
                DisplayPromotions();
            }

        }

        private void DisplayPromotions()
        { //writes to a location what chesspieces it can be promoted too.

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
            sbyte[] position = new sbyte[2] {-1,0 };
            CheckPosistions(position); //left

            position = new sbyte[2] { 1, 0 };
            CheckPosistions(position); //right

            position = new sbyte[2] { 0, -1 };
            CheckPosistions(position); //up

            position = new sbyte[2] { 0, 1 };
            CheckPosistions(position); //down

            hasMoved = true;

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

        //Knight(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : this(location_, colour_, design_, team_, spawnLocation_, ID) { }

        public Knight(byte[] colour_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        { //maybe do not have the moves and attacks, design and suck as parameters, but rather part of the code, since you have changed from abstract to non abstract class
          //redo the constructors when you are sure what you will need. So far: spawnlocation, id and team
            Design = new string[]
            {
                " ^_",
                " |>",
                "-k-"
            };
            MovePattern = new byte[][] { new byte[] { 6 } }; //maybe drop the idea of a movePattern variable and just write it into the specialised move code 
            Draw();

        }


    }



    abstract public class ChessPiece //still got a lot to read and learn about what is the best choice for a base class, class is abstract, everything is abstract, nothing is abstract and so on. 
    {//when put on a location, check if there is an allie, if there is invalid move, if enemy, call that pieces removeDraw and call their Taken using TakeEnemyPiece
        protected uint[] location = new uint[2]; //x,y
        protected byte[] colour; // https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/inheritance 
        protected string[] design;
        protected byte[][] movePattern;
        protected byte[][] attack; //in almost everycase it is the same as movePattern, so it can be set to null. If it is not null, i.e. pawn, it should be called when moving if there are enemies at the right location
        protected bool team;
        protected uint[] mapLocation;
        protected uint[] oldMapLocation; 
        protected string id;
        protected bool canDoMove;
        protected bool hasBeenTaken = false;
        protected byte squareSize = Settings.SquareSize;
        protected List<uint[,]> possibleEndLocations = new List<uint[,]>();
        protected string teamString; //come up with a better name
        protected bool couldMove; 

        public ChessPiece(byte[] colour_, bool team_, uint[] mapLocation_, string ID)
        { //for testing the code, just create a single player and a single piece. 
            //location should be the console x,y values, but instead of being given, it should be calculated out from the maplocation and square size
            Colour = colour_;
            SetTeam(team_);
            MapLocation = mapLocation_; //what should this actually be done, is it the actually values on the console or is it values that fits the map matrix and location is then the actually console location...
            this.ID = ID; //String.Format("{0}n:{1}", team, i); team = currentTurn == true ? "-" : "+"; n being the chesspiece type
            LocationUpdate();
            MapMatrix.Map[mapLocation[0], mapLocation[1]] = ID;
            teamString = team ? "+" : "-";
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

        } //consider for each of the properties what kind they should have

        /// <summary>
        /// sets the colour of the chesspiece.
        /// </summary>
        protected byte[] Colour { set => colour = value; }

        /// <summary>
        /// Sets and gets the design of the chesspieice. 
        /// </summary>
        protected string[] Design { get => design; set => design = value; }

        //remove 
        protected byte[][] MovePattern { set => movePattern = value; }

        //remove 
        protected byte[][] AttackPattern { get => attack; set => attack = value; }

        protected bool Team { get => team; } //need to know it own team, but does it need to know the others's teams, the IDs can be used for that or just the matrix map. 

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

        protected bool CanDoMove { get => canDoMove; set => canDoMove = value; } //what was the canDoMove suppose to be for again?

        public bool CouldMove { get => couldMove; }

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
        /// Calculates the, legal, end locations that a chess piece can move too.
        /// </summary>
        protected virtual void EndLocations()
        {

        }

        /// <summary>
        /// Calculates the, legal, location(s) the chesspiece is able to move too. 
        /// </summary>
        protected virtual void Move() //Currently play problem. If the piece cannot move, the player will become stuck on it. Need to ensrue that if there are no legal moves, it should return the control to the player and let them select another option
        {
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

        protected void TakeEnemyPiece(uint[] locationToCheck)
        {
            string feltID = MapMatrix.Map[locationToCheck[0], locationToCheck[1]];
            if (feltID != "")
                if (teamString != feltID.Split(':')[0])
                {
                    foreach (ChessPiece chessHostile in ChessList.GetList(!team))
                    {
                      if(chessHostile.GetID == feltID)
                        {
                            chessHostile.Taken();
                        }  
                    }
                } 
        }

        protected bool FeltMove(uint[] currentLocation) //currently it overwrites the colour of an endlocation. Endlocations need to be repainted, but what is the best method? Repaint all? Track down the one to repaint? 
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            SquareHighLight(false, currentLocation);
            foreach (uint[,] loc in possibleEndLocations)
            {
                uint[] endloc_ = new uint[2] { loc[0, 0], loc[1, 0] };
                if(endloc_[0] == currentLocation[0] && endloc_[1] == currentLocation[1])
                {
                    Paint(Settings.SelectMoveSquareColour, loc);
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
            //Location[0] = mapLocation[0] * squareSize + (mapLocation[0] + 1) * 1 + Settings.Offset[0]; //(matpLocation[0]+1) is for the amount of spaces between the offsets and first square and the space between all the squares
            //7*5+8+2 = 35+10 = 45, x
            //1*5+2+2 = 5+4 = 9, y 
            /*{1,2}
             * 1*5+2+2 = 9
             * 2*5+3+2 = 15
             * {0,0}
             * 0*5+1+2 = 3
             * 0*5+1+2 = 3
             */
            //Location[1] = mapLocation[1] * squareSize + (mapLocation[1] + 1) * 1 + Settings.Offset[1];
            Location = new uint[2] { mapLocation[0] * squareSize + (mapLocation[0] + Settings.Spacing) * 1 + Settings.Offset[0] , mapLocation[1] * squareSize + (mapLocation[1] + Settings.Spacing) * 1 + Settings.Offset[1] };
        }

        /// <summary>
        /// Draws the chesspiece at its specific location
        /// </summary>
        protected void Draw()
        {
            byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
            int drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            int drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            uint locationForColour = (mapLocation[0] + mapLocation[1]) % 2; //if zero, background colour is "white", else background colour is "black".
            byte[] colours = locationForColour == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
            for (int i = 0; i < design[0].Length; i++) //why does all the inputs, length, count and so on use signed variable types... 
            { //To fix, the background colour is overwritten with the default colour, black, rather than keeping the current background colour.
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("\x1b[48;2;" + colours[0] + ";" + colours[1] + ";" + colours[2] + "m\x1b[38;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m{0}\x1b[0m", design[i], colours);
            }
        }

        /// <summary>
        /// Displays a piece in another colour if it is hovered over. 
        /// </summary>
        /// <param name="hover">If true, the piece will be coloured in a different colour. If false, the piece will have its normal colour.</param>
        public void IsHoveredOn(bool hover) //when a player hovers over a piece all this code with true, if and when they move to another piece call this piece again but with false 
        { //consider allowing a custom colour or just inverse colour. 
            if (hover)
            {
                byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
                int drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
                int drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
                uint locationForColour = (mapLocation[0] + mapLocation[1]) % 2; //if zero, background colour is "white", else background colour is "black".
                byte[] backColours = locationForColour == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
                byte[] colours = Settings.SelectPieceColour;
                for (int i = 0; i < design[0].Length; i++)
                {
                    Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                    Console.Write("\x1b[48;2;" + backColours[0] + ";" + backColours[1] + ";" + backColours[2] + "m\x1b[38;2;" + colours[0] + ";" + colours[1] + ";" + colours[2] + "m{0}\x1b[0m", design[i], colours);
                }
            }
            else
                Draw();
        }

        /// <summary>
        /// removes the visual identication of a chesspiece at its current location.
        /// </summary>
        protected void RemoveDraw(uint[] locationToRemove)
        {
            byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
            int drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            int drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            uint locationForColour = (locationToRemove[0] + locationToRemove[1]) % 2; //if zero, background colour is "white", else background colour is "black".
            byte[] colours = locationForColour == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
            for (int i = 0; i < design[0].Length; i++)
            {
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("\x1b[48;2;" + colours[0] + ";" + colours[1] + ";" + colours[2] + "m".PadRight(design[0].Length+1, ' ') + "\x1b[0m");
            }
        }

        /// <summary>
        /// updates the map matrix with the new location of the chess piece and sets the old location to zero. 
        /// </summary>
        protected void UpdateMapMatrix(uint[] oldMapLocation) //need to call this before the LocationUpdate
        {
            MapMatrix.Map[mapLocation[0], mapLocation[1]] = ID;
            MapMatrix.Map[oldMapLocation[0], oldMapLocation[1]] = "";
        }


        /// <summary>
        /// Set a chesspeice set to be taken so it can be removed. 
        /// </summary>
        public void Taken()
        {//call by another piece, the one that takes this piece. 
            hasBeenTaken = true;
            //it should remove itself from the map matrix. 
            RemoveDraw(mapLocation); //if the piece is taken, the other piece stands on this ones location, so removeDraw might remove the other piece. Consider how to implement the Taken/Move regarding that. 
        }

        /// <summary>
        /// Sets the team of the chesspiece.
        /// </summary>
        /// <param name="team_">True for .... False for ....</param>
        protected void SetTeam(bool team_)
        {
            team = team_;
        }


        protected void NoneDisplayPossibleMove()
        {
            foreach (uint[,] end in possibleEndLocations)
            {
                byte colourLoc = (byte)((end[0, 0] + end[1, 0]) % 2);
                byte[] backColour = colourLoc == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
                Paint(backColour, end);
            }
        }

        /// <summary>
        /// Display the possible, legal, moves a chesspiece can take. 
        /// </summary>
        protected void DisplayPossibleMove()
        {
            foreach (uint[,] end in possibleEndLocations)
            {
                Paint(Settings.SelectMoveSquareColour, end);
            }
            //needs to draw at every end location
            //what should be drawn, where should it and how to restore back to the default design and colour
        }

        protected void Paint(byte[] colour, uint[,] locationEnd)
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

    //    protected void TakeEnemyPiece()
    //    {
    //        //consider this aproach: Player select a location. This chesspiece then checks the location for an ID string or "". If "" call the removeDraw, move the piece and call Draw.
    //        //If there is an ID string, find that chesspiece and call its Taken. Then call removeDraw, move the piece and the call Draw. 
    //        string newLocationCurrentValue = MapMatrix.Map[mapLocation[0], mapLocation[1]]; //should the map have been updated already or should this line of code some new location
    //        if (newLocationCurrentValue != "")
    //        {
    //            foreach (ChessPiece chesspiece in ChessList.GetList(!Team)) //remember to ensure it gets the other teams list... so at some point figure out if false is white or black...
    //            {
    //                if (chesspiece.ID == newLocationCurrentValue)
    //                {
    //                    chesspiece.Taken(); //huh, got access to all functions and class scope values... guess that makes sense... Figure out some way to prevent that, so read
    //                    break;
    //                }
    //            }
    //        }
    //        RemoveDraw(mapLocation);
    //        //UpdateMapMatrix();
    //        //call move function
    //        Draw();


    //    }

    }


}
