using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// The class for rocks.
    /// </summary>
    sealed class Rook : ChessPiece
    {
        private bool hasMoved = false;

        /// <summary>
        /// The constructor for the rook chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public Rook(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                "^^^",
                "|=|",
                "-R-"
            };
            mostImportantDesignPart = new byte[] { 1, 2 };
            DesignResizer();
            Draw();
            directions = new int[][]
                        {
                            new int[]{-1,0},
                            new int[]{1,0},
                            new int[]{0,1},
                            new int[]{0,-1}
                        };
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        {
            possibleEndLocations = new List<int[,]>();
            sbyte[] position = new sbyte[2] { -1, 0 };
            CheckPosistions(position); //left

            position = new sbyte[2] { 1, 0 };
            CheckPosistions(position); //right

            position = new sbyte[2] { 0, -1 };
            CheckPosistions(position); //up

            position = new sbyte[2] { 0, 1 };
            CheckPosistions(position); //down

            if (possibleEndLocations.Count != 0)
            { //need to make sure that if a player selects the rook and it cannot move, it does not prevent castling from happening. 
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
                        if (teamIcon != feltID.Split(':')[0])
                        {
                            Add(loc);
                        }
                        currentCanMove = false;
                    }

                } while (currentCanMove);
            }

            void Add(sbyte[] posistions)
            {
                possibleEndLocations.Add(new int[,] { { (mapLocation[0] + posistions[0]), (mapLocation[1] + posistions[1]) } });
            }
        }

        /// <summary>
        /// Returns true if the chess piece has moved at any point, false otherwise. 
        /// </summary>
        public override bool SpecialBool
        {
            get
            {
                return HasMoved;
            }
            set
            {
                HasMoved = value;
            }
        }

        /// <summary>
        /// Returns true if the chess piece has moved at any point, false otherwise. 
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
        /// Calls the castling function. 
        /// </summary>
        /// <returns></returns>
        public override bool SpecialChessPieceFunction()
        { //used to set its position after castling. So RemoveDraw, update locations, Draw, set variable regarding if it has moved to true. 
          //called by the active piece, so its own and the one it is castling with. 
          //if it was moved, it return false, true if it can castle. 

            //if (shouldCastle && !HasMoved)
            //Castling();
            Castling();
            return false;
        }

        public override void NetworkUpdate(int[] newLocation = null, bool captured = false, bool repaintLocation = false)
        {
            if (captured)
                Captured();
            else if (newLocation != null)
            {
                hasMoved = true;
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
        /// Moves the chess piece after the rules of castling. 
        /// </summary>
        private void Castling()
        {
            RemoveDraw(mapLocation);
            int[] newLocation = new int[2];
            int[] oldLocation = mapLocation;
            newLocation[1] = mapLocation[1];
            newLocation[0] = mapLocation[0] == 0 ? 3 : 5;
            mapLocation = newLocation;
            UpdateMapMatrix(oldLocation);
            LocationUpdate();
            Draw();
            hasMoved = true;
        }

    }
}
