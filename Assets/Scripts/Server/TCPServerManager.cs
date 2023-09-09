using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using Sirenix.OdinInspector;
using UnityEngine;

public class TCPServerManager : MonoBehaviour
{
    [FoldoutGroup("서버")]
    [Title("소켓")]
    private TcpClient socket;
    [FoldoutGroup("서버")]
    [Title("스트림")]
    public NetworkStream stream;
    [FoldoutGroup("서버")]
    [Title("읽기")]
    public StreamWriter writer;
    [FoldoutGroup("서버")]
    [Title("쓰기")]
    public StreamReader reader;
    
    public static TCPServerManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void ServerConnect()
    {
        try
        {
            socket = new TcpClient("192.168.200.167", 12345);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            Debug.Log("서버 접속 완료");
        }
        catch (Exception e)
        {
            Debug.Log("서버 접속 실패" + e);
            Invoke("ServerConnect", 1.0f);
        }
    }
}
