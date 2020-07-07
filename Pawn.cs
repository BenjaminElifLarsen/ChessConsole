using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// The class for pawns.
    /// </summary>
    sealed class Pawn : ChessPiece
    {
        private bool firstTurn = true;
        private bool hasMoved = false;
        private sbyte moveDirection;
        private Dictionary<string, byte> promotions = new Dictionary<string, byte>(4);
        // https://docs.microsoft.com/en-us/dotnet/standard/generics/covariance-and-contravariance

        /// <summary>
        /// The constructor for the pawn chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public Pawn(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                " - ",
                " | ",
                "-P-"
            };
            moveDirection = team ? (sbyte)-1 : (sbyte)1;
            mostImportantDesignPart = new byte[] { 1, 2 };
            DesignResizer();
            Draw();
            promotions.Add("knight", 4);
            promotions.Add("rook", 5);
            promotions.Add("bishop", 3);
            promotions.Add("queen", 2);
            directions = new int[][] { new int[] { 0, moveDirection } };
            canPromoted = true;
        }

        /// <summary>
        /// Returns true if the pawn has moved at some point. False otherwise. 
        /// </summary>
        public bool HasMoved { get => hasMoved; }

        /// <summary>
        /// A modified version of the base Move function. Designed to check if the player uses a double move. 
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
                                    firstTurn = false; 
                                    couldMove = true;
                                    oldMapLocation = new int[2] { mapLocation[0], mapLocation[1] };
                                    mapLocation = new int[2] { cursorLocation[0], cursorLocation[1] };
                                    hasSelected = true;
                                    if (Math.Abs((sbyte)(oldMapLocation[1]) - (sbyte)(cursorLocation[1])) == 2)
                                    {
                                        SpecialBool = true;
                                    }
                                    else
                                    {
                                        SpecialBool = false;
                                        if (oldMapLocation[0] != cursorLocation[0])
                                        {
                                            if (MapMatrix.Map[cursorLocation[0], cursorLocation[1]] == "")
                                                CaptureEnemyPiece(new int[] { cursorLocation[0], cursorLocation[1] - moveDirection }); //minus since the direction the pawn is moving is the oppesite direction of the hostile pawn is at. 
                                            else
                                                CaptureEnemyPiece(cursorLocation);
                                        }
                                    }
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
                hasMoved = true;
            }
            else
            {
                possibleEndLocations = null;
                couldMove = false;
            }
        }

        /// <summary>
        /// Used by the online play and used to update a piece that has been changed by the other player that is not on this computer. 
        /// </summary>
        /// <param name="newLocation"></param>
        /// <param name="captured"></param>
        public override void NetworkUpdate(int[] newLocation = null, bool captured = false, bool repaintLocation = false)
        {
            if (captured)
            {
                Captured();
            }
            else if (newLocation != null)
            {
                if (Math.Abs(newLocation[1] - mapLocation[1]) == 2)
                    specialBool = true;
                RemoveDraw(mapLocation);
                mapLocation = newLocation;
                LocationUpdate();
                Draw();
            }
            else if (repaintLocation)
            {
                DisplayPossibleMove();
                SquareHighLight(true, cursorLocation);
            }
        }

        /// <summary>
        /// Overriden control function of the base class. Checks if the chess piece is ready for a promotion. 
        /// </summary>
        public override void Control()
        {
            isSelected = true;
            Move();
            RemoveDraw(oldMapLocation);
            LocationUpdate();
            Draw();
            UpdateMapMatrix(oldMapLocation);
            Promotion();
            isSelected = false;
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        {
            possibleEndLocations = new List<int[,]>();
            if ((!team && mapLocation[1] != 0) || (team && mapLocation[1] != 7))
                if (MapMatrix.Map[mapLocation[0], mapLocation[1] + moveDirection] == "")
                {
                    possibleEndLocations.Add(new int[,] { { mapLocation[0], (int)(mapLocation[1] + moveDirection) } });
                }
            if (firstTurn)
            {
                if (MapMatrix.Map[mapLocation[0], (mapLocation[1] + moveDirection * 2)] == "" && MapMatrix.Map[mapLocation[0], (mapLocation[1] + moveDirection)] == "")
                {
                    possibleEndLocations.Add(new int[,] { { mapLocation[0], (int)(mapLocation[1] + moveDirection * 2) } });
                }

            }
            CheckAttackPossbilities();

        }

        /// <summary>
        /// Checks if there possible hostile piece that can be taken. If there is, they locations are added to the possibleEndLocations.
        /// </summary>
        private void CheckAttackPossbilities()
        {
            if ((!team && mapLocation[1] != 0) || (team && mapLocation[1] != 7))
            {
                if (mapLocation[0] != 0) //check square to the left side
                    LocationCheck(-1);
                if (mapLocation[0] != 7) //check square to the right side
                    LocationCheck(1);

                EnPassant();
            }

            void LocationCheck(sbyte direction) //find a better name
            {
                string locationID = MapMatrix.Map[mapLocation[0] + direction, mapLocation[1] + moveDirection];
                if (locationID != "")
                {
                    string teamID = locationID.Split(':')[0];
                    if (teamID != teamIcon)
                        possibleEndLocations.Add(new int[,] { { (int)(mapLocation[0] + direction), (int)(mapLocation[1] + moveDirection) } });
                }
            }

            void EnPassant()
            {
                byte chessAmount = (byte)ChessList.GetList(!team).Count;
                for (byte i = 0; i < chessAmount; i++) //goes through all chess pieces
                {
                    string idCheck = ChessList.GetList(!team)[i].GetID;
                    if (idCheck.Split(':')[1] == "6") //checks if the current piece is a pawn
                    {
                        if (ChessList.GetList(!team)[i].SpecialBool) //checks if the pawn as double moved and en passant is allowed.
                        {
                            int[] hostileLocation = ChessList.GetList(!team)[i].GetMapLocation;
                            if (hostileLocation[1] == mapLocation[1]) //does the pawn and en-passant pawn share the same y. 
                                if (hostileLocation[0] == mapLocation[0] + 1 || hostileLocation[0] == mapLocation[0] - 1) //Checks if the pawn x is a location that allows it to be en passant.  
                                {
                                    possibleEndLocations.Add(new int[,] { { hostileLocation[0], hostileLocation[1] + moveDirection } });
                                }
                        }
                    }
                }

            }

        }



        /// <summary>
        /// Promotes the pawn. 
        /// </summary>
        private void Promotion()
        {
            if ((!team && mapLocation[1] == 7) || (team && mapLocation[1] == 0))
            {
                bool chosen = false;
                string command = "Write to Select: ";
                string answer = "";

                DisplayPromotions();
                Console.SetCursorPosition(Settings.PromotionWriteLocation[0], Settings.PromotionWriteLocation[1] + 2);
                Console.Write(command);
                do //not really happy with this, it does not fit the rest of the game. Consider other ways to do it.
                {
                    Console.SetCursorPosition(Settings.PromotionWriteLocation[0] + command.Length, Settings.PromotionWriteLocation[1] + 2);
                    Console.Write("".PadLeft(answer.Length));
                    Console.SetCursorPosition(Settings.PromotionWriteLocation[0] + command.Length, Settings.PromotionWriteLocation[1] + 2);
                    answer = Console.ReadLine();
                    foreach (string promotionKey in promotions.Keys)
                    {
                        if (promotionKey.ToLower() == answer.ToLower())
                        {
                            chosen = true;
                            break;
                        }
                    }
                } while (!chosen);

                answer = answer.ToLower();
                Console.SetCursorPosition(0, Settings.PromotionWriteLocation[1]); //removes the written promotion text
                Console.WriteLine("".PadLeft(Settings.WindowSize[0]));
                Console.WriteLine("".PadLeft(Settings.WindowSize[0]));
                Console.WriteLine("".PadLeft(Settings.WindowSize[0]));
                Captured();
                string[] IDParts = ID.Split(':');
                string newID;
                IDParts[1] = promotions[answer].ToString();
                switch (answer)
                {
                    case "knight":
                        //IDParts[1] = "4";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]); //The P is to indicate that the piece used to be a pawn.
                        ChessList.GetList(team).Add(new Knight(colour, team, mapLocation, newID, Publishers.PubKey, Publishers.PubCapture));
                        break;

                    case "bishop":
                        //IDParts[1] = "3";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]);
                        ChessList.GetList(team).Add(new Bishop(colour, team, mapLocation, newID, Publishers.PubKey, Publishers.PubCapture));
                        break;

                    case "rook":
                        //IDParts[1] = "5";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]);
                        ChessList.GetList(team).Add(new Rook(colour, team, mapLocation, newID, Publishers.PubKey, Publishers.PubCapture));
                        break;

                    case "queen":
                        //IDParts[1] = "2";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]);
                        ChessList.GetList(team).Add(new Queen(colour, team, mapLocation, newID, Publishers.PubKey, Publishers.PubCapture));
                        break;

                }
                ChessList.GetList(team)[ChessList.GetList(team).Count - 1].SpecialBool = true;
            }

        }

        /// <summary>
        /// Displays the possible promotions
        /// </summary>
        private void DisplayPromotions()
        { //writes to a location what chesspieces it can be promoted too.
            string promotionsString = $"Possible Promotions:{Environment.NewLine}{"".PadLeft(Settings.PromotionWriteLocation[0])}";
            foreach (string key in promotions.Keys)
            {
                char[] charArray = key.ToCharArray();
                charArray[0] = (char)((int)charArray[0] - 32);
                string keyUpper = new string(charArray);
                promotionsString += keyUpper + " ";
            }
            Console.SetCursorPosition(Settings.PromotionWriteLocation[0], Settings.PromotionWriteLocation[1]);
            Console.Write(promotionsString);
        }

    }
}
