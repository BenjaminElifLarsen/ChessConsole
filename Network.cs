using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chess
{
    /// <summary>
    /// Contains classes for transmitting and receivering data and a support class.
    /// </summary>
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

        /// <summary>
        /// Class for transmitting data.
        /// </summary>
        public class Transmit
        {
            private static string ipAddress_other;
            /// <summary>
            /// Gets the IP address of the other player.
            /// </summary>
            public static string OtherPlayerIpAddress { get => ipAddress_other; set => ipAddress_other = value; }

            /// <summary>
            /// Sets up the transmitter with <paramref name="IPaddress"/>.
            /// If it succeeded it return true, else false.
            /// </summary>
            /// <param name="IPaddress">The IP address to connect too.</param>
            /// <returns>Returns true if it can connect to <paramref name="IPaddress"/>, else false.</returns>
            public static bool TransmitSetup(string IPaddress, bool transmit = false)
            {
                try
                {
                    int port = 23000;
                    IPAddress transmitterAddress = IPAddress.Parse(IPaddress);
                    transmitter = new TcpClient(IPaddress, port); 
                    if (transmit)
                    {
                        NetworkStream networkStream = transmitter.GetStream();
                        Converter.Conversion.ValueToBitArrayQuick(2, out byte[] dataArray);
                        networkStream.Write(dataArray, 0, dataArray.Length);
                        networkStream.Close();
                    }
                    return true;
                }
                catch (Exception e)
                { //write anything out?
                    Debug.WriteLine(e);
                    if (!GameStates.LostConnection)
                    {
                        Console.Clear();
                        Console.WriteLine("Connection could not be established/was lost");
                        Console.WriteLine("{0} to return to the menu", Settings.SelectKey);
                        while (Console.ReadKey().Key != Settings.SelectKey) ;
                        GameStates.VictoryType = null;
                        GameStates.LostConnection = true;
                        GameStates.GameEnded = true;
                    }
                    return false;
                }

            }

            /// <summary>
            /// Called to ensure that the receiver of the other player is still running. 
            /// </summary>
            /// <param name="ipAddress">The IP address to contact.</param>
            /// <returns></returns>
            public static void StillConnected(object ipAddress)
            {
                bool isConnected = true;
                double counter = 0;
                ushort maxValue = 3000;
                Thread connectThread = new Thread(new ThreadStart(Run));
                connectThread.Start();
                DateTime oldTime = DateTime.Now;
                do
                {
                    DateTime newTime = DateTime.Now;
                    counter = (newTime - oldTime).TotalMilliseconds;

                    Thread.Sleep(1);
                } while (!GameStates.GameEnded && isConnected && counter < maxValue);
                GameStates.LostConnection = isConnected; 
                GameStates.GameEnded = true; //needs to call the events instead of calling the variables directly. Forgot about this function when I made the switch.
                if (GameStates.PlayerTeam == false)
                    GameStates.TurnCounter++;
                void Run()
                {
                    do
                    {
                        oldTime = DateTime.Now;
                        isConnected = GeneralValueTransmission(10, (string)ipAddress, true);
                        Debug.WriteLine("Connection active: " + isConnected.ToString());
                        Thread.Sleep(maxValue / 2);
                    } while (!GameStates.GameEnded && isConnected);
                }
            }

            //https://docs.microsoft.com/en-us/dotnet/api/system.net.networkinformation.tcpconnectioninformation?redirectedfrom=MSDN&view=netcore-3.1

            /// <summary>
            /// Transmit the map data to the IP address stored in <c>OtherPlayerIpAddress</c>.
            /// </summary>
            public static bool TransmitMapData(string ipAddress)
            {
                //should it only allow IPv4 or also IPv6?
                try //is it better to use Socket or TcpListiner/TcpClient?
                { // https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.networkstream?view=netcore-3.1
                    //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket?view=netcore-3.1
                    //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.sockettype?view=netcore-3.1#System_Net_Sockets_SocketType_Stream
                    //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/using-an-asynchronous-client-socket?view=netcore-3.1
                    //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/sockets?view=netcore-3.1
                    byte[] receptionAnswerByte = new byte[4];
                    short receptionAnswer = -1;

                    //contacts the receiver of the other player. 
                    TransmitSetup(ipAddress);

                    //converts the map to a string.
                    string mapData = NetworkSupport.MapToStringConvertion();

                    //open transmission
                    NetworkStream networkStream = transmitter.GetStream();

                    //transmit data so the receiver knows it is about to receive map data
                    byte[] data;
                    Converter.Conversion.ValueToBitArrayQuick(1, out data);
                    networkStream.Write(data, 0, data.Length);

                    //convert and transmit map data size.
                    byte[] mapdataByte = Converter.Conversion.ASCIIToByteArray(mapData);
                    byte[] mapdataByteSize = null;
                    Converter.Conversion.ValueToBitArrayQuick(mapdataByte.Length, out mapdataByteSize);
                    networkStream.Write(mapdataByteSize, 0, mapdataByteSize.Length);

                    Debug.WriteLine($"Transmitting map. {mapdataByte.Length} bytes");

                    //wait on answer from the receiver
                    networkStream.Read(receptionAnswerByte, 0, 4);  //the reads are used to ensure that the TransmitData is not ahead of ReceiveData. This ensures the next turn is not started before the map and chess pieces are ready.
                    Converter.Conversion.ByteConverterToInterger(receptionAnswerByte, ref receptionAnswer);
                    //should be 0.

                    //transmit the mapdataByte
                    networkStream.Write(mapdataByte, 0, mapdataByte.Length);

                    //wait on answer from the receiver. 
                    networkStream.Read(receptionAnswerByte, 0, 4); //penty of exceptions to deal with regarding .Write() and .Read()
                    //it does not seem like there is a function that allows to control whether a networkstream can write or not, so the .CanWrite() will be needed most likely. Try and find out what determine if it possible to write to a stream or not. 
                    //same for .CanRead()
                    //After the CanRead() page: "Provide the appropriate FileAccess enumerated value in the constructor to set the readability and writability of the NetworkStream.", so most likely will not have to worry about it. Can be useful for other projects
                    Converter.Conversion.ByteConverterToInterger(receptionAnswerByte, ref receptionAnswer);
                    //should be 1. 

                    //shut down transmission
                    networkStream.Close();
                    transmitter.Close();
                    return true;

                }
                catch (ObjectDisposedException e) //this should only be entered if the receiver has been shut down... I think
                {
                    Debug.WriteLine(e);
                    transmitter.Close();

                    Publishers.PubNet.TransmitAnyData(won: null, lostConnection: true, gameEnded: true);
                    return false;
                }
                catch (Exception e)
                { //what to do if this is entered?
                    Debug.WriteLine(e);

                    Publishers.PubNet.TransmitAnyData(won: null, lostConnection: true, gameEnded: true);
                    return false;
                }

            }

            /// <summary>
            /// Used to transmit integers while the chess gameplay is going on.
            /// <c>Network.Receive.ReceiveGameLoop</c> will receive the data.
            /// Will return true if it success transmitting <paramref name="data"/> to <paramref name="ipAddress"/>, else false.
            /// </summary>
            /// <param name="data"> The int value to transmit.</param>
            /// <param name="ipAddress">The IP address to transmit it too.</param>
            /// <param name="waitOnAnswer">If true, it will wait on receiving data back.</param>
            /// <returns>Returns true if it can trasmit data, else false. 
            /// If <paramref name="waitOnAnswer"/> is true, it will return true if it receives an answer back, else it will return false.</returns>
            public static bool GeneralValueTransmission(int data, string ipAddress, bool waitOnAnswer = false)
            {
                Debug.WriteLine("Data transmission: " + data);
                bool returnValue;
                try
                {
                    TransmitSetup(ipAddress);
                    Converter.Conversion.ValueToBitArrayQuick(data, out byte[] dataArray);
                    NetworkStream networkStream = transmitter.GetStream();
                    networkStream.Write(dataArray, 0, dataArray.Length);
                    if (waitOnAnswer)
                    {
                        byte[] returnData = new byte[4];
                        int answer = 0;
                        networkStream.Read(returnData, 0, 4);
                        Converter.Conversion.ByteConverterToInterger(returnData, ref answer);
                        if (answer == 1)
                            returnValue = true;
                        else
                            returnValue = false;
                    }
                    else
                        returnValue = true;
                    networkStream.Close();
                    transmitter.Close();
                    return returnValue;
                }
                catch
                {
                    returnValue = false;
                    return returnValue;
                }

            }

            /// <summary>
            /// Transmit any <paramref name="data"/> string to the IP-address stored in <c>OtherPlayerIpAddress</c>.
            /// </summary>
            /// <param name="data">String to transmit.</param>
            /// <returns>Returns true when <paramref name="data"/> has been trasmitted. </returns>
            public static bool GeneralDataTransmission(string data, string ipAddress, bool sentResponse = false)
            {
                Debug.WriteLine("Transmission: " + data);
                try
                {
                    byte[] reply = new byte[1];

                    //connect to server
                    TransmitSetup(ipAddress);
                    NetworkStream networkStream = transmitter.GetStream();

                    if (sentResponse)
                    {
                        Converter.Conversion.ValueToBitArrayQuick(2, out byte[] byteArray);
                        networkStream.Write(byteArray, 0, byteArray.Length);
                    }

                    //read data
                    networkStream.Read(reply, 0, reply.Length);

                    //transmit data string length
                    byte[] stringByte = Converter.Conversion.ASCIIToByteArray(data);
                    byte[] stringByteLengthByte;
                    Converter.Conversion.ValueToBitArrayQuick(stringByte.Length, out stringByteLengthByte);
                    networkStream.Write(stringByteLengthByte, 0, stringByteLengthByte.Length);

                    //read data
                    networkStream.Read(reply, 0, reply.Length);

                    //transmit string byte array
                    networkStream.Write(stringByte, 0, stringByte.Length);

                    //read data
                    networkStream.Read(reply, 0, reply.Length); //have while loops that runs as long time there is no data to read and if anything goes wrong can return false.

                    //shut down
                    networkStream.Close();
                    transmitter.Close();

                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return false;
                }

            }
        }

        /// <summary>
        /// Class for receiving data.
        /// </summary>
        public class Receive
        {
            //public delegate void turnChangedDel(string nogetTekst);
            //public static event turnChangedDel TurnChanged;
            /// <summary>
            /// Starts the receiver. 
            /// </summary>
            public static void Start()
            {
                Debug.WriteLine("Receiver starting up");
                receiver.Start();
                Debug.WriteLine("Receiver has started up");

                //TurnChanged?.Invoke("hello");

            }

            /// <summary>
            /// Stops the receiver.
            /// </summary>
            public static void Stop()
            {
                Debug.WriteLine("Receiver stopping");
                receiver.Stop();
                Debug.WriteLine("Receiver has stopped");
            }

            /// <summary>
            /// Waits on a client connection to the listiner and returns the IP address of the client. 
            /// If the search for a player is aborted, it will return null.
            /// </summary>
            /// <returns>Returns the IP address of the client. If the search for a player is aborted, returns null.</returns>
            public static string GetConnectionIpAddress(bool waitOnResponse = false)
            {
                bool received = true;
                while (received)
                {
                    while (!receiver.Pending() && !GameStates.NetSearch.Abort)
                    {

                    }
                    if (!GameStates.NetSearch.Abort)
                    {
                        TcpClient client = receiver.AcceptTcpClient();
                        string endPoint = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                        if (waitOnResponse)
                        {
                            NetworkStream networkStream = client.GetStream();
                            byte[] dataArray = new byte[4];
                            networkStream.Read(dataArray, 0, dataArray.Length);
                            int data = 0;
                            Converter.Conversion.ByteConverterToInterger(dataArray, ref data);
                            if (data == 1)
                                received = false;
                            else
                                received = true;
                            networkStream.Close();
                        }
                        client.Close();
                        if (received)
                        {
                            GameStates.NetSearch.Searching = false;
                            Debug.WriteLine(endPoint);
                            return endPoint;
                        }
                    }
                }
                return null;
            }

            /// <summary>
            /// Waits on a client to connect and transmit data. The data is then converted into a ASCII string. //rewrite
            /// </summary>
            /// <returns>Returns an ASCII string received from a client.</returns>
            public static string GeneralDataReception(bool WaitOnResponse = false)
            {
                /* Client connect to server. 
                 * Waits on answer from the server
                 * Client writes the string and waits on an answer
                 * Receiver reads the bytes and convert them to a string.
                 * Receiver transmit an answer and the function returns the string. 
                 * Client reads the answer and the function returns "true"
                 */
                string data = null;

                while (data == null)
                {
                    //wats on a requist for connection
                    while (!receiver.Pending()) ;
                    data = Connection(); //maybe thread this. 
                }

                return data;

                string Connection()
                {
                    try { 
                        TcpClient client = receiver.AcceptTcpClient();
                        NetworkStream networkStream = client.GetStream();
                        byte[] receivedData;

                        if (WaitOnResponse)
                        {
                            receivedData = new byte[4];
                            networkStream.Read(receivedData, 0, receivedData.Length);
                            uint response = 0;
                            Converter.Conversion.ByteConverterToInterger(receivedData, ref response);
                            Debug.WriteLine("Connection Response: " + response.ToString());
                            if (response == 1)
                                return null;
                        }

                        //writes to the client so it knows a connection has been established.
                        networkStream.Write(new byte[] { 0 }, 0, 1);

                        //reads data that states the length of the string that is going to be transmitted
                        receivedData = new byte[4];
                        networkStream.Read(receivedData, 0, receivedData.Length);
                        uint dataLength = 0;
                        Converter.Conversion.ByteConverterToInterger(receivedData, ref dataLength);
                        Debug.WriteLine("Data length is: " + dataLength);

                        //writes an answer so the transmitter knows it can transmit the string byte array.
                        networkStream.Write(new byte[] { 1 }, 0, 1);

                        //reads string data sent by the client 
                        receivedData = new byte[dataLength];
                        networkStream.Read(receivedData, 0, receivedData.Length);
                        Debug.WriteLine("Received data length is: " + receivedData.Length);

                        //converts it to a string
                        string data = Converter.Conversion.ByteArrayToASCII(receivedData);

                        Debug.WriteLine("Received: " + data);

                        //writes an answer back, so the transmitter knows it can stop.
                        networkStream.Write(new byte[] { 2 }, 0, 1);

                        //close connections
                        networkStream.Close();
                        client.Close();

                        //returns the string 
                        return data;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(""+e);
                        return "error";
                    }
                }
            }

            /// <summary>
            /// Sets up the receiver with the <paramref name="IPaddress"/>.
            /// </summary>
            /// <param name="IPaddress">IP addresss to initialise the receiver with.</param>
            /// <returns>Returns true if the receiver is initialised, else false.</returns>
            public static bool ReceiveSetup(string IPaddress, int portNumber = 23000)
            {
                try
                { //tries to start up the receiver. 
                    int port = portNumber;
                    IPAddress receiverAddress = IPAddress.Parse(IPaddress);
                    receiver = new TcpListener(receiverAddress, port);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            //public static void ReceiveSetup()
            //{ //try catch
            //    Int32 port = 23000;
            //    IPAddress receiverAddress = IPAddress.Parse("127.0.0.1"); //change later
            //    receiver = new TcpListener(receiverAddress, port);
            //    //OSSupportsIPv4 aand OSSupportsIPv6 might be useful at some points, used to check whether the underlying operating system and network adaptors support those specific IPvs
            //}

            /// <summary>
            /// Is run while the chess gameplay is going on. 
            /// What more to write?
            /// </summary>
            /// <param name="team">bool: True for white player, false for black player.</param>
            public static void ReceiveGameLoop(object team/*, TcpClient otherPlayer*/)
            {

                //Receive.TurnChanged += Receive_TurnChanged;
                List<Thread> clientThread = new List<Thread>();
                try
                {

                    while (!GameStates.GameEnded)
                    {
                        //waits on someone to connect.
                        while (!receiver.Pending())
                        {
                            if (GameStates.GameEnded) //breaks if the game ends while waiting, e.g. this player ends the game. 
                                break;
                        }
                        if (GameStates.GameEnded) //if the game has ended, no reason to do anything else of the loop. //read to see if there is a better way to do this.
                            break;
                        clientThread.Add(new Thread(new ThreadStart(NonLoopPart)));
                        clientThread[clientThread.Count - 1].Start();

                    }
                }
                catch (IOException e) //failed read, write or connection closed (forced or not forced).
                { //this should catch in case of a lost connection, but what to do? Go back to the main menu? Try some times to get a connection and it failes n times, do something else?
                    Debug.WriteLine(e); //might not be needed with the InnerException, need to read some more up on InnerException
                    Debug.WriteLine(e.InnerException); //helps with identified which specific error. 
                    //receiver.Stop();
                }
                catch (Exception e) //other.
                { //what to do. Some of the possible expections are known from testing, e.g. nulls in the new map array (caused by a bug in the code, might want to catch it anyway).
                    Debug.WriteLine(e);
                    Debug.WriteLine(e.InnerException);
                }

                void NonLoopPart()
                {
                    TcpClient otherPlayer = null;
                    NetworkStream networkStream = null;
                    try
                    {
                        byte[] tranmissionAnswerByte = new byte[4];
                        byte[] dataSizeByte = new byte[4];
                        ushort dataSize = 0;

                        //accept client and connects
                        otherPlayer = receiver.AcceptTcpClient();
                        networkStream = otherPlayer.GetStream();

                        //receive data to know what will happen. 
                        while (!networkStream.DataAvailable) ;
                        byte[] typeOfTransmission = new byte[4]; //rename
                        networkStream.Read(typeOfTransmission, 0, 4);
                        int type = 0;
                        Converter.Conversion.ByteConverterToInterger(typeOfTransmission, ref type);

                        Debug.WriteLine("Received data: " + type);

                        if (type == 1)
                        {//connected client is about to transmit map data

                            //receive the size of the mapdata string
                            while (!networkStream.DataAvailable) ;

                            networkStream.Read(dataSizeByte, 0, dataSizeByte.Length);
                            Converter.Conversion.ByteConverterToInterger(dataSizeByte, ref dataSize);
                            byte[] data = new byte[dataSize];

                            //transmit an answer, so the client knows that it can transmit the mapdata.
                            Converter.Conversion.ValueToBitArrayQuick(0, out tranmissionAnswerByte);
                            networkStream.Write(tranmissionAnswerByte, 0, tranmissionAnswerByte.Length);

                            //waits on data
                            while (!networkStream.DataAvailable) ;

                            //receive the map data
                            networkStream.Read(data, 0, dataSize); //how to ensure the data is correct? Since it is using TCP, it does not matter. Would matter if it was UDP. 

                            //decode data 
                            string mapdataString = Converter.Conversion.ByteArrayToASCII(data);
                            string[,] newMap = NetworkSupport.StringToMapConvertion(mapdataString);

                            //update the chess pieces and the map
                            //MapMatrix.UpdateOldMap(); 
                            NetworkSupport.UpdatedChessPieces(newMap, (bool)team);
                            NetworkSupport.UpdateMap(newMap);

                            //transmit an answer depending on the received data. This is needed to prevent the game from continue before the map and pieces are updated.
                            Converter.Conversion.ValueToBitArrayQuick(1, out tranmissionAnswerByte);
                            networkStream.Write(tranmissionAnswerByte, 0, tranmissionAnswerByte.Length);

                            Publishers.PubNet.IsTurn(true);
                        }
                        else if (type == 2) //draw //not really needed
                        { 
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: null);
                        }
                        else if (type == 3) //being asked for a draw
                        {
                            Console.Clear();
                            Publishers.PubNet.Pause(true);
                            string[] drawOptions = { "Accept Draw", "Decline Draw" };
                            string title = "Other play ask for draw";
                            string answer = Menu.MenuAccess(drawOptions, title);
                            if(!GameStates.LostConnection)
                                switch (answer)
                                { //the transmitter answer will need to be transmitted by the GeneralValueTransmission since the ReceiveGameLoop will receive the data
                                    case "Accept Draw":

                                        //update the turn counter if black player
                                        if (GameStates.PlayerTeam == false)
                                            GameStates.TurnCounter++;
                                        Publishers.PubNet.TransmitAnyData(gameEnded: true, won: null, otherPlayerSurrendered: true);

                                        //transmit answer
                                        Transmit.GeneralValueTransmission(30, Transmit.OtherPlayerIpAddress);
                                        break;

                                    case "Decline Draw":
                                        //transmit answer
                                        Transmit.GeneralValueTransmission(31, Transmit.OtherPlayerIpAddress);

                                        //redraw map and pieces.
                                        Console.Clear();
                                        ChessTable.RepaintBoardAndPieces();
                                        NetworkSupport.RepaintEndLocation((bool)team);
                                        break;
                                }
                            Publishers.PubNet.Pause(false);
                        }
                        else if (type == 4) //this player is victory
                        {
                            if ((bool)team == false)
                                GameStates.TurnCounter++;
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: true);
                        }
                        else if (type == 5) //this player is defeated
                        {
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: false);
                        }
                        else if (type == 6) //draw by gamerules.
                        {
                            if ((bool)team == false)
                                GameStates.TurnCounter++;
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: null);
                        }
                        else if (type == 10) //contacted to ensure there is a connection
                        {
                            Converter.Conversion.ValueToBitArrayQuick(1, out byte[] array);
                            networkStream.Write(array, 0, array.Length);
                        }
                        else if (type == 30) //draw was accepted
                        {
                            if ((bool)team == false)
                                GameStates.TurnCounter++;
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: null, pause: false);
                        }
                        else if (type == 31) //draw was denied
                        {
                            Publishers.PubNet.Pause(false);
                            Console.Clear();
                            ChessTable.RepaintBoardAndPieces();
                        }
                        else if (type == 41) //won by the other player surrendered
                        {
                            if (GameStates.PlayerTeam == false)
                                GameStates.TurnCounter++;
                            Publishers.PubNet.TransmitAnyData(gameEnded: true, won: true, otherPlayerSurrendered: true);
                        }
                         //networkStream.Flush(); //"... however, because NetworkStream is not buffered, it has no effect on network streams." - https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.networkstream.flush?view=netcore-3.1

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    finally
                    {
                        if (otherPlayer != null) //ensures the connection is closed.
                        {
                            if (networkStream != null)
                                networkStream.Close();
                            otherPlayer.Close();
                        }
                    }
                }
            }


            //private static void Receive_TurnChanged(string nogetTekst)
            //{
            //    throw new NotImplementedException();
            //}
        }

        /// <summary>
        /// Class to support the network, only used by Network. //not fully true anymore.
        /// </summary>
        public static class NetworkSupport
        {
            /// <summary>
            /// Repaints the end location(s) 
            /// </summary>
            /// <param name="team"></param>
            public static void RepaintEndLocation(bool team)
            {
                foreach (ChessPiece chePie in ChessList.GetList(team))
                {
                    if (chePie.IsSelected)
                    {
                        chePie.NetworkUpdate(repaintLocation: true);
                    }
                }
            }

            /// <summary>
            /// Gets the, internetwork, address used by the computer when contacting the router to get out on the internet. 
            /// </summary>
            /// <returns></returns>
            public static string LocalAddress
            {
                get
                {
                    string localIP;
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) //https://en.wikipedia.org/wiki/Berkeley_sockets
                    { //https://stackoverflow.com/questions/6803073/get-local-ip-address
                        socket.Connect("8.8.8.8", 65530);
                        IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                        localIP = endPoint.Address.ToString();
                    }
                    return localIP;
                }
            }

            /// <summary>
            /// Convert the MapMatrix.Map into a string. Each entry is seperated using a !. 
            /// </summary>
            /// <returns>Returns a string consisting of all MapMatrix.Map entires.</returns>
            public static string MapToStringConvertion()
            {
                string mapString = "";
                for (int x = 0; x < MapMatrix.Map.GetLength(0); x++)
                {
                    for (int y = 0; y < MapMatrix.Map.GetLength(1); y++)
                    {
                        if (y == 7 && x == 7)
                            mapString += MapMatrix.Map[x, y]; //last part of the string does not need a !, if it got one, the string will be split into 65 parts.
                        else
                            mapString += MapMatrix.Map[x, y] + "!"; //the ! is to help seperate the string into a 1d array.
                    }
                }
                return mapString;
            }

            /// <summary>
            /// Converts a string into a 2d string array, size of 8,8. Each entry is taken from <paramref name="data"/>.
            /// </summary>
            /// <param name="data">The string to convert.</param>
            /// <returns>Returns a 8,8 array of strings.</returns>
            public static string[,] StringToMapConvertion(string data)
            {
                string[] mapStringSeperated = data.Split('!');
                string[,] newMap = new string[8, 8];
                byte y = 0;
                for (int n = 0; n < mapStringSeperated.Length; n++)
                {
                    byte x = (byte)Math.Floor(n / 8d);
                    newMap[x, y] = mapStringSeperated[n];
                    y = y == 7 ? (byte)0 : (byte)(y + 1);
                }
                return newMap;
            }

            /// <summary>
            /// Find any changes between <paramref name="newMap"/> and MapMatrix.Map and updates the chess pieces accordingly. 
            /// </summary>
            /// <param name="newMap">The map, 2d array of 8,8, used to compare to MapMatrix.Map.</param>
            /// <param name="team">The team of the player that has just moved. E.g. if white moves a piece it is true. If black, false.</param>
            public static void UpdatedChessPieces(string[,] newMap, bool team)
            {
                string[,] oldMap = new string[8, 8];
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                        oldMap[x, y] = MapMatrix.Map[x, y];

                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        if (newMap[x, y] != oldMap[x, y])
                        {
                            string feltIDNew = newMap[x, y];
                            string feltIDOld = oldMap[x, y];
                            PawnPromotionCheck(feltIDNew, x, y);
                            if (feltIDNew != "" && feltIDOld == "")
                            {//a piece has moved to the, empty, location
                                if (feltIDNew.Split(':')[1] == "6") //pawn
                                {
                                    foreach (ChessPiece chePie in ChessList.GetList(!team)) //find the other player's piece that has moved.
                                    {
                                        if (chePie.GetID == feltIDNew)
                                        {
                                            if (chePie.GetMapLocation[0] != x) //en passant 
                                            {
                                                //find out the move direction and check the y in the other direction. 
                                                sbyte direction = (sbyte)(chePie.GetMapLocation[1] - y) > 0 ? (sbyte)1 : (sbyte)-1; //if positive, the pieces has moved up. If negative, it has moved down.
                                                string capturedPawn = oldMap[x, y + direction];
                                                foreach (ChessPiece chePieCaptured in ChessList.GetList(team)) //finds the player's piece that has been taken.
                                                {
                                                    if (chePieCaptured.GetID == capturedPawn)
                                                    {
                                                        chePieCaptured.NetworkUpdate(captured: true);
                                                        break;
                                                    }
                                                }
                                            }
                                            chePie.NetworkUpdate(new int[] { x, y });
                                            break;
                                        }
                                    }
                                }
                                else if (feltIDNew.Split(':')[1] == "1") //king
                                {
                                    foreach (ChessPiece chePie in ChessList.GetList(!team))
                                    {
                                        if (chePie.GetID == feltIDNew)
                                        {
                                            if (Math.Abs(chePie.GetMapLocation[0] - x) > 1) //true only if the king castled.
                                            {
                                                //rewrite the comments to be better, spent a little to much time trying to remember why it is written as it is and what is happening...
                                                //Finds direction of the castling to determine the rock. 
                                                //Checking both ends of x on the y of the king. If one of those two ends do not have a rock on the new map and there was a rock on the old map, it must be the castling rock.
                                                string leftRock = newMap[0, chePie.GetMapLocation[1]];
                                                string rightRock = newMap[7, chePie.GetMapLocation[1]];
                                                string leftRockOld = oldMap[0, chePie.GetMapLocation[1]];
                                                string chosenRock = leftRock != leftRockOld ? leftRock : rightRock; //if true, chosenRock is left rock. Else, chosenRock is right rock.
                                                //either new locations, of the rooks, will be the same as their old locations if they have not castled.
                                                //this code can only be reached if the king has moved, and moved using castling, so neither of the rocks have been moved since the last change to the map.

                                                bool chosenRockDirection = leftRock != leftRockOld ? true : false; //if true, chosenRock go left. Else, chosenRock go right
                                                foreach (ChessPiece chePieRock in ChessList.GetList(!team))
                                                {
                                                    if (chePieRock.GetID == chosenRock)
                                                    {
                                                        sbyte location = chosenRockDirection ? (sbyte)1 : (sbyte)-1; //Left rock goes to the right side of the king. Right rock goes to left side of the king. 
                                                        chePieRock.NetworkUpdate(new int[] { x + location, y });
                                                        break;
                                                    }
                                                }
                                            }
                                            chePie.NetworkUpdate(new int[] { x, y });
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
                                            chePie.NetworkUpdate(new int[] { x, y });
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (feltIDOld != "" && feltIDNew != "" && feltIDOld != feltIDNew)
                            {//a piece has been taken and a new stand on it or pawn promotion.

                                foreach (ChessPiece chePie in ChessList.GetList(team)) //find the player's piece that has been taken.
                                {
                                    if (chePie.GetID == feltIDOld)
                                    {
                                        chePie.NetworkUpdate(captured: true);
                                        break;
                                    }
                                }
                                foreach (ChessPiece chePie in ChessList.GetList(!team)) //find the other player's piece that has moved.
                                {
                                    if (chePie.GetID == feltIDNew)
                                    {
                                        chePie.NetworkUpdate(new int[] { x, y });
                                        break;
                                    }
                                }

                            }

                        }
                    }

                void PawnPromotionCheck(string feltIDNew, int x, int y)
                {
                    if (feltIDNew != "")
                        if (feltIDNew.Split(':')[2].ToCharArray().Length == 2)
                        {
                            foreach (ChessPiece chePie in ChessList.GetList(!team))
                            {
                                if (chePie.GetID.Split(':')[2] + "P" == feltIDNew.Split(':')[2] && chePie.CanBePromoted)
                                {//checks the last two parts to see if there is a pawn promotion

                                    byte[] colour = team ? Settings.BlackColour : Settings.WhiteColour;
                                    string chessNumber = feltIDNew.Split(':')[1];
                                    chePie.NetworkUpdate(captured: true);
                                    if (chessNumber == "2")
                                    {
                                        ChessList.GetList(!team).Add(new Queen(colour, !team, new int[] { x, y }, feltIDNew, Publishers.PubKey, Publishers.PubCapture));
                                    }
                                    else if (chessNumber == "3")
                                    {
                                        ChessList.GetList(!team).Add(new Bishop(colour, !team, new int[] { x, y }, feltIDNew, Publishers.PubKey, Publishers.PubCapture));
                                    }
                                    else if (chessNumber == "4")
                                    {
                                        ChessList.GetList(!team).Add(new Knight(colour, !team, new int[] { x, y }, feltIDNew, Publishers.PubKey, Publishers.PubCapture));
                                    }
                                    else if (chessNumber == "5")
                                    {
                                        ChessList.GetList(!team).Add(new Rook(colour, !team, new int[] { x, y }, feltIDNew, Publishers.PubKey, Publishers.PubCapture));
                                    }
                                    break;
                                }
                            }
                        }
                }
            }

            public static void UpdateMap(string[,] map)
            {
                MapMatrix.Map = map;
            }
        }
    }
}
