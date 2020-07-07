using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// The class for bishops. 
    /// </summary>
    sealed class Bishop : ChessPiece
    {
        /// <summary>
        /// The constructor for the bishop chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>

        public Bishop(byte[] colour_, bool team_, int[] spawnLocation_, string ID, KeyPublisher keyPub, CapturePublisher capPub) : base(colour_, team_, spawnLocation_, ID, keyPub, capPub)
        {
            Design = new string[]
            {
                $"_{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Plus_Minus}{Settings.CVTS.DEC.DEC_Deactive}_",
                $"{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Vertical_Line}{Settings.CVTS.DEC.DEC_Deactive}O{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Vertical_Line}{Settings.CVTS.DEC.DEC_Deactive}",
                $"-B-"
            };
            mostImportantDesignPart = new byte[] { 1, 2 };
            DesignResizer();
            //Design = new string[]
            //{ //changes the length of the first string so code needs to be changed to either use the amount of strings in the array or something else. Also the code that control the paint location
            //    $"_{Settings.CVTS.DEC.DEC_Active}{Settings.CVTS.DEC.DEC_Plus_Minus}{Settings.CVTS.DEC.DEC_Deactive}_",
            //    "|O|",
            //    "-B-"
            //};
            Draw();
            directions = new int[][]
                        {
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
            sbyte[] position = new sbyte[2] { -1, -1 };
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
                        //Solucation might not work as intended as it the current values cannot go negative and if posistion is 0 - 1 it will reach the max value. This will be caugt, but consider a different approach. 
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
                            break;
                        }
                        currentCanMove = false;
                    }

                } while (currentCanMove);
            }

            void Add(sbyte[] posistions)
            {
                possibleEndLocations.Add(new int[,] { { (int)(mapLocation[0] + posistions[0]), (mapLocation[1] + posistions[1]) } });
            }
        }
    }
}
