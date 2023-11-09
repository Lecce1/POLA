using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
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
    [FoldoutGroup("패널")] 
    [Title("노트 폴더")] 
    public GameObject noteFolder;
    [FoldoutGroup("패널")] 
    [Title("카메라 인포")] 
    public GameObject cameraInfo;
    
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
    public int score;
    [FoldoutGroup("정보")] 
    [Title("콤보")]
    public int combo;
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
    
    [FoldoutGroup("결과 창")] 
    [Title("활성화 여부")] 
    public bool isResultPanel;
    [FoldoutGroup("결과 창")] 
    [Title("패널")] 
    public GameObject resultPanel;
    [FoldoutGroup("결과 창")] 
    [Title("성공 실패 텍스트")] 
    public Text clearText;
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
    [Title("Score 텍스트")] 
    public Text scoreText;
    [FoldoutGroup("결과 창")] 
    [Title("Combo 텍스트")] 
    public Text comboText;
    [FoldoutGroup("결과 창")] 
    [Title("Press 텍스트")] 
    public Text pressText;
    
    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static GameManager instance;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(1f);

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
            playerController.GetComponent<PlayerInput>().enabled = true;
        }
    }

    public void Finish()
    {
        isCountDown = true;
        playerController.GetComponent<Animator>().SetBool("isCountDown", isCountDown);

        if (playerController.isDead)
        {
            clearText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Fail", LocalizationSettings.SelectedLocale);
            pressText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Press_Retry", LocalizationSettings.SelectedLocale);
        }
        else
        {
            clearText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Success", LocalizationSettings.SelectedLocale);
            pressText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", "Press_Home", LocalizationSettings.SelectedLocale);
        }
        
        perfectText.text = perfectCount.ToString();
        greatText.text = greatCount.ToString();
        goodText.text = goodCount.ToString();
        missText.text = missCount.ToString();
        scoreText.text = score.ToString();
        comboText.text = combo.ToString();
        
        if (bottomPanel.activeSelf)
        {
            bottomPanel.SetActive(false);
        }
        
        resultPanel.GetComponent<Animation>().Play();
        StartCoroutine(Finish_Check());
    }

    IEnumerator Finish_Check()
    {
        while (true)
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
            
            yield return null;
        }
    }
}
