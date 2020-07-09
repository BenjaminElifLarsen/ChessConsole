using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Chess
{
    /// <summary>
    /// The menu class. 
    /// </summary>
    class Menu
    {
        private static ConsoleKeyInfo key;
        private static bool isActive = false;

        /// <summary>
        /// Sets up the Menu for use
        /// </summary>
        public Menu()
        {
            Settings.CVTS.ActivateCVTS();
            Console.CursorVisible = false;
            Publishers.SetKeyClass();
            Publishers.PubKey.RaiseKeyEvent += KeyEventHandler;
            Publishers.SetNetClass();
            Publishers.SetCaptureClass();
            NetworkUpdateReceiver networkUpdateReceiver = new NetworkUpdateReceiver(Publishers.PubNet);
        }

        /// <summary>
        /// Runs the menu.
        /// </summary>
        public void Run()
        {
            Thread controlSystem = new Thread(Publishers.PubKey.KeyPresser);
            controlSystem.Name = "Control Thread";
            controlSystem.Start();
            MainMenu();
        }

        /// <summary>
        /// Sets the window size to the values in the Settings.WindowSize or the max size if Settings.WindowSize is big.
        /// </summary>
        private void WindowSize()
        {
            int[] windowsSize = new int[2];
            windowsSize[0] = Settings.WindowSize[0] > Console.LargestWindowWidth ? Console.LargestWindowWidth : Settings.WindowSize[0];
            windowsSize[1] = 20 > Console.LargestWindowHeight ? Console.LargestWindowHeight : 20;
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);
        }

        /// <summary>
        /// Sets the window size to the values in the width/height or the max size if Settings.WindowSize is big.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window</param>
        private void WindowSize(byte width, byte height)
        {
            int[] windowsSize = new int[2];
            windowsSize[0] = width > Console.LargestWindowWidth ? Console.LargestWindowWidth : width;
            windowsSize[1] = height > Console.LargestWindowHeight ? Console.LargestWindowHeight : height;
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);
        }

        /// <summary>
        /// The main menu. 
        /// </summary>
        private void MainMenu()
        {
            string option;
            string title = "Main Menu";
            string[] options =
            {
                "Local Play",
                "Net Play",
                "Rules",
                "Interaction",
                "Settings",
                "Exit"
            };

            do
            {
                WindowSize();
                Console.Title = Settings.GetTitle + ": Main Menu";
                Console.Clear();
                option = Interact(options, title);
                switch (option)
                {
                    case "Local Play":
                        LocalPlayMenu();
                        break;

                    case "Rules":
                        RulesMenu();
                        break;

                    case "Interaction":
                        InteractionMenu();
                        break;

                    case "Exit":
                        Environment.Exit(0);
                        break;

                    case "Net Play":
                        NetMenu();
                        break;

                    case "Settings":
                        SettingMenu();
                        break;
                }

            } while (true);
        }

        /// <summary>
        /// Settings menu. 
        /// </summary>
        private void SettingMenu()
        {
            bool run = true;
            string title = "Settings Menu";
            string[] options =
            {
                "Square Size",
                "Back"
            };
            Console.Title = Settings.GetTitle + ": " + title;
            do
            {
                string option = Interact(options, title);
                Console.Clear();
                switch (option)
                {
                    case "Square Size":
                        Settings.SetSquareSize = GetNewSize();
                        Settings.UpdateScreen();
                        break;

                    case "Back":
                        run = false;
                        break;
                }
            } while (run);

            byte GetNewSize()
            {
                byte value;
                string writtenValue;
                string[] text = String.Format("Enter a square size. Value is limited to minimum of 1 and maximum of 9. {1} Default is 5. Current value is {0}. {1} Enter to confirm value.", Settings.SquareSize, Environment.NewLine).Split(' ');
                string textOut = "";
                ushort length = 0;
                foreach (string word in text) //ensures the text does not go off the screen.
                {
                    length += (ushort)(word.Length + 1);
                    if (length < Settings.WindowSize[0] && word[0] != Environment.NewLine[0])
                        textOut += word + " ";
                    else if (word[0] != Environment.NewLine[0])
                    {
                        length = (ushort)(word.Length + 1);
                        textOut += Environment.NewLine + word + " ";
                    }
                    else if (word[0] == Environment.NewLine[0])
                    {
                        length = 0;
                        textOut += Environment.NewLine;
                    }
                }
                Console.WriteLine(textOut);
                Console.CursorVisible = true;
                int yLocation = Console.CursorTop;
                do
                {
                    writtenValue = Console.ReadLine();
                    Console.CursorTop = yLocation;
                    Console.Write("".PadLeft(Settings.WindowSize[0]));
                    Console.CursorLeft = 0;
                } while (!Byte.TryParse(writtenValue, out value));
                Console.CursorVisible = false;
                return value;
            }
        }

        /// <summary>
        /// Used for testing and practicing stuff.
        /// </summary>
        private void TestMenu()
        {

        }

        /// <summary>
        /// The net menu. Allows for people to host, join and return the main menu.
        /// </summary>
        private void NetMenu()
        {
            Console.Title = Settings.GetTitle + ": Net Menu";
            string title = "Net Menu";
            string[] options = { "Host", "Join", "Back" };
            string option;

            do
            {
                Console.Clear();
                option = Interact(options, title);

                switch (option)
                {
                    case "Host":
                        Host();
                        break;

                    case "Join":
                        Join();
                        break;

                    case "Back":
                        break;

                    case "Test":
                        Console.Clear();
                        Abort(); //testing
                        break;
                }
            } while (option != options[2]);


            void Host() //ensure good comments in all of this net code and code related to the net
            {
                //gets and displays the IP address the joiner needs
                string ipAddress = Network.NetworkSupport.LocalAddress;
                Console.Clear();
                Console.CursorTop = Settings.MenuOffset[1];
                Console.WriteLine($"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}IP Address: {Settings.CVTS.BrightCyanForeground}{ipAddress}{Settings.CVTS.Reset}");
                Console.WriteLine($"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}Waiting on player{Settings.CVTS.Reset}");

                //starts up the reciver. 
                Network.Receive.ReceiveSetup(ipAddress); //need to catch if it fails
                Network.Receive.Start();
                GameStates.NetSearch.Searching = true;

                //gives the player the options to abort searching for a player. 
                Thread abortThread = new Thread(Abort);
                abortThread.Start();

                //waits on the joiner to connect to the receiver so their IP-address is arquired to ensure data can be transmitted to their receiver.  
                //Network.Receive.GetConnectionIpAddress() loops until Network.Transmit.TransmitSetup(ipAddress) in Join() has ran or GameStates.NetSeach.Abort is true.
                string ipAddressJoiner = Network.Receive.GetConnectionIpAddress(true);

                //if not aborted, run the rest of the set-up.
                if (!GameStates.NetSearch.Abort)
                {
                    Console.Clear();

                    Network.Transmit.OtherPlayerIpAddress = ipAddressJoiner; //transmitter now got the ip address needed to contact the receiver of the joiner

                    //select colour
                    string[] colourOptions = { "White", "Black" };
                    string colourTitle = "Select your colour";
                    string colour = null;
                    colour = Interact(colourOptions, colourTitle);
                    Console.CursorTop += 1;
                    Console.WriteLine($"{Settings.CVTS.BrightWhiteForeground}Conneting{Settings.CVTS.Reset}");

                    //transmit the colour
                    bool couldSend = Network.Transmit.GeneralDataTransmission(colour, ipAddressJoiner);

                    if (couldSend) { 
                        //wait on response from the player joined to ensure they are ready.
                        string response = Network.Receive.GeneralDataReception(true, 5000);

                        //starts game, string colour parameters that decide whoes get the first turn.
                        if (response == "ready")
                        { 
                            bool firstMove = colour == "White" ? true : false;
                            GameStates.PlayerTeam = firstMove;
                            Console.WriteLine($"{Settings.CVTS.BrightWhiteForeground}Game Starting{Settings.CVTS.Reset}");
                            Start(firstMove);
                        }
                        else
                        {
                            ErrorHandling();
                        }
                    }
                }//if aborted, skip the rest
                else
                {
                    Network.Receive.Stop();
                    GameStates.Reset();
                }
            }

            void Join()
            {
                string ipAddress;
                //ensure it is a proper address.
                string ownIpAddress = Network.NetworkSupport.LocalAddress;

                //sets up and starts the receiver
                Network.Receive.ReceiveSetup(ownIpAddress);
                Network.Receive.Start();

                GameStates.NetSearch.Searching = true;
                Debug.WriteLine("Join Function");
                do
                {
                    Console.Clear();
                    Console.CursorTop = Settings.MenuOffset[1];
                    Console.Write($"" +
                        $"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}Enter {Settings.CVTS.BrightCyanForeground}host IP address{Settings.CVTS.BrightWhiteForeground}.{Environment.NewLine}" +
                        $"{"".PadLeft(Settings.MenuOffset[0])}Press {Settings.CVTS.BrightRedForeground}Enter{Settings.CVTS.BrightWhiteForeground} to comfirm.{Environment.NewLine}" +
                        $"{"".PadLeft(Settings.MenuOffset[0])}If {Settings.CVTS.BrightCyanForeground}empty{Settings.CVTS.BrightWhiteForeground}, return to menu.{Environment.NewLine}" +
                        $"{"".PadLeft(Settings.MenuOffset[0])}Address:{Settings.CVTS.Reset} ");
                    Console.CursorVisible = true;
                    ipAddress = Console.ReadLine();
                    if (ipAddress == "")
                        GameStates.NetSearch.Abort = true;
                    Network.Transmit.OtherPlayerIpAddress = ipAddress;
                    Console.CursorVisible = false;
                } while (!GameStates.NetSearch.Abort && !Network.Transmit.TransmitSetup(ipAddress, out _, true)); //starts up the transmitter to ensure the host' receiver can get the joiner' IP address and give it to host' transmitter. 
                Debug.WriteLine("TransmitSetup has run or abort");
                if (!GameStates.NetSearch.Abort)
                {
                    Debug.WriteLine("Search not aborted");
                    //function is run to allows the host' transmitter to get the ip address of the client' receiver. Port is known in the code, just the IP-address that is missing... can be better explained...
                    Console.WriteLine($"{Settings.CVTS.BrightWhiteForeground}Connecting{Settings.CVTS.Reset}");

                    //gets the colour the host is using and selects the other
                    string colourHost = Network.Receive.GeneralDataReception();
                    if(colourHost == "White" || colourHost == "Black")
                    {
                        string colour = colourHost == "White" ? "Black" : "White";

                        //send ready data.
                        bool couldSend = Network.Transmit.GeneralDataTransmission("ready", ipAddress, true);
                        
                        if (couldSend) { 
                            //starts game, the string colour parameter decides who get the first turn.
                            bool firstMove = colour == "White" ? true : false;
                            GameStates.PlayerTeam = firstMove;
                            Console.WriteLine($"{Settings.CVTS.BrightWhiteForeground}Game Starting{Settings.CVTS.Reset}");
                            Start(firstMove);
                        }
                        else
                        {
                            ErrorHandling();
                        }
                    }
                    else
                    {
                        ErrorHandling();
                    }
                }
                else
                {
                    Network.Receive.Stop();
                    GameStates.Reset();
                }

            }

            void ErrorHandling()
            {
                Network.Receive.Stop();
                GameStates.Reset();
                Console.Clear();
                Console.WriteLine("An error occured. Press {0} to go back.", Settings.SelectKey);
                isActive = true;
                while (key.Key != Settings.SelectKey) ;
                isActive = false;
                key = new ConsoleKeyInfo();

            }

            void Start(bool playerStarter)
            {
                Console.Clear();
                ChessTable chess = new ChessTable();
                chess.NetPlay(playerStarter);
            }

            void Abort()
            { //threated. Maybe not needed to be threaten, Network.Receive.GetConnectionIpAddress() should be threaten then
                GameStates.NetSearch.Searching = true; //just here for testning
                int[] cursorPosistion = new int[] { Console.CursorLeft, Console.CursorTop };
                Console.WriteLine($"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightWhiteForeground}Abort?{Settings.CVTS.Reset}");
                Console.WriteLine($"{"".PadLeft(Settings.MenuOffset[0])}{Settings.CVTS.BrightRedForeground}{Settings.CVTS.Underscore}Y{Settings.CVTS.BrightWhiteForeground}{Settings.CVTS.Underscore_Off}es{Settings.CVTS.Reset}");
                isActive = true;
                while (GameStates.NetSearch.Searching) //bool, true if searching for a player. False if a player is found. Store it in GameStates?
                {
                    if ((int)key.Key != 0)
                    {
                        //ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Y)
                        {
                            GameStates.NetSearch.Searching = false;
                            GameStates.NetSearch.Abort = true;
                        }
                    }
                }
                isActive = false;
            }

        }

        /// <summary>
        /// Writes out all text stored in the Interaction.txt file.
        /// </summary>
        private void InteractionMenu()
        {
            Console.Title = Settings.GetTitle + ": Interaction";
            try
            {
                string[] interaction = File.ReadAllLines("Interaction.txt");
                PrintOut(interaction);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e); //this block is also entered if something else goes wrong, e.g. System.NullReferenceException and index size
                Console.WriteLine("Interaction.txt could not be found.");
                Console.WriteLine("{0}{1} to return.", Environment.NewLine, Settings.SelectKey);
            }
            PrintOutMenuExit();
        }

        /// <summary>
        /// Function that is used to prevent the exiting of the PrintOut() until the Settings.SelectKey is pressed. 
        /// </summary>
        private void PrintOutMenuExit()
        {
            isActive = true;
            while (key.Key != Settings.SelectKey) ;
            isActive = false;
            key = new ConsoleKeyInfo();
        }

        /// <summary>
        /// Writes out all text stored in the Rules.txt file.
        /// </summary>
        private void RulesMenu()
        {
            Console.Title = Settings.GetTitle + ": Rules";
            try
            {
                string[] about = File.ReadAllLines("Rules.txt");
                PrintOut(about);

            }
            catch
            {
                Console.WriteLine("Rules.txt could not be found.");
                Console.WriteLine("{0}{1} to return.", Environment.NewLine, Settings.SelectKey);
            }
            PrintOutMenuExit();
        }

        /// <summary>
        /// Writes out every string in <paramref name="writeOut"/> to the console.
        /// </summary>
        /// <param name="writeOut">String array to write out to the console.</param>
        /// <param name="keywords">String array of words to be coloured differently.</param>
        /// <param name="exitwords">String array of words to be coloured differently.</param>
        /// <param name="keyColour">Can be any foreground colour, except of the extented type, from Settings.CVTS</param>
        /// <param name="exitColour">Can be any foreground colour, except of the extented type, from Settings.CVTS</param>
        private void PrintOut(string[] writeOut, string keyColour = null, string exitColour = null)
        {
            Console.Clear();

            keyColour = keyColour == null ? Settings.CVTS.BrightCyanForeground : keyColour;
            exitColour = exitColour == null ? Settings.CVTS.BrightRedForeground : exitColour;
            sbyte lineNumber = -1;
            foreach (string str in writeOut)
            {
                lineNumber++;
                byte extraLines = 0;
                string[] words = str.Split(' ');
                string newStr = "";
                int stringLength = 0;
                int wordNumber = 0;

                foreach (string word in words)
                {
                    wordNumber++;
                    bool isKeyword = false;
                    bool isExitWord = false;
                    bool isOffset = false;

                    char[] wordChar = word.ToCharArray();
                    string addToString = null;
                    bool comma = false;
                    bool dot = false;
                    bool semicolon = false;
                    bool colon = false;
                    SpecialEndSign(word);

                    if (wordChar.Length > 2)
                    {
                        bool isCapitalised = false;
                        byte endPoint = comma || dot || semicolon || colon ? (byte)2 : (byte)1;
                        if (wordChar[0] == Settings.TextOffsetChar)
                        {
                            endPoint = comma || dot || semicolon || colon ? (byte)1 : (byte)0;
                            char[] function = new char[wordChar.Length - (endPoint + 1)];
                            for (int i = 1; i < wordChar.Length - (endPoint); i++)
                                function[i - 1] = wordChar[i];
                            isCapitalised = (int)function[0] < 97 ? true : false;
                            if (!isCapitalised)
                                function[0] = (char)((int)function[0] - 32);
                            string findProperty = new string(function);
                            Type setting = typeof(Settings);
                            byte[] offset = (byte[])setting.GetProperty(findProperty).GetValue(setting);
                            addToString = "".PadLeft(offset[0]);
                            isOffset = true;
                        }
                        else if (wordChar[0] == Settings.KeywordBrackets[0] && wordChar[wordChar.Length - endPoint] == Settings.KeywordBrackets[1])
                        {
                            char[] property = new char[wordChar.Length - (1 + endPoint)];
                            for (int i = 1; i < wordChar.Length - endPoint; i++)
                            {
                                property[i - 1] = wordChar[i];
                            }
                            isCapitalised = (int)property[0] < 97 ? true : false;
                            if (!isCapitalised)
                                property[0] = (char)((int)property[0] - 32);
                            string findProperty = new string(property);
                            Type setting = typeof(Settings);
                            try
                            {

                                if (setting.GetProperty(findProperty).PropertyType.Name == "ConsoleKey")
                                    addToString = !isCapitalised ? setting.GetProperty(findProperty).GetValue(setting).ToString().ToLower() : setting.GetProperty(findProperty).GetValue(setting).ToString();

                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e);
                                addToString = "erorr";
                            }
                            isExitWord = true;
                        }
                        else if (wordChar[0] == Settings.ExitWordBrackets[0] && wordChar[wordChar.Length - endPoint] == Settings.ExitWordBrackets[1])
                        {
                            char[] keyWord = new char[wordChar.Length - (1 + endPoint)];
                            for (int i = 1; i < wordChar.Length - endPoint; i++)
                            {
                                keyWord[i - 1] = wordChar[i];
                            }
                            addToString = new string(keyWord);
                            isKeyword = true;
                        }

                    }
                    if (addToString == null)
                        addToString = word;

                    if (stringLength + addToString.Length + 1 >= Console.WindowWidth && wordNumber != words.Length)
                    {
                        stringLength = 0 + "".PadLeft(Settings.TextOffset[0]).Length;
                        newStr += Environment.NewLine + "".PadLeft(Settings.TextOffset[0]);
                        lineNumber++;
                        extraLines++;
                    }

                    if (!isKeyword && !isExitWord)
                        newStr += Settings.CVTS.BrightWhiteForeground + addToString + Settings.CVTS.Reset;
                    else if (isKeyword)
                        newStr += keyColour + addToString + Settings.CVTS.Reset;
                    else if (isExitWord)
                        newStr += exitColour + addToString + Settings.CVTS.Reset;

                    if (dot && (isKeyword || isExitWord))
                    {
                        newStr += ".";
                        stringLength++;
                    }
                    else if (comma && (isKeyword || isExitWord))
                    {
                        newStr += ",";
                        stringLength++;
                    }
                    else if (semicolon && (isKeyword || isExitWord))
                    {
                        newStr += ";";
                        stringLength++;
                    }
                    else if (colon && (isKeyword || isExitWord))
                    {
                        newStr += ":";
                        stringLength++;
                    }

                    if (!isOffset)
                    {
                        newStr += " ";
                        stringLength += addToString.Length + 1;
                    }

                    void SpecialEndSign(string wordToCheck)
                    {

                        comma = wordToCheck.EndsWith(',');
                        dot = wordToCheck.EndsWith('.');
                        semicolon = wordToCheck.EndsWith(';');
                        colon = wordToCheck.EndsWith(':');

                    }
                }
                Console.CursorTop = lineNumber + Settings.MenuTitleOffset[1] - extraLines;
                Console.CursorLeft = 0;
                //https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/reflection
                //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection
                Console.Write($"{newStr}");


            }
            Console.SetCursorPosition(0, 0);
            if (lineNumber + 1 > Console.WindowHeight)
                WindowSize((byte)Console.WindowWidth, (byte)(lineNumber + 1));
        }

        /// <summary>
        /// The local play menu.
        /// </summary>
        private void LocalPlayMenu()
        {
            Console.Clear();
            ChessTable chess = new ChessTable();
            chess.LocalPlay();
        }

        /// <summary>
        /// Allows other classes to use to menu interaction and displaying. 
        /// </summary>
        /// <param name="options">String array of options.</param>
        /// <returns>Returns the selected option.</returns>
        public static string MenuAccess(string[] options, string title = null)
        {
            return Interact(options, title);
        }

        /// <summary>
        /// Allows for the select of an index in <paramref name="options"/>. This function will also call the <c>Display</c> function.
        /// </summary>
        /// <param name="options">String array of options.</param>
        /// <returns>Returns the selected option.</returns>
        private static string Interact(string[] options, string title = null)
        { 
            Debug.WriteLine("Is active: " + GameStates.IsInMenu);
            Debug.WriteLine(title + ": Pre while");
            while (GameStates.IsInMenu) ;
            Debug.WriteLine(title + ": Post while");
            GameStates.IsInMenu = true;
            Debug.WriteLine("Is active: " + GameStates.IsInMenu);
            bool selected = false;
            byte currentLocation = 0;
            string answer = null;
            isActive = true;
            Console.Clear();
            do
            {
                Display(options, currentLocation, Settings.MenuColour, Settings.MenuColourHovered, Settings.MenuOffset, title, Settings.MenuColourTitle, Settings.MenuTitleOffset);
                Console.CursorTop = 0;
                Console.CursorLeft = 0;
                if (Move())
                {
                    answer = options[currentLocation];
                    selected = true;
                }
                key = new ConsoleKeyInfo();
            } while (!selected);
            isActive = false;
            GameStates.IsInMenu = false;
            Debug.WriteLine(title + ": Closing");
            return answer;

            bool Move()
            {
                while ((int)key.Key == 0) ;
                if (key.Key == Settings.UpKey && currentLocation > 0)
                {
                    currentLocation--;
                }
                else if (key.Key == Settings.DownKey && currentLocation < options.Length - 1)
                {
                    currentLocation++;
                }
                else if (key.Key == Settings.SelectKey)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Displays the <paramref name="options"/> in the console in the colours given by <paramref name="optionColours"/> and <paramref name="hoveredColour"/>. 
        /// </summary>
        /// <param name="options">String list. Each entry is considered an option.</param>
        /// <param name="hoveredOverOption">Indicate which option is the currently hovered over.</param>
        /// <param name="optionColours">The default colour of the options.</param>
        /// <param name="hoveredColour">The colour of the hovered over option.</param>
        /// <param name="offset">Offset to the top left corner.</param>
        private static void Display(string[] options, byte hoveredOverOption, byte[] optionColours, byte[] hoveredColour, byte[] offset, string title = null, byte[] titleColour = null, byte[] titleOffset = null)
        {
            byte yOffSetSupport = 0;
            if (title != null)
            {
                if (titleOffset == null)
                {
                    yOffSetSupport++;
                    Console.SetCursorPosition(offset[0], offset[1]);
                }
                else
                    Console.SetCursorPosition(titleOffset[0], titleOffset[1]);
                if (titleColour == null)
                    Paint(title, optionColours);
                else
                    Paint(title, titleColour);
            }

            for (int i = 0; i < options.Length; i++)
            {
                Console.SetCursorPosition(offset[0], offset[1] + i + yOffSetSupport);
                if (i == hoveredOverOption)
                {
                    Paint(options[i], hoveredColour);
                }
                else
                {
                    Paint(options[i], optionColours);
                }
            }

            void Paint(string option, byte[] colour)
            {
                Console.Write(Settings.CVTS.ExtendedForegroundColour_RGB + colour[0] + ";" + colour[1] + ";" + colour[2] + "m{0}" + Settings.CVTS.Reset, option);
            }
        }

        /// <summary>
        /// If the menu is the active one it will sets its <c>key</c> to <c>e.Key.key</c>.
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
