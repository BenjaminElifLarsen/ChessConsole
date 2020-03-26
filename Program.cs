using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Chess
{

    public class MapMatrix
    {
        private MapMatrix() { }
        public static sbyte[,] map = new sbyte[8, 8];
    }

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
            lineColour = new byte[] { 122, 122, 122 };
            lineColourBase = new byte[] { 87, 65, 47 };
            squareColour1 = new byte[] { 182, 123, 91 };
            squareColour2 = new byte[] { 135, 68, 31 };
            offset = new byte[] { 2, 2 };

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
            for (int k = 0; k < distance; k += 1 + squareSize)
                for (int i = 1; i < distance - 1; i++)
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
                            if (location % 2 == 0)
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
            white = new Player(colourWhite, true, whiteSpawnLocation);
            byte[] colourBlack =
            {
                0,0,0
            };
            black = new Player(colourBlack, false, blackSpawnLocation);
        }

    }

    class Player
    { //this class is set to be an abstract in the UML, but is that really needed? 
        private byte[] colour;
        private bool currentTurn; //rename to either black or white so it makes more sense 
        private List<ChessPiece> chessPieces = new List<ChessPiece>();
        private uint[,] spawnLocations; //start with the pawns, left to right and then the rest, left to right
        private sbyte directionMultiplier;

        public Player(byte[] colour, bool startTurn, uint[,] spawnLocations)
        {
            this.colour = colour;
            this.currentTurn = startTurn;
            this.spawnLocations = spawnLocations;
            directionMultiplier = startTurn ? (sbyte)1 : (sbyte)-1;
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
            { //first move, it got the possiblity of moving 1 or 2 //the pawn class should check if it, that is a specific pawn, have moved or not. If not it should give the option of moving 2 squares forward
                new uint[] {1 }
            };

            uint[][] rockMove =
            { //it can move 1 to 7 squares in each direciton. With current design decision it will contain 7*4 arrays, a little to much
                //consider another way to do these moves. Also since the queen have many more moves, 7*8 moves
                //maybe other than just the 1-4 values used, use 5 for unlimited move in diagonal directions and 6 for unlimited non-diagnonal directions, where the code should calculate the max amount of distance the 
                //piece can move in each direction, i.e. when the piece hits a wall or another piece. 
                    //This just leave how to select the different direction, but then again, it same should be done for the "normal" 1-4 values. 
                    //Of course, the maximum move distance and move selection should be done over in the specific chesspiece. 
                    //Git, are you working? Does not seem so, 3 hours spent and back to restore my work... yay... good job Git...
            };

            string team;
            team = currentTurn == true ? "-" : "+";

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

    sealed class King : ChessPiece
    { //this one really need to keep an eye on all other pieces and their location
        public King(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_, team_, spawnLocation_, ID) { }

        public bool IsInDanger()
        { //if true, it should force the player to move it. Also, it needs to check each time the other player has made a move 


            return false;
        }

    }

    sealed class Queen : ChessPiece
    {
        public Queen(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_, team_, spawnLocation_, ID) { }
    }

    sealed class Pawn : ChessPiece
    {
        private bool firstTurn = false;
        private bool canAttactLeft;
        private bool canAttackRight; 
        public Pawn(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_, team_, spawnLocation_, ID) { }
    }

    sealed class Rock : ChessPiece
    {
        public Rock(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_, team_, spawnLocation_, ID) { }
    }

    sealed class Bishop : ChessPiece
    {
        public Bishop(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_, team_, spawnLocation_, ID) { }
    }

    sealed class Knight : ChessPiece
    {

        //Knight(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : this(location_, colour_, design_, team_, spawnLocation_, ID) { }

        public Knight(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_,team_,spawnLocation_,ID)
        { //maybe do not have the moves and attacks, design and suck as parameters, but rather part of the code, since you have changed from abstract to non abstract class
            //redo the constructors when you are sure what you will need. So far: spawnlocation, id and team
        }




    }

    abstract public class ChessPiece //still got a lot to read and learn about what is the best choice for a base class, class is abstract, everything is abstract, nothing is abstract and so on. 
    {//when put on a location, check if there is an allie, if there is invalid move, if enemy, call that pieces removeDraw and call their Taken using TakeEnemyPiece
        protected uint[] location; //x,y
        protected byte[] colour;
        protected string[] design;
        protected byte[][] movePattern;
        protected byte[][] attack; //in almost everycase it is the same as movePattern, so it can be set to null. If it is not null, i.e. pawn, it should be called when moving if there are enemies at the right location
        protected bool team;
        protected uint[] spawnLocation;
        protected string id;
        protected bool canDoMove;
        protected bool hasBeenTaken = false;
        protected byte squareSize; 

        public ChessPiece(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID)
        { //for testing the code, just create a single player and a single piece. 
            Location = location;
            Colour = colour;
            Design = design_;
            SetTeam(team_);
            SpawnLocation = spawnLocation_;
            this.ID = ID; //String.Format("{0}5:{1}", team, i); team = currentTurn == true ? "-" : "+";
            Draw();
        }

        public bool BeenTaken { get => hasBeenTaken; } //use by other code to see if the piece have been "taken" and should be removed from game. 

        protected bool SetBeenTaken { set => hasBeenTaken = value; }

        protected uint[] Location { get => location; set => location = value; } //consider for each of the properties what kind they should have

        protected byte[] Colour { set => colour = value; }

        protected string[] Design { get => design; set => design = value; }
        protected byte[][] MovePattern { set => movePattern = value; }

        protected byte[][] AttackPattern { get => attack; set => attack = value; }

        protected bool Team { get => team; } //need to know it own team, but does it need to know the others's teams, the IDs can be used for that or just the matrix map. 

        protected uint[] SpawnLocation { set => spawnLocation = value; }

        protected string ID { get => id; set => id = value; } //maybe split into two. Get being protected and the set being public 

        protected bool CanDoMove { get => canDoMove; set => canDoMove = value; } //what was the canDoMove suppose to be for again?

        public void Control()
        {
            Move();
            RemoveDraw();
            Draw();
            UpdateMapMatrix();
        }

        protected void Move()
        { 
            //calculate each possible, legal, end location. Maybe have, in the class scope, a variable byte[,] legalEndLocations that the DisplayPossibleMove can use. 
            DisplayPossibleMove(); //actually all of this is needed be done in the derived classes, since movement (and attacks) depends on the type of piece. 
            //how to best do this and DisplayPossibleMove()... 
            //how to know which location they have selected out of all the possible location?
            //before fully starting to implement the move and display, focus on just moving a single piece around to ensure the (remove)draw function work and the matrix map is updated and all of that. 
            //Then set up two pieces, one of each player, and see if the map and such are working correctly and if they got the same location if the correct one is removed. 
        }

        protected void Draw()
        {
            byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
            int drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            int drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            for (int i = 0; i < design[0].Length; i++) //why does all the inputs, length, count and so on use signed variable types... 
            {
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m{0}",design[i]); //becareful, this one is not ending with a "\x1b[0m".
            }

        }

        public void IsHoveredOn()
        { //consider allowing a custom colour or just inverse colour. 
            byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
            int drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            int drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            for (int i = 0; i < design[0].Length; i++)  
            {
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("\x1b[48;2;" + 255 + ";" + 0 + ";" + 0 + "m{0}", design[i]); //this one is not ending with a "\x1b[0m".
            }
        }

        protected void RemoveDraw()
        {
            byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
            int drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            int drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            for (int i = 0; i < design[0].Length; i++) 
            {
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("".PadRight(design[0].Length,' ')); 
            }
        }


        protected void UpdateMapMatrix()
        { //need to either give the array[,] or have a class that it can acess it from. Since it is an array, an update in one instance will update the array in all instances. 

        }

        public void Taken()
        {//call by another piece, the one that takes this piece. 
            hasBeenTaken = true;
            RemoveDraw(); //if the piece is taken, the other piece stands on this ones location, so removeDraw might remove the other piece. Consider how to implement the Taken/Move regarding that. 
        }

        protected void SetTeam(bool team_)
        {
            team = team_;
        }

        protected void DisplayPossibleMove()
        {
            //needs to draw at every end location
        }

        protected void TakeEnemyPiece()
        {
            //how to find and get the enemy piece. The lists so far only exist in the players. Maybe have a class that only contains the two lists and when called a method to get a list, 
            //it use a conditional operator to return the list of the other team. 
            //the lists cannot be inheritance, since the piece will need to the other team's list and the player only contains their team's list.   
        }

    }


}
