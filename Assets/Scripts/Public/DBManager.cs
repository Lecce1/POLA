using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

[Serializable]
class LocalData
{
    public int chapter = 1;
    public int currentChapter = 1;
    public float musicValue;
    public float sfxValue;
    public int supportLanguageNum = 2;
    public bool isVibration;
    public bool isKeysound;
    public int language;
    public bool isTutorial;
    public int latency;
    public StageArray[] stageArray = new StageArray[8];
}

[Serializable]
public class StageArray
{
    public Stage[] stage = new Stage[6];
}

[Serializable]
public class Stage
{
    public string name;
    public int starCount;
    public int score;
    public int perfect;
    public int great;
    public int good;
    public int miss;
    public int bpm;
    public AudioClip audio;
    public Material polaroid;
}

public class DBManager : MonoBehaviour
{
    [FoldoutGroup("유저 서버 DB")]
    [Title("닉네임")]
    public string nickName;
    [FoldoutGroup("게임 DB")]
    [Title("챕터 값")]
    public int chapter = 1;
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
    [Title("튜토리얼 여부")] 
    public bool isTutorial;
    [FoldoutGroup("게임 DB")] 
    [Title("시네머신 여부")]
    public bool isCinemachine;
    [FoldoutGroup("게임 DB")] 
    [Title("레이턴시 값")]
    public int latency;
    [FoldoutGroup("게임 DB")] 
    [Title("JSON 불러오기 여부")]
    public bool isJsonLoad = false;
    [FoldoutGroup("게임 DB")] 
    [Title("레이턴시 씬 이름")]
    public string lobbySceneName = "Lobby";
    [FoldoutGroup("게임 DB")] 
    [Title("게임 씬 이름")]
    public string gameSceneName = "Game";
    [FoldoutGroup("게임 DB")] 
    [Title("레이턴시 씬 이름")]
    public string latencySceneName = "Latency";
    [FoldoutGroup("게임 DB")] 
    [Title("스테이지 별 DB")]
    public StageArray[] stageArray = new StageArray[8];
    
    [FoldoutGroup("설정 DB")] 
    [Title("설정 음악 값")]
    public float musicValue = 1;
    [FoldoutGroup("설정 DB")] 
    [Title("설정 효과음 값")]
    public float sfxValue = 1;
    [FoldoutGroup("설정 DB")]
    [Title("설정 진동 여부")]
    public bool isVibration = true;
    [FoldoutGroup("설정 DB")]
    [Title("설정 키음 여부")]
    public bool isKeysound = true;
    [FoldoutGroup("설정 DB")] 
    [Title("언어")] 
    public int language;
    [FoldoutGroup("설정 DB")] 
    [Title("지원 언어 갯수")]
    public int supportLanguageNum = 2;
    [FoldoutGroup("설정 DB")] 
    [Title("json 주소")]
    public string jsonPath;
    
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

    void Init()
    {
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            jsonPath = Path.Combine(Application.persistentDataPath, "database.json");
        }
        else 
        {
            jsonPath = Path.Combine(Application.dataPath, "database.json"); 
        }

        JsonLoad();
        nickName = String.Empty;
    }
    
    void JsonLoad() 
    {
        LocalData localData = new LocalData();
        
        if (!File.Exists(jsonPath)) 
        {
            Reset();
        } 
        else 
        {
            localData = JsonUtility.FromJson<LocalData>(File.ReadAllText(jsonPath));

            if (localData != null)
            {
                isTutorial = localData.isTutorial;
                chapter = localData.chapter;
                currentChapter = localData.currentChapter;
                musicValue = localData.musicValue;
                sfxValue = localData.sfxValue;
                isVibration = localData.isVibration;
                isKeysound = localData.isKeysound;
                language = localData.language;
                supportLanguageNum = localData.supportLanguageNum;
                latency = localData.latency;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[language];

                for (int i = 1; i < localData.stageArray.Length; i++)
                {
                    for (int j = 0; j < localData.stageArray[i].stage.Length; j++)
                    {
                        if (localData.stageArray[i] != null)
                        {   
                            stageArray[i].stage[j].starCount = localData.stageArray[i].stage[j].starCount;
                            stageArray[i].stage[j].score =localData.stageArray[i].stage[j].score;
                            stageArray[i].stage[j].perfect= localData.stageArray[i].stage[j].perfect;
                            stageArray[i].stage[j].great= localData.stageArray[i].stage[j].great;
                            stageArray[i].stage[j].good = localData.stageArray[i].stage[j].good;
                            stageArray[i].stage[j].miss = localData.stageArray[i].stage[j].miss;
                        }
                    }
                }
            }
        }

        isJsonLoad = true;
    }

    void JsonSave() 
    {
        LocalData localData = new LocalData();
        localData.isTutorial = isTutorial;
        localData.chapter = chapter;
        localData.currentChapter = currentChapter;
        localData.musicValue = musicValue;
        localData.sfxValue = sfxValue;
        localData.isVibration = isVibration;
        localData.isKeysound = isKeysound;
        localData.language = language;
        localData.supportLanguageNum = supportLanguageNum;
        localData.latency = latency;

        for (int i = 1; i < localData.stageArray.Length; i++)
        {
            localData.stageArray[i] = new StageArray();
            
            for (int j = 0; j < 6; j++)
            {
                localData.stageArray[i].stage[j] = new Stage();
            }
            
            for (int j = 0; j < localData.stageArray[i].stage.Length; j++)
            {
                if (localData.stageArray[i] != null && localData.stageArray[i].stage[j].score < stageArray[i].stage[j].score)
                {
                    localData.stageArray[i].stage[j].starCount = stageArray[i].stage[j].starCount;
                    localData.stageArray[i].stage[j].score = stageArray[i].stage[j].score;
                    localData.stageArray[i].stage[j].perfect = stageArray[i].stage[j].perfect;
                    localData.stageArray[i].stage[j].great = stageArray[i].stage[j].great;
                    localData.stageArray[i].stage[j].good = stageArray[i].stage[j].good;
                    localData.stageArray[i].stage[j].miss = stageArray[i].stage[j].miss;
                }
            }
        }
        
        string json = JsonUtility.ToJson(localData, true);
        File.WriteAllText(jsonPath, json);
    }

    void Reset()
    {
        isTutorial = true;
        chapter = 2;
        musicValue = 1;
        sfxValue = 1;
        isVibration = true;
        isKeysound = true;
        latency = 0;
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

        supportLanguageNum = language;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[language];
        JsonSave();
    }

    void OnApplicationQuit()
    {
        if (currentPlatform == "CONSOLE")
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }

        JsonSave();
    }
}
