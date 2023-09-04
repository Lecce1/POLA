using Sirenix.OdinInspector;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    [FoldoutGroup("유저 DB")]
    [Title("닉네임")]
    public string nickName;
    
    public static DBManager instance;
    
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
