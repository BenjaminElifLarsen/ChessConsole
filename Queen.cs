using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// The class for queens.
    /// </summary>
    sealed class Queen : ChessPiece
    {
        /// <summary>
        /// The constructor for the queen chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public Queen(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                "_w_",
                "~|~",
                "-Q-"
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

            position = new sbyte[2] { -1, -1 };
            CheckPosistions(position); //left, up

            position = new sbyte[2] { 1, -1 };
            CheckPosistions(position); //right, up

            position = new sbyte[2] { -1, 1 };
            CheckPosistions(position); //,left down

            position = new sbyte[2] { 1, 1 };
            CheckPosistions(position); //right, down

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
                possibleEndLocations.Add(new int[,] { { (int)(mapLocation[0] + posistions[0]), (int)(mapLocation[1] + posistions[1]) } });
            }
        }

    }
}
