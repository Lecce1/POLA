using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("판정 캔버스")] 
    public GameObject verdictCanvas;
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
    
    [FoldoutGroup("텍스트")] 
    [Title("플레이어 상태")] 
    public Text playerStateText;
    
    [FoldoutGroup("매니저")] 
    [Title("플랫폼")] 
    public PlatformManager platformManager;
    [FoldoutGroup("매니저")] 
    [Title("사운드")] 
    public AudioManager audioManager;
    
    [FoldoutGroup("기타")] 
    [Title("패널 열림 여부")] 
    [SerializeField]
    public bool isPanelOpen = false;
    [FoldoutGroup("기타")] 
    [Title("카운트 다운 여부")] 
    [SerializeField]
    public bool isCountDown = false;
    
    [FoldoutGroup("플레이어")] 
    [Title("플레이어")] 
    public PlayerController playerController;

    [FoldoutGroup("정보")] 
    [Title("판정 이미지")] 
    [SerializeField]
    private Sprite[] verdictImage = new Sprite[4];
    [FoldoutGroup("정보")] 
    [Title("스코어")] 
    [SerializeField]
    private int score;
    [FoldoutGroup("정보")] 
    [Title("스코어")] 
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            StatUpdate();
        }
    }
    [FoldoutGroup("정보")] 
    [Title("콤보")]
    [SerializeField]
    private int combo;
    [FoldoutGroup("정보")] 
    [Title("콤보")] 
    public int Combo
    {
        get { return combo; }
        set
        {
            combo = value;
            StatUpdate();
        }
    }
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
    [Title("Good 갯수")] 
    [SerializeField]
    private int missCount;
    
    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static GameManager instance;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(1f);

    public GameObject cameraInfo;

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
        
        StartCoroutine(CountDown());
        Init();
    }

    void Init()
    {
        score = 0;
        combo = 0;
        StatUpdate();
    }

    public void StatUpdate()
    {
        playerStateText.text = "Score: " + score + "\nCombo: " + combo + "\n Health: " + playerController.health;
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
            case "Set":
                if (!set.activeSelf)
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
                    set.SetActive(true);
                    backStack.Push(set);
                    isPanelOpen = true;
                    countDownPanel.SetActive(false);
                }
                break;
            
            case "Stage":
                Time.timeScale = 1;
                SceneManager.LoadScene(DBManager.instance.gameSceneName);
                break;
            
            case "Back":
                Time.timeScale = 1;
                
                if (DBManager.instance != null)
                {
                    DBManager.instance.nextScene = "Track";
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
        isCountDown = true;
        playerController.GetComponent<Animator>().SetBool("isCountDown", isCountDown);
        countDownPanel.gameObject.SetActive(true);
        audioManager.audio.Pause();
        playerController.GetComponent<PlayerInput>().enabled = false;

        int i = 3;
        
        while (i > 0)
        {
            countDownPanel.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
            i--;
            yield return waitForSeconds;
        }

        countDownPanel.transform.GetChild(0).GetComponent<Text>().text = "GO!";
        isCountDown = false;
        playerController.GetComponent<Animator>().SetBool("isCountDown", isCountDown);
        audioManager.audio.Play();
        playerController.GetComponent<PlayerInput>().enabled = true;
        Invoke(nameof(CountDownDisable), 0.5f);
    }

    private void CountDownDisable()
    {
        if (playerController.enabled)
        {
            countDownPanel.gameObject.SetActive(false);
        }
    }
}
