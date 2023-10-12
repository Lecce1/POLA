using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    [FoldoutGroup("유저 서버 DB")]
    [Title("닉네임")]
    public string nickName;
    [FoldoutGroup("유저 서버 DB")]
    [Title("코인")]
    public int coin;
    [FoldoutGroup("유저 서버 DB")]
    [Title("크리스탈")]
    public int crystal;
    [FoldoutGroup("게임 DB")]
    [Title("이동 할 씬")]
    public string nextScene;
    [FoldoutGroup("게임 DB")]
    [Title("로비 왼쪽 포탈")]
    public List<Vector3> lobbyLeftPortal = new List<Vector3>{new Vector3(460,0.6f,0), new Vector3(960,0.6f,0)};
    [FoldoutGroup("게임 DB")]
    [Title("로비 오른쪽 포탈")]
    public List<Vector3> lobbyRightPortal = new List<Vector3>{new Vector3(40,0.6f,0), new Vector3(540,0.6f,0)};
    
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

    public void Init()
    {
        nickName = String.Empty;
    }
}
