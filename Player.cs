using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// 
    /// </summary>
    class Player //got to ensure that no spawnlocation is overlapping and deal with it in case there is an overlap. 
    { //this class is set to be an abstract in the UML, but is that really needed? 
        private bool white;
        private string team;
        private int selectedChessPiece;
        private int[] location; //x,y
        private bool didMove = false;
        private ConsoleKeyInfo key;
        private bool isActive = false;

        public Player(bool startTurn, KeyPublisher keyPub)
        {
            this.white = startTurn;
            team = white == true ? "+" : "-";
            keyPub.RaiseKeyEvent += KeyEventHandler;
            isActive = startTurn;
        }

        /// <summary>
        /// Function that calls functions needed for playing the game.
        /// </summary>
        public void Control()
        {
            isActive = true;
            do
            {
                bool didNotSurrender = HoverOver();
                if (didNotSurrender)
                {
                    MovePiece();
                    didMove = ChessList.GetList(white)[selectedChessPiece].CouldMove;
                }
                else
                {
                    break;
                }

            } while (!didMove);
            isActive = false;
        }

        /// <summary>
        /// Allows access to the PlayerMenu() function if pressing esc
        /// </summary>
        public void ControlOnlyMenu()
        {
            isActive = true;
            while (!GameStates.IsTurn && !GameStates.GameEnded)
                if ((int)key.Key != 0)
                    if (key.Key == ConsoleKey.Escape)
                        PlayerMenu();
            isActive = false;
        }

        /// <summary>
        /// Lets the player hover over a felt on the boardgame and gets the ID of that felt. If the ID is a chesspiece, it will be highlighted. 
        /// </summary>
        private bool HoverOver()
        {
            string lastMapLocationID;
            int? lastPiece = 0;
            ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(true);
            bool hasSelected = false;
            bool? friendlyPiece = true;
            location = ChessList.GetList(white)[0].GetMapLocation;
            SquareHighLight(true);
            do
            {
                bool? selected = FeltMove(location);
                if (lastPiece != null && !GameStates.GameEnded) //if a previous chess piece has been hovered over, remove the highlight. 
                {
                    if (friendlyPiece == true)
                        ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(false);
                    else if (friendlyPiece == false)
                        ChessList.GetList(!white)[(int)lastPiece].IsHoveredOn(false);
                    lastPiece = null;
                    lastMapLocationID = null;
                    friendlyPiece = null;
                }
                string squareID = MapMatrix.Map[location[0], location[1]];
                if (selected == null)
                {
                    if (GameStates.GameEnded)
                    {
                        hasSelected = true;
                        return false;
                    }
                }
                else if (squareID != "") //is there a chess piece on the location
                    if (squareID.Split(':')[0] == team) //is it on the team
                    {
                        int posistion = 0; //used for later finding the posistion in the List<ChessPiece> the chosen chesspiece got.
                        foreach (ChessPiece piece in ChessList.GetList(white))
                        {
                            if (piece.GetID == squareID)
                            { //correct chess piece and highlight it
                                piece.IsHoveredOn(true);
                                lastMapLocationID = piece.GetID;
                                lastPiece = posistion;
                                friendlyPiece = true;
                                if (selected == true)
                                {
                                    if (ProtectKing.Protect.Count != 0) //ensures only pieces that can protect the king can be moved
                                    {
                                        foreach (string id in ProtectKing.Protect)
                                        {
                                            if (piece.GetID == id)
                                            {
                                                hasSelected = true;
                                                selectedChessPiece = posistion;
                                                ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(false);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        hasSelected = true;
                                        selectedChessPiece = posistion;
                                        ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(false);
                                        break;
                                    }

                                }
                                break;
                            }
                            posistion++;
                        }
                    }
                    else
                    {
                        int posistion = 0;
                        if (!Settings.CanHighLight)
                            foreach (ChessPiece chePie in ChessList.GetList(!white))
                            {
                                if (chePie.GetID == squareID)
                                {
                                    friendlyPiece = false;
                                    chePie.IsHoveredOn(true, true);
                                    lastPiece = posistion;
                                    break;
                                }
                                posistion++;
                            }
                    }

            } while (!hasSelected);
            SquareHighLight(false);
            return true;
        }

        /// <summary>
        /// Menu that allows a player to stay on playing, ask for a draw or surrender 
        /// </summary>
        private void PlayerMenu()
        { //should allow the player to surrender, ask for a draw or return to play.
            //remember, you can call the board paint and chess paint functions to recreate the visual board and visual pieces.
            //Should people be allowed to use the menu while it is not their turn? If they are, have to deal with a menu appearing while they e.g. moving a piece and such.
            Console.Title = Settings.GetTitle + ": Player Menu";
            string[] playerOptions =
            {
                "Stay Playing",
                "Call for Draw",
                "Game Stats",
                "Surrender"
            };
            Console.Clear();
            string title = "Options";
            isActive = false;
            string answer = Menu.MenuAccess(playerOptions, title);
            isActive = true;
            switch (answer)
            {
                case "Stay Playing":
                    Console.Clear();
                    ChessTable.GameRunTitle();
                    ChessTable.RepaintBoardAndPieces();
                    break;

                case "Call for Draw":
                    if (GameStates.IsOnline)
                    {
                        Network.Transmit.GeneralValueTransmission(3, Network.Transmit.OtherPlayerIpAddress);
                        GameStates.Pause = true;
                        Console.Clear();
                        Console.SetCursorPosition(Settings.MenuTitleOffset[0], Settings.MenuTitleOffset[1]);
                        Console.WriteLine("Waiting on answer");
                        while (GameStates.Pause) ;
                        ChessTable.GameRunTitle();
                    }
                    else
                    {
                        Console.Clear();
                        string drawTitle = "Other player wants to draw";
                        string[] drawOptions = { "Accept Draw", "Decline Draw" };
                        string drawAnswer = Menu.MenuAccess(drawOptions, drawTitle);
                        switch (drawAnswer)
                        {
                            case "Accept Draw":
                                GameStates.GameEnded = true;
                                GameStates.VictoryType = null;
                                break;

                            case "Decline Draw":
                                Console.Clear();
                                ChessTable.RepaintBoardAndPieces();
                                ChessTable.GameRunTitle();
                                break;
                        }
                    }
                    break;

                case "Surrender":
                    GameStates.GameEnded = true;
                    GameStates.VictoryType = false;
                    GameStates.WhiteWin = !white;
                    if (GameStates.IsOnline /*&& !GameStates.IsTurn*/)
                    {
                        if (GameStates.PlayerTeam == false)
                            GameStates.TurnCounter++;
                        Network.Transmit.GeneralValueTransmission(41, Network.Transmit.OtherPlayerIpAddress);
                    }
                    break;

                case "Game Stats":
                    GameStatsDisplay();
                    ChessTable.GameRunTitle();
                    ChessTable.RepaintBoardAndPieces();
                    break;
            }
        }

        /// <summary>
        /// Displays the stats of the game.
        /// </summary>
        private void GameStatsDisplay()
        {
            string mainColour = Settings.CVTS.BrightWhiteForeground;
            string highLightColour = Settings.CVTS.BrightCyanForeground;
            string underscore = Settings.CVTS.Underscore;
            string underscore_off = Settings.CVTS.Underscore_Off;
            string reset = Settings.CVTS.Reset;
            string enterColour = Settings.CVTS.BrightRedForeground;

            Console.Clear();
            Console.CursorTop = Settings.MenuOffset[1];
            Console.WriteLine($"\x1b]2;Chess: Game Stats\x07" + //sets the title
                $"{"".PadLeft(Settings.MenuTitleOffset[0])}{mainColour}{underscore}Stats:{reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{mainColour}Amount of white pieces left: {highLightColour}{underscore}{ChessList.GetList(true).Count}{underscore_off}{mainColour}.{reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{mainColour}Amount of black pieces left: {highLightColour}{underscore}{ChessList.GetList(false).Count}{underscore_off}{mainColour}.{reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{mainColour}Amount of turns: {highLightColour}{underscore}{GameStates.TurnCounter}{underscore_off}{mainColour}.{reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{mainColour}Turns since last capture or pawn move: {highLightColour}{underscore}{GameStates.TurnDrawCounter}{underscore_off}{mainColour}.{reset}{Environment.NewLine}" +
                $"{"".PadLeft(Settings.MenuOffset[0])}{enterColour}Enter {mainColour}to return.{reset}");
            Console.ReadLine();
            Console.Clear();
        }

        /// <summary>
        /// Allows the player to either move to a connecting square to <paramref name="currentLocation"/> or select the <paramref name="currentLocation"/>.
        /// </summary>
        /// <param name="currentLocation">The current location on the board.</param>
        /// <returns>Returns true if enter is pressed, null if escape is pressed, else false.</returns>
        private bool? FeltMove(int[] currentLocation)
        {
            //ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
            //while (Console.KeyAvailable) //flushes any keys pressed to early (does not seem to work all the time).
            //{
            //    Console.ReadKey(true);
            //}

            while (/*!Console.KeyAvailable*/ (int)key.Key == 0 && !GameStates.GameEnded) ;
            //if(!GameStates.GameEnded)
            //    keyInfo = Console.ReadKey(true);

            while (GameStates.Pause) ;

            if (!GameStates.GameEnded)
            {
                if (key.Key == Settings.UpKey && currentLocation[1] > 0)
                {
                    SquareHighLight(false);
                    currentLocation[1]--;
                    SquareHighLight(true);
                }
                else if (key.Key == Settings.DownKey && currentLocation[1] < 7)
                {
                    SquareHighLight(false);
                    currentLocation[1]++;
                    SquareHighLight(true);
                }
                else if (key.Key == Settings.LeftKey && currentLocation[0] > 0)
                {
                    SquareHighLight(false);
                    currentLocation[0]--;
                    SquareHighLight(true);
                }
                else if (key.Key == Settings.RightKey && currentLocation[0] < 7)
                {
                    SquareHighLight(false);
                    currentLocation[0]++;
                    SquareHighLight(true);
                }
                else if (key.Key == Settings.SelectKey)
                {
                    key = new ConsoleKeyInfo();
                    return true;
                }
                else if (key.Key == Settings.EscapeKey)
                {
                    PlayerMenu();
                    SquareHighLight(true);
                    key = new ConsoleKeyInfo();
                    return null;
                }
                key = new ConsoleKeyInfo();
                return false;
            }
            key = new ConsoleKeyInfo();
            return null;
        }

        /// <summary>
        /// Highlights or removes the hightlight of a sqaure. 
        /// </summary>
        /// <param name="isHighlighted">If true highlights the square. If false, it will remove the highligh.</param>
        private void SquareHighLight(bool isHighlighted)
        {
            byte squareSize = Settings.SquareSize;
            int startLocationX = location[0] * squareSize + (location[0] + Settings.Spacing + Settings.EdgeSpacing) * 1 + Settings.Offset[0];
            int startLocationY = location[1] * squareSize + (location[1] + Settings.Spacing + Settings.EdgeSpacing) * 1 + Settings.Offset[1];
            bool emptySquare = MapMatrix.Map[location[0], location[1]] == "" ? true : false;
            bool shallHighlight = true;
            if (!Settings.CanHighLight)
                if (!emptySquare)
                    shallHighlight = false;

            if (isHighlighted)
            {
                byte[] colour = Settings.SelectSquareColour;
                if (shallHighlight)
                    Paint(colour);
            }
            else
            {
                byte colorLocator = (byte)((location[0] + location[1]) % 2);
                byte[] colour = colorLocator == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
                if (shallHighlight)
                    Paint(colour);
            }

            void Paint(byte[] colour)
            {
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
        }

        /// <summary>
        /// Function that calls the selected chess piece control function. 
        /// </summary>
        private void MovePiece()
        {
            List<int[,]> locations = null;
            locations = ProtectKing.GetListFromDic(ChessList.GetList(white)[selectedChessPiece].GetID);
            if (locations == null)
            {
                locations = ProtectKing.GetListFromProtectingKingDic(ChessList.GetList(white)[selectedChessPiece].GetID);
            }
            if (locations != null)
                ChessList.GetList(white)[selectedChessPiece].SetEndLocations = locations;
            isActive = false;
            ChessList.GetList(white)[selectedChessPiece].Control();
            isActive = true;
        }

        /// <summary>
        /// If the player is the active one it will sets its <c>key</c> to <c>e.Key.key</c>.
        /// </summary>
        /// <param name="sender">The object that invoked the event.</param>
        /// <param name="e">The parameter containing the variables and their values of ControlEvents.</param>
        protected void KeyEventHandler(object sender, ControlEvents.KeyEventArgs e)
        {
            if (isActive)
                key = e.Key;
        }

    }
}
