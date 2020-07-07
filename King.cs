using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Chess
{
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
                    string writeLout = String.Format("{0}{1}", letter, 8 - loc[1]);
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

            foreach (int[] pos in directions)
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
            else if (newLocation != null)
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
                        if (chepie.GetMapLocation[1] == mapLocation[1]) //ensures that no pawn promoted rook can be castled with.
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
}
