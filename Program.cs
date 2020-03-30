using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Chess
{   //https://www.chessvariants.com/d.chess/chess.html

    public class MapMatrix
    {
        private MapMatrix()
        {
            for (int n = 0; n < 8; n++)
                for (int m = 0; m < 8; m++)
                    map[n, m] = "";
        }
        public static string[,] map = new string[8, 8];
    }

    public class ChessList
    {
        private ChessList() { }
        private static List<ChessPiece> chessListBlack = new List<ChessPiece>();
        private static List<ChessPiece> chessListWhite = new List<ChessPiece>();
        public static void SetChessListBlack(List<ChessPiece> list)
        {
            chessListBlack = list;
        }
        public static void SetChessListWhite(List<ChessPiece> list)
        {
            chessListWhite = list;
        }
        public static List<ChessPiece> GetList(bool team)
        {
            return team == true ? chessListBlack : chessListWhite;
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
        private static byte[] offset = new byte[] { 2, 2 };
        public static byte SquareSize { get => squareSize; }
        public static byte[] LineColour { get => lineColour; }
        public static byte[] LineColourBase { get => lineColourBase; }
        public static byte[] SquareColour1 { get => squareColour1; }
        public static byte[] SquareColour2 { get => squareColour2; }
        public static byte[] Offset { get => offset; } //remember the '|' and '-'
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

        public Player(byte[] colour, bool startTurn, uint[,] spawnLocations)
        {
            this.colour = colour;
            this.currentTurn = startTurn;
            this.spawnLocations = spawnLocations;
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
        public King(byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, design_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "^V^",
                "*|*",
                "-K-"
            };
        } //king cannot move next to another king

        public bool IsInDanger()
        { //if true, it should force the player to move it. Also, it needs to check each time the other player has made a move 
            //should also check if it even can move, if it cannot the game should end. //find the other player's chesspieces on the map matrix, look at the IDs and see if there is a clear legal move that touces the king.
            //hmm... could also look check specific squares for specific chesspieces, e.g. check all left, right, up and down squares for rocks and queen, check specific squares that 3 squares away for knights and so on. 
            //the king can, however, take a chesspiece as long time that piece is not protected by another nor is the other king. Cannot move next to the hostile king 
            return false;
        }

        private bool HasMoved { get; } //if moved, castling cannot be done

        private bool Castling()
        { //king moves two squares in the direction of the chosen rock, the rock moves to the other side of the king. Neither should have moved in the game and the space between them needs to be empty. Also, no of the squares should be threanten by
            //hostile piece??? 


            return false;
        }

    }

    sealed class Queen : ChessPiece
    {
        public Queen(byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, design_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "_w_",
                "~|~",
                "-Q-"
            };
        }

    }

    sealed class Pawn : ChessPiece
    {
        private bool firstTurn = false;
        private bool canAttactLeft;
        private bool canAttackRight; 
        public Pawn(byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, design_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                " - ",
                " | ",
                "-P-"
            };
        }

        private void CheckAttackPossbilities()
        {
            //should check if it can take a piece and if it can, ensure that it is given to the displayPossibleMoves. 
            //read up on the "en-passant" rule regarding taking another pawn that has double moved.
        }

        private bool HasDoubleSquareMoved { get; set; }

        private void Promotion()
        { //pawn can become any other type of chesspiece. It should remove itself and create a new instance of the chosen piece on the same location.

        }

        private void DisplayPromotions()
        { //writes to a location what chesspieces it can be promoted too.

        }

    }

    sealed class Rock : ChessPiece
    {
        public Rock(byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, design_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "^^^",
                "|=|",
                "-R-"
            };
        }

        private bool HasMoved { get; } //if moved, castling cannot be done

        private bool Castling()
        {


            return false;
        }

    }

    sealed class Bishop : ChessPiece
    {
        public Bishop(byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, design_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "_+_",
                "|O|",
                "-B-"
            };
        }

    }

    sealed class Knight : ChessPiece
    {

        //Knight(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : this(location_, colour_, design_, team_, spawnLocation_, ID) { }

        public Knight(byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(colour_, design_,team_,spawnLocation_,ID)
        { //maybe do not have the moves and attacks, design and suck as parameters, but rather part of the code, since you have changed from abstract to non abstract class
          //redo the constructors when you are sure what you will need. So far: spawnlocation, id and team
            Design = new string[]
            {
                " ^_",
                " |>",
                "-k-"
            };
            MovePattern = new byte[][] { new byte[] {6} }; //maybe drop the idea of a movePattern variable and just write it into the specialised move code 


        }




    }

    abstract public class ChessPiece //still got a lot to read and learn about what is the best choice for a base class, class is abstract, everything is abstract, nothing is abstract and so on. 
    {//when put on a location, check if there is an allie, if there is invalid move, if enemy, call that pieces removeDraw and call their Taken using TakeEnemyPiece
        protected uint[] location; //x,y
        protected byte[] colour; // https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/inheritance 
        protected string[] design;
        protected byte[][] movePattern;
        protected byte[][] attack; //in almost everycase it is the same as movePattern, so it can be set to null. If it is not null, i.e. pawn, it should be called when moving if there are enemies at the right location
        protected bool team;
        protected uint[] mapLocation;
        protected string id;
        protected bool canDoMove;
        protected bool hasBeenTaken = false;
        protected byte squareSize; 

        public ChessPiece(byte[] colour_, string[] design_, bool team_, uint[] mapLocation_, string ID)
        { //for testing the code, just create a single player and a single piece. 
            //location should be the console x,y values, but instead of being given, it should be calculated out from the maplocation and square size
            Colour = colour;
            Design = design_;
            SetTeam(team_);
            MapLocation = mapLocation_; //what should this actually be done, is it the actually values on the console or is it values that fits the map matrix and location is then the actually console location...
            this.ID = ID; //String.Format("{0}n:{1}", team, i); team = currentTurn == true ? "-" : "+"; n being the chesspiece type
            LocationUpdate();
            Draw();
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
        protected uint[] Location { get => location; set => location = value; } //consider for each of the properties what kind they should have

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

        /// <summary>
        /// Gets and set the ID of the chesspiece. //maybe have some code that ensures the ID is unique 
        /// </summary>
        protected string ID { get => id; set => id = value; } //maybe split into two. Get being protected and the set being public 

        protected bool CanDoMove { get => canDoMove; set => canDoMove = value; } //what was the canDoMove suppose to be for again?

        /// <summary>
        /// Function that "controls" a piece. What to explain and how to..
        /// </summary>
        public virtual void Control()
        {
            Move();
            RemoveDraw(); 
            LocationUpdate();
            Draw();
            //UpdateMapMatrix();
        }

        /// <summary>
        /// Calculates the, legal, location(s) the chesspiece is able to move too. 
        /// </summary>
        protected virtual void Move()
        { 
            //calculate each possible, legal, end location. Maybe have, in the class scope, a variable byte[,] legalEndLocations that the DisplayPossibleMove can use. 
            DisplayPossibleMove(); //actually all of this is needed be done in the derived classes, since movement (and attacks) depends on the type of piece. 
            //how to best do this and DisplayPossibleMove()... 
            //how to know which location they have selected out of all the possible location?
            //before fully starting to implement the move and display, focus on just moving a single piece around to ensure the (remove)draw function work and the matrix map is updated and all of that. 
            //Then set up two pieces, one of each player, and see if the map and such are working correctly and if they got the same location if the correct one is removed. 
            //Should this function also figure out which location the player chose, set the new location and all that or should another function do that? 
        }

        /// <summary>
        /// updates the location that is used for displaying the chesspiece on the chessboard
        /// </summary>
        protected void LocationUpdate() //where should this one being called from
        { 
            Location[0] = mapLocation[0] * squareSize + (mapLocation[0]+1) * 1 + Settings.Offset[0]; //(matpLocation[0]+1) is for the amount of spaces between the offsets and first square and the space between all the squares
            //7*5+8+2 = 35+10 = 45, x
            //1*5+2+2 = 5+4 = 9, y 
            /*{1,2}
             * 1*5+2+2 = 9
             * 2*5+3+2 = 15
             * {0,0}
             * 0*5+1+2 = 3
             * 0*5+1+2 = 3
             */
            Location[1] = mapLocation[0] * squareSize + (mapLocation[0] + 1) * 1 + Settings.Offset[0];
        }

        /// <summary>
        /// Draws the chesspiece at its specific location
        /// </summary>
        protected void Draw()
        {
            byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
            int drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            int drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            for (int i = 0; i < design[0].Length; i++) //why does all the inputs, length, count and so on use signed variable types... 
            {
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m{0}",design[i]); //be careful, this one is not ending with a "\x1b[0m".
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
                for (int i = 0; i < design[0].Length; i++)
                {
                    Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                    Console.Write("\x1b[48;2;" + 255 + ";" + 0 + ";" + 0 + "m{0}", design[i]); //this one is not ending with a "\x1b[0m".
                }
            }
            else
                Draw();
        }

        /// <summary>
        /// removes the visual identication of a chesspiece at its current location.
        /// </summary>
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

        /// <summary>
        /// updates the map matrix with the new location of the chess piece and sets the old location to zero. 
        /// </summary>
        protected void UpdateMapMatrix(uint[] oldMapLocation) //need to call this before the LocationUpdate
        { 
            MapMatrix.map[mapLocation[0], mapLocation[1]] = ID;
            MapMatrix.map[oldMapLocation[0], oldMapLocation[1]] = "";
        }


        /// <summary>
        /// Set a chesspeice set to be taken so it can be removed. 
        /// </summary>
        public void Taken()
        {//call by another piece, the one that takes this piece. 
            hasBeenTaken = true;
            //it should remove itself from the map matrix. 
            RemoveDraw(); //if the piece is taken, the other piece stands on this ones location, so removeDraw might remove the other piece. Consider how to implement the Taken/Move regarding that. 
        }

        /// <summary>
        /// Sets the team of the chesspiece.
        /// </summary>
        /// <param name="team_">True for .... False for ....</param>
        protected void SetTeam(bool team_)
        {
            team = team_;
        }

        /// <summary>
        /// Display the possible, legal, moves a chesspiece can take. 
        /// </summary>
        protected void DisplayPossibleMove()
        {
            //needs to draw at every end location
            //what should be drawn, where should it and how to restore back to the default design and colour
        }

        protected void TakeEnemyPiece() 
        {
            //consider this aproach: Player select a location. This chesspiece then checks the location for an ID string or "". If "" call the removeDraw, move the piece and call Draw.
                //If there is an ID string, find that chesspiece and call its Taken. Then call removeDraw, move the piece and the call Draw. 
            string newLocationCurrentValue = MapMatrix.map[mapLocation[0], mapLocation[1]]; //should the map have been updated already or should this line of code some new location
            if(newLocationCurrentValue != "")
            {
                foreach (ChessPiece chesspiece in ChessList.GetList(Team)) //remember to ensure it gets the other teams list... so at some point figure out if false is white or black...
                {
                    if (chesspiece.ID == newLocationCurrentValue)
                    {
                        chesspiece.Taken(); //huh, got access to all functions and class scope values... guess that makes sense... Figure out some way to prevent that, so read
                        break;
                    }
                }
            }
            RemoveDraw();
            //UpdateMapMatrix();
            //call move function
            Draw();


        }

    }


}
