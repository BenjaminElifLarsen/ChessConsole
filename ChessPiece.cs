using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Chess
{
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
        protected string teamIcon; 
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
            this.ID = ID;
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
        public bool BeenCaptured { get => hasBeenTaken; } //use by other code to see if the piece have been "captured" and should be removed from game. 

        /// <summary>
        /// Gets and sets the location of the chesspiece on the console.  
        /// </summary>
        protected int[] Location
        {
            set
            {
                location = value;
            }
            get
            {
                int[] loc = new int[2] { location[0], location[1] };
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
        protected string ID { get => id; set => id = value; } 

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

                    char[] newDesign = new char[Settings.SquareSize];
                    byte mostImportLocation = mostImportantDesignPart[0] < Settings.SquareSize ? mostImportantDesignPart[0] : (byte)(newDesign.Length - 1);
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
        /// Updates the piece, if it has been modified by the other player. If any combinations of the parameters are not null/false and <paramref name="captured"/> is true the piece will be set as captured only.
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
            else if (newLocation != null)
            {
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
        /// Allows the chesspiece to move. 
        /// </summary>
        protected virtual void Move()
        {
            oldMapLocation = null;
            bool hasSelected = false;
            if (possibleEndLocations == null)
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
                                if (endloc_[0] == mapLocation[0] && endloc_[1] == mapLocation[1])
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
                    Publishers.PubCapture.Capture(feltID);
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

            while ((int)key.Key == 0 && !GameStates.GameEnded) ;

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
                        if (currentLocation[0] == loc[0, 0] && currentLocation[1] == loc[0, 1])
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
                            if (squareID.Split(':')[0] != teamIcon)
                            {
                                foreach (ChessPiece chePie in ChessList.GetList(!team))
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
                            else
                            {
                                foreach (ChessPiece chePie in ChessList.GetList(team)) //consider using delegate/event for this. 
                                {
                                    if (chePie.GetID == squareID) //put the content of this if-statement into a function in ChessPiece. 
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
            drawLocationX = Location[0] + (int)(Settings.SquareSize - designSize) / 2; 
            drawLocationY = Location[1] + (int)(Settings.SquareSize - designSize) / 2; 
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
            for (int i = 0; i < design.Length; i++) 
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
            byte[] colours;
            if (!otherPlayer)
                colours = Settings.SelectPieceColour;
            else
                colours = new byte[] { (byte)(255 - Settings.SelectPieceColour[0]), (byte)(255 - Settings.SelectPieceColour[1]), (byte)(255 - Settings.SelectPieceColour[2]) };

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
        public void Captured()
        { 
            hasBeenTaken = true;
            MapMatrix.Map[mapLocation[0], mapLocation[1]] = "";
            RemoveDraw(mapLocation);
            RemoveSubscriptions();
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
                        else if (squareID.Split(':')[0] != teamIcon)
                        {
                            foreach (ChessPiece chePie in ChessList.GetList(!team))
                            {
                                if (chePie.GetID == squareID)
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
        /// If a piece shares the ID with the value in <paramref name="e"/> it will runs its capture code. 
        /// </summary>
        /// <param name="sender">The object that invoked the event.</param>
        /// <param name="e">The parameter containing the variables and their values of ControlEvents.CaptureEventArgs</param>
        protected void CaptureEventHandler(object sender, ControlEvents.CaptureEventArgs e)
        {
            if (e.ID == ID)
                Captured();
        }

        /// <summary>
        /// Removes all subscriptions.
        /// </summary>
        public void RemoveSubscriptions()
        {
            Publishers.PubCapture.RaiseCaptureEvent -= CaptureEventHandler;
            Publishers.PubKey.RaiseKeyEvent -= KeyEventHandler;
        }

    }
}
