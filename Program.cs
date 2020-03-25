using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Chess
{
    class Program
    {
        static void Main(string[] args)
        {
            ChessTable chess = new ChessTable();
            
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
        private uint[,] whiteSpawnLocation;
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
            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);

            squareSize = 5;
            lineColour = new byte[] {122,122,122 };
            lineColourBase = new byte[] { 87, 65, 47 };
            squareColour1 = new byte[] {182,123,91 };
            squareColour2 = new byte[] {135,68,31 };
            offset = new byte[] {2,2 };

            windowsSize[0] = (byte)(9 + 8 * squareSize + 10);
            windowsSize[1] = (byte)(9 + 8 * squareSize + 10);
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);
            BoardSetup();
        }

        private void BoardSetup()
        {//8 squares in each direction. Each piece is 3*3 currently, each square is 5*5 currently. 
            Console.CursorVisible = false;
            ushort distance = (ushort)(9 + 8 * squareSize);
            for (int k = 0; k < distance; k++)
                for (int i = 0; i < distance; i += 1 + squareSize)
                {
                    Console.SetCursorPosition(i + offset[0], k + offset[1]);
                    Console.Write("\x1b[48;2;" + lineColourBase[0] + ";" + lineColourBase[1] + ";" + lineColourBase[2] + "m ");
                    Console.SetCursorPosition(i + offset[0], k + offset[1]);
                    Console.Write("\x1b[38;2;" + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m|" + "\x1b[0m");
                }
            for(int k = 0; k < distance; k += 1 + squareSize)
                for (int i = 1; i < distance-1; i++)
                {
                    Console.SetCursorPosition(i + offset[0], k + offset[1]);
                    Console.Write("\x1b[48;2;" + lineColourBase[0] + ";" + lineColourBase[1] + ";" + lineColourBase[2] + "m ");
                    Console.SetCursorPosition(i + offset[0], k + offset[1]);
                    Console.Write("\x1b[38;2;" + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m-" + "\x1b[0m");
                }
            BoardColouring();
        }

        private void BoardColouring()
        {
            //need to be inside of a loop that moves it to the different places there should be coloured
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
                            Console.SetCursorPosition(i + offset[0] + 1 + n, k + offset[1] + 1 + m);
                            if( location % 2 == 0)
                                Console.Write("\x1b[48;2;" + squareColour1[0] + ";" + squareColour1[1] + ";" + squareColour1[2] + "m " + "\x1b[0m");
                            else if (location % 2 == 1)
                                Console.Write("\x1b[48;2;" + squareColour2[0] + ";" + squareColour2[1] + ";" + squareColour2[2] + "m " + "\x1b[0m");
                        }
                    }
                    location++;
                }
                location++;
            }

        }

        private void PlayerSetup()
        {
            byte[] colourWhite =
            {
                255,255,255
            };
            white = new Player(colourWhite,true,whiteSpawnLocation);
            byte[] colourBlack =
            {
                0,0,0
            };
            black = new Player(colourBlack,false,blackSpawnLocation);
        }

    }

    class Player
    { //this class is set to be an abstract in the UML, but is that really needed? 
        private byte[] colour;
        private bool currentTurn;
        private List<ChessPiece> chessPieces = new List<ChessPiece>();
        private uint[,] spawnLocations; //start with the pawns, left to right and then the rest, left to right
        private sbyte directionMultiplier;

        public Player(byte[] colour, bool startTurn, uint[,] spawnLocations)
        {
            this.colour = colour;
            this.currentTurn = startTurn;
            this.spawnLocations = spawnLocations;
            directionMultiplier = startTurn ? (sbyte)1: (sbyte)-1; 
            CreatePieces();
        }

        public void Control()
        {
            HoverOver();
            MovePiece();
        }

        private void HoverOver()
        {
            //go through the list, e.g. if player press left arror it goes - 1 and if at 0 it goes to 7? Or should the code let the player move through the board and hightlight the square they are standing on and if there 
                //is a chess piece in their control its gets highlighted instead of and they can select it? 

            SelectPiece();
        }

        private void SelectPiece()
        {

        }

        private void MovePiece()
        {

        }

        private void CreatePieces()
        {
            string[] pawnDesign =
            {
                " - ",
                " | ",
                "-P-"
            };
            string[] rockDesign =
            {
                "^^^",
                "|=|",
                "-R-"
            };

            string[] knightDesign =
            {
                " ^_",
                " |>",
                "-k-"
            };

            string[] bishopDesign =
            {
                "_+_",
                "|O|",
                "-B-"
            };

            string[] queenDesign =
            {
                "_w_",
                "~|~",
                "-Q-"
            };

            string[] kingDesign =
            {
                "^V^",
                "*|*",
                "-K-"
            };

            uint[][] pawnMove =
            { //first move, it got the possiblity of moving 1 or 2

            };

            uint[][] rockMove =
            { //it can move 1 to 7 squares in each direciton. With current design decision it will contain 7*4 arrays, a little to much
                //consider another way to do these moves. Also since the queen have many more moves, 7*8 moves
                //maybe other than just the 1-4 values used, use 5 for unlimited move in diagonal directions and 6 for unlimited non-diagnonal directions, where the code should calculate the max amount of distance the 
                //piece can move in each direction, i.e. when the piece hits a wall or another piece. 
                    //This just leave how to select the different direction, but then again, it same should be done for the "normal" 1-4 values. 
                    //Of course, the maximum move distance and move selection should be done over in the specific chesspiece. 
                    //Git, are you working?
            };

            string team;
            team = currentTurn == true ? "-": "+";

            for (int i = 0; i < 8; i++)
            {//loop that creates each piece 
                string pawnID = String.Format("{0}6:{1}", team, i);
                //set other values for each piece and create them.
                //chessPieces.Add
            }

            for (int i = 0; i < 2; i++)
            { //loop that creates each piece 
                string rockID = String.Format("{0}5:{1}", team, i);
                string bishopID = String.Format("{0}3:{1}", team, i);
                string knightID = String.Format("{0}4:{1}", team, i);
                //set other values for each piece and create them.
                //chessPieces.Add
            }

            string queenID = String.Format("{0}2:{1}", team, 0);
            string kingID = String.Format("{0}1:{1}", team, 0);
        }

        public bool Turn(bool turn)
        {


            return currentTurn;
        }

    }



    class ChessPiece //after the UML this class should be abstract and the same for its methods
    {//when put on a location, check if there is an allie, if there is invalid move, if enemy, call that pieces removeDraw and call their Taken using TakeEnemyPiece
        private uint[] location; //x,y
        private byte[] colour;
        private string[] design;
        private byte[][] movePattern;
        private byte[][] attack; //in almost everycase it is the same as movePattern, so it can be set to null. If it is not null, i.e. pawn, it should be called when moving if there are enemies at the right location
        private bool? team;
        private uint[] spawnLocation;
        private string ID;
        private bool canDoMove;

        ChessPiece(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID)
        {

        }

        public void Control()
        {

        }

        private void Move()
        {

        }

        private void Draw()
        {

        }

        public void IsHoveredOn()
        {

        }

        private void RemoveDraw()
        {

        }

        public void Taken()
        {

        }

        private void SetTeam(bool? team_)
        {

        }

        private void DisplayPossibleMove()
        {

        }

        private void TakeEnemyPiece()
        {

        }

    }


}
