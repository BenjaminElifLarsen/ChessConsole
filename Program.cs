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
            squareColour1 = new byte[] {255,255,0 };
            squareColour2 = new byte[] {0,255,255 };
            offset = new byte[] {2,2 };

            windowsSize[0] = (byte)(9 + 8 * squareSize + 10);
            windowsSize[1] = (byte)(9 + 8 * squareSize + 10);
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);
            BoardSetup();
        }

        private void BoardSetup()
        {//8 squares in each direction.
            ushort distanceX = (ushort)(9 + 8 * squareSize);
            for (int k = 0; k < distanceX; k++)
                for (int i = 0; i < distanceX; i += 1 + squareSize)
                {
                    Console.SetCursorPosition(i + offset[0], k + offset[1]);
                    Console.Write("\x1b[38;2;" + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m|" + "\x1b[0m");
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
    {
        private byte[] colour;
        private bool currentTurn;
        private List<ChessPiece> chessPieces = new List<ChessPiece>();
        private uint[,] spawnLocations;
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

        }

        private void HoverOver()
        {

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



        }

        public bool Turn(bool turn)
        {


            return currentTurn;
        }

    }

    class ChessPiece
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
