using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// The class for knights.
    /// </summary>
    sealed class Knight : ChessPiece
    {
        /// <summary>
        /// The constructor for the knight chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public Knight(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                " ^_",
                " |>",
                "-k-"
            };
            mostImportantDesignPart = new byte[] { 1, 2 };
            DesignResizer();
            Draw();
            directions = new int[][]
                {
                        new int[] { 1, 2 },
                        new int[] { 2, 1 },
                        new int[] { -1, -2 },
                        new int[] { -2, -1 },
                        new int[] { 1, -2 },
                        new int[] { 2, -1 },
                        new int[] { -1, 2 },
                        new int[] { -2, 1 }
                };
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        { //there must be a better way to do this...
            possibleEndLocations = new List<int[,]>();
            sbyte[] potenieltLocation = { -2, -1 }; //2 down left
            if (mapLocation[0] + potenieltLocation[0] >= 0 && mapLocation[1] + potenieltLocation[1] >= 0)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { 2, -1 }; //2 up left
            if (mapLocation[0] + potenieltLocation[0] <= 7 && mapLocation[1] + potenieltLocation[1] >= 0)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { 2, 1 }; //2 up right
            if (mapLocation[0] + potenieltLocation[0] <= 7 && mapLocation[1] + potenieltLocation[1] <= 7)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { -2, 1 }; //2 down right
            if (mapLocation[0] + potenieltLocation[0] >= 0 && mapLocation[1] + potenieltLocation[1] <= 7)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { -1, -2 }; //down 2 left
            if (mapLocation[0] + potenieltLocation[0] >= 0 && mapLocation[1] + potenieltLocation[1] >= 0)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { 1, -2 }; //up 2 left
            if (mapLocation[0] + potenieltLocation[0] <= 7 && mapLocation[1] + potenieltLocation[1] >= 0)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { 1, 2 }; //up 2 right
            if (mapLocation[0] + potenieltLocation[0] <= 7 && mapLocation[1] + potenieltLocation[1] <= 7)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }
            potenieltLocation = new sbyte[2] { -1, 2 }; //down 2 right
            if (mapLocation[0] + potenieltLocation[0] >= 0 && mapLocation[1] + potenieltLocation[1] <= 7)
            {
                if (CheckPosistions(potenieltLocation))
                    Add(potenieltLocation);
            }

            bool CheckPosistions(sbyte[] posistions)
            { //returns true if the piece can legal move there, false otherwise. 
                string feltID = MapMatrix.Map[mapLocation[0] + posistions[0], mapLocation[1] + posistions[1]];
                if (feltID != "")
                {
                    if (feltID.Split(':')[0] != teamIcon)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            void Add(sbyte[] posistions)
            {
                possibleEndLocations.Add(new int[,] { { (mapLocation[0] + posistions[0]), (mapLocation[1] + posistions[1]) }, });
            }
        }

    }
}
