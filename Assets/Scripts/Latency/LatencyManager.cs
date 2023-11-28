using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LatencyManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("시작 패널")]
    public GameObject startPanel;
    [FoldoutGroup("패널")] 
    [Title("결과 패널")]
    public GameObject result;
    [FoldoutGroup("패널")] 
    [Title("결과 오프셋 Title")]
    public Text offsetTitleText;
    [FoldoutGroup("패널")] 
    [Title("결과 오프셋 Bottom 패널")] 
    public GameObject offsetBottomPanel;
    [FoldoutGroup("패널")] 
    [Title("레이턴시 텍스트")]
    [SerializeField]
    private Text latencyText;
    [FoldoutGroup("패널")] 
    [Title("Press 텍스트")]
    [SerializeField]
    private Text pressText;
    [FoldoutGroup("패널")] 
    [Title("일시정지")] 
    public GameObject esc;
    [FoldoutGroup("패널")] 
    [Title("설정")] 
    public GameObject set;
    [FoldoutGroup("패널")] 
    [Title("상단 패널")] 
    public GameObject topPanel;
    [FoldoutGroup("패널")] 
    [Title("하단 패널")] 
    public GameObject bottomPanel;
    
    [FoldoutGroup("오브젝트")] 
    public GameObject[] latencyNoteList = new GameObject[30];
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject latencyNote;
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject latencyFolder;
    
    [FoldoutGroup("설정 패널")] 
    [Title("음악 Slider")]
    public Slider set_Music_Slider;
    [FoldoutGroup("설정 패널")] 
    [Title("효과음 Slider")]
    public Slider set_Sfx_Slider;
    [FoldoutGroup("설정 패널")] 
    [Title("진동 Toggle")]
    public Toggle set_Vibration_Toggle;
    
    [FoldoutGroup("기타")] 
    [Title("플레이어")] 
    public LatencyPlayerController latencyPlayerController;
    [FoldoutGroup("기타")] 
    [Title("시작 여부")] 
    public bool isStart;
    [FoldoutGroup("기타")] 
    [Title("종료 여부")] 
    public bool isFinish;
    [FoldoutGroup("기타")] 
    [Title("결과 패널 활성화 여부")] 
    public bool isResultPanelOpen;
    [FoldoutGroup("기타")] 
    [Title("패널 열림 여부")] 
    [SerializeField]
    public bool isPanelOpen;
    
    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static LatencyManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        backStack = new Stack<GameObject>();
    }

    void Start()
    {
        StartCoroutine(nameof(Init));
    }

    IEnumerator Init()
    {
        if (!startPanel.activeSelf)
        {
            startPanel.SetActive(true);
        }

        if (bottomPanel.activeSelf)
        {
            bottomPanel.SetActive(false);
        }
        
        for (int i = 1; i <= latencyNoteList.Length; i++)
        {
            var obj = Instantiate(latencyNote, latencyFolder.transform, true);
            obj.transform.position = transform.forward * (8 * i) + transform.forward;
            latencyNoteList[i - 1] = obj;
        }

        while (true)
        {
            if (Input.anyKey)
            {
                Button("Start");
                break;
            }

            yield return null;
        }
    }

    public void Button(string type)
    {
        switch (type)
        {
            case "Start":
                isStart = true;
                LatencyAudioManager.instance.audioBGM.Play();
                startPanel.SetActive(false);
                bottomPanel.SetActive(true);
                latencyPlayerController.animator.SetBool("isReady", false);
                break;
            
            case "Esc":
                if (!esc.activeSelf)
                {
                    if (topPanel.activeSelf)
                    {
                        topPanel.SetActive(false);
                    }
                    
                    if (bottomPanel.activeSelf)
                    {
                        bottomPanel.SetActive(false);
                    }
                    
                    LatencyAudioManager.instance.audioBGM.Pause();
                    Time.timeScale = 0;
                    esc.SetActive(true);
                    backStack.Push(esc);
                    isPanelOpen = true;
                }
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
                    set.SetActive(true);
                    backStack.Push(set);

                    if (bottomPanel.activeSelf)
                    {
                        bottomPanel.SetActive(false);
                    }
                }
                break;

            case "Restart":
                Time.timeScale = 1;
                SceneManager.LoadScene(DBManager.instance.latencySceneName);
                break;
            
            case "Back":
                Time.timeScale = 1;
                
                if (DBManager.instance != null)
                {
                    DBManager.instance.nextScene = "Lobby";
                }
                
                SceneManager.LoadScene("Loading");
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

        bool isCheck = false;

        switch (backStack.Pop().name)
        {
            case "Esc":
                if (esc.activeSelf)
                {
                    esc.SetActive(false);
                    isCheck = true;
                    
                    if (!topPanel.activeSelf)
                    {
                        topPanel.SetActive(true);
                    }
                    
                    if (!bottomPanel.activeSelf)
                    {
                        bottomPanel.SetActive(true);
                    }

                    Time.timeScale = 1;
                    LatencyAudioManager.instance.audioBGM.Play();
                }
                break;
            
            case "Set":
                if (set.activeSelf)
                {
                    set.SetActive(false);
                    isCheck = true;
                    
                    if (!topPanel.activeSelf)
                    {
                        topPanel.SetActive(true);
                    }
                    
                    if (!bottomPanel.activeSelf)
                    {
                        bottomPanel.SetActive(true);
                    }

                    Time.timeScale = 1;
                    LatencyAudioManager.instance.audioBGM.Play();
                }
                break;
        }

        if (isCheck)
        {
            isPanelOpen = false;
        }
    }
    
    public void Set_Change(string type)
    {
        switch (type)
        {
            case "Music":
                DBManager.instance.musicValue = set_Music_Slider.value;
                LatencyAudioManager.instance.audioMixer.SetFloat("Music", DBManager.instance.musicValue * 80 - 80);
                break;
            
            case "Sfx":
                DBManager.instance.sfxValue = set_Sfx_Slider.value;
                LatencyAudioManager.instance.audioMixer.SetFloat("FX", DBManager.instance.sfxValue * 80 - 80);
                break;
            
            case "Vibration":
                if (set_Vibration_Toggle.isOn)
                {
                    DBManager.instance.isVibration = true;
                }
                else if (!set_Vibration_Toggle.isOn)
                {
                    DBManager.instance.isVibration = false;
                }
                break;
        }
    }

    public void Finish(int count, int latencyAvg)
    {
        isFinish = true;
        bottomPanel.SetActive(false);
        isResultPanelOpen = true;
        result.SetActive(true);
        
        if (count < 10)
        {
            offsetTitleText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Latency", "Latency_Fail", LocalizationSettings.SelectedLocale);
            pressText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Press_Retry", LocalizationSettings.SelectedLocale);
            offsetBottomPanel.SetActive(false);
        }
        else
        {
            offsetTitleText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Latency", "Latency_Success", LocalizationSettings.SelectedLocale);
            pressText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Press_Home", LocalizationSettings.SelectedLocale);
            latencyText.text = latencyAvg + "ms";
        }
        
        StartCoroutine(Finish_Check(latencyAvg, count));
    }
    
    IEnumerator Finish_Check(int latencyAvg, int count)
    {
        yield return new WaitForSeconds(0.5f);
        
        while (true)
        {
            if (Input.anyKey)
            {
                if (count < 10)
                {
                    SceneManager.LoadScene(DBManager.instance.latencySceneName);
                }
                else
                {
                    DBManager.instance.latency = latencyAvg;
                    DBManager.instance.nextScene = DBManager.instance.lobbySceneName;
                    SceneManager.LoadScene("Loading");
                }
            }
            
            yield return null;
        }
    }
}