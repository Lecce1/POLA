using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;

public class DBManager : MonoBehaviour
{
    [FoldoutGroup("유저 서버 DB")]
    [Title("닉네임")]
    public string nickName;
    [FoldoutGroup("게임 DB")]
    [Title("이동 할 씬")]
    public string nextScene;
    [FoldoutGroup("게임 DB")] 
    [Title("현재 땅")]
    public int currentGround;
    [FoldoutGroup("게임 DB")] 
    [Title("현재 루트 인덱스")]
    public int currentRouteIdx;
    [FoldoutGroup("게임 DB")] 
    [Title("현재 챕터")]
    public int currentChapter;
    [FoldoutGroup("게임 DB")] 
    [Title("현재 스테이지")]
    public int currentStage;
    [FoldoutGroup("게임 DB")] 
    [Title("현재 플랫폼")]
    public string currentPlatform;
    [FoldoutGroup("게임 DB")] 
    [Title("지원 언어 갯수")]
    public int supportLanguageNum = 2;
    [FoldoutGroup("게임 DB")] 
    [Title("튜토리얼 여부")] 
    public bool isTutorial;
    [FoldoutGroup("게임 DB")] 
    [Title("언어")] 
    public int language;
    [FoldoutGroup("게임 DB")] 
    [Title("게임 씬 이름")]
    public string gameSceneName = "Game";
    [FoldoutGroup("게임 DB")] 
    [Title("시네머신 여부")]
    public bool isCinemachine;
    
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
