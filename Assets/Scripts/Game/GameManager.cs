using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("일시정지")] 
    public GameObject esc;
    [FoldoutGroup("패널")] 
    [Title("설정")] 
    public GameObject set;
    [FoldoutGroup("패널")] 
    [Title("메인 패널")] 
    public GameObject mainPanel;
    [FoldoutGroup("패널")] 
    [Title("탑 패널")] 
    public GameObject topPanel;
    [FoldoutGroup("패널")] 
    [Title("카운트다운 패널")] 
    public GameObject countDownPanel;
    [FoldoutGroup("패널")] 
    [Title("판정 이미지")] 
    public Image verdictImage;
    [FoldoutGroup("패널")] 
    [Title("노트 폴더")] 
    public GameObject noteFolder;
    [FoldoutGroup("패널")] 
    [Title("진행률 슬라이더")] 
    public Slider progress;
    [FoldoutGroup("패널")] 
    [Title("판정 프리팹")] 
    public GameObject verdictPrefab;
    [FoldoutGroup("패널")] 
    [Title("일시정지 버튼")] 
    public GameObject stopBtn;
    [FoldoutGroup("패널")] 
    [Title("배경화면")]
    public GameObject backgroundCanvas;
    
    [FoldoutGroup("HP")] 
    [Title("HP 이미지")] 
    public Image hp;
    [FoldoutGroup("HP")] 
    [Title("HP 리스트")]
    public List<Sprite> hpList;
    
    [FoldoutGroup("콤보")] 
    [Title("콤보 텍스트")]
    public Text currentComboText;

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
    [Title("오디오")] 
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
    private Sprite[] verdictImageList = new Sprite[4];
    [FoldoutGroup("정보")] 
    [Title("랭크 점수")] 
    public float rankScore;
    [FoldoutGroup("정보")] 
    [Title("점수")] 
    public int score;
    [FoldoutGroup("정보")] 
    [Title("콤보")]
    public int maxCombo;
    [FoldoutGroup("정보")] 
    [Title("콤보")]
    public int currentCombo;
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
    [FoldoutGroup("정보")] 
    [Title("첫 시작 여부 판단")] 
    public bool isStart = true;
    [FoldoutGroup("정보")] 
    [Title("일시정지 판단")] 
    public bool isKeyOnPause = false;
    
    [FoldoutGroup("결과 창")] 
    [Title("활성화 여부")] 
    public bool isResultPanel;
    [FoldoutGroup("결과 창")] 
    [Title("패널")] 
    public GameObject resultPanel;
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
    [Title("Score 텍스트")] 
    public Text scoreText;
    [FoldoutGroup("결과 창")] 
    [Title("Press 텍스트")] 
    public Text pressText;    
    [FoldoutGroup("결과 창")] 
    [Title("노트 갯수")] 
    [SerializeField]
    private int noteCount = 0;
    
    [FoldoutGroup("트랙 별 DB")] 
    public Stage[] chapter = new Stage[8];
    
    
    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static GameManager instance;
    private WaitForSeconds waitForBeat;

    [Serializable]
    public class Stage
    {
        public GameObject[] stage = new GameObject[6];
        public AudioClip[] audio = new AudioClip[6];
        public Material[] skybox = new Material[6];
        public Sprite[] background = new Sprite[6];
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
        StartCoroutine(nameof(Init));
    }

    IEnumerator Init()
    {
        while (!DBManager.instance.isJsonLoad)
        {
            yield return null;
        }
        
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
        backgroundCanvas.GetComponent<BackGroundMover>().Init(chapter[DBManager.instance.currentChapter - 1].background[DBManager.instance.currentStage -1]);
        audioManager.audio.clip = chapter[DBManager.instance.currentChapter - 1].audio[DBManager.instance.currentStage - 1];
        audioManager.bpm = chapter[DBManager.instance.currentChapter - 1].bpm[DBManager.instance.currentStage - 1];
        waitForBeat = new WaitForSeconds(60f / audioManager.bpm);
        GameObject temp = Instantiate(chapter[DBManager.instance.currentChapter - 1].stage[DBManager.instance.currentStage - 1]);
        noteFolder = temp;
        temp.transform.position = Vector3.zero;
        rankScore = 0;
        maxCombo = 0;
        
        foreach (var item in noteFolder.GetComponentsInChildren<Obstacle>())
        {
            if (item.beatLength == 0)
            {
                noteCount++;
            }
            else
            {
                noteCount += (int)(item.beatLength * 8f);
            }
        }

        VerdictBar.lastObject = noteFolder.transform.GetChild(noteFolder.transform.childCount - 1).gameObject;
        VerdictBar.lastObject = Verdict.GetObstacle(VerdictBar.lastObject).beatLength != 0 ? VerdictBar.lastObject.transform.GetChild(VerdictBar.lastObject.transform.childCount - 1).GetChild(0).gameObject : VerdictBar.lastObject.transform.gameObject;
        playerController.Init();
        StartCoroutine(CountDown());
    }

    public void ShowVerdict(int idx, Obstacle obstacle)
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

        if (idx != 3)
        {
            currentCombo++;
            score += obstacle.scoreList[idx];
        }
        else
        {
            if (maxCombo < currentCombo)
            {
                maxCombo = currentCombo;
            }

            currentCombo = 0;
        }

        if (currentCombo >= 3)
        {
            currentComboText.color = new Color(1, 1, 1, 1);
        }
        else
        {
            currentComboText.color = new Color(1, 1, 1, 0);
        }

        currentComboText.GetComponent<Animation>().Stop();
        currentComboText.GetComponent<Animation>().Play();
        currentComboText.text = currentCombo.ToString();
        verdictImage.sprite = verdictImageList[idx];
        GameObject verdictPrefab = Instantiate(this.verdictPrefab, topPanel.transform, true);
        verdictPrefab.GetComponent<VerdictPrefab>().isUp = playerController.verdict.isUp;
        verdictPrefab.GetComponent<Image>().sprite = verdictImageList[idx];
        verdictPrefab.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        verdictPrefab.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250);
    }

    public void Button(string type)
    {
        switch (type)
        {
            case "Esc":
                if (!esc.activeSelf)
                {
                    if (mainPanel.activeSelf)
                    {
                        mainPanel.SetActive(false);
                    }

                    audioManager.audio.Pause();
                    Time.timeScale = 0;
                    esc.SetActive(true);
                    backStack.Push(esc);
                    isPanelOpen = true;
                    countDownPanel.SetActive(false);
                    isKeyOnPause = true;
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
            
            case "Sfx":
                DBManager.instance.sfxValue = set_Sfx_Slider.value;
                AudioManager.instance.audioMixer.SetFloat("FX", DBManager.instance.sfxValue * 80 - 80);
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
                    
                    if (!mainPanel.activeSelf)
                    {
                        mainPanel.SetActive(true);
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
                    
                    if (!mainPanel.activeSelf)
                    {
                        mainPanel.SetActive(true);
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
        if (stopBtn.activeSelf)
        {
            stopBtn.SetActive(false);
        }
        
        if (audioManager.audio.clip.loadState == AudioDataLoadState.Unloaded)
        {
            audioManager.audio.clip.LoadAudioData();
        }

        isCountDown = true;

        if (!isStart)
        {
            playerController.GetComponent<Animator>().SetBool("isReady", isCountDown);
        }
        
        countDownPanel.gameObject.SetActive(true);
        audioManager.audio.Pause();
        countDownPanel.transform.GetChild(0).GetComponent<Text>().text = "READY";
        audioManager.PlayAudio("Ready");
        
        for (int i = 0; i < 2; i++)
        {
            yield return waitForBeat;
        }
        
        // int j = 3;
        //
        // while (j > 0)
        // {
        //     countDownPanel.transform.GetChild(0).GetComponent<Text>().text = j.ToString();
        //     j--;
        //     yield return waitForBeat;
        // }
        
        countDownPanel.transform.GetChild(0).GetComponent<Text>().text = "GO!!";
        audioManager.PlayAudio("Go");
        yield return waitForBeat;
        audioManager.audio.Play();
        isCountDown = false;
        playerController.GetComponent<Animator>().SetBool("isReady", isCountDown);
        StartCoroutine(audioManager.Progress());
        isKeyOnPause = false;
        isStart = false;
        
        if (!stopBtn.activeSelf)
        {
            stopBtn.SetActive(true);
        }
        
        Invoke(nameof(CountDownDisable), 0.5f);
    }

    void CountDownDisable()
    {
        if (playerController.enabled && !isKeyOnPause)
        {
            countDownPanel.gameObject.SetActive(false);
        }
    }

    public void Finish()
    {
        int currentChapter = DBManager.instance.currentChapter;
        int currentStage = DBManager.instance.currentStage - 1;
        isCountDown = true;
        playerController.GetComponent<Animator>().SetBool("isReady", isCountDown);
        rankScore = (int)((perfectCount + greatCount * 0.3f + goodCount * 0.1f) / noteCount * 100);
        int rankIdx;

        if (maxCombo == 0)
        {
            maxCombo = currentCombo;
        }
        
        if (maxCombo < currentCombo)
        {
            maxCombo = currentCombo;
        }
        
        if (playerController.isDead)
        {
            rankIdx = 0;
        }
        else
        {
            if (rankScore >= 96)
            {
                rankIdx = 4;
            }
            else if (rankScore >= 90 && rankScore <= 95)
            {
                rankIdx = 3;
            }
            else if (rankScore >= 80 && rankScore <= 89)
            {
                rankIdx = 2;
            }
            else if (rankScore >= 70 && rankScore <= 79)
            {
                rankIdx = 1;
            }
            else if (rankScore >= 60 && rankScore <= 69)
            {
                rankIdx = 1;
            }
            else if (rankScore >= 50 && rankScore <= 59)
            {
                rankIdx = 1;
            }
            else
            {
                rankIdx = 1;
            }
        }
        
        DBManager.instance.stageArray[currentChapter].stage[currentStage].starCount = rankIdx;
        perfectText.text = perfectCount.ToString();
        greatText.text = greatCount.ToString();
        goodText.text = goodCount.ToString();
        missText.text = missCount.ToString();
        comboText.text = maxCombo.ToString();
        scoreText.text = score.ToString();
        DBManager.instance.stageArray[currentChapter].stage[currentStage].score = score;
        DBManager.instance.stageArray[currentChapter].stage[currentStage].perfect = perfectCount;
        DBManager.instance.stageArray[currentChapter].stage[currentStage].great = greatCount;
        DBManager.instance.stageArray[currentChapter].stage[currentStage].good = goodCount;
        DBManager.instance.stageArray[currentChapter].stage[currentStage].miss = missCount;

        if (mainPanel.activeSelf)
        {
            mainPanel.SetActive(false);
        }

        resultPanel.GetComponent<Animator>().Play("Result");

        if (playerController.isDead)
        {
            pressText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Press_Retry", LocalizationSettings.SelectedLocale);
        }
        else
        {
            pressText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Press_Home", LocalizationSettings.SelectedLocale);
        }
        
        audioManager.audio.Stop();
        audioManager.PlayAudio("Result");
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
