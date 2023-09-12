using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Random2 = System.Random;

public class TCPServer : MonoBehaviour
{
    private bool isCreate = false;
    public Text currentUser;
    public Text currentRoom;
    TcpListener server = new TcpListener(IPAddress.Any, 12345);
    public List<ServerClient> clients = new List<ServerClient>();
    List<ServerClient> disconnectClients = new List<ServerClient>();
    List<Room> rooms = new List<Room>();

    void Update()
    {
        currentUser.text = $"접속 인원 : {clients.Count}";
        currentRoom.text = $"방 갯수 : {rooms.Count}";

        if (isCreate == true)
        {
            Renewal();
            Disconnect();
            RoomRenewal();
        }
    }

    void Renewal()
    {
        foreach (ServerClient c in clients)
        {
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectClients.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();

                if (s.DataAvailable)
                {
                    string data = new StreamReader(s, true).ReadLine();
                    if (data != null)
                    {
                        OnIncomingData(c, data);
                    }
                }
            }
        }
    }

    void Disconnect()
    {
        for (int i = 0; i < disconnectClients.Count - 1; i++)
        {
            for (int j = 0; j < rooms.Count; j++)
            {
                if (rooms[j].client1 == disconnectClients[i])
                {
                    rooms[j].client1 = null;
                }
                else if (rooms[j].client2 == disconnectClients[i])
                {
                    rooms[j].client2 = null;
                }
            }

            clients.Remove(disconnectClients[i]);
            disconnectClients.RemoveAt(i);
        }
    }

    void RoomRenewal()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if ((rooms[i].client1 == null && rooms[i].client2 == null) || rooms[i].isFinish == true)
            {
                rooms.RemoveAt(i);
            }
        }
    }

    public void Create()
    {
        try
        {
            server.Start();
            StartListening();
            isCreate = true;
        }
        catch (Exception e)
        {
            Debug.Log($"Create error: {e.Message}");
        }
    }

    public void Destroy()
    {
        foreach (ServerClient c in clients)
        {
            c.tcp.Close();
        }

        try
        {
            server.Stop();
            clients.Clear();
            disconnectClients.Clear();
            rooms.Clear();
            isCreate = false;
        }
        catch (Exception e)
        {
            Debug.Log($"Create error: {e.Message}");
        }
    }

    bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();
    }

    void OnIncomingData(ServerClient c, string data)
    {
        if (data.Split('|')[0] != "Sync")
        {
            
        }

        if (data.Split('|')[0] == "Client")
        {
            c.clientName = data.Split('|')[1];
            return;
        }
        else if (data.Split('|')[0] == "Chat")
        {
            Broadcast($"{data}", clients);
        }
        else if (data.Split('|')[0] == "Matching" && data.Split('|')[2] == "True")
        {
            bool isMatching = false;

            if (rooms.Count != 0)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].currentPlayer == 1)
                    {
                        rooms[i].currentPlayer = 2;

                        if (rooms[i].client1 == null)
                        {
                            rooms[i].client1 = c;
                        }
                        else if (rooms[i].client2 == null)
                        {
                            rooms[i].client2 = c;
                        }

                        rooms[i].Start();
                        isMatching = true;
                        break;
                    }
                }

                if (isMatching == false)
                {
                    rooms.Add(new Room(c));
                }
            }
            else if (rooms.Count == 0)
            {
                rooms.Add(new Room(c));
            }
        }
        else if (data.Split('|')[0] == "Matching" && data.Split('|')[2] == "False")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1 == c || rooms[i].client2 == c)
                {
                    rooms[i].currentPlayer = rooms[i].currentPlayer - 1;

                    if (rooms[i].client1 == c)
                    {
                        rooms[i].client1 = null;
                    }
                    else if (rooms[i].client2 == c)
                    {
                        rooms[i].client2 = null;
                    }

                    break;
                }
            }
        }
        else if (data.Split('|')[0] == "CGenerate")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1 == c)
                {
                    Broadcast($"SGenerate|Client1|{rooms[i].client1.clientName}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1, rooms[i].client2);
                    break;
                }
                else if (rooms[i].client2 == c)
                {
                    Broadcast($"SGenerate|Client2|{rooms[i].client2.clientName}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1, rooms[i].client2);
                    break;
                }
            }
        }
        else if (data.Split('|')[0] == "CFade")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1 == c || rooms[i].client2 == c)
                {
                    if (data.Split('|')[1] == "Blue")
                    {
                        Broadcast($"SFade|Blue|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1, rooms[i].client2);
                        break;
                    }
                    else if (data.Split('|')[1] == "Red")
                    {
                        Broadcast($"SFade|Red|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1, rooms[i].client2);
                        break;
                    }
                }
            }
        }
        else if (data.Split('|')[0] == "CStart")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1 == c || rooms[i].client2 == c)
                {
                    if (rooms[i].isStart == false)
                    {
                        rooms[i].isStart = true;
                        break;
                    }
                }
            }
        }
        else if (data.Split('|')[0] == "Sync")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1.clientName == data.Split('|')[1] && rooms[i].currentRound == int.Parse(data.Split('|')[6]))
                {
                    Broadcast($"{data}", rooms[i].client2);
                    rooms[i].client1Pos = data;
                    rooms[i].isSync1 = true;
                    break;
                }
                else if (rooms[i].client2.clientName == data.Split('|')[1] && rooms[i].currentRound == int.Parse(data.Split('|')[6]))
                {
                    Broadcast($"{data}", rooms[i].client1);
                    rooms[i].client2Pos = data;
                    rooms[i].isSync2 = true;
                    break;
                }
            }
        }
        else if (data.Split('|')[0] == "CJump")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1 == c)
                {
                    string name = data.Split('|')[1];
                    Broadcast($"SJump|{name}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client2);
                }
                else if (rooms[i].client2 == c)
                {
                    string name = data.Split('|')[1];
                    Broadcast($"SJump|{name}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1);
                }
            }
        }
        else if (data.Split('|')[0] == "CSlide")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1 == c)
                {
                    string name = data.Split('|')[1];
                    Broadcast($"SSlide|{name}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client2);
                }
                else if (rooms[i].client2 == c)
                {
                    string name = data.Split('|')[1];
                    Broadcast($"SSlide|{name}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1);
                }
            }
        }
        else if (data.Split('|')[0] == "CBoost")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1 == c || rooms[i].client2 == c)
                {
                    string onOff = data.Split('|')[1];
                    string name = data.Split('|')[2];
                    Broadcast($"SBoost|{onOff}|{name}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1, rooms[i].client2);
                }
            }
        }
        else if (data.Split('|')[0] == "CDiamond")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1 == c || rooms[i].client2 == c)
                {
                    if (data.Split('|')[1] == "Blue")
                    {
                        rooms[i].blueScore++;
                    }
                    else if (data.Split('|')[1] == "Red")
                    {
                        rooms[i].redScore++;
                    }

                    Broadcast($"SScore|{rooms[i].blueScore}|{rooms[i].redScore}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1, rooms[i].client2);
                }
            }
        }
        else if (data.Split('|')[0] == "CAttack")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1 == c)
                {
                    string name = data.Split('|')[1];
                    Broadcast($"SAttack|{name}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client2);
                }
                else if (rooms[i].client2 == c)
                {
                    string name = data.Split('|')[1];
                    Broadcast($"SAttack|{name}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1);
                }
            }
        }
        else if (data.Split('|')[0] == "CNextRound")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1 == c || rooms[i].client2 == c)
                {
                    if (rooms[i].isNext == false)
                    {
                        rooms[i].isNext = true;
                        rooms[i].Start();
                        break;
                    }
                }
            }
        }
        else if (data.Split('|')[0] == "CGiveUp")
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].client1.clientName == data.Split('|')[1])
                {
                    Broadcast($"SGiveUp|{rooms[i].client1.clientName}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1, rooms[i].client2);
                }
                else if (rooms[i].client2.clientName == data.Split('|')[1])
                {
                    Broadcast($"SGiveUp|{rooms[i].client2.clientName}|{rooms[i].client1.clientName}|{rooms[i].client2.clientName}", rooms[i].client1, rooms[i].client2);
                }

                rooms[i].isStart = false;
                rooms[i].isFinish = true;
            }
        }
    }

    public void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (var c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.Log($"Broadcast error: {e.Message}");
            }
        }
    }

    public void Broadcast(string data, ServerClient client)
    {
        try
        {
            StreamWriter writer = new StreamWriter(client.tcp.GetStream());
            writer.WriteLine(data);
            writer.Flush();
        }
        catch (Exception e)
        {
            Debug.Log($"Broadcast error: {e.Message}");
        }
    }

    public void Broadcast(string data, ServerClient client1, ServerClient client2)
    {
        try
        {
            StreamWriter writer = new StreamWriter(client1.tcp.GetStream());
            writer.WriteLine(data);
            writer.Flush();

            StreamWriter writer2 = new StreamWriter(client2.tcp.GetStream());
            writer2.WriteLine(data);
            writer2.Flush();
        }
        catch (Exception e)
        {
            Debug.Log($"Broadcast error: {e.Message}");
        }
    }
    
    /*IEnumerator BackUp(string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("DATA", data);
        UnityWebRequest request = UnityWebRequest.Post("enddl2560.dothome.co.kr/Yeppi/BackUp.php", form);
        yield return request.SendWebRequest();
        request.Dispose();
    }*/
}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}

