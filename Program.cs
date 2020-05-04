using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Chess
{   //https://www.chessvariants.com/d.chess/chess.html
    //team == false, player = black, else player = white. White top, black bottom
    /// <summary>
    /// Class that contains a 2D array that is used to keep the location of all chess pieces on the board.
    /// </summary>
    public class MapMatrix
    {
        private static string[,] map = new string[8, 8];
        private static bool mapPrepared = false;
        private MapMatrix()
        {

        }

        /// <summary>
        /// Prepares the map for usages.
        /// </summary>
        public static void PrepareMap()
        {
            if (mapPrepared == false)
                for (int n = 0; n < 8; n++)
                    for (int m = 0; m < 8; m++)
                        map[n, m] = "";
            mapPrepared = true;
        }

        /// <summary>
        /// Used to get and set values on the board.
        /// </summary>
        public static string[,] Map { get => map; set => map = value; }
    }

    /// <summary>
    /// Class that contains two lists, one of white chess pieces and one of black chess pieces.
    /// </summary>
    public class ChessList
    {
        private ChessList() { }
        private static List<ChessPiece> chessListBlack = new List<ChessPiece>();
        private static List<ChessPiece> chessListWhite = new List<ChessPiece>();
        //public static void SetChessListBlack(List<ChessPiece> list)
        //{
        //    chessListBlack = list;
        //}
        /// <summary>
        /// Sets the chess pieces.
        /// </summary>
        /// <param name="list">The list containing the chess pieces.</param>
        /// <param name="team">The team that <paramref name="list"/> belongs too.</param>
        public static void SetChessList(List<ChessPiece> list, bool team)
        {
            if (team)
                chessListWhite = list;
            else
                chessListBlack = list;
        }
        /// <summary>
        /// Returns a list depending on <paramref name="team"/>.
        /// </summary>
        /// <param name="team">True for white, false for black.</param>
        /// <returns>Returns a list of either black or white chess pieces depending on <paramref name="team"/>.</returns>
        public static List<ChessPiece> GetList(bool team)
        {
            return team == false ? chessListBlack : chessListWhite;
        }
    }

    /// <summary>
    /// Class that contain information about pieces that can protect the king, if the king is checked. 
    /// </summary>
    public class ProtectKing
    {
        private ProtectKing() { }
        private static List<string> chessListProtectKing = new List<string>();
        private static Dictionary<string, List<int[,]>> chessPiecesAndEndLocations = new Dictionary<string, List<int[,]>>();
        /// <summary>
        /// List of all pieces that can protect the king, if the king is checked. If the king can move, it is also in the list.
        /// </summary>
        public static List<string> Protect { get => chessListProtectKing; set => chessListProtectKing = value; }
        /// <summary>
        /// Dictionary containing all the pieces that can prevent the king from being checked and the locations they can move too to prevent the check. If the king can move, it will also be in this list, but its value will be null.
        /// The IDs are the keys.
        /// </summary>
        public static Dictionary<string,List<int[,]>> ProtectEndLocations { get => chessPiecesAndEndLocations; set => chessPiecesAndEndLocations = value; }
        /// <summary>
        /// Will return a list of endlocations for a specific ID. If the ID is not a key, it will return null.
        /// </summary>
        /// <param name="chesspiece">The ID of the chesspiece.</param>
        /// <returns></returns>
        public static List<int[,]> GetListFromDic(string chesspiece)
        {
            try
            {
                return chessPiecesAndEndLocations[chesspiece];
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Class that contains the settings used in the chess game. 
    /// </summary>
    public class Settings
    { //consider having settings for write locations related to the king check. Also what should be written should be location, perhaps, e.g. e5. So convert the first number of each location to a letter.
        private Settings() { }
        private static byte squareSize = 5;
        private static byte[] lineColour = new byte[] { 122, 122, 122 };
        private static byte[] lineColourBase = new byte[] { 87, 65, 47 };
        private static byte[] squareColour1 = new byte[] { 182, 123, 91 };
        private static byte[] squareColour2 = new byte[] { 135, 68, 31 };
        private static byte[] hoverOverSquareColour = new byte[] { 193, 76, 29 };
        private static byte[] chessPieceHoverOverSquareColour = new byte[] { 34, 124, 66 };
        private static byte[] chessPieceHoverOver = new byte[] { 31, 135, 113 };
        private static byte[] menuColour = new byte[] { 0, 255, 0 };
        private static byte[] menuColourHovered = new byte[] { 255, 0, 0 };
        private static byte[] offset = new byte[] { 4, 2 }; //works as it should
        private static byte[] menuOffset = new byte[] { 2, 2 };
        private static char lineX = '-'; //works as it should
        private static char lineY = '|'; //works as it should
        private static byte extraSpacing = 1; //if changes, numbers and letters do not move down, edges moves the correct amount and the squares moves to very much wrong locations
        private static byte edgeSize = (byte)(extraSpacing + 1); //does not affect top and left side numbers and letters in the correct way
        private static byte[] windowSizeModifer = new byte[] { 20, 4 }; //not a setting that should be access too.
        private static int[] windowSize = new int[] { squareSize * 8 + 9 + 2 * edgeSize + offset[0] * 2 + windowSizeModifer[0], squareSize * 8 + 9 + 2 * edgeSize + offset[1] * 2 + windowSizeModifer[1] };
        private static int[,] writeLocationCheckHeader = new int[,] { { windowSize[0] - windowSizeModifer[0], 10 }, { windowSize[0] - windowSizeModifer[0] + 8, 10 } };
        private static int[,] writeLocationCheck = new int[,] { { writeLocationCheckHeader[0, 0], writeLocationCheckHeader[0, 1] + 2 }, { writeLocationCheckHeader[1, 0], writeLocationCheckHeader[1, 1] + 2 } }; //x,y //each line should contain two symbols, e.g. D5, A2 etc..
        //Black    White
        //king     king
        //----     ----
        //D5       A6
        //         D4
        //need to deal with nulls in the places that calls the different settings. 
        private static int[] writeLocationPromotion = new int[] { offset[0] + edgeSize + 2, windowSize[1] - windowSizeModifer[1] };
        /// <summary>
        /// Gets the size of the squares. 
        /// </summary>
        public static byte SquareSize { get => squareSize; }
        /// <summary>
        /// ???
        /// </summary>
        public static byte[] LineColour { get => lineColour; }
        /// <summary>
        /// ???
        /// </summary>
        public static byte[] LineColourBase { get => lineColourBase; }
        /// <summary>
        /// Gets the first square board colour.
        /// </summary>
        public static byte[] SquareColour1 { get => squareColour1; }
        /// <summary>
        /// Gets the second square board colour.
        /// </summary>
        public static byte[] SquareColour2 { get => squareColour2; }
        /// <summary>
        /// Gets the colour used to display the possible end locations a chess piece can move too.
        /// </summary>
        public static byte[] SelectSquareColour { get => hoverOverSquareColour; }
        /// <summary>
        /// Gets the colour used to display the hovered over square for a chess piece move. 
        /// </summary>
        public static byte[] SelectMoveSquareColour { get => chessPieceHoverOverSquareColour; }
        /// <summary>
        /// Gets the colour used to display a chess piece if it is hovered over.
        /// </summary>
        public static byte[] SelectPieceColour { get => chessPieceHoverOver; }
        /// <summary>
        /// Gets the offset from the top left corner to the top left part of the board.
        /// </summary>
        public static byte[] Offset { get => offset; }
        /// <summary>
        /// Gets the char used for the x line on the board.
        /// </summary>
        public static char GetLineX { get => lineX; }
        /// <summary>
        /// Gets the char used for the y line on the board.
        /// </summary>
        public static char GetLineY { get => lineY; }
        /// <summary>
        /// Get the spacing...
        /// </summary>
        public static byte Spacing { get => extraSpacing; } //not all paint functions are used this one properly. 
        /// <summary>
        /// Get the edge size...
        /// </summary>
        public static byte EdgeSpacing { get => edgeSize; }
        /// <summary>
        /// Gets the size of the window.
        /// </summary>
        public static int[] WindowSize { get => windowSize; } //consider having two settings for player write locations
        /// <summary>
        /// Gets the locations to write the check header too.
        /// </summary>
        public static int[,] CheckHeaderLocation { get => writeLocationCheckHeader; }
        /// <summary>
        /// Gets the locations to write check out too. 
        /// </summary>
        public static int[,] CheckWriteLocation { get => writeLocationCheck; }
        /// <summary>
        /// Gets the location to write out the promotions. 
        /// </summary>
        public static int[] PromotionWriteLocation { get => writeLocationPromotion; }

        /// <summary>
        /// The colour of the menu options.
        /// </summary>
        public static byte[] MenuColour { get => menuColour; }
        /// <summary>
        /// The colour of the hovered over menu option.
        /// </summary>
        public static byte[] MenuColourHovered { get => menuColourHovered; }
        /// <summary>
        /// The offset of the menu. 
        /// </summary>
        public static byte[] MenuOffset { get => menuOffset; }


    }

    class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            menu.Run();
        }
    } //for the network testing, transmit and receive on two different ports. Consider if it is better to do this in the final version or better to transmit and receive on the same port

    class Network
    {
        //for the transmission and reception of the map array. You could write it into to a string, encode it and trasmit it. Then decode it and, knowing the size of the array, write it back into the map.
        //given you always know when the program should trasmit and recieve data, transmit after ones play and recieve until it receives data, is it not needed to multitread this part of the program?
        //in the menu class, the network menu will call another play function that is designed for network.
        /* https://docs.microsoft.com/en-us/dotnet/framework/network-programming/?redirectedfrom=MSDN
         * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/network-programming-samples
         * https://www.codeproject.com/Articles/1415/Introduction-to-TCP-client-server-in-C
         * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/how-to-create-a-socket
         * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/using-client-sockets
         * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/listening-with-sockets
         * https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.bind?view=netcore-3.1
         * https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.listen?view=netcore-3.1
         * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/introducing-pluggable-protocols
         * https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?redirectedfrom=MSDN&view=netcore-3.1
         * https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient.connect?view=netcore-3.1#System_Net_Sockets_TcpClient_Connect_System_String_System_Int32_
         * https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=netcore-3.1
         * http://csharp.net-informations.com/communications/csharp-chat-client.htm
         * https://stackoverflow.com/questions/7508942/loopback-localhost-question
         * https://stackoverflow.com/questions/12952679/socket-programming-in-c-sharp-the-client-never-connects-to-the-server
         * https://stackoverflow.com/questions/15162312/tcpclient-not-connecting-to-local-computer
         * https://stackoverflow.com/questions/37308169/c-sharp-tcp-socket-client-refuses-connection
         * http://csharp.net-informations.com/communications/csharp-socket-programming.htm
         * https://www.geeksforgeeks.org/socket-programming-in-c-sharp/
         * https://www.codeproject.com/Articles/10649/An-Introduction-to-Socket-Programming-in-NET-using
         * https://www.c-sharpcorner.com/article/socket-programming-in-C-Sharp/
         */
        static TcpClient transmitter = null;
        static TcpListener receiver = null;
        //is it a good idea to have them as two classes? 
        /* you will need to be able to call functions to transmit and receive at times from the ChessTable class. 
         * Wether it should transmit after whites or blacks control and recevice before that colour depends on the team.  
         */
        public Network() 
        {
            //Receive reTest = new Receive();
            //Transmit trTest = new Transmit();
            Receive.ReceiveSetup();
            Transmit.TransmitSetup();
        }

        public class Transmit
        {
            public Transmit()
            {
                //TcpClient transmitter = null; //for now just use the 127.0.0.1 
                Int32 port = 23000;
                IPAddress transmitterAddress = IPAddress.Parse("127.0.0.1");
                //transmitter = new TcpClient(transmitterAddress.ToString(), port);
                transmitter = new TcpClient("localhost", port);
                //hmmm... cannot connect, the computer denies connection... Can't ping this computer either... If a way around is not found, just write as much code as possible and boot up the laptop, at least it can be pinged. 
                //got a lot to read and study anyway.
                //need a try catch 
            }

            public static void TransmitSetup()
            { //might need a try catch
                Int32 port = 23000;
                IPAddress transmitterAddress = IPAddress.Parse("127.0.0.1");
                transmitter = new TcpClient("localhost", port);
            }

            public static void TransmitData()
            { //might need a try catch
                //should it only allow IPv4 or also IPv6?
                try //is it better to use Socket or TcpListiner/TcpClient?
                { // https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.networkstream?view=netcore-3.1
                    //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket?view=netcore-3.1
                    //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.sockettype?view=netcore-3.1#System_Net_Sockets_SocketType_Stream
                    //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/using-an-asynchronous-client-socket?view=netcore-3.1
                    //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/sockets?view=netcore-3.1
                    byte[] receptionAnswerByte = new byte[1];
                    byte receptionAnswer; 
                    TransmitSetup();
                    string mapData = NetworkSupport.MapToStringConvertion();
                    //should transmit a signal to the receiver and wait on an answer. If it does not receive an answer, do what? 
                    //transmit map data.
                    //data should be sent back to indicate that the data has been received. 
                    byte[] mapdataByte = System.Text.Encoding.ASCII.GetBytes(mapData); //could use your own converter
                    byte[] mapdataByteSize = null;
                    Converter.Conversion.ValueToBitArrayQuick(mapdataByte.Length, out mapdataByteSize); //use your own converter. For some reason the xml it not showing... fixed
                    //open transmission
                    NetworkStream networkStream = transmitter.GetStream();

                    //transmit the size of the array first, so the receiver knows how much data is coming.
                    networkStream.Write(mapdataByteSize, 0, mapdataByteSize.Length);

                    //wait on answer from the receiver
                    //what should the response be? That is type. String? Interger, e.g. 0 for no error? What to do in case of an error
                    networkStream.Read(receptionAnswerByte, 0, 1); //depending on the answer, different things should be done, e.g. retransmission of data, nothing etc.
                    receptionAnswer = receptionAnswerByte[0];

                    //transmit the mapdataByte
                    networkStream.Write(mapdataByte, 0, mapdataByte.Length);

                    //wait on answer from the receiver. These might need a loop. Not fully sure if NetworkStream.Read() waits until it reads something or not. Find more information about (network)stream and how it works. What does it look like in the background
                    networkStream.Read(receptionAnswerByte, 0, 1); //penty of exceptions to deal with regarding .Write() and .Read()
                    //it does not seem like there is a function that allows to control whether a networkstream can write or not, so the .CanWrite() will be needed most likely. Try and find out what determine if it possible to write to a stream or not. 
                    //same for .CanRead()
                    //After the CanRead() page: "Provide the appropriate FileAccess enumerated value in the constructor to set the readability and writability of the NetworkStream.", so most likely will not have to worry about it. Can be useful for other projects
                    receptionAnswer = receptionAnswerByte[0];

                    //shut down transmission
                    networkStream.Close();
                    transmitter.Close();

                }
                catch
                {

                }

            }

        }

        public class Receive
        {
            public Receive()
            { //try catch
                //
                //TcpListener receiver = null; //for now just use the 127.0.0.1 
                Int32 port = 23000;
                IPAddress receiverAddress = IPAddress.Parse("127.0.0.1");
                receiver = new TcpListener(receiverAddress, port);
            }

            public static void ReceiveSetup()
            { //try catch
                Int32 port = 23000;
                IPAddress receiverAddress = IPAddress.Parse("127.0.0.1");
                receiver = new TcpListener(receiverAddress, port);
            }

            public static void ReceiveData(bool team)
            { //try catch
                try //Maybe thread this function to ensure that there it is always ready to recieve, and transmit response, data. Else a player might try to transmit data before the receiver is ready. 
                {
                    bool otherPlayersTurn = true;
                    TcpClient otherPlayer;
                    receiver.Start();
                    //string mapdataString;
                    while (otherPlayersTurn) //find a bool for condition
                    {
                        byte[] tranmissionAnswerByte = new byte[1];
                        otherPlayer = receiver.AcceptTcpClient();
                        NetworkStream networkStream = otherPlayer.GetStream();
                        //how to know how much data to receive? And transmit for that sake? 
                        //Maybe calculate it from the mapstring, done in the transmitter, transmit that size data to the receiver and when it acknowledge it got it, transmit the data itself
                        byte[] dataSizeByte = new byte[4];
                        //use the converter lib. you wrote.
                        ushort dataSize = 0; //use the converter here

                        //receive the size of the mapdata string
                        networkStream.Read(dataSizeByte, 0, dataSizeByte.Length);
                        Converter.Conversion.ByteConverterToInterger(dataSizeByte, ref dataSize); //figure out how to solve this conflict. Seems like it is fixed.
                        byte[] data = new byte[dataSize];

                        //transmit an answer depending if it received data
                        Converter.Conversion.ValueToBitArrayQuick(0, out tranmissionAnswerByte);
                        networkStream.Write(tranmissionAnswerByte, 0, tranmissionAnswerByte.Length);

                        //receive the map data
                        networkStream.Read(data, 0, dataSize); //how to ensure the data is correct? Since it is using TCP, does it matter? 

                        //transmit an answer depending on the received data
                        Converter.Conversion.ValueToBitArrayQuick(0, out tranmissionAnswerByte);
                        networkStream.Write(tranmissionAnswerByte, 0, tranmissionAnswerByte.Length);

                        //shutdown connection to client. 
                        networkStream.Close();
                        otherPlayer.Close();

                        //decode data and update the chess pieces and the map
                        string mapdataString = System.Text.Encoding.ASCII.GetString(data);
                        string[,] newMap = NetworkSupport.StringToMapConvertion(mapdataString);
                        NetworkSupport.UpdatedChessPieces(newMap, team);
                        NetworkSupport.UpdateMap(newMap);
                        otherPlayersTurn = false; //change later.
                    }

                    receiver.Stop();
                }
                catch
                {

                }

            }


        }

        static class NetworkSupport
        {
            public static string MapToStringConvertion()
            {
                string mapString = "";
                for (int x = 0; x < MapMatrix.Map.GetLength(0); x++)
                {
                    for (int y = 0; y < MapMatrix.Map.GetLength(1); y++)
                    {
                        mapString += MapMatrix.Map[x, y] + "!"; //the ! is to help seperate the string into a 1d array.
                    }
                }
                return mapString;
            }

            public static string[,] StringToMapConvertion(string data)
            { //Should not overwrite the map. It should just return an array and then somewhere in the code that calls it or another function recreate the board after this array. 
                //need a way to figure out if chess piece specific code has been activated/change, e.g. hasMoved, specialbools etc.
                //maybe have that chesspiece reaction code in this class. Maybe have a network function in all chesspieces that allows to set a new maplocation and call the (remove)draw. 
                //so instead of recreating all chesspieces, find that one spot on the maps that differ. If the change is from ID to "" or otherway find that piece and move it. If a ID is replace with an ID from the other team...
                //call the taken pieces been taken fucntion and then move the other piece.
                //remember for castling, two pieces will have moved, and en passant. 
                //for pawns, just check if if the piece has moved one or two squares.
                string[] mapStringSeperated = data.Split('!'); //how well does it work regarding to the ""s. E.g. "+..."!""!""!"-..:"!. Will this give "+..." "-..." or "+..." " " " " "-..." or "+..." "" "" "-..."? Add the non-ID filled square setting to Settings
                string[,] newMap = new string[8, 8];
                byte y = 0;
                for (int n = 0; n < mapStringSeperated.Length; n++)
                {
                    byte x = (byte)Math.Floor(n / 8d);
                    y = y < 8 ? (byte)0 : y++;
                    newMap[x, y] = mapStringSeperated[n]; 
                }
                return newMap;
            }

            public static void UpdatedChessPieces(string[,] map, bool team)
            {
                for(int x = 0; x < 8; x++) 
                    for(int y = 0; y < 8; y++)
                    {
                        if(map[x,y] != MapMatrix.Map[x, y])
                        {
                            string feltIDNew = map[x, y];
                            string feltIDOld = MapMatrix.Map[x, y];
                            if(feltIDNew != "" && feltIDOld == "")
                            {//moved
                                //look for the location of the feltIDOld on the variable map
                                //need to check if there is a pawn that is missing, i.e. en passant, above or below the current location. 
                                if(feltIDNew.Split(':')[1] == "6")
                                {
                                    foreach (ChessPiece chePie in ChessList.GetList(!team)) //find the other player's piece that has moved.
                                    {
                                        if (chePie.GetID == feltIDNew)
                                        {
                                            if(Math.Abs(y-chePie.GetMapLocation[1]) == 2) //moved 2, i.e. double moved
                                            {
                                                chePie.Network(new int[] { x, y });
                                                
                                            }
                                            else //moved 1
                                            {
                                                //need to check if it did an en passant or not.
                                                if(chePie.GetMapLocation[0] != x)
                                                {
                                                    //find out the move direction and check the y in the other direction. 

                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (ChessPiece chePie in ChessList.GetList(!team)) //find the other player's piece that has moved.
                                    {
                                        if (chePie.GetID == feltIDNew)
                                        {
                                            chePie.Network(new int[] { x, y });
                                            break;
                                        }
                                    }
                                }
                            }
                            else if(feltIDOld != "" && feltIDNew != "" && feltIDOld != feltIDNew)
                            {//a piece has been taken and a new stand on it. //does not deal with the en passant
                                foreach (ChessPiece chePie in ChessList.GetList(team)) //find the player's piece that has been taken.
                                {
                                    if(chePie.GetID == feltIDOld)
                                    {
                                        chePie.Network(captured: true);
                                        break;
                                    }
                                }
                                foreach (ChessPiece chePie in ChessList.GetList(!team)) //find the other player's piece that has moved.
                                {
                                    if(chePie.GetID == feltIDNew)
                                    {
                                        chePie.Network(new int[] { x, y });
                                        break;
                                    }
                                }
                            }
                            //else if (feltIDNew == "" && feltIDOld != "")
                            //{//should be able to do deal with en passant. Just need to get the direction, depending on the player, look at the above or below square for a pawn ID and then update its location. 

                            //}
                        }
                    }
            }

            public static void UpdateMap(string[,] map)
            {
                MapMatrix.Map = map;
            }

        }

    }


    /// <summary>
    /// The menu class. 
    /// </summary>
    class Menu
    {

        public Menu()
        {
            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);
            Console.CursorVisible = false;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        /// <summary>
        /// Runs the menu.
        /// </summary>
        public void Run()
        {
            MainMenu();
        }

        /// <summary>
        /// The main menu. 
        /// </summary>
        private void MainMenu()
        {
            string option;
            string[] options =
            {
                "Local Play",
                "Net Play",
                "Rules",
                "Exit"
            };

            do
            {
                Console.Clear();
                option = Interact(options);

                switch(option)
                {
                    case "Local Play":
                        LocalPlayMenu();
                        break;

                    case "Rules":
                        RulesMenu();
                        break;

                    case "Exit":
                        Environment.Exit(0);
                        break;

                    case "Net Play":
                        Network Nettest = new Network();
                        break;

                }

            } while (true);
        }

        /// <summary>
        /// Writes out all text stored in the Rules.txt file.
        /// </summary>
        private void RulesMenu()
        {
            Console.Clear();
            //make it read from a text file at some point.

            try
            {
                string[] about = File.ReadAllLines("Rules.txt");
                foreach (string str in about)
                { //if a line is longer than the console is wide, the program should split the line into minor pieces and display each of them.
                    Console.WriteLine(str);
                }
            }
            catch
            {
                Console.WriteLine("Rules.txt could not be found.");
            }
            Console.WriteLine("\nEnter to return.");
            Console.ReadLine();


        }

        /// <summary>
        /// The local play menu.
        /// </summary>
        private void LocalPlayMenu()
        {
            Console.Clear();
            ChessTable chess = new ChessTable();
            chess.Play();
        }

        /// <summary>
        /// Allows for the select of an index in <paramref name="options"/>. This function will also call the <c>Display</c> function.
        /// </summary>
        /// <param name="options">String array of options.</param>
        /// <returns>Returns the selected option.</returns>
        private string Interact(string[] options)
        { //used to move around in the displayed options. All it should do is being a function that checks if up/down key arrows are pressed and then 
            //increase or decrease a variable used for the hoveredOverOption in Display().
            bool selected = false;
            byte currentLocation = 0;
            string answer = null;

            do
            {
                Display(options, currentLocation, Settings.MenuColour, Settings.MenuColourHovered, Settings.MenuOffset);
                if (Move())
                {
                    answer = options[currentLocation];
                    selected = true;
                }
            } while (!selected);

            return answer;

            bool Move()
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.UpArrow && currentLocation > 0)
                {
                    currentLocation--;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow && currentLocation < options.Length)
                {
                    currentLocation++;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
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
        private void Display(string[] options, byte hoveredOverOption, byte[] optionColours, byte[] hoveredColour, byte[] offset)
        { //write a new one instead of recycling the old code. The hoveredOverOption is simply the index of the option in options. 
            //start simple, make it more complex when the simple versions, Display and Interact, are working without any problems.  
            for(int i = 0; i < options.Length; i++)
            {
                Console.SetCursorPosition(offset[0], offset[1] + i);
                if(i == hoveredOverOption)
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
                Console.Write("\x1b[38;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m{0}", option);
            }
        }

    }


    class ChessTable
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        private Player white; //white top
        private Player black; //black bottom
        private int[,] whiteSpawnLocation; //since the map matrix is being used, no real reason for having these outside their player class
        private int[,] blackSpawnLocation;
        private byte squareSize;
        private byte[] lineColour;
        private byte[] lineColourBase;
        private byte[] squareColour1;
        private byte[] squareColour2;
        private byte[] offset;
        private int[] windowsSize = new int[2];
        private bool[,] isCheckedTwiceInARow = new bool[,] { { false, false }, { false, false } }; //[0][] is white. [1][0] is black. 
        private string[,] threefoldRepetition = new string[2,3];
        private string[,] lastUpdateMap; //find a better name

        public ChessTable()
        {
            MapMatrix.PrepareMap();
            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);

            squareSize = 5;
            lineColour = new byte[] { 122, 122, 122 };
            lineColourBase = new byte[] { 87, 65, 47 };
            squareColour1 = new byte[] { 182, 123, 91 };
            squareColour2 = new byte[] { 135, 68, 31 };
            offset = new byte[] { 2, 2 };

            windowsSize[0] = Settings.WindowSize[0];
            windowsSize[1] = Settings.WindowSize[1];
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);
            blackSpawnLocation = new int[,] {
                { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 },
                { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }
            };
            whiteSpawnLocation = new int[,] {
                { 0, 6 }, { 1, 6 }, { 2, 6 }, { 3, 6 }, { 4, 6 }, { 5, 6 }, { 6, 6 }, { 7, 6 },
                { 0, 7 }, { 1, 7 }, { 2, 7 }, { 3, 7 }, { 4, 7 }, { 5, 7 }, { 6, 7 }, { 7, 7 }
            };
            BoardSetup();
            byte[] colourWhite =
            {
                255,255,255
            };
            byte[] colourBlack =
            {
                0,0,0
            };
            CreatePieces(true, whiteSpawnLocation, colourWhite);
            CreatePieces(false, blackSpawnLocation, colourBlack);

            PlayerSetup();
        }

        /// <summary>
        /// Sets up the board and paint the outlines. 
        /// </summary>
        private void BoardSetup()
        {//8 squares in each direction. Each piece is 3*3 currently, each square is 5*5 currently. 
            Console.CursorVisible = false;
            ushort distance = (ushort)(9 + 8 * squareSize);
            string[] letters = { "a", "b", "c", "d", "e", "f", "g", "h" };
            string[] numbers = { "1", "2", "3", "4", "5", "6", "7", "8" };
            byte alignment = (byte)Math.Ceiling(Settings.SquareSize / 2f);
            for (int k = 0; k < distance; k++)
                for (int i = 0; i < distance; i += 1 + squareSize)
                {
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing, k + Settings.Offset[1] + Settings.EdgeSpacing);
                    Console.Write("\x1b[48;2;" + lineColourBase[0] + ";" + lineColourBase[1] + ";" + lineColourBase[2] + "m ");
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing, k + Settings.Offset[1] + Settings.EdgeSpacing);
                    Console.Write("\x1b[38;2;" + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + "\x1b[0m", Settings.GetLineY);
                }
            for (int k = 0; k < distance; k += 1 + squareSize)
                for (int i = 1; i < distance - 1; i++)
                {
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing, k + Settings.Offset[1] + Settings.EdgeSpacing);
                    Console.Write("\x1b[48;2;" + lineColourBase[0] + ";" + lineColourBase[1] + ";" + lineColourBase[2] + "m ");
                    Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing, k + Settings.Offset[1] + Settings.EdgeSpacing);
                    Console.Write("\x1b[38;2;" + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m{0}" + "\x1b[0m", Settings.GetLineX);
                }

            for (int k = 0; k < numbers.Length; k++)
            {
                Console.SetCursorPosition(Settings.Offset[0], k + Settings.EdgeSpacing + Settings.Offset[1] + alignment + (Settings.SquareSize * k));
                Console.Write(numbers[k]);
                Console.SetCursorPosition(Settings.Offset[0] + distance + Settings.EdgeSpacing + Settings.Spacing, k + Settings.EdgeSpacing + Settings.Offset[1] + alignment + (Settings.SquareSize * k));
                Console.Write(numbers[k]);
            }

            for (int k = 0; k < letters.Length; k++)
            {
                Console.SetCursorPosition(k + Settings.EdgeSpacing + Settings.Offset[0] + alignment + (Settings.SquareSize * k), Settings.Offset[1]);
                Console.Write(letters[k]);
                Console.SetCursorPosition(k + Settings.EdgeSpacing + Settings.Offset[0] + alignment + (Settings.SquareSize * k), Settings.Offset[1] + distance + Settings.Spacing + Settings.EdgeSpacing);
                Console.Write(letters[k]);
            }

            BoardColouring();
            KingWriteLocationSetup();
        }

        /// <summary>
        /// Colour the board. 
        /// </summary>
        private void BoardColouring()
        {
            ushort distance = (ushort)(8 + 8 * squareSize);
            byte location = 1;
            for (int n = 0; n < distance; n += 1 + squareSize)
            {
                for (int m = 0; m < distance; m += 1 + squareSize)
                {
                    for (int i = 0; i < squareSize; i++)
                    {
                        for (int k = 0; k < squareSize; k++)
                        {
                            Console.SetCursorPosition(i + Settings.Offset[0] + Settings.EdgeSpacing + Settings.Spacing + n, k + Settings.Offset[1] + Settings.Spacing + Settings.EdgeSpacing + m);
                            if (location % 2 == 1)
                                Console.Write("\x1b[48;2;" + squareColour1[0] + ";" + squareColour1[1] + ";" + squareColour1[2] + "m " + "\x1b[0m");
                            else if (location % 2 == 0)
                                Console.Write("\x1b[48;2;" + squareColour2[0] + ";" + squareColour2[1] + ";" + squareColour2[2] + "m " + "\x1b[0m");
                        }
                    }
                    location++;
                }
                location++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void KingWriteLocationSetup()
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
        /// Function that checks if the game has reached a draw.
        /// </summary>
        /// <returns>Returns true if the game is in a draw, else false.</returns>
        private bool Draw()
        { //maybe have an 2d string array with a size 2*3. Each time a piece is moved, the last index is overwritten by the second last index and second last index overwritten by the first.
            /* Then the first index is overwritten by the new value.  
             * The value should be a combination of the ID and the location. E.g. +:2:1-45. If at any moment the entire coloum is the same, the game id draw... or is it just lost for that player? 
             * 
             * Also need to check if no pawn or capture has happen in the last 50 turns. Also how to check for a stalemate, that is not enough pices to check the king. 
             * Maybe also have a draw/stalemate function the in the player plus the surrender function.
             */
            if (ChessList.GetList(true).Count == 1 && ChessList.GetList(false).Count == 1)
            {
                return true;
            }
            //how to check for a chesspiece is just moving back and forward. 
            return false;

            void ThreefoldRepetition(bool team) //should this function be changed from an nested function to a normal function.
            {//how to best now which move is done. Could look through the mapmatrix and look for any changes. So have an old copy of it. 
                /* Before player turn, copy current mapmatrix into the "old" mapmatrix. Let player move. Compare old to new maptrix. 
                 * Find the change and added it to the threefoldRepetition array
                 * Check if the same player has done the exactly same move 3 tiems in row. If true, draw.
                 * 
                 * So have two for loops for x an y movement, nested. Run until you find a difference, save into the array and end both loops. 
                 *
                 *This idea will not work that easily
                 *  "threefold repetition rule (also known as repetition of position) states that a player can claim a draw if the same position occurs three times, or will occur after their next move, with the same player to move. 
                 *  The repeated positions do not need to occur in succession."
                 * "In chess, in order for a position to be considered the same, each player must have the same set of legal moves each time, including the possible rights to castle and capture en passant. 
                 * Positions are considered the same if the same type of piece is on a given square. For example, if a player has two knights and the knights are on the same squares, it does not matter if the positions of the two knights have been exchanged. 
                 * The game is not automatically drawn if a position occurs for the third time – one of the players, on their turn, must claim the draw with the arbiter. "
                 * https://en.wikipedia.org/wiki/Threefold_repetition
                 * 
                 * ...
                 * Read up some more to make sure you understand it correctly. Also might be easier to not have a function for this, but let the player(s) use a draw function.
                 */

                byte teamIndex = team ? (byte)0 : (byte)1;
                for (int i = 2; i > 0; i--)
                {
                    threefoldRepetition[teamIndex, i] = threefoldRepetition[teamIndex, i - 1];
                }
                for (int n = 0; n < MapMatrix.Map.GetLength(0); n++)
                {
                    for (int m = 0; m < MapMatrix.Map.GetLength(1); m++)
                    {
                        if (lastUpdateMap[n,m] != MapMatrix.Map[n,m])
                        {
                            threefoldRepetition[teamIndex, 0] = MapMatrix.Map[n, m]; //need to add the last location... 
                        }
                    }
                    //lastUpdateMap
                }
            }
        }

        /// <summary>
        /// Contains the code that allows the game to be played and loops through it until either a draw or one side wins.
        /// </summary>
        private void GameLoop()
        {
            bool gameEnded = false; bool whiteWon = false;
            do //should the game show what pieces that can save the king from a threat or should the player figure that out themselves? How much to hold their hand
            {
                gameEnded = PlayerControl(true);
                if (gameEnded)
                {
                    whiteWon = true;
                    break;
                } 
                gameEnded = PlayerControl(false);
            } while (!gameEnded);
            

            Console.Clear();
            Console.Write("White won = {0}", whiteWon); //just to ensure that the correct team is considered the winner. 
            Console.ReadLine();

            unsafe bool PlayerControl(bool team)
            {

                bool checkmate = false; bool draw = false;
                Player player;
                if (team)
                    player = white;
                else
                    player = black;
                player.Control();
                ProtectKing.ProtectEndLocations.Clear();
                checkmate = CheckmateChecker(!team,out List<string> saveKingList);
                //A piece, that is keeping the king safe, can be moved such that the king is under treat, which is not allowed after the rules. 
                //how to solve that one problem. 
                //if proven problematic to solve, focus on the menu and the net play to keep learning. So if not solved by the 28/4, keep a break from it and move to the other parts. 
                ProtectKing.Protect = saveKingList;
                //if the list is empty and the king is checked. Checkmate 
                draw = Draw(); //maybe move this one out to the outer loop
                if (checkmate || draw) //checkmate seems to work as it should. 
                    return true;
                
                for (int i = ChessList.GetList(team).Count - 1; i >= 0; i--) //somewhere in the player, have a function to surrender. 
                {
                    if (ChessList.GetList(team)[i].BeenTaken) 
                        ChessList.GetList(team).RemoveAt(i);
                }
                for(int i = ChessList.GetList(!team).Count - 1; i >= 0; i--)
                {
                    if (ChessList.GetList(!team)[i] is Pawn && ChessList.GetList(!team)[i].SpecialBool == true)
                        ChessList.GetList(!team)[i].SpecialBool = false;
                }
                return false;
            }
        }

        /// <summary>
        /// Runs the loop and code that plays the game.
        /// </summary>
        public void Play()
        {
            GameLoop();
        }

        /// <summary>
        /// Check if a king, depending on <paramref name="team"/> is checkmated or not. If the king is checkmated, it will return true. Else false. 
        /// Also <paramref name="canProtectKing"/> contains the ID of all pieces that can protect the king, if the king is checked. The king will also be in the list if the king can move. 
        /// </summary>
        /// <param name="team">True for white, false for black.</param>
        /// <returns></returns>
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
            if (/*!kingCanMove &&*/ isCheked)
            {
                foreach (ChessPiece chePie in ChessList.GetList(team))
                { 
                    string[] idParts = chePie.GetID.Split(':');
                    int[] chePieLocation = chePie.GetMapLocation;
                    string[] feltIDParts = MapMatrix.Map[locations[0][0], locations[0][1]].Split(':');
                    isKnight = feltIDParts[1] == "4" ? true : false;

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
                        int[][] movement = new int[][]
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
                        if (QRBCheck(movement, chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            //return false;
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Rock)
                    {
                        int[][] movement = new int[][]
                        {
                            new int[]{-1,0},
                            new int[]{1,0},
                            new int[]{0,1},
                            new int[]{0,-1}
                        };
                        if (QRBCheck(movement, chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            //return false;
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Bishop)
                    {
                        int[][] movement = new int[][]
                        {
                            new int[]{-1,-1},
                            new int[]{-1,1},
                            new int[]{1,-1},
                            new int[]{1,1}
                        };
                        if (QRBCheck(movement, chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            //return false;
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Knight)
                    {
                        if (KnightCheck(chePie.GetMapLocation, out List<int[,]> endLocation))
                        {
                            //return false;
                            ProtectKing.ProtectEndLocations.Add(chePie.GetID, endLocation);
                            canProtectKing.Add(chePie.GetID);
                        }
                    }
                    else if (chePie is Pawn pawn) //had a bug at a time that added it to the list of pieces that could prevent a check, even though it could not reach. Only happened for a non-moved piece. Y difference was 3.
                    {
                        if (PawnCheck(chePie.GetMapLocation, pawn.HasMoved, out List<int[,]> endLocation))
                        {
                            //Debug.WriteLine("{0}", chePie.GetID);
                            //return false;
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

            bool KnightCheck(int[] ownLocation, out List<int[,]> endLocations)
            {
                int[] kingHostileDifference = new int[] { kingLocation[0] - locations[0][0], kingLocation[1] - locations[0][1] };
                endLocations = new List<int[,]>();
                if (KnightCanReach(ownLocation, ref endLocations))
                    return true;
                else if (!isKnight)
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

                    while (distance < biggestDifference) //finds all locations between the king and hostile piece.
                    {
                        newLocation[0] += movement[0];
                        newLocation[1] += movement[1];
                        string feltID = MapMatrix.Map[newLocation[0], newLocation[1]];
                        if (feltID != "")
                            break;
                        if (KnightCanReach(newLocation, ref endLocations))
                            return true;
                        distance++;
                    }
                }
                return false;

                bool KnightCanReach(int[] standLocation, ref List<int[,]> endLoc)
                {
                    int[] locationDifference = new int[] { standLocation[0] - locations[0][0], standLocation[1] - locations[0][1] };
                    int[][] movement = new int[][] { new int[] { 1, 2 }, new int[] { 2, 1 }, new int[] { -1, -2 }, new int[] { -2, -1 }, new int[] { 1, -2 }, new int[] { 2, -1 }, new int[] { -1, 2 }, new int[] { -2, 1 } };
                    foreach (int[] mov in movement)
                    {
                        int[] movLocation = new int[] { ownLocation[0] + mov[0], ownLocation[1] + mov[1] };
                        if (movLocation[0] == standLocation[0] && movLocation[1] == standLocation[1])
                        {
                            if(MapMatrix.Map[movLocation[0], movLocation[1]] =="")
                                endLoc.Add(new int[,] { { movLocation[0], movLocation[1] } });
                            //return true;
                        }
                    }
                    if (endLoc.Count != 0)
                        return true;
                    else
                        return false;
                }
            }

            bool PawnCheck(int[] ownLocation, bool hasMoved, out List<int[,]> endLocations)
            {
                int direction = team ? -1 : 1; 
                int[] locationDifference = new int[] { ownLocation[0] - locations[0][0], ownLocation[1] - locations[0][1] };
                endLocations = new List<int[,]>();
                if (locationDifference[0] == 1) 
                { 
                    if (locationDifference[1] == -direction)
                    {
                        endLocations.Add(new int[,] { {ownLocation[0] +1 , ownLocation[1] + direction } });
                        return true;
                    }
                }
                else if (locationDifference[0] == -1)
                {
                    if (locationDifference[1] == -direction)
                    {
                        endLocations.Add(new int[,] { { ownLocation[0] - 1, ownLocation[1] + direction } });
                        return true;
                    } 
                }

                if (!isKnight)
                {

                    int xBig = kingLocation[0] > locations[0][0] ? kingLocation[0] : locations[0][0];
                    int xSmall = kingLocation[0] > locations[0][0] ? locations[0][0] : kingLocation[0];
                    if (ownLocation[0] > xSmall && ownLocation[0] < xBig) 
                    {
                        int[] directions = new int[]{ kingLocation[0] - locations[0][0], kingLocation[1] - locations[0][1]}; 
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

                        int[] standLocation = new int[2] { kingLocation[0], kingLocation[1]};
                        do
                        { //finds the y of the square that is between king and hostile piece that got the same x as the pawn.
                            standLocation[0] += movement[0];
                            standLocation[1] += movement[1]; //end location on y can reach 8
                        } while (standLocation[0] != ownLocation[0]); 

                        int yDistance = standLocation[1] - ownLocation[1];
                        int maxRange = hasMoved ? 1 : 2;
                        int pos = 0;
                        if (maxRange >= Math.Abs(yDistance))
                        {
                            do
                            {
                                pos++;
                                string feltID = MapMatrix.Map[ownLocation[0], ownLocation[1] + direction*pos];
                                if (feltID != "")
                                    return false;
                            } while (pos < maxRange);
                            endLocations.Add(new int[,] { {ownLocation[0],ownLocation[1]+maxRange*direction } });
                            return true;
                        }
                    }
                }
                return false;
            }

            bool QRBCheck(int[][] directions, int[] ownLocation, out List<int[,]> endLocations)
            {
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
                     * 
                     */

                    if (CanReach(dir, ownLocation, locations[0], ref endLocations))
                    {
                        endLocations.Add(new int[,] { { locations[0][0], locations[0][1] } });
                        return true;
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

                        while (distance < biggestDifference)
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
                    /*Idea: 
                     * if both parts of locationDifference is the same, it need to not have the same signs in the dir
                     * E.g. locationDifference == [2,2], dir needs to be [-1,-1]
                     * If LocationDifference == [-2,2], dir needs to be [1,-1]. 
                     * If locationDifference == [0,2], dir needs to be [0,-1] 
                     * etc.
                     * So if any index in locationDifference is zero, the same index in dir needs to be zero,
                     * else any index in dir needs to be of oppesit sign of the same index in locationDifference. 
                     * How to be check this. Could devide, if they got the same sign the devide will be positive. Different signs will give a negative result. Of course no devide for zero.
                     * So have two bools, one for each index. If true, index in locationDifference and dir is either zero or wiht oppesite signs. If false, got the same signs or not zero in both index.
                     * Any other way? 
                     */

                    int[] currentLocation = new int[] { ownLocation[0], ownLocation[1] };
                    int locationsRemaining = Math.Abs(locationDifference[0]) > Math.Abs(locationDifference[1]) ? Math.Abs(locationDifference[0]) : Math.Abs(locationDifference[1]); //should be the amount of sqaures from start to and with the end square. 
                    string feltID = ""; //maybe have a setting for the default value on the map
                    while (locationsRemaining != 0) //rewrite all of this, also write better comments for the future
                    {
                        currentLocation[0] += dir[0];
                        currentLocation[1] += dir[1];
                        feltID = MapMatrix.Map[currentLocation[0], currentLocation[1]];
                        if (feltID != "" && feltID != MapMatrix.Map[locations[0][0], locations[0][1]]) //currently it does not care if the last square is the one the enemy piece is standing on. Fixed. 
                            return false;

                        if (locationsRemaining == 1)
                        {
                            endLocations.Add(new int[,] { { currentLocation[0], currentLocation[1]} });
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
            byte[] colourWhite =
            {
                255,255,255
            };
            white = new Player(/*colourWhite,*/ true/*, whiteSpawnLocation*/);
            byte[] colourBlack =
            {
                0,0,0
            };
            black = new Player(/*colourBlack,*/ false/*, blackSpawnLocation*/);
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
                ID = String.Format("{0}:6:{1}", team, i);
                spawn = new int[] { spawnLocations[i, 0], spawnLocations[i, 1] };
                chessPieces.Add(new Pawn(colour, white, spawn, ID));
            }
            ID = String.Format("{0}:5:{1}", team, 1);
            spawn = new int[] { spawnLocations[8, 0], spawnLocations[8, 1] };
            chessPieces.Add(new Rock(colour, white, spawn, ID));
            ID = String.Format("{0}:5:{1}", team, 2);
            spawn = new int[] { spawnLocations[15, 0], spawnLocations[15, 1] };
            chessPieces.Add(new Rock(colour, white, spawn, ID));
            ID = String.Format("{0}:4:{1}", team, 1);
            spawn = new int[] { spawnLocations[9, 0], spawnLocations[9, 1] };
            chessPieces.Add(new Knight(colour, white, spawn, ID));
            ID = String.Format("{0}:4:{1}", team, 2);
            spawn = new int[] { spawnLocations[14, 0], spawnLocations[14, 1] };
            chessPieces.Add(new Knight(colour, white, spawn, ID));
            ID = String.Format("{0}:3:{1}", team, 1);
            spawn = new int[] { spawnLocations[10, 0], spawnLocations[10, 1] };
            chessPieces.Add(new Bishop(colour, white, spawn, ID));
            ID = String.Format("{0}:3:{1}", team, 2);
            spawn = new int[] { spawnLocations[13, 0], spawnLocations[13, 1] };
            chessPieces.Add(new Bishop(colour, white, spawn, ID));
            ID = String.Format("{0}:2:{1}", team, 1);
            spawn = new int[] { spawnLocations[11, 0], spawnLocations[11, 1] };
            chessPieces.Add(new Queen(colour, white, spawn, ID));
            ID = String.Format("{0}:1:{1}", team, 1);
            spawn = new int[] { spawnLocations[12, 0], spawnLocations[12, 1] };
            chessPieces.Add(new King(colour, white, spawn, ID));

            ChessList.SetChessList(chessPieces, white);
        }

    }

    class Player //got to ensure that no spawnlocation is overlapping and deal with it in case there is an overlap. 
    { //this class is set to be an abstract in the UML, but is that really needed? 
        //private byte[] colour;
        private bool white;
        //private List<ChessPiece> chessPieces = new List<ChessPiece>();
        //private int[,] spawnLocations; //start with the pawns, left to right and then the rest, left to right
        private string team;
        //private string selectedID;
        private int selectedChessPiece;
        private int[] location; //x,y
        private bool didMove = false;

        public Player(/*byte[] colour,*/ bool startTurn/*, int[,] spawnLocations*/)
        {
            //this.colour = colour;
            this.white = startTurn;
            //this.spawnLocations = spawnLocations;
            team = white == true ? "+" : "-";
            //CreatePieces();
        }

        /// <summary>
        /// Function that calls functions needed for playing the game.
        /// </summary>
        public void Control()
        {
            do
            {
                HoverOver();
                MovePiece();
                didMove = ChessList.GetList(white)[selectedChessPiece].CouldMove;
            } while (!didMove);
        }

        /// <summary>
        /// Lets the player hover over a felt on the boardgame and gets the ID of that felt. If the ID is a chesspiece, it will be highlighted. 
        /// </summary>
        private void HoverOver()
        {
            string lastMapLocationID;
            int? lastPiece = 0;
            ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(true);
            bool hasSelected = false;
            location = ChessList.GetList(white)[0].GetMapLocation;
            SquareHighLight(true);
            do
            {
                bool selected = FeltMove(location);
                if (lastPiece != null) //if a previous chess piece has been hovered over, remove the highlight. 
                {
                    ChessList.GetList(white)[(int)lastPiece].IsHoveredOn(false);
                    lastPiece = null;
                    lastMapLocationID = null;
                }
                string squareID = MapMatrix.Map[location[0], location[1]];
                if (squareID != "") //is there a chess piece on the location
                    if (squareID.Split(':')[0] == team) //is it on the team
                    {
                        int posistion = 0;
                        foreach (ChessPiece piece in ChessList.GetList(white))
                        { 
                            if (piece.GetID == squareID)
                            { //correct chess piece and highlight it
                                piece.IsHoveredOn(true);
                                lastMapLocationID = piece.GetID;
                                lastPiece = posistion;


                                if (selected == true)
                                {
                                    if (ProtectKing.Protect.Count != 0)
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
                                    }

                                }
                            }
                            posistion++;
                        }
                    }

            } while (!hasSelected);
            SquareHighLight(false);
            //SelectPiece();

        }

        /// <summary>
        /// Allows the player to either move to a connecting square to <paramref name="currentLocation"/> or select the <paramref name="currentLocation"/>.
        /// </summary>
        /// <param name="currentLocation">The current location on the board.</param>
        /// <returns></returns>
        private bool FeltMove(int[] currentLocation)
        {
            //uint[] currentLocation = location; //remember that both arrays point to the same memory.

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            SquareHighLight(false);
            if (keyInfo.Key == ConsoleKey.UpArrow && currentLocation[1] > 0)
            {
                currentLocation[1]--;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow && currentLocation[1] < 7)
            {
                currentLocation[1]++;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow && currentLocation[0] > 0)
            {
                currentLocation[0]--;
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow && currentLocation[0] < 7)
            {
                currentLocation[0]++;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                return true;
            }

            SquareHighLight(true);
            return false;

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
            if (isHighlighted)
            {
                byte[] colour = Settings.SelectSquareColour;
                Paint(colour);
            }
            else
            {
                byte colorLocator = (byte)((location[0] + location[1]) % 2);
                byte[] colour = colorLocator == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
                Paint(colour);
            }

            void Paint(byte[] colour)
            {
                for (int n = startLocationX; n < startLocationX + squareSize; n++)
                {
                    Console.SetCursorPosition((int)n, (int)startLocationY);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                    Console.SetCursorPosition((int)n, (int)startLocationY + squareSize - 1);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                }
                for (int n = startLocationY; n < startLocationY + squareSize; n++)
                {
                    Console.SetCursorPosition((int)startLocationX, (int)n);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                    Console.SetCursorPosition((int)startLocationX + squareSize - 1, (int)n);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                }
            }
        }

        //private void SelectPiece()
        //{ //how this function will work will depent on the chosen method for hovering over, i.e. using the list or not.
        //    selectedID = null; //will be needed to find the chosen chess piece in the list if the list is not the selected hover over method

        //}

        /// <summary>
        /// Function that calls the selected chess piece control function. 
        /// </summary>
        private void MovePiece()
        {
            //if (chessPieces[selectedChessPiece] is King working)
            //{
            //    //King working = (King)chessPieces[selectedChessPiece];
            //    //working.KingTest
            //    //do stuff, replace the old one with the updated one
            //    //works, but are there better ways...
            //}
            //chessPieces[selectedChessPiece].Control();
            ChessList.GetList(white)[selectedChessPiece].Control();
        }

    }


    /// <summary>
    /// The class for kings
    /// </summary>
    sealed class King : ChessPiece
    { //this one really need to keep an eye on all other pieces and their location

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
        public King(byte[] colour_, bool team_, int[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "^V^",
                "*|*",
                "-K-"
            };
            Draw();
        }

        /// <summary>
        /// King only function. Returns the list of squares it is treaten from. 
        /// </summary>
        public List<int[]> GetCheckList { get => checkLocations; }

        /// <summary>
        /// Returns true if the king can move. False otherwise. 
        /// </summary>
        public bool CanMove { 
            get
            {
                EndLocations();
                bool canMove = possibleEndLocations.Count != 0;
                possibleEndLocations.Clear();
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
            Move();
            RemoveDraw(oldMapLocation);
            LocationUpdate();
            Draw();
            UpdateMapMatrix(oldMapLocation);
            //CheckWriteOut();
            //checkLocations.Clear();
            castLingCandidates.Clear();
        }

        /// <summary>
        /// Writes out at a specific location, depending on team and given by the Settings class, from where it is treaten.
        /// </summary>
        private void CheckWriteOut()
        {
            if (isChecked || lastAmountOfThreats > 0)
            {
                //string writeLout = "";
                int[,] writeLocation = Settings.CheckWriteLocation; //should be modified so it return a new array rather than the existing array. 
                byte teamLoc;
                teamLoc = team ? (byte)0 : (byte)1;

                int[] writeAt = new int[] { writeLocation[teamLoc, 0], writeLocation[teamLoc, 1] };
                //Console.SetCursorPosition(writeAt[0],writeAt[1]); //maybe have the board write the King White and King Black with another setting, then CheckWrtieLocation setting is that setting plus like 2 
                //Console.WriteLine();
                byte pos = 0;
                for (byte i = 0; i < lastAmountOfThreats; i++)
                {
                    Console.SetCursorPosition(writeAt[0], writeAt[1] + i);
                    Console.Write("".PadLeft(2));
                }
                foreach (int[] loc in checkLocations)
                {
                    char letter = (char)(97 + loc[0]);
                    string writeLout = String.Format("{0}{1}", letter, loc[1] + 1);
                    Console.SetCursorPosition(writeAt[0], writeAt[1] + pos);
                    Console.WriteLine(writeLout);
                    pos++;
                }
                lastAmountOfThreats = pos;
            }
        }

        /// <summary>
        /// Calculates end locations and if legal and is not under threat adds them to a list. 
        /// </summary>
        protected override void EndLocations()
        { 
            FindCastlingOptions(possibleEndLocations);

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
            CheckPosistions(position); //left down

            position = new sbyte[2] { 1, 1 };
            CheckPosistions(position); //right, down

            if (possibleEndLocations.Count != 0)
            {
                hasMoved = true;
            }

            void CheckPosistions(sbyte[] currentPosition)
            {
                sbyte[] loc = new sbyte[2] { currentPosition[0], currentPosition[1] };
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
                        if (teamString != feltID.Split(':')[0])
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
        { //if true, it should force the player to move it. Also, it needs to check each time the other player has made a move 
            //should also check if it even can move, if it cannot the game should end. 
            //the king does not need to move in a check as long time there is a friendly chesspiece that can take the hostile piece. 
            //should all the pieces that can move to prevent a mate be highlighted? Should their endlocations be forced to only those that can prevent a check?
            //how much should the game hold the player in hand?
            //Should the game write to a location that the king is check and the location (letter and number) of the hostile pieces that threatens the king?
            //should the king's constructor have a write location, so the king can do the writting and not the board/player?
            //When should this code be called? Ideally, at the start of the player turn. But should it be called at other moments? E.g. before or after a king movement or should the move code check by itself 
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
            }; //knights and pawns need different way of being checked. 
            QRBCheck(moveDirection, toLookFor);
            //problem: if a square is checked that is not the king's and the code find the king, it will think that direction is fine even though there might be a hostile piece that can take the king on its current posistion and 
            //the posistion that is being checked. Seems to have been fixed, require more testing.
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
                                if (feltIDParts[0] != teamString)
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
            { //two in one direction, one in another direction
                int[] placement_;
                placement_ = new int[] { -2, -1 }; //two left, up
                Placement(placement_);
                placement_ = new int[] { -2, 1 }; //two left, down
                Placement(placement_);
                placement_ = new int[] { 2, -1 }; //two right, up
                Placement(placement_);
                placement_ = new int[] { 2, -1 }; //two right, down
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
                            if (feltIDParts[0] != teamString)
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


            void PawnCheck() //need testing
            {
                sbyte hostileDirection = team ? (sbyte)-1 : (sbyte)1; //if white, pawns to look out for comes for the top. If black, they come from the bottom.
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
                        if (idParts[0] != teamString)
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
            { //can be used to check for queens, rocks and bishops.
                //consider coding it such that it can work with a sbyte[,] and go through multiple directions in a single call.
                //should the checkPiecesToCheckFor also be altered or is it fine 
                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    //need to alter this one. Rock and Bishop are considered to have the same movement as the queen. Maybe, let it take a jaggered checkpiecesToCheckFor
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
                                if (IDstrings[0] != teamString) //checks if it is hostile or not 
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
                castLingCandidates = new List<string>(); //does a string list make sense? 
                foreach (ChessPiece chepie in ChessList.GetList(team))
                {
                    if (chepie is Rock)
                    {
                        if (!chepie.SpecialBool)
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
                                //from the rules, castling cannot happen if the king is checked. Also, it does not matter if the rock's end location is under threat
                                //but all sqaures the king moves through also needs not be under threat. 
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
            EndLocations();
            if (possibleEndLocations.Count != 0)
            {
                DisplayPossibleMove();
                int[] cursorLocation = GetMapLocation;
                do
                {
                    bool selected = FeltMove(cursorLocation);
                    if (selected)
                    {
                        foreach (int[,] loc in possibleEndLocations)
                        {
                            int[] endloc_ = new int[2] { loc[0, 0], loc[0, 1] };
                            if (endloc_[0] == cursorLocation[0] && endloc_[1] == cursorLocation[1])
                            {
                                couldMove = true;
                                bool castling = true;
                                oldMapLocation = new int[2] { mapLocation[0], mapLocation[1] };
                                castling = FeltIDCheck(cursorLocation);
                                if (castling)
                                {
                                    Castling(cursorLocation);
                                    hasSelected = true;
                                    break;
                                }
                                else if (!castling)
                                    TakeEnemyPiece(cursorLocation);
                                mapLocation = new int[2] { cursorLocation[0], cursorLocation[1] };
                                hasSelected = true;
                                break;
                            }
                        }
                    }
                } while (!hasSelected);
                NoneDisplayPossibleMove();
                possibleEndLocations.Clear();
            }
            else
            {
                couldMove = false;
            }

            bool FeltIDCheck(int[] loc_)
            {
                string[] feltIDParts = MapMatrix.Map[loc_[0], loc_[1]].Split(':');
                if (feltIDParts[0] == teamString)
                    if (feltIDParts[1] == "5")
                    {
                        return true;
                    }
                return false;
            }
        }

        /// <summary>
        /// Selects a rock using the map location.
        /// </summary>
        /// <param name="locationOfRock"></param>
        private void Castling(int[] locationOfRock)
        {
            string rockID = MapMatrix.Map[locationOfRock[0], locationOfRock[1]];
            foreach (ChessPiece chePie in ChessList.GetList(team))
            {
                if (chePie.GetID == rockID)
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
        public Queen(byte[] colour_, bool team_, int[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "_w_",
                "~|~",
                "-Q-"
            };
            Draw();
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        { //implement a check for Castling and/or call the Castling function
            //is there a better way to do this than the current way. Currently it can go out of bounds. 
            //could most likely make a nested function of the do while loop
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
                        if (teamString != feltID.Split(':')[0])
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

    /// <summary>
    /// The class for pawns.
    /// </summary>
    sealed class Pawn : ChessPiece
    { //does not check if a square two away from it is empty. Can jump over another piece with the double first turn move.
        //bug: If the chess piece cannot move and have not moved and it is selected, its RemoveDraw code will be run and give an error with the oldMapLocation is null.
        //oldMapLocation is only set in the Move function and is called by RemoveDraw right after. RemoveDraw needs to be able to handle and solve null arrays. 
        private bool firstTurn = true;
        private bool hasMoved = false;
        private sbyte moveDirection;
        private Dictionary<string, byte> promotions = new Dictionary<string, byte>(4);
        // https://docs.microsoft.com/en-us/dotnet/standard/generics/covariance-and-contravariance
        // https://stackoverflow.com/questions/210601/accessing-a-property-of-derived-class-from-the-base-class-in-c-sharp
        //consider added a function in the base call that returns whatever is needed, e.g. can castle, has double moved and not moved after etc. Most likely being a bool. 

        //was a bug at a moment where a pawn could not double move in a game, even through it had not moved. Something, I think, had stod on the location before. 
        //found it what is causing it, if a pawn is selected and cannot move, its firstTurn is still set to false.
        /// <summary>
        /// The constructor for the pawn chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public Pawn(byte[] colour_, bool team_, int[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                " - ",
                " | ",
                "-P-"
            };
            moveDirection = team ? (sbyte)-1 : (sbyte)1;
            //teamString = team ? "+" : "-";
            Draw();
            promotions.Add("Knight", 4); //remember to use these
            promotions.Add("Rock", 5);
            promotions.Add("Bishop", 3);
            promotions.Add("Queen", 2);
        }

        //public override bool SpecialBool { get => firstTurn; set => base.SpecialBool = value; } //this function is causing problems with the firstTurn
        /// <summary>
        /// Returns true if the pawn has moved at some point. False otherwise. 
        /// </summary>
        public bool HasMoved { get => hasMoved;}

        /// <summary>
        /// A modified version of the base Move function. Designed to check if the player uses a double move. 
        /// </summary>
        protected override void Move()
        {
            oldMapLocation = null;
            bool hasSelected = false;
            if (ProtectKing.GetListFromDic(ID) != null)
            {
                possibleEndLocations = ProtectKing.GetListFromDic(ID);
                specialBool = false;
                hasMoved = true;
                couldMove = true;
            }
            else
            {
                EndLocations();
            }
            if (possibleEndLocations.Count != 0)
            {
                DisplayPossibleMove();
                int[] cursorLocation = GetMapLocation;
                do
                {
                    bool selected = FeltMove(cursorLocation);
                    if (selected)
                    {
                        foreach (int[,] loc in possibleEndLocations)
                        {
                            int[] endloc_ = new int[2] { loc[0, 0], loc[0, 1] };
                            if (endloc_[0] == cursorLocation[0] && endloc_[1] == cursorLocation[1])
                            {

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
                                        if(MapMatrix.Map[cursorLocation[0],cursorLocation[1]] == "")
                                            TakeEnemyPiece(new int[] { cursorLocation[0], cursorLocation[1] - moveDirection }); //minus since the direction the pawn is moving is the oppesite direction of the hostile pawn is at. 
                                        else
                                            TakeEnemyPiece(cursorLocation);
                                    }
                                }
                                break;
                            }
                        }
                    }
                } while (!hasSelected);
                NoneDisplayPossibleMove();
                possibleEndLocations.Clear();
                hasMoved = true;
            }
            else
            {
                couldMove = false;
            }

        }

        public override void Network(int[] newLocation = null, bool captured = false)
        {
            if (captured)
                Taken();
            else
            { //for king, rock and pawn it needs more code. 
                if (newLocation != null)
                {
                    if(Math.Abs(newLocation[1] - mapLocation[1]) == 2)
                    {
                        specialBool = true;
                    }
                    RemoveDraw(mapLocation);
                    mapLocation = newLocation;
                    Draw();
                }
            }

        }

        /// <summary>
        /// Overriden control function of the base class. Checks if the chess piece is ready for a promotion. 
        /// </summary>
        public override void Control()
        {
            Move();
            RemoveDraw(oldMapLocation);
            LocationUpdate();
            Draw();
            UpdateMapMatrix(oldMapLocation);
            Promotion();
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        {
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
            firstTurn = firstTurn ? false : false;
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
                //if (firstTurn)
                //{
                    EnPassant();
                //}
            }

            void LocationCheck(sbyte direction) //find a better name
            {
                string locationID = MapMatrix.Map[mapLocation[0] + direction, mapLocation[1] + moveDirection];
                if (locationID != "")
                {
                    string teamID = locationID.Split(':')[0];
                    if (teamID != teamString)
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
                            if(hostileLocation[1] == mapLocation[1]) //does the pawn and en-passant pawn share the same y. 
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
                string command = "Choose: ";
                string answer = "";
                //How to select? Arrowkeys? Numberkeys? Written?
                DisplayPromotions();
                Console.SetCursorPosition(Settings.PromotionWriteLocation[0], Settings.PromotionWriteLocation[1] + 1);
                Console.Write(command);
                do //not really happy with this, it does not fit the rest of the game. Consider other ways to do it.
                {
                    Console.SetCursorPosition(Settings.PromotionWriteLocation[0] + command.Length, Settings.PromotionWriteLocation[1] + 1);
                    Console.Write("".PadLeft(answer.Length));
                    Console.SetCursorPosition(Settings.PromotionWriteLocation[0] + command.Length, Settings.PromotionWriteLocation[1] + 1);
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
                Taken();
                string[] IDParts = ID.Split(':');
                string newID;
                switch (answer)
                {
                    case "knight":
                        IDParts[1] = "4";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]); //The P is to indicate that the piece used to be a pawn.
                        ChessList.GetList(team).Add(new Knight(colour, team, mapLocation, newID));
                        break;

                    case "bishop":
                        IDParts[1] = "3";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]);
                        ChessList.GetList(team).Add(new Bishop(colour, team, mapLocation, newID));
                        break;

                    case "rock":
                        IDParts[1] = "5";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]);
                        ChessList.GetList(team).Add(new Rock(colour, team, mapLocation, newID));
                        break;

                    case "queen":
                        IDParts[1] = "2";
                        newID = String.Format("{0}:{1}:{2}P", IDParts[0], IDParts[1], IDParts[2]);
                        ChessList.GetList(team).Add(new Queen(colour, team, mapLocation, newID));
                        break;

                }
            }

        }

        /// <summary>
        /// Displays the possible promotions
        /// </summary>
        private void DisplayPromotions()
        { //writes to a location what chesspieces it can be promoted too.
            string promotionsString = "";
            foreach (string key in promotions.Keys)
            {
                promotionsString += key + " ";
            }
            Console.SetCursorPosition(Settings.PromotionWriteLocation[0], Settings.PromotionWriteLocation[1]);
            Console.Write(promotionsString);
        }

    }

    /// <summary>
    /// The class for rocks.
    /// </summary>
    sealed class Rock : ChessPiece
    {
        private bool hasMoved = false;

        /// <summary>
        /// The constructor for the rock chess piece. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece.</param>
        /// <param name="spawnLocation_">The start location of the chess piece.</param>
        /// <param name="ID">The ID of the chess piece.</param>
        public Rock(byte[] colour_, bool team_, int[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "^^^",
                "|=|",
                "-R-"
            };
            Draw();
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        { //implement a check for Castling and/or call the Castling function
            //is there a better way to do this than the current way. Currently it can go out of bounds. 
            //could most likely make a nested function of the do while loop
            sbyte[] position = new sbyte[2] { -1, 0 };
            CheckPosistions(position); //left

            position = new sbyte[2] { 1, 0 };
            CheckPosistions(position); //right

            position = new sbyte[2] { 0, -1 };
            CheckPosistions(position); //up

            position = new sbyte[2] { 0, 1 };
            CheckPosistions(position); //down

            if (possibleEndLocations.Count != 0)
            { //need to make sure that if a player selects the rock and it cannot move, it does not prevent castling from happening. 
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
                        if (teamString != feltID.Split(':')[0])
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
        /// Returns true if the ches piece has moved at any point, false otherwise. 
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
        } //if moved, castling cannot be done. How is the king going to call this code. Currently, the king would only be able to call other's functions that are given in the base class.

        /// <summary>
        /// Calss the castling function. 
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

        /// <summary>
        /// Moves the chess piece after the rules of castling. 
        /// </summary>
        private void Castling()
        {
            RemoveDraw(mapLocation);

            if (mapLocation[0] == 0)
            {
                mapLocation[0] = 3;
            }
            else
            {
                mapLocation[0] = 5;
            }
            LocationUpdate();
            Draw();
            hasMoved = true;
        }

    }

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

        public Bishop(byte[] colour_, bool team_, int[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "_+_",
                "|O|",
                "-B-"
            };
            Draw();
        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        {
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
                        if (teamString != feltID.Split(':')[0])
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
        public Knight(byte[] colour_, bool team_, int[] spawnLocation_, string ID) : base(colour_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                " ^_",
                " |>",
                "-k-"
            };
            Draw();

        }

        /// <summary>
        /// Calculates end locations and if legal add them to a list. 
        /// </summary>
        protected override void EndLocations()
        { //there must be a better way to do this...
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
                    if (feltID.Split(':')[0] != teamString)
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
        protected byte squareSize = Settings.SquareSize;
        protected List<int[,]> possibleEndLocations = new List<int[,]>();
        protected string teamString; //come up with a better name
        protected bool couldMove;
        protected bool specialBool;
        //https://en.wikipedia.org/wiki/Chess_piece_relative_value if you ever want to implement an AI this could help 


        /// <summary>
        /// The default chess piece constructor. 
        /// </summary>
        /// <param name="colour_">The colour of the chess piece.</param>
        /// <param name="team_">The team of the chess piece, true for white, false for black.</param>
        /// <param name="mapLocation_">The start location on the map.</param>
        /// <param name="ID">The ID of the chess piece. The constructor does nothing to ensure the ID is unique.</param>
        public ChessPiece(byte[] colour_, bool team_, int[] mapLocation_, string ID)
        {
            Colour = colour_;
            SetTeam(team_);
            MapLocation = mapLocation_;
            this.ID = ID; //String.Format("{0}n:{1}", team, i); team = team_ == true ? "-" : "+"; n being the chesspiece type
            LocationUpdate();
            MapMatrix.Map[mapLocation[0], mapLocation[1]] = ID;
            teamString = ID.Split(':')[0];
        }

        /// <summary>
        /// Returns a bool that indicate if this piece has been "taken" by another player's piece. 
        /// </summary>
        public bool BeenTaken { get => hasBeenTaken; } //use by other code to see if the piece have been "taken" and should be removed from game. 

        /// <summary>
        /// Sets the hasBeenTaken value...
        /// </summary>
        protected bool SetBeenTaken { set => hasBeenTaken = value; }

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
                //return location;
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
        /// Returns the location on the map. 
        /// </summary>
        public int[] GetMapLocation
        {
            get
            {
                int[] mapLo = new int[2] { mapLocation[0], mapLocation[1] };
                return mapLo;
            }
        }

        /// <summary>
        /// Gets and set the ID of the chesspiece. //maybe have some code that ensures the ID is unique 
        /// </summary>
        protected string ID { get => id; set => id = value; } //maybe split into two. Set being protected and the Get being public 

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
        /// Function that "controls" a piece. What to explain and how to..
        /// </summary>
        public virtual void Control()
        {
            Move();
            RemoveDraw(oldMapLocation);
            LocationUpdate();
            Draw();
            UpdateMapMatrix(oldMapLocation);
        }

        /// <summary>
        /// The function of this function depends on the chesspiece. Rock, pawn, and king got different implementations.
        /// </summary>
        /// <returns></returns>
        public virtual bool SpecialChessPieceFunction()
        {
            return false;
        }

        /// <summary>
        /// Calculates the, legal, end locations that a chess piece can move too.
        /// </summary>
        protected virtual void EndLocations()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newLocation"></param>
        /// <param name="captured"></param>
        public virtual void Network(int[] newLocation = null, bool captured = false)
        {
            if (captured)
                Taken();
            else
            { //for king, rock and pawn it needs more code. 
               if(newLocation != null)
                {
                    RemoveDraw(mapLocation);
                    mapLocation = newLocation;
                    Draw();
                }
            }
                
        }

        /// <summary>
        /// Allows the chesspiece to move. 
        /// </summary>
        protected virtual void Move()
        {
            oldMapLocation = null;
            bool hasSelected = false;
            if (ProtectKing.GetListFromDic(ID) != null)
            {
                possibleEndLocations = ProtectKing.GetListFromDic(ID);
            }
            else
            {
                EndLocations();
            }
                if (possibleEndLocations.Count != 0)
                {
                    DisplayPossibleMove();
                    int[] cursorLocation = GetMapLocation;
                    do
                    {
                        bool selected = FeltMove(cursorLocation);
                        if (selected)
                        {
                            foreach (int[,] loc in possibleEndLocations)
                            {
                                int[] endloc_ = new int[2] { loc[0, 0], loc[0, 1] };
                                if (endloc_[0] == cursorLocation[0] && endloc_[1] == cursorLocation[1])
                                {
                                    couldMove = true;
                                    oldMapLocation = new int[2] { mapLocation[0], mapLocation[1] };
                                    TakeEnemyPiece(cursorLocation);
                                    mapLocation = new int[2] { cursorLocation[0], cursorLocation[1] };
                                    hasSelected = true;
                                    break;
                                }
                            }
                        }
                    } while (!hasSelected);
                    NoneDisplayPossibleMove();
                    possibleEndLocations.Clear();
                }
                else
                {
                    couldMove = false;
                }
            
        }

        /// <summary>
        /// Checks if <paramref name="locationToCheck"/> contain an ID and if the ID is hostile, the function will call that ID's chesspiece's Taken function.
        /// </summary>
        /// <param name="locationToCheck">The location to check for a chess piece</param>
        protected void TakeEnemyPiece(int[] locationToCheck)
        {
            string feltID = MapMatrix.Map[locationToCheck[0], locationToCheck[1]];
            if (feltID != "")
                if (teamString != feltID.Split(':')[0])
                {
                    foreach (ChessPiece chessHostile in ChessList.GetList(!team))
                    {
                        if (chessHostile.GetID == feltID)
                        {
                            chessHostile.Taken();
                        }
                    }
                }
        }

        /// <summary>
        /// Allows the chesspiece to select different sqaures on the board. If enter is pressed on a square the chesspiece can move too, it will move to that square. 
        /// </summary>
        /// <param name="currentLocation">The current location of hovered over square. </param>
        /// <returns>Returns true if enter is pressed, else false.</returns>
        protected bool FeltMove(int[] currentLocation)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            SquareHighLight(false, currentLocation);
            foreach (int[,] loc in possibleEndLocations)
            {
                int[] endloc_ = new int[2] { loc[0, 0], loc[0, 1] };
                if (endloc_[0] == currentLocation[0] && endloc_[1] == currentLocation[1])
                {
                    PaintBackground(Settings.SelectMoveSquareColour, loc);
                    break;
                }
            }
            if (keyInfo.Key == ConsoleKey.UpArrow && currentLocation[1] > 0)
            {
                currentLocation[1]--;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow && currentLocation[1] < 7)
            {
                currentLocation[1]++;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow && currentLocation[0] > 0)
            {
                currentLocation[0]--;
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow && currentLocation[0] < 7)
            {
                currentLocation[0]++;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                return true;
            }
            SquareHighLight(true, currentLocation);
            return false;

        }

        /// <summary>
        /// Highlights or dehighlights a sqare.
        /// </summary>
        /// <param name="isHighlighted">If true, the square at <paramref name="currentLocation"/> is highlighted, else it is not highlighted.</param>
        /// <param name="currentLocation">The location to (de)highlight.</param>
        protected void SquareHighLight(bool isHighlighted, int[] currentLocation)
        {
            byte squareSize = Settings.SquareSize;
            int startLocationX = currentLocation[0] * squareSize + (currentLocation[0] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[0];
            int startLocationY = currentLocation[1] * squareSize + (currentLocation[1] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[1];
            if (isHighlighted)
            {
                byte[] colour = Settings.SelectSquareColour;
                Paint(colour);
            }
            else
            {
                byte colorLocator = (byte)((currentLocation[0] + currentLocation[1]) % 2);
                byte[] colour = colorLocator == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
                Paint(colour);
            }

            void Paint(byte[] colour)
            {
                for (int n = startLocationX; n < startLocationX + squareSize; n++)
                {
                    Console.SetCursorPosition((int)n, (int)startLocationY);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                    Console.SetCursorPosition((int)n, (int)startLocationY + squareSize - 1);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                }
                for (int n = startLocationY; n < startLocationY + squareSize; n++)
                {
                    Console.SetCursorPosition((int)startLocationX, (int)n);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                    Console.SetCursorPosition((int)startLocationX + squareSize - 1, (int)n);
                    Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                }
            }
        }

        /// <summary>
        /// updates the location that is used for displaying the chesspiece on the chessboard
        /// </summary>
        protected void LocationUpdate()
        {
            Location = new int[2] { mapLocation[0] * squareSize + (mapLocation[0] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[0], mapLocation[1] * squareSize + (mapLocation[1] + Settings.EdgeSpacing + Settings.Spacing) * 1 + Settings.Offset[1] };
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
        public void IsHoveredOn(bool hover)
        {
            if (hover)
            { //a lot of this code is the same as in other functions, e.g. RemoveDraw and Draw, just with other colours. Consider making a new function with this code and parameters for colour.
                PaintBoth();
            }
            else
                Draw();
        }

        /// <summary>
        /// Calculates the paint location and the paint colour. Paint colour is calculated out from <paramref name="mapLoc"/>.
        /// </summary>
        /// <param name="drawLocationX">The x start posistion.</param>
        /// <param name="drawLocationY">The y start posistion.</param>
        /// <param name="mapLoc">The location on the map matrix that is used to determine background colour.</param>
        /// <returns></returns>
        protected byte[] PaintCalculations(out int drawLocationX, out int drawLocationY, int[] mapLoc)
        { //replace a lot of code with this function
            byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
            drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            int locationForColour = (mapLoc[0] + mapLoc[1]) % 2; //if zero, background colour is "white", else background colour is "black".
            byte[] colours = locationForColour == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
            return colours;
        }

        /// <summary>
        /// Paints the foreground of the current location.
        /// </summary>
        protected void PaintForeground() //these did not really end up saving on code. Consider moving the first 3 or 5 lines of code into a function...
        {

            byte[] colours = PaintCalculations(out int drawLocationX, out int drawLocationY, mapLocation);
            for (int i = 0; i < design[0].Length; i++) //why does all the inputs, length, count and so on use signed variable types... 
            { //To fix, the background colour is overwritten with the default colour, black, rather than keeping the current background colour.
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("\x1b[48;2;" + colours[0] + ";" + colours[1] + ";" + colours[2] + "m\x1b[38;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m{0}\x1b[0m", design[i], colours);
            }
        }

        /// <summary>
        /// Paints both the background and foreground of the current location. 
        /// </summary>
        protected void PaintBoth()
        {
            byte[] backColours = PaintCalculations(out int drawLocationX, out int drawLocationY, mapLocation);
            byte[] colours = Settings.SelectPieceColour;
            for (int i = 0; i < design[0].Length; i++)
            {
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("\x1b[48;2;" + backColours[0] + ";" + backColours[1] + ";" + backColours[2] + "m\x1b[38;2;" + colours[0] + ";" + colours[1] + ";" + colours[2] + "m{0}\x1b[0m", design[i]);
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
                for (int i = 0; i < design[0].Length; i++)
                {
                    Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                    Console.Write("\x1b[48;2;" + colours[0] + ";" + colours[1] + ";" + colours[2] + "m".PadRight(design[0].Length + 1, ' ') + "\x1b[0m");
                }
            }
        }

        /// <summary>
        /// updates the map matrix with the new location of the chess piece and sets the old location to zero. 
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
        /// Set a chesspeice set to be taken so it can be removed and removes its visual representation. 
        /// </summary>
        public void Taken()
        {//call by another piece, the one that takes this piece. 
            hasBeenTaken = true;
            MapMatrix.Map[mapLocation[0], mapLocation[1]] = "";
            RemoveDraw(mapLocation);
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
        protected void NoneDisplayPossibleMove()
        {
            foreach (int[,] end in possibleEndLocations)
            {
                byte colourLoc = (byte)((end[0, 0] + end[0, 1]) % 2);
                byte[] backColour = colourLoc == 0 ? Settings.SquareColour1 : Settings.SquareColour2;
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
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                Console.SetCursorPosition((int)n, (int)startLocationY + squareSize - 1);
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
            }
            for (int n = startLocationY; n < startLocationY + squareSize; n++)
            {
                Console.SetCursorPosition((int)startLocationX, (int)n);
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
                Console.SetCursorPosition((int)startLocationX + squareSize - 1, (int)n);
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m " + "\x1b[0m");
            }
        }

    }


}
