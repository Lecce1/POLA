using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;

public class TCPServerManager : MonoBehaviour
{
    [FoldoutGroup("정보")]
    [Title("Server IP")]
    private string ip = "15.164.215.122";  // 퍼블릭 IPv4 주소
    [FoldoutGroup("정보")]
    [Title("Local IP")]
    private string ip2 = "192.168.200.167";
    [FoldoutGroup("정보")]
    [Title("Port")]
    private string port = "12345";
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

    public void Connect()
    {
        try
        {
            socket = new TcpClient(ip, int.Parse(port));
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            Send("Connect", DBManager.instance.nickName);
            Debug.Log("TCP 서버 접속 완료");
            
            if (TitleManager.instance.errorNetwork.activeSelf)
            {
                TitleManager.instance.errorNetwork.SetActive(false);
            }
        }
        catch (Exception e)
        {
            Debug.Log("TCP 서버 접속 실패" + e);

            if (!TitleManager.instance.errorNetwork.activeSelf)
            {
                TitleManager.instance.errorNetwork.SetActive(true);
            }
            
            Invoke(nameof(Connect), 1.0f);
        }
    }

    public void Send(string type, string data)
    {
        switch (type)
        {
            case "Connect":
                writer.WriteLine($"{type}|" + data);
                break;
        }
        
        writer.Flush();
    }
}