public class Room
{
    public bool isStart;
    public bool isNext;
    public bool isSync1;
    public bool isSync2;
    public bool isFinish;
    private bool isChariot;
    public int maxPlayer;
    public int currentPlayer;
    public int currentRound;
    public int currentMap;
    public float limitTime;
    List<int> map = new List<int>();
    public int randMap;
    List<int> trackRand = new List<int>();
    List<int> boardRand = new List<int>();
    public ServerClient client1;
    public string client1Pos;
    public ServerClient client2;
    public string client2Pos;
    public int blueRound;
    public int redRound;
    public int blueScore;
    public int redScore;
    TCPServer server = new TCPServer();
    Thread chariotThread;
    Thread diamondThread;
    Thread cannonBallThread;
    Thread ballSpawnThread;
    Thread isConnectedThread;
    Thread limitTimeThread;
    Thread statueThread;
    Random2 rand = new Random2();

    public Room(ServerClient client)
    {
        map.Add(1);
        map.Add(2);
        map.Add(3);
        map.Add(1);
        map.Add(2);
        isStart = false;
        isNext = false;
        isSync1 = false;
        isSync2 = false;
        isFinish = false;
        isChariot = false;
        maxPlayer = 2;
        currentPlayer = 1;
        currentRound = 1;
        client1 = client;
        client1Pos = null;
        client2 = null;
        client2Pos = null;
        blueRound = 0;
        redRound = 0;
        blueScore = 0;
        redScore = 0;
        chariotThread = new Thread(Chariot);
        diamondThread = new Thread(Diamond);
        cannonBallThread = new Thread(CannonBall);
        ballSpawnThread = new Thread(BallSpawn);
        isConnectedThread = new Thread(IsConnected);
        limitTimeThread = new Thread(LimitTime);
        statueThread = new Thread(Statue);
    }

