using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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
    [FoldoutGroup("게임 DB")] 
    [Title("현재 스테이지")]
    public int currentStageNum;
    [FoldoutGroup("스테이지 0")] 
    [Title("좌표")]
    public Vector3 stage0_Pos = new Vector3(0,0.6f,0);
    [FoldoutGroup("스테이지 1")] 
    [Title("좌표")]
    public Vector3 stage1_Pos = new Vector3(480,0.6f,0);
    [FoldoutGroup("스테이지 1")] 
    [Title("이름")]
    public string stage1_Title = "스테이지 1";
    [FoldoutGroup("스테이지 1")] 
    [Title("트랙 1번 이름")]
    public string stage1_Track1_Title = "1_1번 제목";
    [FoldoutGroup("스테이지 1")] 
    [Title("트랙 2번 이름")]
    public string stage1_Track2_Title = "1_2번 제목";
    [FoldoutGroup("스테이지 1")] 
    [Title("트랙 3번 이름")]
    public string stage1_Track3_Title = "1_3번 제목";
    [FoldoutGroup("스테이지 1")] 
    [Title("트랙 4번 이름")]
    public string stage1_Track4_Title = "1_4번 제목";
    [FoldoutGroup("스테이지 1")] 
    [Title("트랙 5번 이름")]
    public string stage1_Track5_Title = "1_5번 제목";
    [FoldoutGroup("스테이지 1")] 
    [Title("트랙 6번 이름")]
    public string stage1_Track6_Title = "1_6번 제목";
    [FoldoutGroup("스테이지 2")] 
    [Title("좌표")]
    public Vector3 stage2_Pos = new Vector3(980,0.6f,0);
    [FoldoutGroup("스테이지 2")] 
    [Title("이름")]
    public string stage2_Title = "스테이지 2";
    [FoldoutGroup("스테이지 2")] 
    [Title("트랙 1번 이름")]
    public string stage2_Track1_Title = "2_1번 제목";
    [FoldoutGroup("스테이지 2")] 
    [Title("트랙 2번 이름")]
    public string stage2_Track2_Title = "2_2번 제목";
    [FoldoutGroup("스테이지 2")] 
    [Title("트랙 3번 이름")]
    public string stage2_Track3_Title = "2_3번 제목";
    [FoldoutGroup("스테이지 2")] 
    [Title("트랙 4번 이름")]
    public string stage2_Track4_Title = "2_4번 제목";
    [FoldoutGroup("스테이지 2")] 
    [Title("트랙 5번 이름")]
    public string stage2_Track5_Title = "2_5번 제목";
    [FoldoutGroup("스테이지 2")] 
    [Title("트랙 6번 이름")]
    public string stage2_Track6_Title = "2_6번 제목";
    
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
