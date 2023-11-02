using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;

public class DBManager : MonoBehaviour
{
    [Title("언어")] 
    public int language;
    [Title("현재 플랫폼")]
    public string currentPlatform;
    [Title("게임 씬 이름")]
    public string gameSceneName = "Game";
    [Title("로비 씬 현재 스테이지")]
    public int lobbyCurrentStage = 0;
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
    public List<Vector3> lobbyLeftPortal = new List<Vector3>{new Vector3(460,0.6f,0), new Vector3(960,0.6f,0), new Vector3(1460,0.6f,0), new Vector3(1960,0.6f,0), new Vector3(2460,0.6f,0), new Vector3(2960,0.6f,0),new Vector3(3460,0.6f,0) ,new Vector3(3960,0.6f,0)};
    [FoldoutGroup("게임 DB")]
    [Title("로비 오른쪽 포탈")]
    public List<Vector3> lobbyRightPortal = new List<Vector3>{new Vector3(40,0.6f,0), new Vector3(540,0.6f,0), new Vector3(1040,0.6f,0), new Vector3(1540,0.6f,0), new Vector3(2040,0.6f,0), new Vector3(2540,0.6f,0), new Vector3(3040,0.6f,0), new Vector3(3540,0.6f,0)};
    [FoldoutGroup("게임 DB")] 
    [Title("현재 스테이지")]
    public int currentStageNum;
    [FoldoutGroup("게임 DB")] 
    [Title("지원 언어 갯수")]
    public int supportLanguageNum = 2;
    [FoldoutGroup("스테이지")] 
    [Title("스테이지 좌표 리스트")]
    public List<Vector3> stage_Pos = new List<Vector3>{new Vector3(0,0.6f,0), new Vector3(480,0.6f,0), new Vector3(980,0.6f,0), new Vector3(1480,0.6f,0), new Vector3(1980,0.6f,0), new Vector3(2480,0.6f,0), new Vector3(2980,0.6f,0), new Vector3(3480,0.6f,0)};
    

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

    void Start()
    {
        Init();
    }

    public void Init()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;
 
        switch(systemLanguage)
        {
            case SystemLanguage.English:
                language = 0;
                break;
            
            case SystemLanguage.Korean:
                language = 1;
                break;
        }

        if (language != null)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[language];
        }
        
        nickName = String.Empty;
    }
}