    public void Start()
    {
        if (currentRound == 6 || blueRound == 3 || redRound == 3)
        {
            server.Broadcast($"SResult|{blueRound}|{redRound}|{client1.clientName}|{client2.clientName}", client1, client2);
            isFinish = true;
        }
        else
        {
            int rand = Random.Range(0, 6 - currentRound);
            currentMap = map[rand];
            map.RemoveAt(rand);
            server.Broadcast($"SStart|{currentRound}|{currentMap}|{client1.clientName}|{client2.clientName}", client1, client2);

            if (currentRound == 1)
            {
                isConnectedThread.Start();
                limitTimeThread.Start();
                statueThread.Start();
            }

            if (currentMap == 1)
            {
                if (!chariotThread.IsAlive)
                {
                    chariotThread.Start();
                }

                string text = null;

                for (int i = 0; i < 100; i++)
                {
                    trackRand.Add(Random.Range(0, 11));
                    text += trackRand[i] + "|";
                }

                server.Broadcast($"SRunning|{text}|{client1.clientName}|{client2.clientName}", client1, client2);
            }
            else if (currentMap == 2)
            {
                if (!diamondThread.IsAlive)
                {
                    diamondThread.Start();
                }

                if (!cannonBallThread.IsAlive)
                {
                    cannonBallThread.Start();
                }
            }
            else if (currentMap == 4)
            {
                if (!ballSpawnThread.IsAlive)
                {
                    ballSpawnThread.Start();
                }

                string text = null;

                for (int i = 0; i < 9; i++)
                {
                    boardRand.Add(Random.Range(0, 3));
                    text += boardRand[i] + "|";
                }

                server.Broadcast($"SBallBoard|{text}{client1.clientName}|{client2.clientName}", client1, client2);
            }
        }
    }

