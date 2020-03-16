using System;
using System.Collections.Generic;

namespace Chess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }

    class ChessTable
    {
        private Player white; //white top
        private Player black; //black bottom
        private uint[,] whiteSpawnLocation;
        private uint[,] blackSpawnLocation;
        ChessTable()
        {

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
