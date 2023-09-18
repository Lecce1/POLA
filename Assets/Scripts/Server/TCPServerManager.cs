using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TCPServerManager : MonoBehaviour
{
    [FoldoutGroup("정보")]
    [Title("Server IP")]
    private string ip = "15.164.215.122";  // AWS EC2 퍼블릭 IPv4 주소
    [FoldoutGroup("정보")]
    [Title("Local IP")]
    private string ip2 = "192.168.200.167"; // 집
    [FoldoutGroup("정보")]
    [Title("Local IP2")]
    private string ip3 = "192.168.0.19"; // 자취방
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

    void Start()
    {
        
    }

    public void Connect()
    {
        try
        {
            socket = new TcpClient(ip3, int.Parse(port));
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            Send("Connect", DBManager.instance.nickName);
            Debug.Log("TCP 서버 접속 완료");
            
            Thread thread = new Thread (new ThreadStart(Receive)); 			
            thread.IsBackground = true; 			
            thread.Start();  	
            
            Scene scene = SceneManager.GetActiveScene();

            switch (scene.name)
            {
                case "Title":
                    if (TitleManager.instance.error_Network.activeSelf)
                    {
                        TitleManager.instance.error_Network.SetActive(false);
                    }
                    break;
                
                case "Lobby":
                    if (LobbyManager.instance.error_Network.activeSelf)
                    {
                        LobbyManager.instance.error_Network.SetActive(false);
                    }
                    break;
            }

        }
        catch (Exception e)
        {
            Debug.Log("TCP 서버 접속 실패" + e);

            Scene scene = SceneManager.GetActiveScene();

            switch (scene.name)
            {
                case "Title":
                    if (!TitleManager.instance.error_Network.activeSelf)
                    {
                        TitleManager.instance.error_Network.SetActive(true);
                    }
                    break;
                
                case "Lobby":
                    if (!LobbyManager.instance.error_Network.activeSelf)
                    {
                        LobbyManager.instance.error_Network.SetActive(true);
                    }
                    break;
            }
            
            Invoke(nameof(Connect), 1.0f);
        }
    }
    
    void Receive() 
    { 		
        try 
        {
            Byte[] bytes = new Byte[1024];    
            
            while (true) 
            {
                using (NetworkStream stream = socket.GetStream()) 
                { 					
                    int length; 					
                    			    
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) 
                    { 						
                        var data = new byte[length]; 						
                        Array.Copy(bytes, 0, data, 0, length);
                        string msg = Encoding.ASCII.GetString(data); 						
                        Debug.Log("서버 : " + msg); 					
                    } 				
                } 			
            }         
        }         
        catch (SocketException socketException) {             
            Debug.Log("서버 오류 : " + socketException);         
        }     
    }  	
    
    public void Send(string type, string data)
    {         
        if (socket == null) 
        {
            return;         
        }  	
        
        try 
        {
            NetworkStream stream = socket.GetStream(); 		
            
            if (stream.CanWrite) 
            {          
                switch (type)
                {
                    case "Connect":
                        string msg = $"{type}|" + data;
                        byte[] bytes = Encoding.ASCII.GetBytes(msg);
                        stream.Write(bytes, 0, bytes.Length);
                        break;
                }
            }         
        } 		
        catch (SocketException socketException) {             
            Debug.Log("서버 오류: " + socketException);         
        }     
    }
}