    void Chariot()
    {
        while (isFinish == false)
        {
            if (currentMap == 1 && isStart == true && isChariot == false)
            {
                isChariot = true;
                Delay(30000);
                server.Broadcast($"SChariot|{client1.clientName}|{client2.clientName}", client1, client2);
            }
        }
    }

    void Diamond()
    {
        while (isFinish == false)
        {
            if (currentMap == 2 && isStart == true)
            {
                Delay(5000);
                float x = rand.Next(-12, 14);
                float z = rand.Next(-12, 14);
                server.Broadcast($"SDiamond|{x}|{z}|{client1.clientName}|{client2.clientName}", client1, client2);
            }
        }
    }

    void CannonBall()
    {
        bool isRand = false;
        bool isLine = false;
        int atkSide;
        int atkCannon;

        while (isFinish == false)
        {
            if (currentMap == 2 && isStart == true)
            {
                if (isRand == false)
                {
                    isRand = true;
                    atkSide = rand.Next(0, 4);
                    atkCannon = rand.Next(0, 15);
                    server.Broadcast($"SCannonBall|Rand|{atkSide}|{atkCannon}|{client1.clientName}|{client2.clientName}", client1, client2);
                    Delay(1000);
                    isRand = false;
                }

                if (isLine == false)
                {
                    isLine = true;
                    atkSide = rand.Next(0, 4);
                    atkCannon = rand.Next(0, 2);
                    server.Broadcast($"SCannonBall|Line|{atkSide}|{atkCannon}|{client1.clientName}|{client2.clientName}", client1, client2);
                    Delay(1500);
                    isLine = false;
                }
            }
        }
    }

    void BallSpawn()
    {
        while (isFinish == false)
        {
            if (currentMap == 4 && isStart == true)
            {
                int randBall = rand.Next(0, 3);
                int randSpawn = rand.Next(0, 7);
                server.Broadcast($"SBallSpawn|{randBall}|{randSpawn}|{client1.clientName}|{client2.clientName}", client1, client2);
                Delay(2000);
            }
        }
    }

    void IsConnected()
    {
        while (isFinish == false)
        {
            Delay(1000);

            if (client1.tcp.Client == null && client2.tcp.Client == null && !client1.tcp.Client.Connected && !client2.tcp.Client.Connected)
            {
                client1 = null;
                client2 = null;
            }
            else
            {
                server.Broadcast($"Connect|{client1.clientName}|{client2.clientName}", client1, client2);
            }
        }
    }

    void LimitTime()
    {
        while (isFinish == false)
        {
            if (isStart == false)
            {
                if (currentMap == 1)
                {
                    limitTime = 200;
                }
                else if (currentMap == 2)
                {
                    limitTime = 60;
                }
                else if (currentMap == 3)
                {
                    limitTime = 30;
                }
                else if (currentMap == 4)
                {
                    limitTime = 60;
                }
            }

            while (limitTime > 0 && isStart == true)
            {
                limitTime--;
                server.Broadcast($"LimitTime|{limitTime}", client1, client2);
                Delay(1000);
            }
        }
    }

