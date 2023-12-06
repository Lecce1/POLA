using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("설정")] 
    public GameObject set;
    [FoldoutGroup("패널")] 
    [Title("입장 버튼")] 
    public GameObject join_Btn;
    [FoldoutGroup("패널")] 
    [Title("스테이지 정보")] 
    public GameObject info;
    [FoldoutGroup("패널")] 
    [Title("키 바인딩")] 
    public GameObject keyBinding;
    [FoldoutGroup("패널")] 
    [Title("ESC")] 
    public GameObject esc;
    [FoldoutGroup("패널")] 
    [Title("종료")] 
    public GameObject exit;

    [FoldoutGroup("모바일")] 
    [Title("입장 버튼 텍스트")] 
    public Text join_Btn_Text;
    [FoldoutGroup("모바일")] 
    [Title("탑 패널")] 
    public GameObject top_Panel;
    [FoldoutGroup("모바일")] 
    [Title("뒤로가기 버튼")] 
    public GameObject back_Btn;
    
    [FoldoutGroup("땅")]
    [Title("땅 리스트")] 
    public List<GameObject> ground;
    [FoldoutGroup("땅")]
    [Title("스테이지 땅 리스트")] 
    public GameObject stageGround;
    [FoldoutGroup("땅")]
    [Title("스테이지 땅 머터리얼 리스트")] 
    public List<Material> stageGroundMaterial;
    [FoldoutGroup("땅")]
    [Title("스카이박스 리스트")] 
    public List<Material> stage_Skybox;
    
    [FoldoutGroup("스테이지 문")]
    [Title("문 리스트")] 
    public List<DoorManager> doorList;
    
    [FoldoutGroup("설정 패널")] 
    [Title("음악 Slider")]
    public Slider set_Music_Slider;
    [FoldoutGroup("설정 패널")] 
    [Title("효과음 Slider")]
    public Slider set_Sfx_Slider;
    [FoldoutGroup("설정 패널")] 
    [Title("진동 Toggle")]
    public Toggle set_Vibration_Toggle;
    [FoldoutGroup("설정 패널")] 
    [Title("언어 Dropdown")]
    public Dropdown set_Language_Dropdown;
    [FoldoutGroup("설정 패널")] 
    [Title("언어 왼쪽 화살표")]
    public GameObject set_Language_LeftArrow;
    [FoldoutGroup("설정 패널")] 
    [Title("언어 오른쪽 화살표")]
    public GameObject set_Language_RightArrow;

    [FoldoutGroup("땅 정보")] 
    [Title("MoveRoute 클래스 리스트")]
    public MoveRoute[] moveRoute = new MoveRoute[3];
    [FoldoutGroup("땅 정보")] 
    [Title("플레이어 이동 오프셋")]
    public Vector3 offset = Vector3.up * 0.5f;
    
    [FoldoutGroup("기타")] 
    [Title("입장 버튼 타입")]
    public string join_Btn_Type;
    [FoldoutGroup("기타")] 
    [Title("입장 버튼 온오프 여부")]
    public bool isJoinBtnOn;
    [FoldoutGroup("기타")] 
    [Title("패널 열림 여부")]
    public bool isPanelOpen;
    [FoldoutGroup("기타")] 
    [Title("Info 패널 온오프 여부")] 
    [SerializeField]
    public bool isInfoPanelOn;
    [FoldoutGroup("기타")] 
    [Title("설정 버튼 / 패드 여부")]
    public bool isSetBtn;
    [FoldoutGroup("기타")] 
    [Title("Virtual Camera")]
    public CinemachineVirtualCamera virtualCamera;
    [FoldoutGroup("기타")] 
    [Title("플레이 버튼 Mesh Renderer")]
    public MeshRenderer playBtnMeshRenderer;
    
    [FoldoutGroup("챕터")] 
    [Title("BGM 리스트")]
    public AudioClip[] bgm = new AudioClip[9];
    [FoldoutGroup("챕터")] 
    [Title("스테이지 별 이미지 리스트")]
    public List<Material> stageImageList = new List<Material>();

    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static LobbyManager instance;

    [Serializable]
    public class MoveRoute
    {
        public List<GameObject> routeList;
        public int defaultRouteIdx;
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        backStack = new Stack<GameObject>();
        Application.targetFrameRate = 240;
    }

    void Start()
    {
        StartCoroutine("Init");
    }

    IEnumerator Init()
    {
        while (!DBManager.instance.isJsonLoad)
        {
            yield return null;
        }

        for (int i = 0; i < ground.Count; i++)
        {
            if (i == DBManager.instance.currentGround)
            {
                if (!ground[i].activeSelf)
                {
                    ground[i].SetActive(true);
                }
            }
            else
            {
                if (ground[i].activeSelf)
                {
                    ground[i].SetActive(false);
                }
            }
        }

        playBtnMeshRenderer.material = stageImageList[DBManager.instance.currentChapter - 1];
        LobbyPlayerController.instance.player.transform.position = moveRoute[DBManager.instance.currentGround].routeList[DBManager.instance.currentRouteIdx].transform.position + offset;
        
        if (DBManager.instance.currentGround == 0 || DBManager.instance.currentGround == 1)
        {
            RenderSettings.skybox = stage_Skybox[0];

            if (LobbyAudioManager.instance.bgmAudio.clip != bgm[0])
            {
                if (LobbyAudioManager.instance.bgmAudio.isPlaying)
                {
                    LobbyAudioManager.instance.bgmAudio.Stop();
                }
                        
                LobbyAudioManager.instance.bgmAudio.clip = bgm[0];
                LobbyAudioManager.instance.bgmAudio.Play();
            }
        }
        else if (DBManager.instance.currentGround == 2)
        {
            for (int i = 0; i < doorList.Count; i++)
            {
                doorList[i].Stage_Init();
            }
            
            RenderSettings.skybox = stage_Skybox[DBManager.instance.currentChapter];
            
            for (int i = 0; i < stageGround.transform.childCount; i++)
            {
                stageGround.transform.GetChild(i).GetComponent<MeshRenderer>().material = stageGroundMaterial[DBManager.instance.currentChapter];
            }
            
            if (LobbyAudioManager.instance.bgmAudio.isPlaying)
            {
                LobbyAudioManager.instance.bgmAudio.Stop();
            }
                    
            LobbyAudioManager.instance.bgmAudio.clip = bgm[DBManager.instance.currentChapter];
            LobbyAudioManager.instance.bgmAudio.Play();
        }
    }

    public void DoorInit(string type, string btnText)
    {
        join_Btn_Type = type;
        join_Btn.GetComponent<Button>().onClick.RemoveAllListeners();
        join_Btn.GetComponent<Button>().onClick.AddListener(() => LobbyManager.instance.Button(type));
        join_Btn_Text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Lobby", btnText, LocalizationSettings.SelectedLocale);
        Join_Btn_OnOff(true, false);
    }

    public void Join_Btn_OnOff(bool isOn, bool isInfo)
    {
        if (DBManager.instance.currentPlatform != "MOBILE" && join_Btn.GetComponent<RectTransform>().anchoredPosition.y == 0)
        {
            join_Btn.GetComponent<Animator>().Play("JoinOff");
        }
        
        if (isOn)
        {
            if (join_Btn.activeSelf && DBManager.instance.currentPlatform == "MOBILE")
            {
                join_Btn.GetComponent<Animator>().Play("JoinOn");
            }
            
            isJoinBtnOn = true;
        }
        else if (!isOn)
        {
            if (join_Btn.activeSelf && DBManager.instance.currentPlatform == "MOBILE")
            {
                join_Btn.GetComponent<Animator>().Play("JoinOff");
            }

            if (!isInfo)
            {
                isJoinBtnOn = false;
            }
        }
    }

    public void Button(string type)
    {
        if (DBManager.instance.currentPlatform == "MOBILE" && (join_Btn.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("LobbyJoinOff") ||
            join_Btn.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1f))
        {
            return;
        }
        
        switch (type)
        {
            case "Move":
                LobbyAudioManager.instance.PlayAudio("Button");
                DBManager.instance.currentGround++;
                
                for (int i = 0; i < ground.Count; i++)
                {
                    if (i == DBManager.instance.currentGround)
                    {
                        if (!ground[i].activeSelf)
                        {
                            ground[i].SetActive(true);
                        }
                    }
                    else
                    {
                        if (ground[i].activeSelf)
                        {
                            ground[i].SetActive(false);
                        }
                    }
                }

                if (DBManager.instance.currentGround == 1)
                {
                    moveRoute[DBManager.instance.currentGround].defaultRouteIdx = DBManager.instance.currentChapter - 1;
                    RenderSettings.skybox = stage_Skybox[0];

                    if (LobbyAudioManager.instance.bgmAudio.clip != bgm[0])
                    {
                        if (LobbyAudioManager.instance.bgmAudio.isPlaying)
                        {
                            LobbyAudioManager.instance.bgmAudio.Stop();
                        }
                        
                        LobbyAudioManager.instance.bgmAudio.clip = bgm[0];
                        LobbyAudioManager.instance.bgmAudio.Play();
                    }
                }
                else if (DBManager.instance.currentGround == 2)
                {
                    for (int i = 0; i < doorList.Count; i++)
                    {
                        doorList[i].Stage_Init();
                    }
                    
                    RenderSettings.skybox = stage_Skybox[DBManager.instance.currentChapter];

                    for (int i = 0; i < stageGround.transform.childCount; i++)
                    {
                        stageGround.transform.GetChild(i).GetComponent<MeshRenderer>().material = stageGroundMaterial[DBManager.instance.currentChapter];
                    }
                    
                    if (LobbyAudioManager.instance.bgmAudio.isPlaying)
                    {
                        LobbyAudioManager.instance.bgmAudio.Stop();
                    }
                    
                    LobbyAudioManager.instance.bgmAudio.clip = bgm[DBManager.instance.currentChapter];
                    LobbyAudioManager.instance.bgmAudio.Play();
                }

                if (DBManager.instance.currentPlatform == "MOBILE")
                {
                    if (!back_Btn.activeSelf)
                    {
                        back_Btn.SetActive(true);
                    }
                }
                
                DBManager.instance.currentRouteIdx = moveRoute[DBManager.instance.currentGround].defaultRouteIdx;
                LobbyPlayerController.instance.transform.position = moveRoute[DBManager.instance.currentGround].routeList[moveRoute[DBManager.instance.currentGround].defaultRouteIdx].transform.position + offset;
                LobbyPlayerController.instance.StartCoroutine("Move");
                break;
            
            case "Shop":
                LobbyAudioManager.instance.PlayAudio("Button");
                DBManager.instance.currentGround = 3;
                
                for (int i = 0; i < ground.Count; i++)
                {
                    if (i == DBManager.instance.currentGround)
                    {
                        if (!ground[i].activeSelf)
                        {
                            ground[i].SetActive(true);
                        }
                    }
                    else
                    {
                        if (ground[i].activeSelf)
                        {
                            ground[i].SetActive(false);
                        }
                    }
                }
                
                if (DBManager.instance.currentPlatform == "MOBILE")
                {
                    if (!back_Btn.activeSelf)
                    {
                        back_Btn.SetActive(true);
                    }
                }
                
                DBManager.instance.currentRouteIdx = moveRoute[DBManager.instance.currentGround].defaultRouteIdx;
                LobbyPlayerController.instance.transform.position = moveRoute[DBManager.instance.currentGround].routeList[moveRoute[DBManager.instance.currentGround].defaultRouteIdx].transform.position + offset;
                LobbyPlayerController.instance.StartCoroutine("Move");
                break;
            
            case "Back":
                if (DBManager.instance.currentGround == 3)
                {
                    DBManager.instance.currentGround = 0;
                    DBManager.instance.currentRouteIdx = 0;
                }
                else if (DBManager.instance.currentGround != 0)
                {
                    DBManager.instance.currentGround--;

                    if (DBManager.instance.currentGround == 0)
                    {
                        playBtnMeshRenderer.material = stageImageList[DBManager.instance.currentChapter - 1];
                        DBManager.instance.currentRouteIdx = 3;
                        
                        if (DBManager.instance.currentPlatform == "MOBILE")
                        {
                            if (back_Btn.activeSelf)
                            {
                                back_Btn.SetActive(false);
                            }
                        }
                    }
                    else if (DBManager.instance.currentGround == 1)
                    {
                        DBManager.instance.currentRouteIdx = DBManager.instance.currentChapter - 1;
                    }
                }
                
                for (int i = 0; i < ground.Count; i++)
                {
                    if (i == DBManager.instance.currentGround)
                    {
                        if (!ground[i].activeSelf)
                        {
                            ground[i].SetActive(true);
                        }
                    }
                    else
                    {
                        if (ground[i].activeSelf)
                        {
                            ground[i].SetActive(false);
                        }
                    }
                }
                
                LobbyPlayerController.instance.transform.position = moveRoute[DBManager.instance.currentGround].routeList[DBManager.instance.currentRouteIdx].transform.position + offset;

                if (DBManager.instance.currentGround == 1)
                {
                    RenderSettings.skybox = stage_Skybox[0];
                        
                    if (LobbyAudioManager.instance.bgmAudio.isPlaying)
                    {
                        LobbyAudioManager.instance.bgmAudio.Stop();
                    }

                    if (LobbyAudioManager.instance.bgmAudio.clip != bgm[0])
                    {
                        LobbyAudioManager.instance.bgmAudio.clip = bgm[0];
                        LobbyAudioManager.instance.bgmAudio.Play();
                    }
                }
                
                LobbyAudioManager.instance.PlayAudio("Button");
                break;
            
            case "Stage":
                if (!info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("InfoOff") || info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    if (!isInfoPanelOn)
                    {
                        Info_OnOff(true);
                        Join_Btn_OnOff(false, true);
                    }
                }
                break;
            
            case "Info_Yes":
                LobbyPlayerController.instance.isMoveAvailable = false;
                LobbyAudioManager.instance.PlayAudio("Button");
                StartCoroutine(FadeManager.instance.FadeIn());
                DBManager.instance.nextScene = DBManager.instance.gameSceneName;
                break;
            
            case "Info_No":
                if (info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("InfoOn"))
                {
                    if (info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        Info_OnOff(false);
                    }
                }
                break;
            
            case "Esc":
                if (!esc.activeSelf)
                {
                    esc.SetActive(true);
                    backStack.Push(esc);
                    isPanelOpen = true;

                    if (join_Btn.activeSelf)
                    {
                        join_Btn.SetActive(false);
                    }

                    if (keyBinding.activeSelf)
                    {
                        keyBinding.SetActive(false);
                    }
                    
                    LobbyAudioManager.instance.PlayAudio("Button");
                }
                break;
            
            case "Latency":
                LobbyPlayerController.instance.isMoveAvailable = false;
                LobbyAudioManager.instance.PlayAudio("Button");
                StartCoroutine(FadeManager.instance.FadeIn());
                DBManager.instance.nextScene = DBManager.instance.latencySceneName;
                break;

            case "Set":
                if (!set.activeSelf)
                {
                    if (esc.activeSelf)
                    {
                        esc.SetActive(false);
                    }

                    set_Music_Slider.value = DBManager.instance.musicValue;
                    set_Sfx_Slider.value = DBManager.instance.sfxValue;
                    set_Vibration_Toggle.isOn = DBManager.instance.isVibration;
                    set_Language_Dropdown.value = DBManager.instance.language;
                    Set_Change("Language");
                    set.SetActive(true);
                    backStack.Push(set);
                    isPanelOpen = true;

                        
                    if (join_Btn.activeSelf)
                    {
                        join_Btn.SetActive(false);
                    }

                    if (keyBinding.activeSelf)
                    {
                        keyBinding.SetActive(false);
                    }
                    
                    LobbyAudioManager.instance.PlayAudio("Button");
                }
                break;

            case "Exit":
                if (!exit.activeSelf)
                {
                    if (esc.activeSelf)
                    {
                        esc.SetActive(false);
                    }
                    
                    exit.SetActive(true);
                    backStack.Push(exit);
                    isPanelOpen = true;
                    
                    if (keyBinding.activeSelf)
                    {
                        keyBinding.SetActive(false);
                    }
                    
                    LobbyAudioManager.instance.PlayAudio("Button");
                }
                break;
            
            case "Exit_Yes":
                if (exit.activeSelf)
                {
                    LobbyAudioManager.instance.PlayAudio("Button");
                    Application.Quit();
                }
                break;
            
            case "Exit_No":
                if (exit.activeSelf)
                {
                    Back();
                }
                break;
        }
    }
    
    public void Info_OnOff(bool isOn)
    {
        LobbyAudioManager.instance.PlayAudio("Button");
        
        if (isOn)
        {
            if (DBManager.instance.currentPlatform == "MOBILE" && top_Panel.activeSelf)
            {
                top_Panel.SetActive(false);
            }
            
            TrackInfo.instance.Init();
            
            if (LobbyAudioManager.instance.bgmAudio.isPlaying)
            {
                LobbyAudioManager.instance.bgmAudio.Stop();
            }
            
            LobbyAudioManager.instance.bgmAudio.clip = DBManager.instance.stageArray[DBManager.instance.currentChapter].stage[DBManager.instance.currentStage - 1].audio;
            LobbyAudioManager.instance.bgmAudio.Play();
            info.GetComponent<Animator>().Play("InfoOn");
            isInfoPanelOn = true;
            keyBinding.SetActive(false);
            LobbyPlayerController.instance.isMoveAvailable = false;
            StartCoroutine("VirtualCameraOffset", true);
        }
        else if (!isOn)
        {
            if (DBManager.instance.currentPlatform == "MOBILE" && !top_Panel.activeSelf)
            {
                top_Panel.SetActive(true);
            }
            
            if (LobbyAudioManager.instance.bgmAudio.isPlaying)
            {
                LobbyAudioManager.instance.bgmAudio.Stop();
            }
            
            LobbyAudioManager.instance.bgmAudio.clip = bgm[DBManager.instance.currentChapter];
            LobbyAudioManager.instance.bgmAudio.Play();
            info.GetComponent<Animator>().Play("InfoOff");
            isInfoPanelOn = false;
            keyBinding.SetActive(true);
            LobbyPlayerController.instance.isMoveAvailable = true;
            StartCoroutine("VirtualCameraOffset", false);
        }
    }

    IEnumerator VirtualCameraOffset(bool isOn)
    {
        if (isOn)
        {
            float temp = -4.8f;
        
            while (virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z < -4f)
            {
                temp += Time.deltaTime;
                virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, 1.8f, temp);
                yield return null;
            }
        }
        else if (!isOn)
        {
            float temp = -4f;
        
            while (virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z > -4.8f)
            {
                temp -= Time.deltaTime;
                virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, 1.8f, temp);
                yield return null;
            }
        }

    }

    public void Set_Change(string type)
    {
        switch (type)
        {
            case "Music":
                DBManager.instance.musicValue = set_Music_Slider.value;
                LobbyAudioManager.instance.audioMixer.SetFloat("Music", DBManager.instance.musicValue * 80 - 80);
                break;
            
            case "Sfx":
                DBManager.instance.sfxValue = set_Sfx_Slider.value;
                LobbyAudioManager.instance.audioMixer.SetFloat("FX", DBManager.instance.sfxValue * 80 - 80);
                break;
            
            case "Vibration":
                DBManager.instance.isVibration = set_Vibration_Toggle.isOn;
                break;
            
            case "Language":
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[set_Language_Dropdown.value];
                DBManager.instance.language = set_Language_Dropdown.value;

                if (set_Language_Dropdown.value == 0)
                {
                    set_Language_LeftArrow.SetActive(false);
                    set_Language_RightArrow.SetActive(true);
                }
                else
                {
                    set_Language_LeftArrow.SetActive(true);
                    set_Language_RightArrow.SetActive(false);
                }
                break;
        }
    }

    public void Back()
    {
        if (backStack.Count <= 0)
        {
            Debug.LogError("뒤로가기 패널이 비었습니다.");
            return;
        }

        LobbyAudioManager.instance.PlayAudio("Button");
        bool isCheck = false;
        
        switch (backStack.Pop().name)
        {
            case "Esc":
                if (esc.activeSelf)
                {
                    esc.SetActive(false);
                    isCheck = true;
                }
                break;
            
            case "Set":
                if (set.activeSelf)
                {
                    set.SetActive(false);
                    isCheck = true;
                }
                break;
            
            case "Exit":
                if (exit.activeSelf)
                {
                    exit.SetActive(false);
                    isCheck = true;
                }
                break;
        }

        if (isCheck)
        {
            isPanelOpen = false;
                    
            if (join_Btn.activeSelf == false)
            {
                join_Btn.SetActive(true);
                isSetBtn = false;
            }
            
            if (!keyBinding.activeSelf)
            {
                keyBinding.SetActive(true);
            }
        }
    }
}
