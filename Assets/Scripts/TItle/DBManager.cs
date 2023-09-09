using Sirenix.OdinInspector;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    [FoldoutGroup("유저 DB")]
    [Title("닉네임")]
    public string nickName;
    [FoldoutGroup("유저 DB")]
    [Title("코인")]
    public int coin;
    [FoldoutGroup("유저 DB")]
    [Title("크리스탈")]
    public int crystal;
    
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
