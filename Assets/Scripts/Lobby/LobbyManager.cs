using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LobbyManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("설정")] 
    public GameObject set;
    [FoldoutGroup("패널")] 
    [Title("상점")] 
    public GameObject shop;
    [FoldoutGroup("패널")] 
    [Title("표지판")] 
    public GameObject sign;
    [FoldoutGroup("패널")] 
    [Title("에러_네트워크")] 
    public GameObject error_Network;
    [FoldoutGroup("패널")] 
    [Title("입장 버튼")] 
    public GameObject join_Btn;
    [FoldoutGroup("패널")] 
    [Title("종료")] 
    public GameObject exit;

    [FoldoutGroup("텍스트")] 
    [Title("입장 버튼")] 
    public Text join_Btn_Text;
    
    [FoldoutGroup("스카이박스")]
    public List<Material> stage_Skybox;
    
    [FoldoutGroup("땅")]
    public List<GameObject> stage_Ground;

    [FoldoutGroup("기타")] 
    [Title("입장 버튼 타입")]
    public string join_Btn_Type;
    [FoldoutGroup("기타")] 
    [Title("입장 버튼 온오프 여부")]
    public bool isJoinBtnOn = false;
    [FoldoutGroup("기타")] 
    [Title("패널 열림 여부")]
    public bool isPanelOpen = false;
    [FoldoutGroup("기타")] 
    [Title("설정 버튼 / 패드 여부")]
    public bool isSetBtn = false;

    [FoldoutGroup("설정 패널")] 
    [Title("언어 Dropdown")]
    public Dropdown set_Language_Dropdown;
    [FoldoutGroup("설정 패널")] 
    [Title("언어 왼쪽 화살표")]
    public GameObject set_Language_LeftArrow;
    [FoldoutGroup("설정 패널")] 
    [Title("언어 오른쪽 화살표")]
    public GameObject set_Language_RightArrow;

    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static LobbyManager instance;
    
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
        LobbyPlayerController.instance.player.transform.position = new Vector3(0, 0.6f, 0);
    }

    public void DoorInit(string name, string btnText)
    {
        join_Btn_Type = name;
        join_Btn.GetComponent<Button>().onClick.RemoveAllListeners();
        join_Btn.GetComponent<Button>().onClick.AddListener(() => LobbyManager.instance.Button(name));
        join_Btn_Text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Lobby", btnText, LocalizationSettings.SelectedLocale);
        Join_Btn_OnOff(true, false);
    }

    public void Join_Btn_OnOff(bool isOn, bool onlyStage)
    {
        if (isOn == true)
        {
            if (!onlyStage && join_Btn.activeSelf)
            {
                join_Btn.GetComponent<Animator>().Play("JoinOn");
            }
            
            isJoinBtnOn = true;
        }
        else if (isOn == false)
        {
            if (!onlyStage && join_Btn.activeSelf)
            {
                join_Btn.GetComponent<Animator>().Play("JoinOff");
            }
            
            isJoinBtnOn = false;
        }
    }

    public void Button(string type)
    {
        switch (type)
        {
            case "Stage":
                if (join_Btn.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    if (DBManager.instance != null)
                    {
                        DBManager.instance.nextScene = "Track";
                    }

                    SceneManager.LoadScene("Loading");
                }
                break;

            case "Set":
                if (!set.activeSelf)
                {
                    if (join_Btn.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                    {
                        set.SetActive(true);
                        backStack.Push(set);
                        isPanelOpen = true;
                        set_Language_Dropdown.value = DBManager.instance.language;
                        
                        if (join_Btn.activeSelf)
                        {
                            join_Btn.SetActive(false);
                        }
                    }
                }
                break;
            
            case "Shop":
                if (!shop.activeSelf)
                {
                    if (join_Btn.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                    {
                        shop.SetActive(true);
                        backStack.Push(shop);
                        isPanelOpen = true;
                    
                        if (join_Btn.activeSelf)
                        {
                            join_Btn.SetActive(false);
                        }
                    }
                }
                break;
            
            case "Sign":
                if (!sign.activeSelf)
                {
                    if (join_Btn.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                    {
                        sign.SetActive(true);
                        backStack.Push(sign);
                        isPanelOpen = true;
                    
                        if (join_Btn.activeSelf)
                        {
                            join_Btn.SetActive(false);
                        }
                    }
                }
                break;
            
            case "Exit":
                if (!exit.activeSelf)
                {
                    exit.SetActive(true);
                    backStack.Push(exit);
                    isPanelOpen = true;
                }
                break;
            
            case "Exit_Yes":
                if (exit.activeSelf)
                {
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

    public void Set_Language()
    {
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
    }

    public void SetBtn()
    {
        isSetBtn = true;
        Button("Set");
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
                }
                break;
            
            case "Exit":
                if (exit.activeSelf)
                {
                    exit.SetActive(false);
                    isCheck = true;
                }
                break;
            
            case "Shop":
                if (shop.activeSelf)
                {
                    shop.SetActive(false);
                    isCheck = true;
                }
                break;
            
            case "Sign":
                if (sign.activeSelf)
                {
                    sign.SetActive(false);
                    isCheck = true;
                }
                break;
        }

        if (isCheck == true)
        {
            isPanelOpen = false;
                    
            if (join_Btn.activeSelf == false)
            {
                join_Btn.SetActive(true);
                
                if (!LobbyPlayerController.instance.isDoor && isSetBtn == false)
                {
                    Join_Btn_OnOff(false, false);
                }

                isSetBtn = false;
            }
        }
    }
}