    void Statue()
    {
        while (isFinish == false)
        {
            while (isStart == true)
            {
                if (isSync1 == true && isSync2 == true)
                {
                    if (currentMap == 1)
                    {
                        if (float.Parse(client1Pos.Split('|')[3]) < -10)
                        {
                            isStart = false;
                            server.Broadcast($"SFinish|{currentRound}|Red|{client1.clientName}|{client2.clientName}", client1, client2);
                            redRound++;
                            Reset();
                        }
                        else if (float.Parse(client2Pos.Split('|')[3]) < -10)
                        {
                            isStart = false;
                            server.Broadcast($"SFinish|{currentRound}|Blue|{client1.clientName}|{client2.clientName}", client1, client2);
                            blueRound++;
                            Reset();
                        }
                        else if (limitTime <= 0)
                        {
                            isStart = false;
                            server.Broadcast($"SFinish|{currentRound}|Draw|{client1.clientName}|{client2.clientName}", client1, client2);
                            blueRound++;
                            redRound++;
                            Reset();
                        }
                    }
                    else if (currentMap == 2)
                    {
                        if (float.Parse(client1Pos.Split('|')[3]) < -10)
                        {
                            isStart = false;
                            server.Broadcast($"SFinish|{currentRound}|Red|{client1.clientName}|{client2.clientName}", client1, client2);
                            redRound++;
                            Reset();
                        }
                        else if (float.Parse(client2Pos.Split('|')[3]) < -10)
                        {
                            isStart = false;
                            server.Broadcast($"SFinish|{currentRound}|Blue|{client1.clientName}|{client2.clientName}", client1, client2);
                            blueRound++;
                            Reset();
                        }
                        else if (limitTime <= 0)
                        {
                            isStart = false;

                            if (blueScore > redScore)
                            {
                                server.Broadcast($"SFinish|{currentRound}|Blue|{client1.clientName}|{client2.clientName}", client1, client2);
                                blueRound++;
                            }
                            else if (blueScore < redScore)
                            {
                                server.Broadcast($"SFinish|{currentRound}|Red|{client1.clientName}|{client2.clientName}", client1, client2);
                                redRound++;
                            }
                            else if (blueScore == redScore)
                            {
                                server.Broadcast($"SFinish|{currentRound}|Draw|{client1.clientName}|{client2.clientName}", client1, client2);
                                blueRound++;
                                redRound++;
                            }

                            Reset();
                        }
                    }
                    else if (currentMap == 3)
                    {
                        if (float.Parse(client1Pos.Split('|')[4]) >= 450)
                        {
                            isStart = false;
                            server.Broadcast($"SFinish|{currentRound}|Blue|{client1.clientName}|{client2.clientName}", client1, client2);
                            blueRound++;
                            Reset();
                        }
                        else if (float.Parse(client2Pos.Split('|')[4]) >= 450)
                        {
                            isStart = false;
                            server.Broadcast($"SFinish|{currentRound}|Red|{client1.clientName}|{client2.clientName}", client1, client2);
                            redRound++;
                            Reset();
                        }
                        else if (limitTime <= 0)
                        {
                            isStart = false;
                            server.Broadcast($"SFinish|{currentRound}|Draw|{client1.clientName}|{client2.clientName}", client1, client2);
                            blueRound++;
                            redRound++;
                            Reset();
                        }
                    }
                    else if (currentMap == 4)
                    {
                        if (limitTime <= 0)
                        {
                            isStart = false;

                            if (blueScore > redScore)
                            {
                                server.Broadcast($"SFinish|{currentRound}|Blue|{client1.clientName}|{client2.clientName}", client1, client2);
                                blueRound++;
                            }
                            else if (blueScore < redScore)
                            {
                                server.Broadcast($"SFinish|{currentRound}|Red|{client1.clientName}|{client2.clientName}", client1, client2);
                                redRound++;
                            }
                            else if (blueScore == redScore)
                            {
                                server.Broadcast($"SFinish|{currentRound}|Draw|{client1.clientName}|{client2.clientName}", client1, client2);
                                blueRound++;
                                redRound++;
                            }

                            Reset();
                        }
                    }
                }
            }
        }
    }

    void Reset()
    {
        if (currentMap == 1)
        {
            isChariot = false;
        }

        isNext = false;
        isSync1 = false;
        isSync2 = false;
        client1Pos = null;
        client2Pos = null;
        currentRound++;
        blueScore = 0;
        redScore = 0;
        trackRand.Clear();
        boardRand.Clear();
    }

    static DateTime Delay(int ms)
    {
        DateTime ThisMoment = DateTime.Now;
        TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
        DateTime AfterWards = ThisMoment.Add(duration);

        while (AfterWards >= ThisMoment)
        {
            ThisMoment = DateTime.Now;
        }

        return DateTime.Now;
    }
}
