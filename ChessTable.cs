using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Chess
{
    /// <summary>
    /// Class for creating and displaying the chess table and creating the chess pieces.
    /// </summary>
    class ChessTable
    {
        private Player white;
        private Player black;
        //private int[,] whiteSpawnLocation;
        //private int[,] blackSpawnLocation;
        private int[] windowsSize = new int[2];

        public ChessTable()
        {
            MapMatrix.PrepareMap();
            Settings.CanHighLight = Settings.SquareSize > Settings.ChesspieceDesignSize + 1 ? true : false;
            windowsSize[0] = Settings.WindowSize[0] > Console.LargestWindowWidth ? Console.LargestWindowWidth : Settings.WindowSize[0];
            windowsSize[1] = Settings.WindowSize[1] > Console.LargestWindowHeight ? Console.LargestWindowHeight : Settings.WindowSize[1];
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);

            int[,] blackSpawnLocation = new int[,] {
                { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 },
                { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }
            };
            int[,] whiteSpawnLocation = new int[,] {
                { 0, 6 }, { 1, 6 }, { 2, 6 }, { 3, 6 }, { 4, 6 }, { 5, 6 }, { 6, 6 }, { 7, 6 },
                { 0, 7 }, { 1, 7 }, { 2, 7 }, { 3, 7 }, { 4, 7 }, { 5, 7 }, { 6, 7 }, { 7, 7 }
            };
            BoardSetup();
            CreatePieces(true, whiteSpawnLocation, Settings.WhiteColour);
            CreatePieces(false, blackSpawnLocation, Settings.BlackColour);
            PlayerSetup();
        }

        /// <summary>
        /// Repaints the board and all of the chess pieces.
        /// </summary>
        public static void RepaintBoardAndPieces()
        {
            bool kingcheck;
            BoardSetup();
            foreach (ChessPiece chePie in ChessList.GetList(true))
            {
                chePie.IsHoveredOn(false); //this function is not meant for this, but it works regarding replainting the pieces. 
                if (chePie is King king)
                    kingcheck = king.SpecialBool; //called to get the king, if threaten, to write out the location it is treaten from
            }
            foreach (ChessPiece chePie in ChessList.GetList(!true))
            {
                chePie.IsHoveredOn(false);
                if (chePie is King king)
                    kingcheck = king.SpecialBool;
            }
        }

        /// <summary>
        /// Sets up the board and paint the outlines. 
        /// </summary>
        private static void BoardSetup()
        {//8 squares in each direction. Each piece is 3*3 currently, each square is 5*5 currently. 
            Console.CursorVisible = false;
            ushort distance = (ushort)(9 + 8 * Settings.SquareSize);
            string[] letters = { "a", "b", "c", "d", "e", "f", "g", "h" };
            string[] numbers = { "1", "2", "3", "4", "5", "6", "7", "8" };
            byte alignment = (byte)Math.Ceiling(Settings.SquareSize / 2f);

            //top left corner
            Console.SetCursorPosition(Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Corner_TopLeft + Settings.CVTS.DEC.DEC_Deactive}");

            //top right corner 
            Console.SetCursorPosition(distance - 1 + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Corner_TopRight + Settings.CVTS.DEC.DEC_Deactive}");

            //bottom left corner
            Console.SetCursorPosition(Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, distance - 1 + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Corner_BottomLeft + Settings.CVTS.DEC.DEC_Deactive}");

            //bottom right corner 
            Console.SetCursorPosition(distance - 1 + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, distance - 1 + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
            Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m"
                + Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset,
                $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Corner_BottomRight + Settings.CVTS.DEC.DEC_Deactive}");


            for (int k = 1; k < distance - 1; k++) //vertical lines
                for (int i = 0; i < distance; i += 1 + Settings.SquareSize)
                {

                    float intersectX = (float)Math.Floor(i / (double)(Settings.SquareSize + Settings.Spacing));
                    float intersectY = k % (float)(Settings.SquareSize + Settings.Spacing);
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, k + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m "); //background colour
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, k + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
                    if (intersectY != 0) //no intersection at all
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Vertical_Line + Settings.CVTS.DEC.DEC_Deactive}");
                    else if (intersectY == 0 && intersectX == 0) //intersection on the left side
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Intersection_Left + Settings.CVTS.DEC.DEC_Deactive}");
                    else //intersection at the right side. 
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Intersection_Right + Settings.CVTS.DEC.DEC_Deactive}");

                }
            for (int k = 0; k < distance; k += 1 + Settings.SquareSize) //horizontal lines
                for (int i = 1; i < distance - 1; i++)
                {
                    float intersectX = i % (float)(Settings.SquareSize + Settings.Spacing);
                    float intersectY = (float)Math.Floor(k / (double)(Settings.SquareSize + Settings.Spacing));
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, k + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
                    Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.LineColourBase[0] + ";" + Settings.LineColourBase[1] + ";" + Settings.LineColourBase[2] + "m "); //background colour
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing - 1, k + Settings.Offset[1] + Settings.EdgeSpacing + Settings.Spacing - 1);
                    if (intersectX != 0) //no intersection at all
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Horizontal_Line + Settings.CVTS.DEC.DEC_Deactive}");
                    else if (intersectX == 0 && intersectY == 0) //intersection at the top
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Intersection_Top + Settings.CVTS.DEC.DEC_Deactive}");
                    else if (intersectX == 0 && intersectY == 8) //intersection at the bottom
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Intersection_Bottom + Settings.CVTS.DEC.DEC_Deactive}");
                    else //intersection all other places.
                        Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + Settings.LineColour[0] + ";" + Settings.LineColour[1] + ";" + Settings.LineColour[2] + "m{0}" + Settings.CVTS.Reset, $"{Settings.CVTS.DEC.DEC_Active + Settings.CVTS.DEC.DEC_Intersection_Full + Settings.CVTS.DEC.DEC_Deactive}");


                }

            for (int k = 0; k < numbers.Length; k++) //numbers
            { //the 1s in this and below for loop should be a setting, it is the amount of empty squares between the numbers/letters and the board edge
                Console.SetCursorPosition(Settings.Offset[0] - Settings.EdgeSpacing - 1 + Settings.Spacing, k + Settings.EdgeSpacing + Settings.Spacing + Settings.Offset[1] + alignment + (Settings.SquareSize * k) - 1);
                Console.Write(numbers[7 - k]); //- Settings.EdgeSpacing should not be used in the calculation above.
                Console.SetCursorPosition(Settings.Offset[0] + distance + Settings.EdgeSpacing + Settings.Spacing, k + Settings.Spacing + Settings.EdgeSpacing + Settings.Offset[1] + alignment + (Settings.SquareSize * k) - 1);
                Console.Write(numbers[7 - k]);
            }

            for (int k = 0; k < letters.Length; k++) //letters
            {
                Console.SetCursorPosition(k + Settings.Spacing + Settings.Offset[0] + alignment + (Settings.SquareSize * k), Settings.Offset[1] + Settings.Spacing - Settings.EdgeSpacing - 1);
                Console.Write(letters[k]);
                Console.SetCursorPosition(k + Settings.Spacing + Settings.Offset[0] + alignment + (Settings.SquareSize * k), Settings.Offset[1] + distance + Settings.Spacing + Settings.EdgeSpacing);
                Console.Write(letters[k]);
            }

            BoardColouring();
            KingWriteLocationSetup();
        }

        /// <summary>
        /// Colour the board. 
        /// </summary>
        private static void BoardColouring()
        {
            ushort distance = (ushort)(8 + 8 * Settings.SquareSize);
            byte location = 1;
            for (int n = 0; n < distance; n += 1 + Settings.SquareSize)
            {
                for (int m = 0; m < distance; m += 1 + Settings.SquareSize)
                {
                    for (int i = 0; i < Settings.SquareSize; i++)
                    {
                        for (int k = 0; k < Settings.SquareSize; k++)
                        {
                            Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing + n, k + Settings.Offset[1] + Settings.Spacing + Settings.EdgeSpacing + m);
                            if (location % 2 == 1)
                                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.SquareColour1[0] + ";" + Settings.SquareColour1[1] + ";" + Settings.SquareColour1[2] + "m " + Settings.CVTS.Reset);
                            else if (location % 2 == 0)
                                Console.Write(Settings.CVTS.ExtendedBackgroundColour_RGB + Settings.SquareColour2[0] + ";" + Settings.SquareColour2[1] + ";" + Settings.SquareColour2[2] + "m " + Settings.CVTS.Reset);
                        }
                    }
                    location++;
                }
                location++;
            }
        }

        /// <summary>
        /// Setup the location the two kings are going to write to, if they are under threat. 
        /// </summary>
        private static void KingWriteLocationSetup()
        {
            int[] white = new int[] { Settings.CheckHeaderLocation[0, 0], Settings.CheckHeaderLocation[0, 1] };
            int[] black = new int[] { Settings.CheckHeaderLocation[1, 0], Settings.CheckHeaderLocation[1, 1] };
            Write("White", white);
            Write("Black", black);

            void Write(string colour, int[] location)
            {
                Console.SetCursorPosition(location[0], location[1]);
                Console.Write(colour);
                Console.SetCursorPosition(location[0], location[1] + 1);
                Console.Write("King");
            }
        }


        /// <summary>
        /// Function that checks if the game has reached a draw. It wil lalso update the draw turn counter.
        /// </summary>
        /// <returns>Returns true if the game is in a draw, else false.</returns>
        private bool Draw(bool newMove = false, bool updatePieces = false)
        {

            bool noCapture;
            bool pawnChange = false;
            if (ChessList.GetList(true).Count == 1 && ChessList.GetList(false).Count == 1)
                return true;

            if (newMove)
                if (MapMatrix.LastMoveMap[0, 0] != null)
                    for (int n = 0; n < 8; n++)
                    {
                        for (int m = 0; m < 8; m++)
                        { //check if any pawns has been changed. 
                            string newFelt = MapMatrix.Map[m, n];
                            string oldFelt = MapMatrix.LastMoveMap[m, n];
                            if (newFelt != oldFelt)
                                if (oldFelt != "")
                                    if (oldFelt.Split(':')[1] == "6")
                                        pawnChange = true;
                        }
                    }

            if (updatePieces)
            {
                if (GameStates.PieceAmount[0, 0] == GameStates.PieceAmount[0, 1] && GameStates.PieceAmount[1, 0] == GameStates.PieceAmount[1, 1])
                    noCapture = true;
                else
                    noCapture = false;

                if (noCapture && !pawnChange)
                    GameStates.TurnDrawCounter++;
                else
                    GameStates.TurnDrawCounter = 0;

                if (GameStates.TurnDrawCounter == 70)
                    return true;
            }
            return false;
        }

        private void EndScreen()
        {
            Console.Title = Settings.GetTitle + ": Game Ended";
            string endMessage = null;
            if (!GameStates.LostConnection)
            {
                if (GameStates.Won == null)
                    endMessage = "The Game Ended in a Draw";
                else
                {
                    string firstPart = GameStates.WhiteWin ? "White" : "Black";
                    endMessage = $"{firstPart} Player Won";
                }
            }
            else
            {
                endMessage = "The Connection was lost";
            }
            Console.CursorTop = Settings.MenuOffset[1];
            Console.WriteLine($"{"".PadLeft(Settings.MenuTitleOffset[0])}{Settings.CVTS.BrightWhiteForeground}{Settings.CVTS.Underscore}Game Finished{Settings.CVTS.Underscore_Off}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}{endMessage} {Settings.CVTS.Reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}Amount of Moves: {Settings.CVTS.BrightCyanForeground}{Settings.CVTS.Underscore}{GameStates.TurnCounter}{Settings.CVTS.Reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightRedForeground}Enter{Settings.CVTS.BrightWhiteForeground} to continue. {Settings.CVTS.Reset}");
            while (Console.ReadKey(true).Key != Settings.SelectKey) ;
            Console.Clear();
        }

        /// <summary>
        /// Sets the title of the program while a game is going on.
        /// </summary>
        static public void GameRunTitle()
        {
            Console.Title = Settings.GetTitle + ": Game";
        }

        /// <summary>
        /// Runs the online game loop. <paramref name="starter"/> decides if the player makes the first move or not.
        /// </summary>
        /// <param name="starter">True for first move, false for second move.</param>
        private void GameLoopNet(bool starter)
        {
            GameRunTitle();
            bool whiteTeam = starter;
            bool firstTurn = starter;

            Thread receiveThread = new Thread(Network.Receive.ReceiveGameLoop);
            receiveThread.Name = "Receiver Thread";
            receiveThread.Start(starter);
            Thread connectionThread = new Thread(Network.Transmit.StillConnected);
            connectionThread.Name = "Connection Checker Thread";
            connectionThread.Start(Network.Transmit.OtherPlayerIpAddress);

            GameStates.IsTurn = starter;
            GameStates.LostConnection = false; //added for testing. 
            GameStates.IsOnline = true;
            GameStates.PieceAmount[0, 0] = (byte)ChessList.GetList(true).Count;
            GameStates.PieceAmount[0, 1] = (byte)ChessList.GetList(true).Count;
            GameStates.PieceAmount[1, 0] = (byte)ChessList.GetList(false).Count;
            GameStates.PieceAmount[1, 1] = (byte)ChessList.GetList(false).Count;

            if (MapMatrix.LastMoveMap[0, 0] == null)
                MapMatrix.UpdateOldMap();
            //game loop
            do
            {
                //byte counter = 0;
                //bool connectionExist;
                if (GameStates.IsTurn) //only true when Network.Receive.ReceiveMapData has received map data from the other player' transmitter
                {
                    GameStates.GameEnded = PlayerControlNet(starter);
                    GameStates.IsTurn = false;
                    if (GameStates.GameEnded && GameStates.LostConnection == false)
                    { //transmit a signal to the other player' receiver to let them know the game has ended. 
                        if (GameStates.Won == null && !GameStates.OtherPlayerSurrendered)
                            Network.Transmit.GeneralValueTransmission(6, Network.Transmit.OtherPlayerIpAddress); //Draw  //5 is loss for the other player, 4 is victory for the other player. 
                        else if (GameStates.Won == true && !GameStates.OtherPlayerSurrendered)
                            Network.Transmit.GeneralValueTransmission(5, Network.Transmit.OtherPlayerIpAddress); //other player lost
                        else if (GameStates.Won == false && !GameStates.OtherPlayerSurrendered)
                            Network.Transmit.GeneralValueTransmission(4, Network.Transmit.OtherPlayerIpAddress); //other player won
                        //Network.Receive.Stop();
                    }
                }
                else //not this computer's turn to move. 
                {
                    Player waitingPlayer = starter ? white : black;
                    waitingPlayer.ControlOnlyMenu();
                }

            } while (!GameStates.GameEnded);
            ChessList.RemoveAllPieces();
            Network.Receive.Stop();
            Console.Clear();
            EndScreen();
            GameStates.Reset();
            MapMatrix.AllowForMapPreparation();

            bool PlayerControlNet(bool team)
            {
                bool? checkmate = false; bool draw = false; bool? otherPlayerCheckMate = false;

                GameStates.TurnCounter++;

                for (int i = ChessList.GetList(team).Count - 1; i >= 0; i--) //removes it own, captured, pieces. Needs to be called before player.Control else the default hover overed piece might be one of the captured. 
                {
                    if (ChessList.GetList(team)[i].BeenCaptured)
                        ChessList.GetList(team).RemoveAt(i);
                }

                Player player;
                if (team)
                    player = white;
                else
                    player = black;

                ProtectKing.ProtectEndLocations.Clear();
                ProtectKing.ProtectingTheKing.Clear();
                ProtectKing.CannotMove = ProtectingTheKing(team);

                if (team)
                    PieceAmountUpdate(); //draw counters need to be updated before white player turn.

                if (!firstTurn)
                    Draw(true, updatePieces: team); //a "move", after the rules, consist of both players turn. This call ensures the draw counter is updated before white player's turn
                if (team)
                    MapMatrix.UpdateOldMap();
                firstTurn = false;

                otherPlayerCheckMate = IsKingChecked(!team); //updates whether the other player' king is under threat.
                checkmate = CheckmateChecker(team, out List<string> saveKingList);

                ProtectKing.Protect = saveKingList;

                if (checkmate != null)
                    if (!(bool)checkmate) //if the king is not checkmate, play
                        player.Control();

                otherPlayerCheckMate = CheckmateChecker(!team); //these two updates the write locations
                checkmate = IsKingChecked(team); //these two updates the write locations

                for (int i = ChessList.GetList(team).Count - 1; i >= 0; i--) //ensures any promoted pawns are removed from the list.
                {
                    if (ChessList.GetList(team)[i].BeenCaptured)
                    {
                        ChessList.GetList(team).RemoveAt(i);
                        break;
                    }
                }
                for (int i = ChessList.GetList(!team).Count - 1; i >= 0; i--) //removes captured pieces.
                {
                    if (ChessList.GetList(!team)[i].BeenCaptured)
                    {
                        ChessList.GetList(!team).RemoveAt(i);
                        break;
                    }
                }

                for (int i = ChessList.GetList(!team).Count - 1; i >= 0; i--)
                {
                    if (ChessList.GetList(!team)[i] is Pawn && ChessList.GetList(!team)[i].SpecialBool == true)
                    { //Prevents en passant from happening over multiple moves. 
                        ChessList.GetList(!team)[i].SpecialBool = false;
                        break;
                    }
                }

                if (!team)
                    PieceAmountUpdate(); //needs to be updated after black player turn.

                if (!GameStates.GameEnded)
                    draw = Draw(false, updatePieces: !team); //ensures that draw 50 move counter is updated after black player turn. Auto-draw from the fifty-move rule can only happen at the end of black player turn
                if (!team)
                    MapMatrix.UpdateOldMap();
                if (checkmate == true || draw || otherPlayerCheckMate == true)
                {
                    if (draw)
                        GameStates.Won = null;
                    else if (checkmate == true) //this one would never be true as otherPlayerCheckMate will be true before this one can be true. 
                        GameStates.Won = false;
                    else if (otherPlayerCheckMate == true)
                        GameStates.Won = true;
                    return true;
                }
                if (!GameStates.GameEnded)
                {

                    bool successTransmission = Network.Transmit.TransmitMapData(Network.Transmit.OtherPlayerIpAddress);
                    if (!successTransmission) //if it fails to transmit data end the game
                        return true;
                }
                else
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Updates the GameStates.PieceAmount.
        /// </summary>
        private void PieceAmountUpdate()
        {
            GameStates.PieceAmount[1, 1] = GameStates.PieceAmount[1, 0];
            GameStates.PieceAmount[1, 0] = (byte)ChessList.GetList(false).Count;
            GameStates.PieceAmount[0, 1] = GameStates.PieceAmount[0, 0];
            GameStates.PieceAmount[0, 0] = (byte)ChessList.GetList(true).Count;
        }

        /// <summary>
        /// Contains the code that allows the game to be played and loops through it until either a draw or one side wins.
        /// </summary>
        private void GameLoop()
        {
            GameRunTitle();
            bool gameEnded = false;
            GameStates.IsOnline = false;
            GameStates.PieceAmount[0, 0] = (byte)ChessList.GetList(true).Count;
            GameStates.PieceAmount[0, 1] = (byte)ChessList.GetList(true).Count;
            GameStates.PieceAmount[1, 0] = (byte)ChessList.GetList(false).Count;
            GameStates.PieceAmount[1, 1] = (byte)ChessList.GetList(false).Count;

            do
            {
                gameEnded = PlayerControl(true);
                if (gameEnded || GameStates.GameEnded)
                {
                    GameStates.GameEnded = true;
                    break;
                }
                gameEnded = PlayerControl(false);
                if (gameEnded || GameStates.GameEnded)
                    GameStates.GameEnded = true;
            } while (!GameStates.GameEnded);
            ChessList.RemoveAllPieces();
            Console.Clear();
            EndScreen();
            GameStates.Reset();
            MapMatrix.AllowForMapPreparation();

            unsafe bool PlayerControl(bool team)
            {
                if (team)
                {
                    MapMatrix.UpdateOldMap();
                    GameStates.TurnCounter++;
                }
                bool checkmate = false; bool draw = false;
                Player player;
                if (team)
                    player = white;
                else
                    player = black;

                ProtectKing.CannotMove = ProtectingTheKing(team);

                player.Control();
                IsKingChecked(team);
                ProtectKing.ProtectEndLocations.Clear();
                ProtectKing.ProtectingTheKing.Clear();


                for (int i = ChessList.GetList(team).Count - 1; i >= 0; i--) //ensures any promoted pawns are removed from the list.
                {
                    if (ChessList.GetList(team)[i].BeenCaptured)
                        ChessList.GetList(team).RemoveAt(i);
                }
                for (int i = ChessList.GetList(!team).Count - 1; i >= 0; i--) //removes any captured hostile pieces. 
                {
                    if (ChessList.GetList(!team)[i].BeenCaptured)
                        ChessList.GetList(!team).RemoveAt(i);
                }
                for (int i = ChessList.GetList(!team).Count - 1; i >= 0; i--) //ensures that en passant is set to false if it is true.
                {
                    if (ChessList.GetList(!team)[i] is Pawn && ChessList.GetList(!team)[i].SpecialBool == true)
                        ChessList.GetList(!team)[i].SpecialBool = false;
                }
                PieceAmountUpdate();

                checkmate = CheckmateChecker(!team, out List<string> saveKingList);
                ProtectKing.Protect = saveKingList;

                if (checkmate)
                {
                    GameStates.WhiteWin = team;
                    GameStates.Won = true;
                    return true;
                }

                draw = Draw(team, team);
                if (draw)
                {
                    GameStates.Won = null;
                    return true;
                }
                return false;

            }
        }

        /// <summary>
        /// Calls the function that runs the loop and code that plays the game online.
        /// </summary>
        public void NetPlay(bool playerStarter)
        {
            GameLoopNet(playerStarter);
        }

        /// <summary>
        /// Calls the function that runs the loop and code that plays the game offline.
        /// </summary>
        public void LocalPlay()
        {
            GameLoop();
        }

        /// <summary>
        /// Check if a king, colour depending on <paramref name="team"/>, is checked or not. If the king is checked, it will return true. Else false. 
        /// If null is returned, it means there is no king.
        /// </summary>
        /// <param name="team">True for white, false for black.</param>
        /// <returns>True if the king is checked, else false. If null is returned it means there is no king.</returns>
        private bool? IsKingChecked(bool team)
        {
            foreach (ChessPiece chePie in ChessList.GetList(team))
            {
                if (chePie is King)
                {
                    return chePie.SpecialBool;
                }
            }
            return null; //if this is reached, it would mean there is no king and something has gone very wrong. 
        }

        /// <summary>
        /// Checks if there are any pieces that are protecting the king.
        /// If there is none, the returned list will be empty.
        /// </summary>
        /// <param name="team">The team that should be checked. True for white, false for black.</param>
        /// <returns></returns>
        private List<string> ProtectingTheKing(bool team)
        {
            int[] kingLocation = new int[2];
            int[][] directions = new int[][]
            {
                new int[] {0,1 },
                new int[] {0,-1 },
                new int[] {1,0 },
                new int[] {-1,0 },
                new int[] {1,1 },
                new int[] {1,-1 },
                new int[] {-1,1 },
                new int[] {-1,-1 }
            };
            List<string> protectingPieces = new List<string>();
            foreach (ChessPiece chePie in ChessList.GetList(team))
            {
                if (chePie is King king)
                {
                    kingLocation = king.GetMapLocation;
                    break;
                }
            }
            foreach (ChessPiece chePie in ChessList.GetList(team))
            {
                if (chePie is Pawn pawn)
                {
                    PawnProtecting(pawn);
                }
                else if (chePie.GetID.Split(':')[1] != "1")
                {
                    Protecting(chePie);
                }
            }
            return protectingPieces;

            void PawnProtecting(Pawn pawn)
            {
                int[] hostileLocation = new int[2];
                int[] scaledDifferences = new int[2];
                int[] hostileDiffernece = new int[2];
                int[] differences = new int[] { pawn.GetMapLocation[0] - kingLocation[0], pawn.GetMapLocation[1] - kingLocation[1] };
                if (differences[0] != 0 && (differences[0] == differences[1] || differences[0] == -differences[1]))
                    scaledDifferences[0] = differences[0] > 0 ? 1 : -1; //scales it to be 1 or -1
                else if (differences[0] == 0)
                    scaledDifferences[1] = differences[1] > 0 ? 1 : -1;
                if (differences[1] != 0 && (differences[0] == differences[1] || differences[0] == -differences[1]))
                    scaledDifferences[1] = differences[1] > 0 ? 1 : -1;
                else if (differences[1] == 0)
                    scaledDifferences[0] = differences[0] > 0 ? 1 : -1;
                bool canReachKing = false;
                int[] mov = new int[2];
                foreach (int[] move in directions)
                {
                    if (move[0] == scaledDifferences[0] && move[1] == scaledDifferences[1])
                    {
                        mov = move;
                        canReachKing = true;
                        break;
                    }
                }
                if (canReachKing)
                {

                    List<string> toCheckFor = new List<string>();
                    if (mov[0] != 0 && mov[1] != 0)
                    {
                        toCheckFor.Add("2");
                        toCheckFor.Add("3");
                    }
                    else
                    {
                        toCheckFor.Add("2");
                        toCheckFor.Add("5");
                    }
                    bool keepKingSafe = false;
                    //check all squres in both directions of mov (-mov) 
                    if (PawnFeltCheck(toCheckFor))
                    {
                        keepKingSafe = PawnFeltCheck(new List<string>() { "1" }, true); //need to first check the other direction
                        hostileDiffernece = new int[] { hostileLocation[0] - pawn.GetMapLocation[0], hostileLocation[1] - pawn.GetMapLocation[1] };
                        if (keepKingSafe)
                            protectingPieces.Add(pawn.GetID);
                        //if it is true, remove the (..., true) and just add it to the list in this if-statement.
                    }

                    if (mov[0] == 0 && keepKingSafe) //this means that the king is either above or below the pawn and thus can move. 
                    {
                        List<int[,]> canStandOn = new List<int[,]>();
                        int dir = pawn.GetDirection[0][1];
                        bool cantDouble = pawn.HasMoved;
                        byte maxRange = !cantDouble ? (byte)2 : (byte)1;
                        byte pos = 0;
                        do
                        {
                            pos++;
                            if (MapMatrix.Map[pawn.GetMapLocation[0], pawn.GetMapLocation[1] + (dir * pos)] == "")
                            {
                                canStandOn.Add(new int[,] { { pawn.GetMapLocation[0], pawn.GetMapLocation[1] + (dir * pos) } });
                            }
                            else
                                break;
                        } while (pos < maxRange);
                        //if (canStandOn.Count != 0)
                        ProtectKing.ProtectingTheKing.Add(pawn.GetID, canStandOn);
                    }
                    else if (keepKingSafe && mov[0] == hostileDiffernece[0] && mov[1] == hostileDiffernece[1]) //can capture the hostile
                    {
                        ProtectKing.ProtectingTheKing.Add(pawn.GetID, new List<int[,]>() { new int[,] { { hostileLocation[0], hostileLocation[1] } } });
                    }
                    else if (keepKingSafe) //cannot move
                        ProtectKing.ProtectingTheKing.Add(pawn.GetID, new List<int[,]>());

                }
                bool PawnFeltCheck(List<string> toCheckFor, bool direction = false)
                {
                    int[] loc_ = new int[] { pawn.GetMapLocation[0], pawn.GetMapLocation[1] };
                    bool foundPiece = false;
                    bool shouldCheck = true;
                    string teamSignToCheck;
                    sbyte dir = direction ? (sbyte)-1 : (sbyte)1;
                    do
                    {
                        if (direction)
                            teamSignToCheck = team ? "+" : "-";
                        else
                            teamSignToCheck = team ? "-" : "+";
                        loc_[0] += mov[0] * dir;
                        loc_[1] += mov[1] * dir;
                        if ((loc_[0] <= 7 && loc_[0] >= 0) && (loc_[1] <= 7 && loc_[1] >= 0))
                        {
                            string id = MapMatrix.Map[loc_[0], loc_[1]];
                            if (id != "")
                            {
                                string[] IDparts = id.Split(':');
                                if (IDparts[0] == teamSignToCheck)
                                {
                                    foreach (string checkFor in toCheckFor)
                                    {
                                        if (checkFor == IDparts[1])
                                        {
                                            foundPiece = true;
                                            shouldCheck = false;
                                            if (!direction)
                                            {
                                                hostileLocation[0] = loc_[0];
                                                hostileLocation[1] = loc_[1];
                                            }
                                            break;
                                        }
                                    }
                                    shouldCheck = false;
                                }
                                else
                                {
                                    if (direction && (kingLocation[0] != loc_[0] && kingLocation[1] != loc_[1])) //need to check if the piece is the king. If it is it means the pawn is protecting the king.
                                        foundPiece = true;
                                    shouldCheck = false;
                                    break;
                                }

                                //shouldCheck = true;
                            }
                        }
                        else
                            shouldCheck = false;
                    } while (shouldCheck);
                    return foundPiece;
                }

            }

            void Protecting(ChessPiece pieces)
            {
                int[] scaledDifferences = new int[2];
                int[] differences = new int[] { pieces.GetMapLocation[0] - kingLocation[0], pieces.GetMapLocation[1] - kingLocation[1] };
                if (differences[0] != 0 && (differences[0] == differences[1] || differences[0] == -differences[1]))
                    scaledDifferences[0] = differences[0] > 0 ? 1 : -1; //scales it to be 1 or -1
                else if (differences[0] == 0)
                    scaledDifferences[1] = differences[1] > 0 ? 1 : -1;
                if (differences[1] != 0 && (differences[0] == differences[1] || differences[0] == -differences[1]))
                    scaledDifferences[1] = differences[1] > 0 ? 1 : -1;
                else if (differences[1] == 0)
                    scaledDifferences[0] = differences[0] > 0 ? 1 : -1;
                bool canReachKing = false;

                int[] mov = new int[2];
                foreach (int[] move in directions)
                {
                    if (move[0] == scaledDifferences[0] && move[1] == scaledDifferences[1])
                    {
                        mov = move;
                        canReachKing = true;
                        break;
                    }
                }
                if (canReachKing) //check in the direction of mov for a hostile piece and if the hostile piece got the oppesite of mov
                {
                    List<string> toCheckFor = new List<string>();
                    if (mov[0] != 0 && mov[1] != 0)
                    {
                        toCheckFor.Add("2");
                        toCheckFor.Add("3");
                    }
                    else
                    {
                        toCheckFor.Add("2");
                        toCheckFor.Add("5");
                    }
                    //int[] loc_ = new int[] { chepie.GetMapLocation[0], chepie.GetMapLocation[1] };

                    List<int[,]> endLocations = new List<int[,]>();
                    bool needToCheckOtherDirection = CheckFelts(mov, toCheckFor); //true if it find a hostile pice
                    bool clearPathToKing = false;
                    if (needToCheckOtherDirection) //true if there is nothing between it and the king
                        clearPathToKing = CheckFelts(new int[] { -mov[0], -mov[1] }, new List<string>() { "1" }, true);
                    if (clearPathToKing) //need to ensure a piece can only be moved in its directions. Pawn can be slightly difficult.
                    {
                        foreach (int[] dir in pieces.GetDirection)
                        {
                            if (dir[0] == mov[0] && dir[1] == mov[1])
                            {

                                CheckFelts(mov, toCheckFor, addLocationToDic: true);
                                CheckFelts(new int[] { -mov[0], -mov[1] }, toCheckFor, addLocationToDic: true);
                                //ProtectKing.ProtectingTheKing.Add(pieces.GetID, endLocations);
                                break;
                            }
                        }
                        ProtectKing.ProtectingTheKing.Add(pieces.GetID, endLocations);
                    }

                    bool CheckFelts(int[] mov_, List<string> lookFor, bool addToList = false, bool addLocationToDic = false)
                    { //does not work with pawn movement
                        int[] loc_ = new int[] { pieces.GetMapLocation[0], pieces.GetMapLocation[1] };
                        bool foundPiece = false;
                        bool shouldCheck = true;
                        do
                        {
                            loc_[0] += mov_[0];
                            loc_[1] += mov_[1];
                            if ((loc_[0] <= 7 && loc_[0] >= 0) && (loc_[1] <= 7 && loc_[1] >= 0))
                            {
                                string id = MapMatrix.Map[loc_[0], loc_[1]];
                                if (id != "")
                                {
                                    string[] IDparts = id.Split(':');
                                    string teamSignToCheck = "";
                                    if (addToList)
                                        teamSignToCheck = team ? "+" : "-";
                                    else
                                        teamSignToCheck = team ? "-" : "+";
                                    if (teamSignToCheck == IDparts[0])
                                    {

                                        foreach (string checkFor in lookFor)
                                        {

                                            if (checkFor == IDparts[1])
                                            {
                                                if (addToList)
                                                    protectingPieces.Add(pieces.GetID);
                                                if (addLocationToDic)
                                                    endLocations.Add(new int[,] { { loc_[0], loc_[1] } });
                                                foundPiece = true;
                                                shouldCheck = false; //maybe add a bool foundHostile and later if try, go through a similar loop, but add each location  
                                                break; //if the piece can move in that direction //if foundHostile is true, check the other direction for any piece
                                            }
                                        }
                                        shouldCheck = false;
                                    }
                                    else
                                    {
                                        shouldCheck = false;
                                        break;
                                    }

                                    //shouldCheck = true;
                                }
                                else if (addLocationToDic && id == "")
                                {
                                    endLocations.Add(new int[,] { { loc_[0], loc_[1] } });
                                }
                            }
                            else
                                shouldCheck = false;
                        } while (shouldCheck);
                        return foundPiece;
                    }
                }



            }
        }

        /// <summary>
        /// Check if a king, depending on <paramref name="team"/> is checkmated or not. If the king is checkmated, it will return true. Else false. 
        /// </summary>
        /// <param name="team">True for white, false for black.</param>
        /// <returns>True if the king is checkmated, else false</returns>
        private bool CheckmateChecker(bool team)
        {
            return CheckmateChecker(team, out _);
        }

        /// <summary>
        /// Check if a king, depending on <paramref name="team"/> is checkmated or not. If the king is checkmated, it will return true. Else false. 
        /// Also <paramref name="canProtectKing"/> contains the ID of all pieces that can protect the king, if the king is checked. The king will also be in the list if the king can move. 
        /// </summary>
        /// <param name="team">True for white, false for black.</param>
        /// <returns>True if the king is checkmated, else false</returns>
        private bool CheckmateChecker(bool team, out List<string> canProtectKing)
        {
            List<int[]> locations = new List<int[]>();
            int[] kingLocation = new int[2];
            bool isCheked = false;
            bool isKnight = false;
            bool kingCanMove = false;
            canProtectKing = new List<string>();
            foreach (ChessPiece chePie in ChessList.GetList(team))
            {
                if (chePie is King king)
                {
                    isCheked = chePie.SpecialBool;
                    if (isCheked)
                    {
                        locations = king.GetCheckList;
                        kingLocation = king.GetMapLocation;
                        kingCanMove = king.CanMove;
                        break;
                    }
                }
            }
            if (isCheked)
            {
                foreach (ChessPiece chePie in ChessList.GetList(team))
                {
                    string[] idParts = chePie.GetID.Split(':');
                    int[] chePieLocation = chePie.GetMapLocation;
                    string[] feltIDParts = MapMatrix.Map[locations[0][0], locations[0][1]].Split(':');
                    isKnight = feltIDParts[1] == "4" ? true : false;
                    foreach (string canHelp in ProtectKing.CannotMove)
                    {
                        if (canHelp == chePie.GetID)
                        { //if the ID exist in ProtectKing.CannotMove it is already protecting the king. This ensures it is skipped and minimise this function runtime.
                            break;
                        }
                    }
                    if (chePie is King)
                    {
                        if (kingCanMove)
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, null);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Queen)
                    {
                        int[][] movement = chePie.GetDirection;
                        if (QRBCheck(movement, chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Rook)
                    {
                        int[][] movement = chePie.GetDirection;
                        if (QRBCheck(movement, chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Bishop)
                    {
                        int[][] movement = chePie.GetDirection;
                        if (QRBCheck(movement, chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Knight)
                    {
                        if (KnightCheck(chePie.GetDirection, chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Pawn pawn)
                    {
                        if (PawnCheck(chePie.GetMapLocation, pawn.HasMoved, out List<int[,]> endLocation))
                        {
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                }
                if (canProtectKing.Count != 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;

            bool KnightCheck(int[][] directions, int[] ownLocation, out List<int[,]> endLocations)
            {
                int[] kingHostileDifference = new int[] { kingLocation[0] - locations[0][0], kingLocation[1] - locations[0][1] };
                endLocations = new List<int[,]>();
                KnightCanReach(locations[0], ref endLocations);

                if (!isKnight)
                {
                    int biggestDifference = Math.Abs(kingHostileDifference[0]) < Math.Abs(kingHostileDifference[1]) ? Math.Abs(kingHostileDifference[1]) : Math.Abs(kingHostileDifference[0]);
                    int distance = 2;
                    int[] newLocation = { kingLocation[0], kingLocation[1] };
                    int[] movement = new int[2];
                    if (kingHostileDifference[0] > 0)//left //calculates the location that is needed to go to get from the king to the hostile piece.
                        movement[0] = -1;
                    else if (kingHostileDifference[0] < 0)//right
                        movement[0] = 1;
                    else
                        movement[0] = 0;

                    if (kingHostileDifference[1] > 0)//up
                        movement[1] = -1;
                    else if (kingHostileDifference[1] < 0)//down
                        movement[1] = 1;
                    else
                        movement[1] = 0;

                    while (distance <= biggestDifference) //finds all locations between the king and hostile piece.
                    {
                        newLocation[0] += movement[0];
                        newLocation[1] += movement[1];
                        string feltID = MapMatrix.Map[newLocation[0], newLocation[1]];
                        if (feltID != "")
                            break;
                        KnightCanReach(newLocation, ref endLocations);
                        //return true;
                        distance++;
                    }
                }
                if (endLocations.Count != 0)
                    return true;
                else
                    return false;

                void KnightCanReach(int[] standLocation, ref List<int[,]> endLoc)
                {
                    int[] locationDifference = new int[] { standLocation[0] - locations[0][0], standLocation[1] - locations[0][1] };
                    int[][] movement = directions;
                    foreach (int[] mov in movement)
                    {
                        int[] movLocation = new int[] { ownLocation[0] + mov[0], ownLocation[1] + mov[1] };
                        if (movLocation[0] == standLocation[0] && movLocation[1] == standLocation[1])
                        {
                            if (MapMatrix.Map[movLocation[0], movLocation[1]] == "" || (movLocation[0] == locations[0][0] && movLocation[1] == locations[0][1]))
                                endLoc.Add(new int[,] { { movLocation[0], movLocation[1] } });
                        }
                    }

                }
            }

            bool PawnCheck(int[] ownLocation, bool hasMoved, out List<int[,]> endLocations)
            { //changes this one so it returns depends on endLocations to allow for multiple locations rather than one. 
                int direction = team ? -1 : 1;
                int[] locationDifference = new int[] { ownLocation[0] - locations[0][0], ownLocation[1] - locations[0][1] };
                endLocations = new List<int[,]>();
                if (direction == -1)
                    if (locationDifference[1] == 1) //only a white pawn can move up and positive locationDifference[0] means the pawn is below the threating piece.
                    {
                        if (locationDifference[0] == -1)
                        {
                            endLocations.Add(new int[,] { { ownLocation[0] + 1, ownLocation[1] - 1 } }); //right
                            //return true;
                        }
                        else if (locationDifference[0] == 1)
                        {
                            endLocations.Add(new int[,] { { ownLocation[0] - 1, ownLocation[1] - 1 } }); //left
                            //return true;
                        }
                    }
                if (direction == 1)
                    if (locationDifference[1] == -1) //only a black pawn can move down and negative lcoationDifference[0] means the pawn is above the threatining piece. 
                    {
                        if (locationDifference[0] == -1)
                        {
                            endLocations.Add(new int[,] { { ownLocation[0] + 1, ownLocation[1] + 1 } }); //right
                            //return true;
                        }
                        else if (locationDifference[0] == 1)
                        {
                            endLocations.Add(new int[,] { { ownLocation[0] - 1, ownLocation[1] + 1 } }); //left
                            //return true;
                        }
                    }

                if (!isKnight)
                {

                    int xBig = kingLocation[0] > locations[0][0] ? kingLocation[0] : locations[0][0];
                    int xSmall = kingLocation[0] > locations[0][0] ? locations[0][0] : kingLocation[0];
                    if (ownLocation[0] > xSmall && ownLocation[0] < xBig)
                    {
                        int[] directions = new int[] { kingLocation[0] - locations[0][0], kingLocation[1] - locations[0][1] };
                        int[] movement = new int[2];
                        if (directions[0] > 0)//left //calculates the location that is needed to go to get from the king to the hostile piece.
                            movement[0] = -1;
                        else if (directions[0] < 0)//right
                            movement[0] = 1;

                        if (directions[1] > 0)//up
                            movement[1] = -1;
                        else if (directions[1] < 0)//down
                            movement[1] = 1;
                        else
                            movement[1] = 0;

                        int[] standLocation = new int[2] { kingLocation[0], kingLocation[1] };
                        do
                        { //finds the y of the square that is between king and hostile piece that got the same x as the pawn.
                            standLocation[0] += movement[0];
                            standLocation[1] += movement[1];
                        } while (standLocation[0] != ownLocation[0]);

                        int yDistance = standLocation[1] - ownLocation[1];
                        int maxRange = hasMoved ? 1 : 2;
                        int pos = 0;
                        if (maxRange >= Math.Abs(yDistance))
                        {
                            do
                            {
                                pos++;
                                string feltID = MapMatrix.Map[ownLocation[0], ownLocation[1] + direction * pos];
                                if (feltID != "")
                                    break;
                            } while (pos < Math.Abs(yDistance));
                            if (ownLocation[1] + (pos * direction) == standLocation[1])
                                endLocations.Add(new int[,] { { ownLocation[0], ownLocation[1] + (pos) * direction } });
                        }
                    }
                }
                if (endLocations.Count != 0)
                    return true;
                else
                    return false;
            }

            bool QRBCheck(int[][] directions, int[] ownLocation, out List<int[,]> endLocations)
            { //queen could save king, should have been able to take the hostile piece and stand between it and the king. However, it could only capture the hostile piece.
                endLocations = new List<int[,]>();
                foreach (int[] dir in directions)
                {
                    /* To take:
                     * If locationDifference == e.g. [2,3] no piece can reach it expect maybe for special moves like the knight.
                     * if locationDIfference == e.g. [2,2] it can only be reached by a piece that moves diagonal, i.e. bishop and queen. 
                     * If and only if locationDifference == [1,1] can a pawn reach it.
                     * if locationDifference == e.g. [0,2] or e.g. [2,0] it can only be reached by rock and queen.
                     * In all cases it also need to check the inbetween squares to see if there is a free path.  
                     * If locationDifference == any combination of either 0s and 1s, the king can reach it.
                     * Knights need to check if they can land on the locationDifference square or not. 
                     * 
                     * To intercept: 
                     * Find every square between the king and the hostile piece. Check if any square can be "taken" as above. 
                     */

                    if (CanReach(dir, ownLocation, locations[0], ref endLocations))
                    {
                        endLocations.Add(new int[,] { { locations[0][0], locations[0][1] } });
                    }
                    else if (!isKnight)
                    {
                        int[] kingHostileDifference = new int[] { kingLocation[0] - locations[0][0], kingLocation[1] - locations[0][1] }; //negative right/down, positve left/up
                        int biggestDifference = Math.Abs(kingHostileDifference[0]) < Math.Abs(kingHostileDifference[1]) ? Math.Abs(kingHostileDifference[1]) : Math.Abs(kingHostileDifference[0]);
                        int distance = 2;  //king and hostile piece cannot stand on the same location, so no reason to start at 0. 
                        //if the king and hostile piece is right next to each other, biggestDifference will be 1. Thus the reason for starting with 2  
                        int[] newLocation = { kingLocation[0], kingLocation[1] };
                        int[] movement = new int[2];
                        if (kingHostileDifference[0] > 0)//left //calculates the location that is needed to go to get from the king to the hostile piece.
                            movement[0] = -1;
                        else if (kingHostileDifference[0] < 0)//right
                            movement[0] = 1;
                        else
                            movement[0] = 0; //this if-else statement and the one below, does not seem to work that well when the hostile piece is between the piece running this code and the king. 
                                             //then again, this code should not be reached in that case and most likely only the bug that is causing it to be reached
                        if (kingHostileDifference[1] > 0)//up
                            movement[1] = -1;
                        else if (kingHostileDifference[1] < 0)//down
                            movement[1] = 1;
                        else
                            movement[1] = 0;

                        while (distance <= biggestDifference)
                        {
                            newLocation[0] += movement[0];
                            newLocation[1] += movement[1];
                            string feltID = MapMatrix.Map[newLocation[0], newLocation[1]];
                            if (feltID != "")
                                break;
                            CanReach(dir, ownLocation, newLocation, ref endLocations);
                            //return true;
                            distance++;
                        }
                    }
                }
                if (endLocations.Count != 0)
                    return true;
                else
                    return false;
            }

            bool CanReach(int[] dir, int[] ownLocation, int[] toEndOnLocation, ref List<int[,]> endLocations)
            {
                bool index1Sign; bool index2Sign; bool diagonal; bool straight;

                int[] locationDifference = new int[] { ownLocation[0] - toEndOnLocation[0], ownLocation[1] - toEndOnLocation[1] };  //negative right/down, positve left/up
                if (locationDifference[0] != 0 && dir[0] != 0) //find a way to make this look better
                {
                    int sign = locationDifference[0] / dir[0];
                    index1Sign = sign > 0 ? false : true; //if above zero, the signs are the same. Different signs will give a negative result. The piece can only reach the toEndOnLocation if the signs are different. 
                }
                else if (locationDifference[0] == 0 && dir[0] == 0)
                    index1Sign = true;
                else //if only one of the variables specific index is zero and the other one is not, can never reach the destination
                    index1Sign = false;

                if (locationDifference[1] != 0 && dir[1] != 0)
                {
                    int sign = locationDifference[1] / dir[1];
                    index2Sign = sign > 0 ? false : true;
                }
                else if (locationDifference[1] == 0 && dir[1] == 0)
                    index2Sign = true;
                else
                    index2Sign = false;

                if (locationDifference[0] != 0 && locationDifference[1] != 0) //cannot be reach by a straight movement. 
                {
                    if (Math.Abs(locationDifference[0]) == Math.Abs(locationDifference[1])) //can be reached by a diagonal movement
                    {
                        diagonal = true;
                    }
                    else //cannot be reached by a diagonal movement;
                        diagonal = false;
                }
                else
                    diagonal = true;

                if (locationDifference[0] == 0 || locationDifference[1] == 0) //can be reached by a straight movement.
                {
                    straight = true;
                }
                else
                    straight = false;

                if (index1Sign && index2Sign && (diagonal || straight))
                {
                    /*
                     * if both parts of locationDifference is the same, it need to not have the same signs in the dir
                     * E.g. locationDifference == [2,2], dir needs to be [-1,-1]
                     * If LocationDifference == [-2,2], dir needs to be [1,-1]. 
                     * If locationDifference == [0,2], dir needs to be [0,-1] 
                     * etc.
                     * So if any index in locationDifference is zero, the same index in dir needs to be zero,
                     * else any index in dir needs to be of oppesit sign of the same index in locationDifference. 
                     */

                    int[] currentLocation = new int[] { ownLocation[0], ownLocation[1] };
                    int locationsRemaining = Math.Abs(locationDifference[0]) > Math.Abs(locationDifference[1]) ? Math.Abs(locationDifference[0]) : Math.Abs(locationDifference[1]);
                    string feltID = ""; //maybe have a setting for the default value on the map
                    while (locationsRemaining != 0) //rewrite all of this, also write better comments for the future
                    {
                        currentLocation[0] += dir[0];
                        currentLocation[1] += dir[1];
                        feltID = MapMatrix.Map[currentLocation[0], currentLocation[1]];
                        if (feltID != "" && feltID != MapMatrix.Map[locations[0][0], locations[0][1]])
                            return false;

                        if (locationsRemaining == 1)
                        {
                            endLocations.Add(new int[,] { { currentLocation[0], currentLocation[1] } });
                            return true;
                        }
                        locationsRemaining--;
                    }
                }
                return false;
            }

        }

        /// <summary>
        /// Sets up the two players.
        /// </summary>
        private void PlayerSetup()
        {
            white = new Player(true, Publishers.PubKey);
            black = new Player(false, Publishers.PubKey);
        }

        /// <summary>
        /// Creates the chess pieces. 
        /// </summary>
        private void CreatePieces(bool white, int[,] spawnLocations, byte[] colour)
        {
            string team = white == true ? "+" : "-";
            string ID;
            int[] spawn;
            List<ChessPiece> chessPieces = new List<ChessPiece>();
            for (int i = 0; i < 8; i++)
            {
                ID = String.Format("{0}:6:{1}", team, i + 1);
                spawn = new int[] { spawnLocations[i, 0], spawnLocations[i, 1] };
                chessPieces.Add(new Pawn(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            }
            ID = String.Format("{0}:5:{1}", team, 1);
            spawn = new int[] { spawnLocations[8, 0], spawnLocations[8, 1] };
            chessPieces.Add(new Rook(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:5:{1}", team, 2);
            spawn = new int[] { spawnLocations[15, 0], spawnLocations[15, 1] };
            chessPieces.Add(new Rook(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:4:{1}", team, 1);
            spawn = new int[] { spawnLocations[9, 0], spawnLocations[9, 1] };
            chessPieces.Add(new Knight(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:4:{1}", team, 2);
            spawn = new int[] { spawnLocations[14, 0], spawnLocations[14, 1] };
            chessPieces.Add(new Knight(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:3:{1}", team, 1);
            spawn = new int[] { spawnLocations[10, 0], spawnLocations[10, 1] };
            chessPieces.Add(new Bishop(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:3:{1}", team, 2);
            spawn = new int[] { spawnLocations[13, 0], spawnLocations[13, 1] };
            chessPieces.Add(new Bishop(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:2:{1}", team, 1);
            spawn = new int[] { spawnLocations[11, 0], spawnLocations[11, 1] };
            chessPieces.Add(new Queen(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));
            ID = String.Format("{0}:1:{1}", team, 1);
            spawn = new int[] { spawnLocations[12, 0], spawnLocations[12, 1] };
            chessPieces.Add(new King(colour, white, spawn, ID, Publishers.PubKey, Publishers.PubCapture));

            ChessList.SetChessList(chessPieces, white);
        }
        //protected void KeyEventHandler(object sender, ControlEvents e)
        //{

        //}
    }
}
