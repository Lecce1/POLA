using UnityEngine;

public class TCPServerManager : MonoBehaviour
{
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
}
