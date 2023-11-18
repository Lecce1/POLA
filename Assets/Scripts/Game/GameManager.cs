using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("판정 캔버스")] 
    public GameObject verdictCanvas;
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
    [FoldoutGroup("패널")] 
    [Title("카운트다운 패널")] 
    public GameObject countDownPanel;
    [FoldoutGroup("패널")] 
    [Title("판정 프리팹")] 
    public GameObject verdictPrefab;
    [FoldoutGroup("패널")] 
    [Title("노트 폴더")] 
    public GameObject noteFolder;
    [FoldoutGroup("패널")] 
    [Title("카메라 인포")] 
    public GameObject cameraInfo;
    [FoldoutGroup("패널")] 
    [Title("진행률 슬라이더")] 
    public Slider progress;
    [FoldoutGroup("패널")] 
    [Title("HP 리스트")]
    public List<Image> hpList;

    [FoldoutGroup("설정 패널")] 
    [Title("음악 Slider")]
    public Slider set_Music_Slider;
    [FoldoutGroup("설정 패널")] 
    [Title("효과음 Slider")]
    public Slider set_Sfx_Slider;
    [FoldoutGroup("설정 패널")] 
    [Title("진동 Toggle")]
    public Toggle set_Vibration_Toggle;
    
    [FoldoutGroup("매니저")] 
    [Title("플랫폼")] 
    public PlatformManager platformManager;
    [FoldoutGroup("매니저")] 
    [Title("사운드")] 
    public AudioManager audioManager;
    
    [FoldoutGroup("기타")] 
    [Title("패널 열림 여부")] 
    [SerializeField]
    public bool isPanelOpen;
    [FoldoutGroup("기타")] 
    [Title("카운트 다운 여부")] 
    [SerializeField]
    public bool isCountDown;
    
    [FoldoutGroup("플레이어")] 
    [Title("플레이어")] 
    public PlayerController playerController;

    [FoldoutGroup("정보")] 
    [Title("판정 이미지")] 
    [SerializeField]
    private Sprite[] verdictImage = new Sprite[4];
    [FoldoutGroup("정보")] 
    [Title("스코어")] 
    public float score;
    [FoldoutGroup("정보")] 
    [Title("콤보")]
    public int maxCombo;
    [FoldoutGroup("정보")] 
    [Title("Perfect 갯수")] 
    [SerializeField]
    private int perfectCount;
    [FoldoutGroup("정보")] 
    [Title("Great 갯수")] 
    [SerializeField]
    private int greatCount;
    [FoldoutGroup("정보")] 
    [Title("Good 갯수")] 
    [SerializeField]
    private int goodCount;
    [FoldoutGroup("정보")] 
    [Title("Miss 갯수")] 
    [SerializeField]
    private int missCount;
    
    [FoldoutGroup("결과 창")] 
    [Title("활성화 여부")] 
    public bool isResultPanel;
    [FoldoutGroup("결과 창")] 
    [Title("패널")] 
    public GameObject resultPanel;
    [FoldoutGroup("결과 창")] 
    [Title("랭크 텍스트")] 
    public Text rankText;
    [FoldoutGroup("결과 창")] 
    [Title("Perfect 텍스트")] 
    public Text perfectText;
    [FoldoutGroup("결과 창")] 
    [Title("Great 텍스트")] 
    public Text greatText;
    [FoldoutGroup("결과 창")] 
    [Title("Good 텍스트")] 
    public Text goodText;
    [FoldoutGroup("결과 창")] 
    [Title("Miss 텍스트")] 
    public Text missText;
    [FoldoutGroup("결과 창")] 
    [Title("Combo 텍스트")] 
    public Text comboText;
    [FoldoutGroup("결과 창")] 
    [Title("Press 텍스트")] 
    public Text pressText;
    
    [FoldoutGroup("트랙 별 DB")] 
    public Stage[] chapter = new Stage[8];
    
    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static GameManager instance;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(1f);

    private int noteCount = 0;

    [Serializable]
    public class Stage
    {
        public GameObject[] stage = new GameObject[6];
        public AudioClip[] audio = new AudioClip[6];
        public Material[] skybox = new Material[6];
        public int[] bpm = new int[6];
    }

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
        Init();
    }

    void Init()
    {
        switch (DBManager.instance.currentPlatform)
        {
            case "PC":
                platformManager.SwitchToKeyboard();
                break;
            
            case "CONSOLE":
                platformManager.SwitchToGamepad();
                break;
            
            case "MOBILE":
                platformManager.SwitchToMobile();
                break;
        }

        RenderSettings.skybox = chapter[DBManager.instance.currentChapter - 1].skybox[DBManager.instance.currentStage - 1];
        audioManager.audio.clip = chapter[DBManager.instance.currentChapter - 1].audio[DBManager.instance.currentStage - 1];
        audioManager.bpm = chapter[DBManager.instance.currentChapter - 1].bpm[DBManager.instance.currentStage - 1];
        GameObject temp = Instantiate(chapter[DBManager.instance.currentChapter - 1].stage[DBManager.instance.currentStage - 1]);
        noteFolder = temp;
        temp.transform.position = Vector3.zero;
        score = 0;
        maxCombo = 0;
        
        for (int i = 0; i < noteFolder.transform.childCount; i++)
        {
            if (noteFolder.transform.GetChild(i).childCount == 1)
            {
                noteCount++;
            }
            else if (noteFolder.transform.GetChild(i).childCount > 1)
            {
                noteCount += noteFolder.transform.GetChild(i).childCount;
            }
        }
        
        playerController.Init();

        StartCoroutine(CountDown());
    }

    public void ShowVerdict(int idx)
    {
        switch (idx)
        {
            case 0:
                perfectCount++;
                break;
                
            case 1:
                greatCount++;
                break;
            
            case 2:
                goodCount++;
                break;
            
            case 3:
                missCount++;
                break;
        }
        
        GameObject verdictPrefab = Instantiate(this.verdictPrefab);
        verdictPrefab.GetComponent<Image>().sprite = verdictImage[idx];
        verdictPrefab.transform.SetParent(verdictCanvas.transform);
        verdictPrefab.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        var screenPos = Camera.main.WorldToScreenPoint(cameraInfo.transform.position - new Vector3(0, 1f, 0));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(verdictCanvas.GetComponent<RectTransform>(), screenPos, Camera.main, out var localPos);
        verdictPrefab.transform.localPosition = localPos;
    }

    public void Button(string type)
    {
        switch (type)
        {
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
                    
                    audioManager.audio.Pause();
                    Time.timeScale = 0;
                    esc.SetActive(true);
                    backStack.Push(esc);
                    isPanelOpen = true;
                    countDownPanel.SetActive(false);
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
                SceneManager.LoadScene(DBManager.instance.gameSceneName);
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
    
    public void Set_Change(string type)
    {
        switch (type)
        {
            case "Music":
                DBManager.instance.musicValue = set_Music_Slider.value;
                AudioManager.instance.audioMixer.SetFloat("Music", DBManager.instance.musicValue * 80 - 80);
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
            
            case "Sfx":
                DBManager.instance.sfxValue = set_Sfx_Slider.value;
                AudioManager.instance.audioMixer.SetFloat("FX", DBManager.instance.sfxValue * 80 - 80);
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

                    if (!playerController.isDead)
                    {
                        StartCoroutine(CountDown());
                    }
                    
                    Time.timeScale = 1;
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

                    if (!playerController.isDead)
                    {
                        StartCoroutine(CountDown());
                    }
                    
                    Time.timeScale = 1;
                }
                break;
        }

        if (isCheck)
        {
            isPanelOpen = false;
        }
    }
    
    IEnumerator CountDown()
    {
        if (audioManager.audio.clip.loadState == AudioDataLoadState.Unloaded)
        {
            audioManager.audio.clip.LoadAudioData();
        }

        isCountDown = true;
        playerController.GetComponent<Animator>().SetBool("isCountDown", isCountDown);
        countDownPanel.gameObject.SetActive(true);
        audioManager.audio.Pause();
        playerController.GetComponent<PlayerInput>().enabled = false;

        int i = 4;
        
        while (i > 0)
        {
            if (i == 4)
            {
                countDownPanel.transform.GetChild(0).GetComponent<Text>().text = "Ready";
            }
            else
            {
                countDownPanel.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
            }

            i--;
            yield return waitForSeconds;
        }

        countDownPanel.transform.GetChild(0).GetComponent<Text>().text = "GO!";
        Invoke(nameof(CountDownDisable), 0.5f);
    }

    void CountDownDisable()
    {
        if (playerController.enabled)
        {
            countDownPanel.gameObject.SetActive(false);
            isCountDown = false;
            playerController.GetComponent<Animator>().SetBool("isCountDown", isCountDown);
            audioManager.audio.Play();
            StartCoroutine(audioManager.Progress());
            playerController.GetComponent<PlayerInput>().enabled = true;
        }
    }

    public void Finish()
    {
        isCountDown = true;
        playerController.GetComponent<Animator>().SetBool("isCountDown", isCountDown);
        score = ((perfectCount + greatCount * 0.3f + goodCount * 0.1f + missCount) / noteCount) * 100;

        if (playerController.isDead)
        {
            rankText.text = "F";
        }
        else
        {
            if (score >= 96 && score <= 100)
            {
                rankText.text = "S+";
            }
            else if (score >= 90 && score <= 95)
            {
                rankText.text = "S";
            }
            else if (score >= 80 && score <= 89)
            {
                rankText.text = "A";
            }
            else if (score >= 70 && score <= 79)
            {
                rankText.text = "B";
            }
            else if (score >= 60 && score <= 69)
            {
                rankText.text = "C";
            }
            else if (score >= 50 && score <= 59)
            {
                rankText.text = "D";
            }
            else
            {
                rankText.text = "F";
            }
            
            pressText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Press_Home", LocalizationSettings.SelectedLocale);
        }
        
        perfectText.text = perfectCount.ToString();
        greatText.text = greatCount.ToString();
        goodText.text = goodCount.ToString();
        missText.text = missCount.ToString();
        comboText.text = maxCombo.ToString();
        
        if (bottomPanel.activeSelf)
        {
            bottomPanel.SetActive(false);
        }
        
        resultPanel.SetActive(true);
        resultPanel.GetComponent<Animator>().Play("Result");

        if (playerController.isDead)
        {
            pressText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Press_Retry", LocalizationSettings.SelectedLocale);
        }
        else
        {
            pressText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Press_Home", LocalizationSettings.SelectedLocale);
        }

        StartCoroutine(Finish_Check());
    }

    IEnumerator Finish_Check()
    {
        while (true)
        {
            if (resultPanel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Result") && resultPanel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                if (Input.anyKey)
                {
                    resultPanel.GetComponent<Animator>().speed = 5f;
                }
            }
            else if(resultPanel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Result") && resultPanel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                if (Input.anyKey)
                {
                    if (playerController.isDead)
                    {
                        SceneManager.LoadScene(DBManager.instance.gameSceneName);
                    }
                    else
                    {
                        DBManager.instance.nextScene = "Lobby";
                        SceneManager.LoadScene("Loading");
                    }
                }
            }
            
            yield return null;
        }
    }
}
